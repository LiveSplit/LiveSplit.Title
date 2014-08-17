using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UI.Components
{
    public class Title : IComponent
    {
        public TitleSettings Settings { get; set; }
        public float VerticalHeight { get; set; }
        public GraphicsCache Cache { get; set; }
        protected int FrameCount { get; set; }
        protected Image OldImage { get; set; }

        public float MinimumWidth
        {
            get { return GameNameLabel.X + AttemptCountLabel.ActualWidth + 5; }
        }

        public float HorizontalWidth
        {
            get { return Math.Max(GameNameLabel.ActualWidth, CategoryNameLabel.ActualWidth + (Settings.ShowAttemptCount ? AttemptCountLabel.ActualWidth : 0)) + GameNameLabel.X + 5; }
        }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return null; }
        }

        public float PaddingTop { get { return 0f; } }
        public float PaddingLeft { get { return 7f; } }
        public float PaddingBottom { get { return 0f; } }
        public float PaddingRight { get { return 7f; } }

        protected SimpleLabel GameNameLabel = new SimpleLabel();
        protected SimpleLabel CategoryNameLabel = new SimpleLabel();
        protected SimpleLabel AttemptCountLabel = new SimpleLabel();

        protected Font TitleFont { get; set; }

        public float MinimumHeight { get; set; }

        public Title()
        {
            VerticalHeight = 10;
            Settings = new TitleSettings();
            Cache = new GraphicsCache();
            GameNameLabel = new SimpleLabel();
            CategoryNameLabel = new SimpleLabel();
            AttemptCountLabel = new SimpleLabel();
        }

        private void DrawGeneral(Graphics g, Model.LiveSplitState state, float width, float height, LayoutMode mode)
        {
                if (Settings.BackgroundColor.ToArgb() != Color.Transparent.ToArgb()
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.ToArgb() != Color.Transparent.ToArgb())
                {
                    var gradientBrush = new LinearGradientBrush(
                                new PointF(0, 0),
                                Settings.BackgroundGradient == GradientType.Horizontal
                                ? new PointF(width, 0)
                                : new PointF(0, height),
                                Settings.BackgroundColor,
                                Settings.BackgroundGradient == GradientType.Plain
                                ? Settings.BackgroundColor
                                : Settings.BackgroundColor2);
                    g.FillRectangle(gradientBrush, 0, 0, width, height);
                }
                if (Settings.OverrideTitleFont)
                    TitleFont = Settings.TitleFont;
                else
                    TitleFont = state.LayoutSettings.TextFont;
                MinimumHeight = g.MeasureString("A", TitleFont).Height * 1.7f;
                VerticalHeight = g.MeasureString("A", TitleFont).Height * 1.7f;
                var showGameIcon = state.Run.GameIcon != null && Settings.DisplayGameIcon;
                if (showGameIcon)
                {
                    var icon = state.Run.GameIcon;

                    if (OldImage != icon)
                    {
                        ImageAnimator.Animate(icon, (s, o) => { });
                        OldImage = icon;
                    }

                    /*if (DateTime.Now.Date.Month == 4 && DateTime.Now.Date.Day == 1)
                    {
                        icon = LiveSplit.Web.Share.TwitchEmoteResolver.Resolve("Kappa", true, false, false);
                    }*/

                    var aspectRatio = (float)icon.Width / icon.Height;
                    var drawWidth = height - 4;
                    var drawHeight = height - 4;
                    if (icon.Width > icon.Height)
                    {
                        var ratio = icon.Height / (float)icon.Width;
                        drawHeight *= ratio;
                    }
                    else
                    {
                        var ratio = icon.Width / (float)icon.Height;
                        drawWidth *= ratio;
                    }

                    ImageAnimator.UpdateFrames(icon);

                    g.DrawImage(
                        icon,
                        7 + (height - 4 - drawWidth) / 2,
                        2 + (height - 4 - drawHeight) / 2,
                        drawWidth,
                        drawHeight);
                }

                GameNameLabel.HorizontalAlignment = showGameIcon || mode == LayoutMode.Horizontal ? StringAlignment.Near : StringAlignment.Center;
                GameNameLabel.VerticalAlignment = String.IsNullOrEmpty(state.Run.CategoryName) ?
                StringAlignment.Center : StringAlignment.Near;
                GameNameLabel.X = (showGameIcon ? (height + 8) : (5 + (mode == LayoutMode.Vertical && Settings.ShowAttemptCount && String.IsNullOrEmpty(state.Run.CategoryName) ? AttemptCountLabel.ActualWidth : 0)));
                GameNameLabel.Y = 0;
                GameNameLabel.Width = (showGameIcon ? (width - height - 13) : (width - 10 - (mode == LayoutMode.Vertical && String.IsNullOrEmpty(state.Run.CategoryName) && Settings.ShowAttemptCount ? AttemptCountLabel.ActualWidth : 0))) - (mode == LayoutMode.Vertical && String.IsNullOrEmpty(state.Run.CategoryName) && Settings.ShowAttemptCount ? AttemptCountLabel.ActualWidth : 0);
                GameNameLabel.Height = height;
                GameNameLabel.Font = TitleFont;
                GameNameLabel.Brush = new SolidBrush(Settings.OverrideTitleColor ? Settings.TitleColor : state.LayoutSettings.TextColor);
                GameNameLabel.HasShadow = state.LayoutSettings.DropShadows;
                GameNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                GameNameLabel.Draw(g);

                if (Settings.ShowAttemptCount)
                {
                    AttemptCountLabel.HorizontalAlignment = StringAlignment.Far;
                    AttemptCountLabel.VerticalAlignment = StringAlignment.Far;
                    AttemptCountLabel.X = 0;
                    AttemptCountLabel.Y = height - 40;
                    AttemptCountLabel.Width = width - 5;
                    AttemptCountLabel.Height = 40;
                    AttemptCountLabel.Font = TitleFont;
                    AttemptCountLabel.Brush = new SolidBrush(Settings.OverrideTitleColor ? Settings.TitleColor : state.LayoutSettings.TextColor);
                    AttemptCountLabel.HasShadow = state.LayoutSettings.DropShadows;
                    AttemptCountLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                    AttemptCountLabel.Draw(g);
                }

                CategoryNameLabel.HorizontalAlignment = (showGameIcon || mode == LayoutMode.Horizontal) ? StringAlignment.Near : StringAlignment.Center;
                CategoryNameLabel.VerticalAlignment = String.IsNullOrEmpty(state.Run.GameName) ?
                StringAlignment.Center : StringAlignment.Far;
                CategoryNameLabel.X = (showGameIcon ? (height + 8) : (5 + (mode == LayoutMode.Vertical && Settings.ShowAttemptCount ? AttemptCountLabel.ActualWidth : 0)));
                CategoryNameLabel.Y = 0;
                CategoryNameLabel.Width = (showGameIcon ? (width - height - 13) : (width - 10 - (mode == LayoutMode.Vertical && Settings.ShowAttemptCount ? AttemptCountLabel.ActualWidth : 0))) - (Settings.ShowAttemptCount ? AttemptCountLabel.ActualWidth : 0);
                CategoryNameLabel.Font = TitleFont;
                CategoryNameLabel.Brush = new SolidBrush(Settings.OverrideTitleColor ? Settings.TitleColor : state.LayoutSettings.TextColor);
                CategoryNameLabel.HasShadow = state.LayoutSettings.DropShadows;
                CategoryNameLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                CategoryNameLabel.Height = height;
                CategoryNameLabel.Draw(g);
        }
        public void DrawHorizontal(Graphics g, Model.LiveSplitState state, float height, Region clipRegion)
        {
            DrawGeneral(g, state, HorizontalWidth, height, LayoutMode.Horizontal);
        }

        public void DrawVertical(System.Drawing.Graphics g, Model.LiveSplitState state, float width, Region clipRegion)
        {
            DrawGeneral(g, state, width, VerticalHeight, LayoutMode.Vertical);
        }

        public string ComponentName
        {
            get { return "Title"; }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            return Settings;
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);  
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }


        public void RenameComparison(string oldName, string newName)
        {
        }


        public void Update(IInvalidator invalidator, Model.LiveSplitState state, float width, float height, LayoutMode mode)
        {
            GameNameLabel.Text = state.Run.GameName;

            if (Settings.ShowAttemptCount)
                AttemptCountLabel.Text = state.Run.AttemptCount.ToString();

            CategoryNameLabel.Text = state.Run.CategoryName;

            Cache.Restart();
            Cache["GameIcon"] = state.Run.GameIcon;
            if (Cache.HasChanged)
            {
                if (state.Run.GameIcon == null)
                    FrameCount = 0;
                else
                    FrameCount = state.Run.GameIcon.GetFrameCount(new FrameDimension(state.Run.GameIcon.FrameDimensionsList[0]));
            }
            Cache["GameNameLabel"] = GameNameLabel.Text;
            Cache["CategoryNameLabel"] = CategoryNameLabel.Text;
            Cache["AttemptCountLabel"] = AttemptCountLabel.Text;

            if (invalidator != null && Cache.HasChanged || FrameCount > 1)
            {
                invalidator.Invalidate(0, 0, width, height);
            }
        }

        public void Dispose()
        {
        }
    }
}
