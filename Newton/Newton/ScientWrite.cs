using System;
using System.Windows.Forms;

namespace Newton
{
    public class ScientWrite
    {
        public static Tuple<double, int> mult (Tuple<double, int> a, Tuple<double, int> b)
        {
            double left = a.Item1 * b.Item1;
            int right = a.Item2 + b.Item2;
            return  new Tuple<double, int>(left,right);
        }
        
        public static Tuple<double, int> div (Tuple<double, int> a, Tuple<double, int> b)
        {
            if(b.Item1==0)
                return new Tuple<double, int>(0,0);
            double left = a.Item1 / b.Item2;
            int right = a.Item2 - b.Item2;
            return  new Tuple<double, int>(left,right);
        }
        public static Tuple<double,int> toScient(double num)
        {
            if(num==0)
                return new Tuple<double, int>(0,0);
            int right = longer(num) ;
            if (!(num < 1 && num > -1 && num!=0))
                right -= 1;
            double left = num / (double) (Math.Pow(10,right));
            return new Tuple<double, int>(left,right);
        }
        public static double fromScient(Tuple<double,int> num)
        {
            return (num.Item1 * Math.Pow(10, num.Item2));
        }
        public static int longer(double num)
        {
            int res = 0;
            if (num < 1 && num > -1 && num!=0)
            {
                while (!(((int) num>=1 && (int) num<=9) || ((int) num>=-9 && (int) num<=-1)))
                {
                    num*=10;
                    res--;
                }
            }
            else
            {
                long number = (long) num;
                while (number != 0)
                {
                    number /= 10;
                    res++;
                }
            }
            return res;
        }
    }
}