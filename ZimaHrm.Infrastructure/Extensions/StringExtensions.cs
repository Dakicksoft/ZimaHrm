using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool IsPrivateNetwork(this string ip)
        {
            if (String.IsNullOrEmpty(ip))
                return false;

            if (String.Equals(ip, "::1") || String.Equals(ip, "127.0.0.1"))
                return true;

            // 10.0.0.0 – 10.255.255.255 (Class A)
            if (ip.StartsWith("10."))
                return true;

            // 172.16.0.0 – 172.31.255.255 (Class B)
            if (ip.StartsWith("172."))
            {
                for (var range = 16; range < 32; range++)
                {
                    if (ip.StartsWith("172." + range + "."))
                        return true;
                }
            }

            // 192.168.0.0 – 192.168.255.255 (Class C)
            return ip.StartsWith("192.168.");
        }


        public static bool IsValidIdentifier(this string value)
        {
            if (value == null)
                return false;

            for (int index = 0; index < value.Length; index++)
            {
                if (!Char.IsLetterOrDigit(value[index]) && value[index] != '-')
                    return false;
            }

            return true;
        }

        public static string ToDelimitedString(this IEnumerable<string> values, string delimiter = ",")
        {
            if (string.IsNullOrEmpty(delimiter))
                delimiter = ",";

            var sb = new StringBuilder();
            foreach (var i in values)
            {
                if (sb.Length > 0)
                    sb.Append(delimiter);

                sb.Append(i);
            }

            return sb.ToString();
        }

        public static string[] FromDelimitedString(this string value, string delimiter = ",")
        {
            if (String.IsNullOrEmpty(value))
                return null;

            if (String.IsNullOrEmpty(delimiter))
                delimiter = ",";

            return value.Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        public static string ToLowerUnderscoredWords(this string value)
        {
            var builder = new StringBuilder(value.Length + 10);
            for (int index = 0; index < value.Length; index++)
            {
                char c = value[index];
                if (Char.IsUpper(c))
                {
                    if (index > 0 && value[index - 1] != '_')
                        builder.Append('_');

                    builder.Append(Char.ToLower(c));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static bool AnyWildcardMatches(this string value, IEnumerable<string> patternsToMatch, bool ignoreCase = false)
        {
            if (ignoreCase)
                value = value.ToLower();

            return patternsToMatch.Any(pattern => CheckForMatch(pattern, value, ignoreCase));
        }

        private static bool CheckForMatch(string pattern, string value, bool ignoreCase = true)
        {
            bool startsWithWildcard = pattern.StartsWith("*");
            if (startsWithWildcard)
                pattern = pattern.Substring(1);

            bool endsWithWildcard = pattern.EndsWith("*");
            if (endsWithWildcard)
                pattern = pattern.Substring(0, pattern.Length - 1);

            if (ignoreCase)
                pattern = pattern.ToLower();

            if (startsWithWildcard && endsWithWildcard)
                return value.Contains(pattern);

            if (startsWithWildcard)
                return value.EndsWith(pattern);

            if (endsWithWildcard)
                return value.StartsWith(pattern);

            return value.Equals(pattern);
        }

        public static string ToConcatenatedString<T>(this IEnumerable<T> values, Func<T, string> stringSelector)
        {
            return values.ToConcatenatedString(stringSelector, String.Empty);
        }

        public static string ToConcatenatedString<T>(this IEnumerable<T> values, Func<T, string> action, string separator)
        {
            var sb = new StringBuilder();
            foreach (var item in values)
            {
                if (sb.Length > 0)
                    sb.Append(separator);

                sb.Append(action(item));
            }

            return sb.ToString();
        }

        public static string ReplaceFirst(this string input, string find, string replace)
        {
            if (String.IsNullOrEmpty(input))
                return input;

            var i = input.IndexOf(find, StringComparison.Ordinal);
            if (i < 0)
                return input;

            var pre = input.Substring(0, i);
            var post = input.Substring(i + find.Length);
            return String.Concat(pre, replace, post);
        }

        public static IEnumerable<string> SplitLines(this string text)
        {
            return text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(l => !String.IsNullOrWhiteSpace(l)).Select(l => l.Trim());
        }

        public static string StripInvisible(this string s)
        {
            return s
                .Replace("\r\n", " ")
                .Replace('\n', ' ')
                .Replace('\t', ' ');
        }

        public static string NormalizeLineEndings(this string text, string lineEnding = null)
        {
            if (String.IsNullOrEmpty(lineEnding))
                lineEnding = Environment.NewLine;

            text = text.Replace("\r\n", "\n");
            if (lineEnding != "\n")
                text = text.Replace("\r\n", lineEnding);

            return text;
        }


        public static string Truncate(this string text, int length, string ellipsis, bool keepFullWordAtEnd)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            if (text.Length < length)
                return text;

            text = text.Substring(0, length);

            if (keepFullWordAtEnd && text.LastIndexOf(' ') > 0)
                text = text.Substring(0, text.LastIndexOf(' '));

            return string.Format("{0}{1}", text, ellipsis);
        }

        public static string ToLowerFiltered(this string value, char[] charsToRemove)
        {
            var builder = new StringBuilder(value.Length);

            for (int index = 0; index < value.Length; index++)
            {
                char c = value[index];
                if (Char.IsUpper(c))
                    c = Char.ToLower(c);

                bool includeChar = true;
                for (int i = 0; i < charsToRemove.Length; i++)
                {
                    if (charsToRemove[i] == c)
                    {
                        includeChar = false;
                        break;
                    }
                }

                if (includeChar)
                    builder.Append(c);
            }

            return builder.ToString();
        }

        public static string[] SplitAndTrim(this string s, params string[] separator)
        {
            if (s.IsNullOrEmpty())
                return new string[0];

            var result = ((separator == null) || (separator.Length == 0))
                ? s.Split((char[])null, StringSplitOptions.RemoveEmptyEntries)
                : s.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].Trim();

            return result;
        }

        public static string[] SplitAndTrim(this string s, params char[] separator)
        {
            if (s.IsNullOrEmpty())
                return new string[0];

            var result = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < result.Length; i++)
                result[i] = result[i].Trim();

            return result;

        }

              
        public static bool IsNullOrWhiteSpace(this string item)
        {
            return String.IsNullOrEmpty(item) || item.All(Char.IsWhiteSpace);
        }

        public static string HexEscape(this string value, params char[] anyCharOf)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (anyCharOf == null || anyCharOf.Length == 0) return value;

            var encodeCharMap = new HashSet<char>(anyCharOf);

            var sb = new StringBuilder();
            var textLength = value.Length;
            for (var i = 0; i < textLength; i++)
            {
                var c = value[i];
                if (encodeCharMap.Contains(c))
                {
                    sb.Append('%' + ((int)c).ToString("x"));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static readonly Regex _entityResolver = new Regex("([&][#](?'decimal'[0-9]+);)|([&][#][(x|X)](?'hex'[0-9a-fA-F]+);)|([&](?'html'\\w+);)");

        public static string HtmlEntityEncode(this string value)
        {
            return HtmlEntityEncode(value, true);
        }

        public static string HtmlEntityEncode(this string value, bool encodeTagsToo)
        {
            string str = string.Empty;
            foreach (char ch in value)
            {
                int num = (int)ch;
                switch (num)
                {
                    case 38:
                        if (encodeTagsToo)
                        {
                            str = str + "&amp;";
                            break;
                        }
                        else
                            break;
                    case 60:
                        if (encodeTagsToo)
                        {
                            str = str + "&lt;";
                            break;
                        }
                        else
                            break;
                    case 62:
                        if (encodeTagsToo)
                        {
                            str = str + "&gt;";
                            break;
                        }
                        else
                            break;
                    default:
                        str = (int)ch < 32 || (int)ch > 126 ? str + "&#" + num.ToString((IFormatProvider)NumberFormatInfo.InvariantInfo) + ";" : str + (object)ch;
                        break;
                }
            }
            return str;
        }

        public static string HtmlEntityDecode(this string encodedText)
        {
            return _entityResolver.Replace(encodedText, new MatchEvaluator(ResolveEntityAngleAmp));
        }

        public static string HtmlEntityDecode(this string encodedText, bool encodeTagsToo)
        {
            if (encodeTagsToo)
                return _entityResolver.Replace(encodedText, new MatchEvaluator(ResolveEntityAngleAmp));
            else
                return _entityResolver.Replace(encodedText, new MatchEvaluator(ResolveEntityNotAngleAmp));
        }

        private static string ResolveEntityNotAngleAmp(Match matchToProcess)
        {
            string str;
            if (matchToProcess.Groups["decimal"].Success)
                str = Convert.ToChar(Convert.ToInt32(matchToProcess.Groups["decimal"].Value)).ToString();
            else if (matchToProcess.Groups["hex"].Success)
                str = Convert.ToChar(HexToInt(matchToProcess.Groups["hex"].Value)).ToString();
            else if (matchToProcess.Groups["html"].Success)
            {
                string entity = matchToProcess.Groups["html"].Value;
                switch (entity.ToLower())
                {
                    case "lt":
                    case "gt":
                    case "amp":
                        str = "&" + entity + ";";
                        break;
                    default:
                        str = EntityLookup(entity);
                        break;
                }
            }
            else
                str = "X";
            return str;
        }

        private static string ResolveEntityAngleAmp(Match matchToProcess)
        {
            return !matchToProcess.Groups["decimal"].Success ? (!matchToProcess.Groups["hex"].Success ? (!matchToProcess.Groups["html"].Success ? "Y" : EntityLookup(matchToProcess.Groups["html"].Value)) : Convert.ToChar(HexToInt(matchToProcess.Groups["hex"].Value)).ToString()) : Convert.ToChar(Convert.ToInt32(matchToProcess.Groups["decimal"].Value)).ToString();
        }

        public static int HexToInt(string input)
        {
            int num = 0;
            input = input.ToUpper();
            char[] chArray = input.ToCharArray();
            for (int index = chArray.Length - 1; index >= 0; --index)
            {
                if ((int)chArray[index] >= 48 && (int)chArray[index] <= 57)
                    num += ((int)chArray[index] - 48) * (int)Math.Pow(16.0, (double)(chArray.Length - 1 - index));
                else if ((int)chArray[index] >= 65 && (int)chArray[index] <= 70)
                {
                    num += ((int)chArray[index] - 55) * (int)Math.Pow(16.0, (double)(chArray.Length - 1 - index));
                }
                else
                {
                    num = 0;
                    break;
                }
            }
            return num;
        }

        private static string EntityLookup(string entity)
        {
            string str = "";
            switch (entity)
            {
                case "Aacute":
                    str = Convert.ToChar(193).ToString();
                    break;
                case "aacute":
                    str = Convert.ToChar(225).ToString();
                    break;
                case "acirc":
                    str = Convert.ToChar(226).ToString();
                    break;
                case "Acirc":
                    str = Convert.ToChar(194).ToString();
                    break;
                case "acute":
                    str = Convert.ToChar(180).ToString();
                    break;
                case "AElig":
                    str = Convert.ToChar(198).ToString();
                    break;
                case "aelig":
                    str = Convert.ToChar(230).ToString();
                    break;
                case "Agrave":
                    str = Convert.ToChar(192).ToString();
                    break;
                case "agrave":
                    str = Convert.ToChar(224).ToString();
                    break;
                case "alefsym":
                    str = Convert.ToChar(8501).ToString();
                    break;
                case "Alpha":
                    str = Convert.ToChar(913).ToString();
                    break;
                case "alpha":
                    str = Convert.ToChar(945).ToString();
                    break;
                case "amp":
                    str = Convert.ToChar(38).ToString();
                    break;
                case "and":
                    str = Convert.ToChar(8743).ToString();
                    break;
                case "ang":
                    str = Convert.ToChar(8736).ToString();
                    break;
                case "aring":
                    str = Convert.ToChar(229).ToString();
                    break;
                case "Aring":
                    str = Convert.ToChar(197).ToString();
                    break;
                case "asymp":
                    str = Convert.ToChar(8776).ToString();
                    break;
                case "Atilde":
                    str = Convert.ToChar(195).ToString();
                    break;
                case "atilde":
                    str = Convert.ToChar(227).ToString();
                    break;
                case "auml":
                    str = Convert.ToChar(228).ToString();
                    break;
                case "Auml":
                    str = Convert.ToChar(196).ToString();
                    break;
                case "bdquo":
                    str = Convert.ToChar(8222).ToString();
                    break;
                case "Beta":
                    str = Convert.ToChar(914).ToString();
                    break;
                case "beta":
                    str = Convert.ToChar(946).ToString();
                    break;
                case "brvbar":
                    str = Convert.ToChar(166).ToString();
                    break;
                case "bull":
                    str = Convert.ToChar(8226).ToString();
                    break;
                case "cap":
                    str = Convert.ToChar(8745).ToString();
                    break;
                case "Ccedil":
                    str = Convert.ToChar(199).ToString();
                    break;
                case "ccedil":
                    str = Convert.ToChar(231).ToString();
                    break;
                case "cedil":
                    str = Convert.ToChar(184).ToString();
                    break;
                case "cent":
                    str = Convert.ToChar(162).ToString();
                    break;
                case "chi":
                    str = Convert.ToChar(967).ToString();
                    break;
                case "Chi":
                    str = Convert.ToChar(935).ToString();
                    break;
                case "circ":
                    str = Convert.ToChar(710).ToString();
                    break;
                case "clubs":
                    str = Convert.ToChar(9827).ToString();
                    break;
                case "cong":
                    str = Convert.ToChar(8773).ToString();
                    break;
                case "copy":
                    str = Convert.ToChar(169).ToString();
                    break;
                case "crarr":
                    str = Convert.ToChar(8629).ToString();
                    break;
                case "cup":
                    str = Convert.ToChar(8746).ToString();
                    break;
                case "curren":
                    str = Convert.ToChar(164).ToString();
                    break;
                case "dagger":
                    str = Convert.ToChar(8224).ToString();
                    break;
                case "Dagger":
                    str = Convert.ToChar(8225).ToString();
                    break;
                case "darr":
                    str = Convert.ToChar(8595).ToString();
                    break;
                case "dArr":
                    str = Convert.ToChar(8659).ToString();
                    break;
                case "deg":
                    str = Convert.ToChar(176).ToString();
                    break;
                case "Delta":
                    str = Convert.ToChar(916).ToString();
                    break;
                case "delta":
                    str = Convert.ToChar(948).ToString();
                    break;
                case "diams":
                    str = Convert.ToChar(9830).ToString();
                    break;
                case "divide":
                    str = Convert.ToChar(247).ToString();
                    break;
                case "eacute":
                    str = Convert.ToChar(233).ToString();
                    break;
                case "Eacute":
                    str = Convert.ToChar(201).ToString();
                    break;
                case "Ecirc":
                    str = Convert.ToChar(202).ToString();
                    break;
                case "ecirc":
                    str = Convert.ToChar(234).ToString();
                    break;
                case "Egrave":
                    str = Convert.ToChar(200).ToString();
                    break;
                case "egrave":
                    str = Convert.ToChar(232).ToString();
                    break;
                case "empty":
                    str = Convert.ToChar(8709).ToString();
                    break;
                case "emsp":
                    str = Convert.ToChar(8195).ToString();
                    break;
                case "ensp":
                    str = Convert.ToChar(8194).ToString();
                    break;
                case "epsilon":
                    str = Convert.ToChar(949).ToString();
                    break;
                case "Epsilon":
                    str = Convert.ToChar(917).ToString();
                    break;
                case "equiv":
                    str = Convert.ToChar(8801).ToString();
                    break;
                case "Eta":
                    str = Convert.ToChar(919).ToString();
                    break;
                case "eta":
                    str = Convert.ToChar(951).ToString();
                    break;
                case "eth":
                    str = Convert.ToChar(240).ToString();
                    break;
                case "ETH":
                    str = Convert.ToChar(208).ToString();
                    break;
                case "Euml":
                    str = Convert.ToChar(203).ToString();
                    break;
                case "euml":
                    str = Convert.ToChar(235).ToString();
                    break;
                case "euro":
                    str = Convert.ToChar(8364).ToString();
                    break;
                case "exist":
                    str = Convert.ToChar(8707).ToString();
                    break;
                case "fnof":
                    str = Convert.ToChar(402).ToString();
                    break;
                case "forall":
                    str = Convert.ToChar(8704).ToString();
                    break;
                case "frac12":
                    str = Convert.ToChar(189).ToString();
                    break;
                case "frac14":
                    str = Convert.ToChar(188).ToString();
                    break;
                case "frac34":
                    str = Convert.ToChar(190).ToString();
                    break;
                case "frasl":
                    str = Convert.ToChar(8260).ToString();
                    break;
                case "gamma":
                    str = Convert.ToChar(947).ToString();
                    break;
                case "Gamma":
                    str = Convert.ToChar(915).ToString();
                    break;
                case "ge":
                    str = Convert.ToChar(8805).ToString();
                    break;
                case "gt":
                    str = Convert.ToChar(62).ToString();
                    break;
                case "hArr":
                    str = Convert.ToChar(8660).ToString();
                    break;
                case "harr":
                    str = Convert.ToChar(8596).ToString();
                    break;
                case "hearts":
                    str = Convert.ToChar(9829).ToString();
                    break;
                case "hellip":
                    str = Convert.ToChar(8230).ToString();
                    break;
                case "Iacute":
                    str = Convert.ToChar(205).ToString();
                    break;
                case "iacute":
                    str = Convert.ToChar(237).ToString();
                    break;
                case "icirc":
                    str = Convert.ToChar(238).ToString();
                    break;
                case "Icirc":
                    str = Convert.ToChar(206).ToString();
                    break;
                case "iexcl":
                    str = Convert.ToChar(161).ToString();
                    break;
                case "Igrave":
                    str = Convert.ToChar(204).ToString();
                    break;
                case "igrave":
                    str = Convert.ToChar(236).ToString();
                    break;
                case "image":
                    str = Convert.ToChar(8465).ToString();
                    break;
                case "infin":
                    str = Convert.ToChar(8734).ToString();
                    break;
                case "int":
                    str = Convert.ToChar(8747).ToString();
                    break;
                case "Iota":
                    str = Convert.ToChar(921).ToString();
                    break;
                case "iota":
                    str = Convert.ToChar(953).ToString();
                    break;
                case "iquest":
                    str = Convert.ToChar(191).ToString();
                    break;
                case "isin":
                    str = Convert.ToChar(8712).ToString();
                    break;
                case "iuml":
                    str = Convert.ToChar(239).ToString();
                    break;
                case "Iuml":
                    str = Convert.ToChar(207).ToString();
                    break;
                case "kappa":
                    str = Convert.ToChar(954).ToString();
                    break;
                case "Kappa":
                    str = Convert.ToChar(922).ToString();
                    break;
                case "Lambda":
                    str = Convert.ToChar(923).ToString();
                    break;
                case "lambda":
                    str = Convert.ToChar(955).ToString();
                    break;
                case "lang":
                    str = Convert.ToChar(9001).ToString();
                    break;
                case "laquo":
                    str = Convert.ToChar(171).ToString();
                    break;
                case "larr":
                    str = Convert.ToChar(8592).ToString();
                    break;
                case "lArr":
                    str = Convert.ToChar(8656).ToString();
                    break;
                case "lceil":
                    str = Convert.ToChar(8968).ToString();
                    break;
                case "ldquo":
                    str = Convert.ToChar(8220).ToString();
                    break;
                case "le":
                    str = Convert.ToChar(8804).ToString();
                    break;
                case "lfloor":
                    str = Convert.ToChar(8970).ToString();
                    break;
                case "lowast":
                    str = Convert.ToChar(8727).ToString();
                    break;
                case "loz":
                    str = Convert.ToChar(9674).ToString();
                    break;
                case "lrm":
                    str = Convert.ToChar(8206).ToString();
                    break;
                case "lsaquo":
                    str = Convert.ToChar(8249).ToString();
                    break;
                case "lsquo":
                    str = Convert.ToChar(8216).ToString();
                    break;
                case "lt":
                    str = Convert.ToChar(60).ToString();
                    break;
                case "macr":
                    str = Convert.ToChar(175).ToString();
                    break;
                case "mdash":
                    str = Convert.ToChar(8212).ToString();
                    break;
                case "micro":
                    str = Convert.ToChar(181).ToString();
                    break;
                case "middot":
                    str = Convert.ToChar(183).ToString();
                    break;
                case "minus":
                    str = Convert.ToChar(8722).ToString();
                    break;
                case "Mu":
                    str = Convert.ToChar(924).ToString();
                    break;
                case "mu":
                    str = Convert.ToChar(956).ToString();
                    break;
                case "nabla":
                    str = Convert.ToChar(8711).ToString();
                    break;
                case "nbsp":
                    str = Convert.ToChar(160).ToString();
                    break;
                case "ndash":
                    str = Convert.ToChar(8211).ToString();
                    break;
                case "ne":
                    str = Convert.ToChar(8800).ToString();
                    break;
                case "ni":
                    str = Convert.ToChar(8715).ToString();
                    break;
                case "not":
                    str = Convert.ToChar(172).ToString();
                    break;
                case "notin":
                    str = Convert.ToChar(8713).ToString();
                    break;
                case "nsub":
                    str = Convert.ToChar(8836).ToString();
                    break;
                case "ntilde":
                    str = Convert.ToChar(241).ToString();
                    break;
                case "Ntilde":
                    str = Convert.ToChar(209).ToString();
                    break;
                case "Nu":
                    str = Convert.ToChar(925).ToString();
                    break;
                case "nu":
                    str = Convert.ToChar(957).ToString();
                    break;
                case "oacute":
                    str = Convert.ToChar(243).ToString();
                    break;
                case "Oacute":
                    str = Convert.ToChar(211).ToString();
                    break;
                case "Ocirc":
                    str = Convert.ToChar(212).ToString();
                    break;
                case "ocirc":
                    str = Convert.ToChar(244).ToString();
                    break;
                case "OElig":
                    str = Convert.ToChar(338).ToString();
                    break;
                case "oelig":
                    str = Convert.ToChar(339).ToString();
                    break;
                case "ograve":
                    str = Convert.ToChar(242).ToString();
                    break;
                case "Ograve":
                    str = Convert.ToChar(210).ToString();
                    break;
                case "oline":
                    str = Convert.ToChar(8254).ToString();
                    break;
                case "Omega":
                    str = Convert.ToChar(937).ToString();
                    break;
                case "omega":
                    str = Convert.ToChar(969).ToString();
                    break;
                case "Omicron":
                    str = Convert.ToChar(927).ToString();
                    break;
                case "omicron":
                    str = Convert.ToChar(959).ToString();
                    break;
                case "oplus":
                    str = Convert.ToChar(8853).ToString();
                    break;
                case "or":
                    str = Convert.ToChar(8744).ToString();
                    break;
                case "ordf":
                    str = Convert.ToChar(170).ToString();
                    break;
                case "ordm":
                    str = Convert.ToChar(186).ToString();
                    break;
                case "Oslash":
                    str = Convert.ToChar(216).ToString();
                    break;
                case "oslash":
                    str = Convert.ToChar(248).ToString();
                    break;
                case "otilde":
                    str = Convert.ToChar(245).ToString();
                    break;
                case "Otilde":
                    str = Convert.ToChar(213).ToString();
                    break;
                case "otimes":
                    str = Convert.ToChar(8855).ToString();
                    break;
                case "Ouml":
                    str = Convert.ToChar(214).ToString();
                    break;
                case "ouml":
                    str = Convert.ToChar(246).ToString();
                    break;
                case "para":
                    str = Convert.ToChar(182).ToString();
                    break;
                case "part":
                    str = Convert.ToChar(8706).ToString();
                    break;
                case "permil":
                    str = Convert.ToChar(8240).ToString();
                    break;
                case "perp":
                    str = Convert.ToChar(8869).ToString();
                    break;
                case "Phi":
                    str = Convert.ToChar(934).ToString();
                    break;
                case "phi":
                    str = Convert.ToChar(966).ToString();
                    break;
                case "Pi":
                    str = Convert.ToChar(928).ToString();
                    break;
                case "pi":
                    str = Convert.ToChar(960).ToString();
                    break;
                case "piv":
                    str = Convert.ToChar(982).ToString();
                    break;
                case "plusmn":
                    str = Convert.ToChar(177).ToString();
                    break;
                case "pound":
                    str = Convert.ToChar(163).ToString();
                    break;
                case "Prime":
                    str = Convert.ToChar(8243).ToString();
                    break;
                case "prime":
                    str = Convert.ToChar(8242).ToString();
                    break;
                case "prod":
                    str = Convert.ToChar(8719).ToString();
                    break;
                case "prop":
                    str = Convert.ToChar(8733).ToString();
                    break;
                case "psi":
                    str = Convert.ToChar(968).ToString();
                    break;
                case "Psi":
                    str = Convert.ToChar(936).ToString();
                    break;
                case "quot":
                    str = Convert.ToChar(34).ToString();
                    break;
                case "radic":
                    str = Convert.ToChar(8730).ToString();
                    break;
                case "rang":
                    str = Convert.ToChar(9002).ToString();
                    break;
                case "raquo":
                    str = Convert.ToChar(187).ToString();
                    break;
                case "rarr":
                    str = Convert.ToChar(8594).ToString();
                    break;
                case "rArr":
                    str = Convert.ToChar(8658).ToString();
                    break;
                case "rceil":
                    str = Convert.ToChar(8969).ToString();
                    break;
                case "rdquo":
                    str = Convert.ToChar(8221).ToString();
                    break;
                case "real":
                    str = Convert.ToChar(8476).ToString();
                    break;
                case "reg":
                    str = Convert.ToChar(174).ToString();
                    break;
                case "rfloor":
                    str = Convert.ToChar(8971).ToString();
                    break;
                case "rho":
                    str = Convert.ToChar(961).ToString();
                    break;
                case "Rho":
                    str = Convert.ToChar(929).ToString();
                    break;
                case "rlm":
                    str = Convert.ToChar(8207).ToString();
                    break;
                case "rsaquo":
                    str = Convert.ToChar(8250).ToString();
                    break;
                case "rsquo":
                    str = Convert.ToChar(8217).ToString();
                    break;
                case "sbquo":
                    str = Convert.ToChar(8218).ToString();
                    break;
                case "Scaron":
                    str = Convert.ToChar(352).ToString();
                    break;
                case "scaron":
                    str = Convert.ToChar(353).ToString();
                    break;
                case "sdot":
                    str = Convert.ToChar(8901).ToString();
                    break;
                case "sect":
                    str = Convert.ToChar(167).ToString();
                    break;
                case "shy":
                    str = Convert.ToChar(173).ToString();
                    break;
                case "sigma":
                    str = Convert.ToChar(963).ToString();
                    break;
                case "Sigma":
                    str = Convert.ToChar(931).ToString();
                    break;
                case "sigmaf":
                    str = Convert.ToChar(962).ToString();
                    break;
                case "sim":
                    str = Convert.ToChar(8764).ToString();
                    break;
                case "spades":
                    str = Convert.ToChar(9824).ToString();
                    break;
                case "sub":
                    str = Convert.ToChar(8834).ToString();
                    break;
                case "sube":
                    str = Convert.ToChar(8838).ToString();
                    break;
                case "sum":
                    str = Convert.ToChar(8721).ToString();
                    break;
                case "sup":
                    str = Convert.ToChar(8835).ToString();
                    break;
                case "sup1":
                    str = Convert.ToChar(185).ToString();
                    break;
                case "sup2":
                    str = Convert.ToChar(178).ToString();
                    break;
                case "sup3":
                    str = Convert.ToChar(179).ToString();
                    break;
                case "supe":
                    str = Convert.ToChar(8839).ToString();
                    break;
                case "szlig":
                    str = Convert.ToChar(223).ToString();
                    break;
                case "Tau":
                    str = Convert.ToChar(932).ToString();
                    break;
                case "tau":
                    str = Convert.ToChar(964).ToString();
                    break;
                case "there4":
                    str = Convert.ToChar(8756).ToString();
                    break;
                case "theta":
                    str = Convert.ToChar(952).ToString();
                    break;
                case "Theta":
                    str = Convert.ToChar(920).ToString();
                    break;
                case "thetasym":
                    str = Convert.ToChar(977).ToString();
                    break;
                case "thinsp":
                    str = Convert.ToChar(8201).ToString();
                    break;
                case "thorn":
                    str = Convert.ToChar(254).ToString();
                    break;
                case "THORN":
                    str = Convert.ToChar(222).ToString();
                    break;
                case "tilde":
                    str = Convert.ToChar(732).ToString();
                    break;
                case "times":
                    str = Convert.ToChar(215).ToString();
                    break;
                case "trade":
                    str = Convert.ToChar(8482).ToString();
                    break;
                case "Uacute":
                    str = Convert.ToChar(218).ToString();
                    break;
                case "uacute":
                    str = Convert.ToChar(250).ToString();
                    break;
                case "uarr":
                    str = Convert.ToChar(8593).ToString();
                    break;
                case "uArr":
                    str = Convert.ToChar(8657).ToString();
                    break;
                case "Ucirc":
                    str = Convert.ToChar(219).ToString();
                    break;
                case "ucirc":
                    str = Convert.ToChar(251).ToString();
                    break;
                case "Ugrave":
                    str = Convert.ToChar(217).ToString();
                    break;
                case "ugrave":
                    str = Convert.ToChar(249).ToString();
                    break;
                case "uml":
                    str = Convert.ToChar(168).ToString();
                    break;
                case "upsih":
                    str = Convert.ToChar(978).ToString();
                    break;
                case "Upsilon":
                    str = Convert.ToChar(933).ToString();
                    break;
                case "upsilon":
                    str = Convert.ToChar(965).ToString();
                    break;
                case "Uuml":
                    str = Convert.ToChar(220).ToString();
                    break;
                case "uuml":
                    str = Convert.ToChar(252).ToString();
                    break;
                case "weierp":
                    str = Convert.ToChar(8472).ToString();
                    break;
                case "Xi":
                    str = Convert.ToChar(926).ToString();
                    break;
                case "xi":
                    str = Convert.ToChar(958).ToString();
                    break;
                case "yacute":
                    str = Convert.ToChar(253).ToString();
                    break;
                case "Yacute":
                    str = Convert.ToChar(221).ToString();
                    break;
                case "yen":
                    str = Convert.ToChar(165).ToString();
                    break;
                case "Yuml":
                    str = Convert.ToChar(376).ToString();
                    break;
                case "yuml":
                    str = Convert.ToChar((int)byte.MaxValue).ToString();
                    break;
                case "zeta":
                    str = Convert.ToChar(950).ToString();
                    break;
                case "Zeta":
                    str = Convert.ToChar(918).ToString();
                    break;
                case "zwj":
                    str = Convert.ToChar(8205).ToString();
                    break;
                case "zwnj":
                    str = Convert.ToChar(8204).ToString();
                    break;
            }
            return str;
        }

        /// <summary>
        /// Formats a string to an invariant culture
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatInvariant(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.InvariantCulture, format, objects);
        }

        /// <summary>
        /// Formats a string to the current culture.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string FormatCurrent(this string format, params object[] objects)
        {
            return string.Format(CultureInfo.CurrentCulture, format, objects);
        }
        [DebuggerStepThrough]
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// Determines whether the string is null, empty or all whitespace.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
       
        /// <summary>
        /// Türkçe karakterli string normalize eder
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToNormalizationString(this string value)
        {
            return String.Join("", value.Normalize(NormalizationForm.FormD)
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }
        public static bool IsDateTime(string txtDate)
        {
            DateTime tempDate;
            return DateTime.TryParse(txtDate, out tempDate);
        }
        public static string ToFullText(this string str)
        {
            string searchTerm = null;
            if (!string.IsNullOrEmpty(str))
            {
                string[] keywords = str.Trim().Split(null);
                foreach (var keyword in keywords)
                {
                    searchTerm += string.Format("\"{0}*\" AND ", keyword);
                }
                if (searchTerm != null)
                    searchTerm = searchTerm.Substring(0, searchTerm.LastIndexOf(" AND "));
            }
            return searchTerm;
        }
        public static string RemoveWhiteSpace(this string self)
        {
            return new string(self.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string[] HexTbl = Enumerable.Range(0, 256).Select(v => v.ToString("X2")).ToArray();

        /// <summary>
        /// Returns true if strings starts with otherString, ignoring case.
        /// </summary>
        public static bool ToStartsWithIgnoreCase(this string s, string otherString)
        {
            return s.StartsWith(otherString, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsValidEmail(this string value)
        {

            bool isEmail = Regex.IsMatch(value, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;

        }

        /// <summary>
        ///     Checks if date with dateFormat is parse-able to System.DateTime format returns boolean value if true else false
        /// </summary>
        /// <param name="data">String date</param>
        /// <param name="dateFormat">date format example dd/MM/yyyy HH:mm:ss</param>
        /// <returns>boolean True False if is valid System.DateTime</returns>
        public static bool IsDateTime(this string data, string dateFormat)
        {
            // ReSharper disable once RedundantAssignment
            DateTime dateVal = default(DateTime);
            return DateTime.TryParseExact(data, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateVal);
        }

        /// <summary>
        ///     Converts the string representation of a number to its 32-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int32</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int32.MinValue or greater than System.Int32.MaxValue
        /// </remarks>
        public static int ToInt32(this string value)
        {
            int number;
            Int32.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 64-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int64</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int64.MinValue or greater than System.Int64.MaxValue
        /// </remarks>
        public static long ToInt64(this string value)
        {
            long number;
            Int64.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 16-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int16</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int16.MinValue or greater than System.Int16.MaxValue
        /// </remarks>
        public static short ToInt16(this string value)
        {
            short number;
            Int16.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its System.Decimal equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Decimal</returns>
        /// <remarks>
        ///     The conversion fails if the s parameter is null, is not a number in a valid format, or represents a number
        ///     less than System.Decimal.MinValue or greater than System.Decimal.MaxValue
        /// </remarks>
        public static Decimal ToDecimal(this string value)
        {
            Decimal number;
            Decimal.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts string to its boolean equivalent
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>boolean equivalent</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">
        ///         thrown in the event no boolean equivalent found or an empty or whitespace
        ///         string is passed
        ///     </exception>
        /// </remarks>
        public static bool ToBoolean(this string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("value");
            }
            string val = value.ToLower().Trim();
            switch (val)
            {
                case "false":
                    return false;
                case "f":
                    return false;
                case "true":
                    return true;
                case "t":
                    return true;
                case "yes":
                    return true;
                case "no":
                    return false;
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    throw new ArgumentException("Invalid boolean");
            }
        }

        /// <summary>
        ///     Returns an enumerable collection of the specified type containing the substrings in this instance that are
        ///     delimited by elements of a specified Char array
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="separator">
        ///     An array of Unicode characters that delimit the substrings in this instance, an empty array containing no
        ///     delimiters, or null.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the element to return in the collection, this type must implement IConvertible.
        /// </typeparam>
        /// <returns>
        ///     An enumerable collection whose elements contain the substrings in this instance that are delimited by one or more
        ///     characters in separator.
        /// </returns>
        public static IEnumerable<T> SplitTo<T>(this string str, params char[] separator) where T : IConvertible
        {
            return str.Split(separator, StringSplitOptions.None).Select(s => (T)Convert.ChangeType(s, typeof(T)));
        }

        /// <summary>
        ///     Returns an enumerable collection of the specified type containing the substrings in this instance that are
        ///     delimited by elements of a specified Char array
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="options">StringSplitOptions <see cref="StringSplitOptions" /></param>
        /// <param name="separator">
        ///     An array of Unicode characters that delimit the substrings in this instance, an empty array containing no
        ///     delimiters, or null.
        /// </param>
        /// <typeparam name="T">
        ///     The type of the element to return in the collection, this type must implement IConvertible.
        /// </typeparam>
        /// <returns>
        ///     An enumerable collection whose elements contain the substrings in this instance that are delimited by one or more
        ///     characters in separator.
        /// </returns>
        public static IEnumerable<T> SplitTo<T>(this string str, StringSplitOptions options, params char[] separator)
            where T : IConvertible
        {
            return str.Split(separator, options).Select(s => (T)Convert.ChangeType(s, typeof(T)));
        }

        /// <summary>
        ///     Converts string to its Enum type
        ///     Checks of string is a member of type T enum before converting
        ///     if fails returns default enum
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="value"> The string representation of the enumeration name or underlying value to convert</param>
        /// <param name="defaultValue"></param>
        /// <returns>Enum object</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">
        ///         enumType is not an System.Enum.-or- value is either an empty string ("") or
        ///         only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration
        ///     </exception>
        /// </remarks>
        public static T ToEnum<T>(this string value, T defaultValue = default(T)) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("Type T Must of type System.Enum");
            }

            T result;
            bool isParsed = Enum.TryParse(value, true, out result);
            return isParsed ? result : defaultValue;
        }

        /// <summary>
        ///     Replaces one or more format items in a specified string with the string representation of a specified object.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="arg0">An System.Object to format</param>
        /// <returns>A copy of format in which any format items are replaced by the string representation of arg0</returns>
        /// <exception cref="ArgumentNullException">format or args is null.</exception>
        /// <exception cref="System.FormatException">
        ///     format is invalid.-or- The index of a format item is less than zero, or
        ///     greater than or equal to the length of the args array.
        /// </exception>
        public static string Format(this string value, object arg0)
        {
            return string.Format(value, arg0);
        }

        /// <summary>
        ///     Replaces the format item in a specified string with the string representation of a corresponding object in a
        ///     specified array.
        /// </summary>
        /// <param name="value">A composite format string</param>
        /// <param name="args">An object array that contains zero or more objects to format</param>
        /// <returns>
        ///     A copy of format in which the format items have been replaced by the string representation of the
        ///     corresponding objects in args
        /// </returns>
        /// <exception cref="ArgumentNullException">format or args is null.</exception>
        /// <exception cref="System.FormatException">
        ///     format is invalid.-or- The index of a format item is less than zero, or
        ///     greater than or equal to the length of the args array.
        /// </exception>
        public static string Format(this string value, params object[] args)
        {
            return string.Format(value, args);
        }

        /// <summary>
        ///     Gets empty String if passed value is of type Null/Nothing
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.String</returns>
        /// <remarks></remarks>
        public static string GetEmptyStringIfNull(this string val)
        {
            return (val != null ? val.Trim() : "");
        }

        /// <summary>
        ///     Checks if a string is null and returns String if not Empty else returns null/Nothing
        /// </summary>
        /// <param name="myValue">String value</param>
        /// <returns>null/nothing if String IsEmpty</returns>
        /// <remarks></remarks>
        public static string GetNullIfEmptyString(this string myValue)
        {
            if (myValue == null || myValue.Length <= 0)
            {
                return null;
            }
            myValue = myValue.Trim();
            if (myValue.Length > 0)
            {
                return myValue;
            }
            return null;
        }

        /// <summary>
        ///     IsInteger Function checks if a string is a valid int32 value
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>Boolean True if isInteger else False</returns>
        public static bool IsInteger(this string val)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            int retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Int32.TryParse(val, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Read in a sequence of words from standard input and capitalize each
        ///     one (make first letter uppercase; make rest lowercase).
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>Word with capitalization</returns>
        public static string Capitalize(this string s)
        {
            if (s.Length == 0)
            {
                return s;
            }
            return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        }

        /// <summary>
        ///     Gets first character in string
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.string</returns>
        public static string FirstCharacter(this string val)
        {
            return (!string.IsNullOrEmpty(val))
                ? (val.Length >= 1)
                    ? val.Substring(0, 1)
                    : val
                : null;
        }

        /// <summary>
        ///     Gets last character in string
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.string</returns>
        public static string LastCharacter(this string val)
        {
            return (!string.IsNullOrEmpty(val))
                ? (val.Length >= 1)
                    ? val.Substring(val.Length - 1, 1)
                    : val
                : null;
        }

        /// <summary>
        ///     Check a String ends with another string ignoring the case.
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true or false</returns>
        public static bool EndsWithIgnoreCase(this string val, string suffix)
        {
            if (val == null)
            {
                throw new ArgumentNullException("val", "val parameter is null");
            }
            if (suffix == null)
            {
                throw new ArgumentNullException("suffix", "suffix parameter is null");
            }
            if (val.Length < suffix.Length)
            {
                return false;
            }
            return val.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Check a String starts with another string ignoring the case.
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true or false</returns>
        public static bool StartsWithIgnoreCase(this string val, string prefix)
        {
            if (val == null)
            {
                throw new ArgumentNullException("val", "val parameter is null");
            }
            if (prefix == null)
            {
                throw new ArgumentNullException("prefix", "prefix parameter is null");
            }
            if (val.Length < prefix.Length)
            {
                return false;
            }
            return val.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        ///     Replace specified characters with an empty string.
        /// </summary>
        /// <param name="s">the string</param>
        /// <param name="chars">list of characters to replace from the string</param>
        /// <example>
        ///     string s = "Friends";
        ///     s = s.Replace('F', 'r','i','s');  //s becomes 'end;
        /// </example>
        /// <returns>System.string</returns>
        public static string Replace(this string s, params char[] chars)
        {
            return chars.Aggregate(s, (current, c) => current.Replace(c.ToString(CultureInfo.InvariantCulture), ""));
        }

        /// <summary>
        ///     Remove Characters from string
        /// </summary>
        /// <param name="s">string to remove characters</param>
        /// <param name="chars">array of chars</param>
        /// <returns>System.string</returns>
        public static string RemoveChars(this string s, params char[] chars)
        {
            var sb = new StringBuilder(s.Length);
            foreach (char c in s.Where(c => !chars.Contains(c)))
            {
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Validate email address
        /// </summary>
        /// <param name="email">string email address</param>
        /// <returns>true or false if email if valid</returns>
        public static bool IsEmailAddress(this string email)
        {
            string pattern =
                "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(email, pattern).Success;
        }

        /// <summary>
        ///     IsNumeric checks if a string is a valid floating value
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Boolean True if isNumeric else False</returns>
        /// <remarks></remarks>
        public static bool IsNumeric(this string val)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Double.TryParse(val, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Truncate String and append ... at end
        /// </summary>
        /// <param name="s">String to be truncated</param>
        /// <param name="maxLength">number of chars to truncate</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Truncate(this string s, int maxLength)
        {
            if (String.IsNullOrEmpty(s) || maxLength <= 0)
            {
                return String.Empty;
            }
            if (s.Length > maxLength)
            {
                return s.Substring(0, maxLength) + "...";
            }
            return s;
        }

        /// <summary>
        ///     Function returns a default String value if given value is null or empty
        /// </summary>
        /// <param name="myValue">String value to check if isEmpty</param>
        /// <param name="defaultValue">default value to return if String value isEmpty</param>
        /// <returns>returns either String value or default value if IsEmpty</returns>
        /// <remarks></remarks>
        public static string GetDefaultIfEmpty(this string myValue, string defaultValue)
        {
            if (!String.IsNullOrEmpty(myValue))
            {
                myValue = myValue.Trim();
                return myValue.Length > 0 ? myValue : defaultValue;
            }
            return defaultValue;
        }

        /// <summary>
        ///     Convert a string to its equivalent byte array
        /// </summary>
        /// <param name="val">string to convert</param>
        /// <returns>System.byte array</returns>
        public static byte[] ToBytes(this string val)
        {
            var bytes = new byte[val.Length * sizeof(char)];
            Buffer.BlockCopy(val.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        ///     Reverse string
        /// </summary>
        /// <param name="val">string to reverse</param>
        /// <returns>System.string</returns>
        public static string Reverse(this string val)
        {
            var chars = new char[val.Length];
            for (int i = val.Length - 1, j = 0; i >= 0; --i, ++j)
            {
                chars[j] = val[i];
            }
            val = new String(chars);
            return val;
        }

        /// <summary>
        ///     Appends String quotes for type CSV data
        /// </summary>
        /// <param name="val">val</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ParseStringToCsv(this string val)
        {
            return '"' + GetEmptyStringIfNull(val).Replace("\"", "\"\"") + '"';
        }


        /// <summary>
        ///     Count number of occurrences in string
        /// </summary>
        /// <param name="val">string containing text</param>
        /// <param name="stringToMatch">string or pattern find</param>
        /// <returns></returns>
        public static int CountOccurrences(this string val, string stringToMatch)
        {
            return Regex.Matches(val, stringToMatch, RegexOptions.IgnoreCase).Count;
        }

        /// <summary>
        ///     Converts a Json string to dictionary object method applicable for single hierarchy objects i.e
        ///     no parent child relationships, for parent child relationships <see cref="JsonToExpanderObject" />
        /// </summary>
        /// <param name="val">string formated as Json</param>
        /// <returns>IDictionary Json object</returns>
        /// <remarks>
        ///     <exception cref="ArgumentNullException">if string parameter is null or empty</exception>
        /// </remarks>
        public static IDictionary<string, object> JsonToDictionary(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("val");
            }
            return
                (Dictionary<string, object>)JsonConvert.DeserializeObject(val, typeof(Dictionary<string, object>));
        }

        /// <summary>
        ///     Converts a Json string to ExpandoObject method applicable for multi hierarchy objects i.e
        ///     having zero or many parent child relationships
        /// </summary>
        /// <param name="json">string formated as Json</param>
        /// <returns>System.Dynamic.ExpandoObject Json object<see cref="ExpandoObject" />ExpandoObject</returns>
        public static dynamic JsonToExpanderObject(this string json)
        {
            var converter = new ExpandoObjectConverter();
            return JsonConvert.DeserializeObject<ExpandoObject>(json, converter);
        }

        /// <summary>
        ///     Converts a Json string to object of type T method applicable for multi hierarchy objects i.e
        ///     having zero or many parent child relationships, Ignore loop references and do not serialize if cycles are detected.
        /// </summary>
        /// <typeparam name="T">object to convert to</typeparam>
        /// <param name="json">json</param>
        /// <returns>object</returns>
        public static T JsonToObject<T>(this string json)
        {
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        ///     Removes the first part of the string, if no match found return original string
        /// </summary>
        /// <param name="val">string to remove prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no prefix or original string</returns>
        public static string RemovePrefix(this string val, string prefix, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(val) && (ignoreCase ? val.StartsWithIgnoreCase(prefix) : val.StartsWith(prefix)))
            {
                return val.Substring(prefix.Length, val.Length - prefix.Length);
            }
            return val;
        }

        /// <summary>
        ///     Removes the end part of the string, if no match found return original string
        /// </summary>
        /// <param name="val">string to remove suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns>trimmed string with no suffix or original string</returns>
        public static string RemoveSuffix(this string val, string suffix, bool ignoreCase = true)
        {
            if (!string.IsNullOrEmpty(val) && (ignoreCase ? val.EndsWithIgnoreCase(suffix) : val.EndsWith(suffix)))
            {
                return val.Substring(0, val.Length - suffix.Length);
            }
            return null;
        }

        /// <summary>
        ///     Appends the suffix to the end of the string if the string does not already end in the suffix.
        /// </summary>
        /// <param name="val">string to append suffix</param>
        /// <param name="suffix">suffix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string AppendSuffixIfMissing(this string val, string suffix, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(val) || (ignoreCase ? val.EndsWithIgnoreCase(suffix) : val.EndsWith(suffix)))
            {
                return val;
            }
            return val + suffix;
        }

        /// <summary>
        ///     Appends the prefix to the start of the string if the string does not already start with prefix.
        /// </summary>
        /// <param name="val">string to append prefix</param>
        /// <param name="prefix">prefix</param>
        /// <param name="ignoreCase">Indicates whether the compare should ignore case</param>
        /// <returns></returns>
        public static string AppendPrefixIfMissing(this string val, string prefix, bool ignoreCase = true)
        {
            if (string.IsNullOrEmpty(val) || (ignoreCase ? val.StartsWithIgnoreCase(prefix) : val.StartsWith(prefix)))
            {
                return val;
            }
            return prefix + val;
        }

        /// <summary>
        ///     Checks if the String contains only Unicode letters.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="val">string to check if is Alpha</param>
        /// <returns>true if only contains letters, and is non-null</returns>
        public static bool IsAlpha(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            return val.Trim().Replace(" ", "").All(Char.IsLetter);
        }

        /// <summary>
        ///     Checks if the String contains only Unicode letters, digits.
        ///     null will return false. An empty String ("") will return false.
        /// </summary>
        /// <param name="val">string to check if is Alpha or Numeric</param>
        /// <returns></returns>
        public static bool IsAlphaNumeric(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            return val.Trim().Replace(" ", "").All(Char.IsLetterOrDigit);
        }

        /// <summary>
        ///     Convert string to Hash using Sha512
        /// </summary>
        /// <param name="val">string to hash</param>
        /// <returns>Hashed string</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CreateHashSha512(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("val");
            }
            var sb = new StringBuilder();
            using (SHA512 hash = SHA512.Create())
            {
                byte[] data = hash.ComputeHash(val.ToBytes());
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Convert string to Hash using Sha256
        /// </summary>
        /// <param name="val">string to hash</param>
        /// <returns>Hashed string</returns>
        public static string CreateHashSha256(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("val");
            }
            var sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                byte[] data = hash.ComputeHash(val.ToBytes());
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Convert url query string to IDictionary value key pair
        /// </summary>
        /// <param name="queryString">query string value</param>
        /// <returns>IDictionary value key pair</returns>
        public static IDictionary<string, string> QueryStringToDictionary(this string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }
            if (!queryString.Contains("?"))
            {
                return null;
            }
            string query = queryString.Replace("?", "");
            if (!query.Contains("="))
            {
                return null;
            }
            return query.Split('&').Select(p => p.Split('=')).ToDictionary(
                key => key[0].ToLower().Trim(), value => value[1]);
        }

        /// <summary>
        ///     Reverse back or forward slashes
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="direction">
        ///     0 - replace forward slash with back
        ///     1 - replace back with forward slash
        /// </param>
        /// <returns></returns>
        public static string ReverseSlash(this string val, int direction)
        {
            switch (direction)
            {
                case 0:
                    return val.Replace(@"/", @"\");
                case 1:
                    return val.Replace(@"\", @"/");
                default:
                    return val;
            }
        }

        /// <summary>
        ///     Replace Line Feeds
        /// </summary>
        /// <param name="val">string to remove line feeds</param>
        /// <returns>System.string</returns>
        public static string ReplaceLineFeeds(this string val)
        {
            return Regex.Replace(val, @"^[\r\n]+|\.|[\r\n]+$", "");
        }

        /// <summary>
        ///     Validates if a string is valid IPv4
        ///     Regular expression taken from <a href="http://regexlib.com/REDetails.aspx?regexp_id=2035">Regex reference</a>
        /// </summary>
        /// <param name="val">string IP address</param>
        /// <returns>true if string matches valid IP address else false</returns>
        public static bool IsValidIPv4(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            return Regex.Match(val,
                @"(?:^|\s)([a-z]{3,6}(?=://))?(://)?((?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?))(?::(\d{2,5}))?(?:\s|$)")
                .Success;
        }

        /// <summary>
        ///     Calculates the amount of bytes occupied by the input string encoded as the encoding specified
        /// </summary>
        /// <param name="val">The input string to check</param>
        /// <param name="encoding">The encoding to use</param>
        /// <returns>The total size of the input string in bytes</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentNullException">encoding is null</exception>
        public static int GetByteSize(this string val, Encoding encoding)
        {
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            return encoding.GetByteCount(val);
        }

        /// <summary>
        ///     Extracts the left part of the input string limited with the length parameter
        /// </summary>
        /// <param name="val">The input string to take the left part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring starting at startIndex 0 until length</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string Left(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("val");
            }
            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                    "length cannot be higher than total string length or less than 0");
            }
            return val.Substring(0, length);
        }

        /// <summary>
        ///     Extracts the right part of the input string limited with the length parameter
        /// </summary>
        /// <param name="val">The input string to take the right part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring taken from the input string</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string Right(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("val");
            }
            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                    "length cannot be higher than total string length or less than 0");
            }
            return val.Substring(val.Length - length);
        }

        /// <summary>
        ///     ToTextElements
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToTextElements(this string val)
        {
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(val);
            while (elementEnumerator.MoveNext())
            {
                string textElement = elementEnumerator.GetTextElement();
                yield return textElement;
            }
        }

        /// <summary>
        ///     Check if a string does not start with prefix
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <param name="prefix">prefix</param>
        /// <returns>true if string does not match prefix else false, null values will always evaluate to false</returns>
        public static bool DoesNotStartWith(this string val, string prefix)
        {
            return val == null || prefix == null ||
                   !val.StartsWith(prefix, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Check if a string does not end with prefix
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <param name="suffix">suffix</param>
        /// <returns>true if string does not match prefix else false, null values will always evaluate to false</returns>
        public static bool DoesNotEndWith(this string val, string suffix)
        {
            return val == null || suffix == null ||
                   !val.EndsWith(suffix, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Checks if a string is null
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <returns>true if string is null else false</returns>
        public static bool IsNull(this string val)
        {
            return val == null;
        }

        /// <summary>
        ///     Checks if a string is null or empty
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <returns>true if string is null or is empty else false</returns>
        public static bool IsNullOrEmpty(this string val)
        {
            return String.IsNullOrEmpty(val);
        }

        /// <summary>
        ///     Checks if string length is a certain minimum number of characters, does not ignore leading and trailing
        ///     white-space.
        ///     null strings will always evaluate to false.
        /// </summary>
        /// <param name="val">string to evaluate minimum length</param>
        /// <param name="minCharLength">minimum allowable string length</param>
        /// <returns>true if string is of specified minimum length</returns>
        public static bool IsMinLength(this string val, int minCharLength)
        {
            return val != null && val.Length >= minCharLength;
        }

        /// <summary>
        ///     Checks if string length is consists of specified allowable maximum char length. does not ignore leading and
        ///     trailing white-space.
        ///     null strings will always evaluate to false.
        /// </summary>
        /// <param name="val">string to evaluate maximum length</param>
        /// <param name="maxCharLength">maximum allowable string length</param>
        /// <returns>true if string has specified maximum char length</returns>
        public static bool IsMaxLength(this string val, int maxCharLength)
        {
            return val != null && val.Length <= maxCharLength;
        }

        /// <summary>
        ///     Checks if string length satisfies minimum and maximum allowable char length. does not ignore leading and
        ///     trailing white-space
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <param name="minCharLength">minimum char length</param>
        /// <param name="maxCharLength">maximum char length</param>
        /// <returns>true if string satisfies minimum and maximum allowable length</returns>
        public static bool IsLength(this string val, int minCharLength, int maxCharLength)
        {
            return val != null && val.Length >= minCharLength && val.Length <= minCharLength;
        }

        /// <summary>
        ///     Gets the number of characters in string checks if string is null
        /// </summary>
        /// <param name="val">string to evaluate length</param>
        /// <returns>total number of chars or null if string is null</returns>
        public static int? GetLength(string val)
        {
            return val == null ? (int?)null : val.Length;
        }

        /// <summary>
        ///     Create basic dynamic SQL where parameters from a JSON key value pair string
        /// </summary>
        /// <param name="value">json key value pair string</param>
        /// <param name="useOr">if true constructs parameters using or statement if false and</param>
        /// <returns></returns>
        public static string CreateParameters(this string value, bool useOr)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            IDictionary<string, object> searchParamters = value.JsonToDictionary();
            var @params = new StringBuilder("");
            if (searchParamters == null)
            {
                return @params.ToString();
            }
            for (int i = 0; i <= searchParamters.Count() - 1; i++)
            {
                string key = searchParamters.Keys.ElementAt(i);
                var val = (string)searchParamters[key];
                if (!string.IsNullOrEmpty(key))
                {
                    @params.Append(key).Append(" like '").Append(val.Trim()).Append("%' ");
                    if (i < searchParamters.Count() - 1 && useOr)
                    {
                        @params.Append(" or ");
                    }
                    else if (i < searchParamters.Count() - 1)
                    {
                        @params.Append(" and ");
                    }
                }
            }
            return @params.ToString();
        }

        public static bool IsValidUrl(this string val)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(val, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        /// <summary>
        /// Removes all invalid file name characters from the string, and then returns ToPascalCase for the resulting string.
        /// Also guarantees the string will not end in a period or space.
        /// Do not call this on the null string.
        /// </summary>
        public static string ToSafeFileName(this string text)
        {
            return text.RemoveCharacters(Path.GetInvalidFileNameChars()).TrimEnd('.').EnglishToPascal();
        }


        /// <summary>
        /// Removes all instances of all given characters from the
        /// given string, and returns this new string.
        /// </summary>
        public static string RemoveCharacters(this string text, params char[] characters)
        {
            return ReplaceCharactersWithString(text, "", characters);
        }

        /// <summary>
        /// Removes whitespace from between words, capitalizes the first letter of each word, and lowercases the remainder of each word (ex: "one two" becomes "OneTwo").
        /// Trims the resulting string.
        /// Do not call this on the null string.
        /// </summary>
        public static string EnglishToPascal(this string text)
        {
            return ConcatenateWithDelimiter("", text.Separate().Select(t => t.ToLower().CapitalizeString()).ToArray());
        }
        private static string ReplaceCharactersWithString(string text, string replacementString, params char[] characters)
        {
            if (text == null)
                return null;

            foreach (var character in characters)
                text = text.Replace(character.ToString(), replacementString);
            return text;
        }

        /// <summary>
        /// Creates a single string consisting of each string in the given list, delimited by the given delimiter.  Empty strings
        /// are handled intelligently in that you will not get two delimiters in a row, or a delimiter at the end of the string.
        /// Whitespace is trimmed from the given strings before concatenation.
        /// Null strings are treated as empty strings.
        /// </summary>
        public static string ConcatenateWithDelimiter(string delimiter, params string[] strings)
        {
            var tokens = strings.Select(i => (i ?? "").Trim()).Where(i => i.Length > 0).ToList();
            if (!tokens.Any())
                return "";
            var result = new StringBuilder(tokens.First());
            foreach (var token in tokens.Skip(1))
                result.Append(delimiter + token);
            return result.ToString();
        }

        /// <summary>
        /// Splits this non null string into a list of non null substrings using white space characters as separators. Empty substrings will be excluded from the
        /// list, and therefore, if this string is empty or contains only white space characters, the list will be empty.
        /// All strings in the resulting list are trimmed, not explicitly, but by definition because any surrounding whitespace would have counted as part of the delimiter.
        /// </summary>
        public static List<string> Separate(this string s)
        {
            // Impossible to respond to R# warning because if you replace inline separators, the compiler can't figure out what method to call.
            string[] separators = null;
            return s.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Returns the given string with its first letter-or-digit character capitalized.
        /// </summary>
        public static string CapitalizeString(this string text)
        {
            if (text == null)
                return null;

            return new string(text.ToCharArray().Select((c, index) => index == GetIndexOfFirstLetterOrDigit(text) ? Char.ToUpper(c) : c).ToArray());
        }
        private static int GetIndexOfFirstLetterOrDigit(string text)
        {
            return text.IndexOfAny(text.ToCharArray().Where(Char.IsLetterOrDigit).ToArray());
        }

        public static string CamelCase(this string value)
        {
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }

        public static string RemoveSpecialCharacters(this string value)
        {
            var dictionary = new Dictionary<char, char[]>
            {
                {'a', new[] {'à', 'á', 'ä', 'â', 'ã'}},
                {'A', new[] {'À', 'Á', 'Ä', 'Â', 'Ã'}},
                {'c', new[] {'ç'}},
                {'C', new[] {'Ç'}},
                {'e', new[] {'è', 'é', 'ë', 'ê'}},
                {'E', new[] {'È', 'É', 'Ë', 'Ê'}},
                {'i', new[] {'ì', 'í', 'ï', 'î'}},
                {'I', new[] {'Ì', 'Í', 'Ï', 'Î'}},
                {'o', new[] {'ò', 'ó', 'ö', 'ô', 'õ'}},
                {'O', new[] {'Ò', 'Ó', 'Ö', 'Ô', 'Õ'}},
                {'u', new[] {'ù', 'ú', 'ü', 'û'}},
                {'U', new[] {'Ù', 'Ú', 'Ü', 'Û'}}
            };

            value = dictionary.Keys.Aggregate(value, (x, y) => dictionary[y].Aggregate(x, (z, c) => z.Replace(c, y)));

            return new Regex("[^0-9a-zA-Z._ ]+?").Replace(value, string.Empty);
        }

    }
}
