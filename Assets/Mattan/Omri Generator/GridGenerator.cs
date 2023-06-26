using UnityEngine;
using TileMap_Auto_Generation;
using System.Collections.Generic;
using UnityEngine;

class RandomDoubler{  }
public static class GridGenerator {
    static double NextDouble() => (double)Random.Range(0f,1f);
    
    public static List<Point3D> 
    GenerateDoubleGridOver(int width, int height, int fromX, int fromZ, int rangeY, int distBetweenDots, int subDotsCount) //subDotsCount -> num of point between grid points
        {
            int rangeYFrom = 0;
            var distBetweensubDots = distBetweenDots / subDotsCount;

            List<Point3D> grid = new List<Point3D>();

            for (int i = fromX - (distBetweenDots); i <= height + fromX + distBetweenDots; i += distBetweenDots)
            {
                for (int j = fromZ - (distBetweenDots); j <= width + fromZ + distBetweenDots; j += distBetweenDots)
                {
                    grid.Add(new Point3D(i, j, (NextDouble() * rangeY) + rangeYFrom,0));
                    Debug.Log($"grid[{grid.Count - 1}]: " + grid[grid.Count - 1]);

                    for (int m = 0; m < subDotsCount; m++)
                    {
                        for (int n = 0; n < subDotsCount; n++)
                        {
                            var atX = i + (m * distBetweensubDots);
                            var atY = j + (n * distBetweensubDots);
                            if (!(m == 0 && n == 0 ||
                                m * distBetweensubDots == distBetweenDots && n == 0 ||
                                m == 0 && n * distBetweensubDots == distBetweenDots||
                                m * distBetweensubDots == distBetweenDots && n * distBetweensubDots == distBetweenDots))
                            {
                                var point = new Point3D(atX, atY, (NextDouble() * rangeY) + rangeYFrom, 1);
                                grid.Add(point);
                            }
                        }
                    }
                    if (distBetweenDots % subDotsCount != 0)
                    {
                        if (i == height + fromX + distBetweenDots)
                        {
                            for (int m = 1; m <= subDotsCount; m++)
                            {
                                grid.Add(new Point3D(i, j + (m * distBetweensubDots), (NextDouble() * rangeY) + rangeYFrom, 1));
                            }
                        }

                        if (j == width + fromZ + distBetweenDots)
                        {
                            for (int m = 1; m <= subDotsCount; m++)
                            {
                                grid.Add(new Point3D(i + (1 * distBetweensubDots), j, (NextDouble() * rangeY) + rangeYFrom, 1));
                            }
                        }
                    }
                }
            }
            return grid;
        }
}