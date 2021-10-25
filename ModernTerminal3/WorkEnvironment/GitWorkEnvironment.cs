﻿using ModernTerminal3.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.WorkEnvironment {
	class GitWorkEnvironment : IWorkEnvironment {
		public GitWorkEnvironment() {}

		public EscapeCodeString GetPromptString() {
			string gitRepoPath = GetGitRepoPath();
			if (gitRepoPath == null) {
				return new EscapeCodeString("");
			}

			using (GitInterface gitInterface = new GitInterface(gitRepoPath)) {
				string branchName = gitInterface.GetBranchName();
				string originUrl = GetRepoName(gitInterface.GetRemoteOrigin());
				return TerminalColors.FGRed + "(" + originUrl + ":" + branchName + ")" + TerminalColors.Reset;
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