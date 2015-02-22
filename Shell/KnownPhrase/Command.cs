using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shell {
	
	class Command : KnownPhrase {

		public enum Cmds { NONE, START, CMD, KEY, OPER, CONST, VAR, DELVAR, BOARD, CLNBOARD, HIDE, BYE };

		public static List<Command> Commands = null;
		public static string CmdChar = Character.Characters[0].Phrase;
		public static SolidColorBrush SystemCmdColor = Brushes.Khaki;

		public static List<Run> CmdCharInfo = new List<Run>() {

			new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.STANDARD].Text) { Foreground = RunOfText.EntryPrefixColor },
			new Run("'") { Foreground = RunOfText.StandardTextColor },
			new Run(Character.Characters[0].Phrase) { Foreground = Command.SystemCmdColor },
			new Run("'") { Foreground = RunOfText.StandardTextColor },
			new Run(RunOfText.Prefixes[(int)RunOfText.Prefix.PAUSE].Text) { Foreground = RunOfText.DescriptionPartPrefixColor },
			new Run(Character.Characters[0].Description + "\n") { Foreground = RunOfText.StandardTextColor }
		};

		/* Properties */
		public Cmds Cmd { get; set; }

		/* Public methods */
		public Command(object[] args) {

			Cmd = (Cmds)args[0];
			Description = (string)args[1];
		}
		public static void InitializeCommands() {

			Commands = new List<Command>();

			Commands.Add(new Command(new object[] { Cmds.START, "shows initial screen" }));
			Commands.Add(new Command(new object[] { Cmds.CMD, "shows list of all system commands" }));
			Commands.Add(new Command(new object[] { Cmds.KEY, "shows keys available" }));
			Commands.Add(new Command(new object[] { Cmds.OPER, "shows math operators" }));
			Commands.Add(new Command(new object[] { Cmds.CONST, "shows constant values" }));
			Commands.Add(new Command(new object[] { Cmds.VAR, "shows variables" }));
			Commands.Add(new Command(new object[] { Cmds.DELVAR, "deletes all variables from memory" }));
			Commands.Add(new Command(new object[] { Cmds.BOARD, "shows calculation board" }));
			Commands.Add(new Command(new object[] { Cmds.CLNBOARD, "cleans calculation board" }));
			Commands.Add(new Command(new object[] { Cmds.HIDE, "minimizes window" }));
			Commands.Add(new Command(new object[] { Cmds.BYE, "exits program" }));
		}
		public static void ExecuteSysCmd(Cmds cmd) {

			// If static page command
			if (StaticPage.StaticPages.Any(page => page.InvokeCmd == cmd)) {

				StaticPage.ShowStaticPage(cmd);

			// Not static page command
			} else {

				switch (cmd) {

					// Shows variables
					case Cmds.VAR:

						Variable.RefreshVariables();
						break;

					// Deletes variables
					case Cmds.DELVAR:

						Variable.DeleteVariables();
						Variable.RefreshVariables();
						break;

					// Shows board
					case Cmds.BOARD:

						BoardEntry.RefreshBoard();
						break;

					// Empties board
					case Cmds.CLNBOARD:

						BoardEntry.DeleteBoardEntries();
						BoardEntry.RefreshBoard();
						break;

					// Minimizes window
					case Cmds.HIDE:

						MainWindow.ui.WindowState = WindowState.Minimized;
						break;

					// Exits program
					case Cmds.BYE:

						/* Clearing text, sleeping a bit to better show it and closing application */

						MainWindow.closingApp = true;

						MainWindow.ui.textBlock.Text = String.Empty;
						MainWindow.ui.textBox.Text = String.Empty;
						MainWindow.ui.timeLabel.Content = String.Empty;

						Task.Factory.StartNew(new Action(() => {

							Thread.Sleep(MainWindow.beforeCloseDelay);
							MainWindow.ui.Dispatcher.BeginInvoke(new Action(() => { MainWindow.ui.Close(); }));
						}));

						break;

					default:
						break;
				}
			}
		}
	}
}