using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions; // Para validar email
using System.Windows.Forms;
using RecursosHumanos.Application.DTOs;
using RecursosHumanos.Application.Services;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public class FrmColaboradores : Form
{
    private readonly ColaboradorService _service;
    private readonly EmpresaService _empresaService;
    
    // Estado
    private Guid? _idSeleccionado = null;

    // Componentes de UX
    private ErrorProvider _errorProvider; // <--- NUEVO: Para iconos rojos
    private Button btnGuardar; // Referencia global para poder deshabilitarlo

    // Controles
    private ComboBox cmbEmpresas;
    private TextBox txtNombre, txtEdad, txtTel, txtEmail;
    private DataGridView dgvDatos;
    private Panel panelFormulario;

    public FrmColaboradores(ColaboradorService service, EmpresaService empresaService)
    {
        _service = service;
        _empresaService = empresaService;
        
        // Inicializar ErrorProvider
        _errorProvider = new ErrorProvider();
        _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink; // Para que no parpadee molesto

        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Gestión de Colaboradores";
        this.Size = new Size(1000, 600);
        this.BackColor = ThemeHelper.BackgroundColor;
        // AutoValidate evita que el foco se quede atrapado en un control inválido,
        // permitiendo al usuario corregir otros campos libremente.
        this.AutoValidate = AutoValidate.EnableAllowFocusChange;

        // --- 1. Panel Izquierdo (Inputs) ---
        panelFormulario = new Panel();
        panelFormulario.Dock = DockStyle.Left;
        panelFormulario.Width = 320;
        panelFormulario.BackColor = Color.White;
        panelFormulario.Padding = new Padding(20);
        this.Controls.Add(panelFormulario);

        var lblTitulo = new Label { Text = "Colaborador", Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeHelper.PrimaryColor };
        panelFormulario.Controls.Add(lblTitulo);

        // Campos
        CrearCampoInput("Empresa:", out cmbEmpresas);
        CrearCampoInput("Nombre Completo:", out txtNombre);
        CrearCampoInput("Edad:", out txtEdad);
        CrearCampoInput("Teléfono:", out txtTel);
        CrearCampoInput("Email:", out txtEmail);

        // Configurar Validaciones (Eventos)
        ConfigurarValidaciones();

        // --- Botones ---
        btnGuardar = new Button { Text = "Guardar", Height = 45, Dock = DockStyle.Top };
        ThemeHelper.EstilizarBoton(btnGuardar);
        btnGuardar.Click += async (s, e) => await Guardar();

        var btnCancelar = new Button { Text = "Cancelar Edición", Height = 30, Dock = DockStyle.Top, FlatStyle = FlatStyle.Flat, ForeColor = Color.Gray };
        btnCancelar.Click += (s, e) => Limpiar();

        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnCancelar);
        panelFormulario.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top });
        panelFormulario.Controls.Add(btnGuardar);

        var btnEliminar = new Button { Text = "Eliminar Seleccionado", Height = 45, Dock = DockStyle.Bottom };
        ThemeHelper.EstilizarBoton(btnEliminar, esPeligroso: true);
        btnEliminar.Click += async (s, e) => await Eliminar();
        panelFormulario.Controls.Add(btnEliminar);

        // --- 2. Grilla ---
        dgvDatos = new DataGridView();
        dgvDatos.Dock = DockStyle.Fill;
        ThemeHelper.EstilizarGrid(dgvDatos);
        dgvDatos.Click += (s, e) => SeleccionarFila();
        this.Controls.Add(dgvDatos);
        dgvDatos.BringToFront();

        await CargarCombos();
        await CargarDatos();
    }

    // --- Lógica de Validación "En Vivo" ---
    private void ConfigurarValidaciones()
    {
        // Al salir del textbox (Validating), verificamos.
        // También limpiamos el error si el usuario empieza a escribir (TextChanged) para mejor UX.

        // 1. Nombre
        txtNombre.Validating += (s, e) => ValidarRequerido(txtNombre, "El nombre es obligatorio.");
        txtNombre.TextChanged += (s, e) => _errorProvider.SetError(txtNombre, ""); 

        // 2. Edad (Requerido + Numérico + Mayor de edad)
        txtEdad.Validating += (s, e) => ValidarEdad();
        txtEdad.TextChanged += (s, e) => _errorProvider.SetError(txtEdad, "");

        // 3. Email (Formato)
        txtEmail.Validating += (s, e) => ValidarEmail();
        txtEmail.TextChanged += (s, e) => _errorProvider.SetError(txtEmail, "");

        // 4. Empresa (Combo requerido)
        cmbEmpresas.Validating += (s, e) => ValidarCombo(cmbEmpresas, "Seleccione una empresa.");
        cmbEmpresas.SelectedIndexChanged += (s, e) => _errorProvider.SetError(cmbEmpresas, "");
    }

    private bool ValidarRequerido(Control ctrl, string msj)
    {
        if (string.IsNullOrWhiteSpace(ctrl.Text))
        {
            _errorProvider.SetError(ctrl, msj);
            return false;
        }
        _errorProvider.SetError(ctrl, "");
        return true;
    }

    private bool ValidarCombo(ComboBox cmb, string msj)
    {
        if (cmb.SelectedIndex == -1 && cmb.SelectedValue == null)
        {
            _errorProvider.SetError(cmb, msj);
            return false;
        }
        _errorProvider.SetError(cmb, "");
        return true;
    }

    private bool ValidarEdad()
    {
        if (string.IsNullOrWhiteSpace(txtEdad.Text)) 
        {
            _errorProvider.SetError(txtEdad, "Ingrese la edad.");
            return false;
        }
        if (!int.TryParse(txtEdad.Text, out int edad) || edad < 18)
        {
            _errorProvider.SetError(txtEdad, "Debe ser un número válido (18+).");
            return false;
        }
        _errorProvider.SetError(txtEdad, "");
        return true;
    }

    private bool ValidarEmail()
    {
        if (string.IsNullOrWhiteSpace(txtEmail.Text)) 
        {
             _errorProvider.SetError(txtEmail, "El email es obligatorio.");
             return false;
        }
        // Regex simple para email
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!regex.IsMatch(txtEmail.Text))
        {
            _errorProvider.SetError(txtEmail, "Formato de correo inválido.");
            return false;
        }
        _errorProvider.SetError(txtEmail, "");
        return true;
    }

    private bool EsFormularioValido()
    {
        // Ejecutamos todas las validaciones manualmente para asegurar que 
        // el ErrorProvider se muestre si el usuario le dio directo a "Guardar"
        bool v1 = ValidarRequerido(txtNombre, "Nombre obligatorio.");
        bool v2 = ValidarEdad();
        bool v3 = ValidarEmail();
        bool v4 = true;
        
        // Si es modo CREAR, la empresa es obligatoria. En modo EDITAR, es opcional (según tu lógica actual)
        if (_idSeleccionado == null) 
            v4 = ValidarCombo(cmbEmpresas, "Seleccione empresa.");

        return v1 && v2 && v3 && v4;
    }

    // --- Lógica Principal ---

    private async Task Guardar()
    {
        // 1. Validar antes de hacer nada
        if (!EsFormularioValido()) 
        {
            MessageBox.Show("Por favor corrija los errores marcados en rojo.", "Datos inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return; 
        }

        // 2. Feedback Visual: Deshabilitar botón
        btnGuardar.Enabled = false;
        btnGuardar.Text = "Guardando...";
        
        try 
        {
            int edad = int.Parse(txtEdad.Text); // Seguro porque ya validamos

            if (_idSeleccionado == null)
            {
                // CREAR
                var id = await _service.CrearAsync(new ColaboradorCreateDto(txtNombre.Text, edad, txtTel.Text, txtEmail.Text));
                if (cmbEmpresas.SelectedValue != null) 
                    await _service.AsignarEmpresaAsync(id, (Guid)cmbEmpresas.SelectedValue);
                
                MessageBox.Show("Colaborador Creado Correctamente");
            }
            else
            {
                // ACTUALIZAR
                var dto = new ColaboradorUpdateDto(_idSeleccionado.Value, txtNombre.Text, edad, txtTel.Text, txtEmail.Text);
                await _service.ActualizarAsync(dto);
                MessageBox.Show("Colaborador Actualizado Correctamente");
            }

            Limpiar();
            await CargarDatos();
        } 
        catch (Exception ex) 
        { 
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
        }
        finally
        {
            // 3. Restaurar botón SIEMPRE (incluso si hubo error)
            btnGuardar.Enabled = true;
            btnGuardar.Text = "Guardar";
        }
    }

    // ... (El resto de métodos auxiliares se mantienen igual) ...

    private void CrearCampoInput(string labelText, out TextBox textBox)
    {
        var panelItem = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0, 5, 20, 5) }; // Padding derecho extra para el icono
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
        var panelItem = new Panel { Height = 60, Dock = DockStyle.Top, Padding = new Padding(0, 5, 20, 5) };
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
        cmbEmpresas.SelectedIndex = -1;
    }

    private async Task CargarDatos() => dgvDatos.DataSource = await _service.ObtenerTodosAsync();

    private void SeleccionarFila()
    {
        if (dgvDatos.SelectedRows.Count > 0)
        {
            var row = dgvDatos.SelectedRows[0];
            _idSeleccionado = (Guid)row.Cells["Id"].Value;

            txtNombre.Text = row.Cells["NombreCompleto"].Value?.ToString();
            txtEdad.Text = row.Cells["Edad"].Value?.ToString();
            txtTel.Text = row.Cells["Telefono"].Value?.ToString();
            txtEmail.Text = row.Cells["CorreoElectronico"].Value?.ToString();
            
            // Limpiar errores visuales al cargar datos válidos
            _errorProvider.Clear();
        }
    }

    private async Task Eliminar()
    {
        if (dgvDatos.SelectedRows.Count == 0) return;
        
        if(MessageBox.Show("¿Está seguro de eliminar este registro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
             try 
             {
                 await _service.EliminarAsync((Guid)dgvDatos.SelectedRows[0].Cells["Id"].Value);
                 await CargarDatos();
                 Limpiar();
             }
             catch (Exception ex) { MessageBox.Show(ex.Message); }
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
        _errorProvider.Clear(); // Quitar iconos rojos
    }
}