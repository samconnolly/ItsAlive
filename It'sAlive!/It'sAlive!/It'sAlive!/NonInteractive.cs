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
    class NonInteractive
    {
        public Vector2 position;
        public float layer;
        private Texture2D tex;
        private Rectangle rect;
        private int anims;
        public bool anim = false;
        public int animNum =0;
        private int timer;
        private int currentFrame;
        private int frate = 160;
        
        private int frames;

        public NonInteractive(Vector2 itemPosition, float drawLayer, Texture2D itemTex, int animations = 1, int nFrames = 1)
        {
            this.position = itemPosition;
            this.layer = drawLayer;
            this.tex = itemTex;
            this.anims = animations;
            this.rect.Width = tex.Width/nFrames;
            this.frames = nFrames;
            this.rect.Height = tex.Height/anims;
            this.rect.Y = animNum * rect.Height;


        }

        public void SetAnim(int animNumber)
        {
            animNum = animNumber;
            rect.Y = animNum * rect.Height;
        }

        public void Update(GameTime gametime)
        {

            if (anim == true)
            {
                timer += gametime.ElapsedGameTime.Milliseconds;


                if (timer >= frate)
                {
                    timer = 0;

                    if (currentFrame++ == frames - 1)
                    {
                        currentFrame = 0;
                        anim = false;
                    }

                    rect.X = currentFrame * rect.Width;
                }
            }

            else
            {
                rect.X = 0;
            }

        }

        public void Render(SpriteBatch sbatch)
        {
            sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
        }
    }
}
