using SlowRobotics.Voxels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Simulation.ReactionDiffusion
{
    class BZReaction
    {
        public VoxelGridT<float> grid;
        HashSet<int> birthRules = new HashSet<int> { 5, 7 };
        HashSet<int> survivalRules = new HashSet<int> { 6 };
        public int generations = 0;

        public BZReaction(VoxelGridT<float> _grid)
        {
            grid = _grid;
        }

        public VoxelGridT<float> getData()
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
                        StepBZReaction(grid.get(x, y, z), x, y, z);
                        index++;
                    }
                }
            }
        }

        void StepBZReaction(VoxelGridT<float>.Voxel c, int x, int y, int z)
        {

            float cellVal = c.Data;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    for (int k = -1; k <= 1; k++)
                    {
                        int nx = x + i;
                        int ny = y + j;
                        int nz = z + k;
                        VoxelGridT<float>.Voxel neighbour = grid.get(nx, ny, nz);
                    }
                }
            }

        }

    }
}
