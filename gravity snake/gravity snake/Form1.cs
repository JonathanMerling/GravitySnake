using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gravity_snake
{
    public partial class Form1 : Form
    {
        public static score[] scores = new score[0];
        public static Label[] vscores = new Label[10];
        public static string t;
        ball sh = new ball(new vector(325, 325), 40);
        vector gh = new vector(0, 0);
        Random rnand = new Random();
        vector[] food = new vector[1];
        int points = 0;
        Label poi = new Label();
        int add = 0;
        bool b = true;
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            sh.interval = timer1.Interval;
            for (int i = 0; i < food.Length; i++) create(i);
            poi.Left = 651;
            poi.Top = 0;
            poi.Text = "points:0";
            this.Controls.Add(poi);
            poi.Height = 13;
            for (int i = 0; i < 10; i++)
            {
                vscores[i] = new Label();
                vscores[i].Top = 13 * (i + 1);
                vscores[i].Left = 651;
                vscores[i].Height = 13;
                this.Controls.Add(vscores[i]);
            }
            for (int i = 0; i < Math.Min(10, scores.Length); i++) vscores[i].Text = i + "-" + scores[i].player + " : " + scores[i].points + " points";
        }
        void create(int i)
        {
            food[i] = new Point(rnand.Next(50, 600), rnand.Next(50, 600));
            for (int j = 0; j < food.Length; j++)
                if (j != i)
                    if ((food[i] - food[j]).length < 20)
                    {
                        create(i);
                        return;
                    }
            int[] s = sh.size;
            for (int j = 0; j < sh.tail.Length; j++) if ((food[i] - sh.tail[j]).length < s[j] + 10)
                {
                    create(i);
                }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            gh = e.Location;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            b ^= true;
            Invalidate();
            if (Math.Min(gh.x, gh.y) < 5 || Math.Max(gh.x, gh.y) > 645) return;
            sh.move(gh);
            for (int i = 0; i < food.Length; i++) if ((food[i] - sh.tail[sh.tail.Length - 1]).length < sh.endsize + 10)
                {
                    add += 10;
                    create(i);
                    points++;
                    poi.Text = "points:" + points;
                }
            if (add > 0 && b)
            {
                add--;
                sh.add = true;
            }
            Invalidate();
            if (sh.GameEnd)
            {
                timer1.Enabled = false;
                MessageBox.Show("game over");
                sh = new ball(new vector(325, 325), 40);
                sh.interval = timer1.Interval;
                score[] tmp = scores;
                scores = new score[scores.Length + 1];
                for (int i = 0; i < tmp.Length; i++) scores[i] = tmp[i];
                scores[tmp.Length] = new score(points);
                points = 0;
                poi.Text = "points:0";
                add = 0;
                sort.sort2(scores);
                for (int i = 0; i < Math.Min(10, scores.Length); i++) vscores[i].Text = i + "-" + scores[i].player + " : " + scores[i].points + " points";
                t = vscores[0].Text;
                timer1.Enabled = true;
                try
                {
                    gh = MousePosition;
                }
                catch
                {
                    gh = new vector(0, 0);
                }
            }
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                Point[] p = { new Point(i, i), new Point(i, 650 - i), new Point(650 - i, 650 - i), new Point(650 - i, i), new Point(i, i) };
                e.Graphics.DrawLines(new Pen(Color.Black), p);
            }
            sh.paint(e.Graphics);
            foreach (vector fed in food) e.Graphics.FillEllipse(new SolidBrush(Color.Salmon), new Rectangle((Point)(fed - new Point(10, 10)), new Size(20, 20)));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(0, 0);
        }
    }
    public struct vector
    {
        public double x, y;
        public vector(double x, double y)
        {
            this.x = x;
            this.y = y;
            
        }
        public double length
        {
            get
            {
                return Math.Sqrt(x * x + y * y);
            }
            set
            {
                double n = value / length;
                x *= n;
                y *= n;
            }
        }
        public static vector operator +(vector a, vector b)
        {
            return new vector(a.x + b.x, a.y + b.y);
        }
        public static vector operator *(vector a, double b)
        {
            return new vector(a.x * b, a.y * b);
        }
        public static vector operator *(double a, vector b)
        {
            return b * a;
        }
        public static double operator *(vector a, vector b)
        {
            return a.x * b.x + a.y * b.y;
        }
        public static vector operator -(vector a)
        {
            return -1 * a;
        }
        public static vector operator -(vector a, vector b)
        {
            return a + -b;
        }
        public static implicit operator vector(Point a)
        {
            return new vector(a.X, a.Y);
        }
        public static explicit operator Point(vector a)
        {
            return new Point((int)a.x, (int)a.y);
        }
        public static vector operator /(vector a, double b)
        {
            return a * (1 / b);
        }
        public double argument
        {
            get
            {
                double arr;
                if (x == 0)
                {
                    if (y != 0) arr = Math.PI / 2;
                    else arr = 0;
                }
                else
                {
                    arr = Math.Atan(y / x);
                }
                if (x < 0) arr += Math.PI;
                arr *= 180 / Math.PI;
                return arr;
            }
            set
            {
                double r = length;
                double t = value / 180 * Math.PI;
                x = r * Math.Cos(t);
                y = r * Math.Sin(t);
            }
        }
    }
    public class ball
    {
        vector velocity;
        public vector[] tail = new vector[120];
        public double gravity = 70.57;
        public bool add = false;
        public const int cleng = 10, clen = 60;
        public ball(vector sp)
        {
            for (int i = 0; i < tail.Length; i++) tail[i] = sp;
            velocity = new vector(0, 0);
        }
        public void move(vector gh)
        {
            if (add)
            {
                vector[] ntail = new vector[tail.Length + 1];
                for (int i = 0; i < tail.Length; i++) ntail[i] = tail[i];
                ntail[tail.Length] = tail[tail.Length - 1];
                tail = ntail;
                add = false;
            }
            else
            {
                for (int i = 0; i < tail.Length - 1; i++) tail[i] = tail[i + 1];
            }
            int end = interval * 100;
            for (int i = 0; i < end; i++)
            {
                vector a = gh - tail[tail.Length - 1];
                a.length = gravity * Math.Pow(Math.Log(tail.Length), 1.5);
                tail[tail.Length - 1] += (velocity + a / 200000) / 100000;
                velocity += a / 100000;
            }
        }
        public void paint(Graphics a)
        {
            Brush bru = new SolidBrush(Color.Green);
            int[] si = size;
            for (int i = 0; i < tail.Length; i++)
            {
                vector move = new vector(si[i], si[i]);
                if (((int)(tail.Length - i - 1) / 10) % 2 == 0)
                {
                    bru = new SolidBrush(Color.Green);
                }
                else
                {
                    bru = new SolidBrush(Color.SaddleBrown);
                }
                a.FillEllipse(bru, new Rectangle((Point)(tail[i] - move), new Size((Point)(move * 2))));
            }
            vector dir = velocity;
            if (velocity.x == 0 && velocity.y == 0)
            {
                dir.x = 1;
            }
            vector m = new vector(endsize * 5 / 6, endsize * 5 / 6);
            dir.length = endsize / 2;
            a.DrawArc(new Pen(Color.Black), new Rectangle((Point)(tail[tail.Length - 1] - m), new Size((Point)(m * 2))), (float)dir.argument - 50, (float)100);
            Point a1 = (Point)(tail[tail.Length - 1] + dir);
            dir.length = endsize * 2 / 3;
            dir.argument -= 5;
            Point a2 = (Point)(tail[tail.Length - 1] + dir);
            dir.argument += 10;
            Point a3 = (Point)(tail[tail.Length - 1] + dir);
            Point[] b = { a1, a2, a3, a1 };
            a.DrawLines(new Pen(Color.Black), b);
            dir.length = endsize / 3;
            dir.argument += 15;
            vector d = new vector(2, 2);
            Size elps = new Size((Point)(2 * d));
            a.FillEllipse(new SolidBrush(Color.Black), new Rectangle((Point)(tail[tail.Length - 1] + dir - d), elps));
            dir.argument -= 40;
            a.FillEllipse(new SolidBrush(Color.Black), new Rectangle((Point)(tail[tail.Length - 1] + dir - d), elps));
        }
        public ball(vector sp, int lenght)
        {
            tail = new vector[lenght];
            for (int i = 0; i < tail.Length; i++) tail[i] = sp;
            velocity = new vector(0, 0);
        }
        bool[] checkl
        {
            get
            {
                bool[] re = new bool[tail.Length];
                int s = endsize - startsize;
                int[] si = size;
                for (int i = 0; i < re.Length; i++)
                {
                    vector a = tail[tail.Length - 1] - tail[i];
                    re[i] = a.length < endsize + si[i];
                }
                return re;
            }
        }
        bool[] checl
        {
            get
            {
                bool[] re = new bool[tail.Length];
                int s = endsize - startsize;
                int[] si = size;
                for (int i = 0; i < re.Length; i++)
                {
                    vector a = tail[tail.Length - 1] - tail[i];
                    re[i] = a.length < endsize + si[i]-cleng;
                }
                return re;
            }
        }
        public bool GameEnd
        {
            get
            {
                bool[] on = checkl, on1 = checl;
                for (int i = 0; i < on.Length; i++) if (on1[i]) for (int j = i; j < on.Length; j++) if (!on[j]) return true;
                for (int i = 0; i < on.Length - clen; i++) if (on[i])
                    {
                        bool b = true;
                        for (int j = 1; j < clen - 1; j++)
                        {
                            b &= !on[i + j];
                        }
                        if (b) return true;
                    }
                Point head = (Point)tail[tail.Length - 1];
                if (head.X < 5 + endsize || head.Y < 5 + endsize || head.X > 645 - endsize || head.Y > 645 - endsize) return true;
                return false;
            }
        }
        public int startsize = 10, endsize = 30, interval = 100;
        public int[] size
        {
            get
            {
                int[] re = new int[tail.Length];
                int s = endsize - startsize;
                for (int i = 0; i < re.Length; i++) re[i] = i * s / (tail.Length - 1) + startsize;
                return re;
            }
        }
    }
    public class score
    {
        public static string user = "usr";
        public string player;
        public int points;
        public score(int points)
        {
            player = user;
            this.points = points;
        }
    }
    public static class sort
    {
        public static void sorting(score[] arr)
        {
            for (int i = 1; i < arr.Length; i += i)
            {
                for (int j = 0; j + i < arr.Length; j += i * 2)
                {
                    int s0 = j;
                    int s1 = j + i;
                    int e0 = s1;
                    int e1 = Math.Min(j + i * 2, arr.Length);
                    score[] tmp = new score[Math.Min(i * 2, arr.Length - j)];
                    for (int i1 = 0; i1 < tmp.Length; i1++)
                    {
                        if (s0 == e0)
                        {
                            tmp[i1] = arr[s1];
                            s1++;
                            continue;
                        }
                        if (s1 == e1)
                        {
                            tmp[i1] = arr[s0];
                            s0++;
                            continue;
                        }
                        if (arr[s0].points > arr[s1].points)
                        {
                            tmp[i1] = arr[s0];
                            s0++;
                        }
                        else
                        {
                            tmp[i1] = arr[s1];
                            s1++;
                        }
                    }
                    for (int i1 = 0; i1 < tmp.Length; i1++) arr[i1 + j] = tmp[i1];
                }
            }
        }
        public static void sort2(score[] arr)
        {
            for (int i = arr.Length - 1; i > 0 && arr[i].points > arr[i - 1].points; i--)
            {
                score tmp = arr[i];
                arr[i] = arr[i - 1];
                arr[i - 1] = tmp;
            }
        }
    }
}
