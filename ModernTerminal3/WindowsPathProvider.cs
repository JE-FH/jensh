using System;
using System.Collections.Generic;
using System.IO;
using Mono.Unix;

namespace ModernTerminal3 {
	public class WindowsPathProvider : IPathProvider {
		public IEnumerable<string> GetSearchPaths(string relative_path) {
			string raw_paths = Environment.GetEnvironmentVariable("Path");
			if (raw_paths == null) {
				throw new Exception("Path environment variable is not defined");
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
			string raw_extension = Environment.GetEnvironmentVariable("PATHEXT");
			if (raw_extension == null) {
				throw new Exception("PATHEXT environment variable is not defined");
			}

			return raw_extension.Split(";");
		}

		public bool CanBeExecutable(string path) {
			return File.Exists(path);
		}
	}
}