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
        private readonly int _vertexPerSide = 6;

        private VertexBuffer _topVerticesBuffer;
        private VertexBuffer _baseVerticesBuffer;
        private VertexBuffer _sideVerticesBuffer;
        private IndexBuffer _iBuffer;
        private IndexBuffer _sideIndexBuffer;

        private VertexPositionNormalTexture [] _topVertices;
        private VertexPositionNormalTexture [] _baseVertices;
        private VertexPositionNormalTexture [] _sideVertices;
        
        private int _height;
        private int _sides;
        private short [] _indices;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor that receives the params to build the Prism ** lim_sides: 3-x
        /// </summary>
        /// <param name="height"></param>
        /// <param name="sides"> can only be set from 3-x sides</param>
        public Prism(GraphicsDevice device, Texture2D topTex, Texture2D sideTex)
        {
            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

            _device = device;
            _sideTex = sideTex;
            _topTex = topTex;
            
            _effect = new BasicEffect(device);
            _effect.View = Matrix.CreateLookAt(new Vector3(4f, 4f, 4f), Vector3.Zero, Vector3.Up);
            _effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.01f, 1000f);
            _effect.LightingEnabled = false;
            _effect.VertexColorEnabled = true;
            _effect.TextureEnabled = true;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the prism
        /// </summary>
        public void CreatePrism(int height, float radius, int sides)
        {
            //variables
            _sides = sides;
            _height = height;

            //sets the size of the vertices array accordingly
            _baseVertices = new VertexPositionNormalTexture [_sides + 1];
            _topVertices = new VertexPositionNormalTexture [_sides + 1];
            _sideVertices = new VertexPositionNormalTexture [_sides * 2 + 2];

            //iteration to create each vertex inside the array vertices
            for (int i = 0; i < _sides; i++)
            {
                float angle = (float)i * (float)(2 * Math.PI) / (float)_sides;
                float x = (float)Math.Cos(angle);
                float y = 0f;
                float z = (float)Math.Sin(angle);

                _baseVertices [i] = new VertexPositionNormalTexture(new Vector3(x, y, z), Vector3.Down, new Vector2(x,z));
                _topVertices [i] = new VertexPositionNormalTexture(new Vector3(x, _height, -z),Vector3.Up, new Vector2(x,z));
                
                _sideVertices [i * 2] = new VertexPositionNormalTexture(
                    new Vector3(x, _height, z), new Vector3(x, y, z), new Vector2((x/radius + 1) * 0.5f,(z/radius + 1) * 0.5f));
                
                _sideVertices [i * 2 + 1] = new VertexPositionNormalTexture(
                    new Vector3(x, y, z),new Vector3(x, y, z), new Vector2((x/radius + 1) * 0.5f,(z/radius + 1) * 0.5f));
                
                //so on the last side it makes the last 2 vertex on the same position as the beginning (to overlap)
                if (i == _sides - 1)
                { 
                    //set the i to 0 by making the angle of 0
                    x = (float)Math.Cos(0);
                    z = (float)Math.Sin(0);
                    
                    //going to the last 2 vertex positions 
                    _sideVertices[_sides * 2] = new VertexPositionNormalTexture(
                        new Vector3(x, _height, z), new Vector3(x, y, z), new Vector2((x/radius + 1) * 0.5f,(z/radius + 1) * 0.5f));
                    
                    _sideVertices[_sides * 2 + 1] = new VertexPositionNormalTexture(
                        new Vector3(x, y, z), new Vector3(x, y, z), new Vector2((x/radius + 1) * 0.5f,(z/radius + 1) * 0.5f));
                }
            }

            int indexCount = _sides * 2 + 2;

            _indices = new short [indexCount];
            
            for (short i = 0; i < _sides; i++)
            {
                _indices [2 * i] = (short)(i % _sides);
                _indices [2 * i + 1] = (short)_sides;
            }

            //BUFFERS
            
            _baseVerticesBuffer = new VertexBuffer(_device, typeof(VertexPositionNormalTexture), _baseVertices.Length, BufferUsage.None);
            _baseVerticesBuffer.SetData<VertexPositionNormalTexture>(_baseVertices);
            
            _topVerticesBuffer = new VertexBuffer(_device, typeof(VertexPositionNormalTexture), _topVertices.Length, BufferUsage.None);
            _topVerticesBuffer.SetData<VertexPositionNormalTexture>(_topVertices);

            _sideVerticesBuffer = new VertexBuffer(_device, typeof(VertexPositionNormalTexture), _sideVertices.Length, BufferUsage.None);
            _sideVerticesBuffer.SetData<VertexPositionNormalTexture>(_sideVertices);
            
            _sideIndexBuffer = new IndexBuffer(_device, typeof(short), _sides + 1, BufferUsage.None);
            _sideIndexBuffer.SetData<short>();

            _iBuffer = new IndexBuffer(_device, typeof(short), indexCount, BufferUsage.None);
            _iBuffer.SetData<short>(_indices);
        }
        
        
        /// <summary>
        /// Draws the Primitives
        /// </summary>
        public void Draw()
        {
            _effect.Texture = _sideTex;
            _effect.CurrentTechnique.Passes [0].Apply();
        
            /*renders sides*/
            _device.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, _sideVertices, 0, 2 * _sides);
            
            _effect.Texture = _topTex;
            _effect.CurrentTechnique.Passes [0].Apply();
            
            /*renders Bottom base*/
            _device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                PrimitiveType.TriangleStrip,
                _baseVertices,
                0,
                _baseVertices.Length,
                _indices,
                0,
                _indices.Length - 2);
            
            _effect.Texture = _topTex;
            _effect.CurrentTechnique.Passes [0].Apply();

            /*renders Top base*/
            _device.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
             PrimitiveType.TriangleStrip,
             _topVertices,
             0,
             _topVertices.Length,
             _indices,
             0,
             _indices.Length - 2);
        }
        #endregion
    }
}