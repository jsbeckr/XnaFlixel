using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaFlixel
{
    //@desc		This class wraps the baseline .NET collection and adds a couple of extra functions...
    public class FlxGroup : FlxObject
    {
    	#region Constants

    	#endregion

    	#region Fields

    	/// <summary>
    	/// Array of all the <code>FlxObject</code>s that exist in this layer.
    	/// </summary>
    	public List<FlxObject> members;
    	/// <summary>
    	/// Helpers for moving/updating group members.
    	/// </summary>
    	protected Vector2 _last;
    	protected bool _first;

    	#endregion

    	#region Properties

    	#endregion

    	#region Constructors

    	/// <summary>
    	/// Constructor
    	/// </summary>
    	public FlxGroup()
    		: base()
    	{
    		_group = true;
    		Solid = false;
    		members = new List<FlxObject>();
    		_last = Vector2.Zero;
    		_first = true;
    	}

    	#endregion

    	#region Methods for/from SuperClass/Interface

    	/// <summary>
    	/// Automatically goes through and calls update on everything you added,
    	/// override this function to handle custom input and perform collisions.
    	/// </summary>
    	override public void Update()
    	{
    		SaveOldPosition();
    		UpdateMotion();
    		UpdateMembers();
    		UpdateFlickering();
    	}

    	/// <summary>
    	/// Automatically goes through and calls render on everything you added,
    	/// override this loop to control render order manually.
    	/// </summary>
    	override public void Render(SpriteBatch spriteBatch)
    	{
    		RenderMembers(spriteBatch);
    	}

    	/// <summary>
    	/// Calls kill on the group and all its members.
    	/// </summary>
    	override public void Kill()
    	{
    		KillMembers();
    		base.Kill();
    	}

    	/// <summary>
    	/// Override this function to handle any deleting or "shutdown" type operations you might need,
    	/// such as removing traditional Flash children like Sprite objects.
    	/// </summary>
    	override public void Destroy()
    	{
    		DestroyMembers();
    		base.Destroy();
    	}

    	/// <summary>
    	/// If the group's position is reset, we want to reset all its members too.
    	/// 
    	/// @param	X	The new X position of this object.
    	/// @param	Y	The new Y position of this object.
    	/// </summary>
    	override public void Reset(float X, float Y)
    	{
    		SaveOldPosition();
    		base.Reset(X,Y);
    		float mx = 0;
    		float my = 0;
    		bool moved = false;
    		if((base.X != _last.X) || (base.Y != _last.Y))
    		{
    			moved = true;
    			mx = base.X - _last.X;
    			my = base.Y - _last.Y;
    		}
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && o.Exists)
    			{
    				if(moved)
    				{
    					if (o._group)
    						o.Reset((o.X + mx), (o.Y + my));
    					else
    					{
    						o.X += mx;
    						o.Y += my;
    						if (Solid)
    						{
    							o.colHullX.Width += ((mx > 0) ? mx : -mx);
    							if (mx < 0)
    								o.colHullX.x += mx;
    							o.colHullY.x = base.X;
    							o.colHullY.Height += ((my > 0) ? my : -my);
    							if (my < 0)
    								o.colHullY.y += my;
    							o.colVector.X += mx;
    							o.colVector.Y += my;
    						}
    					}
    				}
    			}
    		}
    	}

    	#endregion

    	#region Static Methods

    	#endregion

    	#region Public Methods

    	/// <summary>
    	/// Adds a new <code>FlxObject</code> subclass (FlxSprite, FlxBlock, etc) to the list of children
    	///
    	/// @param	Object			The object you want to add
    	/// @param	ShareScroll		Whether or not this FlxObject should sync up with this layer's scrollFactor
    	///
    	/// @return	The same <code>FlxObject</code> object that was passed in.
    	/// </summary>
    	public FlxObject Add(FlxObject Object)
    	{
    		return Add(Object, false);
    	}

    	public FlxObject Add(FlxObject Object, bool ShareScroll)
    	{
    		if (members.IndexOf(Object) < 0)
    			members.Add(Object);
    		//members[members.Count] = Object;
    		if(ShareScroll)
    			Object.scrollFactor = scrollFactor;
    		return Object;
    	}

    	/// <summary>
    	/// Replaces an existing <code>FlxObject</code> with a new one.
    	/// 
    	/// @param	OldObject	The object you want to replace.
    	/// @param	NewObject	The new object you want to use instead.
    	/// 
    	/// @return	The new object.
    	/// </summary>
    	public FlxObject Replace(FlxObject OldObject, FlxObject NewObject)
    	{
    		int index = members.IndexOf(OldObject);
    		if((index < 0) || (index >= members.Count))
    			return null;
    		members[index] = NewObject;
    		return NewObject;
    	}

    	/// <summary>
    	/// Removes an object from the group.
    	/// 
    	/// @param	Object	The <code>FlxObject</code> you want to remove.
    	/// @param	Splice	Whether the object should be cut from the array entirely or not.
    	/// 
    	/// @return	The removed object.
    	/// </summary>
    	public FlxObject Remove(FlxObject Object)
    	{
    		return Remove(Object, false);
    	}

    	public FlxObject Remove(FlxObject Object, bool Splice)
    	{
    		int index = members.IndexOf(Object);
    		if((index < 0) || (index >= members.Count))
    			return null;
    		if(Splice)
    			members.RemoveAt(index);
    		else
    			members[index] = null;
    		return Object;
    	}

    	/// <summary>
    	/// Call this function to sort the group according to a particular value and order.
    	/// Due to differences in language capabilities between AS3/C#, you must implement
    	/// your own IComparer interface for each sorting operation you want to perform.
    	/// 
    	/// For example, to sort game objects for Zelda-style overlaps you might call
    	/// sort by an objects "y" member at the bottom of your <code>FlxState.update()</code>
    	/// override.  To sort all existing objects after a big explosion or bomb attack,
    	/// you might sort by "exists."
    	/// 
    	/// @param	Sorter	The <code>IComparer</code> object which will receive the sorting
    	///          comparisons.
    	/// </summary>
    	public void Sort(IComparer<FlxObject> Sorter)
    	{
    		members.Sort(Sorter);
    	}

    	/// <summary>
    	/// Call this function to retrieve the first object with exists == false in the group.
    	/// This is handy for recycling in general, e.g. respawning enemies.
    	/// 
    	/// @return	A <code>FlxObject</code> currently flagged as not existing.
    	/// </summary>
    	public FlxObject GetFirstAvail()
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && !o.Exists)
    				return o;
    		}
    		return null;
    	}

    	/// <summary>
    	/// Call this function to retrieve the first index set to 'null'.
    	/// Returns -1 if no index stores a null object.
    	/// 
    	/// @return	An <code>int</code> indicating the first null slot in the group.
    	/// </summary>
    	public int GetFirstNull()
    	{
    		int i = 0;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			if(members[i] == null)
    				return i;
    			else
    				i++;
    		}
    		return -1;
    	}

    	/// <summary>
    	/// Finds the first object with exists == false and calls reset on it.
    	/// 
    	/// @param	X	The new X position of this object.
    	/// @param	Y	The new Y position of this object.
    	/// 
    	/// @return	Whether a suitable <code>FlxObject</code> was found and reset.
    	/// </summary>
    	public bool ResetFirstAvail(int X, int Y)
    	{
    		FlxObject o = GetFirstAvail();
    		if(o == null)
    			return false;
    		o.Reset(X,Y);
    		return true;
    	}

    	/// <summary>
    	/// Call this function to retrieve the first object with exists == true in the group.
    	/// This is handy for checking if everything's wiped out, or choosing a squad leader, etc.
    	/// 
    	/// @return	A <code>FlxObject</code> currently flagged as existing.
    	/// </summary>
    	public FlxObject GetFirstExtant()
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && o.Exists)
    				return o;
    		}
    		return null;
    	}

    	/// <summary>
    	/// Call this function to retrieve the first object with dead == false in the group.
    	/// This is handy for checking if everything's wiped out, or choosing a squad leader, etc.
    	/// 
    	/// @return	A <code>FlxObject</code> currently flagged as not dead.
    	/// </summary>
    	public FlxObject GetFirstAlive()
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && o.Exists && !o.Dead)
    				return o;
    		}
    		return null;
    	}

    	/// <summary>
    	/// Call this function to retrieve the first object with dead == true in the group.
    	/// This is handy for checking if everything's wiped out, or choosing a squad leader, etc.
    	/// 
    	/// @return	A <code>FlxObject</code> currently flagged as dead.
    	/// </summary>
    	public FlxObject GetFirstDead()
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && o.Dead)
    				return o;
    		}
    		return null;
    	}

    	/// <summary>
    	/// Call this function to find out how many members of the group are not dead.
    	/// 
    	/// @return	The number of <code>FlxObject</code>s flagged as not dead.  Returns -1 if group is empty.
    	/// </summary>
    	public int CountLiving()
    	{
    		int count = -1;
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if(o != null)
    			{
    				if(count < 0)
    					count = 0;
    				if(o.Exists && !o.Dead)
    					count++;
    			}
    		}
    		return count;
    	}

    	/// <summary>
    	/// Call this function to find out how many members of the group are dead.
    	/// 
    	/// @return	The number of <code>FlxObject</code>s flagged as dead.  Returns -1 if group is empty.
    	/// </summary>
    	public int CountDead()
    	{
    		int count = -1;
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if(o != null)
    			{
    				if(count < 0)
    					count = 0;
    				if(o.Dead)
    					count++;
    			}
    		}
    		return count;
    	}

    	/// <summary>
    	/// Returns a count of how many objects in this group are on-screen right now.
    	/// 
    	/// @return	The number of <code>FlxObject</code>s that are on screen.  Returns -1 if group is empty.
    	/// </summary>
    	public int CountOnScreen()
    	{
    		int count = -1;
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if(o != null)
    			{
    				if(count < 0)
    					count = 0;
    				if(o.OnScreen())
    					count++;
    			}
    		}
    		return count;
    	}		

    	/// <summary>
    	/// Returns a member at random from the group.
    	/// 
    	/// @return	A <code>FlxObject</code> from the members list.
    	/// </summary>
    	public FlxObject GetRandom()
    	{
    		int c = 0;
    		FlxObject o = null;
    		int l = members.Count;
    		int i = (int)(FlxU.random()*l);
    		while((o == null) && (c < members.Count))
    		{
    			o = members[(++i)%l] as FlxObject;
    			c++;
    		}
    		return o;
    	}

    	/// <summary>
    	/// Internal function, helps with the moving/updating of group members.
    	/// </summary>
    	protected void SaveOldPosition()
    	{
    		if(_first)
    		{
    			_first = false;
    			_last.X = 0;
    			_last.Y = 0;
    			return;
    		}
    		_last.X = X;
    		_last.Y = Y;
    	}

    	/// <summary>
    	/// Internal function that actually goes through and updates all the group members.
    	/// Depends on <code>saveOldPosition()</code> to set up the correct values in <code>_last</code> in order to work properly.
    	/// </summary>
    	virtual protected void UpdateMembers()
    	{
    		float mx = 0;
    		float my = 0;
    		bool moved = false;
    		if((X != _last.X) || (Y != _last.Y))
    		{
    			moved = true;
    			mx = X - _last.X;
    			my = Y - _last.Y;
    		}
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && o.Exists)
    			{
    				if(moved)
    				{
    					if (o._group)
    						o.Reset((o.X + mx), (o.Y + my));
    					else
    					{
    						o.X += mx;
    						o.Y += my;
    					}
    				}
    				if(o.Active)
    					o.Update();
    				if(moved && o.Solid)
    				{
    					o.colHullX.Width += ((mx>0)?mx:-mx);
    					if(mx < 0)
    						o.colHullX.x += mx;
    					o.colHullY.x = X;
    					o.colHullY.Height += ((my>0)?my:-my);
    					if(my < 0)
    						o.colHullY.y += my;
    					o.colVector.X += mx;
    					o.colVector.Y += my;
    				}
    			}
    		}
    	}

    	/// <summary>
    	/// Internal function that actually loops through and renders all the group members.
    	/// </summary>
    	protected void RenderMembers(SpriteBatch spriteBatch)
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if((o != null) && o.Exists && o.Visible)
    				o.Render(spriteBatch);
    		}
    	}

    	/// <summary>
    	/// Internal function that actually loops through and destroys each member.
    	/// </summary>
    	protected void DestroyMembers()
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if(o != null)
    				o.Destroy();
    		}
    		members.Clear();
    	}

    	/// <summary>
    	/// Internal function that calls kill on all members.
    	/// </summary>
    	protected void KillMembers()
    	{
    		int i = 0;
    		FlxObject o;
    		int ml = members.Count;
    		while(i < ml)
    		{
    			o = members[i++] as FlxObject;
    			if(o != null)
    				o.Kill();
    		}
    	}

    	#endregion

    	#region Private Methods

    	#endregion
    }
}
