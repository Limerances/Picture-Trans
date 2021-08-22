using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace form_1__version_5_内核_
{
    class Package
    {
        public string[] files;
        public int head;
        public int sore;
        public char[] chs;
        public double[] rate;
        public string path_save;

        public void trans()
        {
            Action<string> del = (string str) => { Tran.TextBox_watch.AppendText(str + "\r\n"); };
            Action<string,string> del_2 = (string str_1, string str_2) => { Tran.TextBox_pro.Text = str_1 + "/" + str_2; };
            int i;
            for (i = head; i < sore; ++i)
            {
                Bitmap bmp_manege = new Bitmap(files[i]);
                Bitmap receive = Tran.Manege(chs, rate, bmp_manege, i + 1);
                receive.Save(path_save + @"/" + Path.GetFileName(files[i]));
                Tran.TextBox_watch.Invoke(del, Path.GetFileNameWithoutExtension(files[i]) + " Finish!");
                Tran.TextBox_pro.Invoke(del_2, (Tran.pos + 1).ToString(), files.Length.ToString());
                Console.WriteLine("1");
                Tran.pos += 1;
            }
            Tran.end += 1;
        }
    }
}
