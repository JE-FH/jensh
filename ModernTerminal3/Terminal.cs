﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using ModernTerminal3.Helpers;

namespace ModernTerminal3 {
	internal class Terminal {
		Dictionary<string, ICommandHandler> commands;
		HashSet<char> escapeableCharacters;
		public Terminal() {
			commands = new Dictionary<string, ICommandHandler>();
			escapeableCharacters = new HashSet<char>();
			escapeableCharacters.Add('"');
			escapeableCharacters.Add('\\');
			escapeableCharacters.Add(' ');
		}

		public void Run() {
			while (true) {
				PrintPrompt();
				var input = Console.ReadLine();
				var parsed_input = SplitCommand(input);
				for (int i = 0; i < parsed_input.Length; i++) {
					parsed_input[i] = ReplaceVariables(parsed_input[i]);
				}

				if (parsed_input.Length > 0) {
					HandleCommand(parsed_input[0], parsed_input.AsSpan(1).ToArray());
				}
			}
		}

		private void PrintPrompt() {
			Console.Write($"{GetIdentityString()} {GetLocationString()}");

			EscapeCodeString workEnvironmentString = GetWorkEnvironmentString();
			if (workEnvironmentString.RealLength == 0) {
				Console.Write("$ ");
			} else {
				Console.Write($" {workEnvironmentString}$ ");
			}
		}

		private EscapeCodeString GetIdentityString() {
			return TerminalColors.FGBrightGreen + Environment.UserName + "@" + Environment.MachineName + TerminalColors.Reset;
		}

		private EscapeCodeString GetLocationString() {
			return TerminalColors.FGBrightBlue + Directory.GetCurrentDirectory() + TerminalColors.Reset;
		}

		private EscapeCodeString GetWorkEnvironmentString() {
			string gitRepoPath = GetGitRepoPath();
			if (gitRepoPath == null) {
				return new EscapeCodeString("");
			}
			
			using (GitInterface gitInterface = new GitInterface(gitRepoPath)) {
				string branchName = gitInterface.GetBranchName();
				string originUrl = GetRepoName(gitInterface.GetRemoteOrigin());
				return TerminalColors.FGRed + "(" + originUrl + ":" + branchName + ")" + TerminalColors.Reset;
			}
		}

		private string GetRepoName(string originUrl) {
			string github = "https://github.com/";
			if (originUrl.StartsWith(github)) {
				return originUrl.Substring(github.Length, originUrl.Length - github.Length - 4);
			} else {
				return originUrl;
			}
		}

		private string GetGitRepoPath() {
			string lastDir = "";
			string dir = Environment.CurrentDirectory;
			while (dir != lastDir) {

				if (Directory.Exists(Path.Combine(dir, ".git"))) {
					return dir;
				}
				lastDir = dir;
				dir = Path.GetFullPath(Path.Combine(dir, ".."));
			}
			return null;
		}

		public void AddCommand(ICommandHandler command) {
			commands.Add(command.CommandName, command);
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
			var proc = Process.Start(command_name, args);
			proc.WaitForExit();
			return proc.ExitCode;
		}

		string[] SplitCommand(string command_input) {
			bool escaped = false;
			bool in_string = false;
			string acc = "";
			List<string> rv = new();
			foreach (char c in command_input) {
				if (escaped) {
					if (escapeableCharacters.Contains(c)) {
						acc += c;
					} else {
						acc += '\\';
						acc += c;
					}
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
	}
}
