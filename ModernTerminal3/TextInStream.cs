using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	public class TextInStream : IJenshInStream {
		TextReader _target;
		bool _isNativeConsole;

		public TextInStream(TextReader target, bool isNativeConsole = false) {
			_target = target;
			_isNativeConsole = isNativeConsole;
		}

		public bool IsNativeConsole() {
			return _isNativeConsole;
		}

		public string Read(int length) {
			char[] buffer = new char[length];
			_target.Read(buffer, 0, length);
			return new string(buffer);
		}

		public string ReadLine() {
			return _target.ReadLine();
		}

		public string ReadToEnd() {
			return _target.ReadToEnd();
		}
	}
}
