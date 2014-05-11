using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Papercut.SMTP;

namespace Papercut.Service
{
    public partial class Service : ServiceBase
    {
        private List<SmtpServer> smtpServers;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "/console")
            {
                Service ServicesToRun = new Service();
                ServicesToRun.OnStart(args);
                Console.ReadLine();
                ServicesToRun.OnStop();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new Service() 
                };
                ServiceBase.Run(ServicesToRun);
            }
        }

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                smtpServers = new List<SmtpServer>();
                // Start listening for connections
                string ipAddress = ConfigurationManager.AppSettings.Get("IPAddress");
                string ports = ConfigurationManager.AppSettings.Get("ports");
                if (!string.IsNullOrEmpty(ipAddress) && !string.IsNullOrEmpty(ports))
                {
                    foreach (string port in ports.Split(new char[] { ',', ';', ':', '/', '|' }))
                    {
                        int prt = 0;
                        int.TryParse(port, out prt);
                        if (prt != 0)
                        {
                            Logger.WriteDebug("Starting SMTP server on port " + port);
                            SmtpServer aServer = new SmtpServer();
                            aServer.Bind(ipAddress, prt);
                            this.smtpServers.Add(aServer);
                        }
                    }
                }
                else
                    throw new System.Configuration.ConfigurationException("IP address and Ports must be configured in app.config");
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error occurred on startup.", ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (smtpServers != null)
                {
                    foreach (SmtpServer svr in smtpServers)
                    {
                        svr.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError("Error occurred on shutdown.", ex);
            }
        }
    }
}
