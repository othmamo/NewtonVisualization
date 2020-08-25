using System.Runtime.CompilerServices;

namespace Newton
{
    public class Objets
    {
        private double mass;
        private double posY;
        private double posX;
        private double posY0;

        public Objets(double mass, double posY0)
        {
            posX = 0;
            posY = posY0;
            this.mass = mass;
            this.posY0 = posY0;
        }

        public double getMasse() { return mass; }
        public double getPosY0() { return posY0; }
        public double getPosY() { return posY; }
        public double getPosX() { return posX; }
        public void setPosY(double pos) { posY = pos; }
        public void setPosX(double pos) { posX = pos; }

    }
}