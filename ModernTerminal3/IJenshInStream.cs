using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	public interface IJenshInStream {
		string ReadLine();
		string ReadToEnd();
		string Read(int length);
		bool IsNativeConsole();
	}
}
