using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Shell {
	
	public class Token {

		public enum TokenRole { NONE, GROUPING_SYMBOL, NUMERIC_VALUE, MATH_OPERATOR, CONST_VAR_NAME, VARIABLE_NAME, FUNCTION_NAME, NAME };

		public static List<Token> OutputTokens = new List<Token>();
		public static List<Token> WorkingTokens = new List<Token>();

		/* Fields */
		public TokenRole Type { get; set; }
		public string Text { get; set; }
		public int StartIndex { get; set; }

		/* Public methods */
		public Token(object[] args) {

			Type = (TokenRole)args[0];
			Text = (string)args[1];

			try { StartIndex = (int)args[2]; } catch { }
		}
		public Token(Token token) {

			Type = token.Type;
			Text = token.Text;
			StartIndex = token.StartIndex;
		}

		public static bool TryParsingNumericToken(int tokenIndex) {

			double tryParseResult = 0;



			// If value token
			if (OutputTokens[tokenIndex].Type == TokenRole.NUMERIC_VALUE) {

				// If parsing successful
				if (Double.TryParse(OutputTokens[tokenIndex].Text, out tryParseResult)) { OutputTokens[tokenIndex].Text = tryParseResult.ToString(); }

				// If error during parsing
				else {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_NUMERIC_VALUE, OutputTokens[tokenIndex].Text, OutputTokens[tokenIndex].StartIndex, OutputTokens[tokenIndex].Text.Length };
					return false;
				}

			// If true token
			} else if (OutputTokens[tokenIndex].Text == "true") {

				OutputTokens[tokenIndex].Text = "1";
				OutputTokens[tokenIndex].Type = TokenRole.NUMERIC_VALUE;

			// If false token
			} else if (OutputTokens[tokenIndex].Text == "false") {

				OutputTokens[tokenIndex].Text = "0";
				OutputTokens[tokenIndex].Type = TokenRole.NUMERIC_VALUE;
			}

			return true;
		}
		public static void AddZeroWorkingTokens() {

			// Adding zeros
			for (int i = WorkingTokens.Count - 1; i >= 1; --i) {

				// Minus or plus found right after left bracket 
				if ((WorkingTokens[i - 1].Text == "(" || WorkingTokens[i - 1].Text == "[") && (WorkingTokens[i].Text == "-" || WorkingTokens[i].Text == "+")) {

					WorkingTokens.Insert(i, new Token(new object[] { TokenRole.NUMERIC_VALUE, "0" }));
				}
			}

			if (WorkingTokens[0].Text == "-" || WorkingTokens[0].Text == "+") { WorkingTokens.Insert(0, new Token(new object[] { TokenRole.NUMERIC_VALUE, "0" })); }

			#region CONSOLE_PRINT_OUT
			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'AddZeroWorkingTokens'\n");
				Console.WriteLine("WorkingTokens:\n");
				foreach (var token in WorkingTokens) { Console.WriteLine("	'" + token.Text + "'	" + token.Type); }
				Console.ReadKey();
			}
			#endregion
		}
		public static bool ValidateWorkingTokens() {

			bool relOperAppeared = false;



			// Going through all tokens
			for (int i = 0; i < WorkingTokens.Count; ++i) {

				MathOperator operObj = null;
				ConstantValue constObj = null;
				Variable varObj = null;

				// Operator token encountered
				if (WorkingTokens[i].Type == TokenRole.MATH_OPERATOR) {

					operObj = MathOperator.MathOperators.Find(obj => obj.Phrase.ToLower() == WorkingTokens[i].Text);
					if (operObj != null && operObj.Role == MathOperator.MathOperatorRole.RELATIONAL) { relOperAppeared = true; }

					// Invalid token
					if (operObj == null) {

						Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_MATH_OPERATOR, WorkingTokens[i].Text, WorkingTokens[i].StartIndex, WorkingTokens[i].Text.Length };
						#region CONSOLE_PRINT_OUT

						if (MainWindow.consolePrintOutEnabled) {

							Console.Clear();
							Console.WriteLine("METHOD: 'ValidateWorkingTokens'\n");
							Console.WriteLine("Invalid token found: " + WorkingTokens[i].Text);
							Console.ReadKey();
						}

						#endregion
						return false;
					
					// Invalid use of '=' operator
					} else if (WorkingTokens[i].Text == "=" && (i != 1 || WorkingTokens[i - 1].Type != TokenRole.VARIABLE_NAME)) {

						Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_USE_OF_OPER, WorkingTokens[i].Text, WorkingTokens[i].StartIndex, WorkingTokens[i].Text.Length };
						#region CONSOLE_PRINT_OUT

						if (MainWindow.consolePrintOutEnabled) {

							Console.Clear();
							Console.WriteLine("METHOD: 'ValidateWorkingTokens'\n");
							Console.WriteLine("Invalid token found: " + WorkingTokens[i].Text);
							Console.ReadKey();
						}

						#endregion
						return false;
					}

				// Name token encountered
				} else if (WorkingTokens[i].Type == TokenRole.NAME) {

					// Trying to recognize token
					operObj = MathOperator.MathOperators.Find(obj => obj.Phrase.ToLower() == WorkingTokens[i].Text);
					if (operObj == null) {
						constObj = ConstantValue.ConstantValues.Find(obj => obj.Phrase.ToLower() == WorkingTokens[i].Text.ToLower());
						if (constObj == null) { varObj = Variable.Variables.Find(obj => obj.Phrase.ToLower() == WorkingTokens[i].Text); }
					}
					


					// Unknown token
					if (operObj == null && constObj == null && varObj == null) {

						// If iterated token is one letter token and there is '=' sign right after it
						if (i < WorkingTokens.Count - 1 && WorkingTokens[i].Text.Length == 1 && Char.IsLetter(WorkingTokens[i].Text[0]) && WorkingTokens[i + 1].Text == "=") {

							OutputTokens[i].Type = WorkingTokens[i].Type = TokenRole.VARIABLE_NAME;

						// Invalid token
						} else {

							Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_EXPRESSION, WorkingTokens[i].Text, WorkingTokens[i].StartIndex, WorkingTokens[i].Text.Length };
							#region CONSOLE_PRINT_OUT

							if (MainWindow.consolePrintOutEnabled) {

								Console.Clear();
								Console.WriteLine("METHOD: 'ValidateWorkingTokens'\n");
								Console.WriteLine("Invalid token found: " + WorkingTokens[i].Text);
								Console.ReadKey();
							}

							#endregion
							return false;
						}

					// Valid math operator token found
					} else if (operObj != null) { 
						
						OutputTokens[i].Type = WorkingTokens[i].Type = TokenRole.FUNCTION_NAME;

					// Valid constant value token found
					} else if (constObj != null) {

						// If trying to use constant value name as a variable
						if (i < WorkingTokens.Count - 1 && WorkingTokens[i + 1].Text == "=") {

							// Updating textbox input - making constant uppercase
							OutputTokens[i].Text = OutputTokens[i].Text.ToUpper();
							Expression.expression = String.Empty;
							foreach (var token in OutputTokens) { Expression.expression += token.Text; }
							Input.input = Expression.expression;

							Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_VAR_NAME, WorkingTokens[i].Text, WorkingTokens[i].StartIndex, WorkingTokens[i].Text.Length };
							#region CONSOLE_PRINT_OUT

							if (MainWindow.consolePrintOutEnabled) {

								Console.Clear();
								Console.WriteLine("METHOD: 'ValidateWorkingTokens'\n");
								Console.WriteLine("Invalid token found: " + WorkingTokens[i].Text);
								Console.ReadKey();
							}

							#endregion
							return false;

						} else {

							OutputTokens[i].Type = WorkingTokens[i].Type = TokenRole.CONST_VAR_NAME;
							WorkingTokens[i].Text = constObj.Value.ToString();
						}
					
					// Valid variable token found
					} else if (varObj != null) {

						// If it is just changing value of already existing variable
						if (i == 0 && i < WorkingTokens.Count - 1 && WorkingTokens[i + 1].Text == "=") {

							OutputTokens[i].Type = WorkingTokens[i].Type = TokenRole.VARIABLE_NAME;

						} else {

							OutputTokens[i].Type = WorkingTokens[i].Type = TokenRole.VARIABLE_NAME;
							WorkingTokens[i].Text = varObj.Value.ToString();
						}
					}
				}
			}



			// If there was relational operator found and no brackets around expression
			if (relOperAppeared && (OutputTokens.First().Type != TokenRole.GROUPING_SYMBOL || OutputTokens.Last().Type != TokenRole.GROUPING_SYMBOL)) {

				OutputTokens.Insert(0, new Token(new object[] { Token.TokenRole.GROUPING_SYMBOL, "(", 0 }));
				WorkingTokens.Insert(0, new Token(new object[] { Token.TokenRole.GROUPING_SYMBOL, "(", 0 }));

				for (int i = 1; i < OutputTokens.Count; ++i) {

					OutputTokens[i].StartIndex += 1;
					WorkingTokens[i].StartIndex += 1;
				}

				OutputTokens.Add(new Token(new object[] { Token.TokenRole.GROUPING_SYMBOL, ")", OutputTokens.Last().StartIndex + OutputTokens.Last().Text.Length }));
				WorkingTokens.Add(new Token(new object[] { Token.TokenRole.GROUPING_SYMBOL, ")", WorkingTokens.Last().StartIndex + WorkingTokens.Last().Text.Length }));
			}

			#region CONSOLE_PRINT_OUT
			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'ValidateWorkingTokens'\n");
				Console.WriteLine("CurrentExpression: " + Expression.expression + "\n");
				Console.WriteLine("WorkingTokens: \n");
				foreach (var token in WorkingTokens) { Console.WriteLine("	'" + token.Text + "'	" + token.Type); }
				Console.ReadKey();
			}
			#endregion
			return true;
		}
		public static void FormatOutputTokens() {

			/* Adding spaces to make sure the output looks better, making sure constant tokens text is uppercase, adding colors */



			// Updating CurrentExpression
			Expression.expression = String.Empty;
			for (int i = 0; i < OutputTokens.Count; ++i) {

				// If token is any math operator
				if (OutputTokens[i].Type == TokenRole.MATH_OPERATOR) {

					MathOperator oper = MathOperator.MathOperators.Find(obj => obj.Phrase == OutputTokens[i].Text);

					// Valid operator
					if (oper != null) {

						// Token is minus or plus right after opening bracket
						if (i > 0 && (OutputTokens[i].Text == "-" || OutputTokens[i].Text == "+") && (OutputTokens[i - 1].Text == "(" || OutputTokens[i - 1].Text == "[")) {

							// No spaces
							Expression.expression += OutputTokens[i].Text;

						// Token is minus or plus right after function argument delimiter ','
						} else if (i > 0 && (OutputTokens[i].Text == "-" || OutputTokens[i].Text == "+") && OutputTokens[i - 1].Text == ", ") {

							// No spaces
							Expression.expression += OutputTokens[i].Text;

						// Token is minus or plus at the zero index
						} else if (i == 0 && (OutputTokens[i].Text == "-" || OutputTokens[i].Text == "+")) {

							// No spaces
							Expression.expression += OutputTokens[i].Text;

						// Token is multiplication sign after numeric value and before const / variable
						} else if (OutputTokens[i].Text == "*" && i > 0 && i < OutputTokens.Count - 1 && OutputTokens[i - 1].Type == TokenRole.NUMERIC_VALUE && (OutputTokens[i + 1].Type == TokenRole.CONST_VAR_NAME || OutputTokens[i + 1].Type == TokenRole.VARIABLE_NAME)) {

							// Removing asteriks
							OutputTokens[i].Text = String.Empty;
							Expression.expression += OutputTokens[i].Text;

						} else {

							// If operator is only one argument
							if (oper.OneArgument == true && oper.TwoArgument == false) {

								// No spaces
								Expression.expression += OutputTokens[i].Text;

							} else {

								// Adding spaces before and after
								OutputTokens[i].Text = (" " + OutputTokens[i].Text + " ");
								Expression.expression += OutputTokens[i].Text;
							}
						}

					// Not valid
					} else {

						// No spaces
						Expression.expression += OutputTokens[i].Text;
					}

					// Color
					Expression.ExpressionInColor.Add(new Run(OutputTokens[i].Text) { Foreground = MathOperator.MathOperatorColor });

				// If token is a function operator
				} else if (OutputTokens[i].Type == TokenRole.FUNCTION_NAME) {

					// No spaces
					Expression.expression += OutputTokens[i].Text;

					// Color
					Expression.ExpressionInColor.Add(new Run(OutputTokens[i].Text) { Foreground = Function.FunctionColor });

				// If token is a constant value
				} else if (OutputTokens[i].Type == TokenRole.CONST_VAR_NAME) {

					// Changing phrase to uppercase
					OutputTokens[i].Text = OutputTokens[i].Text.ToUpper();
					Expression.expression += OutputTokens[i].Text;

					// Color
					Expression.ExpressionInColor.Add(new Run(OutputTokens[i].Text) { Foreground = ConstantValue.ConstColor });

				// Grouping symbol
				} else if (OutputTokens[i].Type == TokenRole.GROUPING_SYMBOL) {

					// If token is ',' delimiter
					if (OutputTokens[i].Text == ",") {

						// Adding space after
						OutputTokens[i].Text = (OutputTokens[i].Text + " ");
						Expression.expression += OutputTokens[i].Text;

						// Color
						Expression.ExpressionInColor.Add(new Run(OutputTokens[i].Text) { Foreground = Command.SystemCmdColor });

					} else {

						// No spaces
						Expression.expression += OutputTokens[i].Text;

						// Color
						Expression.ExpressionInColor.Add(new Run(OutputTokens[i].Text) { Foreground = GroupingSymbol.GroupingSymbolColor });
					}

				} else {

					// No spaces
					Expression.expression += OutputTokens[i].Text;
			
					// Color
					Expression.ExpressionInColor.Add(new Run(OutputTokens[i].Text) { Foreground = RunOfText.StandardTextColor });
				}
			}

			Input.input = Expression.expression;
			#region CONSOLE_PRINT_OUT
			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'FormatOutputTokens'\n");
				Console.WriteLine("CurrentExpression: " + Expression.expression + "\n");
				Console.WriteLine("OutputTokens: \n");
				foreach (var token in OutputTokens) { Console.WriteLine("	'" + token.Text + "'	" + token.Type); }
				Console.ReadKey();
			}
			#endregion
		}

		public static TokenRole CharTypeToTokenRole(Character.CharacterType charType) {

			switch (charType) {

				case Character.CharacterType.NUMERIC:
					return TokenRole.NUMERIC_VALUE;

				case Character.CharacterType.ARITHMETIC:
					return TokenRole.MATH_OPERATOR;

				case Character.CharacterType.GROUPING:
					return TokenRole.GROUPING_SYMBOL;

				case Character.CharacterType.ALPHABETIC:
					return TokenRole.NAME;

				default:
					return TokenRole.NONE;
			}
		}
	}
}