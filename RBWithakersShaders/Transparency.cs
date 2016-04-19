using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace RBWithakersShaders
{
    public class Transparency : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(20, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        float angle = 0;
        float distance = 10;

        Model model; //, modelB;
        Texture2D texture;

        Vector3 cameraPosition;
        Skybox skybox;

        //Matrix[] choppersPos;

        Vector3 viewVector;

        float transparency;

        public Transparency()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            transparency = 0.2f;

            //choppersPos = new Matrix[10];

            //for (Byte i = 0; i < 10; i++)
            //{
            //    choppersPos[i] = Matrix.CreateTranslation(10, 1 + 3 * i, -30);
            //}

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            model = Content.Load<Model>("Models/Helicopter");
            //modelB = Content.Load<Model>("Models/UntexturedSphere");
            effect = Content.Load<Effect>("Effects/Transparency");
            texture = Content.Load<Texture2D>("Textures/HelicopterTexture");

            skybox = new Skybox("Skyboxes/Sunset", Content);
        }


        protected override void UnloadContent()
        {
            Content.Unload();
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            cameraPosition = distance * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
            Vector3 cameraTarget = new Vector3(0, 0, 0);
            viewVector = Vector3.Transform(cameraTarget - cameraPosition, Matrix.CreateRotationY(0));
            viewVector.Normalize();

            angle += 0.006f;
            view = Matrix.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.LightSeaGreen);
            GraphicsDevice.Clear(Color.Black);

            //for (Byte i = 0; i < 10; i++)
            //{
            //    // drawing cannot be mixed ??? DrawModel => exception
            //    DrawModel(modelB, choppersPos[i], view, projection);
            //}

            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;

            skybox.Draw(view, projection, cameraPosition);

            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;

            // warning: if the same model is used, even with too separated content.load calls,
            // the use of DrawModel don't work cause effect cannot be cast into BasicEffect type (is here any static somewhere ?)
            DrawModelWithEffect(model, world, view, projection);

            base.Draw(gameTime);
        }


        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = world * mesh.ParentBone.Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
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
                    effect.Parameters["ViewVector"].SetValue(viewVector);
                    effect.Parameters["ModelTexture"].SetValue(texture);
                    effect.Parameters["Transparency"].SetValue(transparency);

                    //effect.Parameters["AmbientColor"].SetValue(Color.Green.ToVector4());
                    //effect.Parameters["AmbientIntensity"].SetValue(0.5f);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                }
                mesh.Draw();
            }
        }
    }
}