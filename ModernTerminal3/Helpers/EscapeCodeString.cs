using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernTerminal3.Helpers {
	class EscapeCodeString {
		public int RealLength { get; set; }
		public string Data { get; set; }

		public EscapeCodeString(int realLength, string data) {
			RealLength = realLength;
			Data = data;
		}

		public EscapeCodeString(string data) {
			RealLength = data.Length;
			Data = data;
		}

		public static EscapeCodeString operator +(EscapeCodeString a, EscapeCodeString b) {
			return new EscapeCodeString(a.RealLength + b.RealLength, a.Data + b.Data);
		}

		public static EscapeCodeString operator +(EscapeCodeString a, string b) {
			return new EscapeCodeString(a.RealLength + b.Length, a.Data + b);
		}

		public static implicit operator string(EscapeCodeString str) {
			return str.Data;
		}
		public override string ToString() {
			return Data;
		}
	}

	static class TerminalColors {
		public static readonly EscapeCodeString BGBlack = new EscapeCodeString(0, "\x1b[40m");
		public static readonly EscapeCodeString BGRed = new EscapeCodeString(0, "\x1b[41m");
		public static readonly EscapeCodeString BGGreen = new EscapeCodeString(0, "\x1b[42m");
		public static readonly EscapeCodeString BGYellow = new EscapeCodeString(0, "\x1b[43m");
		public static readonly EscapeCodeString BGBlue = new EscapeCodeString(0, "\x1b[44m");
		public static readonly EscapeCodeString BGMagenta = new EscapeCodeString(0, "\x1b[45m");
		public static readonly EscapeCodeString BGCyan = new EscapeCodeString(0, "\x1b[46m");
		public static readonly EscapeCodeString BGWhite = new EscapeCodeString(0, "\x1b[47m");
		public static readonly EscapeCodeString BGBrightBlack = new EscapeCodeString(0, "\x1b[100m");
		public static readonly EscapeCodeString BGBrightRed = new EscapeCodeString(0, "\x1b[101m");
		public static readonly EscapeCodeString BGBrightGreen = new EscapeCodeString(0, "\x1b[102m");
		public static readonly EscapeCodeString BGBrightYellow = new EscapeCodeString(0, "\x1b[103m");
		public static readonly EscapeCodeString BGBrightBlue = new EscapeCodeString(0, "\x1b[104m");
		public static readonly EscapeCodeString BGBrightMagenta = new EscapeCodeString(0, "\x1b[105m");
		public static readonly EscapeCodeString BGBrightCyan = new EscapeCodeString(0, "\x1b[106m");
		public static readonly EscapeCodeString BGBrightWhite = new EscapeCodeString(0, "\x1b[107m");

		public static readonly EscapeCodeString FGBlack = new EscapeCodeString(0, "\x1b[30m");
		public static readonly EscapeCodeString FGRed = new EscapeCodeString(0, "\x1b[31m");
		public static readonly EscapeCodeString FGGreen = new EscapeCodeString(0, "\x1b[32m");
		public static readonly EscapeCodeString FGYellow = new EscapeCodeString(0, "\x1b[33m");
		public static readonly EscapeCodeString FGBlue = new EscapeCodeString(0, "\x1b[34m");
		public static readonly EscapeCodeString FGMagenta = new EscapeCodeString(0, "\x1b[35m");
		public static readonly EscapeCodeString FGCyan = new EscapeCodeString(0, "\x1b[36m");
		public static readonly EscapeCodeString FGWhite = new EscapeCodeString(0, "\x1b[37m");
		public static readonly EscapeCodeString FGBrightBlack = new EscapeCodeString(0, "\x1b[90m");
		public static readonly EscapeCodeString FGBrightRed = new EscapeCodeString(0, "\x1b[91m");
		public static readonly EscapeCodeString FGBrightGreen = new EscapeCodeString(0, "\x1b[92m");
		public static readonly EscapeCodeString FGBrightYellow = new EscapeCodeString(0, "\x1b[93m");
		public static readonly EscapeCodeString FGBrightBlue = new EscapeCodeString(0, "\x1b[94m");
		public static readonly EscapeCodeString FGBrightMagenta = new EscapeCodeString(0, "\x1b[95m");
		public static readonly EscapeCodeString FGBrightCyan = new EscapeCodeString(0, "\x1b[96m");
		public static readonly EscapeCodeString FGBrightWhite = new EscapeCodeString(0, "\x1b[97m");

		public static readonly EscapeCodeString Bold = new EscapeCodeString(0, "\x1b[1m");

		public static readonly EscapeCodeString Reset = new EscapeCodeString(0, "\x1b[0m");
	}
}
