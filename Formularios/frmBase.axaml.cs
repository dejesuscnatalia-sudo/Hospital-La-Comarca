using Avalonia.Controls;
using Avalonia.Interactivity;
using Hospital_La_Comarca.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital_La_Comarca.Formularios
{
    public partial class frmBase : Window
    {
        public frmBase()
        {
            InitializeComponent();
            if (!Design.IsDesignMode)   // la vista previa de Rider no debe conectarse a la base de datos
            {
                CargarOpciones();
                CargarElementos();
            }
        }

        // ---------- ComboBox (clsOpcion guarda Valor y Texto) ----------
        private List<clsOpcion> _opciones = new List<clsOpcion>();
        private void CargarOpciones()
        {
            // Con base de datos seria como en frmAlumnos:
            //   Clases.clsConexion cn = new Clases.clsConexion(s.ConsultaGeneral());
            //   DataSet ds = cn.Consultar();
            //   y se recorre ds.Tables[0].Rows para llenar la lista.
            _opciones = new List<clsOpcion>();
            _opciones.Add(new clsOpcion(1, "Opcion uno"));
            _opciones.Add(new clsOpcion(2, "Opcion dos"));
            comboOpciones.ItemsSource = _opciones;
        }
        private int ValorOpcion()
        {
            return ((clsOpcion)comboOpciones.SelectedItem).Valor;
        }
        private void SeleccionarOpcion(int valor)
        {
            comboOpciones.SelectedItem = _opciones.FirstOrDefault(o => o.Valor == valor);
        }
        private void comboOpciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Design.IsDesignMode)
                return;
            // Aqui se recargaria el siguiente combo si dependiera de este (cascada, como en frmUnidades)
        }

        // ---------- ListBox (mismo patron que frmSolicitudes) ----------
        private List<clsOpcion> _elementos = new List<clsOpcion>();
        private Dictionary<int, string> _detalles = new Dictionary<int, string>();
        private void CargarElementos()
        {
            _elementos = new List<clsOpcion>();
            _detalles = new Dictionary<int, string>();
            // Ejemplo; en un caso real saldria de un DataSet
            _elementos.Add(new clsOpcion(1, "Elemento 1"));
            _detalles[1] = "Detalle del elemento 1";
            _elementos.Add(new clsOpcion(2, "Elemento 2"));
            _detalles[2] = "Detalle del elemento 2";
            listaElementos.ItemsSource = _elementos;
        }
        private void listaElementos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsOpcion seleccion = listaElementos.SelectedItem as clsOpcion;
            if (seleccion != null && _detalles.ContainsKey(seleccion.Valor))
                txtNombre.Text = _detalles[seleccion.Valor];
        }

        // ---------- Validaciones ----------
        private async Task<bool> ObtenerEntero(TextBox campo, string descripcion)
        {
            if (!int.TryParse(campo.Text, out int valor) || valor <= 0)
            {
                await clsMessageBox.Show(descripcion + " debe de ser un numero entero positivo");
                campo.Focus();
                return false;
            }
            return true;
        }
        private async Task<bool> CampoVacio(TextBox campo, string descripcion)
        {
            if (string.IsNullOrWhiteSpace(campo.Text))
            {
                await clsMessageBox.Show("Escriba " + descripcion);
                campo.Focus();
                return true;
            }
            return false;
        }

        // ---------- Buscar / Guardar / Eliminar ----------
        Clases.clsConexion c;
        private async Task Buscar()
        {
            if (!await ObtenerEntero(txtClave, "La clave"))
                return;

            // DataSet ds = new DataSet();
            // clsEntidad entidad = new clsEntidad(int.Parse(txtClave.Text));
            // c = new Clases.clsConexion(entidad.Buscar());
            // ds = c.Consultar();
            // if (ds.Tables["Tabla"].Rows.Count > 0) { txtNombre.Text = ...; SeleccionarOpcion(...); }
            // else await MessageBox.Show("No existe ese registro");
            await clsMessageBox.Show("Buscar");
        }

        private async Task Guardar()
        {
            if (!await ObtenerEntero(txtClave, "La clave") || await CampoVacio(txtNombre, "el nombre"))
                return;
            if (comboOpciones.SelectedItem == null)
            {
                await clsMessageBox.Show("Seleccione una opcion");
                comboOpciones.Focus();
                return;
            }

            // Se busca primero: si existe se pide confirmacion y se modifica, si no existe se inserta
            // DialogResult resultado = await MessageBox.Show("¿Seguro que quiere actualizar el siguiente registro?",
            //     "Confirmar modificacion", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            // if (resultado == DialogResult.OK) { ... c.Ejecutar(); }
            await clsMessageBox.Show("Registro Realizado con exito");
        }

        private async Task Eliminar()
        {
            if (!await ObtenerEntero(txtClave, "La clave"))
                return;

            // Igual que en frmEstados: el try/catch evita que una llave foranea violada cierre el programa
            // try { c = new Clases.clsConexion(entidad.Eliminar()); c.Ejecutar(); await MessageBox.Show("Registro Eliminado"); }
            // catch (Exception) { await MessageBox.Show("No se pudo eliminar. Revise que no tenga registros relacionados"); }
            await clsMessageBox.Show("Eliminar");
        }

        // ---------- Eventos ----------
        private async void btnAccion_Click(object sender, RoutedEventArgs e)
        {
            await Guardar();
        }

        private async void msRegistrar_Click(object sender, RoutedEventArgs e)
        {
            await Guardar();
        }
        private async void tsRegistrar_Click(object sender, RoutedEventArgs e)
        {
            await Guardar();
        }

        private async void msBuscar_Click(object sender, RoutedEventArgs e)
        {
            await Buscar();
        }
        private async void tsBuscar_Click(object sender, RoutedEventArgs e)
        {
            await Buscar();
        }

        private async void msEliminar_Click(object sender, RoutedEventArgs e)
        {
            await Eliminar();
        }
        private async void tsEliminar_Click(object sender, RoutedEventArgs e)
        {
            await Eliminar();
        }

        private void msSalir_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
