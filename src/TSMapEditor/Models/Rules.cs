﻿using Microsoft.Xna.Framework;
using Rampastring.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using TSMapEditor.Initialization;
using TSMapEditor.Models.ArtConfig;
using TSMapEditor.UI;

namespace TSMapEditor.Models
{
    public class Rules
    {
        public List<UnitType> UnitTypes = new List<UnitType>();
        public List<InfantryType> InfantryTypes = new List<InfantryType>();
        public List<BuildingType> BuildingTypes = new List<BuildingType>();
        public List<AircraftType> AircraftTypes = new List<AircraftType>();
        public List<TerrainType> TerrainTypes = new List<TerrainType>();
        public List<OverlayType> OverlayTypes = new List<OverlayType>();
        public List<SmudgeType> SmudgeTypes = new List<SmudgeType>();

        public List<string> Sides = new List<string>();
        public List<InfantrySequence> InfantrySequences = new List<InfantrySequence>();
        public List<RulesColor> Colors = new List<RulesColor>();
        public List<TiberiumType> TiberiumTypes = new List<TiberiumType>();
        public List<AnimType> AnimTypes = new List<AnimType>();
        public List<GlobalVariable> GlobalVariables = new List<GlobalVariable>();
        public List<Weapon> Weapons = new List<Weapon>();

        public List<HouseType> RulesHouseTypes = new List<HouseType>();

        public TutorialLines TutorialLines { get; set; }
        public Themes Themes { get; set; }

        /// <summary>
        /// Initializes rules types from an INI file.
        /// </summary>
        public void InitFromINI(IniFile iniFile, IInitializer initializer, bool isMapIni = false)
        {
            InitFromTypeSection(iniFile, "VehicleTypes", UnitTypes);
            InitFromTypeSection(iniFile, "InfantryTypes", InfantryTypes);
            InitFromTypeSection(iniFile, "BuildingTypes", BuildingTypes);
            InitFromTypeSection(iniFile, "AircraftTypes", AircraftTypes);
            InitFromTypeSection(iniFile, "TerrainTypes", TerrainTypes);
            InitFromTypeSection(iniFile, "OverlayTypes", OverlayTypes);
            InitFromTypeSection(iniFile, "SmudgeTypes", SmudgeTypes);
            InitFromTypeSection(iniFile, "Animations", AnimTypes);
            InitFromTypeSection(iniFile, "Weapons", Weapons);

            if (!isMapIni)
            {
                if (Constants.UseCountries)
                    InitFromTypeSection(iniFile, "Countries", RulesHouseTypes);
                else
                    InitFromTypeSection(iniFile, "Houses", RulesHouseTypes);
            }

            // Go through all the lists and get object properties
            UnitTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            InfantryTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            BuildingTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            AircraftTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            TerrainTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            OverlayTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            SmudgeTypes.ForEach(ot => initializer.ReadObjectTypePropertiesFromINI(ot, iniFile));
            Weapons.ForEach(w => initializer.ReadObjectTypePropertiesFromINI(w, iniFile));
            AnimTypes.ForEach(a => initializer.ReadObjectTypePropertiesFromINI(a, iniFile));
            RulesHouseTypes.ForEach(ht => initializer.ReadObjectTypePropertiesFromINI(ht, iniFile));

            if (!isMapIni)
                InitColors(iniFile);

            if (!isMapIni)
                InitTiberiums(iniFile);

            InitSides(iniFile);

            if (!isMapIni)
            {
                // Don't load local variables defined in the map as globals

                IniSection variableNamesSection = iniFile.GetSection("VariableNames");
                if (variableNamesSection != null)
                {
                    for (int i = 0; i < variableNamesSection.Keys.Count; i++)
                    {
                        var kvp = variableNamesSection.Keys[i];
                        GlobalVariables.Add(new GlobalVariable(int.Parse(kvp.Key, CultureInfo.InvariantCulture), kvp.Value));
                    }
                }
            }
        }

