using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;

namespace RecursosHumanos.WinForms;

public class FrmDepartamentos : Form
{
    private readonly DepartamentoService _service;
    private readonly PaisService _paisService;
    
    private ComboBox cmbPaises;
    private TextBox txtNombre;
    private DataGridView dgvDatos;

    public FrmDepartamentos(DepartamentoService service, PaisService paisService)
    {
        _service = service;
        _paisService = paisService;
        
        // 1. Dibujamos los controles primero
        InitializeComponentManual(); 
        
        // 2. Nos suscribimos al evento Load para cargar datos de forma segura
        this.Load += async (s, e) => await CargarDatosIniciales();
    }

    private void InitializeComponentManual()
    {
        this.Text = "Gestión de Departamentos";
        this.Size = new Size(600, 450);
        this.StartPosition = FormStartPosition.CenterScreen;

        var lblPais = new Label { Text = "Seleccione País:", Location = new Point(20, 20), AutoSize = true };
        cmbPaises = new ComboBox { Location = new Point(20, 45), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        
        var lblNom = new Label { Text = "Nombre Depto:", Location = new Point(240, 20), AutoSize = true };
        txtNombre = new TextBox { Location = new Point(240, 45), Width = 200 };

        var btnGuardar = new Button { Text = "Guardar", Location = new Point(460, 43), Width = 100 };
        // Validamos antes de llamar a guardar
        btnGuardar.Click += async (s, e) => {
            if (ValidarFormulario()) await Guardar();
        };

        var btnEliminar = new Button { Text = "Eliminar", Location = new Point(460, 75), Width = 100 };
        btnEliminar.Click += async (s, e) => await Eliminar();

        dgvDatos = new DataGridView { Location = new Point(20, 110), Size = new Size(540, 280), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        this.Controls.Add(lblPais); this.Controls.Add(cmbPaises);
        this.Controls.Add(lblNom); this.Controls.Add(txtNombre);
        this.Controls.Add(btnGuardar); this.Controls.Add(btnEliminar);
        this.Controls.Add(dgvDatos);
    }

    private async Task CargarDatosIniciales()
    {
        try 
        {
            // 1. Cargar Países
            var paises = await _paisService.ObtenerTodosAsync();
            if (paises != null && paises.Count > 0)
            {
                cmbPaises.DataSource = paises;
                cmbPaises.DisplayMember = "Nombre";
                cmbPaises.ValueMember = "Id";
                cmbPaises.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No hay países registrados. Primero agregue un país.");
                this.Close(); // Cerramos si no se puede usar
                return;
            }

            // 2. Cargar Grilla
            await RecargarGrilla();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}");
        }
    }

    private async Task RecargarGrilla()
    {
        var lista = await _service.ObtenerTodosAsync();
        dgvDatos.DataSource = lista;
    }

    private bool ValidarFormulario()
    {
        if (cmbPaises.SelectedValue == null)
        {
            MessageBox.Show("Error: No se ha seleccionado ningún país.");
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            MessageBox.Show("El nombre es obligatorio.");
            return false;
        }
        return true;
    }

    private async Task Guardar()
    {
        try 
        {
            var nuevoDto = new DepartamentoCreateDto(txtNombre.Text, (Guid)cmbPaises.SelectedValue);
            await _service.CrearAsync(nuevoDto);
            
            MessageBox.Show("Departamento guardado correctamente.");
            txtNombre.Text = "";
            await RecargarGrilla();
        } 
        catch (Exception ex) 
        { 
            MessageBox.Show($"Error al guardar: {ex.Message}"); 
        }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        try {
            var id = (Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value;
            await _service.EliminarAsync(id);
            await RecargarGrilla();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}