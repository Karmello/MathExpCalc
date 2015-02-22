using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shell {

	public class Character : KnownPhrase {

		public static List<Character> Characters;
		public enum Edge { NONE, LEFT, RIGHT };
		public enum CharacterType { NONE, NUMERIC, ARITHMETIC, GROUPING, ALPHABETIC }

		/* Properties */
		public CharacterType CharType { get; set; }
		public bool FirstIndexAllowed { get; set; }
		public bool LastIndexAllowed { get; set; }
		public List<char> WrongRightAfters { get; set; }
		public List<char> WrongTypeFirstFollowers { get; set; }

		/* Public methods */
		public Character(object[] args) {

			CharType = (CharacterType)args[0];

			Phrase = (string)args[1];
			Description = (string)args[2];

			FirstIndexAllowed = (bool)args[3];
			LastIndexAllowed = (bool)args[4];
			WrongRightAfters = args[5] as List<char>;
			WrongTypeFirstFollowers = args[6] as List<char>;
		}
		public static void DefineCharacters() {

			Characters = new List<Character>();

			// System command char
			Characters.Add(new Character(new object[] { CharacterType.ALPHABETIC, "$", "Dollar sign indicates system command", true, true, null, null }));

			// Numeric chars
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "0", "zero digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "1", "one digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "2", "two digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "3", "three digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "4", "four digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "5", "five digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "6", "six digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "7", "seven digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "8", "eight digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, "9", "nine digit", true, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.NUMERIC, ".", "full stop", false, false, new List<char>() { ',', '(', '[', ')', ']' }, null }));
			
			// Operator chars
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "!", "exclamation mark", false, true, null, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "^", "caret", false, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "*", "asteriks", false, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "/", "forward slash", false, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "+", "plus", true, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "-", "minus", true, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "=", "equals sign", false, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "<", "left angle bracket", false, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, ">", "right angle bracket", false, false, new List<char>() { ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.ARITHMETIC, "%", "percentage", false, false, new List<char>() { ')', ']' }, null }));

			// Grouping chars
			Characters.Add(new Character(new object[] { CharacterType.GROUPING, ",", "comma", false, false, new List<char>() { ',', '.', ')', ']' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.GROUPING, "(", "left bracket", true, false, new List<char>() { '.', ',', ')', ']', '!', '^', '*', '/', '=', '<', '>', '%' }, new List<char>() { ']' } }));
			Characters.Add(new Character(new object[] { CharacterType.GROUPING, ")", "right bracket", false, true, new List<char>() { '.' }, null }));
			Characters.Add(new Character(new object[] { CharacterType.GROUPING, "[", "left bracket", true, false, new List<char>() { '.', ',', ')', ']', '!', '^', '*', '/', '=', '<', '>', '%' }, new List<char>() { ')' } }));
			Characters.Add(new Character(new object[] { CharacterType.GROUPING, "]", "right bracket", false, true, new List<char>() { '.' }, null }));
		}
	}
}