using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;

namespace RecursosHumanos.WinForms;

public class FrmPaises : Form
{
    private readonly PaisService _service;
    
    // Controles
    private TextBox txtNombre;
    private DataGridView dgvDatos;
    private Guid? _idSeleccionado = null;

    public FrmPaises(PaisService service)
    {
        _service = service;
        ConfigurarUI(); // Dibujamos la pantalla
        CargarDatos();  // Cargamos la lista
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Países";
        this.Size = new Size(500, 400);

        var lbl = new Label { Text = "Nombre del País:", Location = new Point(20, 20), AutoSize = true };
        txtNombre = new TextBox { Location = new Point(20, 45), Width = 200 };
        
        var btnGuardar = new Button { Text = "Guardar", Location = new Point(230, 43), Width = 80 };
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnEliminar = new Button { Text = "Eliminar", Location = new Point(320, 43), Width = 80 };
        btnEliminar.Click += async (s, e) => await Eliminar();

        dgvDatos = new DataGridView { Location = new Point(20, 90), Size = new Size(440, 250), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
        dgvDatos.Click += (s, e) => SeleccionarFila();

        this.Controls.Add(lbl);
        this.Controls.Add(txtNombre);
        this.Controls.Add(btnGuardar);
        this.Controls.Add(btnEliminar);
        this.Controls.Add(dgvDatos);
    }

    private async Task CargarDatos()
    {
        dgvDatos.DataSource = await _service.ObtenerTodosAsync();
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
        var id = (Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value;
        await _service.EliminarAsync(id);
        Limpiar();
        await CargarDatos();
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