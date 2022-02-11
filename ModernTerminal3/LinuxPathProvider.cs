using System;
using System.Collections.Generic;
using System.IO;
using Mono.Unix;

namespace ModernTerminal3 {
	public class LinuxPathProvider : IPathProvider {
		private uint uid;
		private uint gid;
		
		public LinuxPathProvider() {
			uid = Mono.Unix.Native.Syscall.geteuid();
			gid = Mono.Unix.Native.Syscall.getegid();
		}
		
		public IEnumerable<string> GetSearchPaths(string relative_path) {
			string raw_paths = Environment.GetEnvironmentVariable("PATH");
			if (raw_paths == null) {
				throw new Exception("PATH environment variable is not defined");
			}

			var paths = raw_paths.Split(Path.PathSeparator);
			List<string> rv = new();
			rv.Add(relative_path);
			foreach (string path in paths) {
				rv.Add(Path.Join(path, relative_path));
			}
			return rv.ToArray();
		}

		public IEnumerable<string> GetExecutableExtensions() {
			return new string[] { };
		}

		public bool CanBeExecutable(string path) {
			var thing = new Mono.Unix.UnixFileInfo(path);
			if (!thing.Exists) {
				return false;
			}
			var perms = thing.FileAccessPermissions;
			return (
				       ((perms & FileAccessPermissions.OtherExecute) != 0) ||
				       ((perms & FileAccessPermissions.UserExecute) != 0 && thing.OwnerUserId == uid) ||
				       ((perms & FileAccessPermissions.GroupExecute) != 0 && thing.OwnerGroupId == gid)
			       );
		}
	}
}