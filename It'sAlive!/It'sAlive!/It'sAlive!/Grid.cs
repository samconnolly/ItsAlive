using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace It_sAlive_
{
    class Grid
    {

        double nearWidth; //= 1080;
        double farWidth;  //= 800;
        double height;
        public double rows; // = 20;
        public double columns; // = 15;
        
        double tileWidth;
        double tileHeight;
        double widthScale;

        private bool reachSquares;
        
        Vector2 roomPosition; // bottom left!

        public Grid(int nearRoomWidth, int farRoomWidth, int floorHeight, int nColumns, int nRows, Vector2 roomPosition, bool reachSquares = false)
        {
            this.nearWidth = (double)nearRoomWidth;
            this.farWidth = (double)farRoomWidth;
            this.height = (double)floorHeight;
            this.rows = (double)(nRows);
            this.columns = (double)(nColumns);
            this.roomPosition = roomPosition;
            this.tileHeight = height / rows;
            this.widthScale = (nearWidth - farWidth) / rows;
            this.reachSquares = reachSquares;

        }

        public Vector2 CartesianCoords(Vector2 gridCoords)
        {
            double xTile = gridCoords.X;
            double yTile = gridCoords.Y;


            double y = roomPosition.Y - ((yTile - 0.5) * tileHeight);
            tileWidth = (widthScale * (rows - (yTile - 1)) + farWidth) / columns;
            double x = roomPosition.X + (nearWidth / 2.0) + (((xTile - 0.5) - (columns / 2.0)) * tileWidth);

            Vector2 position = new Vector2((int)x, (int)y);

            return position;
        }

   

        public Vector2 EdgeCartesianCoords(Vector2 gridCoords)
        {
            double xTile = gridCoords.X;
            double yTile = gridCoords.Y;


            double y = roomPosition.Y - ((yTile-1.0) * tileHeight);
            tileWidth = (widthScale *  (rows - (yTile-1)) + farWidth)/columns;
            double x = roomPosition.X + (nearWidth / 2.0) + (((xTile-1.0) - (columns / 2.0)) * tileWidth);

            Vector2 position = new Vector2((int)x, (int)y);

            return position;

        }

        public Vector2 reverseCoords(Vector2 position)
        {
            int yTile = (int) (((roomPosition.Y - position.Y) / tileHeight) + 1.0);
            tileWidth = (widthScale * (rows - (yTile - 1)) + farWidth) / columns;
            int xTile = (int)((position.X - roomPosition.X - 0.5 * nearWidth) / tileWidth + 1 + columns/2);

            return new Vector2(xTile, yTile);
        }

        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch sbatch,ReachableArea reach)
        {
            // draw the grid

            Texture2D blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Microsoft.Xna.Framework.Color.White });

            for (int x = 1; x < (columns+2); x++)
            {

                Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(x,1)), EdgeCartesianCoords(new Vector2(x, (float)rows+1))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                float length = Vector2.Distance(line.Item1, line.Item2);

                sbatch.Draw(blank, line.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Blue, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0.5f);

            }

            for (int y = 1; y < (rows+2); y++)
            {

                Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(1, y)), EdgeCartesianCoords(new Vector2((float)columns+1, y))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                float length = Vector2.Distance(line.Item1, line.Item2);

                sbatch.Draw(blank, line.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Blue, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0.5f);

            }

            // highlight the accessibility of squares

            if (reachSquares == true)
            {
                for (int x = 1; x < (reach.width +1); x++)
                {
                    for (int y = 1; y < (reach.height+1); y++)
                    {
                        
                        if (reach.Value(x, y) == 1)
                        {


                            Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(x, y)), EdgeCartesianCoords(new Vector2(x, y + 1))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                            float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                            float length = Vector2.Distance(line.Item1, line.Item2);

                            sbatch.Draw(blank, line.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Red, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0.49f);


                            Tuple<Vector2, Vector2> line2 = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(x, y)), EdgeCartesianCoords(new Vector2(x + 1, y))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                            float angle2 = (float)Math.Atan2(line2.Item2.Y - line2.Item1.Y, line2.Item2.X - line2.Item1.X);
                            float length2 = Vector2.Distance(line2.Item1, line2.Item2);

                            sbatch.Draw(blank, line2.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Red, angle2, Vector2.Zero, new Vector2(length2, 3.0f), SpriteEffects.None, 0.49f);

                            Tuple<Vector2, Vector2> line3 = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(x+1, y+1)), EdgeCartesianCoords(new Vector2(x, y + 1))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                            float angle3 = (float)Math.Atan2(line3.Item2.Y - line3.Item1.Y, line3.Item2.X - line3.Item1.X);
                            float length3 = Vector2.Distance(line3.Item1, line3.Item2);

                            sbatch.Draw(blank, line3.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Red, angle3, Vector2.Zero, new Vector2(length3, 3.0f), SpriteEffects.None, 0.49f);


                            Tuple<Vector2, Vector2> line4 = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(x+1, y+1)), EdgeCartesianCoords(new Vector2(x + 1, y))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                            float angle4 = (float)Math.Atan2(line4.Item2.Y - line4.Item1.Y, line4.Item2.X - line4.Item1.X);
                            float length4 = Vector2.Distance(line4.Item1, line4.Item2);

                            sbatch.Draw(blank, line4.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Red, angle4, Vector2.Zero, new Vector2(length4, 3.0f), SpriteEffects.None, 0.49f);

                        }
                    }
                }
            }
        }

    }


}
