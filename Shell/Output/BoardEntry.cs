using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {

	class BoardEntry {

		public static List<BoardEntry> BoardEntries = new List<BoardEntry>();

		/* Properties */
		public int EntryNum { get; set; }
		public Run[] EntryRuns { get; set; }
		public Output.OutputRole EntryRole { get; set; }
		public string EntryText { get; set; }

		/* Public methods */
		public BoardEntry(int entryNum, Run[] entryRuns, Output.OutputRole entryRole) {

			EntryNum = entryNum;
			EntryRuns = new Run[entryRuns.Length];

			for (int i = 0; i < entryRuns.Length; ++i) {

				EntryRuns[i] = entryRuns[i];
				EntryText += entryRuns[i].Text;
			}

			EntryRole = entryRole;
		}

		public static void UpdateBoard(Run[] text, Output.OutputRole outputRole) {

			// Warning out
			if (outputRole == Output.OutputRole.WARNING) {

				// If warning out
				if (outputRole == Output.OutputRole.WARNING) {

					// Removing all warning entries
					BoardEntry.DeleteSpecifiedTypesOfEntries(new List<Output.OutputRole> { Output.OutputRole.WARNING });
					RefreshBoard();
				}

				Warning.warningEntryOut = true;

				// Changing prefix to the one without new line sign if necessary
				if (BoardEntries.Count == 0) {
					text[0] = RunOfText.Prefixes[(int)RunOfText.Prefix.WARNING_NO_EMPTY_LINE];
				}
			}



			switch (outputRole) {

				// Calculation, warning or phrase description out
				case Output.OutputRole.CALCULATION:
				case Output.OutputRole.VAR_DEFINITION:
				case Output.OutputRole.WARNING:
				case Output.OutputRole.PHRASE_DESC:

					// Board visible
					if (Output.ActiveScreen == Output.Screens.BOARD) {

						// Empty board
						if (BoardEntries.Count == 0) { Output.ClearScreen(); }

						BoardEntry.AddBoardEntry(BoardEntries.Count + 1, text, outputRole);
						MainWindow.ui.textBlock.Inlines.AddRange(text);
						MainWindow.ui.Scroller.ScrollToBottom();

					// Static page visible
					} else if (Output.ActiveScreen == Output.Screens.STATIC_PAGE || Output.ActiveScreen == Output.Screens.VARS) {

						BoardEntry.AddBoardEntry(BoardEntries.Count + 1, text, outputRole);
						RefreshBoard();
					}

					break;

				default:
					break;
			}
		}
		public static void ShowBoard() {

			if (BoardEntries.Count > 0) {

				foreach (var entry in BoardEntries) {
					MainWindow.ui.textBlock.Inlines.AddRange(entry.EntryRuns);
				}

			} else {

				MainWindow.ui.textBlock.Inlines.Add(new Run("Board is empty"));
			}

			Output.ActiveScreen = Output.Screens.BOARD;
			MainWindow.ui.Scroller.ScrollToBottom();
		}
		public static void RefreshBoard() {

			Output.ClearScreen();
			ShowBoard();
		}

		public static void AddBoardEntry(int num, Run[] text, Output.OutputRole role) {

			BoardEntry.BoardEntries.Add(new BoardEntry(num, text, role));
		}
		public static void DeleteBoardEntries() {

			BoardEntry.BoardEntries.Clear();
		}
		public static bool DeleteSpecifiedTypesOfEntries(List<Output.OutputRole> types) {

			bool anythingDeleted = false;

			for (int i = BoardEntry.BoardEntries.Count - 1; i >= 0; --i) {
				if (types.Contains(BoardEntry.BoardEntries[i].EntryRole)) {

					BoardEntry.BoardEntries.RemoveAt(i);
					anythingDeleted = true;
				}
			}

			// If warning deleted
			if (types.Contains(Output.OutputRole.WARNING)) {
				Warning.warningEntryOut = false;
			}

			return anythingDeleted;
		}
	}
}