using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using ModernTerminal3.Helpers;
using ModernTerminal3.WorkEnvironment;
using System.Runtime.InteropServices;

namespace ModernTerminal3 {
	class CommandSplitInfo {
		public string ParsedData;
		public string RawData;
		public int StartIndex;
		public int EndIndex;
	}

	internal class Terminal : ITerminalReaderInfoProvider {
		Dictionary<string, ICommandHandler> commands;
		List<IWorkEnvironment> WorkEnvironments;
		HashSet<char> escapeableCharacters;
		List<string> _commandHistory;
		public ICommandHandler DefaultCommandHandler { get; set; }

		public Terminal() {
			commands = new Dictionary<string, ICommandHandler>();
			WorkEnvironments = new List<IWorkEnvironment>();
			escapeableCharacters = new HashSet<char>();
			escapeableCharacters.Add('"');
			escapeableCharacters.Add('\\');
			escapeableCharacters.Add(' ');
			_commandHistory = new List<string>();
		}

		public void Run() {
			while (true) {
				//Make sure that the console mode is correct and hasnt been changed by a command
				SetCorrectConsoleMode();

				PrintPrompt();

				TerminalReader commandLineReader = new TerminalReader(this);
				var input = commandLineReader.ReadLine();


				var parsed_input = SplitCommand(input);
				var finished_parts = ExtractAndFinishCommandParts(parsed_input);

				if (parsed_input.Length > 0) {
					HandleCommand(finished_parts[0], finished_parts.Skip(1).ToArray());
					if (
						_commandHistory.Count == 0 ||
						_commandHistory[^1] != input
					) {
						_commandHistory.Add(input);
					}
				}
			}
		}

		private void SetCorrectConsoleMode() {
			NativeConsoleOperation.OutConsoleMode = ConsoleInMode.ENABLE_PROCESSED_INPUT | ConsoleInMode.ENABLE_ECHO_INPUT;
			NativeConsoleOperation.SetOutputConsoleCP(65001);
		}

		private void PrintPrompt() {

			EscapeCodeString workEnvironmentString = GetWorkEnvironmentString();
			if (workEnvironmentString.RealLength == 0) {
				Console.Write($"{GetIdentityString()} {GetLocationString()}$ ");
			} else {
				Console.Write($"{workEnvironmentString} {GetLocationString()}$ ");
			}
		}

		private EscapeCodeString GetWorkEnvironmentString() {
			foreach (IWorkEnvironment workEnvironemnt in WorkEnvironments) {
				EscapeCodeString thing = workEnvironemnt.GetPromptString();
				if (thing != null) {
					return thing;
				}
			}
			return null;
		}

		private EscapeCodeString GetIdentityString() {
			return TerminalColors.FGBrightGreen + Environment.UserName + "@" + Environment.MachineName + TerminalColors.Reset;
		}

		private EscapeCodeString GetLocationString() {
			var currentDirectory = Directory.GetCurrentDirectory();
			var home_directory = GetVarValue("USERPROFILE");
			if (home_directory != null) {
				currentDirectory = currentDirectory.Replace(home_directory, "~");
			}
			return TerminalColors.FGBrightBlue + currentDirectory + TerminalColors.Reset;
		}

		public void AddCommand(ICommandHandler command) {
			commands.Add(command.CommandName, command);
		}

		public void AddWorkEnvironment(IWorkEnvironment workEnvironment) {
			WorkEnvironments.Add(workEnvironment);
		}

		int HandleCommand(string command_name, string[] args) {
			bool found = commands.TryGetValue(command_name, out ICommandHandler handler);
			try {
				if (found) {
					return handler.CommandCalled(command_name, args);
				} else {
					return CallExternalProgram(command_name, args);
				}
			} catch (Win32Exception e) {
				Console.WriteLine($"({e.NativeErrorCode}) {e.Message}");
				return 1;
			}
		}
		int CallExternalProgram(string command_name, string[] args) {
			return DefaultCommandHandler.CommandCalled(command_name, (new string[] { command_name }).Concat(args).ToArray());
		}

		CommandSplitInfo[] SplitCommand(string command_input) {
			bool escaped = false;
			bool in_string = false;
			string acc = "";
			string rawAcc = "";
			int startIndex = 0;
			List<CommandSplitInfo> rv = new();
			for (int i = 0; i < command_input.Length; i++) {
				char c = command_input[i];
				if (escaped) {
					rawAcc += c;
					if (escapeableCharacters.Contains(c)) {
						acc += c;
					} else {
						acc += '\\';
						acc += c;
					}
					escaped = false;
				} else {
					if (c == '"') {
						rawAcc += c;
						if (in_string) {
							in_string = false;
						} else {
							in_string = true;
						}
					} else if (c == ' ' && !in_string) {
						if (rawAcc.Length > 0) {
							rv.Add(new CommandSplitInfo() {
								ParsedData = acc,
								RawData = rawAcc,
								StartIndex = startIndex,
								EndIndex = i
							});
							startIndex = i + 1;
							acc = "";
							rawAcc = "";
						}
					} else if (c == '\\') {
						rawAcc += c;
						escaped = true;
					} else {
						rawAcc += c;
						acc += c;
					}
				}
			}
			if (rawAcc.Length > 0) {
				rv.Add(new CommandSplitInfo() {
					ParsedData = acc,
					RawData = rawAcc,
					StartIndex = startIndex,
					EndIndex = command_input.Length
				});
			}
			return rv.ToArray();
		}

