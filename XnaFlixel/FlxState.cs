using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaFlixel
{
    /// <summary>
    /// This is the basic game "state" object - e.g. in a simple game
    /// you might have a menu state and a play state.
    /// It acts as a kind of container for all your game objects.
    /// You can also access the game's background color
    /// and screen buffer through this object.
    /// FlxState is kind of a funny class from the technical side,
    /// it is just a regular Flash Sprite display object,
    /// with one member variable: a flixel <code>FlxGroup</code>.
    /// This means you can load it up with regular Flash stuff
    /// or with flixel elements, whatever works!
    /// </summary>
    public abstract class FlxState
    {
    	#region Constants

    	#endregion

    	#region Fields

    	/// <summary>
    	/// This static variable holds the screen buffer,
    	/// so you can draw to it directly if you want.
    	/// </summary>
    	//static public FlxSprite screen;
    	/// <summary>
    	/// This static variable indicates the "clear color"
    	/// or default background color of the game.
    	/// Change it at ANY time using <code>FlxState.bgColor</code>.
    	/// </summary>
    	static public Color bgColor;
    	/// <summary>
    	/// Internal group used to organize and display objects you add to this state.
    	/// </summary>
    	public FlxGroup defaultGroup;

    	#endregion

    	#region Properties

    	#endregion

    	#region Constructors

    	/// <summary>
    	/// Creates a new <code>FlxState</code> object,
    	/// instantiating <code>screen</code> if necessary.
    	/// </summary>
    	public FlxState()
    	{
    		defaultGroup = new FlxGroup();
    	}

    	#endregion

    	#region Methods for/from SuperClass/Interface

    	#endregion

    	#region Static Methods

    	#endregion

    	#region Public Methods

    	/// <summary>
    	/// Override this function to set up your game state.
    	/// This is where you create your groups and game objects and all that good stuff.
    	/// </summary>
    	virtual public void Create()
    	{
    		bgColor = FlxG.backColor;
    		//nothing to create initially
    	}

    	//@desc		Adds a new FlxCore subclass (FlxSprite, FlxBlock, etc) to the game loop
    	//@param	Core	The object you want to add to the game loop
    	virtual public FlxObject Add(FlxObject Core)
    	{
    		return defaultGroup.Add(Core);
    	}

    	/// <summary>
    	/// Override this function to do special pre-processing FX like motion blur.
    	/// You can use scaling or blending modes or whatever you want against
    	/// <code>FlxState.screen</code> to achieve all sorts of cool FX.
    	/// </summary>
    	virtual public void PreProcess(SpriteBatch spriteBatch)
    	{
    		spriteBatch.GraphicsDevice.Clear(bgColor); //Default behavior - just overwrite buffer with background color
    	}

    	/// <summary>
    	/// Automatically goes through and calls update on everything you added to the game loop,
    	/// override this function to handle custom input and perform collisions/
    	/// </summary>
    	virtual public void Update()
    	{
    		// Update all time-related stuff.
    		defaultGroup.Update();
    	}

    	/// <summary>
    	/// This function collides <code>defaultGroup</code> against <code>defaultGroup</code>
    	/// (basically everything you added to this state).
    	/// </summary>
    	virtual public void Collide()
    	{
    		FlxU.collide(defaultGroup,defaultGroup);
    	}

    	/// <summary>
    	/// Automatically goes through and calls render on everything you added to the game loop,
    	/// override this loop to manually control the rendering process.
    	/// </summary>
    	virtual public void Render(SpriteBatch spriteBatch)
    	{
    		// Render everything that should display on the screen.

    		//spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
    		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
    		//spriteBatch.GraphicsDevice.SamplerStates[0].Filter = TextureFilter.Point;
    		defaultGroup.Render(spriteBatch);
    		spriteBatch.End();

    	}

    	/// <summary>
    	/// Override this function to do special pre-processing FX like light bloom.
    	/// You can use scaling or blending modes or whatever you want against
    	/// <code>FlxState.screen</code> to achieve all sorts of cool FX.
    	/// </summary>
    	virtual public void PostProcess(SpriteBatch spriteBatch)
    	{
    		//no fx by default
    	}

    	/// <summary>
    	/// Override this function to handle any deleting or "shutdown" type operations you
    	/// might need (such as removing traditional Flash children like Sprite objects).
    	/// </summary>
    	virtual public void Destroy()
    	{
    		defaultGroup.Destroy();
    	}

    	#endregion

    	#region Private Methods

    	#endregion
    }
}
