using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {
	
	class Digit : KnownPhrase {

		public static List<Digit> Digits;

		/* Public methods */
		public Digit(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
		}
		public static void DefineDigits() {

			Digits = new List<Digit>();

			Digits.Add(new Digit(new object[] { "0", "integer number" }));
			Digits.Add(new Digit(new object[] { "1", "integer number" }));
			Digits.Add(new Digit(new object[] { "2", "integer number" }));
			Digits.Add(new Digit(new object[] { "3", "integer number" }));
			Digits.Add(new Digit(new object[] { "4", "integer number" }));
			Digits.Add(new Digit(new object[] { "5", "integer number" }));
			Digits.Add(new Digit(new object[] { "6", "integer number" }));
			Digits.Add(new Digit(new object[] { "7", "integer number" }));
			Digits.Add(new Digit(new object[] { "8", "integer number" }));
			Digits.Add(new Digit(new object[] { "9", "integer number" }));
		}
	}
}