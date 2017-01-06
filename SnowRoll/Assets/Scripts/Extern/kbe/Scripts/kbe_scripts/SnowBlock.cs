namespace KBEngine
{
    public class SnowBlock : KBEngine.GameObject   
    {
    	public Combat combat = null;
    	
    	public static SkillBox skillbox = new SkillBox();
    	
		public SnowBlock()
		{
            mEntity_SDK = new SDK.Lib.SnowBlock();
            mEntity_SDK.setEntity_KBE(this);
        }
		
		public override void __init__()
		{
            this.mEntity_SDK.setThisId((uint)this.id);
            mEntity_SDK.init();
        }

		public override void onDestroy ()
		{
			if(isPlayer())
			{
				KBEngine.Event.deregisterIn(this);
			}

            if (null != mEntity_SDK)
            {
                mEntity_SDK.dispose();
                mEntity_SDK = null;
            }
        }

        public override void SetPosition(object old)
        {
            base.SetPosition(old);

            if (null != this.mEntity_SDK)
            {
                this.mEntity_SDK.setPos(this.position);
            }
        }

        public override void SetDirection(object old)
        {
            base.SetDirection(old);

            if (null != this.mEntity_SDK)
            {
                this.mEntity_SDK.setRotateEulerAngle(this.direction);
            }
        }

        public virtual void updatePlayer(float x, float y, float z, float yaw)
		{
	    	position.x = x;
	    	position.y = y;
	    	position.z = z;
			
	    	direction.z = yaw;
		}
		
		public override void onEnterWorld()
		{
			base.onEnterWorld();
		}

        public override void onLeaveWorld()
        {
            base.onLeaveWorld();

            if (null != this.mEntity_SDK)
            {
                this.mEntity_SDK.dispose();
            }
        }
    }
}