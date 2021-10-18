using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace ModernTerminal3 {
	internal class Terminal {
		Dictionary<string, ICommandHandler> commands;
		public Terminal() {
			commands = new Dictionary<string, ICommandHandler>();
		}

		public void Run() {
			while (true) {
				Console.Write($"\x1B[92m{Environment.UserName}@{Environment.MachineName}\x1B[0m \x1B[94m{Directory.GetCurrentDirectory()}\x1B[0m$ ");
				var input = Console.ReadLine();
				var parsed_input = split_commands(input);
				if (parsed_input.Length > 0) {
					HandleCommand(parsed_input[0], parsed_input);
				}
			}
		}

		int HandleCommand(string command_name, string[] args) {
			bool found = commands.TryGetValue(command_name, out ICommandHandler handler);
			try {
				if (found) {
					return handler.CommandCalled(args);
				} else {
					return CallExternalProgram(command_name, args);
				}
			} catch (Win32Exception e) {
				Console.WriteLine($"({e.NativeErrorCode}) {e.Message}");
				return 1;
			}
		}
		int CallExternalProgram(string command_name, string[] args) {
			var proc = Process.Start(command_name, args);
			proc.WaitForExit();
			return proc.ExitCode;
		}

		string[] split_commands(string command_input) {
			bool escaped = false;
			bool in_string = false;
			string acc = "";
			List<string> rv = new();
			foreach (char c in command_input) {
				if (escaped) {
					acc += c;
					escaped = false;
				} else {
					if (c == '"') {
						if (in_string) {
							in_string = false;
						} else {
							in_string = true;
						}
					} else if (c == ' ' && !in_string) {
						if (acc.Length > 0) {
							rv.Add(acc);
							acc = "";
						}
					} else if (c == '\\') {
						escaped = true;
					} else {
						acc += c;
					}
				}
			}
			if (acc.Length > 0) {
				rv.Add(acc);
			}
			return rv.ToArray();
		}
	}
}
