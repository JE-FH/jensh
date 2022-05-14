using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	public class TitleCommand : ICommandHandler {
		public string CommandName => "title";
		public TitleCommand() {}

		public int CommandCalled(TerminalEnvironment terminalEnvironment, string command_name, string[] arguments) {
			if (arguments.Length != 1) {
				terminalEnvironment.OutStream.WriteLine("Expected atleast 1 argumnent");
				return 1;
			}

			Console.Title = arguments[0];
			return 0;
		}
	}
}
