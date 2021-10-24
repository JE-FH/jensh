using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	public class ChangeDirectoryCommand : ICommandHandler {
		public string CommandName => "cd";

		public ChangeDirectoryCommand() {

		}

		public int CommandCalled(string command_name, string[] arguments) {
			if (arguments.Length < 1) {
				Console.WriteLine("Expected atleast 1 argument");
				return 1;
			}

			try {
				if (Path.IsPathRooted(arguments[0])) {
					Environment.CurrentDirectory = arguments[0];
				} else {
					Environment.CurrentDirectory = Path.GetFullPath(Path.Join(Environment.CurrentDirectory, arguments[0]));
				}
				return 0;
			} catch (Exception ex) {
				Console.WriteLine(ex.Message);
				return 1;
			}
		}
	}
}
