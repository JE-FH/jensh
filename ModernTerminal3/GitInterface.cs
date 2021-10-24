using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using LibGit2Sharp;

namespace ModernTerminal3 {
	class GitInterface : IDisposable {
		Repository Repo;
		public GitInterface(string repositoryPath) {
			Repo = new Repository(repositoryPath);
		}

		public void Dispose() {
			if (Repo != null) {
				Repo.Dispose();
			}
		}

		public string GetBranchName() {
			foreach (var branch in Repo.Branches) {
				if (branch.IsCurrentRepositoryHead) {
					return branch.FriendlyName;
				}
			}
			return null;
		} 

		public string GetRemoteOrigin() {
			return Repo.Config.Get<string>("remote.origin.url").Value;
		}
	}
}
