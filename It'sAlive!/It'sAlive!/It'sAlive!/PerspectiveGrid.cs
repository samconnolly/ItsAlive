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
    class PerspectiveGrid
    {
        
        double vanishingHeight; //= 900;
        double roomEndHeight; //= 450;
        double roomWidth; //= 800;
        double nearHeight; //= 450;
        public double rows; // = 20;
        public double columns; // = 15;
        public double angle;
        public double gridY;
     
        double perspectiveScaling;
        double baseTileWidth;
        double tileDepth;
        double tileAngularWidth;
        double halfTheta;

        Vector2 roomPosition = Vector2.Zero; // bottom left!

        public PerspectiveGrid(int nearWallHeight, int vanishingPointHeight, int roomEndingHeight,
        int roomNearWidth, int nColumns, int nRows, Vector2 roomPosition)
        {
            this.roomWidth = (double)roomNearWidth;
            this.roomEndHeight = (double)roomEndingHeight;
            this.vanishingHeight = (double)vanishingPointHeight;
            this.nearHeight = (double)nearWallHeight;
            this.perspectiveScaling = vanishingHeight / nearWallHeight;
            this.rows = (double)nRows;
            this.columns = (double)nColumns;
            this.roomPosition = roomPosition;
            this.halfTheta = Math.Atan((roomWidth / 2.0) / vanishingHeight);
            this.tileAngularWidth = halfTheta / (columns / 2.0);
            this.baseTileWidth = roomWidth / nRows;
            this.tileDepth = (vanishingHeight / (rows)) * (Math.Tan(2.0 * (Math.Asin(roomEndHeight / (2.0 * vanishingHeight)))));

        }

        public Vector2 CartesianCoords(Vector2 gridCoords)
        {
            double xTile = gridCoords.X;
            double yTile = gridCoords.Y;

           
            angle = (xTile * tileAngularWidth) - halfTheta + (tileAngularWidth/2.0);
            

            double MidAngle = Math.Atan(((yTile) * tileDepth) / vanishingHeight);


            double MidHeight = 2.0 * vanishingHeight * (Math.Sin(MidAngle / 2.0));

            gridY = MidHeight;

            double y = roomPosition.Y - gridY - (roomEndHeight / (rows*2.0));
            

            double x = roomPosition.X + (roomWidth/2.0) + ((900 - gridY)* Math.Tan(angle)); 
            
          
            Vector2 position = new Vector2((int)x, (int)y);

            return position;
        }

        public float CartesianScale(Vector2 gridCoords)
        {
            double yT = gridCoords.Y;

            double MidAngle = Math.Atan(((yT + 0.5) * tileDepth) / vanishingHeight);


            double MidHeight = 2.0 * vanishingHeight * (Math.Sin(MidAngle / 2.0));


            Vector2 corner = EdgeCartesianCoords(new Vector2((float)columns + 1, (float)rows + 1));

            double xOffset = (double)corner.X;
            double offAngle = Math.Atan((roomWidth / 2.0) / (vanishingHeight - nearHeight));
            double nearAngle = (Math.PI / 2.0) - offAngle;
            double farHeight = xOffset * Math.Tan(nearAngle) - nearHeight;

            double scaleRatio = farHeight / nearHeight;
            double scaleValue = (1.0 - scaleRatio) / roomEndHeight;

            float scale = (float) (1.0 - (MidHeight * scaleValue));

            return scale;
        }

        public Vector2 EdgeCartesianCoords(Vector2 gridCoords)
        {
            double xTile = gridCoords.X;
            double yTile = gridCoords.Y;
                       

            angle = (xTile * tileAngularWidth) - halfTheta;


            double nearEdgeAngle = Math.Atan((yTile * tileDepth) / vanishingHeight);
            
            double nearEdgeHeight = 2.0*vanishingHeight*(Math.Sin(nearEdgeAngle/2.0));

            gridY = nearEdgeHeight;
            

            double x = roomPosition.X + (roomWidth / 2.0) + ((900 - gridY) * Math.Tan(angle));

            
            double y = roomPosition.Y - gridY;

            

            Vector2 position = new Vector2((int)x, (int)y);

            return position;
        }

        public void Draw(GraphicsDevice graphicsDevice, SpriteBatch sbatch)
        {
            Texture2D blank = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Microsoft.Xna.Framework.Color.White });

            for (int x = 0; x < (columns + 1); x++)
            {

                Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(x, 0)), EdgeCartesianCoords(new Vector2(x, (float)rows))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                float length = Vector2.Distance(line.Item1, line.Item2);

                sbatch.Draw(blank, line.Item1 + new Vector2(0,0), null, Microsoft.Xna.Framework.Color.Red, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 1);

            }

            for (int y = 0; y < (rows + 1); y++)
            {

                Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(EdgeCartesianCoords(new Vector2(0, y)), EdgeCartesianCoords(new Vector2((float)columns, y))); //CartesianCoords(new Vector2(x, 0)), CartesianCoords(new Vector2(x, (float) rows-1))

                float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                float length = Vector2.Distance(line.Item1, line.Item2);

                sbatch.Draw(blank, line.Item1 + new Vector2(0, 0), null, Microsoft.Xna.Framework.Color.Red, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 1);

            }
        }

    }


}
