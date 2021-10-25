using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Helpers {
	static class PathHelper {
		public static string GetTargetPath(string path) {
			if (Path.IsPathRooted(path)) {
				return path;
			} else {
				return Path.GetFullPath(Path.Join(Environment.CurrentDirectory, path));
			}
		}
	}
}
