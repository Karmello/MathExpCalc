using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {

	static class Warning {

		public enum WarningType { INVALID_EDGE_CHAR, INVALID_FOLLOWING_CHAR, WRONG_BRACKET, EXTRA_BRACKET, UNKNOWN_COMMAND, INVALID_EXPRESSION, INVALID_MATH_OPERATOR, INVALID_NUMERIC_VALUE, INVALID_FUNCTION_CALL, INVALID_FACTORIAL_ARG, ZERO_DIVISION, INVALID_VAR_NAME, INVALID_USE_OF_OPER };
		public static SolidColorBrush WarningColor = Brushes.Red;

		public static bool warningEntryOut = false;

		/* Public methods */
		public static Run[] CreateDynamicWarningMessage(Warning.WarningType warningType, int[] infoNumbers, Character.Edge edge = Character.Edge.NONE) {

			#region CONSOLE_PRINT_OUT

			if (MainWindow.consolePrintOutEnabled) { Console.WriteLine("\n" + warningType); }

			#endregion

			List<Run> runOfText = new List<Run>();
			int start = -1;
			int length = -1;
			string unknownString = String.Empty;
			string text = String.Empty;
			Character previousCharObj = null;
			Character invalidCharObj = null;



			switch (warningType) {

				// Initial syntax validation
				#region INVALID_EDGE_CHAR

				case Warning.WarningType.INVALID_EDGE_CHAR:

					// Getting char object
					invalidCharObj = Character.Characters.Find(c => c.Phrase[0] == Input.input[infoNumbers[0]]);

					// Adding runs
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid syntax at "));
					runOfText.Add(new Run("pos. " + (infoNumbers[0] + 1) + "\n") { Foreground = WarningColor });
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);

					// First character
					if (edge == Character.Edge.LEFT) { runOfText.Add(new Run("Mathematical expression must not start with " + invalidCharObj.Description));

					// Last character
					} else if (edge == Character.Edge.RIGHT) { runOfText.Add(new Run("Mathematical expression must not end with " + invalidCharObj.Description)); }

					runOfText.Add(new Run("\n"));

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select(infoNumbers[0], 1);
					break;

				#endregion
				#region INVALID_FOLLOWING_CHAR

				case Warning.WarningType.INVALID_FOLLOWING_CHAR:

					// Getting char object
					previousCharObj = Character.Characters.Find(c => c.Phrase[0] == Input.input[infoNumbers[0] - 1]);
					invalidCharObj = Character.Characters.Find(c => c.Phrase[0] == Input.input[infoNumbers[0]]);

					// Adding runs
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid syntax at "));
					runOfText.Add(new Run("pos. " + (infoNumbers[0] + 1) + "\n") { Foreground = WarningColor });
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);

					// Two different chars
					if (previousCharObj.Phrase != invalidCharObj.Phrase) {
						runOfText.Add(new Run(invalidCharObj.Description[0].ToString().ToUpper() + invalidCharObj.Description.Substring(1) + " must not appear after " + previousCharObj.Description));

					// The same chars
					} else { runOfText.Add(new Run(invalidCharObj.Description[0].ToString().ToUpper() + invalidCharObj.Description.Substring(1) + " must not appear twice in a row")); }

					runOfText.Add(new Run("\n"));

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select(infoNumbers[0], 1);
					break;

				#endregion
				#region WRONG_BRACKET

				case Warning.WarningType.WRONG_BRACKET:

					// Getting char object
					GroupingSymbol bracketObj = GroupingSymbol.GroupingSymbols.Find(c => c.Phrase[0] == Input.input[infoNumbers[0]]);
					if (bracketObj.Shape == GroupingSymbol.GroupingSymbolShape.ROUND) { text = "square"; } else if (bracketObj.Shape == GroupingSymbol.GroupingSymbolShape.SQUARE) { text = "round"; }

					// Adding runs
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid syntax at "));
					runOfText.Add(new Run("pos. " + (infoNumbers[0] + 1) + "\n") { Foreground = WarningColor });
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run("Change this bracket to " + text + " one"));

					runOfText.Add(new Run("\n"));

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select(infoNumbers[0], 1);
					break;

				#endregion
				#region EXTRA_BRACKET

				case Warning.WarningType.EXTRA_BRACKET:

					// Adding runs
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid syntax at "));
					runOfText.Add(new Run("pos. " + (infoNumbers[0] + 1) + "\n") { Foreground = WarningColor });
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run("Remove or close this bracket"));
					runOfText.Add(new Run("\n"));

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select(infoNumbers[0], 1);
					break;

				#endregion

				// Command validation
				#region UNKNOWN_COMMAND

				case Warning.WarningType.UNKNOWN_COMMAND:

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Unknown command '") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run(MainWindow.ui.textBox.Text.Substring(1)) { Foreground = Warning.WarningColor });
					runOfText.Add(new Run("'\n") { Foreground = RunOfText.StandardTextColor });

					MainWindow.ui.textBox.Select(1, MainWindow.ui.textBox.Text.Length - 1);
					break;

				#endregion

				// Math expressions validation
				#region INVALID_EXPRESSION

				case Warning.WarningType.INVALID_EXPRESSION:

					unknownString = (string)Input.exceptionInfo[1];
					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];

					text = "Invalid mathematical expression '";

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run(text) { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run(unknownString) { Foreground = Warning.WarningColor });
					runOfText.Add(new Run("' at ") { Foreground = RunOfText.StandardTextColor });

					if (length > 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });

					} else if (length == 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					}

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run("Type in '") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run("$oper") { Foreground = Command.SystemCmdColor });
					runOfText.Add(new Run("', '") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run("$const") { Foreground = Command.SystemCmdColor });
					runOfText.Add(new Run("' or '") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run("$var") { Foreground = Command.SystemCmdColor });
					runOfText.Add(new Run("' to see available expressions\n") { Foreground = RunOfText.StandardTextColor });

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region INVALID_MATH_OPERATOR

				case WarningType.INVALID_MATH_OPERATOR:

					unknownString = (string)Input.exceptionInfo[1];
					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];

					text = "Invalid math operator '";

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run(text) { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run(unknownString) { Foreground = Warning.WarningColor });
					runOfText.Add(new Run("' at ") { Foreground = RunOfText.StandardTextColor });

					if (length > 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });

					} else if (length == 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					}

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run("Type in '") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run("$oper") { Foreground = Command.SystemCmdColor });
					runOfText.Add(new Run("' to see a list of all operators\n") { Foreground = RunOfText.StandardTextColor });

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region INVALID_NUMERIC_VALUE:
				
				case WarningType.INVALID_NUMERIC_VALUE:

					unknownString = (string)Input.exceptionInfo[1];
					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];

					text = "Invalid numeric value '";

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run(text) { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run(unknownString) { Foreground = Warning.WarningColor });
					runOfText.Add(new Run("' at ") { Foreground = RunOfText.StandardTextColor });

					if (length > 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });

					} else if (length == 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					}

					MainWindow.ui.textBox.Text = Input.input;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region INVALID_FACTORIAL_ARG

				case Warning.WarningType.INVALID_FACTORIAL_ARG:

					start = (int)Input.exceptionInfo[2];
					text = (MathOperator.MathOperators.Find(oper => oper.Phrase == (string)Input.exceptionInfo[1]) as ArithmeticOperator).ErrorDescription;

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid factorial operation at ") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run(text + "\n") { Foreground = RunOfText.StandardTextColor });

					MainWindow.ui.textBox.Text = Expression.expression;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region INVALID_FUNCTION_CALL

				case Warning.WarningType.INVALID_FUNCTION_CALL:

					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];
					text = (MathOperator.MathOperators.Find(oper => oper.Phrase == (string)Input.exceptionInfo[1]) as Function).ErrorDescription;

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid function call at ") { Foreground = RunOfText.StandardTextColor });
					runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });
					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run(text + "\n") { Foreground = RunOfText.StandardTextColor });

					MainWindow.ui.textBox.Text = Expression.expression;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region ZERO_DIVISION

				case Warning.WarningType.ZERO_DIVISION:

					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];
					text = (MathOperator.MathOperators.Find(oper => oper.Phrase == (string)Input.exceptionInfo[1])).ErrorDescription;

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid division operation at ") { Foreground = RunOfText.StandardTextColor });

					if (length > 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });

					} else if (length == 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					}

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run(text + "\n"));


					MainWindow.ui.textBox.Text = Expression.expression;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region INVALID_VAR_NAME

				case WarningType.INVALID_VAR_NAME:

					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];
					text = ConstantValue.ConstantValues.Find(obj => obj.Phrase == (Input.exceptionInfo[1]).ToString().ToUpper()).Phrase;

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid use of constant at ") { Foreground = RunOfText.StandardTextColor });

					if (length > 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });

					} else if (length == 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					}

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run("'"));
					runOfText.Add(new Run(text) { Foreground = ConstantValue.ConstColor });
					runOfText.Add(new Run("' is already a constant, you can't use it as a variable. Choose different name.\n"));



					MainWindow.ui.textBox.Text = Expression.expression;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion
				#region INVALID_USE_OF_OPER

				case WarningType.INVALID_USE_OF_OPER:

					text = (string)Input.exceptionInfo[1];
					start = (int)Input.exceptionInfo[2];
					length = (int)Input.exceptionInfo[3];
					MathOperator op = MathOperator.MathOperators.Find(obj => obj.Phrase == text);

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING]);
					runOfText.Add(new Run("Invalid use of operator at ") { Foreground = RunOfText.StandardTextColor });

					if (length > 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "-" + (start + length) + "\n") { Foreground = Warning.WarningColor });

					} else if (length == 1) {
						runOfText.Add(new Run("pos. " + (start + 1) + "\n") { Foreground = Warning.WarningColor });
					}

					runOfText.Add(RunOfText.Prefixes[(int)RunOfText.Prefix.DESCRIPTION]);
					runOfText.Add(new Run(op.ErrorDescription));
					runOfText.Add(new Run("\n"));

					MainWindow.ui.textBox.Text = Expression.expression;
					MainWindow.ui.textBox.Select((int)Input.exceptionInfo[2], (int)Input.exceptionInfo[3]);
					break;

				#endregion

				default:
					break;
			}

			return runOfText.ToArray();
		}
	}
}