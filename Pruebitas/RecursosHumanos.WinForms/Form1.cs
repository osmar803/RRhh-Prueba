using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using RecursosHumanos.WinForms.Helpers;

namespace RecursosHumanos.WinForms;

public partial class Form1 : Form
{
    private Panel panelMenu;
    private Panel panelLogo;
    private Panel panelContenedor; 
    private Button btnVolver;      
    private Label lblBienvenida;
    
    private Form? formularioActivo = null; 
    private IServiceScope? scopeActivo = null;

    // BLOQUEO DE SEGURIDAD
    private bool estaCargando = false;

    public Form1()
    {
        this.Text = "Sistema de RRHH - Inicio";
        this.Size = new Size(1150, 750); 
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = ThemeHelper.BackgroundColor;

        InitializeComponentsModerno();
    }

    private void InitializeComponentsModerno()
    {
        panelMenu = new Panel { Dock = DockStyle.Left, Width = 230, BackColor = ThemeHelper.PrimaryColor };
        this.Controls.Add(panelMenu);

        panelLogo = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(39, 39, 58) };
        var lblTitulo = new Label { 
            Text = "RRHH\nSystem", ForeColor = Color.White, Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill 
        };
        panelLogo.Controls.Add(lblTitulo);
        panelMenu.Controls.Add(panelLogo);

        // Botones
        AgregarBotonMenu("Colaboradores", () => AbrirFormularioEnPanel<FrmColaboradores>());
        AgregarBotonMenu("Empresas", () => AbrirFormularioEnPanel<FrmEmpresas>());
        AgregarBotonMenu("Municipios", () => AbrirFormularioEnPanel<FrmMunicipios>());
        AgregarBotonMenu("Departamentos", () => AbrirFormularioEnPanel<FrmDepartamentos>());
        AgregarBotonMenu("Países", () => AbrirFormularioEnPanel<FrmPaises>());

        panelContenedor = new Panel { 
            Dock = DockStyle.Fill, 
            BackColor = ThemeHelper.BackgroundColor,
            AutoScroll = true // Esto evita que se corte la pantalla
        };
        this.Controls.Add(panelContenedor);
        panelContenedor.BringToFront(); 

        btnVolver = new Button {
            Text = "⬅  Volver al Menú Principal", Height = 40, Dock = DockStyle.Top,
            FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(119, 136, 153), 
            ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Cursor = Cursors.Hand,
            Visible = false
        };
        btnVolver.FlatAppearance.BorderSize = 0;
        btnVolver.Click += (s, e) => VolverAlInicio();
        panelContenedor.Controls.Add(btnVolver);

        lblBienvenida = new Label {
            Text = "Bienvenido al Sistema de RRHH\n\nSeleccione una opción del menú lateral.",
            Font = new Font("Segoe UI", 16F, FontStyle.Regular), ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill
        };
        panelContenedor.Controls.Add(lblBienvenida);
    }

    private void AgregarBotonMenu(string texto, Action accion)
    {
        Button btn = new Button {
            Dock = DockStyle.Top, Height = 60, Text = "  " + texto, TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Flat, ForeColor = Color.Gainsboro, Font = new Font("Segoe UI", 11F),
            TextImageRelation = TextImageRelation.ImageBeforeText, Padding = new Padding(10, 0, 0, 0)
        };
        btn.FlatAppearance.BorderSize = 0;
        // Evitamos que el clic se procese si ya estamos cargando
        btn.Click += (s, e) => {
            if (!estaCargando) accion();
        };
        panelMenu.Controls.Add(btn);
        btn.BringToFront(); 
        panelLogo.SendToBack();
    }

    private async void AbrirFormularioEnPanel<T>() where T : Form
    {
        // 1. Verificación doble de seguridad
        if (estaCargando) return;

        try 
        {
            estaCargando = true;
            panelMenu.Enabled = false; // DESHABILITA TODO EL MENÚ DE INMEDIATO
            this.Cursor = Cursors.WaitCursor;

            CerrarFormularioActivo();

            // 2. PAUSA DE SEGURIDAD: 
            // Esto permite que el motor de la base de datos termine de liberar los recursos anteriores
            await Task.Delay(250); 

            scopeActivo = Program.ServiceProvider.CreateScope();
            var form = scopeActivo.ServiceProvider.GetRequiredService<T>();
            
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            
            panelContenedor.Controls.Add(form);
            formularioActivo = form;

            lblBienvenida.Visible = false;
            btnVolver.Visible = true; 
            
            form.Show();
            form.BringToFront(); 
        }
        catch (Exception ex)
        {
            // Ignoramos errores de cancelación por navegación rápida
            if (!(ex is OperationCanceledException || ex.InnerException is OperationCanceledException))
            {
                MessageBox.Show("Error al cargar módulo: " + ex.Message);
            }
        }
        finally
        {
            // 3. LIBERACIÓN LENTA:
            // Esperamos un momento antes de permitir clics de nuevo para evitar rebotes
            await Task.Delay(100);
            estaCargando = false;
            panelMenu.Enabled = true;
            this.Cursor = Cursors.Default;
        }
    }

    private void VolverAlInicio()
    {
        CerrarFormularioActivo();
        btnVolver.Visible = false;
        lblBienvenida.Visible = true;
    }

    private void CerrarFormularioActivo()
    {
        if (formularioActivo != null)
        {
            formularioActivo.Hide(); // Ocultar para que no se vea feo mientras se destruye
            formularioActivo.Close();
            panelContenedor.Controls.Remove(formularioActivo);
            formularioActivo.Dispose();
            formularioActivo = null;
        }

        if (scopeActivo != null)
        {
            scopeActivo.Dispose(); // Libera el DbContext
            scopeActivo = null;
        }
    }
}