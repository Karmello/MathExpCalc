using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {

	static class Calculator {

		/* Public methods */
		public static string MakeCalculation(string a, Token operToken, string b) {

			double num1 = 0, num2 = 0;

			Double.TryParse(a, out num1);
			Double.TryParse(b, out num2);

			#region CONSOLE_PRINT_OUT

			if (MainWindow.consolePrintOutEnabled) {

				Console.Clear();
				Console.WriteLine("METHOD: 'MakeCalculation'\n");
				Console.WriteLine("Arguments:\n");
				Console.WriteLine("a = " + "'" + a + "'");
				Console.WriteLine("operation = " + "'" + operToken.Text + "'");
				Console.WriteLine("b = " + "'" + b + "'");
				Console.ReadKey();
			}

			#endregion



			switch (operToken.Text) {

				// Arithmetic operators
				case "!": return FactorialScenario(ref num1, ref operToken);
				case "**": case "^": case "pwr": return (Math.Pow(num1, num2)).ToString();
				case "%": return (num1 % num2).ToString();
				case "*": return (num1 * num2).ToString();
				case "/": return ZeroDivisionScenario(ref num1, ref operToken, ref num2);
				case "//": return ZeroDivisionScenario(ref num1, ref operToken, ref num2);
				case "+": return (num1 + num2).ToString();
				case "-": return (num1 - num2).ToString();
				case "=": return EqualsScenario(a, b);

				// Relational operators
				case "==": Expression.isBoolean = true; if (num1 == num2) { return (1).ToString(); } else { return (0).ToString(); }
				case "!=": Expression.isBoolean = true; if (num1 != num2) { return (1).ToString(); } else { return (0).ToString(); }
				case ">": Expression.isBoolean = true; if (num1 > num2) { return (1).ToString(); } else { return (0).ToString(); }
				case ">=": Expression.isBoolean = true; if (num1 >= num2) { return (1).ToString(); } else { return (0).ToString(); }
				case "<": Expression.isBoolean = true; if (num1 < num2) { return (1).ToString(); } else { return (0).ToString(); }
				case "<=": Expression.isBoolean = true; if (num1 <= num2) { return (1).ToString(); } else { return (0).ToString(); }
				
				// Functions
				case "sqrt": return SquareRootScenario(ref num1, ref operToken);
				case "cbrt": return CubeRootScenario(ref num1, ref operToken);
				case "abs": return (Math.Abs(num1)).ToString();
				case "mod": return (num1 % num2).ToString();
				case "round": return RoundScenario(ref num1, ref operToken, ref num2);
				case "floor": return (Math.Floor(num1)).ToString();
				case "ceil": return (Math.Ceiling(num1)).ToString();
				case "min": return (Math.Min(num1, num2)).ToString();
				case "max": return (Math.Max(num1, num2)).ToString();
				case "log": return (Math.Log(num1, num2)).ToString();
				case "exp": return (Math.Exp(num1)).ToString();
				case "sind": return Math.Sin(num1 * (Math.PI / 180)).ToString();
				case "cosd": return Math.Cos(num1 * (Math.PI / 180)).ToString();
				case "tand": return Math.Tan(num1 * (Math.PI / 180)).ToString();

			}

			return (0).ToString();
		}

		/* Private methods */
		private static string FactorialScenario(ref double num1, ref Token operToken) {

			double factorial = 1;



			// Factorial number is not an integer
			if (Math.Round(num1) != num1) {

				Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FACTORIAL_ARG, operToken.Text, operToken.StartIndex, operToken.Text.Length };
				return String.Empty;

			// Factorial number is an integer
			} else {

				// Factorial of 0 or 1
				if (num1 == 0 || num1 == 1) {
					return (1).ToString();

				// Factorial of a number greater than or equal to 2 and less than 171
				} else if (num1 >= 2 && num1 < 171) {

					for (int i = 2; i <= num1; ++i) { factorial *= i; }
					return factorial.ToString();

				// Factorial number is less than zero or greater than 170
				} else {

					Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FACTORIAL_ARG, operToken.Text, operToken.StartIndex, operToken.Text.Length };
					return String.Empty;
				}
			}
		}
		private static string SquareRootScenario(ref double num1, ref Token operToken) {

			// Invalid argument
			if (num1 < 0) {

				Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FUNCTION_CALL, operToken.Text, operToken.StartIndex, operToken.Text.Length };
				return String.Empty;

			// Valid argument
			} else { return (Math.Sqrt(num1)).ToString(); }
		}
		private static string CubeRootScenario(ref double num1, ref Token operToken) {

			if (num1 < 0) {

				return (0 - Math.Pow(-num1, 0.333333333333333333)).ToString();

			} else {

				return (Math.Pow(num1, 0.333333333333333333)).ToString();
			}
		}
		private static string ZeroDivisionScenario(ref double num1, ref Token operToken, ref double num2) {

			// Zero division happened
			if (num2 == 0) {

				Input.exceptionInfo = new object[] { Warning.WarningType.ZERO_DIVISION, operToken.Text, operToken.StartIndex, operToken.Text.Length };
				return String.Empty;
				
			// Valid division
			} else {

				// Float pointing division
				if (operToken.Text == "/") {

					return (num1 / num2).ToString();

				// Floar division
				} else if (operToken.Text == "//") {

					return (Math.Floor(num1 / num2)).ToString();
				}
			}

			return String.Empty;
		}
		private static string RoundScenario(ref double num1, ref Token operToken, ref double num2) {

			if (num2 < 0 || num2 > 15) {

				Input.exceptionInfo = new object[] { Warning.WarningType.INVALID_FUNCTION_CALL, operToken.Text, operToken.StartIndex, operToken.Text.Length };
				return String.Empty;

			} else {
				return (Math.Round(num1, (int)num2)).ToString();
			}
		}
		private static string EqualsScenario(string a, string b) {

			Variable variable = null;
			variable = Variable.Variables.Find(obj => obj.Phrase == a);

			// Variable already exists
			if (variable != null) {

				variable.Value = Double.Parse(b);

			// New variable
			} else {

				Variable.AddNewVariable(a, Double.Parse(b));
			}

			return b;
		}
	}
}