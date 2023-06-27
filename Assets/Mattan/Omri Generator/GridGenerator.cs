using UnityEngine;
using TileMap_Auto_Generation;
using System.Collections.Generic;

public static class GridGenerator {
    public static double NextDouble() => (double)Random.Range(0f,1f);
    public static int Next(int n) => Random.Range(0,n);
    
    public static List<Point3D> 
    GenerateDoubleGridOver(float sizeX, float sizeZ, float fromX, float fromZ, float rangeY, float distBetweenDots, float subDotsCount) //subDotsCount -> num of point between grid points
        {
            
            int rangeYFrom = 0;
            var distBetweensubDots = distBetweenDots / subDotsCount;

            List<Point3D> grid = new List<Point3D>();

            // for (int xIndex = (int)(fromX - (distBetweenDots)); xIndex <= sizeZ + fromX + distBetweenDots; xIndex += (int)distBetweenDots)
            for (int xIndex = (int)(fromX); xIndex <= sizeX + fromX + distBetweenDots; xIndex += (int)distBetweenDots)
            {
                // for (int zIndex = (int)(fromZ - (distBetweenDots)); zIndex <= sizeX + fromZ + distBetweenDots; zIndex += (int)distBetweenDots)
                for (int zIndex = (int)(fromZ); zIndex <= sizeZ + fromZ + distBetweenDots; zIndex += (int)distBetweenDots)
                {
                    grid.Add(new Point3D(xIndex, zIndex, (NextDouble() * rangeY) + rangeYFrom,0));
                    // Debug.Log($"grid[{grid.Count - 1}]: " + grid[grid.Count - 1]);

                    for (int m = 0; m < subDotsCount; m++)
                    {
                        for (int n = 0; n < subDotsCount; n++)
                        {
                            var atX = xIndex + (m * distBetweensubDots);
                            var atZ = zIndex + (n * distBetweensubDots);
                            if (!(m == 0 && n == 0 ||
                                m * distBetweensubDots == distBetweenDots && n == 0 ||
                                m == 0 && n * distBetweensubDots == distBetweenDots||
                                m * distBetweensubDots == distBetweenDots && n * distBetweensubDots == distBetweenDots))
                            {
                                var point = new Point3D(atX, atZ, (NextDouble() * rangeY) + rangeYFrom, 1);
                                grid.Add(point);
                            }
                        }
                    }
                    if (distBetweenDots % subDotsCount != 0)
                    {
                        if (xIndex == sizeX + fromX + distBetweenDots)
                        {
                            for (int m = 1; m <= subDotsCount; m++)
                            {
                                grid.Add(new Point3D(xIndex, zIndex + (m * distBetweensubDots), (NextDouble() * rangeY) + rangeYFrom, 1));
                            }
                        }

                        if (zIndex == sizeZ + fromZ + distBetweenDots)
                        {
                            for (int m = 1; m <= subDotsCount; m++)
                            {
                                grid.Add(new Point3D(xIndex + (1 * distBetweensubDots), zIndex, (NextDouble() * rangeY) + rangeYFrom, 1));
                            }
                        }
                    }
                }
            }
            return grid;
        }
}