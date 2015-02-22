using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {

	class Delimiter : KnownPhrase {

		public static List<Delimiter> Delimiters;

		/* Public methods */
		public Delimiter(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
		}
		public static void DefineDelimiters() {

			Delimiters = new List<Delimiter>();
			Delimiters.Add(new Delimiter(new object[] { ".", "float number delimiter" }));
			Delimiters.Add(new Delimiter(new object[] { ",", "function argument delimiter" }));
		}
	}
}