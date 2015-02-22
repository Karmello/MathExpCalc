using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {

	public class KnownPhrase {

		public static List<KnownPhrase> knownPhrases;

		/* Properties */
		public string Phrase { get; set; }
		public string Description { get; set; }
		public string OppositePhrase { get; set; }

		/* Public methods */
		public KnownPhrase() { }
		public KnownPhrase(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
			OppositePhrase = (string)args[2];
		}
		public static void DefineKnownPhrases() {

			knownPhrases = new List<KnownPhrase>();

			knownPhrases.Add(new KnownPhrase(new object[] { "true", "Boolean value", "false" }));
			knownPhrases.Add(new KnownPhrase(new object[] { "false", "Boolean value", "true" }));
		}
	}
}