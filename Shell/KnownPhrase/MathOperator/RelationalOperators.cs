using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {

	class RelationalOperator : MathOperator {

		/* Public methods */
		public RelationalOperator(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
			OppositePhrase = (string)args[2];

			OneArgument = (bool)args[3];
			TwoArgument = (bool)args[4];
			Role = (MathOperatorRole)args[5];
			Precedence = (MathOperatorPrecedence)args[6];
			ErrorDescription = (string)args[7];
		}
		public static void DefineRelationalOperators() {

			MathOperators.Add(new RelationalOperator(new object[] { "==", "equal to", "!=", false, true, MathOperatorRole.RELATIONAL, MathOperator.MathOperatorPrecedence.FIFTH, String.Empty }));
			MathOperators.Add(new RelationalOperator(new object[] { "!=", "not equal to", "==", false, true, MathOperatorRole.RELATIONAL, MathOperator.MathOperatorPrecedence.FIFTH, String.Empty }));
			MathOperators.Add(new RelationalOperator(new object[] { ">", "greater than", "<", false, true, MathOperatorRole.RELATIONAL, MathOperator.MathOperatorPrecedence.FIFTH, String.Empty }));
			MathOperators.Add(new RelationalOperator(new object[] { ">=", "greater than or equal to", "<=", false, true, MathOperatorRole.RELATIONAL, MathOperator.MathOperatorPrecedence.FIFTH, String.Empty }));
			MathOperators.Add(new RelationalOperator(new object[] { "<", "less than", ">", false, true, MathOperatorRole.RELATIONAL, MathOperator.MathOperatorPrecedence.FIFTH, String.Empty }));
			MathOperators.Add(new RelationalOperator(new object[] { "<=", "less than or equal to", ">=", false, true, MathOperatorRole.RELATIONAL, MathOperator.MathOperatorPrecedence.FIFTH, String.Empty }));
		}
	}
}