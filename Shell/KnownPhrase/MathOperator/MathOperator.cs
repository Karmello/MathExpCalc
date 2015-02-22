using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Shell {
	
	class MathOperator : KnownPhrase {

		public enum MathOperatorRole { ARITHMETIC, RELATIONAL, FUNCTION };
		public enum MathOperatorPrecedence { FIRST, SECOND, THIRD, FORTH, FIFTH };
		public static List<MathOperator> MathOperators = new List<MathOperator>();
		public static SolidColorBrush MathOperatorColor = Brushes.Khaki;

		/* Properties */
		public bool OneArgument { get; set; }
		public bool TwoArgument { get; set; }
		public MathOperatorRole Role { get; set; }
		public MathOperatorPrecedence Precedence { get; set; }
		public string ErrorDescription { get; set; }

		/* Public methods */
		public static List<int> DetermineOperatorsPresedence(List<Token> setOfTokens) {

			/* This function returns indexes of arithmetic operators tokens in mathematical precedence order */

			var indexes = new List<int>();
			var operatorsFound = new List<Tuple<int, MathOperatorPrecedence>>();
			ArithmeticOperator arithOperObj = null;
			RelationalOperator relOperObj = null;



			// Going through all tokens
			for (int i = 0; i < setOfTokens.Count; ++i) {

				// If operator token
				if (setOfTokens[i].Type == Token.TokenRole.MATH_OPERATOR) {

					// Trying to get operator object
					arithOperObj = MathOperators.Find(obj => (obj.Phrase == setOfTokens[i].Text)) as ArithmeticOperator;
					relOperObj = MathOperators.Find(obj => (obj.Phrase == setOfTokens[i].Text)) as RelationalOperator;

					// If it's valid arithmetic operator token
					if (arithOperObj != null) {

						operatorsFound.Add(new Tuple<int, MathOperatorPrecedence>(i, arithOperObj.Precedence));
						arithOperObj = null;

						// If it's valid relational operator token
					} else if (relOperObj != null) {

						operatorsFound.Add(new Tuple<int, MathOperatorPrecedence>(i, relOperObj.Precedence));
						relOperObj = null;
					}
				}
			}



			// If any operators found
			if (operatorsFound.Count > 0) {

				// Sorting operators according to mathematical operators precedence
				operatorsFound = new List<Tuple<int, MathOperatorPrecedence>>(operatorsFound.OrderBy(item => item.Item2).ThenBy(item => item.Item1));

				foreach (var item in operatorsFound) { indexes.Add(item.Item1); }
				#region CONSOLE_PRINT_OUT

				if (MainWindow.consolePrintOutEnabled) {

					Console.Clear();
					Console.WriteLine("METHOD: 'DetermineOperatorsPresedence'\n");
					Console.WriteLine("Operators in mathematical order:\n");

					int no = 1;
					foreach (var op in operatorsFound) {
						Console.WriteLine((no++).ToString() + ") '" + setOfTokens[op.Item1].Text + "' - (" + op.Item2 + ")");
					}

					Console.ReadKey();
				}

				#endregion

			} else {

				#region CONSOLE_PRINT_OUT

				if (MainWindow.consolePrintOutEnabled) {

					Console.Clear();
					Console.WriteLine("METHOD: 'DetermineOperatorsPresedence'\n");
					Console.WriteLine("No two argument operators found");
					Console.ReadKey();
				}

				#endregion
			}

			return indexes;
		}
	}
}