        private void InitColors(IniFile iniFile)
        {
            var colorsSection = iniFile.GetSection("Colors");
            if (colorsSection != null)
            {
                foreach (var kvp in colorsSection.Keys)
                {
                    Colors.Add(new RulesColor(kvp.Key, kvp.Value));
                }
            }

            // Assign Rules housetype colors
            RulesHouseTypes.ForEach(ht =>
            {
                var color = Colors.Find(c => c.Name == ht.Color);

                if (color != null)
                    ht.XNAColor = color.XNAColor;
            });
        }

        private void InitTiberiums(IniFile iniFile)
        {
            var tiberiumsSection = iniFile.GetSection("Tiberiums");
            if (tiberiumsSection != null)
            {
                for (int i = 0; i < tiberiumsSection.Keys.Count; i++)
                {
                    var kvp = tiberiumsSection.Keys[i];
                    var tiberiumType = new TiberiumType(kvp.Value, i);

                    var tiberiumTypeSection = iniFile.GetSection(kvp.Value);
                    if (tiberiumTypeSection != null)
                    {
                        tiberiumType.ReadPropertiesFromIniSection(tiberiumTypeSection);

                        TiberiumTypes.Add(tiberiumType);
                        var rulesColor = Colors.Find(c => c.Name == tiberiumType.Color);
                        if (rulesColor != null)
                            tiberiumType.XNAColor = rulesColor.XNAColor;
                    }
                }
            }
        }

        private void InitSides(IniFile iniFile)
        {
            var sidesSection = iniFile.GetSection("Sides");
            if (sidesSection != null)
            {
                foreach (var kvp in sidesSection.Keys)
                {
                    Sides.Add(kvp.Key);
                }
            }
        }

        public void InitArt(IniFile iniFile, IInitializer initializer)
        {
            TerrainTypes.ForEach(tt => initializer.ReadObjectTypeArtPropertiesFromINI(tt, iniFile));

            SmudgeTypes.ForEach(st => initializer.ReadObjectTypeArtPropertiesFromINI(st, iniFile));

            BuildingTypes.ForEach(bt => initializer.ReadObjectTypeArtPropertiesFromINI(bt, iniFile,
                string.IsNullOrWhiteSpace(bt.Image) ? bt.ININame : bt.Image));

            UnitTypes.ForEach(ut => initializer.ReadObjectTypeArtPropertiesFromINI(ut, iniFile,
                string.IsNullOrWhiteSpace(ut.Image) ? ut.ININame : ut.Image));

            AircraftTypes.ForEach(ut => initializer.ReadObjectTypeArtPropertiesFromINI(ut, iniFile,
                string.IsNullOrWhiteSpace(ut.Image) ? ut.ININame : ut.Image));

            InfantryTypes.ForEach(it => initializer.ReadObjectTypeArtPropertiesFromINI(it, iniFile,
                string.IsNullOrWhiteSpace(it.Image) ? it.ININame : it.Image));

            OverlayTypes.ForEach(ot => initializer.ReadObjectTypeArtPropertiesFromINI(ot, iniFile,
                string.IsNullOrWhiteSpace(ot.Image) ? ot.ININame : ot.Image));

            AnimTypes.ForEach(a => initializer.ReadObjectTypeArtPropertiesFromINI(a, iniFile, a.ININame));
        }

        public void InitEditorOverrides(IniFile iniFile)
        {
            List<GameObjectType> gameObjectTypes = new List<GameObjectType>();
            gameObjectTypes.AddRange(UnitTypes);
            gameObjectTypes.AddRange(InfantryTypes);
            gameObjectTypes.AddRange(BuildingTypes);
            gameObjectTypes.AddRange(AircraftTypes);
            gameObjectTypes.AddRange(TerrainTypes);
            gameObjectTypes.AddRange(OverlayTypes);
            gameObjectTypes.AddRange(SmudgeTypes);

            var section = iniFile.GetSection("ObjectCategoryOverrides");
            if (section != null)
            {
                foreach (var keyValuePair in section.Keys)
                {
                    var obj = gameObjectTypes.Find(o => o.ININame == keyValuePair.Key);
                    if (obj != null)
                        obj.EditorCategory = keyValuePair.Value;
                }
            }

            section = iniFile.GetSection("IgnoreTypes");
            if (section != null)
            {
                foreach (var keyValuePair in section.Keys)
                {
                    var obj = gameObjectTypes.Find(o => o.ININame == keyValuePair.Key);
                    if (obj != null)
                        obj.EditorVisible = !section.GetBooleanValue(keyValuePair.Key, !obj.EditorVisible);
                }
            }
        }

