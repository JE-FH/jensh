using System;

namespace ModernTerminal3
{
	[Flags]
	public enum ConsoleInMode {
		None = 0,
		ProcessedInput = 1,
		EchoInput = 2,
	}

	[Flags]
	public enum ConsoleOutMode {
		None = 0,
		ProcessedOutput = 1,
		WrapAtEolOutput = 2,
		EnableEscapeSequences = 4
	}
	
	interface INativeConsole {
		void SetOutputCodePage(uint codePage);
		ConsoleOutMode OutMode { get; set; }
		ConsoleInMode InMode { get; set; }
	}
}