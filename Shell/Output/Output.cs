using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Shell {

	static class Output {

		public enum Screens { STATIC_PAGE, BOARD, VARS }
		public enum OutputRole { PAGE, PHRASE_DESC, CALCULATION, NOT_CALCULATION, VAR_DEFINITION, WARNING, ALL_TYPES };

		public static Screens ActiveScreen = Screens.STATIC_PAGE;

		/* Public methods */
		public static void StaticPageOut(int pageIndex) {

			MainWindow.ui.textBlock.Inlines.AddRange(StaticPage.StaticPages[pageIndex].Text);
		}
		public static void ClearScreen() {

			MainWindow.ui.textBlock.Text = String.Empty;
		}
	}
}