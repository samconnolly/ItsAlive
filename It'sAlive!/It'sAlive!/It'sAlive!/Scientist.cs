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
    class Scientist
    {

        // texture
        public Texture2D charTex;
        private Rectangle rect;
        public int width = 0;
        public int height = 0;

        // animation
        public int timer = 0;
        public int msecsTweenFrames = 120;
        public int currentFrame = 0;
        public int numberOfFrames = 4;
        public int numberOfAnims = 5;
        public int anim = 0;
        private float scale = 5.0f;
        private float layer;

        // bools
        public bool walking = false;
        public bool doing = false;
        public bool corpseWork = false;
        public bool animStart = true;
        public bool animating = false;

        // walking
        public List<Vector2> drawPath;
        public Path path;
        public Grid grid;
        
        public Vector2 gridPosition = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 walkingOffset = Vector2.Zero;
        public Vector2 walkingTarget = Vector2.Zero;
        public Vector2 direction = Vector2.Zero;
        public Vector2 offset = Vector2.Zero;
        public Vector2 corpsePosition;
        
        public double targetDistance = 0;
        public double distanceGone = 0;
        
        public int pathStep = 0;

        // actions
        public MenuAction action = null;
        public FloorObject floorObject = null;
        public MiniProgressBar progBar;

        // The public instance of the object

        public Scientist(Texture2D texture, Vector2 gridPosition, Grid grid, ReachableArea reachable, FloorObject table)
        {
            // texture & position
            this.charTex = texture;
            this.width = texture.Width / numberOfFrames;
            this.height = texture.Height / numberOfAnims;
            this.gridPosition = gridPosition;
            this.layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y;
            this.corpsePosition = table.gridPosition + new Vector2(1, -1);

            // set path to use reachable area
            this.path = new Path(reachable);
            this.grid = grid;

            // set rectangle for animation
            rect.X = 0;
            rect.Y = 0;
            rect.Width = width;
            rect.Height = height;


        }

        // Animate corpse - set to walk behind table and laugh

        public void Animate(Resurrect resurrect)
        {
            animating = true;

            if (gridPosition != resurrect.sciPos)
            {
                walking = true;
                drawPath = path.PathList(gridPosition, resurrect.sciPos, grid);
                pathStep = 1;
                walkingTarget = drawPath[pathStep];
            }

            else
            {
                doing = true;
            }
        }

        public void Update(GameTime gametime, GraphicsDevice graphicsDevice, Cursor cursor, NumericalCounter research,NumericalCounter money, NumericalCounter madness, List<MiniProgressBar> proglist, ReachableArea reachable)
        {
            // get most recent reachable areas
            path.Update(reachable);

            // update position from grid position
                        
            position = grid.CartesianCoords(gridPosition);
            layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y - 0.01f;

            offset = new Vector2((width * scale) / 2.0f, height * scale); // factor of 2 here and in the draw command are just for this test anim, so it's a decent size...

            // walking....

            if (action != null && walking == false && doing == false)
            {
                // to corpse
                if (corpseWork == true)
                {
                    if (gridPosition != corpsePosition)
                    {
                        walking = true;
                        drawPath = path.PathList(gridPosition, corpsePosition, grid);
                        pathStep = 1;
                        walkingTarget = drawPath[pathStep];
                    }

                    else
                    {
                        doing = true;
                        currentFrame = 0;
                        gridPosition = drawPath[pathStep - 1];
                        position = grid.CartesianCoords(gridPosition);
                    }
                }
                    
                // to machine
                else if (gridPosition !=  floorObject.opPos)
                {
                    walking = true;
                    drawPath = path.PathList(gridPosition, floorObject.opPos, grid);
                    pathStep = 1;
                    walkingTarget = drawPath[pathStep];
                }

                // arrived at destination, start doing
                else
                {                    
                    doing = true;
                    currentFrame = 0;
                    gridPosition = drawPath[pathStep - 1];
                    position = grid.CartesianCoords(gridPosition);
                }
            }

            if (walking == true)
            {
                direction = (grid.CartesianCoords(walkingTarget) - grid.CartesianCoords(gridPosition));
                targetDistance = direction.Length();
                direction.Normalize();

                walkingOffset += direction * gametime.ElapsedGameTime.Milliseconds * 0.2f;
                distanceGone = walkingOffset.Length();

                if (distanceGone < targetDistance)
                {
                    Vector2 move = walkingOffset;
                    position += move;
                }

                else
                {
                    pathStep += 1;


                    if (pathStep < drawPath.Count)
                    {
                        gridPosition = drawPath[pathStep - 1];
                        walkingTarget = drawPath[pathStep];
                        position = grid.CartesianCoords(gridPosition);
                    }

                    else
                    {
                        walking = false;
                        doing = true;
                        currentFrame = 0;
                        gridPosition = drawPath[pathStep-1];
                        position = grid.CartesianCoords(gridPosition);
                    }

                    walkingOffset = Vector2.Zero;
                    targetDistance = 0;
                    distanceGone = 0;


                }
            }

            // update animation frame

            timer += gametime.ElapsedGameTime.Milliseconds;


            if (timer >= msecsTweenFrames)
            {
                timer = 0;

                if (walking == true)
                {
                    if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
                    {
                        if (direction.X >= 0)
                        {
                            anim = 1;
                        }

                        else
                        {
                            anim = 0;
                        }

                        if (direction.Y >= 0)
                        {
                            layer -= 0.2f / (float)grid.rows + 0.01f;
                        }
                    }

                    if (Math.Abs(direction.Y) >= Math.Abs(direction.X))
                    {
                        if (direction.Y >= 0)
                        {
                            anim = 2;
                            layer -= 0.2f / (float)grid.rows + 0.01f;
                        }

                        else
                        {
                            anim = 3;
                        }
                    }

                    if (currentFrame++ == numberOfFrames - 1)
                    {
                        currentFrame = 0;
                    }
                }


                // start up menu action if doing is done, animate doing
                if (doing == true)
                {
                    anim = 4;

                    if (action != null)
                    {
                        // if not staying, run anim once, start progress bar
                        if (action.remain == false)
                        {
                            if (currentFrame++ == numberOfFrames - 1)
                            {
                                doing = false;
                                proglist.Add(new MiniProgressBar(graphicsDevice, floorObject.position + new Vector2(-5, -105), action, floorObject));
                                progBar = proglist[-1];
                                action = null;

                            }
                        }

                        // if staying to work machine...
                        else
                        {
                            // if starting, create progress bar and start anim
                            if (animStart == true)
                            {
                                animStart = false;

                                if (corpseWork == true)
                                {
                                    proglist.Add(new MiniProgressBar(graphicsDevice, position + new Vector2(0, -100), action, null));
                                    progBar = proglist[proglist.Count -1];
                                }

                                else
                                {
                                    proglist.Add(new MiniProgressBar(graphicsDevice, floorObject.position + new Vector2(-5, -105), action, floorObject));
                                    progBar = proglist[proglist.Count -1];
                                }
                            }

                            // run anim until machine work is finished
                            else
                            {
                                if (currentFrame++ == numberOfFrames - 1)
                                {
                                    currentFrame = 0;

                                    if (action.done == true)
                                    {
                                        doing = false;
                                        action.done = false;
                                        action = null;
                                        animStart = true;

                                        if (corpseWork == true)
                                        {
                                            corpseWork = false;
                                        }
                                    }
                                }


                            }
                        }

                    }

                    // animate animation if animating....
                    else if (animating == true)
                    {
                        anim = 4;

                        if (currentFrame++ == numberOfFrames - 1)
                        {
                            doing = false;
                            animating = false;

                        }

                    }
                }

                // if not walking, stand!
                if (walking == false && doing == false)
                {
                    anim = 2;
                    currentFrame = 0;
                }

                // set anim and frame
                rect.X = currentFrame * width;
                rect.Y = height * anim;
            }
            
        }

        // Render!
        public void Render(SpriteBatch sbatch)
        {
            // scaling from perspective grid class!

            sbatch.Draw(charTex, position - offset, rect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);


        }
    }


}

