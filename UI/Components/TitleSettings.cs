using Fetze.WinFormsColor;
using LiveSplit.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public partial class TitleSettings : UserControl
    {
        public bool ShowAttemptCount { get; set; }
        public bool ShowFinishedRunsCount { get; set; }
        public bool ShowCount { get { return ShowAttemptCount || ShowFinishedRunsCount; } }
        public bool DisplayGameIcon { get; set; }

        public Color TitleColor { get; set; }
        public bool OverrideTitleColor { get; set; }
        public bool CenterTitle { get; set; }

        public string TitleFontString { get { return String.Format("{0} {1}", TitleFont.FontFamily.Name, TitleFont.Style); } }

        public Font TitleFont { get; set; }
        public bool OverrideTitleFont { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }
        public GradientType BackgroundGradient { get; set; }
        public String GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (GradientType)Enum.Parse(typeof(GradientType), value); }
        }

        public TitleSettings()
        {
            InitializeComponent();
            ShowAttemptCount = true;
            ShowFinishedRunsCount = false;
            DisplayGameIcon = true;
            TitleFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
            OverrideTitleFont = false;
            TitleColor = Color.FromArgb(255, 255, 255, 255);
            OverrideTitleColor = false;
            CenterTitle = true;
            BackgroundColor = Color.FromArgb(255, 42, 42, 42);
            BackgroundColor2 = Color.FromArgb(255, 19, 19, 19);
            BackgroundGradient = GradientType.Vertical;

            chkAttemptCount.DataBindings.Add("Checked", this, "ShowAttemptCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFinishedRuns.DataBindings.Add("Checked", this, "ShowFinishedRunsCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFont.DataBindings.Add("Checked", this, "OverrideTitleFont", false, DataSourceUpdateMode.OnPropertyChanged);
            lblFont.DataBindings.Add("Text", this, "TitleFontString", false, DataSourceUpdateMode.OnPropertyChanged);
            chkColor.DataBindings.Add("Checked", this, "OverrideTitleColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkCenter.DataBindings.Add("Checked", this, "CenterTitle", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor.DataBindings.Add("BackColor", this, "TitleColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayGameIcon.DataBindings.Add("Checked", this, "DisplayGameIcon", false, DataSourceUpdateMode.OnPropertyChanged);

            cmbGradientType.SelectedIndexChanged += cmbGradientType_SelectedIndexChanged;
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);

            chkFont.CheckedChanged += chkFont_CheckedChanged;
            chkColor.CheckedChanged += chkColor_CheckedChanged;

            this.Load += TitleSettings_Load;
        }

        void TitleSettings_Load(object sender, EventArgs e)
        {
            chkColor_CheckedChanged(null, null);
            chkFont_CheckedChanged(null, null);
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
            Version version;
            if (element["Version"] != null)
                version = Version.Parse(element["Version"].InnerText);
            else
                version = new Version(1, 0, 0, 0);
            ShowAttemptCount = Boolean.Parse(element["ShowAttemptCount"].InnerText);
            if (version >= new Version(1, 2))
            {
                TitleFont = GetFontFromElement(element["TitleFont"]);
                if (version >= new Version(1, 3))
                    OverrideTitleFont = Boolean.Parse(element["OverrideTitleFont"].InnerText);
                else
                    OverrideTitleFont = !Boolean.Parse(element["UseLayoutSettingsFont"].InnerText);
            }
            else
            {
                TitleFont = new Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);
                OverrideTitleFont = false;
            }

            TitleColor = ParseColor(element["TitleColor"], Color.FromArgb(255, 255, 255, 255));
            OverrideTitleColor = ParseBool(element["OverrideTitleColor"], false);
            BackgroundColor = ParseColor(element["BackgroundColor"], Color.FromArgb(42, 42, 42, 255));
            BackgroundColor2 = ParseColor(element["BackgroundColor2"], Color.FromArgb(19, 19, 19, 255));
            GradientString = ParseString(element["BackgroundGradient"], GradientType.Vertical.ToString());
            DisplayGameIcon = ParseBool(element["DisplayGameIcon"], true);
            ShowFinishedRunsCount = ParseBool(element["ShowFinishedRunsCount"], false);
            CenterTitle = ParseBool(element["CenterTitle"], false);
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            parent.AppendChild(ToElement(document, "Version", "1.5"));
            parent.AppendChild(ToElement(document, "ShowAttemptCount", ShowAttemptCount));
            parent.AppendChild(ToElement(document, "ShowFinishedRunsCount", ShowFinishedRunsCount));
            parent.AppendChild(ToElement(document, "OverrideTitleFont", OverrideTitleFont));
            parent.AppendChild(ToElement(document, "OverrideTitleColor", OverrideTitleColor));
            parent.AppendChild(CreateFontElement(document, "TitleFont", TitleFont));
            parent.AppendChild(ToElement(document, "CenterTitle", CenterTitle));
            parent.AppendChild(ToElement(document, TitleColor, "TitleColor"));
            parent.AppendChild(ToElement(document, BackgroundColor, "BackgroundColor"));
            parent.AppendChild(ToElement(document, BackgroundColor2, "BackgroundColor2"));
            parent.AppendChild(ToElement(document, "BackgroundGradient", BackgroundGradient));
            parent.AppendChild(ToElement(document, "DisplayGameIcon", DisplayGameIcon));
            return parent;
        }

        private Font ChooseFont(Font previousFont, int minSize, int maxSize)
        {
            var dialog = new FontDialog();
            dialog.Font = previousFont;
            dialog.MinSize = minSize;
            dialog.MaxSize = maxSize;
            try
            {
                var result = dialog.ShowDialog(this);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.Font;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                MessageBox.Show("This font is not supported.", "Font Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return previousFont;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            TitleFont = ChooseFont(TitleFont, 7, 20);
            lblFont.Text = TitleFontString;
        }

        private Font GetFontFromElement(XmlElement element)
        {
            if (!element.IsEmpty)
            {
                var bf = new BinaryFormatter();

                var base64String = element.InnerText;
                var data = Convert.FromBase64String(base64String);
                var ms = new MemoryStream(data);
                return (Font)bf.Deserialize(ms);
            }
            return null;
        }

        private XmlElement CreateFontElement(XmlDocument document, String elementName, Font font)
        {
            var element = document.CreateElement(elementName);

            if (font != null)
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter();

                    bf.Serialize(ms, font);
                    var data = ms.ToArray();
                    var cdata = document.CreateCDataSection(Convert.ToBase64String(data));
                    element.InnerXml = cdata.OuterXml;
                }
            }

            return element;
        }

        private Color ParseColor(XmlElement colorElement, Color defaultColor)
        {
            return colorElement != null ? Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber)) : defaultColor;
        }

        private bool ParseBool(XmlElement boolElement, bool defaultBool)
        {
            return boolElement != null ? Boolean.Parse(boolElement.InnerText) : defaultBool;
        }

        private string ParseString(XmlElement stringElement, String defaultString)
        {
            return stringElement != null ? stringElement.InnerText : defaultString;
        }

        private XmlElement ToElement(XmlDocument document, Color color, string name)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var picker = new ColorPickerDialog();
            picker.SelectedColor = picker.OldColor = button.BackColor;
            picker.SelectedColorChanged += (s, x) => button.BackColor = picker.SelectedColor;
            picker.ShowDialog(this);
            button.BackColor = picker.SelectedColor;
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }
    }
}
