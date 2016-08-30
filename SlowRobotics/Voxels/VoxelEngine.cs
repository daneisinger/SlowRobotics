using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    class VoxelEngine
    {
        public VoxelGrid grid;
        HashSet<int> birthRules = new HashSet<int> {5,7};
        HashSet<int> survivalRules = new HashSet<int> {6};
        public int generations = 0;

        public VoxelEngine(VoxelGrid _grid)
        {
            grid = _grid;
        }

        public VoxelGrid getData()
        {
            return grid;
        }

        public void run()
        {
            generations++;
            int index = 0;
            for (int z = 0; z < grid.d; z++)
            {
                for (int y = 0; y < grid.h; y++)
                {
                    for (int x = 0; x < grid.w; x++)
                    {
                        BZReaction(grid.getCell(x, y, z));
                        index++;
                    }
                }
            }
        }

        void BZReaction(Cell c)
        {
            
            float cellVal = c.get();

            for(int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        int nx = c.x+i;
                        int ny = c.y+j;
                        int nz = c.z+k;
                         Cell neighbour = grid.getCell(nx, ny, nz);
                    }
                }
            }
           
        }
       
    }
}
