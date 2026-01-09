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
        
        // Manejo de carga inicial con protección contra cancelaciones de tareas
        this.Load += async (s, e) => {
            try 
            {
                await CargarDatos();
            }
            catch (OperationCanceledException) { /* Ignorar al cerrar el scope rápidamente */ }
            catch (Exception ex) { MessageBox.Show("Error al cargar: " + ex.Message); }
        };
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Países";
        this.Size = new Size(850, 500); 
        this.BackColor = ThemeHelper.BackgroundColor;

        // --- 1. Panel de Ayuda / Instrucciones ---
        var panelAyuda = new Panel { 
            Dock = DockStyle.Top, 
            Height = 50, 
            BackColor = ThemeHelper.InfoBackgroundColor, 
            Padding = new Padding(10) 
        };
        
        var lblInstruccion = new Label {
            Text = "ℹ️ Para actualizar: Seleccione un país de la lista, modifique el nombre en el panel izquierdo y guarde.",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        ThemeHelper.EstilizarLabelAyuda(lblInstruccion);
        
        panelAyuda.Controls.Add(lblInstruccion);
        this.Controls.Add(panelAyuda);


        // --- 2. Panel Izquierdo (Formulario) ---
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
        btnCancelar.Click += (s,e) => Limpiar(); 

        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnCancelar); 
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // --- 3. Grilla de Datos ---
        dgvDatos = new DataGridView { Dock = DockStyle.Fill };
        ThemeHelper.EstilizarGrid(dgvDatos);
        dgvDatos.Click += (s, e) => SeleccionarFila(); 
        
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront(); 
    }

    private void CrearCampoInput(string labelText, out TextBox textBox) {
        var p = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        
        p.Controls.Add(textBox); 
        p.Controls.Add(l); 
        l.BringToFront(); 
        
        panelFormulario.Controls.Add(p); 
        p.BringToFront();
    }

    private async Task CargarDatos() {
        try {
            dgvDatos.DataSource = await _service.ObtenerTodosAsync();
        } catch (OperationCanceledException) { }
    }

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
            txtNombre.Text = dgvDatos.SelectedRows[0].Cells["Nombre"]?.Value?.ToString() ?? "";
        }
    }

    private void Limpiar() { txtNombre.Text = ""; _idSeleccionado = null; dgvDatos.ClearSelection(); }
}