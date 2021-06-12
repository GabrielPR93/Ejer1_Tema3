using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ServicioEjercicio
{
    public partial class ServicioEjercicio : ServiceBase
    {
       
        int puertoPorDefecto = 135; 
        int puerto;
        bool llave = true;
        public ServicioEjercicio()
        {
            InitializeComponent();
        }
        public void escribeEvento(string mensaje)
        {
            string nombre = "Servidor Ejercicio";
            string logDestino = "Application";
            if (!EventLog.SourceExists(nombre))
            {
                EventLog.CreateEventSource(nombre, logDestino);
            }
            EventLog.WriteEntry(nombre, mensaje);
        }
        protected override void OnStart(string[] args)
        {
            string msg = "";
            escribeEvento("Se inicio el Servidor Ejercicio");
            
            bool puertoArchivo = leeFichero();

            IPEndPoint ie = new IPEndPoint(IPAddress.Any,puerto);
            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                    try
                    {
                        s.Bind(ie);
                        s.Listen(3);
                        escribeEvento("Puerto: " + ie.Port);
                    }
                    catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                    {
                        escribeEvento("Error puerto en uso");
                    if (puertoArchivo)
                    {
                        puertoArchivo = false;
                        llave = true;
                        puerto = puertoPorDefecto;
                    }
                    else
                    {
                        llave = false;
                    }
                }
                while (llave)
                {

                    using (Socket sCliente = s.Accept())
                    {
                        IPEndPoint ieCliente = (IPEndPoint)sCliente.RemoteEndPoint;
                        Console.WriteLine("Client connected:{0} at port {1}", ieCliente.Address, ieCliente.Port);

                        using (NetworkStream ns = new NetworkStream(sCliente))
                        using (StreamReader sr = new StreamReader(ns))
                        using (StreamWriter sw = new StreamWriter(ns))
                        {
                     
                            try
                            {
                                msg = sr.ReadLine();
                              
                                if (msg != null)
                                {
                                    switch (msg)
                                    {
                                        case "HORA":
                                            string hora = DateTime.Now.ToString("hh:mm");
                                            sw.WriteLine(hora);
                                            sw.Flush();
                                            break;
                                        case "FECHA":
                                            string fecha = DateTime.Now.Date.ToString("dd/MM/yyyy");
                                            sw.WriteLine(fecha);
                                            sw.Flush();
                                            break;
                                        case "TODO":
                                            string todo = DateTime.Now.ToString();
                                            sw.WriteLine(todo);
                                            sw.Flush();
                                            break;
                                        case "APAGAR":
                                            sw.WriteLine("server off");
                                            sw.Flush();
                                            llave = false;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine(e.Message);
                                break;

                            }
                        }
                        Console.WriteLine("finished connection:{0} at port {1}", ieCliente.Address, ieCliente.Port);
                    }
                }
            }
        }

        protected override void OnStop()
        {
            escribeEvento("Se paro el Servidor Ejercicio");
        }

        protected bool leeFichero()
        {
            try
            {
                using (StreamReader sr=new StreamReader(Environment.GetEnvironmentVariable("PROGRAMDATA")+"//configuracion.txt"))
                {
                    string linea = sr.ReadLine();
                    if (linea!=null)
                    {
                        puerto = Int32.Parse(linea);
                        return true;
                    }
                }

            }
            catch (IOException)
            {
                escribeEvento("Error al leer el archivo");
                puerto = puertoPorDefecto;
            }
            return false;
        }


    }
}
