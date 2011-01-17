using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaFlixel
{
    public delegate void FlxButtonClick();

    /// <summary>
    /// A simple button class that calls a function when clicked by the mouse.
    /// Supports labels, highlight states, and parallax scrolling.
    /// </summary>
    public class FlxButton : FlxGroup
    {
        /// <summary>
        /// Set this to true if you want this button to function even while the game is paused.
        /// </summary>
        public bool pauseProof;

        /// <summary>
        /// Used for checkbox-style behavior.
        /// </summary>
        protected bool _onToggle;

        /// <summary>
        /// Stores the 'off' or normal button state graphic.
        /// </summary>
        protected FlxSprite _off;
        /// <summary>
        /// Stores the 'on' or highlighted button state graphic.
        /// </summary>
        protected FlxSprite _on;
        /// <summary>
        /// Stores the 'off' or normal button state label.
        /// </summary>
        protected FlxText _offT;
        /// <summary>
        /// Stores the 'on' or highlighted button state label.
        /// </summary>
        protected FlxText _onT;
        /// <summary>
        /// This function is called when the button is clicked.
        /// </summary>
        protected FlxButtonClick _callback;
        /// <summary>
        /// Tracks whether or not the button is currently pressed.
        /// </summary>
        protected bool _pressed;
        /// <summary>
        /// Whether or not the button has initialized itself yet.
        /// </summary>
        protected bool _initialized;
        /// <summary>
        /// Helper variable for correcting its members' <code>scrollFactor</code> objects.
        /// </summary>
        protected Vector2 _sf;

        /// <summary>
        /// Creates a new <code>FlxButton</code> object with a gray background
        /// and a callback function on the UI thread.
        /// 
        /// @param	X			The X position of the button.
        /// @param	Y			The Y position of the button.
        /// @param	Callback	The function to call whenever the button is clicked.
        /// </summary>
        public FlxButton(int X, int Y, FlxButtonClick Callback)
            : base()
        {
            x = X;
            y = Y;
            width = 100;
            height = 20;
            _off = new FlxSprite().createGraphic((int)width, (int)height, new Color(0x7f, 0x7f, 0x7f));
            _off.solid = false;
            add(_off, true);
            _on = new FlxSprite().createGraphic((int)width, (int)height, Color.White);
            _on.solid = false;
            add(_on, true);
            _offT = null;
            _onT = null;
            _callback = Callback;
            _onToggle = false;
            _pressed = false;
            _initialized = false;
            _sf = Vector2.Zero;
            pauseProof = false;
        }

        /// <summary>
        /// Set your own image as the button background.
        /// 
        /// @param	Image				A FlxSprite object to use for the button background.
        /// @param	ImageHighlight		A FlxSprite object to use for the button background when highlighted (optional).
        /// 
        /// @return	This FlxButton instance (nice for chaining stuff together, if you're into that).
        /// </summary>
        public FlxButton loadGraphic(FlxSprite Image, FlxSprite ImageHighlight)
        {
            _off = replace(_off, Image) as FlxSprite;
            if (ImageHighlight == null)
            {
                if (_on != _off)
                    remove(_on);
                _on = _off;
            }
            else
                _on = replace(_on, ImageHighlight) as FlxSprite;
            _on.solid = _off.solid = false;
            _off.scrollFactor = scrollFactor;
            _on.scrollFactor = scrollFactor;
            width = _off.width;
            height = _off.height;
            refreshHulls();
            return this;
        }

        /// <summary>
        /// Add a text label to the button.
        /// 
        /// @param	Text				A FlxText object to use to display text on this button (optional).
        /// @param	TextHighlight		A FlxText object that is used when the button is highlighted (optional).
        /// 
        /// @return	This FlxButton instance (nice for chaining stuff together, if you're into that).
        /// </summary>
        public FlxButton loadText(FlxText Text, FlxText TextHighlight)
        {
            if (Text != null)
            {
                if (_offT == null)
                {
                    _offT = Text;
                    add(_offT);
                }
                else
                    _offT = replace(_offT, Text) as FlxText;
            }
            if (TextHighlight == null)
                _onT = _offT;
            else
            {
                if (_onT == null)
                {
                    _onT = TextHighlight;
                    add(_onT);
                }
                else
                    _onT = replace(_onT, TextHighlight) as FlxText;
            }
            _offT.scrollFactor = scrollFactor;
            _onT.scrollFactor = scrollFactor;
            return this;
        }


        /// <summary>
        /// Called by the game loop automatically, handles mouseover and click detection.
        /// </summary>
        override public void update()
        {
            if (!_initialized)
            {
                if (FlxG.state == null) return;
                FlxG.mouse.addMouseListener(onMouseUp);
                _initialized = true;
            }

            base.update();

            visibility(false);
            if (overlapsPoint(FlxG.mouse.x, FlxG.mouse.y))
            {
                if (!FlxG.mouse.pressed())
                    _pressed = false;
                else if (!_pressed)
                    _pressed = true;
                visibility(!_pressed);
            }
            if (_onToggle) visibility(_off.visible);
        }

        /// <summary>
        /// Use this to toggle checkbox-style behavior.
        /// </summary>
        public bool on
        {
            get
            {
                return _onToggle;
            }
            set
            {
                _onToggle = value;
            }
        }

        /// <summary>
        /// Called by the game state when state is changed (if this object belongs to the state)
        /// </summary>
        override public void destroy()
        {
            if (FlxG.mouse != null)
                FlxG.mouse.removeMouseListener(onMouseUp);
        }

        override public void render(SpriteBatch spriteBatch)
        {
            base.render(spriteBatch);
            if ((_off != null) && _off.exists && _off.visible) _off.render(spriteBatch);
            if ((_on != null) && _on.exists && _on.visible) _on.render(spriteBatch);
            if (_offT != null)
            {
                if ((_offT != null) && _offT.exists && _offT.visible) _offT.render(spriteBatch);
                if ((_onT != null) && _onT.exists && _onT.visible) _onT.render(spriteBatch);
            }
        }

        /// <summary>
        /// Internal function for handling the visibility of the off and on graphics.
        /// 
        /// @param	On		Whether the button should be on or off.
        /// </summary>
        protected void visibility(bool On)
        {
            if (On)
            {
                _off.visible = false;
                if (_offT != null) _offT.visible = false;
                _on.visible = true;
                if (_onT != null) _onT.visible = true;
            }
            else
            {
                _on.visible = false;
                if (_onT != null) _onT.visible = false;
                _off.visible = true;
                if (_offT != null) _offT.visible = true;
            }
        }

        /// <summary>
        /// Internal function for handling the actual callback call (for UI thread dependent calls like <code>FlxU.openURL()</code>).
        /// </summary>
        private void onMouseUp(object Sender, FlxMouseEvent MouseEvent)
        {
            if (!exists || !visible || !active || !FlxG.mouse.justReleased() || (FlxG.pause && !pauseProof) || (_callback == null)) return;
            if (overlapsPoint(FlxG.mouse.x, FlxG.mouse.y)) _callback();
        }

    }
}
