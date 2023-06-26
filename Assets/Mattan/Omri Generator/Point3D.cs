using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap_Auto_Generation
{

    public class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public int Type { get; set; }

        public static Point3D ZERO = new Point3D(0, 0, 0);



        public Point3D(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
            Type = -1;
        }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Type = -1;
        }
        public Point3D(int type)
        {
            X = 0;
            Y = 0;
            Z = 0;
            Type = type;
        }

        public Point3D(double x, double y, double z, int type)
        {
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }

        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static Point3D operator *(Point3D p, double scalar)
        {
            return new Point3D(p.X * scalar, p.Y * scalar, p.Z * scalar);
        }

        public static Point3D operator /(Point3D p, double scalar)
        {
            if (scalar == 0)
            {
                throw new ArgumentException("Cannot divide by zero.");
            }

            return new Point3D(p.X / scalar, p.Y / scalar, p.Z / scalar);
        }

        public static Point3D operator ~(Point3D p)
        {
            return new Point3D(p.X, p.Y, 0);
        }

        public static bool operator ==(Point3D p1, Point3D p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z);
        }

        public static bool operator !=(Point3D p1, Point3D p2)
        {
            return !(p1 == p2);
            // return (p1.X != p2.X || p1.Y != p2.Y || p1.Z != p2.Z);
        }

        public double Distance2D(Point3D p)
        {
            double dx = p.X - this.X;
            double dy = p.Y - this.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public Point3D FindClosestPointInList(List<Point3D> points)
        {
            var target = points.First();
            double minDist = this.Distance2D(target);

            foreach (var p in points)
            {
                var dist = p.Distance2D(this);
                if (dist < minDist)
                {
                    target = p;
                    minDist = dist;
                }
            }

            return target;
        }

        public static double Distance2D(Point3D p1, Point3D p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        

        public double Distance3D(Point3D p)
        {
            double dx = p.X - this.X;
            double dy = p.Y - this.Y;
            double dz = p.Z - this.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public static double Distance3D(Point3D p1, Point3D p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double dz = p2.Z - p1.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public override string ToString()
        {
            return $"(X,Y,Z).(type) - ({(int)X},{(int)Y},{(int)Z}).({Type})";
        }

        public static Point3D Copy(Point3D p)
        {
            return new Point3D(p.X, p.Y, p.Z);
        }

        public Point3D Copy()
        {
            return new Point3D(this.X, this.Y, this.Z);
        }

        public Point3D flatenCopyOnX()
        {
            return new Point3D(0, Y, Z);
        }
        public Point3D flatenCopyOnY()
        {
            return new Point3D(X, 0, Z);
        }
        public Point3D flatenCopyOnZ()
        {
            return new Point3D(X, Y, 0);
        }
        public Point3D flatenCopyOnXY()
        {
            return new Point3D(0, 0, Z);
        }
        public Point3D flatenCopyOnYZ()
        {
            return new Point3D(X, 0, 0);
        }
        public Point3D flatenCopyOnXZ()
        {
            return new Point3D(0, Y, 0);
        }

        public Point3D flatenOnLine(Point3D p1, Point3D p2)
        {
            if(p2.X - p1.X ==0 ) 
                return new Point3D(p1.X,this.Y);
            double m = (p2.Y - p1.Y) / (p2.X - p1.X); // p1-p2 -> y=mx+n
            double n = p1.Y - m * p1.X; // p1-p2 -> y=mx+n

            if (m == 0)
            {
                return new Point3D(this.X,p1.Y);
            }
            double mPerp = -1 / m; // pT-p? -> y=mx+n
            double nPerp = this.Y - mPerp * this.X; // pT-p? -> y=mx+n

            double intersectionX = (nPerp - n) / (m - mPerp); // Calculate the x-coordinate of the intersection point
            double intersectionY = m * intersectionX + n; // Calculate the y-coordinate of the intersection point

            return new Point3D (intersectionX,intersectionY);
        }
    }
}
