using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmMunicipios : Form
{
    private readonly MunicipioService _service;
    private readonly DepartamentoService _deptoService;
    private Guid? _idSeleccionado = null;

    private ComboBox cmbDeptos;
    private TextBox txtNombre;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmMunicipios(MunicipioService service, DepartamentoService deptoService)
    {
        _service = service;
        _deptoService = deptoService;
        ConfigurarUI();

        // Manejo de carga inicial con protección contra cancelaciones
        this.Load += async (s, e) => {
            try 
            {
                await CargarCombos();
                await CargarDatos();
            }
            catch (OperationCanceledException) { /* Ignorar cancelación al cerrar scope */ }
            catch (Exception ex) { MessageBox.Show("Error al cargar: " + ex.Message); }
        };
    }

    private void ConfigurarUI()
    {
        this.Text = "Gestión de Municipios";
        this.Size = new Size(950, 550);
        this.BackColor = ThemeHelper.BackgroundColor;

        // --- Panel Ayuda ---
        var panelAyuda = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = ThemeHelper.InfoBackgroundColor, Padding = new Padding(10) };
        var lblInstruccion = new Label {
            Text = "ℹ️ Para actualizar: Seleccione un municipio, edite el nombre o departamento y guarde cambios.",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
        };
        ThemeHelper.EstilizarLabelAyuda(lblInstruccion);
        panelAyuda.Controls.Add(lblInstruccion);
        this.Controls.Add(panelAyuda);

        // --- Panel Izquierdo ---
        panelFormulario = new Panel { Dock = DockStyle.Left, Width = 300, BackColor = Color.White, Padding = new Padding(20) };
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "Municipio", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        CrearCampoInput("Nombre Municipio:", out txtNombre);
        CrearCampoInput("Departamento:", out cmbDeptos);

        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s, e) => Limpiar();

        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnCancelar);
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // --- Grilla ---
        dgvDatos = new DataGridView { Dock = DockStyle.Fill };
        ThemeHelper.EstilizarGrid(dgvDatos);
        dgvDatos.Click += (s, e) => SeleccionarFila();
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();
    }

    private void CrearCampoInput(string label, out TextBox txt) {
        var p = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = label, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        txt = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        p.Controls.Add(txt); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }

    private void CrearCampoInput(string label, out ComboBox cmb) {
        var p = new Panel { Height = 65, Dock = DockStyle.Top, Padding = new Padding(0,5,0,5) };
        var l = new Label { Text = label, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        cmb = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        p.Controls.Add(cmb); p.Controls.Add(l); l.BringToFront();
        panelFormulario.Controls.Add(p); p.BringToFront();
    }

    private async Task CargarCombos() {
        try {
            cmbDeptos.DataSource = await _deptoService.ObtenerTodosAsync();
            cmbDeptos.DisplayMember = "Nombre"; cmbDeptos.ValueMember = "Id";
        } catch (OperationCanceledException) { }
    }

    private async Task CargarDatos() {
        try {
            dgvDatos.DataSource = await _service.ObtenerTodosAsync();
        } catch (OperationCanceledException) { }
    }

    private async Task Guardar() {
        if (cmbDeptos.SelectedValue == null || string.IsNullOrWhiteSpace(txtNombre.Text)) return;
        try {
            if (_idSeleccionado == null) {
                await _service.CrearAsync(new MunicipioCreateDto(txtNombre.Text, (Guid)cmbDeptos.SelectedValue));
                MessageBox.Show("Guardado");
            } else {
                await _service.ActualizarAsync(new MunicipioUpdateDto(_idSeleccionado.Value, txtNombre.Text, (Guid)cmbDeptos.SelectedValue));
                MessageBox.Show("Actualizado");
            }
            Limpiar(); await CargarDatos();
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void SeleccionarFila() {
        if (dgvDatos.SelectedRows.Count > 0) {
            var row = dgvDatos.SelectedRows[0];
            _idSeleccionado = (Guid)row.Cells["Id"].Value;
            txtNombre.Text = row.Cells["Nombre"].Value.ToString();
            if(row.Cells["DepartamentoId"].Value != null) cmbDeptos.SelectedValue = row.Cells["DepartamentoId"].Value;
        }
    }

    private async Task Eliminar() {
        if (dgvDatos.SelectedRows.Count == 0) return;
        if(MessageBox.Show("¿Eliminar?", "Confirme", MessageBoxButtons.YesNo) == DialogResult.Yes){
            await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
            Limpiar(); await CargarDatos();
        }
    }

    private void Limpiar() { txtNombre.Text = ""; cmbDeptos.SelectedIndex = -1; _idSeleccionado = null; }
}