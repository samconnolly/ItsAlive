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
        private Texture2D noSquare;
        private Texture2D yesSquare;
        
        Vector2 roomPosition; // bottom left!

        public Grid(int nearRoomWidth, int farRoomWidth, int floorHeight, int nColumns, int nRows, Vector2 roomPosition, bool reachSquares = false, Texture2D noSquare = null, Texture2D yesSquare = null)
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
            this.noSquare = noSquare;
            this.yesSquare = yesSquare;

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
                        //if (reach.Value(x, y) == 0)
                        //{
                        //    sbatch.Draw(yesSquare, CartesianCoords(new Vector2(x, y)), Color.White);
                        //}
                        if (reach.Value(x, y) == 1)
                        {
                            sbatch.Draw(noSquare, CartesianCoords(new Vector2(x, y+1)), Color.White);
                        }
                    }
                }
            }
        }

    }


}
