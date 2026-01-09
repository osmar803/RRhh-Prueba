using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmEmpresas : Form
{
    private readonly EmpresaService _service;
    private readonly MunicipioService _muniService;
    private Guid? _idSeleccionado = null;

    private ComboBox cmbMunicipios;
    private TextBox txtNit, txtRazon, txtComercial, txtTel, txtEmail;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmEmpresas(EmpresaService service, MunicipioService muniService)
    {
        _service = service;
        _muniService = muniService;
        ConfigurarUI();

        // Manejo de carga inicial segura contra cancelaciones de tareas (Scope Dispose)
        this.Load += async (s, e) => {
            try 
            {
                await CargarCombos();
                await CargarDatos();
            }
            catch (OperationCanceledException) { /* Ignorar al cerrar ventana rápido */ }
            catch (Exception ex) { MessageBox.Show("Error al cargar datos: " + ex.Message); }
        };
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Empresas";
        this.Size = new Size(1100, 700);
        this.BackColor = ThemeHelper.BackgroundColor;

        // --- Panel Ayuda ---
        var panelAyuda = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = ThemeHelper.InfoBackgroundColor, Padding = new Padding(10) };
        var lblInstruccion = new Label {
            Text = "ℹ️ Para actualizar: Seleccione una empresa, modifique sus datos (NIT, Razón Social, etc.) y guarde.",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
        };
        ThemeHelper.EstilizarLabelAyuda(lblInstruccion);
        panelAyuda.Controls.Add(lblInstruccion);
        this.Controls.Add(panelAyuda);

        // --- Panel Izquierdo ---
        panelFormulario = new Panel { Dock = DockStyle.Left, Width = 350, BackColor = Color.White, Padding = new Padding(20), AutoScroll = true };
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "Empresa", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        CrearCampoInput("Municipio:", out cmbMunicipios);
        CrearCampoInput("Email:", out txtEmail);
        CrearCampoInput("Teléfono:", out txtTel);
        CrearCampoInput("Nombre Comercial:", out txtComercial);
        CrearCampoInput("Razón Social:", out txtRazon);
        CrearCampoInput("NIT:", out txtNit);

        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s, e) => Limpiar();

        panelFormulario.Controls.Add(new Panel { Height = 20, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnCancelar);
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        dgvDatos = new DataGridView { Dock = DockStyle.Fill };
        ThemeHelper.EstilizarGrid(dgvDatos);
        dgvDatos.Click += (s, e) => SeleccionarFila();
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();
    }

    private void CrearCampoInput(string label, out TextBox txt) { 
        var p = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = label, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray };
        txt = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(txt); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }
    private void CrearCampoInput(string label, out ComboBox cmb) { 
        var p = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = label, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray };
        cmb = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        p.Controls.Add(cmb); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }

    private async Task CargarCombos() {
        try {
            var lista = await _muniService.ObtenerTodosAsync();
            cmbMunicipios.DataSource = lista; 
            cmbMunicipios.DisplayMember = "Nombre"; 
            cmbMunicipios.ValueMember = "Id";
        } catch (OperationCanceledException) { }
    }

    private async Task CargarDatos() {
        try {
            dgvDatos.DataSource = await _service.ObtenerTodosAsync();
        } catch (OperationCanceledException) { }
    }

    private async Task Guardar() {
        if (cmbMunicipios.SelectedValue == null) return;
        try {
            if (_idSeleccionado == null) {
                var dto = new EmpresaCreateDto(txtNit.Text, txtRazon.Text, txtComercial.Text, txtTel.Text, txtEmail.Text, (Guid)cmbMunicipios.SelectedValue);
                await _service.CrearAsync(dto);
                MessageBox.Show("Guardado con éxito");
            } else {
                var dto = new EmpresaUpdateDto(_idSeleccionado.Value, txtNit.Text, txtRazon.Text, txtComercial.Text, txtTel.Text, txtEmail.Text, (Guid)cmbMunicipios.SelectedValue);
                await _service.ActualizarAsync(dto);
                MessageBox.Show("Actualizado con éxito");
            }
            Limpiar(); await CargarDatos();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void SeleccionarFila() {
        if (dgvDatos.SelectedRows.Count > 0) {
            var row = dgvDatos.SelectedRows[0];
            _idSeleccionado = (Guid)row.Cells["Id"].Value;
            
            txtNit.Text = row.Cells["Nit"].Value?.ToString() ?? "";
            txtRazon.Text = row.Cells["RazonSocial"].Value?.ToString() ?? "";
            txtComercial.Text = row.Cells["NombreComercial"].Value?.ToString() ?? "";
            txtTel.Text = row.Cells["Telefono"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["CorreoElectronico"].Value?.ToString() ?? "";
            if(row.Cells["MunicipioId"].Value != null) cmbMunicipios.SelectedValue = row.Cells["MunicipioId"].Value;
        }
    }

    private async Task Eliminar() {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Desea eliminar esta empresa?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) {
            try {
                await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
                Limpiar(); await CargarDatos();
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }

    private void Limpiar() { 
        txtNit.Clear(); txtRazon.Clear(); txtComercial.Clear(); txtTel.Clear(); txtEmail.Clear(); 
        cmbMunicipios.SelectedIndex = -1; _idSeleccionado = null; 
        dgvDatos.ClearSelection();
    }
}