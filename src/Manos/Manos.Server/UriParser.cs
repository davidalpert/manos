

using System;

using System.Text;

namespace Manos.Server {

	public static class UriParser {

		public static bool TryParse (string uri, out string scheme, out string host, out string path, out string query)
		{
			int end;

			uri = HttpUtility.UrlDecode (uri, Encoding.Default);
			
			if (String.IsNullOrEmpty (uri)) {
				scheme = null;
				path = null;
				query = null;
				host = null;
				return false;
			}

			path = null;
			query = null;
			host = null;

			return TryParseScheme (uri, out scheme, out end) &&
			       TryParseHost (uri, end, out host, out end) &&
			       TryParsePath (uri, end, out path, out end) &&
			       TryParseQuery (uri, end, out query);
		}

		public static bool TryParseScheme (string uri, out string scheme, out int end)
		{
			end = uri.IndexOf ("://", StringComparison.InvariantCulture);
			if (end == -1) {
				scheme = "http";
				end = 0;
			} else {
				scheme = uri.Substring (0, end);
				end += 3; // move past the ://
			}

			return true;
		}

		public static bool TryParseHost (string uri, int start, out string host, out int end)
		{
			if (start >= uri.Length) {
				host = null;
				end = -1;
				return false;
			}

			end = uri.IndexOf ('/', start);
			if (end == -1) {
				host = uri.Substring (start);
				end = uri.Length;
			} else if (end <= start) {
				host = null;
				end = start;
			} else {
				host = uri.Substring (start, end - start);
			}

			return true;
		}
		
		public static bool TryParsePath (string uri, int start, out string path, out int end)
		{
			if (start >= uri.Length) {
				path = "/";
				end = start;
				return true;
			}

			end = uri.IndexOf ('?', start);
			if (end == -1) {
				path = uri.Substring (start, uri.Length - start);
				end = uri.Length;
			} else {
				path = uri.Substring (start, end - start);
				++end; // advance past the ?
			}
			
			return true;
		}

		public static bool TryParseQuery (string uri, int start, out string query)
		{
			if (start >= uri.Length) {
				// Perfectly legal to not have a query
				query = String.Empty;
				return true;
			}

			int end = uri.IndexOf ('#', start);
			if (end == -1)
				end = uri.Length;

			query = uri.Substring (start, end - start);
			return true;
		}
	}
}

