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
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            btn = (Button)sender;
            if (btn.Name == "button1")
            {
                btn.Tag = button1.Text;
            }
            else if (btn.Name == "button2")
            {
                btn.Tag = button2.Text;
            }
            else if (btn.Name == "button3")
            {
                btn.Tag = button3.Text;
            }
            else
            {
                btn.Tag = button4.Text;
            }

            IPEndPoint ie = new IPEndPoint(IPAddress.Parse(IP_SERVER), puerto);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Connect(ie);
            }
            catch (SocketException)
            {

                Console.WriteLine("Error connection");
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
            catch (IOException)
            {
                label1.Text = "Error, server off";
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

                    puerto = Int32.Parse( f.textBoxPuerto.Text);
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
                    break;
                case DialogResult.Cancel:
                    MessageBox.Show("Cambio Cancelado", "Cambiar IP/Puerto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
            
            }
        }
    }
}
