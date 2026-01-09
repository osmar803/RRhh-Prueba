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
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Departamentos";
        this.Size = new Size(600, 450);
        this.StartPosition = FormStartPosition.CenterScreen; // Centrado para que se vea mejor

        // --- 1. COMBOBOX DE PAÍSES ---
        var lblPais = new Label { Text = "Seleccione País:", Location = new Point(20, 20), AutoSize = true };
        cmbPaises = new ComboBox { Location = new Point(20, 45), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        
        try 
        {
            // Cargar Paises al combo
            var paises = await _paisService.ObtenerTodosAsync();
            
            // PROTECCIÓN: Validamos que existan datos antes de asignarlos
            if (paises != null && paises.Count > 0)
            {
                cmbPaises.DataSource = paises;
                cmbPaises.DisplayMember = "Nombre";
                cmbPaises.ValueMember = "Id";
                cmbPaises.SelectedIndex = 0; // Seleccionar el primero por defecto
            }
            else
            {
                // Si no hay países, mostramos una alerta visual y deshabilitamos el combo
                MessageBox.Show("No se encontraron países. Por favor registre uno primero en la opción 'Gestión de Países'.", "Advertencia");
                cmbPaises.Enabled = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar la lista de países: " + ex.Message);
        }

        // --- 2. RESTO DE CONTROLES ---
        var lblNom = new Label { Text = "Nombre Depto:", Location = new Point(240, 20), AutoSize = true };
        txtNombre = new TextBox { Location = new Point(240, 45), Width = 200 };

        var btnGuardar = new Button { Text = "Guardar", Location = new Point(460, 43), Width = 100 };
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnEliminar = new Button { Text = "Eliminar", Location = new Point(460, 75), Width = 100 };
        btnEliminar.Click += async (s, e) => await Eliminar();

        dgvDatos = new DataGridView { Location = new Point(20, 110), Size = new Size(540, 280), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        this.Controls.Add(lblPais); this.Controls.Add(cmbPaises);
        this.Controls.Add(lblNom); this.Controls.Add(txtNombre);
        this.Controls.Add(btnGuardar); this.Controls.Add(btnEliminar);
        this.Controls.Add(dgvDatos);

        await CargarDatos();
    }

    private async Task CargarDatos()
    {
        try 
        {
            var datos = await _service.ObtenerTodosAsync();
            if (datos != null) dgvDatos.DataSource = datos;
        }
        catch (Exception ex)
        {
            // Error silencioso en la carga de la grilla para no molestar, o puedes poner MessageBox
            Console.WriteLine(ex.Message);
        }
    }

    private async Task Guardar()
    {
        // Validación extra por si el combo está vacío
        if (cmbPaises.SelectedValue == null) 
        {
            MessageBox.Show("Debe seleccionar un país válido.");
            return;
        }
        if (string.IsNullOrWhiteSpace(txtNombre.Text)) 
        {
            MessageBox.Show("Debe escribir un nombre.");
            return;
        }

        try {
            await _service.CrearAsync(new DepartamentoCreateDto(txtNombre.Text, (Guid)cmbPaises.SelectedValue));
            txtNombre.Text = "";
            await CargarDatos();
            MessageBox.Show("Departamento Guardado");
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        try {
            await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
            await CargarDatos();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }
}