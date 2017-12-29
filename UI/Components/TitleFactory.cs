using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class TitleFactory : IComponentFactory
    {
        public string ComponentName => "Title";

        public string Description => "Shows the current run title, run category, and game icon.";

        public ComponentCategory Category => ComponentCategory.Information;

        public IComponent Create(LiveSplitState state) => new Title();

        public string UpdateName => ComponentName;

        public string XMLURL => "http://livesplit.org/update/Components/update.LiveSplit.Title.xml";

        public string UpdateURL => "http://livesplit.org/update/";

        public Version Version => Version.Parse("1.7.5");
    }
}
