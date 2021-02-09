﻿using Rampastring.Tools;
using System.Collections.Generic;

namespace TSMapEditor.CCEngine
{
    public class Theater
    {
        public Theater(string uiName, string configIniName, string contentMixName,
            string paletteName, string unitPaletteName, string fileExtension,
            char newTheaterBuildingLetter)
        {
            UIName = uiName;
            ConfigININame = configIniName;
            ContentMIXName = contentMixName;
            PaletteName = paletteName;
            UnitPaletteName = unitPaletteName;
            FileExtension = fileExtension;
            NewTheaterBuildingLetter = newTheaterBuildingLetter;
        }

        public string UIName { get; }
        public string ConfigININame { get; }
        public string ContentMIXName { get; }
        public string PaletteName { get; }
        public string UnitPaletteName { get; }
        public string FileExtension { get; }
        public char NewTheaterBuildingLetter { get; }

        public void ReadConfigINI(string baseDirectoryPath)
        {
            IniFile theaterIni = new IniFile(baseDirectoryPath + ConfigININame);
            for (int i = 0; i < 10000; i++)
            {
                IniSection tileSetSection = theaterIni.GetSection(string.Format("TileSet{0:D4}", i));

                if (tileSetSection == null)
                    return;

                TileSet tileSet = new TileSet();
                tileSet.Read(tileSetSection);
                TileSets.Add(tileSet);
            }
        }

        public List<TileSet> TileSets = new List<TileSet>();
    }
}
