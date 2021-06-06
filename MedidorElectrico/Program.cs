using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MedidorModel;
using MedidorModel.DAL;
using MedidorModel.DTO;
using ServidorSocketUtils;

namespace MedidorElectrico
{
    class Program
    {

        private static IMedidorDAL medidorDAL = MedidorDALArchivo.GetInstancia();
        static void Main(string[] args)
        {
            IniciarServidor();
            while (Menu()) ;
        }

        private static bool Menu()
        {
            bool continuar = true;
            Console.WriteLine("Bienvenido");
            Console.WriteLine("1. Ingresar");
            Console.WriteLine("2. Mostrar");
            Console.WriteLine("0. Salir");

            switch (Console.ReadLine().Trim())
            {
                case "1":
                    Ingresar();
                    break;
                case "2":
                    Mostrar();
                    break;
                case "0":
                    continuar = false;
                    break;
                default:
                    Console.WriteLine("Elija bien su opcion");
                    break;
            }
            return continuar;
        }

        static void IniciarServidor()
        {
            int puerto = Convert.ToInt32(ConfigurationManager.AppSettings["puerto"]);
            ServerSocket servidor = new ServerSocket(puerto);
            if (servidor.Iniciar())
            {
                while (true)
                {
                    Console.WriteLine("Esperando cliente.....");
                    Socket cliente = servidor.ObtenerCliente();
                    Console.WriteLine("Cliente conectado");
                    ClienteCom clienteCom = new ClienteCom(cliente);
                    clienteCom.Escribir("Ingrese numero de medidor");
                    string numero = clienteCom.Leer();
                    clienteCom.Escribir("Ingrese fecha");
                    string fecha = clienteCom.Leer();
                    clienteCom.Escribir("Ingrese valor consumo");
                    string valor = clienteCom.Leer();

                    Medidor medidor = new Medidor()
                    {
                        NroMedidor = Convert.ToInt32(numero),
                        Fecha = Convert.ToString(fecha),
                        ValorConsumo = Convert.ToDecimal(valor)
                    };
                    medidorDAL.AgregarMedidor(medidor);
                    clienteCom.Desconectar();
                }

            }
            else
            {
                Console.WriteLine("No es posible conectarse al servidor");
            }
        }


        private static void Ingresar()
        {
            Console.WriteLine("Ingrese datos:  ");
            string datos = Console.ReadLine().Trim();

            string[] dato = datos.Split('|', '|', '|');

            int num = Convert.ToInt32(dato[0]);
            string fecha = Convert.ToString(dato[1]);
            decimal valor = Convert.ToDecimal(dato[2]);

            Medidor medidor = new Medidor()
            {
                NroMedidor = num,
                Fecha = fecha,
                ValorConsumo= valor
            };
  
            medidorDAL.AgregarMedidor(medidor);
        }

         static void Mostrar()
         {
            List<Medidor> medidor = medidorDAL.ObtenerMedidor();
            foreach(Medidor medidors in medidor)
            {
                Console.WriteLine(medidors);
            }
         }


       
    }
}



//List<Medidor> medidors = null;
//lock (medidorDAL)
//{
//    medidors = medidorDAL.ObtenerMedidor();
//}
//foreach (Medidor medidor in medidors)
//{
//    Console.WriteLine(medidor);
//}