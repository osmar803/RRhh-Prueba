using System.Drawing; // Necesario para colores y tamaños
using System.Windows.Forms; // Necesario para crear ventanas
using Microsoft.Extensions.DependencyInjection; // Necesario para abrir otros formularios

namespace RecursosHumanos.WinForms;

public partial class Form1 : Form
{
    // Declaramos los controles (Botones y etiquetas)
    private Label lblTitulo;
    private Button btnPaises;
    private Button btnDeptos;
    private Button btnMunicipios;
    private Button btnEmpresas;
    private Button btnColaboradores;

    public Form1()
    {
        // Configuramos la ventana principal
        this.Text = "Sistema de RRHH";
        this.Size = new Size(400, 500); // Tamaño de la ventana: Ancho 400, Alto 500
        this.StartPosition = FormStartPosition.CenterScreen; // Que aparezca en el centro

        InitializeComponentsPersonalizado();
    }

    private void InitializeComponentsPersonalizado()
    {
        // 1. Título
        lblTitulo = new Label();
        lblTitulo.Text = "Menú Principal";
        lblTitulo.Font = new Font("Arial", 16, FontStyle.Bold);
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(100, 30); // Posición X, Y
        this.Controls.Add(lblTitulo);

        // 2. Botón Países
        btnPaises = CrearBoton("Gestión de Países", 80);
        btnPaises.Click += (s, e) => AbrirFormulario<FrmPaises>();

        // 3. Botón Departamentos
        btnDeptos = CrearBoton("Gestión de Departamentos", 140);
        btnDeptos.Click += (s, e) => AbrirFormulario<FrmDepartamentos>();

        // 4. Botón Municipios
        btnMunicipios = CrearBoton("Gestión de Municipios", 200);
        btnMunicipios.Click += (s, e) => AbrirFormulario<FrmMunicipios>();

        // 5. Botón Empresas
        btnEmpresas = CrearBoton("Gestión de Empresas", 260);
        btnEmpresas.Click += (s, e) => AbrirFormulario<FrmEmpresas>();

        // 6. Botón Colaboradores
        btnColaboradores = CrearBoton("Gestión de Colaboradores", 320);
        btnColaboradores.Click += (s, e) => AbrirFormulario<FrmColaboradores>();
    }

    // Un pequeño truco para no repetir código al crear botones
    private Button CrearBoton(string texto, int posicionY)
    {
        var btn = new Button();
        btn.Text = texto;
        btn.Location = new Point(50, posicionY); // Todos alineados a la izquierda (X=50)
        btn.Size = new Size(280, 40); // Ancho 280, Alto 40
        btn.BackColor = Color.LightGray;
        this.Controls.Add(btn); // ¡Importante! Agregarlo a la ventana
        return btn;
    }

    // Método genérico para abrir cualquier formulario
    private void AbrirFormulario<T>() where T : Form
    {
        try 
        {
            // Pedimos el formulario al contenedor de inyección de dependencias
            var form = Program.ServiceProvider.GetRequiredService<T>();
            form.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al abrir: " + ex.Message);
        }
    }
}