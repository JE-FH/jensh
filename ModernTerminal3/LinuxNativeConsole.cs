namespace ModernTerminal3 {
	public class LinuxNativeConsole : INativeConsole {
		public void SetOutputCodePage(uint codePage) {
			return;
		}

		public ConsoleOutMode OutMode { get; set; }
		public ConsoleInMode InMode { get; set; }
	}
}