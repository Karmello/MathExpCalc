using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {
	
	static class RunOfText {

		/* Colors */
		public static SolidColorBrush StandardTextColor = Brushes.WhiteSmoke;
		public static SolidColorBrush AltTextColor = Brushes.LightSkyBlue;
		public static SolidColorBrush KeyColor = Brushes.Khaki;

		public static SolidColorBrush EntryPrefixColor = Brushes.LightSkyBlue;
		public static SolidColorBrush WarningPartPrefixColor = Brushes.Red;
		public static SolidColorBrush DescriptionPartPrefixColor = Brushes.LightSkyBlue;

		/* Prefixes */
		public enum Prefix { STANDARD = 0, WARNING = 1, WARNING_NO_EMPTY_LINE = 2, DESCRIPTION = 3, RESULT = 4, PAUSE = 5 }

		public static readonly Run[] Prefixes = new Run[] {
			
			new Run("#  ") { Foreground = RunOfText.EntryPrefixColor },
			new Run("\n!:  ") { Foreground = RunOfText.WarningPartPrefixColor },
			new Run("!:  ") { Foreground = RunOfText.WarningPartPrefixColor },
			new Run("?:  ") { Foreground = RunOfText.DescriptionPartPrefixColor },
			new Run(" = ") { Foreground = RunOfText.AltTextColor },
			new Run(" - ") { Foreground = RunOfText.StandardTextColor }
		};

		/* Public methods */
		public static Run[] CreateInitialPage() {

			var page = new List<Run>();

			page.Add(Prefixes[(int)Prefix.STANDARD]);
			page.Add(new Run("Type in '") { Foreground = StandardTextColor });
			page.Add(new Run(Character.Characters[0].Phrase + Command.Commands[1].Cmd.ToString().ToLower()) { Foreground = Command.SystemCmdColor });
			page.Add(new Run("' to see helpful information\n") { Foreground = StandardTextColor });

			return page.ToArray();
		}
		public static Run[] CreateCommandPage() {

			var page = new List<Run>();

			page.Add(new Run("Every system command should be preceded by a ") { Foreground = StandardTextColor });
			page.Add(new Run(Command.CmdChar.ToString()) { Foreground = Command.SystemCmdColor });
			page.Add(new Run(" sign,\nyou can also combine ") { Foreground = StandardTextColor });
			page.Add(new Run(Command.CmdChar.ToString()) { Foreground = Command.SystemCmdColor });
			page.Add(new Run(" with math operators, constants and variables\n\n") { Foreground = StandardTextColor });

			for (int i = 1; i < Command.Commands.Count; ++i) {

				page.Add(new Run("   "));
				page.Add(new Run(Character.Characters[0].Phrase + Command.Commands[i].Cmd.ToString().ToLower()) { Foreground = Command.SystemCmdColor });
				page.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
				page.Add(new Run(Command.Commands[i].Description) { Foreground = StandardTextColor });
				page.Add(new Run("\n"));
			}

			page.Add(new Run("\n\n\nWritten by Kamil Noga\n") { Foreground = StandardTextColor });
			page.Add(new Run("nogakamil.co.nf\n") { Foreground = Command.SystemCmdColor });

			return page.ToArray();
		}
		public static Run[] CreateKeyPage() {

			var page = new List<Run>();

			page.Add(new Run("Keys\n\n   ") { Foreground = StandardTextColor });

			page.Add(new Run("ENTER") { Foreground = KeyColor });
			page.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
			page.Add(new Run("accepts command\n   ") { Foreground = StandardTextColor });
			page.Add(new Run("ESC") { Foreground = KeyColor });
			page.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
			page.Add(new Run("clears command / exits program\n   ") { Foreground = StandardTextColor });
			page.Add(new Run("F5") { Foreground = KeyColor });
			page.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
			page.Add(new Run("cleans calculation board\n   ") { Foreground = StandardTextColor });
			page.Add(new Run("UP / DOWN ARROW") { Foreground = KeyColor });
			page.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
			page.Add(new Run("goes through command history") { Foreground = StandardTextColor });

			return page.ToArray();
		}
		public static Run[] CreateMathOperPage() {

			var page = new List<Run>();
			var last1 = MathOperator.MathOperators.FindLast(obj => obj is Function);
			var last2 = MathOperator.MathOperators.FindLast(obj => obj is ArithmeticOperator);

			page.Add(new Run("Math operators\n\n") { Foreground = StandardTextColor });
			foreach (var oper in MathOperator.MathOperators) {
				page.Add(new Run("	" + oper.Phrase) { Foreground = MathOperator.MathOperatorColor });
				if (oper is Function) { page.Add(new Run("()") { Foreground = MathOperator.MathOperatorColor }); }
				page.Add(new Run("		" + RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
				page.Add(new Run(oper.Description) { Foreground = StandardTextColor });

				if (oper.Phrase == last1.Phrase || oper.Phrase == last2.Phrase) { page.Add(new Run("\n\n")); } else { page.Add(new Run("\n")); }
			}

			page.Add(new Run("\nOther symbols\n\n      ") { Foreground = StandardTextColor });
			foreach (var item in Digit.Digits) {
				page.Add(new Run(item.Phrase + "   ") { Foreground = MathOperator.MathOperatorColor });
			}
			foreach (var item in Delimiter.Delimiters) {
				page.Add(new Run(item.Phrase + "   ") { Foreground = MathOperator.MathOperatorColor });
			}
			foreach (var item in GroupingSymbol.GroupingSymbols) {
				page.Add(new Run(item.Phrase + "   ") { Foreground = MathOperator.MathOperatorColor });
			}
			foreach (var item in KnownPhrase.knownPhrases) {
				page.Add(new Run(item.Phrase + "   ") { Foreground = MathOperator.MathOperatorColor });
			}

			page.Add(new Run("\n"));

			return page.ToArray();
		}
		public static Run[] CreateConstantPage() {

			var page = new List<Run>();

			page.Add(new Run("Constant values\n\n") { Foreground = StandardTextColor });
			
			foreach (var item in ConstantValue.ConstantValues) {

				page.Add(new Run("   " + item.Phrase) { Foreground = ConstantValue.ConstColor });
				page.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = StandardTextColor });
				page.Add(new Run(item.Description + "\n") { Foreground = StandardTextColor });
			}

			return page.ToArray();
		}
	}
}