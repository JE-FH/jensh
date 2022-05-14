using ModernTerminal3.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class DelCommand : ICommandHandler {
		public string CommandName => "rm";

		public DelCommand() {

		}


		public int CommandCalled(TerminalEnvironment terminalEnvironment, string command_name, string[] _arguments) {
			List<string> arguments = new List<string>(_arguments);

			HashSet<char> switches = new HashSet<char>();

			for (int i = arguments.Count - 1; i >= 0; i--) {
				string argument = arguments[i];
				if (argument.StartsWith("-")) {
					for (int j = 1; j < argument.Length; j++) {
						switches.Add(argument[j]);
					}
					arguments.RemoveAt(i);
				}
			}

			if (arguments.Count < 1) {
				terminalEnvironment.ErrStream.WriteLine("No targets specified");
				return 1;
			}

			string targetPath = PathHelper.GetTargetPath(arguments[0]);
			try {
				if (switches.Contains('f')) {
					Directory.Delete(targetPath);
				} else {
					File.Delete(targetPath);
				}
			} catch (Exception ex) {
				terminalEnvironment.ErrStream.WriteLine(ex.Message);
				return 1;
			}
			return 0;
		}
	}
}
