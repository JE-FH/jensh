using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Commands {
	class EchoCommand : ICommandHandler {
		public string CommandName { get => "echo"; }

		public EchoCommand() { }

		public int CommandCalled(string command_name, string[] arguments) {
			for (int i = 0; i < arguments.Length; i++) {
				Console.Write(arguments[i]);
				if (i < arguments.Length - 1) {
					Console.Write(" ");
				}
			}
			Console.WriteLine();
			return 0;
		}
	}
}
