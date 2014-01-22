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
    class Knob
    {
        public Vector2 position;
        public Vector2 rotPosition;
        public float rotation;
        public float layer;
        private Texture2D tex;
        private Rectangle rect;
        private bool start = true;
        public float value = 0;
        private float mouseNum = 0;
        private float diag;
        private int min;
        private int max;

        private MachineControlParameter param;

        public Knob(Vector2 itemPosition, float drawLayer, Texture2D itemTex, MachineControlParameter param)
        {
            this.position = itemPosition;
            this.layer = drawLayer;
            this.tex = itemTex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.diag = (float)Math.Sqrt(Math.Pow((double)rect.Width, 2.0) * 2.0);

            this.min = param.min;
            this.max = param.max;
            this.value = param.value;
            this.param = param;

        }


        public void Update(Cursor cursor)
        {

            // check cursor value adjust
            if (cursor.position.X >= position.X && cursor.position.X <= position.X + rect.Width
                 && cursor.position.Y >= position.Y && cursor.position.Y <= position.Y + rect.Height
                  && cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed
                   && cursor.click == false)
            {
                if (start == true)
                {
                    cursor.click = true;
                    start = false;
                    mouseNum = cursor.position.Y;
                }
            }

            if (start == false)
            {
                value -= (cursor.position.Y - mouseNum)*(max - min)/200;
                mouseNum = cursor.position.Y;

                if (value > max) { value = max; }
                if (value < min) { value = min; }
            }

            param.value = value;

            if (cursor.mouseState.LeftButton == ButtonState.Released && cursor.mouseState.RightButton == ButtonState.Released)
            {
                start = true;
            }

            // update position for rotation

                rotation = ((value - min) / (max - min)) * (float)Math.PI - (float)Math.PI/2.0f;

            rotPosition.X = position.X + (float)Math.Sin((double)rotation - Math.PI / 4.0) * (diag / 2.0f) + (diag / 2.0f);
            rotPosition.Y = position.Y - (float)Math.Sin((double)rotation + Math.PI / 4.0) * (diag / 2.0f) + (diag / 2.0f) - 40.0f;


        }

        public void Render(SpriteBatch sbatch, SpriteFont font)
        {
            sbatch.Draw(tex, rotPosition, rect, Microsoft.Xna.Framework.Color.White, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            sbatch.DrawString(font, value.ToString(), position + new Vector2(100, 220), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
            
        }
    }
}

