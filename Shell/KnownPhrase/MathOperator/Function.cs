using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Shell {

	class Function : MathOperator {

		public static SolidColorBrush FunctionColor = Brushes.Khaki;

		/* Public method */
		public Function(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
			OppositePhrase = (string)args[2];

			OneArgument = (bool)args[3];
			TwoArgument = (bool)args[4];
			Role = (MathOperatorRole)args[5];
			Precedence = (MathOperatorPrecedence)args[6];
			ErrorDescription = (string)args[7];
		}
		public static void DefineFunctions() {
			
			MathOperators.Add(new Function(new object[] { "sqrt", "square root", "pwr", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Square root is one-parameter function which takes greater than or equal to zero numeric value" }));
			MathOperators.Add(new Function(new object[] { "cbrt", "cube root", "\0", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Cube root is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "pwr", "power", "sqrt", false, true, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Power is two-parameter function" }));
			MathOperators.Add(new Function(new object[] { "mod", "finds the remainder of division of one number by another", "\0", false, true, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Modulo is two-parameter function" }));
			MathOperators.Add(new Function(new object[] { "abs", "absolute value", "\0", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Absolute value is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "round", "rounds floating-point number", "\0", true, true, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Round function takes one or two parameters, second must be between 0 and 15 inclusive" }));
			MathOperators.Add(new Function(new object[] { "floor", "rounds floating-point number down to an integer", "ceil", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Floor is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "ceil", "rounds floating-point number up to an integer", "floor", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Ceil is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "min", "finds the smaller of two numbers", "max", false, true, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Min is two-parameter function" }));
			MathOperators.Add(new Function(new object[] { "max", "finds the larger of two numbers", "min", false, true, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Max is two-parameter function" }));
			MathOperators.Add(new Function(new object[] { "log", "logarithm", "\0", false, true, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Logarithm is two-parameter function" }));
			MathOperators.Add(new Function(new object[] { "exp", "returns Euler's constant raised to the specified power", "\0", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Exponent is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "sind", "degree sine function", "\0", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Degree sine is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "cosd", "degree cosine function", "\0", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Degree cosine is one-parameter function" }));
			MathOperators.Add(new Function(new object[] { "tand", "degree tangent function", "\0", true, false, MathOperatorRole.FUNCTION, MathOperatorPrecedence.FIRST, "Degree tangent is one-parameter function" }));
		}
	}
}