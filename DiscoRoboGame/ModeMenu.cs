using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace DiscoRoboGame
{
    internal class ModeMenu : GameModeBase
    {
        private DiscoRoboGame _game;
        public ModeMenu(DiscoRoboGame game)
            : base(game)
        {
            _game = game;
        }

        public override void Reset()
        {
            base.Reset();

            TextObject gameText;

            // Clear any existing object
            GameObjects.Clear();

            // Add the title
            gameText = new TextObject(_game, _game.Fonts["Kootenay"],
                                      new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f,
                                                  _game.GraphicsDevice.Viewport.Height * 0.1f), "TOSY Disco Robo",
                                      TextObject.TextAlignment.Center, TextObject.TextAlignment.Near);
            gameText.Scale = new Vector2(1.25f);
            GameObjects.Add(gameText);

            // Add the new game option
            gameText = new TextObject(_game, _game.Fonts["Kootenay"],
                                      new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f,
                                                  _game.GraphicsDevice.Viewport.Height * 0.3f), "New game",
                                      TextObject.TextAlignment.Center, TextObject.TextAlignment.Near);
            //gameText.Scale = new Vector2(0.6f);
            gameText.Tag = "NewGame";
            GameObjects.Add(gameText);

            // Add the continue game option
            gameText = new TextObject(_game, _game.Fonts["Kootenay"],
                                      new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f,
                                                  _game.GraphicsDevice.Viewport.Height * 0.4f), "Continue game",
                                      TextObject.TextAlignment.Center, TextObject.TextAlignment.Near);
            //gameText.Scale = new Vector2(0.6f);
            gameText.Tag = "ContinueGame";
            gameText.SpriteColor = new Color(Color.Black, 40); // Fade when no game to resume at first
            GameObjects.Add(gameText);

            // Add the high score option
            gameText = new TextObject(_game, _game.Fonts["Kootenay"],
                                      new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f,
                                                  _game.GraphicsDevice.Viewport.Height * 0.5f), "High scores",
                                      TextObject.TextAlignment.Center, TextObject.TextAlignment.Near);
            //gameText.Scale = new Vector2(0.6f);
            gameText.Tag = "HighScores";
            GameObjects.Add(gameText);

            // Add the setting option
            gameText = new TextObject(_game, _game.Fonts["Kootenay"],
                                      new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f,
                                                  _game.GraphicsDevice.Viewport.Height * 0.6f), "Settings",
                                      TextObject.TextAlignment.Center, TextObject.TextAlignment.Near);
            //gameText.Scale = new Vector2(0.6f);
            gameText.Tag = "Settings";
            GameObjects.Add(gameText);

            // Add the exit game option
            gameText = new TextObject(_game, _game.Fonts["Kootenay"],
                                      new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f,
                                                  _game.GraphicsDevice.Viewport.Height * 0.7f), "Exit",
                                      TextObject.TextAlignment.Center, TextObject.TextAlignment.Near);
            //gameText.Scale = new Vector2(0.6f);
            gameText.Tag = "Exit";
            GameObjects.Add(gameText);
        }

        public override void Activate()
        {
            base.Activate();

            // Have the game objects been loaded yet?
            if (GameObjects.Count == 0)
            {
                // No, so exit
                return;
            }

            // See if there is a game active at present
            if (Game.GetGameModeHandler<ModeGame>().GameIsActive)
            {
                // Yes, so enable the continue game option
                ((TextObject)Game.GetObjectByTag("ContinueGame")).SpriteColor = Color.White;
            }
            else
            {
                // No, so disable the continue game option
                ((TextObject)Game.GetObjectByTag("ContinueGame")).SpriteColor = new Color(Color.Black, 40);
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) _game.Exit();

            TouchCollection touches;
            SpriteObject touchedText;

            // Update all game objects
            _game.UpdateAll(gameTime);

            // Handle touched text
            touches = TouchPanel.GetState();
            if (touches.Count == 1 && touches[0].State == TouchLocationState.Pressed)
            {
                touchedText = Game.GetSpriteAtPoint(touches[0].Position);
                if (touchedText != null)
                {
                    switch (touchedText.Tag)
                    {
                        case "NewGame":
                            // Create a new game
                            Game.SetGameMode<ModeGame>();
                            Game.CurrentGameModeHandler.Reset();
                            break;
                        case "ContinueGame":
                            // Continue the last game active
                            if (Game.GetGameModeHandler<ModeGame>().GameIsActive)
                            {
                                Game.SetGameMode<ModeGame>();
                            }
                            break;
                        case "HighScores":
                            // Display the high score page
                            break;
                        case "Settings":
                            // Display the game settings
                            break;
                        case "Exit":
                            // Exit the game
                            _game.Exit();
                            break;
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.SteelBlue);

            _game._spriteBatch.Begin();
            _game.DrawText(gameTime, _game._spriteBatch);
            _game._spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
