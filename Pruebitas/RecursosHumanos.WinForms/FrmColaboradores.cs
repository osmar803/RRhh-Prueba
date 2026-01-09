using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers; // Usar el Helper

namespace RecursosHumanos.WinForms;

public class FrmColaboradores : Form
{
    private readonly ColaboradorService _service;
    private readonly EmpresaService _empresaService;
    
    // Controles
    private ComboBox cmbEmpresas;
    private TextBox txtNombre, txtEdad, txtTel, txtEmail;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmColaboradores(ColaboradorService service, EmpresaService empresaService)
    {
        _service = service;
        _empresaService = empresaService;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Colaboradores";
        this.Size = new Size(1000, 600);
        this.BackColor = ThemeHelper.BackgroundColor;

        // 1. Panel Izquierdo (Inputs)
        panelFormulario = new Panel();
        panelFormulario.Dock = DockStyle.Left;
        panelFormulario.Width = 300;
        panelFormulario.BackColor = Color.White;
        panelFormulario.Padding = new Padding(20);
        this.Controls.Add(panelFormulario);

        // Título del Panel
        var lblTitulo = new Label { Text = "Nuevo Colaborador", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        // Agregamos campos de arriba hacia abajo usando un FlowLayoutPanel o Docking simple
        // Para simplificar, usaré un método auxiliar que apila en el panelFormulario
        CrearCampoInput("Empresa:", out cmbEmpresas);
        CrearCampoInput("Nombre Completo:", out txtNombre);
        CrearCampoInput("Edad:", out txtEdad);
        CrearCampoInput("Teléfono:", out txtTel);
        CrearCampoInput("Email:", out txtEmail);

        // Botones
        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();
        
        // Espaciador
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top }); 
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar Seleccionado", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar); // Se va al fondo del panel izquierdo

        // 2. Grilla (Resto del espacio)
        dgvDatos = new DataGridView();
        dgvDatos.Dock = DockStyle.Fill;
        ThemeHelper.EstilizarGrid(dgvDatos);
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront(); // Asegurar que no quede tapado

        // Cargar Combos y Datos
        await CargarCombos();
        await CargarDatos();
    }

    // Método auxiliar para crear labels y textboxes apilados
    private void CrearCampoInput(string labelText, out TextBox textBox)
    {
        var panelItem = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 5) };
        
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10) };

        panelItem.Controls.Add(textBox); // Primero el textbox (Dock Top invierte orden visual al agregar)
        panelItem.Controls.Add(lbl);
        
        // Truco: Para que quede Label Arriba, Textbox Abajo con DockStyle.Top, 
        // debemos agregarlos al panel contenedor en orden inverso (primero el de abajo)
        // O usar BringToFront().
        lbl.BringToFront(); 

        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront(); // Para que se apilen en orden correcto hacia abajo
    }

    // Sobrecarga para ComboBox
    private void CrearCampoInput(string labelText, out ComboBox comboBox)
    {
        var panelItem = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        comboBox = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        
        panelItem.Controls.Add(comboBox);
        panelItem.Controls.Add(lbl);
        lbl.BringToFront();

        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront();
    }

    private async Task CargarCombos()
    {
        var empresas = await _empresaService.ObtenerTodosAsync();
        cmbEmpresas.DataSource = empresas;
        cmbEmpresas.DisplayMember = "NombreComercial";
        cmbEmpresas.ValueMember = "Id";
    }

    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    // ... (Métodos Guardar y Eliminar se mantienen igual en lógica) ...
    private async Task Guardar()
    {
        if (cmbEmpresas.SelectedValue == null || !int.TryParse(txtEdad.Text, out int edad)) { MessageBox.Show("Verifique datos"); return; }
        try {
            var id = await _service.CrearAsync(new ColaboradorCreateDto(txtNombre.Text, edad, txtTel.Text, txtEmail.Text));
            await _service.AsignarEmpresaAsync(id, (Guid)cmbEmpresas.SelectedValue);
            await CargarDatos();
            MessageBox.Show("Colaborador Guardado");
            Limpiar();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Seguro?", "Eliminar", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
             await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
             await CargarDatos();
        }
    }
    
    private void Limpiar() { txtNombre.Clear(); txtEdad.Clear(); txtTel.Clear(); txtEmail.Clear(); }
}