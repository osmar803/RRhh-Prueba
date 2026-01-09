using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;

namespace RecursosHumanos.WinForms;

public class FrmMunicipios : Form
{
    private readonly MunicipioService _service;
    private readonly DepartamentoService _deptoService;
    private ComboBox cmbDeptos;
    private TextBox txtNombre;
    private DataGridView dgvDatos;

    public FrmMunicipios(MunicipioService service, DepartamentoService deptoService)
    {
        _service = service;
        _deptoService = deptoService;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Municipios";
        this.Size = new Size(600, 450);

        var lblDepto = new Label { Text = "Departamento:", Location = new Point(20, 20), AutoSize = true };
        cmbDeptos = new ComboBox { Location = new Point(20, 45), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        
        var deptos = await _deptoService.ObtenerTodosAsync();
        cmbDeptos.DataSource = deptos;
        cmbDeptos.DisplayMember = "Nombre";
        cmbDeptos.ValueMember = "Id";

        var lblNom = new Label { Text = "Nombre Municipio:", Location = new Point(240, 20), AutoSize = true };
        txtNombre = new TextBox { Location = new Point(240, 45), Width = 200 };

        var btnGuardar = new Button { Text = "Guardar", Location = new Point(460, 43), Width = 100 };
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnEliminar = new Button { Text = "Eliminar", Location = new Point(460, 75), Width = 100 };
        btnEliminar.Click += async (s, e) => await Eliminar();

        dgvDatos = new DataGridView { Location = new Point(20, 110), Size = new Size(540, 280), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        this.Controls.Add(lblDepto); this.Controls.Add(cmbDeptos);
        this.Controls.Add(lblNom); this.Controls.Add(txtNombre);
        this.Controls.Add(btnGuardar); this.Controls.Add(btnEliminar);
        this.Controls.Add(dgvDatos);
        await CargarDatos();
    }

    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private async Task Guardar()
    {
        if (cmbDeptos.SelectedValue == null || string.IsNullOrWhiteSpace(txtNombre.Text)) return;
        await _service.CrearAsync(new MunicipioCreateDto(txtNombre.Text, (Guid)cmbDeptos.SelectedValue));
        txtNombre.Text = "";
        await CargarDatos();
        MessageBox.Show("Municipio Guardado");
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
        await CargarDatos();
    }
}