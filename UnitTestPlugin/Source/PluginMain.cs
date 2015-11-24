﻿/*
The MIT License (MIT)

Copyright (c) 2014 Gustavo S. Wolff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using TestExplorerPanel.Forms;
using TestExplorerPanel.Source.Handlers;
using TestExplorerPanel.Source.Handlers.MessageHandlers;
using TestExplorerPanel.Source.Localization;
using WeifenLuo.WinFormsUI.Docking;

namespace TestExplorerPanel.Source
{
    public class PluginMain : IPlugin
    {
        #region IPlugin Constants

        public int api = 1;
        public string name = "UnitTests";
        public string guid = "93C76C98-D991-4F19-99EE-6188D7E534E2";
        public string help = "www.flashdevelop.org/community/";
        public string description = "FlashDevelop Plugin for Unit Testing for Haxe and AS3";
        public string author = "Gustavo S. Wolff";

        public string settingsFilename;

        #endregion

        private PluginUI ui;

        private Image image;

        private DockContent panel;

        private IEventHandler processHandler;
        private IEventHandler traceHandler;
        private IEventHandler commandHandler;

        #region IPlugin Getters

        public int Api
        {
            get { return api; }
        }

        public string Author
        {
            get { return author; }
        }

        public string Description
        {
            get { return description; }
        }

        public string Guid
        {
            get { return guid; }
        }

        public string Help
        {
            get { return help; }
        }

        public string Name
        {
            get { return name; }
        }

        public object Settings
        {
            get { return settingsFilename; }
        }

        #endregion

        #region IPlugin Implementation

        public void Initialize()
        {
            InitBasics();
            InitLocalization();
            CreatePluginPanel();
            CreateMenuItem();
            AddEventHandlers();
        }

        public void Dispose()
        {
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
        }

        #endregion

        #region Custom Methods

        public void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, nameof(TestExplorerPanel));

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            settingsFilename = Path.Combine(dataPath, "Settings.fdb");

            image = PluginBase.MainForm.FindImage("101");
        }

        public void InitLocalization()
        {
            LocaleVersion language = PluginBase.MainForm.Settings.LocaleVersion;

            switch (language)
            {
                case LocaleVersion.de_DE:
                case LocaleVersion.eu_ES:
                case LocaleVersion.ja_JP:
                case LocaleVersion.zh_CN:

                default:
                    LocalizationHelper.Initialize(LocaleVersion.en_US);
                    break;
            }

            description = LocalizationHelper.GetString("Description");
        }

        public void CreatePluginPanel()
        {
            ui = new PluginUI(this);
            ui.Text = LocalizationHelper.GetString("PluginPanel");

            panel = PluginBase.MainForm.CreateDockablePanel(ui, guid, image, DockState.DockRight);

            processHandler = new ProcessEventHandler(ui);
            traceHandler = new TraceHandler(ui);
            commandHandler = new CommandHandler(ui);
        }

        public void CreateMenuItem()
        {
            string label = LocalizationHelper.GetString("ViewMenuItem");

            ToolStripMenuItem viewMenu = (ToolStripMenuItem) PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem newItem = new ToolStripMenuItem(label, image, new EventHandler(OpenPanel));

            viewMenu.DropDownItems.Add(newItem);
        }

        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(processHandler, EventType.ProcessStart | EventType.ProcessEnd);
            EventManager.AddEventHandler(traceHandler, EventType.Trace);
            EventManager.AddEventHandler(commandHandler, EventType.Command);
        }

        public void OpenPanel(object sender, EventArgs e)
        {
            panel.Show();
        }

        #endregion
    }
}