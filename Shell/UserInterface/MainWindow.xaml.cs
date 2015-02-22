using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shell {

	public partial class MainWindow : Window {

		public static Stopwatch timer = new Stopwatch();
		
		/* Variables */
		public static bool consolePrintOutEnabled = false;
		public static bool closingApp = false;
		public static readonly int beforeCloseDelay = 200;
		public static string timeLabelText = " Execution time:   ";
		private static Brush acceptButtonBorderColor = new BrushConverter().ConvertFromString("#FF265E7E") as Brush;

		public static MainWindow ui = null;

		private static Thickness acceptButtonRect_InitialMargin = new Thickness(635, 327, 0, 0);
		private static Thickness acceptButtonLabel_InitialMargin = new Thickness(648, 325, 0, 0);

		/* Methods */
		public MainWindow() {
			
			ui = this;

			// Setting application culture
			CultureInfo customCulture = new CultureInfo("en-US");
			customCulture.NumberFormat.NumberDecimalSeparator = ".";
			Thread.CurrentThread.CurrentCulture = customCulture;

			// Initializing
			InitializeComponent();
			KnownPhrase.DefineKnownPhrases();
			Character.DefineCharacters();
			ConstantValue.DefineConstantValues();
			Command.InitializeCommands();
			Digit.DefineDigits();
			Delimiter.DefineDelimiters();
			Function.DefineFunctions();
			ArithmeticOperator.DefineArithmeticOperators();
			RelationalOperator.DefineRelationalOperators();
			GroupingSymbol.DefineGroupingSymbols();
			StaticPage.CreateStaticPages();

			// Initial message
			StaticPage.ShowStaticPage(pageIndex: 0);
			this.textBox.Focus();
		}

		/* Events */
		private void textBox_TextChanged(object sender, TextChangedEventArgs e) {

			// Clearing warning entries
			if (Output.ActiveScreen == Output.Screens.BOARD && BoardEntry.BoardEntries.Count > 0 && Warning.warningEntryOut == true) {

				BoardEntry.DeleteSpecifiedTypesOfEntries(new List<Output.OutputRole>() { Output.OutputRole.WARNING });
				BoardEntry.RefreshBoard();
			}

			// If history command in textbox
			if (Input.activeHistoryCmd != -1) { 
				
				// If history command from textbox was changed
				if (!Input.validInputHistory.Contains(textBox.Text)) {
					
					// No active history command anymore
					Input.activeHistoryCmd = -1;
				}
			}
		}
		private void textBox_PreviewKeyDown(object sender, KeyEventArgs e) {

			// Enter
			if (e.Key == Key.Return) {

				if (acceptButton_Rect.Fill != Brushes.LightGray) {

					acceptButton_Rect.Stroke = acceptButtonBorderColor;
					acceptButton_Label.Foreground = Brushes.Black;

					acceptButton_Rect.Fill = Brushes.LightGray;
					acceptButton_Rect.Margin = new Thickness(acceptButtonRect_InitialMargin.Left + 1, acceptButtonRect_InitialMargin.Top + 1, 0, 0);
					acceptButton_Label.Margin = new Thickness(acceptButtonLabel_InitialMargin.Left + 1, acceptButtonLabel_InitialMargin.Top + 1, 0, 0);

					//Output.RemoveFromBoard(Output.OutputRole.WARNING);
					Input.ReceiveInput();
				}
			}
			// Escape
			else if (e.Key == Key.Escape) {

				if (textBox.Text.Length > 0) {

					//Output.RemoveFromBoard(Output.OutputRole.WARNING);
					Input.activeHistoryCmd = -1;
					textBox.Text = String.Empty;

				} else { Command.ExecuteSysCmd(Command.Cmds.BYE); }
			}
			// Down or up arrow
			else if (e.Key == Key.Down || e.Key == Key.Up) {

				// If no digit is selected
				if (textBox.SelectedText.Length == 0) {

					// If there is one of saved commands currently in the textbox
					if (Input.activeHistoryCmd != -1) {

						// Up arrow
						if (e.Key == Key.Up) {

							// Not first saved command in the textbox
							if (Input.activeHistoryCmd > 0) { Input.activeHistoryCmd -= 1; }

							// First saved command active
							else { Input.activeHistoryCmd = Input.validInputHistory.Count - 1; }

						}
						// Down arrow
						else if (e.Key == Key.Down) {

							// Not last saved command in the textbox
							if (Input.activeHistoryCmd < Input.validInputHistory.Count - 1) { Input.activeHistoryCmd += 1; }

							// Last saved command active
							else { Input.activeHistoryCmd = 0; }
						}

						textBox.Text = Input.validInputHistory[Input.activeHistoryCmd];

						// Setting cursor at the end of textbox text
						textBox.Select(textBox.Text.Length, 0);

					} else {

						// At least one saved command in history
						if (Input.validInputHistory.Count > 0) {

							if (e.Key == Key.Up) {

								textBox.Text = Input.validInputHistory.Last();
								Input.activeHistoryCmd = Input.validInputHistory.Count - 1;
							} else if (e.Key == Key.Down) {

								textBox.Text = Input.validInputHistory.First();
								Input.activeHistoryCmd = 0;
							}

							// Setting cursor at the end of textbox text
							textBox.Select(textBox.Text.Length, 0);
						}
					}
				}
			}
			// F5
			else if (e.Key == Key.F5) {

				Command.ExecuteSysCmd(Command.Cmds.CLNBOARD);
			} 
		}
		private void textBox_PreviewKeyUp(object sender, KeyEventArgs e) {

			// Enter
			if (e.Key == Key.Return) {

				acceptButton_Rect.Margin = acceptButtonRect_InitialMargin;
				acceptButton_Label.Margin = acceptButtonLabel_InitialMargin;

				acceptButton_Rect.Fill = Brushes.Black;
				acceptButton_Rect.Stroke = acceptButtonBorderColor;
				acceptButton_Label.Foreground = Brushes.White;
			}
		}

		private void acceptButton_Rect_MouseEnter(object sender, MouseEventArgs e) {

			acceptButton_Rect.Fill = Brushes.White;
			acceptButton_Rect.Stroke = acceptButtonBorderColor;
			acceptButton_Label.Foreground = Brushes.Black;
			
		}
		private void acceptButton_Rect_PreviewMouseDown(object sender, MouseButtonEventArgs e) {

			acceptButton_Rect.Fill = Brushes.LightGray;
			acceptButton_Rect.Margin = new Thickness(acceptButtonRect_InitialMargin.Left + 1, acceptButtonRect_InitialMargin.Top + 1, 0, 0);
			acceptButton_Label.Margin = new Thickness(acceptButtonLabel_InitialMargin.Left + 1, acceptButtonLabel_InitialMargin.Top + 1, 0, 0);
		}
		private void acceptButton_Rect_PreviewMouseUp(object sender, MouseButtonEventArgs e) {

			acceptButton_Rect.Fill = Brushes.White;
			acceptButton_Rect.Margin = acceptButtonRect_InitialMargin;
			acceptButton_Label.Margin = acceptButtonLabel_InitialMargin;

			Input.ReceiveInput();
		}
		private void acceptButton_Rect_MouseLeave(object sender, MouseEventArgs e) {


			acceptButton_Rect.Margin = acceptButtonRect_InitialMargin;
			acceptButton_Label.Margin = acceptButtonLabel_InitialMargin;

			acceptButton_Rect.Fill = Brushes.Black;
			acceptButton_Rect.Stroke = acceptButtonBorderColor;
			acceptButton_Label.Foreground = Brushes.White;
		}
	}
}