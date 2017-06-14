using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KillProcess
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string needKillPortStr = this.needKillPort.Text;
            if(needKillPortStr.Trim().Length==0)
            {
                return;
            }
            int port;
            if (this.isNumberic(needKillPortStr, out port))
            {
                LookAndStop(port);
                MessageBox.Show("操作完成");
            }
            else
            {
                MessageBox.Show("端口必须是数字！");
            }
        }

        //根据端口号，查找该端口所在的进程，并结束该进程
        private void LookAndStop(int port)
        {
            Process pro = new Process();
            // 设置命令行、参数    
            pro.StartInfo.FileName = "cmd.exe";
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.CreateNoWindow = true;
            // 启动CMD    
            pro.Start();
            // 运行端口检查命令    
            pro.StandardInput.WriteLine("netstat -ano");
            pro.StandardInput.WriteLine("exit");
            // 获取结果    
            Regex reg = new Regex("\\s+", RegexOptions.Compiled);
            string line = null;
            string endStr = ":" + Convert.ToString(port);
            while ((line = pro.StandardOutput.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.StartsWith("TCP", StringComparison.OrdinalIgnoreCase))
                {
                    line = reg.Replace(line, ",");
                    string[] arr = line.Split(',');
                    if (arr[1].EndsWith(endStr))
                    {
                        //Console.WriteLine("4001端口的进程ID：{0}", arr[4]);
                        int pid = Int32.Parse(arr[4]);
                        Process pro80 = Process.GetProcessById(pid);
                        // 处理该进程
                        pro80.Kill();
                        break;
                    }
                }
                else if (line.StartsWith("UDP", StringComparison.OrdinalIgnoreCase))
                {
                    line = reg.Replace(line, ",");
                    string[] arr = line.Split(',');
                    if (arr[1].EndsWith(endStr))
                    {
                        //Console.WriteLine("4001端口的进程ID：{0}", arr[3]);
                        int pid = Int32.Parse(arr[3]);
                        Process pro80 = Process.GetProcessById(pid);
                        // 处理该进程
                        pro80.Kill();
                        break;
                    }
                }
            }
            pro.Close();
        }

        private void txtPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)//如果输入的是回车键  
            {
                this.btnOk_Click(sender, e);//触发button事件  
            }
        }

        private bool isNumberic(string message, out int result)
        {
            System.Text.RegularExpressions.Regex rex =
            new System.Text.RegularExpressions.Regex(@"^\d+$");
            result = -1;
            if (rex.IsMatch(message))
            {
                result = int.Parse(message);
                return true;
            }
            else
                return false;
        }
    }
}
