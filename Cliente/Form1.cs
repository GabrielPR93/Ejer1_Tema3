using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cliente
{
    public partial class Form1 : Form
    {
        string IP_SERVER = "127.0.0.1";
        int puerto = 31416;
        string msg;
        Button btn;
        public Form1()
        {
            InitializeComponent();

            button1.Tag = button1.Text;
            button2.Tag = button2.Text;
            button3.Tag = button3.Text;
            button4.Tag = button4.Text;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            btn = (Button)sender;

            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(IP_SERVER), puerto);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ie);
            }
            catch (SocketException)
            {
                MessageBox.Show("Error de conexión", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            try
            {


                using (NetworkStream ns = new NetworkStream(server))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    if (btn.Tag != null)
                    {
                        sw.WriteLine(btn.Tag); //Le manda la opcion al servidor
                        sw.Flush();
                        msg = sr.ReadLine(); //Lee lo que le manda el server
                        label1.Text = msg;
                        //if (btn.Tag=="APAGAR")
                        //{
                        //    this.Close();
                        //}
                    }
                }

                server.Close();
            }
            catch (IOException a)
            {
                MessageBox.Show("Conexion fallida: " + a.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            DialogResult res;
            res = f.ShowDialog();

            switch (res)
            {
                case DialogResult.OK:
                    try
                    {
                        puerto = Int32.Parse(f.textBoxPuerto.Text);
                        if (puerto < 1024 || puerto > 65535)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        IP_SERVER = f.textBoxIP.Text;
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Error de Formato", "Cambiar IP/Puerto", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    catch (OverflowException)
                    {
                        MessageBox.Show("Numero demasiado largo", "Cambiar IP/Puerto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        MessageBox.Show("Puerto no valido (0-65535)", "Cambiar IP/Puerto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case DialogResult.Cancel:
                    MessageBox.Show("Cambio Cancelado", "Cambiar IP/Puerto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;

            }
        }
    }
}
