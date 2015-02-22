using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {

	class GroupingSymbol : KnownPhrase {

		public static List<Tuple<char, int>> brackets = new List<Tuple<char, int>>();
		public static List<int> notClosedBracketsIndexes = new List<int>();
		public static SolidColorBrush GroupingSymbolColor = Brushes.Khaki;

		public static List<GroupingSymbol> GroupingSymbols;
		public enum GroupingSymbolSide { LEFT, RIGHT };
		public enum GroupingSymbolShape { ROUND, SQUARE };

		/* Properties */
		public GroupingSymbolSide Side { get; set; }
		public GroupingSymbolShape Shape { get; set; }

		/* Public methods */
		public GroupingSymbol(object[] args) {

			Phrase = (string)args[0];
			Description = (string)args[1];
			OppositePhrase = (string)args[2];
			
			Side = (GroupingSymbolSide)args[3];
			Shape = (GroupingSymbolShape)args[4];

		}
		public static void DefineGroupingSymbols() {

			GroupingSymbols = new List<GroupingSymbol>();

			GroupingSymbols.Add(new GroupingSymbol(new object[] { "(", "left round bracket", ")", GroupingSymbolSide.LEFT, GroupingSymbolShape.ROUND }));
			GroupingSymbols.Add(new GroupingSymbol(new object[] { ")", "right round bracket", "(", GroupingSymbolSide.RIGHT, GroupingSymbolShape.ROUND }));
			GroupingSymbols.Add(new GroupingSymbol(new object[] { "[", "left square bracket", "]", GroupingSymbolSide.LEFT, GroupingSymbolShape.SQUARE }));
			GroupingSymbols.Add(new GroupingSymbol(new object[] { "]", "right square bracket", "[", GroupingSymbolSide.RIGHT, GroupingSymbolShape.SQUARE }));
		}

		public static void FindNotClosedBracketsIndexes(ref List<int> notClosedBracketsIndexes, string input) {

			GroupingSymbol bracket = null;
			notClosedBracketsIndexes.Clear();



			for (int i = 0; i < input.Length; ++i) {

				bracket = GroupingSymbol.GroupingSymbols.Find(obj => (obj.Phrase[0] == input[i]));

				// Bracket found
				if (bracket != null) {

					// Left bracket found
					if (bracket.Side == GroupingSymbol.GroupingSymbolSide.LEFT) {
						notClosedBracketsIndexes.Add(i);

					// Right bracket found
					} else if (bracket.Side == GroupingSymbol.GroupingSymbolSide.RIGHT) {

						if (notClosedBracketsIndexes.Count > 0) { notClosedBracketsIndexes.RemoveAt(notClosedBracketsIndexes.Count - 1); }
					}

					bracket = null;
				}
			}
		}

		public static Run[] BracketValidation(GroupingSymbol iteratedBracketObj, int bracketIndex) {

			// If so far no brackets found
			if (brackets.Count == 0) {

				// The first bracket is right bracket
				if (GroupingSymbol.GroupingSymbols.Single(obj => (obj.Phrase == iteratedBracketObj.Phrase)).Side == GroupingSymbol.GroupingSymbolSide.RIGHT) {
					return Warning.CreateDynamicWarningMessage(Warning.WarningType.EXTRA_BRACKET, new int[] { bracketIndex }, Character.Edge.NONE);
				}

				brackets.Add(new Tuple<char, int>(iteratedBracketObj.Phrase[0], bracketIndex));

			// If there was at least one bracket found before
			} else {

				Character prevCharObj = Character.Characters.Find(obj => (obj.Phrase[0] == brackets.Last().Item1));
				GroupingSymbol prevBracketObj = GroupingSymbol.GroupingSymbols.Find(obj => (obj.Phrase[0] == brackets.Last().Item1));

				// If previous bracket has wrong followers declared
				if (prevCharObj.WrongTypeFirstFollowers != null) {

					// If bracket should not appear now */
					if (prevCharObj.WrongTypeFirstFollowers.Contains(iteratedBracketObj.Phrase[0])) {

						return Warning.CreateDynamicWarningMessage(Warning.WarningType.WRONG_BRACKET, new int[] { bracketIndex }, Character.Edge.NONE);

					// If bracket is allowed to appear now
					} else {

						// If pair of brackets is closed now properly
						if (prevBracketObj.Side == GroupingSymbol.GroupingSymbolSide.LEFT && iteratedBracketObj.Side == GroupingSymbol.GroupingSymbolSide.RIGHT && prevBracketObj.Phrase[0] == iteratedBracketObj.OppositePhrase[0]) {

							brackets.RemoveAt(brackets.Count - 1);

						} else { brackets.Add(new Tuple<char, int>(iteratedBracketObj.Phrase[0], bracketIndex)); }
					}

				// If previous object has no wrong followers declared
				} else { brackets.Add(new Tuple<char, int>(iteratedBracketObj.Phrase[0], bracketIndex)); }
			}



			// If iterated bracket is not closed bracket
			if (notClosedBracketsIndexes.Contains(bracketIndex)) {
				return Warning.CreateDynamicWarningMessage(Warning.WarningType.EXTRA_BRACKET, new int[] { brackets[0].Item2 }, Character.Edge.NONE);
			}

			return new Run[0];
		}
		public static Run[] FinalBracketsValidation() {

			// If there are some brackets left in brackets array meaning not all of them made pairs
			if (brackets.Count > 0) {
				return Warning.CreateDynamicWarningMessage(Warning.WarningType.EXTRA_BRACKET, new int[] { brackets[0].Item2 }, Character.Edge.NONE);
			}

			return new Run[0];
		}
	}
}