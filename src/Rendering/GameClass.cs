﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rampastring.Tools;
using Rampastring.XNAUI;
using System;
using System.IO;
using TSMapEditor.CCEngine;
using TSMapEditor.Models;
using TSMapEditor.UI;
using TSMapEditor.UI.CursorActions;

namespace TSMapEditor.Rendering
{
    public class GameClass : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private const string GameDirectory = "F:/Pelit/DTA Beta/";

        public GameClass()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            graphics.SynchronizeWithVerticalRetrace = false;
            Window.Title = "DTA Scenario Editor";

            //IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromMilliseconds(4);
        }

        private WindowManager windowManager;

        private readonly char DSC = Path.DirectorySeparatorChar;

        protected override void Initialize()
        {
            base.Initialize();

            AssetLoader.Initialize(GraphicsDevice, Content);
            AssetLoader.AssetSearchPaths.Add(Environment.CurrentDirectory + DSC + "Content" + DSC);

            windowManager = new WindowManager(this, graphics);
            windowManager.Initialize(Content, Environment.CurrentDirectory + DSC + "Content" + DSC);

            windowManager.InitGraphicsMode(2560, 1200, false);
            windowManager.SetRenderResolution(2560, 1200);
            windowManager.CenterOnScreen();
            windowManager.Cursor.LoadNativeCursor(Environment.CurrentDirectory + DSC + "Content" + DSC + "cursor.cur");

            Components.Add(windowManager);

            InitTest();
        }

        private void InitTest()
        {
            
            IniFile rulesIni = new IniFile(Path.Combine(GameDirectory, "INI/Rules.ini"));
            IniFile firestormIni = new IniFile(Path.Combine(GameDirectory, "INI/Enhance.ini"));
            IniFile artIni = new IniFile(Path.Combine(GameDirectory, "INI/Art.ini"));
            IniFile artFSIni = new IniFile(Path.Combine(GameDirectory, "INI/ArtE.INI"));
            IniFile mapIni = new IniFile(Path.Combine(GameDirectory, "Maps/Missions/rsovm6.map"));
            //IniFile mapIni = new IniFile(Path.Combine(GameDirectory, "Maps/Default/a_buoyant_city.map"));
            Map map = new Map();
            map.LoadExisting(rulesIni, firestormIni, artIni, artFSIni, mapIni);

            Console.WriteLine();
            Console.WriteLine("Map loaded.");

            var theater = map.EditorConfig.Theaters.Find(t => t.UIName.Equals(map.Theater, StringComparison.InvariantCultureIgnoreCase));
            if (theater == null)
            {
                throw new InvalidOperationException("Theater of map not found: " + map.Theater);
            }
            theater.ReadConfigINI(GameDirectory);

            CCFileManager ccFileManager = new CCFileManager();
            ccFileManager.GameDirectory = GameDirectory;
            ccFileManager.ReadConfig();
            ccFileManager.LoadPrimaryMixFile(theater.ContentMIXName);

            TheaterGraphics theaterGraphics = new TheaterGraphics(GraphicsDevice, theater, ccFileManager, map.Rules);

            var uiManager = new UIManager(windowManager, map, theaterGraphics);
            windowManager.AddAndInitializeControl(uiManager);
        }
    }
}
