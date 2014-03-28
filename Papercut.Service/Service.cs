using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Papercut.SMTP;

namespace Papercut.Service
{
	public partial class Service : ServiceBase
	{
		private List<SmtpServer> smtpServers;

		public Service()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
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
					Logger.WriteError("Configuration Error", new System.Configuration.ConfigurationException("IP address and Ports must be configured in app.config"));
			}
			catch (Exception ex)
			{
				Logger.WriteError("Error occurred on startup.", ex);
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
