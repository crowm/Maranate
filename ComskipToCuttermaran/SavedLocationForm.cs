using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ComskipToCuttermaran
{
    public class SavedLocationForm : BaseForm
    {
        private string _settingsWindowName;
        private SettingsData.SavedWindowSettings _settings;
        private bool _initialised;

        private List<SplitContainer> _splitters = new List<SplitContainer>();
        private List<BrightIdeasSoftware.ObjectListView> _columnListViews = new List<BrightIdeasSoftware.ObjectListView>();

        public SavedLocationForm()
        {
            _initialised = false;
        }

        public SavedLocationForm(string name)
        {
            _settingsWindowName = name;
            _initialised = false;
        }

        protected void SetSavedLocationName(string name)
        {
            _settingsWindowName = name;
            if (_settings != null)
                _settings = Settings.Current.SavedWindows[_settingsWindowName];
        }

        protected override void OnLoad(EventArgs e)
        {
            if (_settingsWindowName == null)
                _settingsWindowName = this.GetType().Name + "-" + this.Name;
            if (this.DesignMode)
                _settings = new SettingsData.SavedWindowSettings();
            else
                _settings = Settings.Current.SavedWindows[_settingsWindowName];
            base.OnLoad(e);

            if (StartPosition == FormStartPosition.WindowsDefaultLocation)
            {
                if (_settings.RememberLocation)
                    this.Location = _settings.Location;
                if (_settings.RememberLocation)
                    this.Size = _settings.Size;
                if (_settings.Maximised)
                    this.WindowState = FormWindowState.Maximized;
            }

            foreach (var splitter in _splitters)
            {
                LoadSplitContainer(splitter);
            }
            foreach (var listView in _columnListViews)
            {
                LoadObjectListViewColumns(listView);
            }

            _initialised = true;
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            UpdateSavedState();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateSavedState();
        }

        private void UpdateSavedState()
        {
            if (this.DesignMode)
                return;

            if (!_initialised)
                return;

            if (this.WindowState == FormWindowState.Normal)
            {
                _settings.Location = this.Location;
                _settings.Size = this.Size;
                _settings.Maximised = false;
                _settings.RememberLocation = true;
                _settings.RememberSize = true;
                Settings.Save();
            }
            if (this.WindowState == FormWindowState.Maximized)
            {
                _settings.Maximised = true;
                Settings.Save();
            }
        }

        protected void AddSavedSplitContainer(SplitContainer splitter)
        {
            _splitters.Add(splitter);
            if (_initialised)
                LoadSplitContainer(splitter);
        }
        private void LoadSplitContainer(SplitContainer splitContainer)
        {
            var name = "Splitter:" + splitContainer.Name;
            int val;
            if (_settings.ExtraPosition.TryGetValue(name, out val))
                splitContainer.SplitterDistance = val;

            splitContainer.SplitterMoved += (sender, e) =>
            {
                _settings.ExtraPosition[name] = splitContainer.SplitterDistance;
                Settings.Save();
            };
        }

        private Dictionary<string, int> _columnListViewDefaults = new Dictionary<string, int>();
        protected void AddSavedObjectListViewColumns(BrightIdeasSoftware.ObjectListView objectListView)
        {
            _columnListViews.Add(objectListView);
            foreach (ColumnHeader column in objectListView.Columns)
            {
                var name = "Column:" + objectListView.Name + ":" + column.Text;
                _columnListViewDefaults[name] = column.Width;
            }
            if (_initialised)
                LoadObjectListViewColumns(objectListView);
        }
        private void LoadObjectListViewColumns(BrightIdeasSoftware.ObjectListView objectListView)
        {
            foreach (ColumnHeader column in objectListView.Columns)
            {
                var name = "Column:" + objectListView.Name + ":" + column.Text;
                int val;
                if (_settings.ExtraPosition.TryGetValue(name, out val))
                    column.Width = val;
            }

            objectListView.ColumnWidthChanged += (sender, e) =>
            {
                var column = objectListView.Columns[e.ColumnIndex];
                var name = "Column:" + objectListView.Name + ":" + column.Text;
                if (column.Width != _columnListViewDefaults[name])
                {
                    _settings.ExtraPosition[name] = column.Width;
                    Settings.Save();
                }
            };
        }

    }
}
