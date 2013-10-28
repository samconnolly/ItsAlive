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
    class FloorObject
    {
        public string name;
        public List<MenuAction> menuActions;

        public Texture2D objectTex;
        private Rectangle rect;

        public int width = 0;
        public int height = 0;
        public int frames;
        public int anims;
        private int frate = 80;
        private int timer = 0;
        private int currentFrame = 0;
        private int currentAnim = 0;
       
        public Vector2 position = Vector2.Zero;
        public Vector2 gridPosition = Vector2.Zero;
        public Vector2 offset;

        public float layer;
        public float scale = 1.0f;

        public int cost;

        public bool on = false;

        // build icon
        public bool onBuildList = false;
        public Vector2 iconPosition = Vector2.Zero;
        private Texture2D iconTex;
        private Rectangle iconRect;
        private float iconLayer = 0.1f;

        // The public instance of the object

        public FloorObject(Texture2D texture,Texture2D iconTexture, int frameNumber,int animNumber, Vector2 gridPosition, Grid grid, string name,int cost, List<MenuAction> menuActions)
        {

            this.objectTex = texture;
            this.gridPosition = gridPosition;
            this.position = grid.EdgeCartesianCoords(gridPosition);
            this.layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y;

            this.name = name;
            this.menuActions = menuActions;
            this.cost = cost;

            this.frames = frameNumber;
            this.anims = animNumber;

            // Create rectangle for animation

            this.width = texture.Width/frames;
            this.height = texture.Height/anims;

            rect.X = 0;
            rect.Y = 0;
            rect.Width = width;
            rect.Height = height;


            offset = new Vector2(0, ((texture.Height/anims) * scale));// new Vector2((((objectTex.Width * scale / frames) * scale) / 2.0f), (texture.Height * scale) / 2.0f);

            // icon

            this.iconTex = iconTexture;

            if (iconTex != null)
            {
                this.iconRect.Width = iconTex.Width;
                this.iconRect.Height = iconTex.Height;
            }
        }


        public void Update(GameTime gametime)
        {
            timer += gametime.ElapsedGameTime.Milliseconds;

            if (on == true)
            {
                currentAnim = 1;
            }
            else
            {
                currentAnim = 0;
            }

            
            if (timer >= frate)
            {
                timer = 0;

                if (currentFrame++ == frames - 1)
                {
                    currentFrame = 0;
                }

                rect.X = currentFrame * width;
                rect.Y = currentAnim * height;
            }
          


        }

        public void Render(SpriteBatch sbatch)
        {
           

            sbatch.Draw(objectTex, position - offset, rect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);


        }

        public void IconRender(SpriteBatch sbatch, Build build)
        {

            
                sbatch.Draw(iconTex, iconPosition, iconRect, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, iconLayer);
            

        }
    }
}
