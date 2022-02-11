using ModernTerminal3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	class TerminalReader {
		int _startTop;
		int _startLeft;
		int _cursorOffset;
		string _acc;
		int _currentCommandHistoryIndex;
		ITerminalReaderInfoProvider _terminalReaderInfoProvider;
		private INativeConsole nativeConsole;
		public TerminalReader(ITerminalReaderInfoProvider terminalReaderInfoProvider, INativeConsole nativeConsole) {
			_terminalReaderInfoProvider = terminalReaderInfoProvider;
			this.nativeConsole = nativeConsole;
		}

		public string ReadLine() {
			ConsoleInMode oldInMode = nativeConsole.InMode;
			ConsoleOutMode oldOutMode = nativeConsole.OutMode;

			nativeConsole.OutMode = ConsoleOutMode.EnableEscapeSequences | ConsoleOutMode.ProcessedOutput | ConsoleOutMode.WrapAtEolOutput;
			nativeConsole.InMode = ConsoleInMode.None;

			(_startLeft, _startTop) = Console.GetCursorPosition();

			bool running = true;

			_acc = "";
			_cursorOffset = 0;
			_currentCommandHistoryIndex = 0;

			while (running) {
				ConsoleKeyInfo keyInfo = Console.ReadKey(true);
				char c = keyInfo.KeyChar;
				string oldAcc = _acc;
				//Dont add control characters
				if (c > 0x1F && c != 0x7F && (c < 0x80 || c > 0x9F)) {
					_acc = _acc.Insert(_cursorOffset, c.ToString());
					_cursorOffset += 1;
				} else {
					if (keyInfo.Key == ConsoleKey.Backspace) {
						if (_cursorOffset > 0) {
							string part1 = _acc[0..(_cursorOffset - 1)];
							string part2 = "";
							if (_cursorOffset < _acc.Length) {
								part2 = _acc[_cursorOffset..^0];
							}
							_acc = part1 + part2;
							_cursorOffset -= 1;
						}
					} else if (keyInfo.Key == ConsoleKey.Enter) {
						running = false;
					} else if (keyInfo.Key == ConsoleKey.LeftArrow) {
						if (_cursorOffset > 0) {
							_cursorOffset -= 1;
						}
					} else if (keyInfo.Key == ConsoleKey.RightArrow) {
						if (_cursorOffset < _acc.Length) {
							_cursorOffset += 1;
						}
					} else if (keyInfo.Key == ConsoleKey.Tab) {
						(string completion, int newOffset) = _terminalReaderInfoProvider.TabComplete(_acc, _cursorOffset);
						if (completion != null) {
							_acc = completion;
							_cursorOffset = newOffset;
						}
					} else if (keyInfo.Key == ConsoleKey.UpArrow) {
						string lastCommand = _terminalReaderInfoProvider.GetLastCommand(_currentCommandHistoryIndex - 1);
						if (lastCommand != null) {
							_currentCommandHistoryIndex--;
							_acc = lastCommand;
							_cursorOffset = _acc.Length;
						}
					} else if (keyInfo.Key == ConsoleKey.DownArrow) {
						string lastCommand = _terminalReaderInfoProvider.GetLastCommand(_currentCommandHistoryIndex + 1);
						if (lastCommand != null) {
							_currentCommandHistoryIndex++;
							_acc = lastCommand;
							_cursorOffset = _acc.Length;
						}
					} else if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control)) {
						ReprintInput(_acc, _acc + "^C");
						_acc = "";
						break;
					}
				}

				ReprintInput(oldAcc, _acc);
			}

			Console.WriteLine();

			nativeConsole.InMode = oldInMode;
			nativeConsole.OutMode = oldOutMode;
			return _acc;
		}

		private void ReprintInput(string lastInput, string newInput) {
			int width = Console.BufferWidth;
			int height = Console.BufferHeight;
			Console.SetCursorPosition(_startLeft, _startTop);

			Console.Write(newInput[0.._cursorOffset]);

			(int endCursorLeft, int endCursorTop) = Console.GetCursorPosition();

			if (_cursorOffset <= lastInput.Length) {
				Console.Write(newInput[_cursorOffset..^0]);
			}

			if (newInput.Length < lastInput.Length) {
				for (int i = 0; i < (lastInput.Length - newInput.Length); i++) {
					Console.Write(" ");
				}
			}
			Console.SetCursorPosition(endCursorLeft, endCursorTop);
			_startTop -= Math.Max(((_startLeft + newInput.Length - 1) / width) - (height - _startTop) + 1, 0);
		}
	}
}
