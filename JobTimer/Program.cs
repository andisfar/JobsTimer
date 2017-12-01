using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JobTimer
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            System.Diagnostics.Debug.Print("Starting application: " + Application.ProductName);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadExit += Application_ThreadExit;

            var mutex = new System.Threading.Mutex(true, Application.ProductName, out bool result);
            if (!result)
            {
                JobTimerForm.GetInstance.jobTimersIcon.Visible = true;
                MessageBox.Show(JobTimerForm.GetInstance, "The requested application is already running!\nIf you don't see it, check notification area!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                JobTimerForm.GetInstance.jobTimersIcon.Visible = false;
                Application.Exit();
            }
            else
            {
                try
                {
                    Application.Run(JobTimerForm.GetInstance);
                }
                catch (JobTimer.TimersFunctionNotImplemented jtEx)
                {
                    MessageBox.Show(jtEx.Message);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        private static void Application_ThreadExit(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Exiting the " + Application.ProductName + " thread!");
        }
    }
}
