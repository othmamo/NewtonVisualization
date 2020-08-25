using System;
using System.Windows.Forms;

namespace Newton
{
    public class Environnement
    {
        private double gravity;
        private double frottements;
        private double vit0;
        private double angle;

        public Environnement(double gravity, double vit0, double angle)
        {
            this.gravity = gravity;
            this.vit0 = vit0;
            this.angle = angle;
            frottements = calculFrot();
        }
        
        public double DegToRad(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        public double CalculTrajectoire(Objets obj, double x)
        {
            /*return ScientWrite.fromScient(ScientWrite.mult(ScientWrite.toScient(Math.Tan(DegToRad(angle))),ScientWrite.toScient(x))) 
                   + obj.getPosY0() 
                   - ScientWrite.fromScient(ScientWrite.mult(ScientWrite.toScient(gravity * x * x), 
                       ScientWrite.toScient(2 * vit0 * vit0*Math.Pow(Math.Cos(DegToRad(angle)),2))));
*/
            return Math.Tan(DegToRad(angle))*x + obj.getPosY0() - ((gravity * x * x) / (2 * vit0 * vit0*Math.Pow(Math.Cos(DegToRad(angle)),2)));
        }

        public int Agrandissement(int sizeObj,Objets obj, Label graph)
        {
            double x = 0;
            double c = CalculTrajectoire(obj, x);
            double ymax = c;
            while (c>0)
            {
                x++;
                c = CalculTrajectoire(obj, x);
                ymax = Math.Max(ymax, c);
            }
            
            int agx =  (int)((graph.Width-sizeObj)/x);
            if (agx == 0)
                return -1;
            while (ymax * agx > graph.Height)
            {
                agx /= 2;
            }

            if (agx == 0)
                return -2;

            return agx;
        }

        public double calculFrot()
        {
            return 1;
        }


        public double getGravity() { return gravity; }
        public double getVit0() { return vit0; }
        public double getAngle() { return angle; }

    }
}