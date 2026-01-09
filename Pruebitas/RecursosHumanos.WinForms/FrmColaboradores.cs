using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;

namespace RecursosHumanos.WinForms;

public class FrmColaboradores : Form
{
    private readonly ColaboradorService _service;
    private readonly EmpresaService _empresaService;
    private ComboBox cmbEmpresas;
    private TextBox txtNombre, txtEdad, txtTel, txtEmail;
    private DataGridView dgvDatos;

    public FrmColaboradores(ColaboradorService service, EmpresaService empresaService)
    {
        _service = service;
        _empresaService = empresaService;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Colaboradores";
        this.Size = new Size(800, 600);

        CrearCampo("Nombre Completo:", 20, out txtNombre);
        CrearCampo("Edad:", 70, out txtEdad);
        CrearCampo("Teléfono:", 120, out txtTel);
        CrearCampo("Email:", 170, out txtEmail);

        var lblEmp = new Label { Text = "Asignar a Empresa:", Location = new Point(300, 20), AutoSize = true };
        cmbEmpresas = new ComboBox { Location = new Point(300, 45), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        var empresas = await _empresaService.ObtenerTodosAsync();
        cmbEmpresas.DataSource = empresas;
        cmbEmpresas.DisplayMember = "NombreComercial";
        cmbEmpresas.ValueMember = "Id";

        var btnGuardar = new Button { Text = "Guardar Colaborador", Location = new Point(300, 100), Size = new Size(200, 40) };
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnEliminar = new Button { Text = "Eliminar", Location = new Point(300, 150), Size = new Size(200, 40) };
        btnEliminar.Click += async (s, e) => await Eliminar();

        dgvDatos = new DataGridView { Location = new Point(20, 250), Size = new Size(740, 280), ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

        this.Controls.Add(lblEmp); this.Controls.Add(cmbEmpresas);
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
        if (cmbEmpresas.SelectedValue == null || !int.TryParse(txtEdad.Text, out int edad)) { MessageBox.Show("Verifique datos"); return; }
        try {
            var id = await _service.CrearAsync(new ColaboradorCreateDto(txtNombre.Text, edad, txtTel.Text, txtEmail.Text));
            await _service.AsignarEmpresaAsync(id, (Guid)cmbEmpresas.SelectedValue);
            await CargarDatos();
            MessageBox.Show("Colaborador Guardado");
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
        await CargarDatos();
    }
}