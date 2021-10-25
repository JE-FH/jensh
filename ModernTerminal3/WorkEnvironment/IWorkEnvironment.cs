using ModernTerminal3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.WorkEnvironment {
	interface IWorkEnvironment {
		EscapeCodeString GetPromptString();
	}
}
