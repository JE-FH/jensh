using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	public class TextOutStream : IJenshOutStream {
		TextWriter _target;
		bool _isNativeConsole;

		public TextOutStream(TextWriter target, bool isNativeConsole = false) {
			_target = target;
			_isNativeConsole = isNativeConsole;
		}

		public bool IsNativeConsole() {
			return _isNativeConsole;
		}

		public void Write(string message) {
			_target.Write(message);
		}

		public void WriteLine(string message) {
			_target.WriteLine(message);
		}
	}
}
