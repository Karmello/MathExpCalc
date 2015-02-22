using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Shell {

	static class Expression {

		/* Variables */
		public static string expression = String.Empty;
		public static List<Run> ExpressionInColor = new List<Run>();
		public static List<Character.CharacterType> TypeOfCharacters = new List<Character.CharacterType>();
		
		public static bool isBoolean;
		public static object finalResult = 0;

		/* Public methods */
		public static Tuple<Run[], Output.OutputRole> CalculateExpression(string input) {

			Token.OutputTokens.Clear();
			Token.WorkingTokens.Clear();
			expression = String.Empty;
			ExpressionInColor.Clear();

			expression = input;
			isBoolean = false;



			Input.FixInput();

			// All characters known
			if (DetermineTypeOfCharacters()) {

				// Tokenizing successful
				if (TokenizeIntoOutputTokens()) {

					AfterTokenizeAsteriksFix();
					UpdateInputAndExpressionStringsWithOutputTokens();
					
					// Making a deep copy of all the tokens
					foreach (var token in Token.OutputTokens) { Token.WorkingTokens.Add(new Token(token)); }

					// All token valid
					if (Token.ValidateWorkingTokens()) {

						Token.AddZeroWorkingTokens();

						// If there was some illegal action during calculating expression
						if (!LookForBrackets()) {
							return new Tuple<Run[], Output.OutputRole>(Warning.CreateDynamicWarningMessage((Warning.WarningType)Input.exceptionInfo[0], null), Output.OutputRole.WARNING);
						}

					// Invalid operator token found
					} else { return new Tuple<Run[], Output.OutputRole>(Warning.CreateDynamicWarningMessage((Warning.WarningType)Input.exceptionInfo[0], null), Output.OutputRole.WARNING); }
				
				// Error during tokenizing
				} else { return new Tuple<Run[], Output.OutputRole>(Warning.CreateDynamicWarningMessage((Warning.WarningType)Input.exceptionInfo[0], null), Output.OutputRole.WARNING); }

			// Unknown character found
			} else { return new Tuple<Run[], Output.OutputRole>(Warning.CreateDynamicWarningMessage((Warning.WarningType)Input.exceptionInfo[0], null), Output.OutputRole.WARNING); }



			// Adding spaces to output expression and final result check
			Token.FormatOutputTokens();
			ConvertFinalResultIfNecessary();
			


			// Final values
			var finalMessageList = new List<Run>();

			finalMessageList.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.STANDARD].Text) { Foreground = RunOfText.EntryPrefixColor });
			foreach (var run in ExpressionInColor) { finalMessageList.Add(new Run(run.Text) { Foreground = run.Foreground }); }
			
			// Regular calculation
			if (!ExpressionInColor.Any(obj => obj.Text == " = ")) {
				
				finalMessageList.Add(new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.RESULT].Text) { Foreground = MathOperator.MathOperatorColor });
				finalMessageList.Add(new Run(finalResult.ToString()) { Foreground = RunOfText.StandardTextColor });
				finalMessageList.Add(new Run("\n"));

				Run[] finalMessage = finalMessageList.ToArray();
				return new Tuple<Run[], Output.OutputRole>(finalMessage, Output.OutputRole.CALCULATION);

			// Creating new or changing existing variable
			} else {

				finalMessageList.Add(new Run("\n"));
				Run[] finalMessage = finalMessageList.ToArray();
				return new Tuple<Run[], Output.OutputRole>(finalMessage, Output.OutputRole.VAR_DEFINITION);
			}
		}
		public static Run[] MathExpressionValidation(string input) {

			GroupingSymbol iteratedBracketObj = null;
			Character iteratedCharObj = null;
			Character previousCharObj = null;



			GroupingSymbol.brackets.Clear();
			GroupingSymbol.FindNotClosedBracketsIndexes(ref GroupingSymbol.notClosedBracketsIndexes, input);

			for (int i = 0; i < input.Length; ++i) {

				

					// At all indexes
					#region GETTING_CHARACTER_OBJECTS

					iteratedBracketObj = GroupingSymbol.GroupingSymbols.Find(obj => (obj.Phrase[0] == input[i]));
					iteratedCharObj = Character.Characters.Find(obj => (obj.Phrase[0] == input[i]));
					if (i > 0) { previousCharObj = Character.Characters.Find(obj => (obj.Phrase[0] == input[i - 1])); }

					#endregion

					// Only at the first index
					#region FIRST_CHARACTER_VALIDATION

					if (iteratedCharObj != null && i == 0) {

						if (iteratedCharObj.FirstIndexAllowed == false) {
							return Warning.CreateDynamicWarningMessage(Warning.WarningType.INVALID_EDGE_CHAR, new int[] { i }, Character.Edge.LEFT);
						}
					}

					#endregion

					// At all indexes
					#region BRACKET_VALIDATION

					// If iterated char is any bracket it checks if it is allowed to be at its position according to the last bracket
					if (GroupingSymbol.GroupingSymbols.Any(obj => (obj.Phrase[0] == input[i]))) {

						Run[] validationResult = GroupingSymbol.BracketValidation(iteratedBracketObj, i);
						if (validationResult.Length > 0) { return validationResult; }
					}

					#endregion
					#region RIGHT_AFTERS_VALIDATION

					if (i > 0 && iteratedCharObj != null && previousCharObj != null && previousCharObj.WrongRightAfters != null) {

						// Iterated object's symbol should not be after its predecessor's symbol
						if (previousCharObj.WrongRightAfters.Contains(iteratedCharObj.Phrase[0])) {

							return Warning.CreateDynamicWarningMessage(Warning.WarningType.INVALID_FOLLOWING_CHAR, new int[] { i }, Character.Edge.NONE);
						}
					}

					#endregion

					// Only at the last index
					#region LAST_CHARACTER_VALIDATION

					if (iteratedCharObj != null && i == input.Length - 1) {

						if (iteratedCharObj.LastIndexAllowed == false) {
							return Warning.CreateDynamicWarningMessage(Warning.WarningType.INVALID_EDGE_CHAR, new int[] { i }, Character.Edge.RIGHT);
						}
					}

					#endregion

				
			}

			// Succeeded
			return new Run[0];
		}

		/* Private methods */
		private static bool TokenizeIntoOutputTokens() {

			/* This function groups same type chars into tokens, sets token type, parses numeric tokens and sets token start index, also adds asteriks */

			Character.CharacterType newTokenCharType = TypeOfCharacters[0];
			Token.TokenRole newTokenRole = Token.TokenRole.NONE;
			StringBuilder newTokenText = new StringBuilder();
			int newTokenStartIndex = 0;



			newTokenText.Append(expression[0]);

			// Going through all current expression characters starting from index 1
			for (int i = 1; i < expression.Length; ++i) {

				// When encountered character has different type than the previous one has or it is a grouping symbol
				if (TypeOfCharacters[i] != newTokenCharType || TypeOfCharacters[i] == Character.CharacterType.GROUPING) {

					// Updating startIndex
					if (Token.OutputTokens.Count > 0) { newTokenStartIndex += Token.OutputTokens.Last().Text.Length; }

					// Creating new token
					Token.OutputTokens.Add(new Token(new object[] { newTokenRole = Token.CharTypeToTokenRole(newTokenCharType), newTokenText.ToString(), newTokenStartIndex }));
					if (!Token.TryParsingNumericToken(Token.OutputTokens.Count - 1)) { return false; }
					Token.OutputTokens.Last().StartIndex = newTokenStartIndex;

					// Updating variables
					newTokenCharType = TypeOfCharacters[i];
					newTokenText.Clear();
				}

				newTokenText.Append(expression[i]);
			}



			// Creating last token
			if (Token.OutputTokens.Count > 0) { newTokenStartIndex += Token.OutputTokens.Last().Text.Length; }
			Token.OutputTokens.Add(new Token(new object[] { newTokenRole = Token.CharTypeToTokenRole(newTokenCharType), newTokenText.ToString(), newTokenStartIndex }));
			if (!Token.TryParsingNumericToken(Token.OutputTokens.Count - 1)) { return false; }
			Token.OutputTokens.Last().StartIndex = newTokenStartIndex;

			

			#region CONSOLE_PRINT_OUT
			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'TokenizeIntoOutputTokens'\n");
				Console.WriteLine("OutputTokens:\n");
				foreach (var token in Token.OutputTokens) { Console.WriteLine("	'" + token.Text + "'	" + token.StartIndex + "	" + token.Type); }
				Console.ReadKey();
			}
			#endregion

			return true;
		}
		private static void AfterTokenizeAsteriksFix() {

			bool asteriksAdded = false;

	
			
			for (int i = Token.OutputTokens.Count - 1; i >= 1; --i) {

				// Closing bracket and constant value / variable
				if ((Token.OutputTokens[i - 1].Text == ")" || Token.OutputTokens[i - 1].Text == "]") && Token.OutputTokens[i].Type == Token.TokenRole.NAME && (ConstantValue.ConstantValues.Any(c => c.Phrase.ToLower() == Token.OutputTokens[i].Text.ToLower()) || Variable.Variables.Any(v => v.Phrase.ToLower() == Token.OutputTokens[i].Text.ToLower()))) {

					// Adding asteriks
					Token.OutputTokens.Insert(i, new Token(new object[] { Token.TokenRole.MATH_OPERATOR, "*", Token.OutputTokens[i].StartIndex }));
					asteriksAdded = true;

				// Constant / variable and opening bracket
				} else if (Token.OutputTokens[i - 1].Type == Token.TokenRole.NAME && (ConstantValue.ConstantValues.Any(c => c.Phrase.ToLower() == Token.OutputTokens[i - 1].Text.ToLower()) || Variable.Variables.Any(v => v.Phrase.ToLower() == Token.OutputTokens[i - 1].Text.ToLower())) && (Token.OutputTokens[i].Text == "(" || Token.OutputTokens[i].Text == "[")) {

					Token.OutputTokens.Insert(i, new Token(new object[] { Token.TokenRole.MATH_OPERATOR, "*", Token.OutputTokens[i].StartIndex }));
					asteriksAdded = true;

				// Closing bracket and function
				} else if ((Token.OutputTokens[i - 1].Text == ")" || Token.OutputTokens[i - 1].Text == "]") && Token.OutputTokens[i].Type == Token.TokenRole.NAME && MathOperator.MathOperators.Any(obj => (obj.Phrase.ToLower() == Token.OutputTokens[i].Text.ToLower() && obj is Function))) {

					Token.OutputTokens.Insert(i, new Token(new object[] { Token.TokenRole.MATH_OPERATOR, "*", Token.OutputTokens[i].StartIndex }));
					asteriksAdded = true;

				// Numeric value and function
				} else if (Token.OutputTokens[i - 1].Type == Token.TokenRole.NUMERIC_VALUE && Token.OutputTokens[i].Type == Token.TokenRole.NAME && MathOperator.MathOperators.Any(obj => (obj.Phrase.ToLower() == Token.OutputTokens[i].Text.ToLower() && obj is Function))) {

					Token.OutputTokens.Insert(i, new Token(new object[] { Token.TokenRole.MATH_OPERATOR, "*", Token.OutputTokens[i].StartIndex }));
					asteriksAdded = true;
				
				// Numeric value and constant / variable
				} else if (Token.OutputTokens[i - 1].Type == Token.TokenRole.NUMERIC_VALUE && Token.OutputTokens[i].Type == Token.TokenRole.NAME && (ConstantValue.ConstantValues.Any(c => c.Phrase.ToLower() == Token.OutputTokens[i].Text.ToLower()) || Variable.Variables.Any(v => v.Phrase.ToLower() == Token.OutputTokens[i].Text.ToLower()))) {

					Token.OutputTokens.Insert(i, new Token(new object[] { Token.TokenRole.MATH_OPERATOR, "*", Token.OutputTokens[i].StartIndex }));
					asteriksAdded = true;

				// Constant / variable and numeric value
				} else if (Token.OutputTokens[i - 1].Type == Token.TokenRole.NAME && (ConstantValue.ConstantValues.Any(c => c.Phrase.ToLower() == Token.OutputTokens[i - 1].Text.ToLower()) || Variable.Variables.Any(v => v.Phrase.ToLower() == Token.OutputTokens[i - 1].Text.ToLower())) && Token.OutputTokens[i].Type == Token.TokenRole.NUMERIC_VALUE) {

					Token.OutputTokens.Insert(i, new Token(new object[] { Token.TokenRole.MATH_OPERATOR, "*", Token.OutputTokens[i].StartIndex }));
					asteriksAdded = true;
				}



				// Updating indexes of the following tokens
				if (asteriksAdded) { for (int j = i + 1; j < Token.OutputTokens.Count; ++j) { Token.OutputTokens[j].StartIndex += 1; } asteriksAdded = false; }
			}
		}
		private static bool LookForBrackets() {

			var setOfTokens = new List<Token>();
			var rightBracketTokensIndexes = new List<int>();



			// Going through all tokens
			for (int i = Token.WorkingTokens.Count - 1; i >= 0; --i) {

				#region CONSOLE_PRINT_OUT

				if (MainWindow.consolePrintOutEnabled) {

					Console.Clear();
					Console.WriteLine("METHOD: 'LookForBrackets'\n");
					Console.Write("WorkingTokens: ");
					foreach (var token in Token.WorkingTokens) { Console.Write(token.Text + " "); }
					Console.WriteLine("\n\nIterated token: " + Token.WorkingTokens[i].Text);
					Console.ReadKey();

				}

				#endregion

				// Grouping token
				if (Token.WorkingTokens[i].Type == Token.TokenRole.GROUPING_SYMBOL) {

					switch (Token.WorkingTokens[i].Text) {

						// Right
						case ")":
						case "]":

							// Saving opening bracket index
							rightBracketTokensIndexes.Add(i);
							break;

						// Left
						case "(":
						case "[":

							int startIndex = -1;
							int endIndex = -1;

							// Found pair of brackets belongs to a function
							if (IfFunctionBrackets(i)) { startIndex = i - 1;
							} else { startIndex = i; }

							// There is one argument and not two argument operator after right bracket
							if (IfOneArgOperAfterRightBracket(rightBracketTokensIndexes.Last())) { endIndex = rightBracketTokensIndexes.Last() + 1;
							} else { endIndex = rightBracketTokensIndexes.Last(); }

							// Removing right bracket index
							rightBracketTokensIndexes.RemoveAt(rightBracketTokensIndexes.Count - 1);



							// Getting all tokens from opening until closing bracket token including bracket tokens
							setOfTokens = Token.WorkingTokens.GetRange(startIndex, endIndex - startIndex + 1);
							
							// Deleting same set of tokens from original list
							Token.WorkingTokens.RemoveRange(startIndex, endIndex - startIndex + 1);

							// Updating rightBracketTokensIndexes
							for (int j = 0; j < rightBracketTokensIndexes.Count; ++j) { rightBracketTokensIndexes[j] -= (endIndex - startIndex); }



							// Interpreting set of tokens
							Token.WorkingTokens.Insert(startIndex, InterpretSetOfTokens(true, setOfTokens));

							// End of this iteration
							if (Token.WorkingTokens[startIndex].Text == String.Empty) { return false; }
							else if (Token.WorkingTokens.Count == 1) { finalResult = Double.Parse(Token.WorkingTokens[0].Text); }

							break;

						default:
							break;
					}
				}
			}

			// Checking the none bracket expression

			Token result = InterpretSetOfTokens(false, Token.WorkingTokens);
			if (result.Text != String.Empty) { finalResult = Double.Parse(result.Text); } else { return false; }

			return true;
		}
		private static void UpdateInputAndExpressionStringsWithOutputTokens() {

			expression = String.Empty;
			foreach (var token in Token.OutputTokens) { expression += token.Text; }
			Input.input = expression;
		}
		
		private static Token InterpretSetOfTokens(bool fromWithinBrackets, List<Token> setOfTokens) {

			string result = String.Empty;
			string arg3 = "0";
			var funcArgDelimiterIndexes = new List<int>();

			MathOperator beforeBracketsFuncOper = null;
			MathOperator afterBracketsMathOper = null;
			Token beforeBracketsToken = null;
			Token afterBracketsToken = null;



			// If set of tokens comes from within the brackets
			if (fromWithinBrackets) {

				// If first token is a function operator
				if (setOfTokens.First().Type == Token.TokenRole.FUNCTION_NAME) {

					beforeBracketsFuncOper = MathOperator.MathOperators.Find(obj => obj.Phrase == setOfTokens.First().Text);
					beforeBracketsToken = setOfTokens.First();
					setOfTokens.RemoveAt(0);
				}

				// If last token is math operator
				if (setOfTokens.Last().Type == Token.TokenRole.MATH_OPERATOR) {

					afterBracketsMathOper = MathOperator.MathOperators.Find(obj => obj.Phrase == setOfTokens.Last().Text);
					afterBracketsToken = setOfTokens.Last();
					setOfTokens.RemoveAt(setOfTokens.Count - 1);
				}

				// Removing brackets
				setOfTokens.RemoveAt(0);
				setOfTokens.RemoveAt(setOfTokens.Count - 1);
			}

			#region CONSOLE_PRINT_OUT

			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'InterpretSetOfTokens'\n");

				Console.WriteLine("fromWithinBrackets: " + fromWithinBrackets);
				if (beforeBracketsFuncOper != null) { Console.WriteLine("beforeBracketsFuncOper: " + beforeBracketsFuncOper.Phrase); }
				if (afterBracketsMathOper != null) { Console.WriteLine("afterBracketsMathOper: " + afterBracketsMathOper.Phrase); }
				Console.WriteLine("NumOfTokensToInterpret: " + setOfTokens.Count);
				Console.WriteLine("\nTokens to interprete:\n");

				foreach (var token in setOfTokens) {
					Console.Write(token.Text + " ");
				}

				Console.ReadKey();
			}

			#endregion



			// Finding function argument delimiters
			for (int i = 0; i < setOfTokens.Count; ++i) { if (setOfTokens[i].Text == ",") { funcArgDelimiterIndexes.Add(i); } }

			// Inspecting set of tokens, if false returned
			if (!InspectSetOfTokens(setOfTokens, funcArgDelimiterIndexes, fromWithinBrackets, beforeBracketsFuncOper, beforeBracketsToken)) {
				return new Token(new object[] { Token.TokenRole.NONE, result });
			}



			// Interpreting
	
			// No brackets or brackets present but no function delimiter
			if (!fromWithinBrackets || funcArgDelimiterIndexes.Count == 0) {

				var operIndexes = MathOperator.DetermineOperatorsPresedence(setOfTokens);

				// Choosing scenario according to number of operators found
				if (operIndexes.Count >= 1) { result = PerformMathOperations(operIndexes, setOfTokens); } else { result = setOfTokens[0].Text; }

			// Brackets and one function delimiter present
			} else {

				// Dividing set of tokens
				List<Token> leftSideSetOfTokens = setOfTokens.GetRange(0, funcArgDelimiterIndexes[0]);
				List<Token> rightSideSetOfTokens = setOfTokens.GetRange(funcArgDelimiterIndexes[0] + 1, setOfTokens.Count - funcArgDelimiterIndexes[0] - 1);

				// Adding zero token at zero index if necessary
				if (leftSideSetOfTokens.First().Text == "-" || leftSideSetOfTokens.First().Text == "+") { leftSideSetOfTokens.Insert(0, new Token(new object[] { Token.TokenRole.NUMERIC_VALUE, "0" })); }
				if (rightSideSetOfTokens.First().Text == "-" || rightSideSetOfTokens.First().Text == "+") { rightSideSetOfTokens.Insert(0, new Token(new object[] { Token.TokenRole.NUMERIC_VALUE, "0" })); }



				// Choosing scenario according to number of tokens in each set of tokens
				if (leftSideSetOfTokens.Count > 1) {

					List<int> leftSideOrderOfOperations = MathOperator.DetermineOperatorsPresedence(leftSideSetOfTokens);
					result = PerformMathOperations(leftSideOrderOfOperations, leftSideSetOfTokens);

				} else if (leftSideSetOfTokens.Count == 1) { result = leftSideSetOfTokens[0].Text; } 

				if (rightSideSetOfTokens.Count > 1) {

					List<int> rightSideOrderOfOperations = MathOperator.DetermineOperatorsPresedence(rightSideSetOfTokens);
					arg3 = PerformMathOperations(rightSideOrderOfOperations, rightSideSetOfTokens);

				} else if (rightSideSetOfTokens.Count == 1) { arg3 = rightSideSetOfTokens[0].Text; }
			}



			// Checking for errors
			if (result == String.Empty || arg3 == String.Empty) { return new Token(new object[] { Token.TokenRole.NONE, String.Empty }); }



			// Applying before and after brackets operators
			if (beforeBracketsFuncOper != null) { result = Calculator.MakeCalculation(result, beforeBracketsToken, arg3); }
			if (afterBracketsMathOper != null) { result = Calculator.MakeCalculation(result, afterBracketsToken, arg3); }

			#region CONSOLE_PRINT_OUT

			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'InterpretSetOfTokens'\n");
				Console.WriteLine("Returning result: " + result);
				Console.ReadKey();
			}

			#endregion

			return new Token(new object[] { Token.TokenRole.NUMERIC_VALUE, result });
		}
		private static bool InspectSetOfTokens(List<Token> setOfTokens, List<int> funcArgDelimiterIndexes, bool fromWithinBrackets, MathOperator beforeBracketsFuncOper, Token beforeBracketsToken) {

			// At least one function args delimiter found
			if (funcArgDelimiterIndexes.Count > 0) {

				// Not from within brackets
				if (!fromWithinBrackets) {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_EXPRESSION, setOfTokens[funcArgDelimiterIndexes[0]].Text, setOfTokens[funcArgDelimiterIndexes[0]].StartIndex, setOfTokens[funcArgDelimiterIndexes[0]].Text.Length };
					return false; 
				
				// There was no function operator before brackets
				} else if (beforeBracketsFuncOper == null) {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_EXPRESSION, setOfTokens[funcArgDelimiterIndexes[0]].Text, setOfTokens[funcArgDelimiterIndexes[0]].StartIndex, setOfTokens[funcArgDelimiterIndexes[0]].Text.Length };
					return false;

				// Too many delimiters
				} else if (funcArgDelimiterIndexes.Count > 1) {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FUNCTION_CALL, beforeBracketsFuncOper.Phrase, beforeBracketsToken.StartIndex, beforeBracketsToken.Text.Length };
					return false;
				
				// Exactly one delimiter found but function is one-argument
				} else if (beforeBracketsFuncOper.OneArgument == true && beforeBracketsFuncOper.TwoArgument == false) {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FUNCTION_CALL, beforeBracketsFuncOper.Phrase, beforeBracketsToken.StartIndex, beforeBracketsToken.Text.Length };
					return false;
				}
			
			// No function args delimiters found
			} else if (funcArgDelimiterIndexes.Count == 0) {

				// There were brackets, there was function operator before brackets but it was only two-agrs operator
				if (fromWithinBrackets && beforeBracketsFuncOper != null && beforeBracketsFuncOper.OneArgument == false && beforeBracketsFuncOper.TwoArgument == true) {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FUNCTION_CALL, beforeBracketsFuncOper.Phrase, beforeBracketsToken.StartIndex, beforeBracketsToken.Text.Length };
					return false;
				
				// No brackets
				} else if (!fromWithinBrackets) {

					Token funcToken = null;
					funcToken = setOfTokens.Find(token => token.Type == Token.TokenRole.FUNCTION_NAME);

					// Function operator token found
					if (funcToken != null) {

						Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FUNCTION_CALL, funcToken.Text, funcToken.StartIndex, funcToken.Text.Length };
						return false;
					}
				}
			}

			return true;
		}
		private static string PerformMathOperations(List<int> orderOfOperations, List<Token> setOfTokens) {

			string result = String.Empty;



			// Going through all found and sorted operators
			for (int i = 0; i < orderOfOperations.Count; ++i) {

				MathOperator oper = MathOperator.MathOperators.Find(obj => obj.Phrase == setOfTokens[orderOfOperations[i]].Text);

				// If two-argument operator
				if (oper.TwoArgument == true) {

					// Making calculation
					result = Calculator.MakeCalculation(setOfTokens[orderOfOperations[i] - 1].Text.ToString(), setOfTokens[orderOfOperations[i]], setOfTokens[orderOfOperations[i] + 1].Text.ToString());
					if (result == String.Empty) { return result; }

					// Removing num1, operation and num2 tokens
					setOfTokens.RemoveAt(orderOfOperations[i] - 1);
					setOfTokens.RemoveAt(orderOfOperations[i] - 1);
					setOfTokens.RemoveAt(orderOfOperations[i] - 1);

				// If only one-argument operator
				} else if (oper.TwoArgument == false && oper.OneArgument == true) {

					// Making calculation
					result = Calculator.MakeCalculation(setOfTokens[orderOfOperations[i] - 1].Text.ToString(), setOfTokens[orderOfOperations[i]], "0");
					if (result == String.Empty) { return result; }

					// Removing num1, operation
					setOfTokens.RemoveAt(orderOfOperations[i] - 1);
					setOfTokens.RemoveAt(orderOfOperations[i] - 1);
				}



				// Inserting result token
				setOfTokens.Insert(orderOfOperations[i] - 1, new Token(new object[] { Token.TokenRole.NUMERIC_VALUE, result.ToString() }));

				// Updating arithmetic operators indexes
				for (int k = 0; k < orderOfOperations.Count; ++k) {

					// If operator index is bigger than the actual
					if (orderOfOperations[k] > orderOfOperations[i]) { orderOfOperations[k] = orderOfOperations[k] - 2; }
				}
			}

			return setOfTokens[0].Text;
		}

		private static bool DetermineTypeOfCharacters() {

			Character charObj = null;
			TypeOfCharacters.Clear();



			// Going through all current expression characters
			for (int i = 0; i < expression.Length; ++i) {

				// Getting character object
				charObj = Character.Characters.Find(obj => (obj.Phrase[0] == expression[i]));

				// Character valid
				if (charObj != null) {

					TypeOfCharacters.Add(charObj.CharType);

				// Character not valid
				} else if (charObj == null) {

					TypeOfCharacters.Add(Character.CharacterType.ALPHABETIC);
				}
			}

			#region CONSOLE_PRINT_OUT
			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'DetermineTypeOfCharacters'\n");
				Console.WriteLine("CurrentExpression: " + expression + "\n");

				for (int i = 0; i < TypeOfCharacters.Count; ++i) {

					Console.WriteLine("	" + expression[i] + "	" + TypeOfCharacters[i]);
				}

				Console.ReadKey();
			}
			#endregion

			return true;
		}

		private static bool IfFunctionBrackets(int leftBracketTokenIndex) {

			// If zero index or no function token before left bracket token
			if (leftBracketTokenIndex == 0 || Token.WorkingTokens[leftBracketTokenIndex - 1].Type != Token.TokenRole.FUNCTION_NAME) {
				return false;

			// Function token before left bracket token
			} else { return true; }
		}
		private static bool IfOneArgOperAfterRightBracket(int rightBracketTokenIndex) {

			// If last index or not operator token after right bracket
			if (rightBracketTokenIndex == Token.WorkingTokens.Count - 1 || Token.WorkingTokens[rightBracketTokenIndex + 1].Type != Token.TokenRole.MATH_OPERATOR) {

				return false;

			} else {

				// Getting operator object
				MathOperator oper = MathOperator.MathOperators.Find(obj => obj.Phrase == Token.WorkingTokens[rightBracketTokenIndex + 1].Text);

				// If one argument and not two argument operator
				if (oper.OneArgument == true && oper.TwoArgument == false) {

					return true;
				}
			}

			return false;
		}

		private static void ConvertFinalResultIfNecessary() {

			// If one of the tokens was relational operator, meaning it was a boolean expression and true or false has to be printed out as a new entry
			if (isBoolean) {

				if ((double)Expression.finalResult == 1) { Expression.finalResult = "true";
				} else if ((double)Expression.finalResult == 0) { Expression.finalResult = "false"; }

			
			} else {
				
				double a = -1;
				double b = -1;

				// If finalResult is same as currentExpression, meaning single number was entered and value type needs to be printed out
				if (Double.TryParse(Expression.finalResult.ToString(), out a) && Double.TryParse(Expression.expression, out b) && a == b) {

					// Result is float number
					if (Expression.finalResult.ToString().Contains('.')) { Expression.finalResult = "floating-point number";

					// Integer
					} else { Expression.finalResult = "integer number"; }
				}
			}
		}
	}
}