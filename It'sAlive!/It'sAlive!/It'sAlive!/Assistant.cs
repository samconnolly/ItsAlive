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
    class Assistant
    {


        public Texture2D charTex;
        private Rectangle rect;

        public int width = 0;
        public int height = 0;
        public int timer = 0;
        public int outTimer = 0;
        public int msecsTweenFrames = 120;
        public int currentFrame = 0;
        public int numberOfFrames = 4;
        public int numberOfAnims = 6;
        public int anim = 0;
        private float scale = 5.0f;
        private float layer;

        public bool walking = false;
        public bool doing = false;
        public bool animStart = true;
        public bool digging = false;
        public bool dug = false;
        public bool outside = false;
        public bool corpseCarrying = false;
        public bool animating = false;
        public bool twoWork = false;
        public bool corpseWork = false;
        

        public Vector2 gridPosition = Vector2.Zero;
        public Vector2 defaultGridPosition = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 walkingOffset = Vector2.Zero;
        public Vector2 walkingTarget = Vector2.Zero;
        public Vector2 direction = Vector2.Zero;
        private Vector2 doorLocation = new Vector2(6, 20);
        private Vector2 tableLocation;
        public double targetDistance = 0;
        public double distanceGone = 0;
        public Vector2 offset = Vector2.Zero;
        public int pathStep = 0;

        public Grid grid;
        public Path path;
        public List<Vector2> drawPath;

        public MenuAction action = null;
        public FloorObject floorObject = null;
        public MiniProgressBar progBar;

        // The public instance of the object

        public Assistant(Texture2D texture, Vector2 startGridPosition, Grid grid, ReachableArea reachable,FloorObject table)
        {

            this.charTex = texture;
            this.width = texture.Width / numberOfFrames;
            this.height = texture.Height / numberOfAnims;

            this.gridPosition = startGridPosition;
            this.defaultGridPosition = startGridPosition;
            this.layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y;

            this.tableLocation = table.gridPosition + new Vector2(1,-1);

            // walking path
            this.path = new Path(reachable);
            this.grid = grid;

            // Create rectangle for animation

            rect.X = 0;
            rect.Y = 0;
            rect.Width = width;
            rect.Height = height;


        }

        // dig up corpse!
        public void DigUpCorpse(Corpse corpse)
        {
            if (corpse.visible == false)
            {
                digging = true;
                walking = true;
                drawPath = path.PathList(gridPosition, doorLocation, grid);
                pathStep = 1;
                walkingTarget = drawPath[pathStep];
                corpse.rot = 3;
            }          

        }


        public void Animate(Resurrect resurrect)
        {
            animating = true;
            walking = true;
            drawPath = path.PathList(gridPosition, resurrect.assPos, grid);
            pathStep = 1;
            walkingTarget = drawPath[pathStep];

        }

        
        // carryng a corpse back
        public void CarryCorpse()             
        {
            corpseCarrying = true;
            walking = true;
            drawPath = path.PathList(gridPosition,tableLocation , grid);
            pathStep = 1;
            walkingTarget = drawPath[pathStep];
        }

        public void Update(GameTime gametime, GraphicsDevice graphicsDevice, Grid grid, Cursor cursor, NumericalCounter research, NumericalCounter money, 
                                        NumericalCounter madness, List<MiniProgressBar> proglist, Corpse corpse,NonInteractive door,NonInteractive digger, Resurrect resurrect, NonInteractive Switch,
                                        NumericalCounter humanity, NumericalCounter longevity,NumericalCounter lifeForce, Random random, ReachableArea reachable)
        {

            path.Update(reachable);            
            
            if (outside == false)
            {
                position = grid.CartesianCoords(gridPosition);
                layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y - 0.01f;
            }

            offset = new Vector2((width * scale) / 2.0f, height * scale); // factor of 2 here and in the draw command are just for this test anim, so it's a decent size...

            // walking....

            // to use a machine
            if (action != null && walking == false && doing == false)
            {

                // to corpse
                if (corpseWork == true)
                {
                    if (gridPosition != tableLocation)
                    {
                        walking = true;
                        drawPath = path.PathList(gridPosition, tableLocation, grid);
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

                else if (gridPosition != floorObject.opPos)
                {
                    walking = true;

                    drawPath = path.PathList(gridPosition, floorObject.opPos, grid);

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

            
            

            else if (gridPosition != defaultGridPosition && walking == false && doing == false)
            {
                walking = true;
                drawPath = path.PathList(gridPosition, defaultGridPosition, grid);
                pathStep = 1;
                walkingTarget = drawPath[pathStep];
            }

            
                        
            // if he's walking, make him walk!

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
                        gridPosition = drawPath[pathStep - 1];
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
                    // coming through door (either way)
                    if (digging == true)
                    {
                        anim = 4;

                        if (door.animNum == 0)
                        {
                            door.SetAnim(1);
                        }
                           
                        if (currentFrame++ == 2)
                        {
                            
                            digging = false;

                            if (dug == true)
                            {
                                CarryCorpse();
                                doing = false;
                                dug = false;
                                outside = false;
                            }

                            else
                            {
                                outside = true;
                                layer = 0.61f;
                                position += new Vector2(0, -20);
                            }

                            currentFrame = 0;
                            door.SetAnim(0);
                        }

                        else if (dug == true)
                        {
                            layer = 0.595f;
                            anim = 5;
                        }
                    }

                    // animate digging outside
                    else if (outside == true)
                    {
                        outTimer += gametime.ElapsedGameTime.Milliseconds;

                                                

                        if (outTimer >= 150 && dug == false)
                        {
                            digger.anim = true;
                            dug = true;
                            outTimer = 0;
                        }

                        if (dug == true && digger.anim == false && outTimer >= 350)
                        {
                            digging = true;
                            outTimer = 0;
                             
                        }
                    }

                    // animate putting corpse on table
                    else if (corpseCarrying == true)
                    {

                        doing = false;
                        corpseCarrying = false;
                        corpse.visible = true;
                    }

                    // animate animation!
                    else if (animating == true)
                    {
                        anim = 4;

                        if (currentFrame++ == numberOfFrames - 1)
                        {
                            doing = false;
                            animating = false;
                            Switch.SetAnim(1);
                            resurrect.Alive(corpse, humanity, longevity,lifeForce, research,madness, random, this);                           
                        }                        
                    }

                    // if not in the usual spot...
                    else if (gridPosition != defaultGridPosition)
                    {
                        anim = 4;

                        if (action != null)
                        {
                            // if not staying to run machine, run doing anim once, create mini progress bar
                            if (action.remain == false)
                            {
                                if (currentFrame++ == numberOfFrames - 1)
                                {
                                    doing = false;
                                    proglist.Add(new MiniProgressBar(graphicsDevice, floorObject.position + new Vector2(-5, -105), action, floorObject));
                                    progBar = proglist[proglist.Count - 1];
                                    action = null;

                                }
                            }

                            // if staying to run machine...
                            else
                            {
                                // create a progess bar if starting
                                if (animStart == true)
                                {
                                    animStart = false;

                                    if (corpseWork == true)
                                    {
                                        proglist.Add(new MiniProgressBar(graphicsDevice, corpse.position + new Vector2(-5, -105), action, floorObject));
                                        progBar = proglist[proglist.Count - 1];
                                        corpseWork = false;     
                                    }

                                    else
                                    {
                                        proglist.Add(new MiniProgressBar(graphicsDevice, floorObject.position + new Vector2(-5, -105), action, floorObject));
                                        progBar = proglist[proglist.Count - 1];
                                    }
                                }

                                // run animation until finished
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

                                        }
                                    }

                                }
                            }
                        }
                    }

                    // if not doing anything, stop...
                    else
                    {
                        walking = false;
                        doing = false;
                        currentFrame = 0;
                    }


                }

                // if not walking, do standing anim
                if (walking == false && doing == false)
                {
                    anim = 2;
                    currentFrame = 0;
                }

                // set frame and anim
                rect.X = currentFrame * width;
                rect.Y = height * anim;
            }
                                    
        }

        // Render!
        public void Render(SpriteBatch sbatch)
        {
            // scaling from perspective grid class

            sbatch.Draw(charTex, position - offset, rect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);


        }
    }


}

