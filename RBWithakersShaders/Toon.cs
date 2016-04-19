using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RBWithakersShaders
{
    public class Toon : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(20, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);
        Vector3 cameraPosition;
        float angle = 0;
        float distance = 20;

        private Model model;
        private Effect effect;
        private Texture2D texture;

        Vector3 viewVector;

        public Toon()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>("Textures/HelicopterTexture");
            model = Content.Load<Model>("Models/Helicopter");
            effect = Content.Load<Effect>("Effects/Toon");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            cameraPosition = distance * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
            Vector3 cameraTarget = new Vector3(0, 0, 0);
            viewVector = Vector3.Transform(cameraTarget - cameraPosition, Matrix.CreateRotationY(0));
            viewVector.Normalize();

            angle += 0.01f;
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            DrawModelWithEffect(model, world, view, projection);

            base.Draw(gameTime);
        }


        private void DrawModelWithEffect(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                    effect.Parameters["World"].SetValue(world * mesh.ParentBone.Transform);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(world * mesh.ParentBone.Transform)));
                    effect.Parameters["Texture"].SetValue(texture);
                }
                mesh.Draw();
            }
        }
    }
}