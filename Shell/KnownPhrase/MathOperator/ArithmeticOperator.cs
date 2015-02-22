using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {

	class ArithmeticOperator : MathOperator {

		/* Public methods */
		public ArithmeticOperator(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
			OppositePhrase = (string)args[2];

			OneArgument = (bool)args[3];
			TwoArgument = (bool)args[4];
			Role = (MathOperatorRole)args[5];
			Precedence = (MathOperatorPrecedence)args[6];
			ErrorDescription = (string)args[7];
		}
		public static void DefineArithmeticOperators() {

			MathOperators.Add(new ArithmeticOperator(new object[] { "!", "factorial", "\0", true, false, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.FIRST, "Factorial argument must be greater than or equal to zero and less than 171 integer value" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "**", "power", "\0", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.SECOND, "Power is two-argument operation" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "^", "power", "\0", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.SECOND, "Power is two-argument operation" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "*", "multiplication", "/", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.THIRD, "Multiplication is two-argument operation" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "/", "floating point division", "*", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.THIRD, "Can't devide by zero" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "//", "floar division", "\0", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.THIRD, "Can't devide by zero" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "%", "modulo", "\0", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.THIRD, "Modulo is two-argument operation" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "+", "addition", "-", true, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.FORTH, "Addition is two-argument operation" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "-", "subtraction", "+", true, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.FORTH, "Subtraction is two-argument operation" }));
			MathOperators.Add(new ArithmeticOperator(new object[] { "=", "equals", "\0", false, true, MathOperatorRole.ARITHMETIC, MathOperatorPrecedence.FIFTH, "Use equals operator to define new or change value of existing variables" }));
		}
	}
}