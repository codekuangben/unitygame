using SDK.Lib;

namespace KBEngine
{
    /**
     * @brief 场景中的一个玩家分裂
     */
    public class Ball : KBEngine.GameObject   
    {
		public Ball()
		{
            
        }
		
		public override void __init__()
		{
            System.Int32 ownerId = (System.Int32)this.getDefinedProperty("ownereid");

            if (null == this.mEntity_SDK)
            {
                SDK.Lib.Player player = SDK.Lib.Ctx.mInstance.mPlayerMgr.getEntityByThisId((uint)ownerId) as SDK.Lib.Player;

                // 只有主角才有 Avatar ，其它玩家是没有 Avatar ，需要自己创建 Avatar
                if (this.isOwnerPlayer())
                {
                    if (null == player)
                    {
                        Ctx.mInstance.mLogSys.log("Ball::init can not find player main", LogTypeId.eLogSceneInterActive);
                    }
                    else
                    {
                        this.mEntity_SDK = new SDK.Lib.PlayerMainChild(player);
                        (this.mEntity_SDK.mMovement as PlayerMainChildMovement).addParentOrientChangedhandle();

                        Ctx.mInstance.mLogSys.log(string.Format("PlayerMainChild Created, eid = {0} ownerId = {1}", this.id, ownerId), LogTypeId.eLogSceneInterActive);
                    }
                }
                else
                {
                    if (null == player)
                    {
                        Ctx.mInstance.mLogSys.log("Ball::init can not find player other", LogTypeId.eLogSceneInterActive);

                        // 自己创建一个默认的
                        player = new PlayerOther();
                        player.setThisId((uint)ownerId);
                        player.init();
                    }

                    this.mEntity_SDK = new SDK.Lib.PlayerOtherChild(player);

                    Ctx.mInstance.mLogSys.log(string.Format("PlayerOtherChild Created, eid = {0} ownerId = {1}", this.id, ownerId), LogTypeId.eLogSceneInterActive);
                }

                this.mEntity_SDK.setEntity_KBE(this);
                this.mEntity_SDK.setThisId((uint)this.id);

                this.mEntity_SDK.init();

                UnityEngine.Vector3 fromPos = (UnityEngine.Vector3)this.getDefinedProperty("frompos");
                UnityEngine.Vector3 toPos = (UnityEngine.Vector3)this.getDefinedProperty("topos");

                if (UnityEngine.Vector3.zero != fromPos)
                {
                    (this.mEntity_SDK as PlayerChild).setPos(fromPos);
                    (this.mEntity_SDK as PlayerChild).setDestPosForBirth(toPos, false);
                }
                else
                {
                    this.mEntity_SDK.setPos(this.position);
                }

                (this.mEntity_SDK as SDK.Lib.BeingEntity).setRotateEulerAngle(this.direction);

                float radius = (float)getDefinedProperty("radius");
                (this.mEntity_SDK as BeingEntity).setBallRadius(radius);

                if (null != player && EntityType.ePlayerMain == player.getEntityType())
                {
                    (player as PlayerMain).onChildChanged();
                }
            }

            KBEngine.Event.registerIn("onRetEatSnowBlockResult", this, "onRetEatSnowBlockResult");
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
            }
        }

        public override void SetPosition(object old)
        {
            base.SetPosition(old);

            if (null != mEntity_SDK)
            {
                this.mEntity_SDK.setPos(this.position);
            }
        }

        public override void SetDirection(object old)
        {
            base.SetDirection(old);

            if (null != mEntity_SDK)
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

        // 拥有者是否是 MainPlayer
        public bool isOwnerPlayer()
        {
            bool ret = false;

            System.Int32 ownerId = (System.Int32)getDefinedProperty("ownereid");

            if (ownerId == KBEngineApp.app.entity_id)
            {
                ret = true;
            }

            return ret;
        }

        // 设置拥有者 Id
        //public void set_ownereid(System.Int32 ownerId)
        //{

        //}

        // 设置分裂开始位置
        //public void set_frompos(UnityEngine.Vector3 pos)
        //{
        //    // 只有分裂的时候，这个值才会有，如果出生的时候，这个值不会有
        //    if(pos != UnityEngine.Vector3.zero)
        //    {
        //        UnityEngine.Vector3 initPos = (UnityEngine.Vector3)this.getDefinedProperty("frompos");
        //        UnityEngine.Vector3 toPos = (UnityEngine.Vector3)this.getDefinedProperty("topos");

        //        (this.mEntity_SDK as PlayerChild).setPos(initPos);
        //        (this.mEntity_SDK as PlayerChild).setDestPosForBirth(toPos, false);
        //    }
        //}

        public void onRetEatSnowBlockResult(System.Int32 id, float eatSize)
        {
            KBEngine.Entity entity = KBEngine.KBEngineApp.app.findEntity(id);
            if (null != entity && null != entity.mEntity_SDK)
            {
                (entity.mEntity_SDK as BeingEntity).setBallRadius(eatSize);
            }
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