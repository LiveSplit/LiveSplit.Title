using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class TitleSettings : UserControl
    {
        public bool ShowGameName { get; set; }
        public bool ShowCategoryName { get; set; }
        public bool ShowAttemptCount { get; set; }
        public bool ShowFinishedRunsCount { get; set; }
        public bool ShowCount => ShowAttemptCount || ShowFinishedRunsCount;
        public bool CenterTitle { get; set; }
        public bool SingleLine { get; set; }
        public bool DisplayGameIcon { get; set; }

        public bool ShowRegion { get; set; }
        public bool ShowPlatform { get; set; }
        public bool ShowVariables { get; set; }

        public Color TitleColor { get; set; }
        public bool OverrideTitleColor { get; set; }

        public string TitleFontString => SettingsHelper.FormatFont(TitleFont);

        public Font TitleFont { get; set; }
        public bool OverrideTitleFont { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public TitleSettings()
        {
            InitializeComponent();
            ShowGameName = true;
            ShowCategoryName = true;
            ShowAttemptCount = true;
            ShowFinishedRunsCount = false;
            DisplayGameIcon = true;
            TitleFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
            OverrideTitleFont = false;
            TitleColor = Color.FromArgb(255, 255, 255, 255);
            OverrideTitleColor = false;
            CenterTitle = false;
            SingleLine = false;
            ShowRegion = false;
            ShowPlatform = false;
            ShowVariables = true;
            BackgroundColor = Color.FromArgb(255, 42, 42, 42);
            BackgroundColor2 = Color.FromArgb(255, 19, 19, 19);
            BackgroundGradient = GradientType.Vertical;

            chkGameName.DataBindings.Add("Checked", this, "ShowGameName", false, DataSourceUpdateMode.OnPropertyChanged);
            chkCategoryName.DataBindings.Add("Checked", this, "ShowCategoryName", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAttemptCount.DataBindings.Add("Checked", this, "ShowAttemptCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFinishedRuns.DataBindings.Add("Checked", this, "ShowFinishedRunsCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFont.DataBindings.Add("Checked", this, "OverrideTitleFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblFont.DataBindings.Add("Text", this, "TitleFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkColor.DataBindings.Add("Checked", this, "OverrideTitleColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSingleLine.DataBindings.Add("Checked", this, "SingleLine", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor.DataBindings.Add("BackColor", this, "TitleColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayGameIcon.DataBindings.Add("Checked", this, "DisplayGameIcon", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRegion.DataBindings.Add("Checked", this, "ShowRegion", false, DataSourceUpdateMode.OnPropertyChanged);
            chkPlatform.DataBindings.Add("Checked", this, "ShowPlatform", false, DataSourceUpdateMode.OnPropertyChanged);
            chkVariables.DataBindings.Add("Checked", this, "ShowVariables", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        void TitleSettings_Load(object sender, EventArgs e)
        {
            chkColor_CheckedChanged(null, null);
            chkFont_CheckedChanged(null, null);
            chkDisplayGameIcon_CheckedChanged(null, null);
        }

        void chkColor_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = btnColor.Enabled = chkColor.Checked;
        }

        void chkFont_CheckedChanged(object sender, EventArgs e)
        {
            label1.Enabled = lblFont.Enabled = btnFont.Enabled = chkFont.Checked;
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            if (version >= new Version(1, 2))
            {
                TitleFont = SettingsHelper.GetFontFromElement(element["TitleFont"]);
                if (version >= new Version(1, 3))
                    OverrideTitleFont = SettingsHelper.ParseBool(element["OverrideTitleFont"]);
                else
                    OverrideTitleFont = !SettingsHelper.ParseBool(element["UseLayoutSettingsFont"]);
            }
            else
            {
                TitleFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
                OverrideTitleFont = false;
            }

            ShowGameName = SettingsHelper.ParseBool(element["ShowGameName"], true);
            ShowCategoryName = SettingsHelper.ParseBool(element["ShowCategoryName"], true);
            ShowAttemptCount = SettingsHelper.ParseBool(element["ShowAttemptCount"]);
            TitleColor = SettingsHelper.ParseColor(element["TitleColor"], Color.FromArgb(255, 255, 255, 255));
            OverrideTitleColor = SettingsHelper.ParseBool(element["OverrideTitleColor"], false);
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.FromArgb(42, 42, 42, 255));
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.FromArgb(19, 19, 19, 255));
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], GradientType.Vertical.ToString());
            DisplayGameIcon = SettingsHelper.ParseBool(element["DisplayGameIcon"], true);
            ShowFinishedRunsCount = SettingsHelper.ParseBool(element["ShowFinishedRunsCount"], false);
            CenterTitle = SettingsHelper.ParseBool(element["CenterTitle"], false);
            SingleLine = SettingsHelper.ParseBool(element["SingleLine"], false);
            ShowRegion = SettingsHelper.ParseBool(element["ShowRegion"], false);
            ShowPlatform = SettingsHelper.ParseBool(element["ShowPlatform"], false);
            ShowVariables = SettingsHelper.ParseBool(element["ShowVariables"], true);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode() => CreateSettingsNode(null, null);

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            return SettingsHelper.CreateSetting(document, parent, "Version", "1.7") ^
            SettingsHelper.CreateSetting(document, parent, "ShowGameName", ShowGameName) ^
            SettingsHelper.CreateSetting(document, parent, "ShowCategoryName", ShowCategoryName) ^
            SettingsHelper.CreateSetting(document, parent, "ShowAttemptCount", ShowAttemptCount) ^
            SettingsHelper.CreateSetting(document, parent, "ShowFinishedRunsCount", ShowFinishedRunsCount) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTitleFont", OverrideTitleFont) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTitleColor", OverrideTitleColor) ^
            SettingsHelper.CreateSetting(document, parent, "TitleFont", TitleFont) ^
            SettingsHelper.CreateSetting(document, parent, "CenterTitle", CenterTitle) ^
            SettingsHelper.CreateSetting(document, parent, "SingleLine", SingleLine) ^
            SettingsHelper.CreateSetting(document, parent, "TitleColor", TitleColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayGameIcon", DisplayGameIcon) ^
            SettingsHelper.CreateSetting(document, parent, "ShowRegion", ShowRegion) ^
            SettingsHelper.CreateSetting(document, parent, "ShowPlatform", ShowPlatform) ^
            SettingsHelper.CreateSetting(document, parent, "ShowVariables", ShowVariables);
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            var dialog = SettingsHelper.GetFontDialog(TitleFont, 7, 20);
            dialog.FontChanged += (s, ev) => TitleFont = ((CustomFontDialog.FontChangedEventArgs)ev).NewFont;
            dialog.ShowDialog(this);
            lblFont.Text = TitleFontString;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void chkDisplayGameIcon_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDisplayGameIcon.Checked)
            {
                chkCenter.Enabled = true;
                chkCenter.DataBindings.Clear();
                chkCenter.DataBindings.Add("Checked", this, "CenterTitle", false, DataSourceUpdateMode.OnPropertyChanged);
            }
            else
            {
                chkCenter.Enabled = false;
                chkCenter.DataBindings.Clear();
                chkCenter.Checked = true;
            }
        }
    }
}
