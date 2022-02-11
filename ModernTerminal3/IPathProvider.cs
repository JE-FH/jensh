using System.Collections;
using System.Collections.Generic;

namespace ModernTerminal3 {
	public interface IPathProvider {
		IEnumerable<string> GetSearchPaths(string relative_path);
		IEnumerable<string> GetExecutableExtensions();
		bool CanBeExecutable(string path);
	}
}