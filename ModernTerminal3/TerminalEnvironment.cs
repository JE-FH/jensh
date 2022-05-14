using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	public class TerminalEnvironment {
		public IJenshInStream InStream { get; }
		public IJenshOutStream OutStream { get; }
		public IJenshOutStream ErrStream { get; }
		public TerminalEnvironment(IJenshInStream inStream, IJenshOutStream outStream, IJenshOutStream errStream) {
			InStream = inStream;
			OutStream = outStream;
			ErrStream = errStream;
		}
	}
}
