using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
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
    

    class Path
    {

        public ReachableArea reach;
         
        // The public instance class

        public Path(ReachableArea reachable)
        {
            this.reach = reachable;
        }

        // check the traversibility of a square on the grid

        public void Update(ReachableArea reachableArea)
        {
            reach = reachableArea;
        }

        public bool Check(int x,int y) 
        {
            if (reach.Value(x,y) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // return an array of squares adjacent to the input square

        private Vector2[,] Adjacent(Vector2 position)
        {
            Vector2[,] adjacentList = new Vector2[,]
                    { { new Vector2(-1,1),  new Vector2(0,1),  new Vector2(1,1) },
                      { new Vector2(-1,0),  new Vector2(0,0),  new Vector2(1,0) },
                      { new Vector2(-1,-1), new Vector2(0,-1), new Vector2(1,-1) }};

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    adjacentList[x, y] = adjacentList[x, y] + position;
                }
            }

            return adjacentList;
                       
        }

        public List<Vector2> PathList(Vector2 start, Vector2 end, Grid grid)
        {
            Vector2 startPosition = start;
            Vector2 endPosition = end;

           Vector2 currentPosition = startPosition;

           List<Vector2> openList = new List<Vector2>(); // squares to check
           openList.Add(startPosition);

            List<Vector2> openListParent = new List<Vector2>(); // shortest path back from a square
           openListParent.Add(startPosition);

            List<float> openListF = new List<float>(); // usefulness of a square
           openListF.Add(0);

            List<float> openListG = new List<float>(); // distance of a square from start
           openListG.Add(0);

           List<Vector2> closedList = new List<Vector2>();   // checked squares
           List<Vector2> closedParents = new List<Vector2>();

           List<Vector2> path = new List<Vector2>(); // list of vectors between points

           int currentIndex;

           if (startPosition != endPosition)
           {
               while (currentPosition != endPosition)
               {
                   Vector2[,] currentAdjacent = Adjacent(currentPosition);

                   currentIndex = openList.IndexOf(currentPosition);

                   // for each square adjacent to the current position...

                   for (int x = 0; x < 3; x++)
                   {
                       for (int y = 0; y < 3; y++)
                       {


                           // if the square is on the screen and reachable...

                           if (currentAdjacent[x, y] != currentPosition
                                && currentAdjacent[x, y].X >= 0 && currentAdjacent[x, y].Y >= 0
                                && currentAdjacent[x, y].X < grid.columns && currentAdjacent[x, y].Y < grid.rows
                                && closedList.IndexOf(currentAdjacent[x, y]) < 0)
                           {
                               if (Check((int)currentAdjacent[x, y].X, (int)currentAdjacent[x, y].Y) == true)
                               {
                                   // calculate usefulness of the square...

                                   Vector2 Cvector = currentAdjacent[x, y] - currentPosition;
                                   Vector2 Hvector = currentAdjacent[x, y] - endPosition;
                                   double C = Math.Sqrt(Math.Pow((double)Cvector.X,2.0)+ Math.Pow((double)Cvector.Y,2.0));              // distance from current position
                                   float S = openListG[currentIndex];  // distance to current position
                                   float G = (float)C + S;                         // distance from start position
                                   float H = Hvector.Length();              // direct distance to final position
                                   //float H = Math.Abs(Hvector.X) + Math.Abs(Hvector.Y);  // 'Manhatten' distance estimate to final position (gives smoother paths)
                                   float F = G + H;                         // usefulness of this square

                                   // add to open array if not already present

                                   if (openList.IndexOf(currentAdjacent[x, y]) < 0)
                                   {
                                       openList.Add(currentAdjacent[x, y]);
                                       openListParent.Add(currentPosition);
                                       openListF.Add(F);
                                       openListG.Add(G);

                                   }

                                   // if present in open array, check if path from current position is shorter

                                   if (openList.IndexOf(currentAdjacent[x, y]) >= 0)
                                   {
                                       int adjacentIndex = openList.IndexOf(currentAdjacent[x, y]);

                                       if (G < openListG[adjacentIndex])
                                       {
                                           openListParent[adjacentIndex] = currentPosition;
                                           openListF[adjacentIndex] = F;
                                           openListG[adjacentIndex] = G;

                                       }

                                   }

                               }
                           }
                       }
                   }
                   // add current psition to closed list

                   closedList.Add(currentPosition);
                   closedParents.Add(openListParent[currentIndex]);

                   // remove current position from open list

                   openList.Remove(currentPosition);
                   openListF.Remove(openListF[currentIndex]);
                   openListG.Remove(openListG[currentIndex]);
                   openListParent.Remove(openListParent[currentIndex]);
                   
                   // find coordinate with smallest F value, set to current position (if there's a path!)

                   if (openListF.Count > 0)
                   {

                       float minF = openListF.Min();

                       int minFIndex = openListF.IndexOf(minF);



                       currentPosition = openList[minFIndex];
                   }

                   else
                   {
                       break;
                   }

               }

               

               Vector2 pathCurrentPosition = closedList[closedList.Count - 1];
               int pathCurrentIndex = 0;

               path.Add(endPosition); ////////////////

               while (pathCurrentPosition != startPosition)
               {
                   path.Add(pathCurrentPosition);
                   pathCurrentIndex = closedList.IndexOf(pathCurrentPosition);
                   pathCurrentPosition = closedParents[pathCurrentIndex];
               }

               path.Add(startPosition);  ///////////////

               for (int x = 0; x < path.Count; x++)
               {
                   path[x] = new Vector2(path[x].X, path[x].Y);
               }
           }

           path.Reverse();

            return path;
            
        }

        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch sbatch, List<Vector2> path, Vector2 offset,PerspectiveGrid grid)
        {
            Texture2D blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Microsoft.Xna.Framework.Color.White });

            for (int x = 0; x < (path.Count() - 1); x++)
            {
                Vector2 startPoint = grid.CartesianCoords(path[x]);
                Vector2 endPoint = grid.CartesianCoords(path[x + 1]);

                float angle = (float)Math.Atan2(endPoint.Y - startPoint.Y, endPoint.X - startPoint.X);
                float length = Vector2.Distance(startPoint, endPoint);

                sbatch.Draw(blank, startPoint + offset, null, Microsoft.Xna.Framework.Color.Blue, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0);
                
            }
        }


        
    }
}
