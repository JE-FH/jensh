using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3
{
	internal interface ICommandHandler {
		string CommandName {  get; set; }
		int CommandCalled(string[] arguments);
	}
}
