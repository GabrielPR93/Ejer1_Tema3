using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ejer1_Tema3
{
    class Program
    {
        static void Main(string[] args)
        {
            int puerto = 31416;
            int puertoSec = 50000;
            bool flag = true;
            IPEndPoint ie = new IPEndPoint(IPAddress.Any, puerto);

            using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {

                try
                {

              
                s.Bind(ie);     //Enlace con el socket
                s.Listen(3);    //cola de espera
               
                }
                catch (SocketException e) when(e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                {

                    Console.WriteLine($"Port {puerto} in use");
                    flag = false;
                }
                while (flag)
                {

                    using (Socket sCliente = s.Accept())
                    {
                        IPEndPoint ieCliente = (IPEndPoint)sCliente.RemoteEndPoint;
                        Console.WriteLine("Client connected:{0} at port {1}", ieCliente.Address, ieCliente.Port);

                        using (NetworkStream ns = new NetworkStream(sCliente))
                        using (StreamReader sr = new StreamReader(ns))
                        using (StreamWriter sw = new StreamWriter(ns))
                        {

                            string msg = "";

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
                                            flag = false;
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

    }
}
