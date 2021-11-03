using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using LibGit2Sharp;

namespace ModernTerminal3 {
	struct ChangesState {
		public int Modified;
		public int NewFiles;
		public int DeletedFiles;
	}

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
			return Repo.Config.Get<string>("remote.origin.url")?.Value;
		}

		public ChangesState GetChanges() {
			ChangesState rv = new ChangesState() {
				Modified = 0,
				NewFiles = 0,
				DeletedFiles = 0,
			};
			foreach (StatusEntry statusEntry in Repo.RetrieveStatus()) {
				switch (statusEntry.State) {
					case FileStatus.DeletedFromWorkdir:
						rv.DeletedFiles++;
						break;
					case FileStatus.RenamedInWorkdir:
					case FileStatus.ModifiedInWorkdir:
						rv.Modified++;
						break;
					case FileStatus.NewInWorkdir:
						rv.NewFiles++;
						break;
				}
			}
			return rv;
		}
	}
}
