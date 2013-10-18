using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameFramework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DiscoRoboGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DiscoRoboGame : GameHost
    {
        GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;


        public DiscoRoboGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            AddGameModeHandler(new ModeMenu(this));
            AddGameModeHandler(new ModeGame(this));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            Textures.Add("Balloon", Content.Load<Texture2D>("Balloon"));
            Textures.Add("UpArrow", Content.Load<Texture2D>("Arrows-Up-Circular"));
            Textures.Add("DownArrow", Content.Load<Texture2D>("Arrows-Down-Circular"));
            Textures.Add("LeftArrow", Content.Load<Texture2D>("Arrows-Left-Circular"));
            Textures.Add("RightArrow", Content.Load<Texture2D>("Arrows-Right-Circular"));
            Textures.Add("RedLine", Content.Load<Texture2D>("red_line"));

            // Load songs
            Songs.Add("BackgroundMusic", Content.Load<Song>("Please Tell Me Y - Free Style"));

            // Load fonts
            Fonts.Add("Kootenay", Content.Load<SpriteFont>("Kootenay"));

            //Reset the game
            //ResetGame();

            // Activate the initial handler
            SetGameMode<ModeMenu>();
            // Reset the handler
            CurrentGameModeHandler.Reset();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }        
    }
}
