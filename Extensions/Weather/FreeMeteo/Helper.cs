using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Globalization;

namespace FreeMeteo
{
    public class Helper
    {
        private static char[] s_entityEndingChars = new char[] { ';', '&' };


        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes));
        }
        public static string UrlEncode(string str)
        {
            if (str == null)
            {
                return null;
            }
            return UrlEncode(str, Encoding.UTF8);
        }
        public static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }
        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(UrlEncodeToBytes(bytes, offset, count));
        }
        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
            {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }
        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }
        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if ((bytes == null) && (count == 0))
            {
                return null;
            }
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if ((offset < 0) || (offset > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || ((offset + count) > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            return UrlEncodeBytesToBytesInternal(bytes, offset, count, true);
        }
        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++)
            {
                char ch = (char)bytes[offset + i];
                if (ch == ' ')
                {
                    num++;
                }
                else if (!IsSafe(ch))
                {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0))
            {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++)
            {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsSafe(ch2))
                {
                    buffer[num4++] = num6;
                }
                else if (ch2 == ' ')
                {
                    buffer[num4++] = 0x2b;
                }
                else
                {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }
        internal static bool IsSafe(char ch)
        {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9')))
            {
                return true;
            }
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }
        internal static char IntToHex(int n)
        {
            if (n <= 9)
            {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }
        public static string HtmlDecode(string s)
        {
            if (s == null)
            {
                return null;
            }
            if (s.IndexOf('&') < 0)
            {
                return s;
            }
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            HtmlDecode(s, output);
            return sb.ToString();
        }
        public static void HtmlDecode(string s, TextWriter output)
        {
            if (s != null)
            {
                if (s.IndexOf('&') < 0)
                {
                    output.Write(s);
                }
                else
                {
                    int length = s.Length;
                    for (int i = 0; i < length; i++)
                    {
                        char ch = s[i];
                        if (ch == '&')
                        {
                            int num3 = s.IndexOfAny(s_entityEndingChars, i + 1);
                            if ((num3 > 0) && (s[num3] == ';'))
                            {
                                string entity = s.Substring(i + 1, (num3 - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#'))
                                {
                                    try
                                    {
                                        if ((entity[1] == 'x') || (entity[1] == 'X'))
                                        {
                                            ch = (char)int.Parse(entity.Substring(2), NumberStyles.AllowHexSpecifier);
                                        }
                                        else
                                        {
                                            ch = (char)int.Parse(entity.Substring(1));
                                        }
                                        i = num3;
                                    }
                                    catch (FormatException)
                                    {
                                        i++;
                                    }
                                    catch (ArgumentException)
                                    {
                                        i++;
                                    }
                                }
                                else
                                {
                                    i = num3;
                                    char ch2 = Lookup(entity);
                                    if (ch2 != '\0')
                                    {
                                        ch = ch2;
                                    }
                                    else
                                    {
                                        output.Write('&');
                                        output.Write(entity);
                                        output.Write(';');
                                        goto Label_0103;
                                    }
                                }
                            }
                        }
                        output.Write(ch);
                    Label_0103: ;
                    }
                }
            }
        }
        private static string[] _entitiesList = new string[] { 
        "\"-quot", "&-amp", "<-lt", ">-gt", "\x00a0-nbsp", "\x00a1-iexcl", "\x00a2-cent", "\x00a3-pound", "\x00a4-curren", "\x00a5-yen", "\x00a6-brvbar", "\x00a7-sect", "\x00a8-uml", "\x00a9-copy", "\x00aa-ordf", "\x00ab-laquo", 
        "\x00ac-not", "\x00ad-shy", "\x00ae-reg", "\x00af-macr", "\x00b0-deg", "\x00b1-plusmn", "\x00b2-sup2", "\x00b3-sup3", "\x00b4-acute", "\x00b5-micro", "\x00b6-para", "\x00b7-middot", "\x00b8-cedil", "\x00b9-sup1", "\x00ba-ordm", "\x00bb-raquo", 
        "\x00bc-frac14", "\x00bd-frac12", "\x00be-frac34", "\x00bf-iquest", "\x00c0-Agrave", "\x00c1-Aacute", "\x00c2-Acirc", "\x00c3-Atilde", "\x00c4-Auml", "\x00c5-Aring", "\x00c6-AElig", "\x00c7-Ccedil", "\x00c8-Egrave", "\x00c9-Eacute", "\x00ca-Ecirc", "\x00cb-Euml", 
        "\x00cc-Igrave", "\x00cd-Iacute", "\x00ce-Icirc", "\x00cf-Iuml", "\x00d0-ETH", "\x00d1-Ntilde", "\x00d2-Ograve", "\x00d3-Oacute", "\x00d4-Ocirc", "\x00d5-Otilde", "\x00d6-Ouml", "\x00d7-times", "\x00d8-Oslash", "\x00d9-Ugrave", "\x00da-Uacute", "\x00db-Ucirc", 
        "\x00dc-Uuml", "\x00dd-Yacute", "\x00de-THORN", "\x00df-szlig", "\x00e0-agrave", "\x00e1-aacute", "\x00e2-acirc", "\x00e3-atilde", "\x00e4-auml", "\x00e5-aring", "\x00e6-aelig", "\x00e7-ccedil", "\x00e8-egrave", "\x00e9-eacute", "\x00ea-ecirc", "\x00eb-euml", 
        "\x00ec-igrave", "\x00ed-iacute", "\x00ee-icirc", "\x00ef-iuml", "\x00f0-eth", "\x00f1-ntilde", "\x00f2-ograve", "\x00f3-oacute", "\x00f4-ocirc", "\x00f5-otilde", "\x00f6-ouml", "\x00f7-divide", "\x00f8-oslash", "\x00f9-ugrave", "\x00fa-uacute", "\x00fb-ucirc", 
        "\x00fc-uuml", "\x00fd-yacute", "\x00fe-thorn", "\x00ff-yuml", "Œ-OElig", "œ-oelig", "Š-Scaron", "š-scaron", "Ÿ-Yuml", "ƒ-fnof", "ˆ-circ", "˜-tilde", "Α-Alpha", "Β-Beta", "Γ-Gamma", "Δ-Delta", 
        "Ε-Epsilon", "Ζ-Zeta", "Η-Eta", "Θ-Theta", "Ι-Iota", "Κ-Kappa", "Λ-Lambda", "Μ-Mu", "Ν-Nu", "Ξ-Xi", "Ο-Omicron", "Π-Pi", "Ρ-Rho", "Σ-Sigma", "Τ-Tau", "Υ-Upsilon", 
        "Φ-Phi", "Χ-Chi", "Ψ-Psi", "Ω-Omega", "α-alpha", "β-beta", "γ-gamma", "δ-delta", "ε-epsilon", "ζ-zeta", "η-eta", "θ-theta", "ι-iota", "κ-kappa", "λ-lambda", "μ-mu", 
        "ν-nu", "ξ-xi", "ο-omicron", "π-pi", "ρ-rho", "ς-sigmaf", "σ-sigma", "τ-tau", "υ-upsilon", "φ-phi", "χ-chi", "ψ-psi", "ω-omega", "ϑ-thetasym", "ϒ-upsih", "ϖ-piv", 
        " -ensp", " -emsp", " -thinsp", "‌-zwnj", "‍-zwj", "‎-lrm", "‏-rlm", "–-ndash", "—-mdash", "‘-lsquo", "’-rsquo", "‚-sbquo", "“-ldquo", "”-rdquo", "„-bdquo", "†-dagger", 
        "‡-Dagger", "•-bull", "…-hellip", "‰-permil", "′-prime", "″-Prime", "‹-lsaquo", "›-rsaquo", "‾-oline", "⁄-frasl", "€-euro", "ℑ-image", "℘-weierp", "ℜ-real", "™-trade", "ℵ-alefsym", 
        "←-larr", "↑-uarr", "→-rarr", "↓-darr", "↔-harr", "↵-crarr", "⇐-lArr", "⇑-uArr", "⇒-rArr", "⇓-dArr", "⇔-hArr", "∀-forall", "∂-part", "∃-exist", "∅-empty", "∇-nabla", 
        "∈-isin", "∉-notin", "∋-ni", "∏-prod", "∑-sum", "−-minus", "∗-lowast", "√-radic", "∝-prop", "∞-infin", "∠-ang", "∧-and", "∨-or", "∩-cap", "∪-cup", "∫-int", 
        "∴-there4", "∼-sim", "≅-cong", "≈-asymp", "≠-ne", "≡-equiv", "≤-le", "≥-ge", "⊂-sub", "⊃-sup", "⊄-nsub", "⊆-sube", "⊇-supe", "⊕-oplus", "⊗-otimes", "⊥-perp", 
        "⋅-sdot", "⌈-lceil", "⌉-rceil", "⌊-lfloor", "⌋-rfloor", "〈-lang", "〉-rang", "◊-loz", "♠-spades", "♣-clubs", "♥-hearts", "♦-diams"
     };
        private static Hashtable _entitiesLookupTable;
        private static object _lookupLockObject = new object();


        internal static char Lookup(string entity)
        {
            if (_entitiesLookupTable == null)
            {
                lock (_lookupLockObject)
                {
                    if (_entitiesLookupTable == null)
                    {
                        Hashtable hashtable = new Hashtable();
                        foreach (string str in _entitiesList)
                        {
                            hashtable[str.Substring(2)] = str[0];
                        }
                        _entitiesLookupTable = hashtable;
                    }
                }
            }
            object obj2 = _entitiesLookupTable[entity];
            if (obj2 != null)
            {
                return (char)obj2;
            }
            return '\0';
        }
    }
}
