using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {

	class StaticPage {

		public static List<StaticPage> StaticPages = null;

		/* Properties */
		public Command.Cmds InvokeCmd { get; set; }
		public Run[] Text { get; set; }
		
		/* Public methods */
		public StaticPage(object[] args) {

			InvokeCmd = (Command.Cmds)args[0];
			Text = (Run[])args[1];
		}
		public static void CreateStaticPages() {

			StaticPages = new List<StaticPage>();

			StaticPages.Add(new StaticPage(new object[] { Command.Cmds.START, RunOfText.CreateInitialPage() }));
			StaticPages.Add(new StaticPage(new object[] { Command.Cmds.CMD, RunOfText.CreateCommandPage() }));
			StaticPages.Add(new StaticPage(new object[] { Command.Cmds.KEY, RunOfText.CreateKeyPage() }));
			StaticPages.Add(new StaticPage(new object[] { Command.Cmds.OPER, RunOfText.CreateMathOperPage() }));
			StaticPages.Add(new StaticPage(new object[] { Command.Cmds.CONST, RunOfText.CreateConstantPage() }));
		}
		public static void ShowStaticPage(Command.Cmds invokeCmd = Command.Cmds.NONE, int pageIndex = -1) {

			Output.ClearScreen();
			MainWindow.ui.Scroller.ScrollToTop();

			// If invokeCmd argument is given
			if (invokeCmd != Command.Cmds.NONE) {

				pageIndex = StaticPages.FindIndex(obj => obj.InvokeCmd == invokeCmd);
				Output.StaticPageOut(pageIndex);

			// If pageIndex is given
			} else if (pageIndex != -1) {

				Output.StaticPageOut(pageIndex);
			}

			Output.ActiveScreen = Output.Screens.STATIC_PAGE;
		}
	}
}