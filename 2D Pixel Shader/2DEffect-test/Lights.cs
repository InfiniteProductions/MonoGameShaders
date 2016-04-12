using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2DEffect_test
{
    public class Lights : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D lightMask;
        Texture2D surge;

        RenderTarget2D lightsTarget;
        RenderTarget2D mainTarget;

        Effect lightingEffect;

        public Lights()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            lightMask = Content.Load<Texture2D>("lightmask");
            surge = Content.Load<Texture2D>("surge");

            lightingEffect = Content.Load<Effect>("Effect2");

            var pp = GraphicsDevice.PresentationParameters;
            lightsTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            mainTarget = new RenderTarget2D(GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            // Create a Light Mask to pass to the pixel shader
            GraphicsDevice.SetRenderTarget(lightsTarget);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            spriteBatch.Draw(lightMask, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(lightMask, new Vector2(100, 0), Color.White);
            spriteBatch.Draw(lightMask, new Vector2(200, 200), Color.White);
            spriteBatch.Draw(lightMask, new Vector2(300, 300), Color.White);
            spriteBatch.Draw(lightMask, new Vector2(500, 200), Color.White);
            spriteBatch.End();

            // Draw the main scene to the Render Target
            GraphicsDevice.SetRenderTarget(mainTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(surge, new Vector2(100, 0), Color.White);
            spriteBatch.Draw(surge, new Vector2(250, 250), Color.White);
            spriteBatch.Draw(surge, new Vector2(550, 225), Color.White);
            spriteBatch.End();

            // Draw the main scene with a pixel
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            lightingEffect.Parameters["lightMask"].SetValue(lightsTarget);
            lightingEffect.CurrentTechnique.Passes[0].Apply();
            spriteBatch.Draw(mainTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
