using System;
using System.Runtime.InteropServices;

namespace ModernTerminal3 {
	internal class Program {
		static void Main(string[] args) {
			INativeConsole nativeConsole;
			IPathProvider pathProvider;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				nativeConsole = new LinuxNativeConsole();
				pathProvider = new LinuxPathProvider();
			} else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				nativeConsole = new WindowsNativeConsole();
				pathProvider = new WindowsPathProvider();
			} else {
				Console.WriteLine("Unsupported platform");
				return;
			}
			
			var terminal = new Terminal(nativeConsole);

			terminal.DefaultCommandHandler = new Commands.ExternalProgramCommand(pathProvider);

			terminal.AddCommand(new Commands.EchoCommand());
			terminal.AddCommand(new Commands.ListDirCommand());
			terminal.AddCommand(new Commands.ChangeDirectoryCommand());
			terminal.AddCommand(new Commands.MakeDirectoryCommand());
			terminal.AddCommand(new Commands.ExplainCommand());
			terminal.AddCommand(new Commands.DelCommand());
			terminal.AddCommand(new Commands.TitleCommand());
			terminal.AddWorkEnvironment(new WorkEnvironment.GitWorkEnvironment());
			terminal.Run();
		}
	}
}
