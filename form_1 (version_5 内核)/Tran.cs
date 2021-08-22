using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace form_1__version_5_内核_
{
    class Tran
    {
        public static int pos = 0;
        public static int end = 0;
        public static int All_Frame = 0;
        public static int delta_w;
        public static int delta_h;
        public string path_read = null;
        public string path_save = null;
        public int thread_num = 0;
        public static int figure = 0;
        public static int contrast;
        public static TextBox TextBox_watch;
        public static TextBox TextBox_pro;

        public Tran(string path_read, string path_save, int thread_num, int figure,int contrast,TextBox TextBox_watch, TextBox TextBox_pro)
        {
            this.path_read = path_read;
            this.path_save = path_save;
            this.thread_num = thread_num;
            Tran.figure = figure;
            Tran.contrast = contrast;
            Tran.TextBox_watch = TextBox_watch;
            Tran.TextBox_pro = TextBox_pro;
        }
        public void Action()
        {
            Action<string> del = (string str) => { TextBox_watch.AppendText(str + "\r\n"); };
            Stopwatch wa = new Stopwatch();
            wa.Start();
            int i;
            char[] chs = new char[] { ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}' };
            double[] rate = new double[chs.Length];

            Bitmap bmp = new Bitmap(614, 885);
            Graphics g = Graphics.FromImage(bmp);
            Font ft = new Font("Consolas", 512, FontStyle.Regular);
            //g.Clear(Color.Black); //测试在“字体大小记录”中求得的bitmap大小是否使得字体处在中间
            //g.DrawString("龍", ft, Brushes.White, new Point(0, 0));
            //bmp.Save(@"C:\Users\Limerance\Desktop\haha.jpg");
            for (i = 0; i < chs.Length; ++i)
            {
                g.Clear(Color.Black);
                g.DrawString(chs[i].ToString(), ft, Brushes.White, new Point(0, 0));
                rate[i] = getrate(bmp);
                //Console.WriteLine(chs[i] + "   " + rate[i]);
            }

            standar(ref chs, ref rate);
            set_figure_size();
            //for (i = 0; i < chs.Length; ++i)
            //    Console.WriteLine(chs[i] + "   " + rate[i]);
            //Console.ReadKey();

            /////////////////////////////////////////////////////////////////////////////////预处理分界线
            string[] files = Directory.GetFiles(path_read, "*.jpg", SearchOption.TopDirectoryOnly);

            All_Frame = files.Length;

            
            for (i = 0; i < thread_num; ++i)
            {
                Package pack = new Package();
                pack.chs = chs;
                pack.rate = rate;
                pack.path_save = path_save;
                pack.files = files;
                pack.head = files.Length * i / thread_num;
                pack.sore = files.Length * (i + 1) / thread_num;
                Thread th = new Thread(new ThreadStart(pack.trans));
                th.IsBackground = true;
                th.Start();
            }
            while (end != thread_num) { Thread.Sleep(200); }

            wa.Stop();
            TextBox_watch.Invoke(del, wa.Elapsed.ToString());
        }

        public static Bitmap Manege(char[] chs, double[] rate, Bitmap bmp, int order)
        {
            int i, j;
            int height = bmp.Height;
            int width = bmp.Width;
            Bitmap newmap = new Bitmap(width, height + (int)(height * 0.05), PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(newmap);
            g.Clear(Color.Black);

            Font ft = new Font("Consolas",figure , FontStyle.Regular);////////////////////////////////////////////////
            //int delta_w = 14, delta_h = 21;//////////////////////////////////////////////////////////////
            int num_w = width / delta_w, num_h = height / delta_h;

            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int len = height * bd.Stride;
            byte[] buffer = new byte[len];
            IntPtr ptr = bd.Scan0;
            Marshal.Copy(ptr, buffer, 0, len);

            for (j = 0; j < num_h; ++j)
            {
                for (i = 0; i < num_w; ++i)
                {
                    int grey = GetMapGrey(buffer, i, j, delta_w, delta_h, bd.Stride);
                    char c = fitchar(chs, rate, grey);
                    if (i != num_w - 1)
                        g.DrawString(c.ToString(), ft, Brushes.White, new Point(i * delta_w, j * delta_h));
                    else
                        g.DrawString(c.ToString() + "\r\n", ft, Brushes.White, new Point(i * delta_w, j * delta_h));
                }
            }
            //int x = (int)(order * width * 20 / (All_Frame * 23));
            int x = width / 20;
            int y = height + (int)(0.01 * height);
            g.DrawString("Frame: " + order + "/" + All_Frame, new Font("Consolas", 20, FontStyle.Regular), Brushes.White, new Point(x, y));
            bmp.UnlockBits(bd);


            return newmap;
        }

        public static int GetMapGrey(byte[] buffer, int i, int j, int delta_w, int delta_h, int stride)
        {
            int x, y;
            long all = 0;
            for (y = j * delta_h; y < (j + 1) * delta_h; ++y)
            {
                for (x = i * delta_w; x < (i + 1) * delta_w; ++x)
                {
                    all += (long)(buffer[y * stride + x * 3 + 2] * 0.3 + buffer[y * stride + x * 3 + 1] * 0.59 + buffer[y * stride + x * 3] * 0.11);
                }
            }
            all /= (delta_h * delta_w);
            return (int)all;
        }
        public static char fitchar(char[] chs, double[] rate, int grey)
        {
            if (contrast == 2)
                grey = (Enhance(grey) + grey) / 2;////////////////////////////////////////////////////////////////////////////
            else if (contrast == 3)
                grey = Enhance(grey);
            double val = 1 - (double)grey / (double)255;
            int i = 0;
            for (i = 1; i < rate.Length; ++i)
            {
                if (val > rate[i])
                    break;
            }
            return chs[i - 1];
        }

        public static int Enhance(int grey)
        {
            double x = (double)grey;
            if (x < 127.5)
                x = 127.5 - Math.Sqrt(127.5 * 127.5 - x * x);
            else
                x = 127.5 + Math.Sqrt(127.5 * 127.5 - (255 - x) * (255 - x));
            return (int)x;
        }
        public static void standar(ref char[] chs, ref double[] rate)
        {
            Array.Sort(rate, chs);
            Array.Reverse(rate);
            Array.Reverse(chs);
            double mult = 1 / (rate[0] - rate[rate.Length - 1]);
            for (int i = 0; i < rate.Length; ++i)
            {
                rate[i] = 1 - (1 - rate[i]) * mult;
            }
        }

        public void set_figure_size()
        {
            Bitmap bmp = new Bitmap(2000, 100);
            Graphics g = Graphics.FromImage(bmp);
            Font ft = new Font("Consolas", figure, FontStyle.Regular);
            g.DrawString("A", ft, Brushes.White, new Point(0, 0));
            delta_w = (int)Math.Round(g.MeasureString("A", ft).Width);
            delta_h = (int)Math.Round(g.MeasureString("A", ft).Height);
        }
        public static double getrate(Bitmap bmp)
        {
            int height = bmp.Height;
            int width = bmp.Width;
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            //Console.WriteLine(bmp.PixelFormat);
            int len = 3 * height * width;
            byte[] buffer = new byte[len];
            IntPtr ptr = bd.Scan0;
            Marshal.Copy(ptr, buffer, 0, len);

            int i = 0;
            byte ave;
            int count = 0;
            for (i = 0; i < len; i += 3)
            {
                ave = (byte)(buffer[i] * 0.11 + buffer[i + 1] * 0.59 + buffer[i + 2] * 0.3);
                if (ave < 128)
                    count += 1;
            }
            Marshal.Copy(buffer, 0, ptr, len);
            bmp.UnlockBits(bd);

            return 3 * (double)count / (double)len;
        }
    }
}
