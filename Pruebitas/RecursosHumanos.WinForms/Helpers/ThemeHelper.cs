using System.Drawing;
using System.Windows.Forms;

namespace RecursosHumanos.WinForms.Helpers;

public static class ThemeHelper
{
    // Paleta de colores "Moderna"
    public static Color PrimaryColor = Color.FromArgb(51, 51, 76);    // Azul oscuro (Panel lateral)
    public static Color SecondaryColor = Color.FromArgb(0, 150, 136); // Turquesa (Botones acción)
    public static Color DangerColor = Color.FromArgb(220, 53, 69);    // Rojo (Eliminar)
    public static Color BackgroundColor = Color.FromArgb(240, 240, 240); // Gris muy claro (Fondo)
    public static Color TextColor = Color.Black;

    // Método para estilizar botones
    public static void EstilizarBoton(Button btn, bool esPeligroso = false)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        btn.BackColor = esPeligroso ? DangerColor : SecondaryColor;
        btn.ForeColor = Color.White;
        btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btn.Cursor = Cursors.Hand;
        btn.Size = new Size(150, 40); // Tamaño estándar
    }

    // Método para estilizar DataGridView (Tablas)
    public static void EstilizarGrid(DataGridView dgv)
    {
        dgv.BackgroundColor = Color.White;
        dgv.BorderStyle = BorderStyle.None;
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        
        // Cabecera
        dgv.ColumnHeadersDefaultCellStyle.BackColor = PrimaryColor;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        dgv.ColumnHeadersHeight = 35;
        dgv.EnableHeadersVisualStyles = false; // Importante para que tome el color

        // Filas
        dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
        dgv.DefaultCellStyle.SelectionBackColor = SecondaryColor;
        dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        dgv.RowHeadersVisible = false; // Ocultar la primera columna vacía
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Llenar espacio
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    // Método para estilizar Labels y TextBoxes
    public static void EstilizarControles(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is Label lbl)
            {
                lbl.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                lbl.ForeColor = PrimaryColor;
            }
            else if (c is TextBox txt)
            {
                txt.Font = new Font("Segoe UI", 10F);
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (c is ComboBox cmb)
            {
                cmb.Font = new Font("Segoe UI", 10F);
                cmb.FlatStyle = FlatStyle.Flat;
            }
        }
    }
}