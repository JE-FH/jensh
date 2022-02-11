using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class ExternalProgramCommand : ICommandHandler {
		public string CommandName => "EPC";
		private IPathProvider pathProvider;

		public ExternalProgramCommand(IPathProvider pathProvider) {
			this.pathProvider = pathProvider;
		}

		public int CommandCalled(string command_name, string[] arguments) {
			if (arguments.Length < 1) {
				Console.WriteLine("Expected atleast 1 argument");
				return 1;
			}

			List<string> real_arguments = new();
			for (int i = 1; i < arguments.Length; i++) {
				real_arguments.Add(arguments[i]);
			}
			IEnumerable<string> possible_paths = null;

			if (Path.IsPathRooted(arguments[0])) {
				possible_paths = new string[] { arguments[0] };
			} else {
				possible_paths = GetPossiblePaths(arguments[0]);
			}

			var first_executeable = FindFirstExecutable(possible_paths, GetPossibleExtensions());
			if (first_executeable == null) {
				first_executeable = arguments[0];
			}

			var proc = Process.Start(first_executeable, real_arguments);
			Console.CancelKeyPress += Console_IgnoreCancelKey;
			proc.WaitForExit();
			while (Console.KeyAvailable)
				Console.ReadKey(true);
			Console.CancelKeyPress -= Console_IgnoreCancelKey;
			return proc.ExitCode;
		}

		private void Console_IgnoreCancelKey(object sender, ConsoleCancelEventArgs e) {
			e.Cancel = true;
		}

		string FindFirstExecutable(IEnumerable<string> possiblePaths, IEnumerable<string> possibleExtensions) {
			foreach (string path in possiblePaths) {
				if (pathProvider.CanBeExecutable(path)) {
					return path;
				}
				foreach (string extension in possibleExtensions) {
					string full = path + extension;
					if (pathProvider.CanBeExecutable(full)) {
						return path;
					}
				}
			}
			return null;
		}

		IEnumerable<string> GetPossiblePaths(string relativePath) {
			return this.pathProvider.GetSearchPaths(relativePath);
		}

		IEnumerable<string> GetPossibleExtensions() {
			return this.pathProvider.GetExecutableExtensions();
		}
	}
}
