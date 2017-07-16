using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web.UI;

namespace Diagnostics
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class Data : Page
	{
		const string VirtualMemory = "Virtual Memory";
		const string Memory = "Memory";
		const string Name = "Name";
		const string PIDTime = "PID";
		const string Hand = "Handles";
		const string Thread = "Threads";
		const string CPUTime = "CPU";
		
		private void Page_Load(object sender, EventArgs e)
		{
			DoDiagnostics();
		}
	
		private void DoDiagnostics() 
		{
			Hashtable ht = new Hashtable();

			Response.Write(Request.UserHostAddress + "<br>");

			ht.Add("User:", Environment.UserName);
			ht.Add("Interactive User: ", Environment.UserInteractive);
			ht.Add("DOMAIN: ", Environment.UserDomainName);
			ht.Add("Folder: ", Environment.CurrentDirectory);
			ht.Add("Operative System: ", Environment.OSVersion);
			ht.Add("Memory State:" ,String.Concat(Environment.WorkingSet," ","btyes") );
			ht.Add("Application Domain ID: ", AppDomain.GetCurrentThreadId());
			ht.Add("Application Domain Folder: ",AppDomain.CurrentDomain.BaseDirectory);
			try
			{	
				Process p = Process.GetCurrentProcess();
				ht.Add("Thread ID: ",p.Id );
				ht.Add("Priority: ",p.BasePriority);
			}
			catch (Exception e)
			{
				Response.Write(e.Message);
			}
			
			OutputHash(ht);

			Response.Write("Process");
			
			try
			{
				Hashtable hashProc = new Hashtable();
				Process[] procs = Process.GetProcesses();
				FillHash(hashProc);
				OutputHashHeader(hashProc);
				foreach(Process proc in procs)
				{
					proc.Refresh();
					TimeSpan cputime = proc.TotalProcessorTime;

					HandleHash(CPUTime, cputime,hashProc);
					HandleHash(Name, proc.ProcessName,hashProc);
					HandleHash(PIDTime, proc.Id,hashProc);
					/*HandleHash(Time, String.Format(
						"{0}:{1}:{2}", 
						((cputime.TotalHours-1<0?0:cputime.TotalHours-1)).ToString("##0"), 
						cputime.Minutes.ToString("00"), 
						cputime.Seconds.ToString("00")
						),hashProc);*/
					HandleHash(Memory, String.Concat((proc.WorkingSet/1024).ToString(), " kb"), hashProc);
					HandleHash(VirtualMemory, String.Concat((proc.PeakWorkingSet/1024).ToString()," kb"), hashProc);
					HandleHash(Hand, proc.HandleCount, hashProc);
					HandleHash(Thread, proc.Threads.Count, hashProc);
					OutputHashRow(hashProc);	
				}
				OutputHashFooter(hashProc);
			}catch(Exception ex)
			{
				Response.Write(String.Format("Error: {0}",ex.Message));
			}
		}
		
		private void HandleHash(object key,object keyValue,Hashtable hashGeneric )
		{
			if (hashGeneric == null)
			{
				hashGeneric = new Hashtable();
			}

			if (!hashGeneric.Contains(key))
			{
				hashGeneric.Add(key,keyValue);
			}
			else
			{
				hashGeneric[key] = keyValue;
			}
		}

		private void FillHash(Hashtable htGeneric)
		{
			htGeneric.Add(CPUTime,null);
			htGeneric.Add(Name,null);
			htGeneric.Add(PIDTime,null);
			//htGeneric.Add(Time,null);
			htGeneric.Add(Memory,null);
			htGeneric.Add(VirtualMemory,null);
			htGeneric.Add(Hand,null);
			htGeneric.Add(Thread,null);
		}
		private void OutputHash(Hashtable hashGeneric)
		{
			Response.Write("<TABLE style=\"border: 1px solid black;\">");
			foreach (DictionaryEntry t in hashGeneric)
			{
				Response.Write("<TR>");
				Response.Write("<TD>");
				Response.Write(t.Key.ToString());
				Response.Write("</TD>");
				Response.Write("<TD>");
				Response.Write(t.Value.ToString());
				Response.Write("</TD>");
			}
			Response.Write("</TABLE>");
		}

		private void OutputHashHeader(Hashtable htGeneric)
		{
			Response.Write("<TABLE style=\"border: 1px solid black;\">");
			Response.Write("<tr>");
			foreach (DictionaryEntry dc in htGeneric)
			{
				Response.Write(String.Format("<td style=\"border: 1px solid black;\">{0}</td>",dc.Key));
			}
			Response.Write("</tr>");
		}

		private void OutputHashRow(Hashtable htGeneric)
		{
			Response.Write("<tr>");
			foreach (DictionaryEntry dc in htGeneric)
			{
				Response.Write(String.Format("<td style=\"border: 1px solid black;\">{0}</td>",dc.Value));
			}
			Response.Write("</tr>");
		}

		private void OutputHashFooter(Hashtable htGeneric)
		{
			Response.Write("</table>");
		}
		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}

		#endregion
	}
}