        public List<HouseType> GetStandardHouseTypes(IniFile iniFile)
        {
            var houseTypes = GetHouseTypesFrom(iniFile, "Houses");
            if (houseTypes.Count > 0)
                return houseTypes;

            return GetHouseTypesFrom(iniFile, "Countries");
        }

        public List<HouseType> GetHouseTypesFrom(IniFile iniFile, string sectionName)
        {
            var houseTypesSection = iniFile.GetSection(sectionName);
            if (houseTypesSection == null)
                return new List<HouseType>(0);

            var houseTypes = new List<HouseType>();

            foreach (var kvp in houseTypesSection.Keys)
            {
                string houseTypeName = kvp.Value;
                var houseType = new HouseType(houseTypeName);
                houseType.Index = Conversions.IntFromString(kvp.Key, -1);

                if (houseType.Index < 0 || houseTypes.Exists(ht => ht.Index == houseType.Index))
                    throw new INIConfigException($"Invalid index for HouseType in standard houses. Section: {sectionName}, HouseType name: {houseTypeName}");

                InitHouseType(iniFile, houseType);
                houseTypes.Add(houseType);
            }

            return houseTypes;
        }

        private void InitHouseType(IniFile iniFile, HouseType houseType)
        {
            // Fetch some default properties from Rules so they don't need to be duplicated in EditorRules.
            // Aside from the color most of these aren't used, but maybe they might be useful one day.
            var rulesHouseType = RulesHouseTypes.Find(ht => ht.ININame == houseType.ININame);
            if (rulesHouseType != null)
            {
                houseType.CopyBasicPropertiesFrom(rulesHouseType);
            }

            var section = iniFile.GetSection(houseType.ININame);
            if (section == null)
                return;

            houseType.ReadPropertiesFromIniSection(iniFile.GetSection(houseType.ININame));
            //house.Name = iniFile.GetStringValue(house.ININame, "Name", house.ININame);
            //house.Color = iniFile.GetStringValue(house.ININame, "Color", "Grey");
            var color = Colors.Find(c => c.Name == houseType.Color);
            if (color == null)
                houseType.XNAColor = Color.Black;
            else
                houseType.XNAColor = color.XNAColor;
        }

        public List<House> GetHousesFrom(IniFile iniFile, string sectionName)
        {
            var housesSection = iniFile.GetSection(sectionName);
            if (housesSection == null)
                return new List<House>(0);

            var houses = new List<House>();

            foreach (var kvp in housesSection.Keys)
            {
                string houseName = kvp.Value;
                var house = new House(houseName);
                InitHouse(iniFile, house);
                houses.Add(house);
            }

            return houses;
        }

        private void InitHouse(IniFile iniFile, House house)
        {
            var section = iniFile.GetSection(house.ININame);
            if (section == null)
                return;

            house.ReadPropertiesFromIniSection(iniFile.GetSection(house.ININame));
            //house.Name = iniFile.GetStringValue(house.ININame, "Name", house.ININame);
            //house.Color = iniFile.GetStringValue(house.ININame, "Color", "Grey");
            var color = Colors.Find(c => c.Name == house.Color);
            if (color == null)
                house.XNAColor = Color.Black;
            else
                house.XNAColor = color.XNAColor;
        }

