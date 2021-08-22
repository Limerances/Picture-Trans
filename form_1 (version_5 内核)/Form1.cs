using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace form_1__version_5_内核_
{
    public partial class Form1 : Form
    {
        string path_read = null;
        string path_save = null;
        int thread_num = 0;
        int figure = 0;
        bool Onmission = true;
        Thread th_now;
        public Form1()
        {
            InitializeComponent();
        }

        private void button_pathread_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = @"选择输入文件夹";
            fbd.ShowNewFolderButton = true;
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                path_read = fbd.SelectedPath;
                textBox_pathread.Text = path_read;
            }
            else
                MessageBox.Show("请选择合适的文件夹");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = @"选择输出文件夹";
            fbd.ShowNewFolderButton = true;
            fbd.ShowDialog();
            if (fbd.SelectedPath != "")
            {
                path_save = fbd.SelectedPath;
                textBox_pathsave.Text = path_save;
            }
            else
                MessageBox.Show("请选择合适的文件夹");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(textBox_size.Text);
            if (num >= 1 && num <= 200)
            {
                figure = num;
                textBox_size2.Text = figure.ToString();
            }
            else
                MessageBox.Show("请输入合理的字体大小");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int num = Convert.ToInt32(textBox2.Text);
            if (num >= 1 && num <= 8)
            {
                thread_num = num;
                textBox1.Text = thread_num.ToString();
            }
            else
                MessageBox.Show("请输入合理的线程数（不超过8）");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int contrast;
            if(path_read == null || path_save == null || thread_num == 0 || figure == 0)
            {
                MessageBox.Show("请填写所有信息");
                return;
            }
            if (radioButton1.Checked == true)
                contrast = 1;
            else if (radioButton2.Checked == true)
                contrast = 2;
            else if (radioButton3.Checked == true)
                contrast = 3;
            else
            {
                MessageBox.Show("请填写所有信息");
                return;
            }
            if (Onmission == false)
            {
                MessageBox.Show("等待任务执行完成后继续下一个任务");
                return;
            }
            Onmission = false;
            Tran tran = new Tran(path_read, path_save, thread_num, figure, contrast,textBox_watch,textBox_pro);
            Thread th = new Thread(tran.Action);
            th_now = th;
            th.IsBackground = true;
            th.Start();

            Thread check_th = new Thread(Check);
            check_th.IsBackground = true;
            check_th.Start(th);
        }

        public void Check(Object o)
        {
            Thread th = o as Thread;
            while (th != null)
                Thread.Sleep(500);
            Onmission = true;
        }

        private void 切换自动换行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox_watch.WordWrap = (textBox_watch.WordWrap == false) ? true : false;
        }
    }
}
