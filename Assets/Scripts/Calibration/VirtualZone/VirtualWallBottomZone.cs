﻿ using CRI.HelloHouston.Experience;

namespace CRI.HelloHouston.Calibration
{
    public class VirtualWallBottomZone : VirtualZone
    {
        public override ZoneType zoneType
        {
            get
            {
                return ZoneType.WallBottom;
            }
        }
        public override VirtualElement[] virtualElements
        {
            get
            {
                return new VirtualElement[] { wallBottomVirtualElement };
            }
        }

        public override XPZone xpZone
        {
            get
            {
                return xpWallBottomZone;
            }
        }

        protected override void AddXPZone(XPZone xpZone)
        {
            var xpWallBottomZone = xpZone as XPWallBottomZone;
            if (!xpWallBottomZone)
                throw new WrongZoneTypeException();
            this.xpWallBottomZone = xpWallBottomZone;
        }

        public VirtualElement wallBottomVirtualElement;
        public XPWallBottomZone xpWallBottomZone { get; protected set; }
    }
}
