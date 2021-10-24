using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class ExplainCommand : ICommandHandler {
		public string CommandName => "explain";

		public ExplainCommand() {}

		public int CommandCalled(string command_name, string[] arguments) {
			Console.WriteLine($"Got {arguments.Length} arguments");
			foreach (var argument in arguments) {
				Console.WriteLine(argument);
			}
			return 0;
		}
	}
}
