using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Newton
{
    public partial class Form1 : Form
    {
        private Button but_tennis;
        private Objets obj_tennis;
        private Environnement envi;
        private Point[] courbe = null;
        private int sizeFirstMenu; /*width of the first menu*/
        private Button restart;
        private Button launch;
        private Label infos;
        private Label graph;
        private int agrandissement;
        private bool ctrl;
        public Form1()
        {
            InitializeComponent();
            this.AutoScroll = true;
            Restart(1,9.8,14,30,1.8);
        }

        public void Restart(double masse, double gravity, double speed, double angle, double posY0)
        {
            ctrl = true;
            courbe = null;
            Controls.Clear();
            graph = buildLabel();
            envi=new Environnement(gravity,speed,angle);
            obj_tennis = new Objets(masse,posY0);
            sizeFirstMenu = 1000; 
            agrandissement = envi.Agrandissement(10,obj_tennis,graph);
            while (agrandissement==-1)
            {
                graph.Width = (int)(graph.Width*2);
                agrandissement = envi.Agrandissement(10,obj_tennis,graph);
            }
            while (agrandissement==-2)
            {
                graph.Height =(int)(graph.Height*1.2);
                agrandissement = envi.Agrandissement(10,obj_tennis,graph);
            }
            but_tennis = buttonGenerator(graph.Location.X,(int)(graph.Location.Y+(graph.Height)-agrandissement*obj_tennis.getPosY()));
            but_tennis.Size=new Size(10,10);

            infos = buildLabel();
            infos.Size = new Size((sizeFirstMenu-20)/3,60);
            infos.Location = new Point(10,10);
            infos.Text = "Mass: " + obj_tennis.getMasse() + "\nSpeed: " + envi.getVit0() + "\nAngle: " +
                         envi.getAngle() + "\nGravity: " + envi.getGravity();
            
            launch = buttonGenerator((20+(sizeFirstMenu-20)/3), 10);
            launch.Text = "Launch";
            
            restart = buttonGenerator(10+2*(10+(sizeFirstMenu-20)/3), 10);
            restart.Text = "Restart";
            
            but_tennis.MouseClick += new MouseEventHandler(ButtonPosTraject);
            launch.MouseClick+=new MouseEventHandler(Event_Launcher);
            
            restart.MouseClick += new MouseEventHandler(EventRestart);
            graph.Paint += buildAxes;

            graph.MouseClick += new MouseEventHandler(MousePos);
            this.Scroll += panel1_Scroll;
            
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(On_CTRL);
            this.KeyUp += new KeyEventHandler(Off_CTRL);


        }

        private void On_CTRL(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                if(ctrl)
                {
                    ctrl = false;
                    this.KeyDown += ZOOM;
                }
            }
                
        }
        private void Off_CTRL(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                if(!ctrl)
                {
                    ctrl = true;
                    this.KeyDown -= ZOOM;
                }
            }
                
        }
        private void ZOOM(object sender, KeyEventArgs e)
        {
            if(e.KeyCode==Keys.Down)
            {
                MessageBox.Show(e.ToString());
                removePoints(2);
            }
            
        }
        //Redraw if scroll
        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            /*calculer la pos de la fenetre dans la fenetre*/

            if (courbe != null)
            {
                int x = (Math.Abs(graph.Location.X)-10) / agrandissement;
                int y = x + (this.Width)/ agrandissement;
                if (x < 0)
                    x = 0;
                if (y >= courbe.Length)
                    y = courbe.Length - 1;
                if(x<=courbe.Length-1 && y<=courbe.Length-1)
                    print_Courbe(0,x,y);
            }
        }
        
        private void MousePos(object sender, MouseEventArgs e)
        {
            string res = "{ X: "+Math.Round((double)e.Location.X/agrandissement,1)+" m, Y: "+Math.Round((double)(graph.Height-e.Location.Y)/agrandissement,1)+" m }";
            MessageBox.Show(res);
        }
        private void ButtonPosTraject(object sender, MouseEventArgs e)
        {
            Button but = (Button) sender;
            string res = "{ X: "+(Math.Round((double)(but.Location.X-graph.Location.X)/agrandissement,1))+", Y: "
            +(Math.Round((double)(graph.Height-(but.Location.Y-graph.Location.Y))/agrandissement,1))+" }";
            if(Math.Abs((Math.Round((double)(graph.Height-(but.Location.Y-graph.Location.Y))/agrandissement,1)) 
                        - (Math.Round((double)(but.Height)/agrandissement,1))) < 0.1)
                res = "{ X: "+(Math.Round((double)(but.Location.X-graph.Location.X)/agrandissement,1))+", Y: "+0+" }";
            MessageBox.Show(res);
        }
        static double[] Solve(double a, double b, double c)
        {
            double delta = b * b - 4 * a * c;
            if (delta < 0)
            {
                // 0 solution : tableau vide
                return new double[0];
            }
            else if (delta == 0)
            {
                // 1 solution : tableau avec un seul élément
                return new double[] { -b / (2 * a) };
            }
            else // delta > 0
            {
                // 2 solutions : tableau avec 2 éléments
                return new double[]
                {
                    (-b - Math.Sqrt(delta)) / (2 * a),
                    (-b + Math.Sqrt(delta)) / (2 * a),
                };
            }
        }
        
        //Fonction qui divise la tialle d'une courbe par x
        private void removePoints(double x)
        {
            if(x!=0 && courbe!=null)
            {
                int pointsToRemove = (int) ((double) courbe.Length / x);
                int interval = courbe.Length / pointsToRemove;
                Point[] temp = new Point[courbe.Length - pointsToRemove];
                int c = 0;
                for (int i = 0; i < courbe.Length; i++)
                {
                    if (i % interval != 0)
                    {
                        temp[c] = new Point((int)(courbe[i].X/x),courbe[i].Y/2);
                        c++;
                    }
                }

                courbe = temp;
            }

        }
        private void buildAxes(object sender, PaintEventArgs e)
        {
            int agrand = agrandissement;
            double metreNombre = ((double) graph.Width / (double)agrand);
            double cste = 24 / metreNombre;
            agrand = (int)(Math.Round((double)agrand/cste));
            metreNombre = ((double) graph.Width / (double) agrand);
            double metre = (double)graph.Width/metreNombre;
            string unity = "1 chunk = "+agrand/metreNombre+"m";
            double mili = metre / 10;
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
            for (int i = 0; i < metreNombre; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    int x = (int)(agrand * i + mili * j);
                    if (j == 5)
                    {
                        e.Graphics.DrawLine(pen, x, graph.Height, x, graph.Height-6);
                        e.Graphics.DrawLine(pen, 0, x, 6, x);
                    }
                    else
                    {
                        e.Graphics.DrawLine(pen, x, graph.Height, x, graph.Height - 4);
                        e.Graphics.DrawLine(pen, 0, x, 4, x);
                    }
                }

                if (i != 0)
                {
                    e.Graphics.DrawLine(pen, 0, agrand * i, 10, agrand * i);
                    e.Graphics.DrawLine(pen, agrand * i, graph.Height, agrand * i, graph.Height-10);
                }
            }

        }
        
        

        private void EventRestart(object sender, MouseEventArgs e)
        {
            Form f2 = new DataRestart(this);
            f2.ShowDialog();
        }
        private void Event_Launcher(object sender, MouseEventArgs e)
        {
            int i = 0;
            MessageBox.Show((agrandissement+1)+"");
            courbe = new Point[graph.Width/agrandissement+1];
            while (obj_tennis.getPosY() > 0)
            {
                but_tennis.Location = new Point((int)(graph.Location.X+obj_tennis.getPosX()*agrandissement),(int)(graph.Location.Y+graph.Height-obj_tennis.getPosY()*agrandissement));
                courbe[i] = new Point(but_tennis.Location.X-graph.Location.X,but_tennis.Location.Y-graph.Location.Y);
                i++;
                obj_tennis.setPosX(obj_tennis.getPosX()+1);
                obj_tennis.setPosY(envi.CalculTrajectoire(obj_tennis,obj_tennis.getPosX()));
            }
            double lastX = Solve(- (envi.getGravity() / (2 * Math.Pow(envi.getVit0(),2)*Math.Pow(Math.Cos(envi.DegToRad(envi.getAngle())),2))), Math.Tan(envi.DegToRad(envi.getAngle())), obj_tennis.getPosY0())[0];
            obj_tennis.setPosX(lastX);
            but_tennis.Location=new Point((int)(graph.Location.X+obj_tennis.getPosX()*agrandissement),graph.Location.Y+graph.Height-but_tennis.Height);
            courbe[i] = new Point(but_tennis.Location.X-graph.Location.X,but_tennis.Location.Y-graph.Location.Y+but_tennis.Height);
            Array.Resize(ref courbe, i+1);
            if((this.Width-10)/agrandissement>courbe.Length)
                print_Courbe(1,0,courbe.Length-1);
            else
                print_Courbe(1,0,(this.Width-10)/agrandissement);
            launch.MouseClick -= Event_Launcher;
        }

        public void print_Courbe(int timePause, int i, int j)
        {
            Pen p = new Pen(Color.Black);
            Point[] tempCourbe=new Point[j-i+1];
            Array.Copy(courbe,i,tempCourbe,0,j-i+1);
            using (Graphics g = graph.CreateGraphics())
            {
                wait(timePause);
                g.DrawLines(p,tempCourbe);
            }
        }


        public Label buildLabel()
        {
            Label lab = new Label();
            lab.Size = new Size(1000,600);
            lab.AutoSize = false;
            lab.TextAlign = ContentAlignment.TopLeft;
            lab.BorderStyle = BorderStyle.FixedSingle;
            lab.Location=new Point(10,80);
            Controls.Add(lab);
            return lab;
        }
        public Button buttonGenerator(int x, int y)
        {
            var res= new Button();
            res.Height = 60;
            res.Width = (sizeFirstMenu -20)/3;
            res.TextAlign = ContentAlignment.MiddleCenter;
            res.Location=new Point(x,y);
            res.Visible = Enabled;
            this.Controls.Add(res);
            res.BringToFront();
            return res;
        }
        public void wait(int milliseconds)
        {
            System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;
            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();
            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };
            while (timer1.Enabled)
            {
                Application.DoEvents();
            }
        }
    }
}