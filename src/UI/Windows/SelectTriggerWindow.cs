﻿using System;
using Rampastring.XNAUI;
using Rampastring.XNAUI.XNAControls;
using TSMapEditor.Models;

namespace TSMapEditor.UI.Windows
{
    public class SelectTriggerWindow : SelectObjectWindow<Trigger>
    {
        public SelectTriggerWindow(WindowManager windowManager, Map map) : base(windowManager)
        {
            this.map = map;
        }

        private readonly Map map;

        public override void Initialize()
        {
            Name = nameof(SelectTriggerWindow);
            base.Initialize();
        }

        protected override void LbObjectList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbObjectList.SelectedItem == null)
            {
                SelectedObject = null;
                return;
            }

            SelectedObject = (Trigger)lbObjectList.SelectedItem.Tag;
        }

        protected override void ListObjects()
        {
            lbObjectList.Clear();

            foreach (Trigger trigger in map.Triggers)
            {
                lbObjectList.AddItem(new XNAListBoxItem() { Text = $"{trigger.ID} {trigger.Name}", Tag = trigger });
                if (trigger == SelectedObject)
                    lbObjectList.SelectedIndex = lbObjectList.Items.Count - 1;
            }

            // If the initial selection wasn't found for some reason, then clear selection
            if (lbObjectList.SelectedItem == null)
                SelectedObject = null;
        }
    }
}
