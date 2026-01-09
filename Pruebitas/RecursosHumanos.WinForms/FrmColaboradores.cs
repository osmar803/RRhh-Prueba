using System.Drawing;
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmColaboradores : Form
{
    private readonly ColaboradorService _service;
    private readonly EmpresaService _empresaService;
    
    // Estado para controlar Edición
    private Guid? _idSeleccionado = null;

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

        // --- 1. Panel Izquierdo (Inputs) ---
        panelFormulario = new Panel();
        panelFormulario.Dock = DockStyle.Left;
        panelFormulario.Width = 300;
        panelFormulario.BackColor = Color.White;
        panelFormulario.Padding = new Padding(20);
        this.Controls.Add(panelFormulario);

        // Título del Panel
        var lblTitulo = new Label { Text = "Colaborador", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        // Campos (Se agregan en orden inverso visual debido a DockStyle.Top)
        CrearCampoInput("Empresa:", out cmbEmpresas);
        CrearCampoInput("Nombre Completo:", out txtNombre);
        CrearCampoInput("Edad:", out txtEdad);
        CrearCampoInput("Teléfono:", out txtTel);
        CrearCampoInput("Email:", out txtEmail);

        // --- Botones ---
        
        // Botón Guardar
        var btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        // Botón Cancelar (Para salir del modo edición)
        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s, e) => Limpiar();

        // Agregamos al panel (orden inverso)
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top }); // Espaciador
        panelFormulario.Controls.Add(btnCancelar);
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top }); // Espaciador
        panelFormulario.Controls.Add(btnGuardar);

        // Botón Eliminar (Al fondo)
        var btnEliminar = new Button { Text = "Eliminar Seleccionado", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // --- 2. Grilla (Resto del espacio) ---
        dgvDatos = new DataGridView();
        dgvDatos.Dock = DockStyle.Fill;
        ThemeHelper.EstilizarGrid(dgvDatos);
        
        // Evento para seleccionar fila al hacer click
        dgvDatos.Click += (s, e) => SeleccionarFila();

        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();

        // Cargar datos iniciales
        await CargarCombos();
        await CargarDatos();
    }

    private void CrearCampoInput(string labelText, out TextBox textBox)
    {
        var panelItem = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        textBox = new TextBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

        panelItem.Controls.Add(textBox);
        panelItem.Controls.Add(lbl);
        lbl.BringToFront(); 

        panelFormulario.Controls.Add(panelItem);
        panelItem.BringToFront();
    }

    private void CrearCampoInput(string labelText, out ComboBox comboBox)
    {
        var panelItem = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 5) };
        var lbl = new Label { Text = labelText, Dock = DockStyle.Top, Height = 20, Font = new Font("Segoe UI", 9) };
        comboBox = new ComboBox { Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
        
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
        cmbEmpresas.SelectedIndex = -1; // Iniciar sin selección
    }

    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private async Task Guardar()
    {
        // Validaciones básicas
        if (!int.TryParse(txtEdad.Text, out int edad) || string.IsNullOrWhiteSpace(txtNombre.Text)) 
        { 
            MessageBox.Show("Verifique los datos (Nombre y Edad son obligatorios)."); 
            return; 
        }

        try 
        {
            if (_idSeleccionado == null)
            {
                // --- MODO CREAR ---
                if (cmbEmpresas.SelectedValue == null) 
                {
                    MessageBox.Show("Debe seleccionar una empresa para crear.");
                    return;
                }

                var dto = new ColaboradorCreateDto(txtNombre.Text, edad, txtTel.Text, txtEmail.Text);
                var id = await _service.CrearAsync(dto);
                await _service.AsignarEmpresaAsync(id, (Guid)cmbEmpresas.SelectedValue);
                
                MessageBox.Show("Colaborador Creado Exitosamente");
            }
            else
            {
                // --- MODO ACTUALIZAR ---
                var dto = new ColaboradorUpdateDto(_idSeleccionado.Value, txtNombre.Text, edad, txtTel.Text, txtEmail.Text);
                await _service.ActualizarAsync(dto);
                
                // Nota: Si se cambia la empresa en el combo durante la edición,
                // la lógica actual solo actualiza datos personales (DTO). 
                // Si requieres moverlo de empresa, se necesitaría lógica adicional en el servicio.

                MessageBox.Show("Colaborador Actualizado Exitosamente");
            }

            Limpiar();
            await CargarDatos();
        } 
        catch (Exception ex) 
        { 
            MessageBox.Show($"Error: {ex.Message}"); 
        }
    }

    private void SeleccionarFila()
    {
        if (dgvDatos.SelectedRows.Count > 0)
        {
            var row = dgvDatos.SelectedRows[0];
            _idSeleccionado = (Guid)row.Cells["Id"].Value;

            // Rellenar campos
            txtNombre.Text = row.Cells["NombreCompleto"].Value?.ToString();
            txtEdad.Text = row.Cells["Edad"].Value?.ToString();
            txtTel.Text = row.Cells["Telefono"].Value?.ToString();
            txtEmail.Text = row.Cells["CorreoElectronico"].Value?.ToString();
            
            // Nota: Como la grilla de colaboradores suele no traer el ID de la empresa directamente 
            // (a menos que esté en el DTO de respuesta), el combo podría no seleccionarse automáticamente.
            // Para edición pura de datos personales, esto es suficiente.
        }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        
        if(MessageBox.Show("¿Está seguro de eliminar este registro?", "Confirmar Eliminación", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
             try 
             {
                 await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
                 await CargarDatos();
                 Limpiar();
             }
             catch (Exception ex) 
             {
                 MessageBox.Show($"Error al eliminar: {ex.Message}");
             }
        }
    }
    
    private void Limpiar() 
    { 
        txtNombre.Clear(); 
        txtEdad.Clear(); 
        txtTel.Clear(); 
        txtEmail.Clear(); 
        cmbEmpresas.SelectedIndex = -1;
        _idSeleccionado = null; 
        dgvDatos.ClearSelection();
    }
}
