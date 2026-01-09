using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmMunicipios : Form
{
    private readonly MunicipioService _service;
    private readonly DepartamentoService _deptoService;
    
    private ComboBox cmbDeptos;
    private TextBox txtNombre;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmMunicipios(MunicipioService service, DepartamentoService deptoService)
    {
        _service = service;
        _deptoService = deptoService;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Municipios";
        this.Size = new Size(900, 550);
        this.BackColor = ThemeHelper.BackgroundColor;

        // Panel Izquierdo
        panelFormulario = new Panel();
        panelFormulario.Dock = DockStyle.Left;
        panelFormulario.Width = 300;
        panelFormulario.BackColor = Color.White;
        panelFormulario.Padding = new Padding(20);
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "Nuevo Municipio", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        // Inputs
        CrearCampoInput("Nombre Municipio:", out txtNombre);
        CrearCampoInput("Departamento:", out cmbDeptos);

        // Botones
        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();
        
        panelFormulario.Controls.Add(new Panel { Height = 20, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // Grilla
        dgvDatos = new DataGridView();
        dgvDatos.Dock = DockStyle.Fill;
        ThemeHelper.EstilizarGrid(dgvDatos);
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();

        await CargarCombos();
        await CargarDatos();
    }

    // Helpers para crear campos (puedes copiarlos de FrmEmpresas o hacerlos métodos estáticos en ThemeHelper si prefieres)
    private void CrearCampoInput(string labelText, out TextBox textBox) {
        var p = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(textBox); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }
    private void CrearCampoInput(string labelText, out ComboBox comboBox) {
        var p = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        comboBox = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        p.Controls.Add(comboBox); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }

    private async Task CargarCombos() {
        cmbDeptos.DataSource = await _deptoService.ObtenerTodosAsync();
        cmbDeptos.DisplayMember = "Nombre"; cmbDeptos.ValueMember = "Id";
    }
    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private async Task Guardar() {
        if (cmbDeptos.SelectedValue == null || string.IsNullOrWhiteSpace(txtNombre.Text)) return;
        await _service.CrearAsync(new MunicipioCreateDto(txtNombre.Text, (Guid)cmbDeptos.SelectedValue));
        txtNombre.Clear();
        await CargarDatos();
        MessageBox.Show("Guardado");
    }

    private async Task Eliminar() {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Eliminar?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes){
            await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
            await CargarDatos();
        }
    }
}