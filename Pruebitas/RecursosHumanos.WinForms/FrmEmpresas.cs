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
    
    // Controles
    private ComboBox cmbMunicipios;
    private TextBox txtNit, txtRazon, txtComercial, txtTel, txtEmail;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmEmpresas(EmpresaService service, MunicipioService muniService)
    {
        _service = service;
        _muniService = muniService;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        // Configuración Ventana
        this.Text = "Gestión de Empresas";
        this.Size = new Size(1100, 700);
        this.BackColor = ThemeHelper.BackgroundColor;

        // --- 1. Panel Izquierdo (Formulario) ---
        panelFormulario = new Panel();
        panelFormulario.Dock = DockStyle.Left;
        panelFormulario.Width = 350; // Un poco más ancho por los nombres largos
        panelFormulario.BackColor = Color.White;
        panelFormulario.Padding = new Padding(20);
        panelFormulario.AutoScroll = true; // Permite scroll si hay muchos campos
        this.Controls.Add(panelFormulario);

        // Título
        var lblTitulo = new Label { Text = "Nueva Empresa", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        // Campos (Orden inverso para Dock=Top)
        // Nota: Los agregamos usando un método auxiliar para limpiar el código
        CrearCampoInput("Municipio:", out cmbMunicipios);
        CrearCampoInput("Email:", out txtEmail);
        CrearCampoInput("Teléfono:", out txtTel);
        CrearCampoInput("Nombre Comercial:", out txtComercial);
        CrearCampoInput("Razón Social:", out txtRazon);
        CrearCampoInput("NIT:", out txtNit);

        // Botones
        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        panelFormulario.Controls.Add(new Panel { Height = 20, Dock = DockStyle.Top }); // Espacio
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // --- 2. Grilla (Derecha) ---
        dgvDatos = new DataGridView();
        dgvDatos.Dock = DockStyle.Fill;
        ThemeHelper.EstilizarGrid(dgvDatos);
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();

        // Carga inicial
        await CargarCombos();
        await CargarDatos();
    }

    private void CrearCampoInput(string labelText, out TextBox textBox)
    {
        var panelItem = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        
        panelItem.Controls.Add(textBox);
        panelItem.Controls.Add(lbl);
        lbl.BringToFront(); // Label arriba

        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront(); // Mantiene el orden visual correcto
    }

    private void CrearCampoInput(string labelText, out ComboBox comboBox)
    {
        var panelItem = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.Gray };
        comboBox = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        
        panelItem.Controls.Add(comboBox);
        panelItem.Controls.Add(lbl);
        lbl.BringToFront();

        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront();
    }

    private async Task CargarCombos()
    {
        var lista = await _muniService.ObtenerTodosAsync();
        cmbMunicipios.DataSource = lista;
        cmbMunicipios.DisplayMember = "Nombre";
        cmbMunicipios.ValueMember = "Id";
    }

    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private async Task Guardar()
    {
        if (cmbMunicipios.SelectedValue == null) return;
        try {
            var dto = new EmpresaCreateDto(txtNit.Text, txtRazon.Text, txtComercial.Text, txtTel.Text, txtEmail.Text, (Guid)cmbMunicipios.SelectedValue);
            await _service.CrearAsync(dto);
            await CargarDatos();
            MessageBox.Show("Empresa Guardada");
            Limpiar();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Eliminar empresa?", "Confirmar", MessageBoxButtons.YesNo) == DialogResult.Yes) {
            await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
            await CargarDatos();
        }
    }

    private void Limpiar() { txtNit.Clear(); txtRazon.Clear(); txtComercial.Clear(); txtTel.Clear(); txtEmail.Clear(); }
}