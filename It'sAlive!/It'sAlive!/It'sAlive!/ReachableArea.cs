using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace It_sAlive_
{
    // Creates a grid of the squares which are reachable or not in the play area

    class ReachableArea
    {
        public int width;
        public int height;
        private int[,] reachable;

        private List<FloorObject> floorObjectList;

        // The public instance class

        public ReachableArea(Grid grid, List<FloorObject> objectList)
        {
            
            this.width  = (int)grid.columns;
            this.height = (int)grid.rows;

            this.reachable = new int[width, height];

            for (int x = 0; x < (width); x++)
            {
                for (int y = 0; y < (height); y++)
                {
                    reachable[x, y] = 0;
                }
            }

            this.floorObjectList = objectList;

            foreach (FloorObject floorObject in floorObjectList)
            {
                reachable[(int)floorObject.gridPosition.X-1, (int)floorObject.gridPosition.Y-1] = 1;

            }
        }

        // return value from array

        public int Value(int x, int y)
        {

            try
            {
                if (reachable[x-1, y-1] > 0)
                {
                    return 1;
                }

                else
                {
                    return 0;
                }
            }

            catch (IndexOutOfRangeException)
            {
                return 1;
            }


        }

        public void Update(List<FloorObject> objectList)
        {
            for (int x = 0; x < (width); x++)
            {
                for (int y = 0; y < (height); y++)
                {
                    reachable[x, y] = 0;
                }
            }

            this.floorObjectList = objectList;

            foreach (FloorObject floorObject in floorObjectList)
            {
                for (int x = 0; x < floorObject.footprint.X; x++)
                {
                    for (int y = 0; y < floorObject.footprint.Y; y++)
                    {
                        reachable[(int)floorObject.gridPosition.X - 1 + x, (int)floorObject.gridPosition.Y - 1 + y] = 1;
                    }
                }

            }

        }
        
    }
    
}