        private void InitFromTypeSection<T>(IniFile iniFile, string sectionName, List<T> targetList)
        {
            var sectionKeys = iniFile.GetSectionKeys(sectionName);

            if (sectionKeys == null || sectionKeys.Count == 0)
                return;

            int i = targetList.Count;

            foreach (string key in sectionKeys)
            {
                string typeName = iniFile.GetStringValue(sectionName, key, null);

                var objectType = typeof(T);

                // If we encounter the same object listed twice, don't create a duplicate
                var iniNameProperty = objectType.GetProperty("ININame");
                if (targetList.Exists(t => iniNameProperty?.GetValue(t)?.ToString() == typeName))
                    continue;

                // We assume that the type has a constructor
                // that takes a single string (ININame) as a parameter
                var constructor = objectType.GetConstructor(new Type[] { typeof(string) });
                if (constructor == null)
                {
                    throw new InvalidOperationException(typeof(T).FullName +
                        " has no public constructor that takes a single string as an argument!");
                }

                T objectInstance = (T)constructor.Invoke(new object[] { typeName });

                // Set the index property if one exists
                var indexProperty = objectType.GetProperty("Index");
                if (indexProperty != null)
                    indexProperty.SetValue(objectInstance, i);

                targetList.Add(objectInstance);
                i++;
            }
        }

        public InfantrySequence FindOrMakeInfantrySequence(IniFile artIni, string infantrySequenceName)
        {
            var existing = InfantrySequences.Find(seq => seq.ININame == infantrySequenceName);
            if (existing == null)
            {
                existing = new InfantrySequence(infantrySequenceName);
                var section = artIni.GetSection(infantrySequenceName);
                if (section == null)
                    throw new KeyNotFoundException("Infantry sequence not found: " + infantrySequenceName);

                existing.ParseFromINISection(section);
                InfantrySequences.Add(existing);
            }

            return existing;
        }

        public TechnoType FindTechnoType(string technoTypeININame)
        {
            TechnoType returnValue = AircraftTypes.Find(at => at.ININame == technoTypeININame);

            if (returnValue != null)
                return returnValue;

            returnValue = BuildingTypes.Find(bt => bt.ININame == technoTypeININame);

            if (returnValue != null)
                return returnValue;

            returnValue = InfantryTypes.Find(it => it.ININame == technoTypeININame);

            if (returnValue != null)
                return returnValue;

            return UnitTypes.Find(ut => ut.ININame == technoTypeININame);
        }

        public OverlayType FindOverlayType(string overlayTypeININame)
        {
            return OverlayTypes.Find(ot => ot.ININame == overlayTypeININame);
        }

        public void SolveDependencies()
        {
            //UnitTypes.ForEach(ot => SolveUnitTypeDependencies(ot));
            //InfantryTypes.ForEach(ot => SolveInfantryTypeDependencies(ot));
            BuildingTypes.ForEach(ot => SolveBuildingTypeDependencies(ot));
            //AircraftTypes.ForEach(ot => SolveAircraftTypeDependencies(ot));
            //TerrainTypes.ForEach(ot => SolveTerrainTypeDependencies(ot));
            //OverlayTypes.ForEach(ot => SolveOverlayTypeDependencies(ot));
            //SmudgeTypes.ForEach(ot => SolveSmudgeTypeDependencies(ot));
            //Weapons.ForEach(w => initializer.SolveWeaponDependencies(w))
            //AnimTypes.ForEach(a => SolveAnimTypeDependencies(a));
        }

        private void SolveBuildingTypeDependencies(BuildingType type)
        {
            var anims = new List<AnimType>();

            foreach (var buildingAnimType in type.ArtConfig.BuildingAnimTypes)
            {
                AnimType anim = AnimTypes.Find(at => at.ININame == buildingAnimType.ININame);
                if (anim != null)
                {
                    anim.ArtConfig.IsBuildingAnim = true;
                    anim.ArtConfig.BuildingAnimYSort = buildingAnimType.YSort;
                    anim.ArtConfig.BuildingAnimZAdjust = buildingAnimType.ZAdjust;
                    anims.Add(anim);
                }
            }

            type.ArtConfig.Anims = anims.ToArray();

            if (type.Turret && !type.TurretAnimIsVoxel)
            {
                var turretAnim = AnimTypes.Find(at => at.ININame == type.TurretAnim);
                if (turretAnim != null)
                {
                    turretAnim.ArtConfig.IsBuildingAnim = true;
                    type.ArtConfig.TurretAnim = turretAnim;
                }
            }
        }
    }
}
