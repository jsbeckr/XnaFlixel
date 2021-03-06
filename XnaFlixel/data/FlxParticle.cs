namespace XnaFlixel.data
{
    public class FlxParticle : FlxSprite
    {
		protected float _bounce;
		
		public FlxParticle(float Bounce)
		{
			_bounce = Bounce;
		}
		
		override public void HitSide(FlxObject Contact, float Velocity)
		{
			velocity.X = -velocity.X * _bounce;
			if(AngularVelocity != 0)
				AngularVelocity = -AngularVelocity * _bounce;
		}
		
		override public void HitBottom(FlxObject Contact, float Velocity)
		{
			OnFloor = true;
			if(((velocity.Y > 0)?velocity.Y:-velocity.Y) > _bounce*100)
			{
				velocity.Y = -velocity.Y * _bounce;
				if(AngularVelocity != 0)
					AngularVelocity *= -_bounce;
			}
			else
			{
				AngularVelocity = 0;
				base.HitBottom(Contact,Velocity);
			}
			velocity.X *= _bounce;
		}
    }
}
