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
    class Graveyard
    {
        public Vector2 position;
        public float layer;
        private Texture2D tex;
        private Rectangle rect;
        public Vector2 tlcorner;
        public Vector2 brcorner;
        public List<MenuAction> menuActions;

        public Graveyard(Vector2 position, float drawLayer, Texture2D tex, Vector2 topLeftWindowCorner, Vector2 bottomRightWindowCorner, List<MenuAction> menuActions)
        {
            this.position = position;
            this.layer = drawLayer;
            this.tex = tex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.tlcorner = topLeftWindowCorner;
            this.brcorner = bottomRightWindowCorner;
            this.menuActions = menuActions;
        }

        public void Render(SpriteBatch sbatch)
        {
            sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
        }
    }
}

