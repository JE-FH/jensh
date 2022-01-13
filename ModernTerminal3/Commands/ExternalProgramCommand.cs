using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class ExternalProgramCommand : ICommandHandler {
		public string CommandName => "EPC";

		public ExternalProgramCommand() { }

		public int CommandCalled(string command_name, string[] arguments) {
			if (arguments.Length < 1) {
				Console.WriteLine("Expected atleast 1 argument");
				return 1;
			}

			List<string> real_arguments = new();
			for (int i = 1; i < arguments.Length; i++) {
				real_arguments.Add(arguments[i]);
			}
			string[] possible_paths = null;

			if (Path.IsPathRooted(arguments[0])) {
				possible_paths = new string[] { arguments[0] };
			} else {
				possible_paths = GetPossiblePaths(arguments[0]);
			}

			var first_executeable = FindFirstExecuteable(possible_paths, GetPossibleExtensions());
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

		string FindFirstExecuteable(string[] possible_paths, string[] possible_extensions) {
			foreach (string path in possible_paths) {
				if (File.Exists(path)) {
					return path;
				}
				foreach (string extension in possible_extensions) {
					string full = path + extension;
					if (File.Exists(full)) {
						return path;
					}
				}
			}
			return null;
		}

		string[] GetPossiblePaths(string relative_path) {
			//Maybe this should be cached since environment variables dont change
			string raw_paths = Environment.GetEnvironmentVariable("Path");
			if (raw_paths == null) {
				throw new Exception("Path environment variable is not defined");
			}

			var paths = raw_paths.Split(Path.PathSeparator);
			List<string> rv = new();
			rv.Add(relative_path);
			foreach (string path in paths) {
				rv.Add(Path.Join(path, relative_path));
			}
			return rv.ToArray();
		}

		string[] GetPossibleExtensions() {
			string raw_extension = Environment.GetEnvironmentVariable("PATHEXT");
			if (raw_extension == null) {
				throw new Exception("PATHEXT environment variable is not defined");
			}

			return raw_extension.Split(";");
		}
	}
}
