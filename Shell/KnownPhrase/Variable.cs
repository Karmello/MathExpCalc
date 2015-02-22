using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell{

	class Variable : KnownPhrase {

		public static List<Variable> Variables = new List<Variable>();
		public static SolidColorBrush VarColor = Brushes.Khaki;

		/* Properties */
		public double Value { get; set; }

		/* Public methods */
		public Variable(object[] args) {

			Phrase = (string)args[0];
			Value = (double)args[1];
		}
		public static void RefreshVariables() {

			Output.ClearScreen();

			if (Variables.Count > 0) {

				MainWindow.ui.textBlock.Inlines.Add(new Run("Variables\n\n") { Foreground = RunOfText.StandardTextColor });

				foreach (var variable in Variables) {
					MainWindow.ui.textBlock.Inlines.Add(new Run("   " + variable.Phrase) { Foreground = VarColor });
					MainWindow.ui.textBlock.Inlines.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.RESULT].Text) { Foreground = RunOfText.AltTextColor });
					MainWindow.ui.textBlock.Inlines.Add(new Run(variable.Value + "\n") { Foreground = RunOfText.StandardTextColor });
				}
			} else {

				MainWindow.ui.textBlock.Inlines.Add(new Run("No variables, type in something like 'x = 5' to add one") { Foreground = RunOfText.StandardTextColor });
			}

			Output.ActiveScreen = Output.Screens.VARS;
			MainWindow.ui.Scroller.ScrollToBottom();
		}
		public static void AddNewVariable(string name, double value) {

			Variables.Add(new Variable(new object[] { name, value }));
		}
		public static void DeleteVariables() {

			Variables.Clear();
		}
	}
}