		string[] ExtractAndFinishCommandParts(IEnumerable<CommandSplitInfo> commandSplitInfo) {
			//This might make it possible to get environment variables by %"varname"% so %"APPDATA"% would work
			return commandSplitInfo
				.Where((info) => info.ParsedData.Length > 0)
				.Select((info) => ReplaceVariables(info.ParsedData)).ToArray();
		}

		string ReplaceVariables(string arg_str) {
			string rv = "";
			string acc = "";
			bool inside_var = false;
			foreach (var c in arg_str) {
				if (inside_var) {
					if (c == '%') {
						inside_var = false;
						var var_val = GetVarValue(acc);
						//TODO: Maybe this should throw an exception instead of acting like nothing went wrong
						if (var_val != null) {
							rv += var_val;
						} else {
							rv += "%" + acc + "%";
						}
						acc = "";
					} else {
						acc += c;
					}
				} else {
					if (c == '%') {
						inside_var = true;
					} else if (c == '~') {
						var var_val = GetVarValue("USERPROFILE");
						if (var_val != null) {
							rv += var_val;
						} else {
							rv += "~";
						}
					} else {
						rv += c;
					}
				}
			}
			if (inside_var) {
				//TODO: Should this throw an exception
				rv += "%" + acc;
			}
			return rv;
		}


		string GetVarValue(string var_name) {
			return Environment.GetEnvironmentVariable(var_name);
		}

		public (string newData, int newOffset) TabComplete(string currentInput, int offset) {
			CommandSplitInfo[] commandInfo = SplitCommand(currentInput);

			CommandSplitInfo selectedPart = null;
			int selectedPartOffset = 0;


			int currentOffset = 0;
			foreach (CommandSplitInfo part in commandInfo) {
				if (part.StartIndex <= offset && part.EndIndex >= offset) {
					selectedPart = part;
					selectedPartOffset = offset - part.StartIndex;
					break;
				}
				currentOffset = part.EndIndex;
			}
			if (selectedPart == null) {
				return (null, offset);
			}

			string realPath = MakePathAbsolute(ReplaceVariables(selectedPart.ParsedData));
			int newOffset = 0;
			if (Path.EndsInDirectorySeparator(realPath) && Directory.Exists(realPath)) {
				string[] result = Directory.EnumerateFileSystemEntries(realPath).Take(1).ToArray();
				if (result.Length == 0) {
					return (null, offset);
				}

				string joiningPath = Path.GetFileName(result[0]);
				if (Directory.Exists(result[0])) {
					joiningPath += "/";
				}

				selectedPart.RawData = Path.Join(selectedPart.ParsedData, joiningPath);
				newOffset = selectedPart.StartIndex + selectedPart.RawData.Length;
			} else if (Directory.Exists(Path.Join(realPath, ".."))) {
				var searchDir = Path.Join(realPath, "..");
				var pattern = Path.GetFileName(realPath) + "*";
				var result = Directory.GetFileSystemEntries(searchDir, pattern).Take(1).ToArray();

				if (result.Length == 0) {
					//TODO: add bell
					return (null, offset);
				}

				string joiningPath = Path.GetFileName(result[0]);
				if (Directory.Exists(result[0])) {
					joiningPath += "/";
				}

				selectedPart.RawData = selectedPart.ParsedData + joiningPath[Path.GetFileName(realPath).Length..^0];
				newOffset = selectedPart.StartIndex + selectedPart.RawData.Length;
			} else {
				return (null, offset);
			}

			return (string.Join(" ", commandInfo.Select((info) => info.RawData)), newOffset);
		}

		string MakePathAbsolute(string givenPath) {
			if (Path.IsPathRooted(givenPath)) {
				return givenPath;
			} else {
				return Path.GetFullPath(Path.Join(Environment.CurrentDirectory, givenPath));
			}
		}

		public string GetLastCommand(int offset) {
			if (_commandHistory.Count + offset >= 0 && _commandHistory.Count + offset + offset < _commandHistory.Count) {
				return _commandHistory[_commandHistory.Count + offset];
			}
			return null;
		}
	}
}
