using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmDepartamentos : Form
{
    private readonly DepartamentoService _service;
    private readonly PaisService _paisService;
    
    // Estado
    private Guid? _idSeleccionado = null;

    // Controles
    private ComboBox cmbPaises;
    private TextBox txtNombre;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmDepartamentos(DepartamentoService service, PaisService paisService)
    {
        _service = service;
        _paisService = paisService;
        ConfigurarUI();
        this.Load += async (s,e) => await CargarDatosIniciales();
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Departamentos";
        this.Size = new Size(950, 550);
        this.BackColor = ThemeHelper.BackgroundColor;

        // Panel Izquierdo
        panelFormulario = new Panel { Dock = DockStyle.Left, Width = 300, BackColor = Color.White, Padding = new Padding(20) };
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "Departamento", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        CrearCampoInput("Nombre Depto:", out txtNombre);
        CrearCampoInput("País:", out cmbPaises);

        // Botones
        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s, e) => Limpiar(); // Resetear edición

        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnCancelar);
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // Grilla
        dgvDatos = new DataGridView { Dock = DockStyle.Fill };
        ThemeHelper.EstilizarGrid(dgvDatos);
        dgvDatos.Click += (s, e) => SeleccionarFila(); // <--- EVENTO IMPORTANTE
        
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();
    }

    private void CrearCampoInput(string label, out TextBox txt) { /* Igual que antes */
        var p = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = label, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        txt = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(txt); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }
    private void CrearCampoInput(string label, out ComboBox cmb) { /* Igual que antes */
        var p = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = label, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        cmb = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        p.Controls.Add(cmb); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }

    private async Task CargarDatosIniciales() {
        var paises = await _paisService.ObtenerTodosAsync();
        cmbPaises.DataSource = paises; cmbPaises.DisplayMember = "Nombre"; cmbPaises.ValueMember = "Id";
        await RecargarGrilla();
    }
    private async Task RecargarGrilla() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private async Task Guardar() {
        if (cmbPaises.SelectedValue == null || string.IsNullOrWhiteSpace(txtNombre.Text)) return;
        try {
            if (_idSeleccionado == null)
            {
                // MODO CREAR
                await _service.CrearAsync(new DepartamentoCreateDto(txtNombre.Text, (Guid)cmbPaises.SelectedValue));
                MessageBox.Show("Creado correctamente");
            }
            else
            {
                // MODO ACTUALIZAR
                var dto = new DepartamentoUpdateDto(_idSeleccionado.Value, txtNombre.Text, (Guid)cmbPaises.SelectedValue);
                await _service.ActualizarAsync(dto);
                MessageBox.Show("Actualizado correctamente");
            }
            Limpiar();
            await RecargarGrilla();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void SeleccionarFila() {
        if (dgvDatos.SelectedRows.Count > 0) {
            var row = dgvDatos.SelectedRows[0];
            _idSeleccionado = (Guid)row.Cells["Id"].Value;
            txtNombre.Text = row.Cells["Nombre"].Value.ToString();
            
            // Seleccionar el combo (asumiendo que traes PaisId en el DTO, si no, traes PaisNombre)
            if(row.Cells["PaisId"].Value != null) 
                cmbPaises.SelectedValue = row.Cells["PaisId"].Value;
        }
    }

    private async Task Eliminar() {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Eliminar?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes){
            await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
            Limpiar();
            await RecargarGrilla();
        }
    }

    private void Limpiar() {
        txtNombre.Text = "";
        cmbPaises.SelectedIndex = -1;
        _idSeleccionado = null;
        dgvDatos.ClearSelection();
    }
}