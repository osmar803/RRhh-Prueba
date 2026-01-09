using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;

namespace RecursosHumanos.WinForms;

public class FrmEmpresas : Form
{
    private readonly EmpresaService _service;
    private readonly MunicipioService _muniService;
    private ComboBox cmbMunicipios;
    private TextBox txtNit, txtRazon, txtComercial, txtTel, txtEmail;
    private DataGridView dgvDatos;

    public FrmEmpresas(EmpresaService service, MunicipioService muniService)
    {
        _service = service;
        _muniService = muniService;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Empresas";
        this.Size = new Size(800, 600);

        // Campos
        CrearCampo("NIT:", 20, out txtNit);
        CrearCampo("Razón Social:", 70, out txtRazon);
        CrearCampo("Nombre Comercial:", 120, out txtComercial);
        CrearCampo("Teléfono:", 170, out txtTel);
        CrearCampo("Email:", 220, out txtEmail);

        var lblMuni = new Label { Text = "Municipio:", Location = new Point(300, 20), AutoSize = true };
        cmbMunicipios = new ComboBox { Location = new Point(300, 45), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        var munis = await _muniService.ObtenerTodosAsync();
        cmbMunicipios.DataSource = munis;
        cmbMunicipios.DisplayMember = "Nombre";
        cmbMunicipios.ValueMember = "Id";

        var btnGuardar = new Button { Text = "Guardar Empresa", Location = new Point(300, 100), Size = new Size(200, 40) };
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnEliminar = new Button { Text = "Eliminar", Location = new Point(300, 150), Size = new Size(200, 40) };
        btnEliminar.Click += async (s, e) => await Eliminar();

        dgvDatos = new DataGridView { Location = new Point(20, 280), Size = new Size(740, 250), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        this.Controls.Add(lblMuni); this.Controls.Add(cmbMunicipios);
        this.Controls.Add(btnGuardar); this.Controls.Add(btnEliminar);
        this.Controls.Add(dgvDatos);
        await CargarDatos();
    }

    private void CrearCampo(string label, int y, out TextBox txt)
    {
        var lbl = new Label { Text = label, Location = new Point(20, y), AutoSize = true };
        txt = new TextBox { Location = new Point(20, y + 20), Width = 250 };
        this.Controls.Add(lbl);
        this.Controls.Add(txt);
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
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
        await CargarDatos();
    }
}