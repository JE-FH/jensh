using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3 {
	internal static class Win32ConsoleInMode {
		public const int ENABLE_PROCESSED_INPUT = 0x1;
		public const int ENABLE_LINE_INPUT = 0x2;
		public const int ENABLE_ECHO_INPUT = 0x4;
		public const int ENABLE_WINDOW_INPUT = 0x8;
		public const int ENABLE_MOUSE_INPUT = 0x10;
		public const int ENABLE_INSERT_MODE = 0x20;
		public const int ENABLE_QUICK_EDIT_MODE = 0x40;
		public const int ENABLE_VIRTUAL_TERMINAL_INPUT = 0x200;
	}

	internal static class Win32ConsoleOutMode {
		public const int ENABLE_PROCESSED_OUTPUT = 0x1;
		public const int ENABLE_WRAP_AT_EOL_OUTPUT = 0x2;
		public const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x4;
		public const int DISABLE_NEWLINE_AUTO_RETURN = 0x8;
		public const int ENABLE_LVB_GRID_WORLDWIDE = 0x10;
	}
	
	class WindowsNativeConsole : INativeConsole {

		[DllImport("kernel32.dll", EntryPoint = "SetConsoleMode")]
		private static extern void NativeSetConsoleMode(IntPtr hConsoleHandle, int dwMode);

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("kernel32.dll", EntryPoint = "GetConsoleMode")]
		private unsafe static extern bool NativeGetConsoleMode(IntPtr hConsoleHandle, int* dwMode);

		[DllImport("kernel32.dll", EntryPoint = "SetConsoleOutputCP")]
		private static extern bool SetOutputConsoleCP(uint wCodePageID);

		private IntPtr _outHandle = IntPtr.Zero;
		private IntPtr _inHandle = IntPtr.Zero;

		private IntPtr OutHandle {
			get {
				if (_outHandle == IntPtr.Zero) {
					_outHandle = GetStdHandle(-11);
				}
				return _outHandle;
			}
		}

		private IntPtr InHandle {
			get {
				if (_inHandle == IntPtr.Zero) {
					_inHandle = GetStdHandle(-10);
				}
				return _inHandle;
			}
		}


		private int GetConsoleModeWrapper(IntPtr hConsoleHandle) {
			unsafe {
				int rv;
				bool success = NativeGetConsoleMode(hConsoleHandle, &rv);

				if (!success) {
					throw new Exception("Could not get console mode");
				}

				return rv;
			}
		}

		private int OutConsoleMode {
			get {
				return GetConsoleModeWrapper(OutHandle);
			}
			set {
				NativeSetConsoleMode(OutHandle, value);
			}
		}

		private int InConsoleMode {
			get {
				return GetConsoleModeWrapper(InHandle);
			}
			set {
				NativeSetConsoleMode(InHandle, value);
			}
		}

		public void SetOutputCodePage(uint codePage) {
			SetOutputConsoleCP(codePage);
		}

		public ConsoleOutMode OutMode {
			get {
				int nativeSetting = OutConsoleMode;
				ConsoleOutMode rv = ConsoleOutMode.None;
				if ((nativeSetting & Win32ConsoleOutMode.ENABLE_PROCESSED_OUTPUT) != 0)
					rv |= ConsoleOutMode.ProcessedOutput;
				if ((nativeSetting & Win32ConsoleOutMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING) != 0)
					rv |= ConsoleOutMode.EnableEscapeSequences;
				if ((nativeSetting & Win32ConsoleOutMode.ENABLE_WRAP_AT_EOL_OUTPUT) != 0)
					rv |= ConsoleOutMode.WrapAtEolOutput;
				return rv;
			}
			set {
				int nativeValue = 0;
				if ((value & ConsoleOutMode.ProcessedOutput) != 0)
					nativeValue |= Win32ConsoleOutMode.ENABLE_PROCESSED_OUTPUT;
				if ((value & ConsoleOutMode.EnableEscapeSequences) != 0)
					nativeValue |= Win32ConsoleOutMode.ENABLE_VIRTUAL_TERMINAL_PROCESSING;
				if ((value & ConsoleOutMode.WrapAtEolOutput) != 0)
					nativeValue |= Win32ConsoleOutMode.ENABLE_WRAP_AT_EOL_OUTPUT;
				OutConsoleMode = nativeValue;
			}
		}

		public ConsoleInMode InMode {
			get {
				int nativeSetting = InConsoleMode;
				ConsoleInMode rv = ConsoleInMode.None;
				if ((nativeSetting & Win32ConsoleInMode.ENABLE_PROCESSED_INPUT) != 0)
					rv |= ConsoleInMode.ProcessedInput;
				if ((nativeSetting & Win32ConsoleInMode.ENABLE_ECHO_INPUT) != 0)
					rv |= ConsoleInMode.EchoInput;
				return rv;
			} 
			set {
				int nativeValue = 0;
				if ((value & ConsoleInMode.EchoInput) != 0)
					nativeValue |= Win32ConsoleInMode.ENABLE_ECHO_INPUT;
				if ((value & ConsoleInMode.ProcessedInput) != 0)
					nativeValue |= Win32ConsoleInMode.ENABLE_PROCESSED_INPUT;
				InConsoleMode = nativeValue;
			}
		}
	}
}
