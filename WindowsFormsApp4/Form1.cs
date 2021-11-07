using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public static Data dataPoints = new Data();
        public static Graphics gra;
        public Size chartSize= new Size();
        public Point pointStart= new Point();
        public static int sizerY;
        public static int sizerX;
        public static int spacer;
        public Random r = new Random();
        public List<Color> colors = new List<Color>();
        public Form1()
        {
            InitializeComponent();
            gra = this.CreateGraphics();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Proizvod 1,Proizvod 2,Proizvod 3,Proizvod 4,Proizvod 5,Proizvod 6,Proizvod 7,Proizvod 8,Proizvod 9";
            textBox2.Text = "12,15|15,12|13,15|12,15|15,110|13,15|12,15|15,12|13,15";
            textBox3.Text = "Moj grafik";
            textBox4.Text = "Proizvodi";
            textBox5.Text = "Cene";
            chartSize = new Size(800, 300);
            pointStart = new Point(60, 50);
            dataPoints = GetData(textBox2.Text, textBox1.Text);
            Draw(gra);
        }

        public void DrawChart(Graphics g,Point poc,Size size,Data dataPoints,string chartName,string Xname,string Yname,int sizerX,int sizerY,int distancer)
        {
            g.Clear(this.BackColor);

            Font font = new Font(FontFamily.GenericSansSerif, 8);
            Thread tr1 = new Thread(()=>g.DrawRectangle(new Pen(Color.Black,2),new Rectangle(poc,size)));
            Thread tr2= new Thread(() => g.FillRectangle(Brushes.Cornsilk, new Rectangle(poc, size)));
            Thread tr3 = new Thread(() => g.DrawString(chartName, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, new Point(poc.X +(int)(size.Width/2-chartName.Length*6), poc.Y - 25)));
            Thread tr4 = new Thread(() => g.DrawString(Xname, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, new Point(poc.X + (int)(size.Width / 2 - chartName.Length * 6), poc.Y + size.Height + 10)));
            Thread tr5 = new Thread(() => g.DrawString(Yname, new Font(FontFamily.GenericSansSerif, 12), Brushes.Black, new Rectangle(new Point(poc.X - poc.X+10, (int)(poc.Y + size.Height / 2) - 70), new Size(5, 150)), StringFormat.GenericTypographic));
            Thread tr6 = new Thread(() => Novo());
            tr1.Start(); Thread.Sleep(10); tr1.Abort();
            tr2.Start(); Thread.Sleep(10); tr2.Abort();
            tr3.Start(); Thread.Sleep(10); tr3.Abort();
            tr4.Start(); Thread.Sleep(10); tr4.Abort();
            tr5.Start(); Thread.Sleep(10); tr5.Abort();
            tr6.Start(); Thread.Sleep(30); tr6.Abort();

            void Novo()
            {
                int min = dataPoints.GetInterval();
                for(int i = dataPoints.GetInterval(); i < size.Height; i += dataPoints.GetInterval())
                {
                    if (i * sizerY > size.Height)
                    {
                        break;
                    }
                    g.DrawLine(new Pen(Color.LightGray, 0.5f), new Point(poc.X,poc.Y+ (size.Height - i * sizerY)), new Point(poc.X+size.Width-1, poc.Y + (size.Height - i * sizerY)));
                    g.DrawString(min.ToString(), font,Brushes.Black, new Point(poc.X - 30, poc.Y -5 +(size.Height-i * sizerY)));
                    min += dataPoints.GetInterval();
                }
              
            }


            int poz = poc.X + distancer;
            for (int i = 0; i < dataPoints.points.Count; i++)
            {
                g.FillRectangle(new SolidBrush(colors[i]), new Rectangle(new Point(poz, (poc.Y + size.Height) - dataPoints.points[i].Item2 * sizerY), new Size(dataPoints.points[i].Item1 * sizerX, dataPoints.points[i].Item2 * sizerY)));
                poz += distancer + dataPoints.points[i].Item1 * sizerX;
            }
            poz = poc.Y;
            int pozx = 10;
            for(int i = 0; i < dataPoints.names.Count; i++)
            {
                if (poz >= size.Height)
                {
                    poz = poc.Y;
                    pozx += dataPoints.names.Max().Length + 100;
                }
                g.DrawString(dataPoints.names[i], font, Brushes.Black, new Rectangle(new Point(poc.X + size.Width + pozx+20, poz),new Size(dataPoints.names[i].Length+50,15)), StringFormat.GenericTypographic);
                g.FillRectangle(new SolidBrush(colors[i]), new Rectangle(new Point(poc.X + size.Width + pozx, poz), new Size(15, 15)));
                poz += 25;
            }


        }
        public void Draw(Graphics gra)
        {
            sizerY = 10;
            sizerX = 10;
            spacer = 30;
            
            while (dataPoints.getMaxY()*sizerY > chartSize.Height)
            {
                sizerY--;
                if (sizerY == 0)
                {
                    MessageBox.Show("Vrednost za Y prelazi velicinu grafika!S");
                    return;
                }
            }
            while (dataPoints.getSumX() * sizerX + spacer * dataPoints.points.Count > chartSize.Width)
            {
                sizerX--;

                if (sizerX == 0)
                {
                    MessageBox.Show("Nemoguce nacrtati toliko stavki molim povecajte velicinu grafika!");
                    return;
                }
            }
            while (dataPoints.getSumX()*sizerX + spacer * dataPoints.points.Count > chartSize.Width)
            {
                spacer--;
              
                if(spacer==0)
                {
                    MessageBox.Show("Nemoguce nacrtati toliko stavki molim povecajte velicinu grafika!");
                    return;
                }
            }
            
            
            DrawChart(gra, pointStart, chartSize, dataPoints, textBox3.Text, textBox4.Text, textBox5.Text, sizerX,sizerY, spacer);
        }

        public class Data
        {
            public List<Tuple<int,int>> points { get; set; }
            public List<string> names { get; set; }
            public Data()
            {
                points = new List<Tuple<int, int>>();
                names = new List<string>();
            }
            public Data(List<Tuple<int,int>> xy,List<string> names)
            {
                this.names = names;
                this.points = xy;
            }
            public int getSumX()
            {
                int a = 0;
                foreach(var x in points)
                {
                    a += x.Item1;
                }
                return a;
            }
            public int getMaxY()
            {
                int a = 0;
                foreach (var x in points)
                {
                    if (x.Item2 > a)
                    {
                        a = x.Item2;
                    }
                }
                return a;
            }
            public int GetInterval()
            {
                int a = points[0].Item2;
                foreach (var x in points)
                {
                    if (x.Item2 < a)
                    {
                        a = x.Item2;
                    }
                }
                return  a;
            }
        }
        public Data GetData(string s, string name)
        {
            Data temp = new Data();
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox1.Text))
            {
                return temp;
            }
            string[] colums = s.Split('|');
            foreach(string m in colums)
            {
                if (!string.IsNullOrEmpty(m))
                {
                    if (m.Split(',').Length > 2)
                    {
                        MessageBox.Show("Pogresan format podatak molim ispravite!");
                        textBox2.Text = "";
                        return new Data();
                    }
                    temp.points.Add(new Tuple<int, int>(int.Parse(m.Split(',')[0]), int.Parse(m.Split(',')[1])));
                }
                
                
            }
            foreach(string m in name.Split(','))
            {
                if (!string.IsNullOrEmpty(m))
                {
                    temp.names.Add(m);
                }
            }
            for(int i = 0; i < temp.points.Count; i++)
            {
                colors.Add(Color.FromArgb(r.Next(256), r.Next(256), r.Next(256)));
            }
            return temp;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
                dataPoints = GetData(textBox2.Text, textBox1.Text);
                Draw(e.Graphics);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataPoints = GetData(textBox2.Text,textBox1.Text);
            Draw(gra);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
         
            gra.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Proizvod 1,Proizvod 2,Proizvod 3,Proizvod 4,Proizvod 5,Proizvod 6,Proizvod 7,Proizvod 8,Proizvod 9";
            textBox2.Text = "12,15|15,12|13,15|12,15|15,110|13,15|12,15|15,12|13,15";
            dataPoints = GetData(textBox2.Text, textBox1.Text);
            Draw(gra);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Proizvod 1,Proizvod 2,Proizvod 3,Proizvod 4,Proizvod 5,Proizvod 6,Proizvod 7,Proizvod 8,Proizvod 9,Proizvod 10,Proizvod 11,Proizvod 12,Proizvod 13,Proizvod 14,Proizvod 15,Proizvod 16,Proizvod 17,Proizvod 18";
            textBox2.Text = "12,15|15,12|13,15|12,15|15,110|13,15|12,15|15,12|13,15|12,15|15,12|13,15|12,15|15,110|13,15|12,15|15,12|13,15";
            dataPoints = GetData(textBox2.Text, textBox1.Text);
            Draw(gra);
        }
    }
}
