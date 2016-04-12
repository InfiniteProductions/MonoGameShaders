using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DEffect_test
{
    public class LightsExtended : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D lightMask;
        Texture2D surge;

        RenderTarget2D lightsTarget;
        RenderTarget2D mainTarget;

        Effect lightingEffect;

        Vector2 lightPosition;
        float lightSpeed = 5;


        public LightsExtended()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            lightPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            lightMask = Content.Load<Texture2D>("lightmask");
            surge = Content.Load<Texture2D>("surge");

            lightingEffect = Content.Load<Effect>("Effect2b");

            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                lightPosition.Y -= lightSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                lightPosition.Y += lightSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                lightPosition.X -= lightSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                lightPosition.X += lightSpeed;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            // Create a Light Mask to pass to the pixel shader
            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(lightMask, lightPosition, Color.White);
            spriteBatch.End();

            // Draw the main scene to the Render Target
            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.White);  // change color to change light tint/color
            spriteBatch.Begin();
            spriteBatch.Draw(surge, new Vector2(100, 0), Color.White);
            spriteBatch.Draw(surge, new Vector2(250, 250), Color.White);
            spriteBatch.Draw(surge, new Vector2(550, 225), Color.White);
            spriteBatch.End();

            // Draw the main scene with a pixel
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            lightingEffect.Parameters["lightMask"].SetValue(lightsTarget);
            lightingEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
