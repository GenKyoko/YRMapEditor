﻿using TSMapEditor.GameMath;
using Microsoft.Xna.Framework;

namespace TSMapEditor.Models
{
    public class Animation : GameObject
    {
        public Animation(AnimType animType)
        {
            AnimType = animType;
        }

        public Animation(AnimType animType, Point2D position) : this(animType)
        {
            Position = position;
        }

        public override RTTIType WhatAmI() => RTTIType.Anim;

        public AnimType AnimType { get; private set; }
        public House Owner { get; set; }
        public byte Facing { get; set; }

        public Point2D ExtraDrawOffset { get; set; } = new();
        public bool IsBuildingAnim { get; set; }
        public Structure ParentBuilding { get; set; }
        public bool IsTurretAnim { get; set; }

        public override int GetYDrawOffset()
        {
            return Constants.CellSizeY / -2 + AnimType.ArtConfig.YDrawOffset + ExtraDrawOffset.Y;
        }

        public override int GetXDrawOffset()
        {
            return AnimType.ArtConfig.XDrawOffset + ExtraDrawOffset.X;
        }

        public override int GetFrameIndex(int frameCount)
        {
            if (IsBuildingAnim && ParentBuilding != null)
            {
                if (frameCount > 1 && ParentBuilding.HP < Constants.ConditionYellowHP)
                    return frameCount / 4;
            }

            return 0;
        }

        public override int GetShadowFrameIndex(int frameCount)
        {
            if (IsBuildingAnim && ParentBuilding != null)
            {
                if (ParentBuilding.HP < Constants.ConditionYellowHP)
                    return frameCount / 4 * 3;
            }

            return frameCount / 2;
        }

        public override bool Remapable() => IsBuildingAnim;
        public override Color GetRemapColor() => Remapable() ? Owner.XNAColor : Color.White;
    }
}
