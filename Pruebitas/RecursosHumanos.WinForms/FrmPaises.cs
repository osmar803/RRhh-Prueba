using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmPaises : Form
{
    private readonly PaisService _service;
    
    private TextBox txtNombre;
    private DataGridView dgvDatos;
    private Guid? _idSeleccionado = null;
    private Panel panelFormulario;

    public FrmPaises(PaisService service)
    {
        _service = service;
        ConfigurarUI();
        CargarDatos();
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Países";
        this.Size = new Size(800, 450);
        this.BackColor = ThemeHelper.BackgroundColor;

        // Panel Izquierdo
        panelFormulario = new Panel { Dock = DockStyle.Left, Width = 280, BackColor = Color.White, Padding = new Padding(20) };
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "País", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        // Input
        CrearCampoInput("Nombre del País:", out txtNombre);

        // Botones
        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s,e) => Limpiar(); // Botón extra para salir del modo edición

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
        // Evento clave para edición:
        dgvDatos.Click += (s, e) => SeleccionarFila(); 
        
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();
    }

    private void CrearCampoInput(string labelText, out TextBox textBox) {
        var p = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(textBox); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }

    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private async Task Guardar()
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text)) return;
        try {
            if (_idSeleccionado == null)
                await _service.CrearAsync(new PaisCreateDto(txtNombre.Text));
            else
                await _service.ActualizarAsync(new PaisUpdateDto(_idSeleccionado.Value, txtNombre.Text));
            
            Limpiar();
            await CargarDatos();
            MessageBox.Show("Guardado exitoso");
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Eliminar?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes) {
            var id = (Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value;
            await _service.EliminarAsync(id);
            Limpiar();
            await CargarDatos();
        }
    }

    private void SeleccionarFila()
    {
        if (dgvDatos.SelectedRows.Count > 0) {
            _idSeleccionado = (Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value;
            txtNombre.Text = dgvDatos.SelectedRows[0].Cells["Nombre"].Value.ToString();
        }
    }

    private void Limpiar() { txtNombre.Text = ""; _idSeleccionado = null; }
}