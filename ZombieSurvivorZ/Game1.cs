﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace ZombieSurvivorZ
{
    public class Game1 : Game
    {
        public const bool CollisionDebugging = true;


        public static readonly Game1 Current = new();

        private static readonly Dictionary<string, Texture2D> TextureBank = new();
        public static GameWindow Screen { get; private set; }
        public static Vector2 ScreenCenter => new(Screen.ClientBounds.Width / 2, Screen.ClientBounds.Height / 2);
        public static Vector2 ScreenSize => Screen.ClientBounds.Size.ToVector2();



        public static GraphicsDeviceManager Graphics { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }

        public static Camera Camera { get; private set; }

        public static MapManager MapManager { get; private set; }
        public static Player Player { get; private set; }

        public static FiringLines FiringLines { get; private set; }

        public static UpgradeWindowUI UpgradeWindowUI { get; private set; }
        public static HUDDisplayUI HUDDisplayUI { get; private set; }
        public static bool UISuppressClick { get; set; } = false;


        private Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Screen = Window;
            IsMouseVisible = false;

            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.ApplyChanges();


            Camera = new();
            Collision.Initialize();

            base.Initialize();

        }

        public void SetCursorVisible(bool visible)
        {
            IsMouseVisible = visible;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Font.Load();
            ////_spriteBatch = new(GraphicsDevice);

            MapManager = new();
            Player = new();



            var c = new Crate()
            {
                Position = new(200, 100)
            };

            //c = new Crate()
            //{
            //    Position = new(-100, -100)
            //};
            //new Crate()
            //{
            //    Position = new(0, 0)
            //};
            //for (int x = 0; x < 32; x++)
            //{
            //    for (int y = 0; y < 32; y++)
            //    {
            //        new Crate()
            //        {
            //            Position = new(100 + x * 32, 100 + y * 32)
            //        };
            //    }
            //}

            FiringLines = new();
            UpgradeWindowUI = new();
            HUDDisplayUI = new();
        }

        public static Texture2D GetTexture(string name)
        {
            if (!TextureBank.TryGetValue(name, out Texture2D texture))
            {
                texture = Current.Content.Load<Texture2D>(name);
                TextureBank.Add(name, texture);
            }
            return texture;
        }

        public static T GetContent<T>(string name)
        {
            return Current.Content.Load<T>(name);
        }


        protected override void Update(GameTime gameTime)
        {
            Time.gameTime = gameTime;
            Time.time = (float)gameTime.TotalGameTime.TotalSeconds;
            Time.deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Time.frameCount++;

            Input.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UISuppressClick = false;
            //Objects Update
            World.UI.Update(gameTime);
            World.floor.Update(gameTime);
            World.objects.Update(gameTime);

            //Collision Update
            Collision.Update(gameTime);

            //Camera Update
            Camera.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            Matrix transformMatrix = Camera.GetViewMatrix();
            SpriteBatch.Begin(transformMatrix: transformMatrix);
            World.floor.Draw(SpriteBatch, gameTime);
            World.objects.Draw(SpriteBatch, gameTime);
            SpriteBatch.End();


            SpriteBatch.Begin();
            World.UI.Draw(SpriteBatch, gameTime);
            SpriteBatch.End();


            base.Draw(gameTime);
        }
    }
}