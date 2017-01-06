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

            if (isPlayer())
            {
                Event.registerIn("relive", this, "relive");
                Event.registerIn("useTargetSkill", this, "useTargetSkill");
                Event.registerIn("jump", this, "jump");
                Event.registerIn("updatePlayer", this, "updatePlayer");
            }

            if (isPlayer())
            {
                mEntity_SDK = new SDK.Lib.PlayerMain();

                Ctx.mInstance.mLogSys.log(string.Format("MainPlayer Created, eid = {0}", this.id), LogTypeId.eLogSceneInterActive);
            }
            else
            {
                mEntity_SDK = new SDK.Lib.PlayerOther();
                Ctx.mInstance.mLogSys.log(string.Format("PlayerOther Created, eid = {0}", this.id), LogTypeId.eLogSceneInterActive);
            }

            mEntity_SDK.setEntity_KBE(this);
            (this.mEntity_SDK as BeingEntity).setThisId((uint)this.id);

            mEntity_SDK.init();
            (this.mEntity_SDK as BeingEntity).setDestPos(this.position, true);
            (this.mEntity_SDK as BeingEntity).setDestRotate(new Vector3(this.direction.y, this.direction.z, this.direction.x), false);
        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }

            if (null != this.mEntity_SDK)
            {
                this.mEntity_SDK.dispose();
            }
        }

        public void relive(Byte type)
        {
            cellCall("relive", type);
        }

        public bool useTargetSkill(Int32 skillID, Int32 targetID)
        {
            Skill skill = SkillBox.inst.get(skillID);
            if (skill == null)
                return false;

            SCEntityObject scobject = new SCEntityObject(targetID);
            if (skill.validCast(this, scobject))
            {
                skill.use(this, scobject);
                return true;
            }

            return false;
        }

        public void jump()
        {
            cellCall("jump");
        }

        public virtual void onJump()
        {
            Dbg.DEBUG_MSG(className + "::onJump: " + id);
            Event.fireOut("otherAvatarOnJump", new object[] { this });
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
            Event.fireOut("onAddSkill", new object[] { this });

            Skill skill = new Skill();
            skill.id = skillID;
            skill.name = skillID + " ";
            switch (skillID)
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
            Event.fireOut("onRemoveSkill", new object[] { this });
            SkillBox.inst.remove(skillID);
        }

        public override void onEnterWorld()
        {
            base.onEnterWorld();

            if (isPlayer())
            {
                Event.fireOut("onAvatarEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
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
                if (null != this.mEntity_SDK)
                {
                    this.mEntity_SDK.dispose();
                }
            }
        }

        public void set_radius(float radius)
        {
            if (null != this.mEntity_SDK)
            {
                (this.mEntity_SDK as BeingEntity).setBallRadius(radius);
            }
        }

        public override void set_name(object old)
        {
            string name = getDefinedProperty("name") as string;
            (this.mEntity_SDK as BeingEntity).setName(name);
        }

        //Top10排名
        public void updateRankList(byte myRank, Dictionary<string, object> infos)
        {
            List<object> listinfos = (List<object>)infos["values"];
            int count = listinfos.Count;
            RankInfo[] result = new RankInfo[count];
            for (int i = 0; i < count; i++)
            {
                Dictionary<string, object> info = (Dictionary<string, object>)listinfos[i];
                result[i].rank = i + 1;
                result[i].eid = Convert.ToUInt32(info["eid"]);
                result[i].name = (string)info["name"];
            }

            SDK.Lib.Ctx.mInstance.mLuaSystem.receiveToLua_KBE("notifyTop10RankInfoList", new object[] { result, myRank, count });
        }

        //结算排名
        public void notifyResultData(byte myRank, Dictionary<string, object> infos)
        {
            List<object> listinfos = (List<object>)infos["values"];
            int count = listinfos.Count;
            RankInfo[] result = new RankInfo[count];
            for (int i = 0; i < count; i++)
            {
                Dictionary<string, object> info = (Dictionary<string, object>)listinfos[i];
                result[i].rank = i + 1;
                result[i].eid = Convert.ToUInt32(info["eid"]);
                result[i].name = (string)info["name"];
                result[i].radius = (float)Convert.ToDouble(info["finalRadius"]);
                result[i].swallownum = Convert.ToUInt32(info["eatCount"]);
            }
            Ctx.mInstance.mUiMgr.exitForm(UIFormId.eUIJoyStick);
            Ctx.mInstance.mUiMgr.exitForm(UIFormId.eUIForwardForce);
            SDK.Lib.Ctx.mInstance.mLuaSystem.receiveToLua_KBE("notifyResultRankInfoList", new object[] { result, myRank, count });
        }

        //public override void SetPosition(object old)
        //{
        //    base.SetPosition(old);

        //    if (null != this.mEntity_SDK)
        //    {
        //        (this.mEntity_SDK as BeingEntity).setDestPos(this.position);
        //        (this.mEntity_SDK as BeingEntity).setOriginal(this.position);
        //    }
        //}

        //public override void SetDirection(object old)
        //{
        //    base.SetDirection(old);
        //    if (null != this.mEntity_SDK)
        //    {
        //        (this.mEntity_SDK as BeingEntity).setDestRotate(new Vector3(this.direction.y, this.direction.z, this.direction.x));
        //    }
        //}
    }
}