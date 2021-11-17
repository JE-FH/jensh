using System;

namespace ModernTerminal3 {
	internal class Program {
		static void Main(string[] args) {
			Terminal terminal = new();
			terminal.AddCommand(new Commands.EchoCommand());
			terminal.AddCommand(new Commands.ExternalProgramCommand());
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
