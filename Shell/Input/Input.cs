using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {
	
	static class Input {

		public enum InputType { BUILD_IN_CMD, MATH_EXPRESSION };
		public enum ValidationType { EmptyInput, MathExpression };

		/* Variables */
		public static object[] exceptionInfo = null;
		public static string input = String.Empty;
		public static List<string> validInputHistory = new List<string>(200);
		public static int activeHistoryCmd = -1;

		/* Public methods */
		public static void ReceiveInput() {

			MainWindow.timer.Restart();

			Run[] validationResult = null;
			input = MainWindow.ui.textBox.Text;
			input = input.Replace(" ", "").ToLower();



			validationResult = IfEmptyInput(input);

			// Input contains characters different than spaces
			if (validationResult.Length == 0) {

				// System command sign at the zero index
				if (input[0] == Character.Characters[0].Phrase[0]) {

					// If system command sign is the only sign input string contains of
					if (input.Length == 1) {

						BoardEntry.UpdateBoard(Command.CmdCharInfo.ToArray(), Output.OutputRole.PHRASE_DESC);
						MainWindow.ui.textBox.Text = String.Empty;
						SaveInput(InputType.BUILD_IN_CMD);

					} else {

						validationResult = IfInputIsKnownPhrase(input.Replace(Character.Characters[0].Phrase, ""));

						// If input was recognized as a known phrase
						if (validationResult.Length != 0) {

							// If input is a system command
							if (validationResult[0].Text == Character.Characters[0].Phrase) {

								Command cmdObj = Command.Commands.Find(obj => (obj.Cmd.ToString() == input.Replace(Character.Characters[0].Phrase, "").ToUpper()));
								MainWindow.ui.textBox.Text = String.Empty;
								Command.ExecuteSysCmd(cmdObj.Cmd);
								SaveInput(InputType.BUILD_IN_CMD);

							// If input is different known phrase
							} else if (validationResult.Length != 0) {

								BoardEntry.UpdateBoard(validationResult, Output.OutputRole.PHRASE_DESC);
								MainWindow.ui.textBox.Text = String.Empty;
								SaveInput(InputType.BUILD_IN_CMD);
							}

						// If input is a wrong system command
						} else if (input[0] == Character.Characters[0].Phrase[0]) {

							BoardEntry.UpdateBoard(Warning.CreateDynamicWarningMessage(Warning.WarningType.UNKNOWN_COMMAND, new int[] { -1 }, Character.Edge.NONE), Output.OutputRole.WARNING);
						}
					}

				// No system command sign, input is going to be treated as a mathematical expression
				} else {

					validationResult = Expression.MathExpressionValidation(input);

					// Ok after validation
					if (validationResult.Length == 0) {

						// Ready to check for valid mathematical expression
						Tuple<Run[], Output.OutputRole> calculateExpression_Results = Expression.CalculateExpression(input);

						// Adding new entry to console
						BoardEntry.UpdateBoard(calculateExpression_Results.Item1, calculateExpression_Results.Item2);

						// If corrent calculation or assignment a result
						if (calculateExpression_Results.Item2 == Output.OutputRole.CALCULATION || calculateExpression_Results.Item2 == Output.OutputRole.VAR_DEFINITION) {

							MainWindow.ui.textBox.Text = String.Empty;
							SaveInput(InputType.MATH_EXPRESSION);

						// If warning as a result
						} else { exceptionInfo = null; }

					// Something went wrong
					} else { BoardEntry.UpdateBoard(validationResult, Output.OutputRole.WARNING); }
				}

				// Timer
				MainWindow.timer.Stop();
				if (!MainWindow.closingApp) { MainWindow.ui.timeLabel.Content = MainWindow.timeLabelText + MainWindow.timer.ElapsedMilliseconds + " ms"; }

			// Only spaces or empty string as input
			} else {

				//Output.RemoveFromBoard(Output.OutputRole.WARNING);
				MainWindow.ui.textBox.Text = String.Empty;
				MainWindow.ui.timeLabel.Content = MainWindow.timeLabelText + "-";
			}
		}
		public static void FixInput() {

			/* This method adds asterikses, removes unnecessary + signs */



			// Going through all characters backwards
			for (int i = Expression.expression.Length - 1; i >= 1; --i) {

				// Closing and opening bracket next to each other
				if ((Expression.expression[i - 1] == ')' && Expression.expression[i] == '(') || (Expression.expression[i - 1] == ']' && Expression.expression[i] == '[') || (Expression.expression[i - 1] == ')' && Expression.expression[i] == '[') || (Expression.expression[i - 1] == ']' && Expression.expression[i] == '(')) {

					// Adding asteriks
					Expression.expression = Expression.expression.Insert(i, "*");
				
				// Digit and opening bracket
				} else if (Char.IsDigit(Expression.expression[i - 1]) && (Expression.expression[i] == '(' || Expression.expression[i] == '[')) {

					// Adding asteriks
					Expression.expression = Expression.expression.Insert(i, "*");

				// Closing bracket and digit
				} else if ((Expression.expression[i - 1] == ')' || Expression.expression[i - 1] == ']') && Char.IsDigit(Expression.expression[i])) {

					// Adding asteriks
					Expression.expression = Expression.expression.Insert(i, "*");
				
				// Opening bracket, plus and digit
				} else if ((Expression.expression[i - 1] == '(' || Expression.expression[i - 1] == '[') && Expression.expression[i] == '+' && Char.IsDigit(Expression.expression[i + 1])) {

					// Removing plus
					Expression.expression = Expression.expression.Remove(i, 1);
				
				// Plus at the zero index and digit
				} else if (i == 1 && Expression.expression[i - 1] == '+' && (Char.IsDigit(Expression.expression[i]) || Expression.expression[i] == '.')) {

					// Removing plus
					Expression.expression = Expression.expression.Remove(i - 1, 1);
				}
			}

			input = Expression.expression;
		}
		public static void SaveInput(InputType inputType) {

			// There is at least one input saved in history
			if (validInputHistory.Count > 0) {

				// New input is different than the last one
				if (input != validInputHistory.Last()) {

					validInputHistory.Add(input);
					activeHistoryCmd = -1;
				}

				// History is empty
			} else {

				validInputHistory.Add(input);
				activeHistoryCmd = -1;
			}
		}

		/* Private methods */
		private static Run[] IfEmptyInput(string input) {

			// Empty input
			if (input == String.Empty) {

				return new Run[] { new Run("I am not being printed out to console") };

			// Input not empty
			} else {

				return new Run[0];
			}
		}
		private static Run[] IfInputIsKnownPhrase(string _input) {

			// All known application phrases in one list
			var knownPhrases = new List<List<KnownPhrase>>() {

				Command.Commands.ConvertAll(x => (KnownPhrase)x),
				MathOperator.MathOperators.ConvertAll(x => (KnownPhrase)x),
				GroupingSymbol.GroupingSymbols.ConvertAll(x => (KnownPhrase)x),
				Delimiter.Delimiters.ConvertAll(x => (KnownPhrase)x),
				Character.Characters.ConvertAll(x => (KnownPhrase)x),
				ConstantValue.ConstantValues.ConvertAll(x => (KnownPhrase)x),
				Variable.Variables.ConvertAll(x => (KnownPhrase)x),
				KnownPhrase.knownPhrases
			};




			string result = String.Empty;
			KnownPhrase objFound = null;
			var message = new List<Run>();
			SolidColorBrush color = null;
			

			// If openining and closing brackets at the end of input string
			string bracketsInInput = String.Empty;
			if (_input.Length >= 2) {

				if ((_input[_input.Length - 2] == '(' && _input[_input.Length - 1] == ')') || (_input[_input.Length - 2] == '[' && _input[_input.Length - 1] == ']')) {

					// Saving and removing brackets signs
					bracketsInInput = _input[_input.Length - 2].ToString() + _input[_input.Length - 1].ToString();
					_input = _input.Remove(_input.Length - 2);
				}
			}



			// Going through all application math objects
			for (int i = 0; i < knownPhrases.Count; ++i) {
				for (int j = 0; j < knownPhrases[i].Count; ++j) {

					// System command
					if (knownPhrases[i][j] is Command) {

						Command obj = knownPhrases[i][j] as Command;
						if (obj.Cmd.ToString() == _input.ToUpper()) { objFound = obj; break; }

					// Constant value
					} else if (knownPhrases[i][j] is ConstantValue) {

						ConstantValue obj = knownPhrases[i][j] as ConstantValue;
						if (obj.Phrase.ToUpper() == _input.ToUpper()) { objFound = obj; break; }

					// Variable
					} else if (knownPhrases[i][j] is Variable) {

						Variable obj = knownPhrases[i][j] as Variable;
						if (obj.Phrase.ToLower() == _input.ToLower()) { objFound = obj; break; }

					// Arithmetic operator
					} else if (knownPhrases[i][j] is ArithmeticOperator) {

						ArithmeticOperator obj = knownPhrases[i][j] as ArithmeticOperator;
						if (obj.Phrase == _input) { objFound = obj; break; }

					// Relational operator
					} else if (knownPhrases[i][j] is RelationalOperator) {

						RelationalOperator obj = knownPhrases[i][j] as RelationalOperator;
						if (obj.Phrase == _input) { objFound = obj; break; }

					// Grouping symbol
					} else if (knownPhrases[i][j] is GroupingSymbol) {

						GroupingSymbol obj = knownPhrases[i][j] as GroupingSymbol;
						if (obj.Phrase.ToString() == _input) { objFound = obj; break; }

					// Delimiter
					} else if (knownPhrases[i][j] is Delimiter) {

						Delimiter obj = knownPhrases[i][j] as Delimiter;
						if (obj.Phrase == _input) { objFound = obj; break; }

					// Other character different than digit and letter
					} else if (knownPhrases[i][j] is Character) {

						Character obj = knownPhrases[i][j] as Character;

						if (!obj.Description.ToUpper().Contains("DIGIT") && !obj.Description.ToUpper().Contains("LETTER") && obj.Phrase == _input) {
							objFound = obj; break;
						}

					// Known phrase
					} else if (knownPhrases[i][j] is KnownPhrase) {

						KnownPhrase obj = knownPhrases[i][j] as KnownPhrase;
						if (obj.Phrase == _input.ToLower()) { objFound = obj; break; }
					}
				}

				if (objFound != null) { break; }
			}



			// If phrase found
			if (objFound != null) {

				if (objFound is Command) {

					message.Add(new Run(Character.Characters[0].Phrase));
					result = ((Command)objFound).Cmd.ToString();
					_input = Character.Characters[0].Phrase + _input.ToLower();
					color = Command.SystemCmdColor;

				} else if (objFound is ConstantValue) {

					_input = (objFound as ConstantValue).Phrase;
					result = (objFound as ConstantValue).Description;
					color = ConstantValue.ConstColor;

				} else if (objFound is Variable) {

					_input = (objFound as Variable).Phrase;
					result = "variable";
					color = ConstantValue.ConstColor;

				} else if (objFound is Function) {

					if (bracketsInInput != String.Empty) { _input = (objFound as Function).Phrase + bracketsInInput; } else { _input = (objFound as Function).Phrase + "()"; }
					result = (objFound as Function).Description;
					color = MathOperator.MathOperatorColor;

				} else if (objFound is ArithmeticOperator) { 
					
					result = (objFound as ArithmeticOperator).Description;
					color = MathOperator.MathOperatorColor;

				} else if (objFound is RelationalOperator) {

					result = (objFound as RelationalOperator).Description;
					color = MathOperator.MathOperatorColor;

				} else if (objFound is GroupingSymbol) {
					
					result = (objFound as GroupingSymbol).Description;
					color = GroupingSymbol.GroupingSymbolColor;

				} else if (objFound is Delimiter) {

					result = (objFound as Delimiter).Description;
					color = MathOperator.MathOperatorColor;

				} else if (objFound is Character) {

					result = (objFound as Character).Description;
					color = MathOperator.MathOperatorColor;

				} else if (objFound is KnownPhrase) {

					result = (objFound as KnownPhrase).Description;
					_input = objFound.Phrase;
					color = MathOperator.MathOperatorColor;
				}

				message.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.STANDARD].Text) { Foreground = RunOfText.EntryPrefixColor });
				message.Add(new Run("'") { Foreground = RunOfText.StandardTextColor });
				message.Add(new Run(_input) { Foreground = color });
				message.Add(new Run("'") { Foreground = RunOfText.StandardTextColor });
				message.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = RunOfText.StandardTextColor });
				message.Add(new Run(result + "\n") { Foreground = RunOfText.StandardTextColor });

				Input.input = _input;

				#region CONSOLE_PRINT_OUT

				if (MainWindow.consolePrintOutEnabled) {

					Console.Clear();
					Console.WriteLine("METHOD: 'IfInputIsKnownPhrase'\n");
					Console.WriteLine("KnownPhrase: " + result);
					Console.ReadKey();
				}

				#endregion

			// Phrase not found
			} else {

				#region CONSOLE_PRINT_OUT

				if (MainWindow.consolePrintOutEnabled) {

					Console.Clear();
					Console.WriteLine("METHOD: 'IfInputIsKnownPhrase'\n");
					Console.WriteLine("Input is not one of known phrases");
					Console.ReadKey();
				}

				#endregion
			}

			return message.ToArray();
		}
	}
}