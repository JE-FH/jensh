using ModernTerminal3.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.WorkEnvironment {
	class GitWorkEnvironment : IWorkEnvironment {
		public GitWorkEnvironment() { }

		public EscapeCodeString GetPromptString() {
			string gitRepoPath = GetGitRepoPath();
			if (gitRepoPath == null) {
				return new EscapeCodeString("");
			}

			using (GitInterface gitInterface = new GitInterface(gitRepoPath)) {
				string branchName = gitInterface.GetBranchName();
				string originUrl;
				string remoteOrigin = gitInterface.GetRemoteOrigin();
				if (remoteOrigin == null) {
					originUrl = "(no remote)";
				} else {
					originUrl = GetRepoName(remoteOrigin);
				}
				if (branchName == null) {
					branchName = "(no branch)";
				}
				ChangesState changes = gitInterface.GetChanges();
				EscapeCodeString changesStr;
				if (changes.DeletedFiles == 0 && changes.NewFiles == 0 && changes.Modified == 0) {
					changesStr = new EscapeCodeString("staged");
				} else {
					changesStr = new EscapeCodeString("") + 
						changes.NewFiles.ToString() + TerminalColors.Bold + TerminalColors.FGGreen + "+ " +
						TerminalColors.Reset + changes.DeletedFiles.ToString() + TerminalColors.Bold + TerminalColors.FGRed + "- " +
						TerminalColors.Reset + changes.Modified.ToString() + TerminalColors.Bold + TerminalColors.FGYellow + "~";
				}
				return TerminalColors.FGBrightMagenta + originUrl + ":" + branchName + " " + TerminalColors.FGBrightWhite + changesStr + TerminalColors.Reset;
			}
		}

		private string GetRepoName(string originUrl) {
			string github = "https://github.com/";
			if (originUrl.StartsWith(github)) {
				return originUrl.Substring(github.Length, originUrl.Length - github.Length - 4);
			} else {
				return originUrl;
			}
		}

		private string GetGitRepoPath() {
			string lastDir = "";
			string dir = Environment.CurrentDirectory;
			while (dir != lastDir) {

				if (Directory.Exists(Path.Combine(dir, ".git"))) {
					return dir;
				}
				lastDir = dir;
				dir = Path.GetFullPath(Path.Combine(dir, ".."));
			}
			return null;
		}
	}
}
