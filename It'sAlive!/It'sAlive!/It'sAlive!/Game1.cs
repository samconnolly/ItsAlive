using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.CSharp;


namespace It_sAlive_
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            // set window size
            graphics.PreferredBackBufferHeight = 780;
            graphics.PreferredBackBufferWidth = 1040;
            //graphics.IsFullScreen = true;

            Content.RootDirectory = "Content";
        }

     
        protected override void Initialize()
        {
           base.Initialize();
        }

        // Random number seed
        Random random;

        // Input Data Table

        DataTable machines;
        
        // Declare stuff here!

        Cursor cursor;
        
        // Excel database

        // Backdrops

        NonInteractive room;
        NonInteractive door;
        NonInteractive digger;
        Graveyard graveyard;
        
        // HUD

        //CountBar researchB;
        NumericalCounter research;
        NumericalCounter madness;
        NumericalCounter money;
        NumericalCounter papers;
        NumericalCounter lifeForce;
        NumericalCounter longevity;
        NumericalCounter humanity;

        List<MiniProgressBar> progBars;

        Build build;
        Resurrect resurrect;

        // items, peeps

        positionTextBlob blob;

        Scientist Simon;
        Assistant Jeremy;

        //FloorObject machine;
        //FloorObject machine2;

        FloorObject table;
        FloorObject desk;
        FloorObject bookcase;
        FloorObject electricalTransmitter;
        FloorObject lightningAbsorber;
        FloorObject steamComputer;
        FloorObject injectionSystem;
        

        Corpse corpse;

        // lists

        List<FloorObject> floorObjectList;
        List<MenuAction> actionUpdateList;
        List<MenuAction> menuActions;

        // coordinate system, navigation

        //PerspectiveGrid grid;
        Grid grid;
        ReachableArea reachable;
        Path path;
        Path assPath;
        List<Vector2> drawPath;
        List<Vector2> assDrawPath;

        // menu actions

        MenuAction turnOn;
        MenuAction turnOff;
        MenuAction getCorpse;
        MenuAction studyCorpse;
        MenuAction studyLiveCorpse;
        MenuAction dissectCorpse;
        MenuAction partWork;
        MenuAction writePaper;
        MenuAction study;
        MenuAction lightning;
        MenuAction electric;
        MenuAction talk;

        // fonts

        SpriteFont cursorFont;

        protected override void LoadContent()
        {
          
            // load stuff here!

            // random seed
            random = new Random();

            // data table



            // graphics

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // fonts

            cursorFont = Content.Load<SpriteFont>("font");

            // grid
            //grid = new PerspectiveGrid(770, 1500, 400, 1880, 7, 10, new Vector2(10, 1070)); // perspective grid
            grid = new Grid(900, 640, 100, 10, 10, new Vector2(60, 700), false, Content.Load<Texture2D>("rball"),Content.Load<Texture2D>("gball"));


            // HUD

            //researchB = new CountBar(GraphicsDevice, new Vector2(10, 10), 40, 150, 100, 50, Color.Blue, Color.Gray);

            research = new NumericalCounter("Research", new Vector2(10, 170), 0, 100, 0, 0, cursorFont, Color.Green);
            madness = new NumericalCounter("Madness", new Vector2(10, 190), 0, 0, 0, 0, cursorFont, Color.Green);
            money = new NumericalCounter("Money", new Vector2(10, 210), 0, 500, 0, 60, cursorFont, Color.Yellow, true);
            papers = new NumericalCounter("Papers Published", new Vector2(10, 240), 0, 0, 0, 0, cursorFont, Color.Black);
            lifeForce = new NumericalCounter("Life Force", new Vector2(10, 270), 100, 100, 0, 0, cursorFont, Color.Black);
            longevity = new NumericalCounter("Longevity", new Vector2(10, 300), 100, 30, 0, 0, cursorFont, Color.Black);
            humanity = new NumericalCounter("Humanity", new Vector2(10, 330), 100, 30, 0, 0, cursorFont, Color.Black);

            progBars = new List<MiniProgressBar> { };
            build = new Build(new Vector2(5, 715), Content.Load<Texture2D>("buildIcon"), Content.Load<Texture2D>("buildIconH"), Content.Load<Texture2D>("invarrow"), Content.Load<Texture2D>("highinvarrow"), GraphicsDevice);
            resurrect = new Resurrect(new Vector2(880, 615), Content.Load<Texture2D>("reanimate"), Content.Load<Texture2D>("reanimateHigh"), GraphicsDevice);

            // menu actions

            turnOn = new MenuAction("Turn on",true,2,5,1,0,0,0,0,true,true);
            turnOff = new MenuAction("Turn off", true, 2, 5, 1, 0, 0, 0, 0, false,false,true);
            
            getCorpse = new MenuAction("Exhume Cadaver", true, 15, 0, 1, 0, 0, 0, 0, true);
            studyCorpse = new MenuAction("Study Corpse", true, 5, 10, 6, 0, 0, 0, 0, true);
            
            dissectCorpse = new MenuAction("Dissect Corpse", true, 7, 18 * 3, 3 * 10, 0, 0, 0, 0, true);
            study = new MenuAction("Study", true, 10, 50, 0, 0, 0, 0, 0, true);
            

            // dependant actions
            studyLiveCorpse = new MenuAction("Study Corpse", true, 5, 30, 4, 0, 0, 0, 0, true,false,false,new List<NumericalCounter>{humanity,longevity},0.1f,0,0,0,0,0);
            talk = new MenuAction("Talk", true, 10, 20, 1, 0, 0, 0, 0, true, false, false,new List < NumericalCounter >{humanity}, 0.5f, 0, 0, 0, 0, 0);
            partWork = new MenuAction("Part Time Work", true, 10, 15, 0, 0, 0, 0, 0, true);
            writePaper = new MenuAction("Write Paper", true, 10, 0, 0, 0, 0, 0, 0, true, false,false, new List < NumericalCounter >{research}, 0, 0, 0.9f, 0, 0, 0);
            lightning = new MenuAction("Activate Absorber", false, 5, 0, 0, 0, 15, 5, 2, true, false,false, new List < NumericalCounter >{research}, 0, 0, 0, 0.1f, 0.1f, 0.1f);
            electric = new MenuAction("Transfer Electrical Discharge", true, 5, 0, 0, 0, 5, 10, 2, true, false,false, new List < NumericalCounter >{research}, 0, 0, 0, 0.1f, 0.1f, 0.1f);

            // list of dependent actions to update
            actionUpdateList = new List<MenuAction>();
            actionUpdateList.Add(partWork);
            actionUpdateList.Add(writePaper);


            // background stuff

            room = new NonInteractive(Vector2.Zero, 0.6f, Content.Load<Texture2D>("room"));
            door = new NonInteractive(new Vector2(125, 298), 0.59f, Content.Load<Texture2D>("door"),2);
            graveyard = new Graveyard(Vector2.Zero, 0.8f, Content.Load<Texture2D>("back"),new Vector2(450,140),new Vector2(725,480),new List<MenuAction>{getCorpse});
            digger = new NonInteractive(new Vector2(440, 120), 0.79f, Content.Load<Texture2D>("digger"), 1, 10);
            


            // stuff

            cursor = new Cursor(Content.Load<Texture2D>("cursor"),GraphicsDevice);
            
            blob = new positionTextBlob(Content.Load<Texture2D>("gball"),new Vector2(1,1));
            
            Simon = new Scientist(Content.Load<Texture2D>("tmpprof"), new Vector2(6,1),grid);
            Jeremy = new Assistant(Content.Load<Texture2D>("tmpass"), new Vector2(1, 1), grid);

            //machine = new FloorObject(Content.Load<Texture2D>("machine"),null,1, new Vector2(1, 2), grid,"machine",10,menuActions = new List<MenuAction>{turnOn,turnOff,explode});
            //machine2 = new FloorObject(Content.Load<Texture2D>("machine2"),null,4, new Vector2(10,10), grid, "Other Machine",10, menuActions = new List<MenuAction>{turnOn,turnOff,explode});
            table = new FloorObject(Content.Load<Texture2D>("table"), Content.Load<Texture2D>("tableicon"), 1,1, new Vector2(5, 5), grid, "Operating Table",0, menuActions = new List<MenuAction> {  });
            desk = new FloorObject(Content.Load<Texture2D>("desk"), null, 1,1, new Vector2(7, 10), grid, "Desk",10, menuActions = new List<MenuAction> {partWork,writePaper });
            bookcase = new FloorObject(Content.Load<Texture2D>("bookcase"), null, 1,1, new Vector2(9, 10), grid, "Bookcase",10, menuActions = new List<MenuAction> { study });
            lightningAbsorber = new FloorObject(Content.Load<Texture2D>("lightning"), Content.Load<Texture2D>("lightningicon"), 1,2, new Vector2(7, 3), grid, "Lightning Absorber",150, menuActions = new List<MenuAction> { lightning, turnOn,turnOff });
            electricalTransmitter = new FloorObject(Content.Load<Texture2D>("electric"), Content.Load<Texture2D>("electricicon"), 1,1, new Vector2(9, 2), grid, "Electrical Transference Device",150, menuActions = new List<MenuAction> { electric });
            steamComputer = new FloorObject(Content.Load<Texture2D>("machine2"), Content.Load<Texture2D>("machine2icon"), 4, 1, new Vector2(1, 9), grid, "Steam Powered Computer", 200, menuActions = new List<MenuAction> { });

            corpse = new Corpse(Content.Load<Texture2D>("corpse"), new List<MenuAction> { studyCorpse, dissectCorpse }, new List<MenuAction> { talk,studyLiveCorpse });

            // populate initial build list

            build.Add(table);
            build.Add(lightningAbsorber);
            build.Add(electricalTransmitter);
            build.Add(steamComputer);

           // initial list filling

            floorObjectList = new List<FloorObject>();
            //floorObjectList.Add(machine);
            //floorObjectList.Add(machine2);

            floorObjectList.Add(desk);
            floorObjectList.Add(bookcase);

            // navigation etc.
            reachable = new ReachableArea(grid, floorObjectList);
            path = new Path(reachable);
            assPath = new Path(reachable);



            // path testing

            drawPath = new List<Vector2>();
            assDrawPath = new List<Vector2>();
            //////////
        }

     
        protected override void UnloadContent()
        {
        
        }

      
        protected override void Update(GameTime gameTime)
        {
            // check for exit
            KeyboardState kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Escape))
                {
                   this.Exit();
                }
                

            // update stuff
            reachable.Update(floorObjectList);

            cursor.Update(floorObjectList,progBars,Simon,Jeremy, build,graveyard,table,corpse,money,assPath,assDrawPath,grid,resurrect,humanity,longevity,research, random);

            blob.Update(gameTime, grid);

            digger.Update(gameTime);

            //researchB.Update(gameTime);
            research.Update(gameTime);
            madness.Update(gameTime);
            money.Update(gameTime);

            longevity.Update(gameTime);
            humanity.Update(gameTime);
            lifeForce.Update(gameTime);

            resurrect.Update(corpse,lifeForce,humanity,longevity,lightningAbsorber,gameTime);

            corpse.Update(gameTime, dissectCorpse,studyCorpse,longevity,humanity,lifeForce,Simon,talk);

            drawPath = Simon.Update(gameTime,GraphicsDevice, grid,cursor,path,drawPath, research,money,madness,progBars);
            assDrawPath = Jeremy.Update(gameTime, GraphicsDevice, grid, cursor, assPath, assDrawPath, research, money, madness, progBars,corpse,door,digger);

            // update objects
            foreach (FloorObject floorObject in floorObjectList)
            {
                floorObject.Update(gameTime);
            }

            // create progress bars if necessary
            List<MiniProgressBar> remove = new List<MiniProgressBar> { };

            foreach (MiniProgressBar bar in progBars)
            {
                bar.Update(gameTime);
                if (bar.value >= bar.init)
                {
                    remove.Add(bar);
                }
            }

            // remove those no longer needed
            foreach (MiniProgressBar bar in remove)
            {
                bar.action.Complete(research, madness, money,longevity,lifeForce,humanity, bar.floorObject);
                progBars.Remove(bar);
            }

            // update dependant menu actions
            //foreach (MenuAction action in actionUpdateList)
            //{
            //    action.Update();
            //}

            //set number of papers to number of times you've written a paper
            papers.value = writePaper.count;

            // set income to number of papers * 0.1
            money.valueChange = papers.value * 0.1f;

            // UPDATE!   
            base.Update(gameTime);
        }

     
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // ============== Drawing code =============

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            cursor.Render(GraphicsDevice, spriteBatch, cursorFont,build,graveyard,corpse);

            graveyard.Render(spriteBatch);
            digger.Render(spriteBatch);
            room.Render(spriteBatch);
            door.Render(spriteBatch);

            blob.Render(spriteBatch);

            Simon.Render(spriteBatch);
            Jeremy.Render(spriteBatch);

            build.Render(spriteBatch, cursor);
            resurrect.Render(spriteBatch, cursor);
            //researchB.Render(spriteBatch);
            research.Render(spriteBatch);
            madness.Render(spriteBatch);
            money.Render(spriteBatch);
            papers.Render(spriteBatch);
            lifeForce.Render(spriteBatch);
            longevity.Render(spriteBatch);
            humanity.Render(spriteBatch);

            corpse.Render(spriteBatch);

            foreach (FloorObject floorObject in floorObjectList)
            {
                floorObject.Render(spriteBatch);
            }

            foreach (MiniProgressBar bar in progBars)
            {
                bar.Render(spriteBatch);
            }

            grid.Draw(GraphicsDevice, spriteBatch,reachable);
            //Tgrid.Draw(GraphicsDevice, spriteBatch);

            //path.Draw(GraphicsDevice, spriteBatch, drawPath, Vector2.Zero,grid);
            //spriteBatch.DrawString(cursorFont, drawPath.ToString(), new Vector2(300, 120), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // path


            //spriteBatch.DrawString(cursorFont, blob.scale.ToString(), new Vector2(300,30), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // blob scale size
            //spriteBatch.DrawString(cursorFont, grid.angle.ToString(), new Vector2(300,30), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // grid angle of blob
            spriteBatch.DrawString(cursorFont, blob.gridPosition.ToString(), new Vector2(300, 0), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // grid position of blob
            //spriteBatch.DrawString(cursorFont, blob.position.ToString(), new Vector2(300, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // actual position of blob
            //spriteBatch.DrawString(cursorFont, grid.gridY.ToString(), new Vector2(300, 120), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // grid y of blob...
            //spriteBatch.DrawString(cursorFont, machine.position.ToString(), new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // machine position

            //if (cursor.menuHighlightAction != null)
            //{
            //    spriteBatch.DrawString(cursorFont, cursor.menuHighlightAction.name, new Vector2(300, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // highlighted action
            //}
            //.DrawString(cursorFont, cursor.scientistAction, new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // scientist current action
            //spriteBatch.DrawString(cursorFont, cursor.menuMouseover.ToString(), new Vector2(300, 120), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // menu mouseover?
            spriteBatch.DrawString(cursorFont, cursor.position.ToString(), new Vector2(300, 30), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // cursor position

            //spriteBatch.DrawString(cursorFont, Simon.targetDistance.ToString(), new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // to walk
            //spriteBatch.DrawString(cursorFont,Simon.distanceGone.ToString(), new Vector2(300, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // walked
            //spriteBatch.DrawString(cursorFont, Simon.gridPosition.ToString(), new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // Sci Grid position
           // spriteBatch.DrawString(cursorFont,build.menu.ToString(), new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // build menu on/off
            //spriteBatch.DrawString(cursorFont, Simon.corpseWork.ToString(), new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // corpse work?
            //spriteBatch.DrawString(cursorFont, Simon.animStart.ToString(), new Vector2(300, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // anim start?
            //spriteBatch.DrawString(cursorFont, lightningAbsorber.on.ToString(), new Vector2(300, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // turned on?
            spriteBatch.DrawString(cursorFont, "Alive: "+corpse.alive.ToString(), new Vector2(300, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // alive?
            spriteBatch.DrawString(cursorFont, "Fail: "+resurrect.fail.ToString(), new Vector2(300, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // fail?

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
