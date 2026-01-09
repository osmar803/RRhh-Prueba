using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using RecursosHumanos.WinForms.Helpers; // Importar el helper

namespace RecursosHumanos.WinForms;

public partial class Form1 : Form
{
    private Panel panelMenu;
    private Panel panelLogo;
    private Label lblBienvenida;

    public Form1()
    {
        this.Text = "Sistema de RRHH - Inicio";
        this.Size = new Size(900, 600); // Ventana más grande
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = ThemeHelper.BackgroundColor; // Fondo gris claro

        InitializeComponentsModerno();
    }

    private void InitializeComponentsModerno()
    {
        // 1. Panel Lateral (Menú)
        panelMenu = new Panel();
        panelMenu.Dock = DockStyle.Left;
        panelMenu.Width = 220;
        panelMenu.BackColor = ThemeHelper.PrimaryColor;
        this.Controls.Add(panelMenu);

        // 2. Panel Logo (Parte superior del menú)
        panelLogo = new Panel();
        panelLogo.Dock = DockStyle.Top;
        panelLogo.Height = 80;
        panelLogo.BackColor = Color.FromArgb(39, 39, 58); // Un poco más oscuro
        
        var lblTitulo = new Label();
        lblTitulo.Text = "RRHH\nSystem";
        lblTitulo.ForeColor = Color.White;
        lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
        lblTitulo.Dock = DockStyle.Fill;
        panelLogo.Controls.Add(lblTitulo);
        panelMenu.Controls.Add(panelLogo);

        // 3. Botones del Menú (Usando Dock Top para apilarlos)
        // Nota: Los agregamos en orden inverso porque Dock=Top empuja hacia abajo
        AgregarBotonMenu("Colaboradores", () => AbrirFormulario<FrmColaboradores>());
        AgregarBotonMenu("Empresas", () => AbrirFormulario<FrmEmpresas>());
        AgregarBotonMenu("Municipios", () => AbrirFormulario<FrmMunicipios>());
        AgregarBotonMenu("Departamentos", () => AbrirFormulario<FrmDepartamentos>());
        AgregarBotonMenu("Países", () => AbrirFormulario<FrmPaises>());

        // 4. Área de bienvenida (Derecha)
        lblBienvenida = new Label();
        lblBienvenida.Text = "Seleccione una opción del menú\npara comenzar a trabajar.";
        lblBienvenida.Font = new Font("Segoe UI", 18F, FontStyle.Regular);
        lblBienvenida.ForeColor = Color.Gray;
        lblBienvenida.AutoSize = false;
        lblBienvenida.TextAlign = ContentAlignment.MiddleCenter;
        lblBienvenida.Dock = DockStyle.Fill;
        this.Controls.Add(lblBienvenida);
        lblBienvenida.BringToFront();
    }

    private void AgregarBotonMenu(string texto, Action accion)
    {
        Button btn = new Button();
        btn.Dock = DockStyle.Top;
        btn.Height = 60;
        btn.Text = "  " + texto; // Espacio para "icono" imaginario
        btn.TextAlign = ContentAlignment.MiddleLeft;
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.ForeColor = Color.Gainsboro;
        btn.Font = new Font("Segoe UI", 11F);
        btn.TextImageRelation = TextImageRelation.ImageBeforeText;
        btn.Padding = new Padding(10, 0, 0, 0);
        
        // Efecto Hover sencillo
        btn.MouseEnter += (s, e) => btn.BackColor = ThemeHelper.SecondaryColor;
        btn.MouseLeave += (s, e) => btn.BackColor = ThemeHelper.PrimaryColor;
        
        btn.Click += (s, e) => accion();
        
        panelMenu.Controls.Add(btn);
        // Truco: BringToFront para mantener el orden visual correcto con Dock=Top
        btn.BringToFront(); 
        panelLogo.SendToBack(); // El logo siempre arriba
    }

    private void AbrirFormulario<T>() where T : Form
    {
        try 
        {
            var form = Program.ServiceProvider.GetRequiredService<T>();
            // Opcional: Si quisieras abrirlo DENTRO del panel derecho (MDI), requeriría más lógica.
            // Por ahora, mantendremos ShowDialog pero con el nuevo estilo.
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }
}