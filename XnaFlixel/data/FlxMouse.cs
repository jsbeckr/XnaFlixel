using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace XnaFlixel.data
{
    /// <summary>
    /// This class helps contain and track the mouse pointer in your game.
    /// Automatically accounts for parallax scrolling, etc.
    /// </summary>
    public class FlxMouse
    {
    	private Texture2D ImgDefaultCursor;

        private MouseState _curMouse;
        private MouseState _lastMouse;
        private EventHandler<FlxMouseEvent> _mouseEvent;

        public void addMouseListener(EventHandler<FlxMouseEvent> MouseEvent)
        {
            _mouseEvent += MouseEvent;
        }
        public void removeMouseListener(EventHandler<FlxMouseEvent> MouseEvent)
        {
            _mouseEvent -= MouseEvent;
        }

        public float x
        {
#if XBOX360
            get { return 0; }
#else
            get
            {
                //if (_curMouse.X >= FlxG._game.targetLeft)
                //{
                    return (float)((_curMouse.X - FlxG._flxGame.targetLeft) / FlxG.scale) - FlxG.scroll.X;
                //}
                //else
                //{
                //    return 0;
                //}
            }
#endif
        }
        public float y
        {
#if XBOX360
            get { return 0; }
#else
            get { return ((float)_curMouse.Y / FlxG.scale) - FlxG.scroll.Y; }
#endif
        }

		/// <summary>
		/// Current "delta" value of mouse wheel.  If the wheel was just scrolled up, it will have a positive value.  If it was just scrolled down, it will have a negative value.  If it wasn't just scroll this frame, it will be 0.
		/// </summary>
		public int wheel;

		/// <summary>
		/// Current X position of the mouse pointer on the screen.
		/// </summary>
		public int screenX;
		/// <summary>
		/// Current Y position of the mouse pointer on the screen.
		/// </summary>
		public int screenY;
		/// <summary>
		/// Graphical representation of the mouse pointer.
		/// </summary>
		public FlxSprite cursor;


		/// <summary>
		/// Constructor.
		/// </summary>
        public FlxMouse()
        {
			screenX = 0;
			screenY = 0;
            cursor = new FlxSprite();
            cursor.Visible = false;
        }

		public void show()
		{
			if (ImgDefaultCursor == null)
			{
				ImgDefaultCursor = FlxG.Game.Content.Load<Texture2D>("flixel/cursor");
			}

			show(ImgDefaultCursor, 0, 0);
		}

		/// <summary>
		/// Either show an existing cursor or load a new one.
		/// 
		/// @param	Graphic		The image you want to use for the cursor.
		/// @param	XOffset		The number of pixels between the mouse's screen position and the graphic's top left corner.
		////// @param	YOffset		The number of pixels between the mouse's screen position and the graphic's top left corner. 
		/// </summary>
        public void show(Texture2D Graphic)
        {
            show(Graphic, 0, 0);
        }

		public void show(Texture2D Graphic,int XOffset,int YOffset)
		{
			if(Graphic != null)
				load(Graphic,XOffset,YOffset);
			else if(cursor != null)
				cursor.Visible = true;
			else
				load(null);
		}
		
		/// <summary>
		/// Hides the mouse cursor
		/// </summary>
		public void hide()
		{
			if(cursor != null)
			{
				cursor.Visible = false;
			}
		}

		/// <summary>
		/// Load a new mouse cursor graphic
		/// 
		/// @param	Graphic		The image you want to use for the cursor.
		/// @param	XOffset		The number of pixels between the mouse's screen position and the graphic's top left corner.
		////// @param	YOffset		The number of pixels between the mouse's screen position and the graphic's top left corner. 
		/// </summary>
        public void load(Texture2D Graphic)
        {
            load(Graphic, 0, 0);
        }

        public void load(Texture2D Graphic, int XOffset, int YOffset)
		{
			if(Graphic == null)
				Graphic = ImgDefaultCursor;
			cursor = new FlxSprite(screenX,screenY,Graphic);
			cursor.Solid = false;
			cursor.offset.X = XOffset;
			cursor.offset.Y = YOffset;
		}

		/// <summary>
		/// Unload the current cursor graphic.  If the current cursor is visible,
		/// then the default system cursor is loaded up to replace the old one.
		/// </summary>
        public void unload()
		{
			if(cursor != null)
			{
				if(cursor.Visible)
					load(null);
				else
					cursor = null;
			}
		}

        /// <summary>
        /// Called by the internal game loop to update the mouse pointer's position in the game world.
        /// Also updates the just pressed/just released flags.
        /// </summary>
        public void update()
        {
            _lastMouse = _curMouse;
            _curMouse = Mouse.GetState();
            cursor.X = x;
            cursor.Y = y;

            if (_mouseEvent != null)
            {
                if (justPressed())
                {
                    _mouseEvent(this, new FlxMouseEvent(MouseEventType.MouseDown));
                }
                else if (justReleased())
                {
                    _mouseEvent(this, new FlxMouseEvent(MouseEventType.MouseUp));
                }
            }
        }

        public void reset()
        {
            _curMouse = _lastMouse;
            //also get rid of all current event listeners
            _mouseEvent = null;
        }

        public bool pressed()
        {
            return (_curMouse.LeftButton == ButtonState.Pressed ||
                _curMouse.RightButton == ButtonState.Pressed);
        }

        public bool justPressed()
        {
            return ((_curMouse.LeftButton == ButtonState.Pressed && _lastMouse.LeftButton == ButtonState.Released) ||
                (_curMouse.RightButton == ButtonState.Pressed && _lastMouse.RightButton == ButtonState.Released));
        }

        public bool justReleased()
        {
            return ((_curMouse.LeftButton == ButtonState.Released && _lastMouse.LeftButton == ButtonState.Pressed) ||
                (_curMouse.RightButton == ButtonState.Released && _lastMouse.RightButton == ButtonState.Pressed));
        }
    }

}
