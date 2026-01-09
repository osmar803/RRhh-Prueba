using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmColaboradores : Form
{
    private readonly ColaboradorService _service;
    private readonly EmpresaService _empresaService;
    
    private Guid? _idSeleccionado = null;
    private ErrorProvider _errorProvider; 
    private Button btnGuardar; 

    private ComboBox cmbEmpresas;
    private TextBox txtNombre, txtEdad, txtTel, txtEmail;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmColaboradores(ColaboradorService service, EmpresaService empresaService)
    {
        _service = service;
        _empresaService = empresaService;
        
        _errorProvider = new ErrorProvider();
        _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

        ConfigurarUI();

        // Carga inicial protegida contra cancelaciones de Scope
        this.Load += async (s, e) => {
            try 
            {
                await CargarCombos();
                await CargarDatos();
            }
            catch (OperationCanceledException) { /* Ignorar al navegar rápido */ }
            catch (Exception ex) { MessageBox.Show("Error al cargar: " + ex.Message); }
        };
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Colaboradores";
        this.Size = new Size(1000, 600);
        this.BackColor = ThemeHelper.BackgroundColor;
        this.AutoValidate = AutoValidate.EnableAllowFocusChange;

        // --- 1. Panel Ayuda ---
        var panelAyuda = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = ThemeHelper.InfoBackgroundColor, Padding = new Padding(10) };
        var lblInstruccion = new Label {
            Text = "ℹ️ Para actualizar: Seleccione un colaborador, edite sus datos personales o empresa asignada y guarde cambios.",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
        };
        ThemeHelper.EstilizarLabelAyuda(lblInstruccion);
        panelAyuda.Controls.Add(lblInstruccion);
        this.Controls.Add(panelAyuda);

        // --- 2. Panel Izquierdo (Formulario) ---
        panelFormulario = new Panel { Dock = DockStyle.Left, Width = 320, BackColor = Color.White, Padding = new Padding(20) };
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "Colaborador", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        CrearCampoInput("Empresa:", out cmbEmpresas);
        CrearCampoInput("Nombre Completo:", out txtNombre);
        CrearCampoInput("Edad:", out txtEdad);
        CrearCampoInput("Teléfono:", out txtTel);
        CrearCampoInput("Email:", out txtEmail);

        ConfigurarValidaciones();

        btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s, e) => Limpiar();

        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnCancelar);
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar Seleccionado", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // --- 3. Grilla ---
        dgvDatos = new DataGridView { Dock = DockStyle.Fill };
        ThemeHelper.EstilizarGrid(dgvDatos);
        dgvDatos.Click += (s, e) => SeleccionarFila();
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();
    }

    private void CrearCampoInput(string labelText, out TextBox textBox)
    {
        var panelItem = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0, 5, 20, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

        panelItem.Controls.Add(textBox);
        panelItem.Controls.Add(lbl);
        lbl.BringToFront(); 
        
        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront();
    }

    private void CrearCampoInput(string labelText, out ComboBox comboBox)
    {
        var panelItem = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0, 5, 20, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        comboBox = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        
        panelItem.Controls.Add(comboBox);
        panelItem.Controls.Add(lbl);
        lbl.BringToFront(); 

        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront();
    }

    private void ConfigurarValidaciones()
    {
        txtNombre.Validating += (s, e) => ValidarRequerido(txtNombre, "El nombre es obligatorio.");
        txtNombre.TextChanged += (s, e) => _errorProvider.SetError(txtNombre, ""); 
        txtEdad.Validating += (s, e) => ValidarEdad();
        txtEdad.TextChanged += (s, e) => _errorProvider.SetError(txtEdad, "");
        txtEmail.Validating += (s, e) => ValidarEmail();
        txtEmail.TextChanged += (s, e) => _errorProvider.SetError(txtEmail, "");
        cmbEmpresas.Validating += (s, e) => ValidarCombo(cmbEmpresas, "Seleccione una empresa.");
        cmbEmpresas.SelectedIndexChanged += (s, e) => _errorProvider.SetError(cmbEmpresas, "");
    }

    private bool ValidarRequerido(Control ctrl, string msj) {
        if (string.IsNullOrWhiteSpace(ctrl.Text)) { _errorProvider.SetError(ctrl, msj); return false; }
        _errorProvider.SetError(ctrl, ""); return true;
    }

    private bool ValidarCombo(ComboBox cmb, string msj) {
        if (cmb.SelectedIndex == -1 && cmb.SelectedValue == null) { _errorProvider.SetError(cmb, msj); return false; }
        _errorProvider.SetError(cmb, ""); return true;
    }

    private bool ValidarEdad() {
        if (!int.TryParse(txtEdad.Text, out int edad) || edad < 18) { _errorProvider.SetError(txtEdad, "Número válido (18+)."); return false; }
        _errorProvider.SetError(txtEdad, ""); return true;
    }

    private bool ValidarEmail() {
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!regex.IsMatch(txtEmail.Text)) { _errorProvider.SetError(txtEmail, "Email inválido."); return false; }
        _errorProvider.SetError(txtEmail, ""); return true;
    }

    private bool EsFormularioValido() {
        return ValidarRequerido(txtNombre, "Nombre obligatorio.") && ValidarEdad() && ValidarEmail() && 
               (_idSeleccionado != null || ValidarCombo(cmbEmpresas, "Seleccione empresa."));
    }

    private async Task Guardar()
    {
        if (!EsFormularioValido()) return;
        btnGuardar.Enabled = false; 
        btnGuardar.Text = "Guardando...";
        try {
            int edad = int.Parse(txtEdad.Text);
            if (_idSeleccionado == null) {
                var id = await _service.CrearAsync(new ColaboradorCreateDto(txtNombre.Text, edad, txtTel.Text, txtEmail.Text));
                if (cmbEmpresas.SelectedValue != null) await _service.AsignarEmpresaAsync(id, (Guid)cmbEmpresas.SelectedValue);
                MessageBox.Show("Creado");
            } else {
                await _service.ActualizarAsync(new ColaboradorUpdateDto(_idSeleccionado.Value, txtNombre.Text, edad, txtTel.Text, txtEmail.Text));
                MessageBox.Show("Actualizado");
            }
            Limpiar(); await CargarDatos();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
        finally { btnGuardar.Enabled = true; btnGuardar.Text = "Guardar"; }
    }

    private async Task CargarCombos() {
        try {
            var empresas = await _empresaService.ObtenerTodosAsync();
            cmbEmpresas.DataSource = empresas; cmbEmpresas.DisplayMember = "NombreComercial"; cmbEmpresas.ValueMember = "Id"; cmbEmpresas.SelectedIndex = -1;
        } catch (OperationCanceledException) { }
    }

    private async Task CargarDatos() {
        try {
            dgvDatos.DataSource = await _service.ObtenerTodosAsync();
        } catch (OperationCanceledException) { }
    }

    private void SeleccionarFila() {
        if (dgvDatos.SelectedRows.Count > 0) {
            var row = dgvDatos.SelectedRows[0];
            _idSeleccionado = (Guid)row.Cells["Id"].Value;
            txtNombre.Text = row.Cells["NombreCompleto"].Value?.ToString() ?? "";
            txtEdad.Text = row.Cells["Edad"].Value?.ToString() ?? "";
            txtTel.Text = row.Cells["Telefono"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["CorreoElectronico"].Value?.ToString() ?? "";
            _errorProvider.Clear();
        }
    }

    private async Task Eliminar() {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Eliminar?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) {
             try {
                await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
                await CargarDatos(); Limpiar();
             } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }

    private void Limpiar() { 
        txtNombre.Clear(); txtEdad.Clear(); txtTel.Clear(); txtEmail.Clear(); 
        cmbEmpresas.SelectedIndex = -1; _idSeleccionado = null; 
        dgvDatos.ClearSelection(); _errorProvider.Clear();
    }
}