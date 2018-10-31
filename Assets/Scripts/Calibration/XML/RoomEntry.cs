﻿using System;
using System.Xml.Serialization;
using VRCalibrationTool;

namespace CRI.HelloHouston.Calibration.XML
{
    /// <summary>
    /// A room.
    /// </summary>
    [Serializable]
    public class RoomEntry : ItemEntry
    {
        /// <summary>
        /// Name of the room.
        /// </summary>
        public override string name
        {
            get
            {
                return "Room " + index;
            }
        }
        /// <summary>
        /// List of items that made up the room.  
        /// </summary>
        [XmlArrayItem(typeof(BlockEntry), ElementName = "block")]
        [XmlArray("blocks")]
        public BlockEntry[] blocks;
        /// <summary>
        /// Login of the person who calibrated the ItemEntry.
        /// </summary>
        [XmlAttribute("login")]
        public string login;

        public RoomEntry() : base() { }

        public RoomEntry(int index, BlockEntry[] blocks, PositionTag[] points, DateTime date) : base(index, points, date)
        {
            this.blocks = blocks;
        }
    }
}
