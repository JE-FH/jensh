using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	internal interface ICommandHandler {
		string CommandName { get; }
		int CommandCalled(string command_name, string[] arguments);
	}
}
