using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Assignment2
{
    public class Prism
    {
        #region Variables
        private readonly BasicEffect _effect;
        private readonly GraphicsDevice _device;
        private readonly Texture2D _topTex;
        private readonly Texture2D _sideTex;

        private VertexBuffer _topVerticesBuffer;
        private VertexBuffer _baseVerticesBuffer;
        private VertexBuffer _sideVerticesBuffer;
        private IndexBuffer _iBuffer;
        private IndexBuffer _sideIndexBuffer;

        private VertexPositionNormalTexture [] _topVertices;
        private VertexPositionNormalTexture [] _baseVertices;
        private VertexPositionNormalTexture [] _sideVertices;

        private Vector3 _direction = new Vector3(0f, 0f, 0f);
        private float _height;
        private float _radius;
        private float _rotation = 0f;
        private int _sides;
        private short [] _indices;
        private short [] _sideIndices;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor that receives the params to build the Prism ** lim_sides: 3-x
        /// </summary>
        /// <param name="height"></param>
        /// <param name="sides"> can only be set from 3-x sides</param>
        public Prism(GraphicsDevice device, Texture2D topTex, Texture2D sideTex, float height, float radius, int sides)
        {
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

            _device = device;
            _sideTex = sideTex;
            _topTex = topTex;
            _radius = radius;
            _height = height;
            _sides = sides;
            
            _effect = new BasicEffect(device);
            _effect.View = Matrix.CreateLookAt(new Vector3(1.0f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
            _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.01f, 1000f);
            _effect.LightingEnabled = true;
            _effect.TextureEnabled = true;
            
            //LIGHTING
            _effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
            _effect.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
            _effect.DirectionalLight0.Enabled = true;
            _effect.DirectionalLight0.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            _effect.DirectionalLight0.Direction = new Vector3(-1f, -1f, -1f);
            _effect.DirectionalLight0.Direction.Normalize();
            _effect.CurrentTechnique.Passes[0].Apply();
            
            CreatePrism(height, radius, sides);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the prism
        /// </summary>
        public void CreatePrism(float height, float radius, int sides)
        {
            //sets the size of the vertices array accordingly
            _baseVertices = new VertexPositionNormalTexture [_sides + 1];
            _topVertices = new VertexPositionNormalTexture [_sides + 1];
            _sideVertices = new VertexPositionNormalTexture [_sides * 2 + 2];

            //iteration to create each vertex inside the array vertices
            for (int i = 0; i < _sides; i++)
            {
                float angle = (float)i * (float)(2 * Math.PI) / (float)_sides;
                float x = (float)Math.Cos(angle) * radius;
                float y = 0f;
                float z = (float)Math.Sin(angle) * radius;
                
                /* BASES */
                //bottom
                _baseVertices [i] = new VertexPositionNormalTexture(
                    new Vector3(x, y, z), Vector3.Down, new Vector2((x/radius + 1) * 0.5f,(z/radius + 1) * 0.5f));
                
                //top
                _topVertices [i] = new VertexPositionNormalTexture(
                    new Vector3(x, _height, -z),Vector3.Up, new Vector2((x/radius + 1) * 0.5f,(z/radius + 1) * 0.5f));
                
                
                /* SIDES */
                //does top and bottom vertices at a time
                _sideVertices [i * 2] = new VertexPositionNormalTexture(
                    new Vector3(x, _height, z), new Vector3(x, y, z), new Vector2(1f - (float)i/_sides,0));
                
                _sideVertices [i * 2 + 1] = new VertexPositionNormalTexture(
                    new Vector3(x, y, z),new Vector3(x, y, z), new Vector2(1f - (float)i/_sides,1f));
                
                //so on the last side it makes the last 2 vertices on the same position as the beginning (to overlap)
                if (i == _sides - 1)
                { 
                    //set the i to 0 by making the angle of 0
                    x = (float)Math.Cos(0) * radius;
                    z = (float)Math.Sin(0) * radius;
                    
                    //going to the last 2 vertex positions 
                    _sideVertices[_sides * 2] = new VertexPositionNormalTexture(
                        new Vector3(x, _height, z), new Vector3(x, y, z), new Vector2(1f - (float)i/_sides,0f));
                    
                    _sideVertices[_sides * 2 + 1] = new VertexPositionNormalTexture(
                        new Vector3(x, y, z), new Vector3(x, y, z), new Vector2(1f - (float)i/_sides,1f));
                }
            }
            
            _topVertices[_sides] = new VertexPositionNormalTexture(new Vector3(0f,_height, 0f), Vector3.Up, new Vector2(0.5f, 0.5f)); 
            
            int indexCount = _sides * 2 + 2;

            _indices = new short [indexCount];
            _sideIndices = new short[indexCount];
            
            for (short i = 0; i < _sides; i++)
            {
                _indices [2 * i] = (short)(i % _sides);
                _indices [2 * i + 1] = (short)_sides;
            }

            for (short i = 0; i < _sideVertices.Length; i++)
            {
                _sideIndices[i] = i;
            }
            
            //BUFFERS 
            
            _baseVerticesBuffer = new VertexBuffer(_device, typeof(VertexPositionNormalTexture), _baseVertices.Length, BufferUsage.None);
            _baseVerticesBuffer.SetData<VertexPositionNormalTexture>(_baseVertices);
            
            _topVerticesBuffer = new VertexBuffer(_device, typeof(VertexPositionNormalTexture), _topVertices.Length, BufferUsage.None);
            _topVerticesBuffer.SetData<VertexPositionNormalTexture>(_topVertices);

            _sideVerticesBuffer = new VertexBuffer(_device, typeof(VertexPositionNormalTexture), _sideVertices.Length, BufferUsage.None);
            _sideVerticesBuffer.SetData<VertexPositionNormalTexture>(_sideVertices);
            
            _sideIndexBuffer = new IndexBuffer(_device, typeof(short),_sideIndices.Length , BufferUsage.None);
            _sideIndexBuffer.SetData<short>(_sideIndices);

            _iBuffer = new IndexBuffer(_device, typeof(short), _indices.Length, BufferUsage.None);
            _iBuffer.SetData<short>(_indices);
        }
        
        /// <summary>
        /// Updates the Prism
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

           if (ks.IsKeyDown(Keys.Up))
           {
               _direction += 0.05f * _effect.World.Forward;
           }
           else if (ks.IsKeyDown(Keys.Down))
           {
               _direction += 0.05f * _effect.World.Backward;
           }
           else if (ks.IsKeyDown(Keys.Left))
           {
               _rotation += 0.05f;
           }
           else if (ks.IsKeyDown(Keys.Right))
           {
               _rotation -= 0.05f;
           }
           
           _effect.World = Matrix.CreateFromYawPitchRoll(_rotation, 0.0f, 0.0f) * Matrix.CreateTranslation(_direction);
        }
        
        /// <summary>
        /// Draws the Primitives
        /// </summary>
        public void Draw()
        {
            //_effect.World = Matrix.Identity;
            
            _effect.Texture = _sideTex;
            _effect.CurrentTechnique.Passes [0].Apply();
        
            _device.SetVertexBuffer(_sideVerticesBuffer);
            _device.Indices = _sideIndexBuffer;
            
            /*renders sides*/
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, _sideIndices.Length - 2);
            
            _effect.Texture = _topTex;
            _effect.CurrentTechnique.Passes [0].Apply();
            
            _device.SetVertexBuffer(_baseVerticesBuffer);
            _device.Indices = _iBuffer;
            
            /*renders Bottom base*/
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,0,
                0,_indices.Length - 2);
            
            _device.SetVertexBuffer(_topVerticesBuffer);

            /*renders Top base*/
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,0,
                0,_indices.Length - 2);
        }
        #endregion
    }
}