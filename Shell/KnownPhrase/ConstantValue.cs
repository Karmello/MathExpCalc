using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Shell {
	
	class ConstantValue : KnownPhrase {

		public static List<ConstantValue> ConstantValues = null;
		public static SolidColorBrush ConstColor = Brushes.Khaki;
		
		/* Fields */
		public double Value { get; set; }

		/* Public methods */
		public ConstantValue(object[] args) {

			Phrase = (string)args[0];
			Value = (double)args[1];
			Description = (string)args[2];
		}
		public static void DefineConstantValues() {

			ConstantValues = new List<ConstantValue>();

			ConstantValues.Add(new ConstantValue(new object[] { "PI", 3.1415926535897932384626433832795, "the ratio of a circle's circumference to its diameter" }));
			ConstantValues.Add(new ConstantValue(new object[] { "E", 2.71828182845904523536028747135266249775724709369995, "Euler's constant" }));
		}
	}
}
