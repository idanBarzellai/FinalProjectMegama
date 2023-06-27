using TileMap_Auto_Generation;
using System.Collections.Generic;
using System.Linq;

public class LinearInterpolator
    {
        private List<Point3D> grid;

        public LinearInterpolator(List<Point3D> grid)
        {
            this.grid = grid;
        }

        public Point3D[,] GetEnclosingArray(Point3D target, bool includeSubGrid)
        {
            var lookIn = grid;
            if (!includeSubGrid)
            {
                lookIn = lookIn.Where(p => p.Type == 0).ToList();
            }

            var filteredTL = lookIn.Where(p => p.X <= target.X && p.Y <= target.Y).ToList();
            var L = filteredTL.Max(p => p.X);
            var T = filteredTL.Max(p => p.Y);

            var filteredBR = lookIn.Where(p => p.X >= target.X && p.Y >= target.Y).ToList();
            var R = filteredBR.Min(p => p.X);
            var B = filteredBR.Min(p => p.Y);

            Point3D[,] enclosing = new Point3D[2, 2];
            enclosing[0, 0] = lookIn.Find(p => p.X == L && p.Y == T);
            enclosing[1, 1] = lookIn.Find(p => p.X == R && p.Y == B);
            enclosing[0, 1] = lookIn.Find(p => p.X == R && p.Y == T);
            enclosing[1, 0] = lookIn.Find(p => p.X == L && p.Y == B);


            return enclosing;
        }

        public double Calculate(Point3D target, bool includeSubGrid)
        {
            var enclosing = GetEnclosingArray(target, includeSubGrid);

            if (enclosing[0, 0] == enclosing[1, 1])
            {
                return enclosing[0, 0].Z;
            }

            if (enclosing[0, 0] == enclosing[0, 1])
            {
                return Interpolate(enclosing[0, 0].Y, enclosing[0, 0].Z, enclosing[1, 0].Y, enclosing[1, 0].Z, target.Y);
            }

            if (enclosing[0, 0] == enclosing[1, 0])
            {
                return Interpolate(enclosing[0, 0].X, enclosing[0, 0].Z, enclosing[0, 1].X, enclosing[0, 1].Z, target.X);
            }

            var h = BiInterpolate(enclosing[0, 0], enclosing[0, 1], enclosing[1, 0], enclosing[1, 1], target);
            return h;
        }
        public double CalculateAt(float x, float y, bool includeSubGrid)
        {
            return Calculate(new Point3D((double)x, (double)y), includeSubGrid);
        }

        public double BiInterpolate(Point3D TL, Point3D TR, Point3D BL, Point3D BR, Point3D target)
        {
            double TH = Interpolate(TL.X, TL.Z, TR.X, TR.Z, target.X);
            double BH = Interpolate(BL.X, BL.Z, BR.X, BR.Z, target.X);
            double H = Interpolate(TL.Y, TH, BL.Y, BH, target.Y);
            return H;
        }

        public double Interpolate(double x1, double y1, double x2, double y2, double xt)
        {
            var m = (y2 - y1) / (x2 - x1);
            var n = -(m * x1) + y1;
            var r = m * xt + n;
            return r;
        }

    }