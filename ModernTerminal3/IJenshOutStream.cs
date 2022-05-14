using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	public interface IJenshOutStream {
		void Write(string message);
		void WriteLine(string message);
		bool IsNativeConsole();
	}
}
