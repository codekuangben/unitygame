namespace KBEngine
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using SDK.Lib;

    public class Avatar : KBEngine.GameObject   
    {
    	public Combat combat = null;
    	
    	public static SkillBox skillbox = new SkillBox();
    	
		public Avatar()
		{
		}
		
		public override void __init__()
		{
			combat = new Combat(this);
			
			if(isPlayer())
			{
				Event.registerIn("relive", this, "relive");
				Event.registerIn("useTargetSkill", this, "useTargetSkill");
				Event.registerIn("jump", this, "jump");
				Event.registerIn("updatePlayer", this, "updatePlayer");
			}

            KBEngine.Event.registerIn("onRetEatSnowBlockResult", this, "onRetEatSnowBlockResult");

            if (isPlayer())
            {
                mEntity_SDK = new SDK.Lib.PlayerMain();
            }
            else
            {
                mEntity_SDK = new SDK.Lib.PlayerOther();
            }

            mEntity_SDK.setEntity_KBE(this);
            mEntity_SDK.init();

            (this.mEntity_SDK as BeingEntity).setDestPos(this.position, true);
            (this.mEntity_SDK as BeingEntity).setDestRotate(new Vector3(this.direction.y, this.direction.z, this.direction.x), false);
        }

		public override void onDestroy ()
		{
			if(isPlayer())
			{
				KBEngine.Event.deregisterIn(this);
			}

            mEntity_SDK.dispose();
        }
		
		public void relive(Byte type)
		{
			cellCall("relive", type);
		}
		
		public bool useTargetSkill(Int32 skillID, Int32 targetID)
		{
			Skill skill = SkillBox.inst.get(skillID);
			if(skill == null)
				return false;

			SCEntityObject scobject = new SCEntityObject(targetID);
			if(skill.validCast(this, scobject))
			{
				skill.use(this, scobject);
				return true;
			}

			Dbg.DEBUG_MSG(className + "::useTargetSkill: skillID=" + skillID + ", targetID=" + targetID + ". is failed!");
			return false;
		}
		
		public void jump()
		{
			cellCall("jump");
		}
		
		public virtual void onJump()
		{
			Dbg.DEBUG_MSG(className + "::onJump: " + id);
			Event.fireOut("otherAvatarOnJump", new object[]{this});
		}
		
		public virtual void updatePlayer(float x, float y, float z, float yaw)
		{
	    	position.x = x;
	    	position.y = y;
	    	position.z = z;
			
	    	direction.z = yaw;
		}
		
		public virtual void onAddSkill(Int32 skillID)
		{
			Dbg.DEBUG_MSG(className + "::onAddSkill(" + skillID + ")"); 
			Event.fireOut("onAddSkill", new object[]{this});
			
			Skill skill = new Skill();
			skill.id = skillID;
			skill.name = skillID + " ";
			switch(skillID)
			{
				case 1:
					break;
				case 1000101:
					skill.canUseDistMax = 20f;
					break;
				case 2000101:
					skill.canUseDistMax = 20f;
					break;
				case 3000101:
					skill.canUseDistMax = 20f;
					break;
				case 4000101:
					skill.canUseDistMax = 20f;
					break;
				case 5000101:
					skill.canUseDistMax = 20f;
					break;
				case 6000101:
					skill.canUseDistMax = 20f;
					break;
				default:
					break;
			};

			SkillBox.inst.add(skill);
		}
		
		public virtual void onRemoveSkill(Int32 skillID)
		{
			Dbg.DEBUG_MSG(className + "::onRemoveSkill(" + skillID + ")"); 
			Event.fireOut("onRemoveSkill", new object[]{this});
			SkillBox.inst.remove(skillID);
		}
		
		public override void onEnterWorld()
		{
			base.onEnterWorld();

			if(isPlayer())
			{
				Event.fireOut("onAvatarEnterWorld", new object[]{KBEngineApp.app.entity_uuid, id, this});				
				SkillBox.inst.pull();
			}
		}

        public override void onLeaveWorld()
        {
            base.onLeaveWorld();

            if (isPlayer())
            {

            }
            else
            {
                this.mEntity_SDK.dispose();
            }
        }

        public void onRetEatSnowBlockResult(System.Int32 id, float eatSize)
        {
            KBEngine.Entity entity = KBEngine.KBEngineApp.app.findEntity(id);
            if (null != entity && null != entity.mEntity_SDK)
            {
                (entity.mEntity_SDK as BeingEntity).setEatSize(eatSize);
            }
        }

        //public override void set_position(object old)
        //{
        //    base.set_position(old);

        //    if (null != this.mEntity_SDK)
        //    {
        //        (this.mEntity_SDK as BeingEntity).setDestPos(this.position);
        //        (this.mEntity_SDK as BeingEntity).setOriginal(this.position);
        //    }
        //}

        //public override void set_direction(object old)
        //{
        //    base.set_direction(old);
        //    if (null != this.mEntity_SDK)
        //    {
        //        (this.mEntity_SDK as BeingEntity).setDestRotate(new Vector3(this.direction.y, this.direction.z, this.direction.x));
        //    }
        //}
    }
}