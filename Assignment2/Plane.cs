using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment2
{
    public class Plane
    {
        #region Variables
        private BasicEffect _effect;
        private GraphicsDevice _device;
        private Texture2D _texture;
        private VertexPositionTexture[] _vertices;
        public int size;
        
        private short[] _indices;
        #endregion
        
        #region Constructor
        public Plane(GraphicsDevice device, Texture2D texture)
        {
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            
            _device = device;
            _texture = texture;
            
            _effect = new BasicEffect(device);
            _effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.01f, 1000f);
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = false;
            _effect.TextureEnabled = true;
            
            _effect.Texture = _texture;
            
            CreatePlane();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the plane
        /// </summary>
        private void CreatePlane()
        {
            const float planeWidth = 1f;
            
            _vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-planeWidth, 0.0f, -planeWidth), new Vector2(0.0f, 0.0f)),
                new VertexPositionTexture(new Vector3(planeWidth, 0.0f, -planeWidth), new Vector2(1.0f, 0.0f)),
                new VertexPositionTexture(new Vector3(planeWidth, 0.0f, planeWidth), new Vector2(1.0f, 1.0f)),
                new VertexPositionTexture(new Vector3(-planeWidth, 0.0f, planeWidth), new Vector2(0.0f, 1.0f))
            };

            _indices = new short[]
            {
               0,1,3,2
            };
        }
        
        /// <summary>
        /// Draws the plane
        /// </summary>
        public void Draw()
        {
            _effect.World = Matrix.Identity;
            _effect.CurrentTechnique.Passes[0].Apply();

            _device.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, _vertices, 0, _vertices.Length, _indices, 0,2);
        }    
        #endregion
    }
}