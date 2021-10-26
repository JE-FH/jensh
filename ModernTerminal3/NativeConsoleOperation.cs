using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	static class ConsoleInMode {
		public const int ENABLE_PROCESSED_INPUT = 0x1;
		public const int ENABLE_LINE_INPUT = 0x2;
		public const int ENABLE_ECHO_INPUT = 0x4;
		public const int ENABLE_WINDOW_INPUT = 0x8;
		public const int ENABLE_MOUSE_INPUT = 0x10;
		public const int ENABLE_INSERT_MODE = 0x20;
		public const int ENABLE_QUICK_EDIT_MODE = 0x40;
		public const int ENABLE_VIRTUAL_TERMINAL_INPUT = 0x200;
	}

	static class ConsoleOutMode {
		public const int ENABLE_PROCESSED_OUTPUT = 0x1;
		public const int ENABLE_WRAP_AT_EOL_OUTPUT = 0x2;
		public const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x4;
		public const int DISABLE_NEWLINE_AUTO_RETURN = 0x8;
		public const int ENABLE_LVB_GRID_WORLDWIDE = 0x10;
	}

	static class NativeConsoleOperation {

		[DllImport("kernel32.dll", EntryPoint = "SetConsoleMode")]
		private static extern void NativeSetConsoleMode(IntPtr hConsoleHandle, int dwMode);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", EntryPoint = "GetConsoleMode")]
		private unsafe static extern bool NativeGetConsoleMode(IntPtr hConsoleHandle, int* dwMode);

		private static IntPtr _outHandle = IntPtr.Zero;
		private static IntPtr _inHandle = IntPtr.Zero;

		private static IntPtr OutHandle {
			get {
				if (_outHandle == IntPtr.Zero) {
					_outHandle = GetStdHandle(-11);
				}
				return _outHandle;
			}
		}

		private static IntPtr InHandle {
			get {
				if (_inHandle == IntPtr.Zero) {
					_inHandle = GetStdHandle(-10);
				}
				return _inHandle;
			}
		}


		private static int GetConsoleModeWrapper(IntPtr hConsoleHandle) {
			unsafe {
				int rv;
				bool success = NativeGetConsoleMode(hConsoleHandle, &rv);

				if (!success) {
					throw new Exception("Could not get console mode");
				}

				return rv;
			}
		}

		public static int OutConsoleMode {
			get {
				return GetConsoleModeWrapper(OutHandle);
			}
			set {
				NativeSetConsoleMode(OutHandle, value);
			}
		}

		public static int InConsoleMode {
			get {
				return GetConsoleModeWrapper(InHandle);
			}
			set {
				NativeSetConsoleMode(InHandle, value);
			}
		}

	}
}
