using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Newton
{
    public partial class DataRestart : Form
    {
        private Dictionary<string, TextBox> formulaire;
        private Form1 form;
        public DataRestart(Form1 form)
        {
            InitializeComponent();
            formulaire = new Dictionary<string, TextBox>();
            this.form = form;
            dicoLine(100,70, "Mass:");
            dicoLine(100,120, "Speed:");
            dicoLine(100,170, "Gravity:");
            dicoLine(100,220, "Angle:");
            dicoLine(100,270, "Position 0:");
            Button send = new Button();
            send.Size = new Size(150,80);
            send.Location = new Point(490,70+(270+40-70-send.Height)/2);
            send.Text = "Confirm";
            this.Controls.Add(send);
            send.Visible = Enabled;
            send.MouseClick += new MouseEventHandler(confirmSelect);
        }

        private void confirmSelect(object sender, MouseEventArgs e)
        {
            foreach (var val in formulaire.Values)
            {
                if (Int32.TryParse(val.Text, out int a) == false)
                {
                    MessageBox.Show("Error : only numbers can be accepted");
                    return;
                }
            }

             int masse = Int32.Parse(formulaire["Mass:"].Text);
             int gravity = Int32.Parse(formulaire["Gravity:"].Text);
             int speed = Int32.Parse(formulaire["Speed:"].Text);
             int angle = Int32.Parse(formulaire["Angle:"].Text);
             int posY0 = Int32.Parse(formulaire["Position 0:"].Text);
             form.Restart(masse, gravity, speed, angle, posY0);
             this.Close();
        }
        
        public void dicoLine(int x, int y, string txt)
        {
            Label title = new Label();
            title.Size = new Size(100,40);
            title.Location = new Point(x,y);
            title.Text = txt;
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Font = new Font(FontFamily.GenericSansSerif, 18F);
            this.Controls.Add(title);
            
            TextBox input = new TextBox();
            input.Font = new Font(FontFamily.GenericSansSerif, 18F);
            input.Size = new Size(200,40);
            input.Location = new Point(title.Location.X+20+title.Width,y);
            this.Controls.Add(input);
            formulaire.Add(txt,input);
        }
        
    }
}