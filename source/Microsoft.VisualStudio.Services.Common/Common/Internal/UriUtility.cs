// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.UriUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class UriUtility
  {
    private const string c_uriSchemeHttp = "http";
    private const string c_uriSchemeHttps = "https";
    private static readonly ICollection<string> UnsafeUriSchemeList = (ICollection<string>) new HashSet<string>((IEnumerable<string>) new string[2]
    {
      "javascript",
      "vbscript"
    }, (IEqualityComparer<string>) VssStringComparer.UriScheme);
    private static readonly ICollection<string> SafeUriSchemeList = (ICollection<string>) new HashSet<string>((IEnumerable<string>) new string[16]
    {
      "http",
      "https",
      "ftp",
      "gopher",
      "mailto",
      "news",
      "telnet",
      "wais",
      "vstfs",
      "tfs",
      "alm",
      "mtm",
      "mtms",
      "mfbclient",
      "mfbclients",
      "x-mvwit"
    }, (IEqualityComparer<string>) VssStringComparer.UriScheme);
    private const char PathSeparatorChar = '/';
    private const string PathSeparator = "/";
    public static IEqualityComparer<Uri> AbsoluteUriStringComparer = (IEqualityComparer<Uri>) new UriUtility._AbsoluteUriStringComparer();
    public static IEqualityComparer<string> UrlPathIgnoreSeparatorsComparer = (IEqualityComparer<string>) new UriUtility._UrlPathIgnoreSeparatorsComparer();
    private static char[] _htmlEntityEndingChars = new char[2]
    {
      ';',
      '&'
    };

    public static bool IsUriUnsafe(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      return !uri.IsAbsoluteUri || UriUtility.UnsafeUriSchemeList.Contains(uri.Scheme) || UriUtility.IsUriLocalFile(uri);
    }

    public static bool IsUriSafe(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      return uri.IsAbsoluteUri && UriUtility.SafeUriSchemeList.Contains(uri.Scheme) && !UriUtility.IsUriLocalFile(uri);
    }

    public static bool IsUriLocalFile(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      return uri.IsAbsoluteUri && uri.IsFile && !uri.IsUnc;
    }

    public static string GetInvariantAbsoluteUri(Uri uri)
    {
      string relativePath = uri.AbsoluteUri;
      if (uri.Segments.Length > 1)
        relativePath = UriUtility.TrimEndingPathSeparator(relativePath);
      return relativePath.ToLowerInvariant();
    }

    public static Uri Combine(
      string baseUri,
      string relativePath,
      bool treatAbsolutePathAsRelative)
    {
      return UriUtility.Combine(new Uri(baseUri), relativePath, treatAbsolutePathAsRelative);
    }

    public static Uri Combine(Uri baseUri, string relativePath, bool treatAbsolutePathAsRelative)
    {
      if (baseUri == (Uri) null)
        throw new ArgumentNullException(nameof (baseUri));
      if (relativePath == null)
        throw new ArgumentNullException(nameof (relativePath));
      UriBuilder uriBuilder = new UriBuilder(baseUri);
      char[] chArray = new char[1]{ '/' };
      uriBuilder.Path = uriBuilder.Path.TrimEnd(chArray);
      uriBuilder.Path = UriUtility.AppendSlashToPathIfNeeded(uriBuilder.Path);
      if (VssStringComparer.Url.StartsWith(relativePath, "/"))
      {
        if (!treatAbsolutePathAsRelative)
          throw new ArgumentException(CommonResources.AbsoluteVirtualPathNotAllowed((object) relativePath), nameof (relativePath));
        relativePath = relativePath.TrimStart(chArray);
      }
      UriUtility.CheckRelativePath(relativePath);
      Uri relativeUri = new Uri(relativePath, UriKind.Relative);
      return new Uri(uriBuilder.Uri, relativeUri);
    }

    public static bool Equals(Uri uri1, Uri uri2) => UriUtility.AbsoluteUriStringComparer.Equals(uri1, uri2);

    public static string CombinePath(string part1, string part2)
    {
      char[] chArray = new char[1]{ '/' };
      if (string.IsNullOrEmpty(part1))
        return part2;
      return string.IsNullOrEmpty(part2) ? part1 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) part1.TrimEnd(chArray), (object) part2.TrimStart(chArray));
    }

    public static bool IsUriHttp(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      if (!uri.IsAbsoluteUri)
        return false;
      return uri.Scheme == "http" || uri.Scheme == "https";
    }

    public static void CheckUriIsHttp(Uri uri) => UriUtility.CheckUriIsHttp(uri, true);

    public static void CheckUriIsHttp(Uri uri, bool allowPathAndQuery)
    {
      if (!(uri != (Uri) null))
        return;
      if (!UriUtility.IsUriHttp(uri))
        throw new VssServiceException(CommonResources.UriUtility_UriNotAllowed((object) uri.AbsoluteUri));
      if (allowPathAndQuery)
        return;
      if (uri.PathAndQuery.Trim('/').Length > 0)
        throw new VssServiceException(CommonResources.UriUtility_MustBeAuthorityOnlyUri((object) uri, (object) uri.GetLeftPart(UriPartial.Authority)));
    }

    public static void CheckUriIsAbsoluteAndHttp(Uri uri) => UriUtility.CheckUriIsAbsoluteAndHttp(uri, true);

    public static void CheckUriIsAbsoluteAndHttp(Uri uri, bool allowPathAndQuery)
    {
      if (!(uri != (Uri) null))
        return;
      if (!uri.IsAbsoluteUri)
        throw new VssServiceException(CommonResources.UriUtility_AbsoluteUriRequired((object) uri.OriginalString));
      UriUtility.CheckUriIsHttp(uri, allowPathAndQuery);
    }

    public static void CheckRelativePath(string relativePath)
    {
      if (string.IsNullOrEmpty(relativePath))
        return;
      try
      {
        relativePath = relativePath.Replace("\\", "/");
        if (new Uri(relativePath, UriKind.RelativeOrAbsolute).IsAbsoluteUri)
          throw new VssServiceException(CommonResources.UriUtility_RelativePathInvalid((object) relativePath));
      }
      catch (Exception ex)
      {
        throw new VssServiceException(CommonResources.UriUtility_RelativePathInvalid((object) relativePath));
      }
    }

    public static string GetDavUncFromHttpPath(string httppath)
    {
      Uri uri = new Uri(httppath, UriKind.Absolute);
      UriUtility.CheckUriIsHttp(uri);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(Path.DirectorySeparatorChar);
      stringBuilder.Append(Path.DirectorySeparatorChar);
      stringBuilder.Append(uri.Host);
      if (uri.Scheme == Uri.UriSchemeHttps)
        stringBuilder.Append("@SSL");
      if (!uri.IsDefaultPort)
      {
        stringBuilder.Append("@");
        stringBuilder.Append(uri.Port);
      }
      stringBuilder.Append(Path.DirectorySeparatorChar);
      stringBuilder.Append("DavWWWRoot");
      stringBuilder.Append(Path.DirectorySeparatorChar);
      string str = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Replace('/', Path.DirectorySeparatorChar);
      stringBuilder.Append(str);
      return stringBuilder.ToString();
    }

    public static Uri TryGetHttpUriFromDavUncPath(string uncPath)
    {
      Match match = uncPath != null ? new Regex("^\\\\\\\\(?<host>[^\\\\|@]+)(?<ssl>@SSL)?(@(?<port>\\d+))?\\\\DavWWWRoot\\\\(?<path>.+)$", RegexOptions.IgnoreCase | RegexOptions.Singleline).Match(uncPath) : throw new ArgumentNullException(nameof (uncPath));
      if (!match.Success)
        return (Uri) null;
      Group group1 = match.Groups["host"];
      Group group2 = match.Groups["ssl"];
      Group group3 = match.Groups["port"];
      Group group4 = match.Groups["path"];
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}{2}/{3}", (object) (!group2.Success ? Uri.UriSchemeHttp : Uri.UriSchemeHttps), (object) group1.Value, (object) (!group3.Success ? "" : ":" + group3.Value), (object) group4.Value.Replace(Path.DirectorySeparatorChar, '/')), UriKind.Absolute);
    }

    public static bool IsSameMachine(string hostname1, string hostname2)
    {
      bool flag = false;
      try
      {
        flag = !string.IsNullOrEmpty(hostname1) && !string.IsNullOrEmpty(hostname2) && (string.Equals(hostname1, hostname2, StringComparison.OrdinalIgnoreCase) || string.Equals(Dns.GetHostEntry(hostname1).HostName, Dns.GetHostEntry(hostname2).HostName, StringComparison.OrdinalIgnoreCase));
      }
      catch (SocketException ex)
      {
      }
      return flag;
    }

    public static bool IsSubdomainOf(string domain, string parentDomain)
    {
      if (!domain.EndsWith(parentDomain, StringComparison.Ordinal))
        return false;
      return domain.Length == parentDomain.Length || domain[domain.Length - parentDomain.Length - 1] == '.';
    }

    public static Uri GetAbsoluteUriFromString(string uriString)
    {
      Uri uriFromString = UriUtility.GetUriFromString(uriString);
      return !(uriFromString == (Uri) null) ? uriFromString : throw new VssServiceException(CommonResources.UrlNotValid());
    }

    public static Uri GetUriFromString(string val)
    {
      Uri uri;
      return UriUtility.TryCreateAbsoluteUri(val, true, out uri) ? uri : (Uri) null;
    }

    public static bool TryCreateAbsoluteUri(string val, bool requireHttpScheme, out Uri uri)
    {
      uri = (Uri) null;
      val = val?.Trim();
      if (string.IsNullOrEmpty(val))
        return false;
      try
      {
        uri = new Uri(val);
      }
      catch (FormatException ex)
      {
      }
      if ((uri == (Uri) null || !uri.IsAbsoluteUri) && !VssStringComparer.Url.StartsWith(val, "http"))
      {
        if (!VssStringComparer.Url.StartsWith(val, "https"))
        {
          try
          {
            val = "http://" + val;
            uri = new Uri(val);
          }
          catch (FormatException ex)
          {
          }
        }
      }
      if (uri == (Uri) null || requireHttpScheme && !VssStringComparer.Url.StartsWith(uri.Scheme, "http") && !VssStringComparer.Url.StartsWith(uri.Scheme, "https"))
        return false;
      if (uri.IsAbsoluteUri)
        return true;
      uri = (Uri) null;
      return false;
    }

    public static string EnsureStartsWithPathSeparator(string relativePath)
    {
      if (relativePath != null && !VssStringComparer.Url.StartsWith(relativePath, "/"))
        relativePath = "/" + relativePath;
      return relativePath;
    }

    public static string EnsureEndsWithPathSeparator(string relativePath)
    {
      if (relativePath != null && !VssStringComparer.Url.EndsWith(relativePath, "/"))
        relativePath += "/";
      return relativePath;
    }

    public static string TrimStartingPathSeparator(string relativePath) => relativePath?.TrimStart('/');

    public static string TrimEndingPathSeparator(string relativePath) => relativePath?.TrimEnd('/');

    public static string TrimPathSeparators(string relativePath) => relativePath?.Trim('/');

    public static string AppendSlashToPathIfNeeded(string path)
    {
      if (path == null)
        return (string) null;
      int length = path.Length;
      if (length == 0 || path[length - 1] == '/')
        return path;
      path += "/";
      return path;
    }

    public static Uri NormalizePathSeparators(Uri uri)
    {
      if (uri.LocalPath.Contains("//"))
      {
        UriBuilder uriBuilder = new UriBuilder(uri);
        string str = uriBuilder.Path;
        while (str.Contains("//"))
          str = str.Replace("//", "/");
        uriBuilder.Path = str;
        uri = uriBuilder.Uri;
      }
      return uri;
    }

    public static NameValueCollection ParseFragmentString(string fragment) => UriUtility.ParseFragmentString(fragment, Encoding.UTF8);

    public static NameValueCollection ParseFragmentString(string fragment, Encoding encoding) => UriUtility.ParseFragmentString(fragment, encoding, true);

    public static NameValueCollection ParseFragmentString(
      string fragment,
      Encoding encoding,
      bool urlEncoded)
    {
      ArgumentUtility.CheckForNull<string>(fragment, nameof (fragment));
      ArgumentUtility.CheckForNull<Encoding>(encoding, nameof (encoding));
      if (fragment.Length > 0 && fragment[0] == '#')
        fragment = fragment.Substring(1);
      return (NameValueCollection) new UriUtility.HttpValueCollection(fragment, false, urlEncoded, encoding);
    }

    public static NameValueCollection ParseQueryString(string query) => UriUtility.ParseQueryString(query, Encoding.UTF8);

    public static NameValueCollection ParseQueryString(string query, Encoding encoding) => UriUtility.ParseQueryString(query, encoding, true);

    public static NameValueCollection ParseQueryString(
      string query,
      Encoding encoding,
      bool urlEncoded)
    {
      ArgumentUtility.CheckForNull<string>(query, nameof (query));
      ArgumentUtility.CheckForNull<Encoding>(encoding, nameof (encoding));
      if (query.Length > 0 && query[0] == '?')
        query = query.Substring(1);
      return (NameValueCollection) new UriUtility.HttpValueCollection(query, false, urlEncoded, encoding);
    }

    private static byte[] UrlEncode(
      byte[] bytes,
      int offset,
      int count,
      bool alwaysCreateNewReturnValue)
    {
      byte[] numArray = UriUtility.UrlEncode(bytes, offset, count);
      return !alwaysCreateNewReturnValue || numArray == null || numArray != bytes ? numArray : (byte[]) numArray.Clone();
    }

    private static byte[] UrlEncode(byte[] bytes, int offset, int count)
    {
      if (!UriUtility.ValidateUrlEncodingParameters(bytes, offset, count))
        return (byte[]) null;
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < count; ++index)
      {
        char ch = (char) bytes[offset + index];
        if (ch == ' ')
          ++num1;
        else if (!UriUtility.IsUrlSafeChar(ch))
          ++num2;
      }
      if (num1 == 0 && num2 == 0)
        return bytes;
      byte[] numArray1 = new byte[count + num2 * 2];
      int num3 = 0;
      for (int index1 = 0; index1 < count; ++index1)
      {
        byte num4 = bytes[offset + index1];
        char ch = (char) num4;
        if (UriUtility.IsUrlSafeChar(ch))
          numArray1[num3++] = num4;
        else if (ch == ' ')
        {
          numArray1[num3++] = (byte) 43;
        }
        else
        {
          byte[] numArray2 = numArray1;
          int index2 = num3;
          int num5 = index2 + 1;
          numArray2[index2] = (byte) 37;
          byte[] numArray3 = numArray1;
          int index3 = num5;
          int num6 = index3 + 1;
          int hex1 = (int) (byte) UriUtility.IntToHex((int) num4 >> 4 & 15);
          numArray3[index3] = (byte) hex1;
          byte[] numArray4 = numArray1;
          int index4 = num6;
          num3 = index4 + 1;
          int hex2 = (int) (byte) UriUtility.IntToHex((int) num4 & 15);
          numArray4[index4] = (byte) hex2;
        }
      }
      return numArray1;
    }

    private static string UrlEncodeNonAscii(string str, Encoding e)
    {
      if (string.IsNullOrEmpty(str))
        return str;
      if (e == null)
        e = Encoding.UTF8;
      byte[] bytes = e.GetBytes(str);
      return Encoding.ASCII.GetString(UriUtility.UrlEncodeNonAscii(bytes, 0, bytes.Length, false));
    }

    private static byte[] UrlEncodeNonAscii(
      byte[] bytes,
      int offset,
      int count,
      bool alwaysCreateNewReturnValue)
    {
      if (!UriUtility.ValidateUrlEncodingParameters(bytes, offset, count))
        return (byte[]) null;
      int num1 = 0;
      for (int index = 0; index < count; ++index)
      {
        if (UriUtility.IsNonAsciiByte(bytes[offset + index]))
          ++num1;
      }
      if (!alwaysCreateNewReturnValue && num1 == 0)
        return bytes;
      byte[] numArray1 = new byte[count + num1 * 2];
      int num2 = 0;
      for (int index1 = 0; index1 < count; ++index1)
      {
        byte b = bytes[offset + index1];
        if (UriUtility.IsNonAsciiByte(b))
        {
          byte[] numArray2 = numArray1;
          int index2 = num2;
          int num3 = index2 + 1;
          numArray2[index2] = (byte) 37;
          byte[] numArray3 = numArray1;
          int index3 = num3;
          int num4 = index3 + 1;
          int hex1 = (int) (byte) UriUtility.IntToHex((int) b >> 4 & 15);
          numArray3[index3] = (byte) hex1;
          byte[] numArray4 = numArray1;
          int index4 = num4;
          num2 = index4 + 1;
          int hex2 = (int) (byte) UriUtility.IntToHex((int) b & 15);
          numArray4[index4] = (byte) hex2;
        }
        else
          numArray1[num2++] = b;
      }
      return numArray1;
    }

    public static string UrlEncode(string str) => str == null ? (string) null : UriUtility.UrlEncode(str, Encoding.UTF8);

    public static string Base64Encode(string str) => str == null ? (string) null : Convert.ToBase64String(Encoding.UTF8.GetBytes(str));

    public static string UrlEncode(string str, Encoding e) => str == null ? (string) null : Encoding.ASCII.GetString(UriUtility.UrlEncodeToBytes(str, e));

    public static string UrlEncode(byte[] bytes) => bytes == null ? (string) null : Encoding.ASCII.GetString(UriUtility.UrlEncodeToBytes(bytes));

    public static byte[] UrlEncodeToBytes(string str) => str == null ? (byte[]) null : UriUtility.UrlEncodeToBytes(str, Encoding.UTF8);

    public static byte[] UrlEncodeToBytes(byte[] bytes) => bytes == null ? (byte[]) null : UriUtility.UrlEncodeToBytes(bytes, 0, bytes.Length);

    public static byte[] UrlEncodeToBytes(string str, Encoding e)
    {
      if (str == null)
        return (byte[]) null;
      byte[] bytes = e.GetBytes(str);
      return UriUtility.UrlEncode(bytes, 0, bytes.Length, false);
    }

    public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count) => UriUtility.UrlEncode(bytes, offset, count, true);

    public static string UrlPathEncode(string str)
    {
      if (string.IsNullOrEmpty(str))
        return str;
      int num = str.IndexOf('?');
      return num >= 0 ? UriUtility.UrlPathEncode(str.Substring(0, num)) + str.Substring(num) : UriUtility.UrlEncodeSpaces(UriUtility.UrlEncodeNonAscii(str, Encoding.UTF8));
    }

    public static string UrlEncodeUnicode(string value)
    {
      if (value == null)
        return (string) null;
      int length = value.Length;
      StringBuilder stringBuilder = new StringBuilder(length);
      for (int index = 0; index < length; ++index)
      {
        char ch = value[index];
        if (((int) ch & 65408) == 0)
        {
          if (UriUtility.IsUrlSafeChar(ch))
            stringBuilder.Append(ch);
          else if (ch == ' ')
          {
            stringBuilder.Append('+');
          }
          else
          {
            stringBuilder.Append('%');
            stringBuilder.Append(UriUtility.IntToHex((int) ch >> 4 & 15));
            stringBuilder.Append(UriUtility.IntToHex((int) ch & 15));
          }
        }
        else
        {
          stringBuilder.Append("%u");
          stringBuilder.Append(UriUtility.IntToHex((int) ch >> 12 & 15));
          stringBuilder.Append(UriUtility.IntToHex((int) ch >> 8 & 15));
          stringBuilder.Append(UriUtility.IntToHex((int) ch >> 4 & 15));
          stringBuilder.Append(UriUtility.IntToHex((int) ch & 15));
        }
      }
      return stringBuilder.ToString();
    }

    public static string HtmlEncode(string value)
    {
      if (string.IsNullOrEmpty(value) || UriUtility.IndexOfHtmlEncodingChars(value, 0) == -1)
        return value;
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        UriUtility.HtmlEncode(value, (TextWriter) output);
        return output.ToString();
      }
    }

    public static unsafe void HtmlEncode(string value, TextWriter output)
    {
      if (value == null)
        return;
      if (output == null)
        throw new ArgumentNullException(nameof (output));
      int num1 = UriUtility.IndexOfHtmlEncodingChars(value, 0);
      if (num1 == -1)
      {
        output.Write(value);
      }
      else
      {
        int num2 = value.Length - num1;
        fixed (char* chPtr1 = value)
        {
          char* chPtr2 = chPtr1;
          while (num1-- > 0)
            output.Write(*chPtr2++);
          while (num2-- > 0)
          {
            char ch = *chPtr2++;
            if (ch <= '>')
            {
              if (ch <= '&')
              {
                if (ch != '"')
                {
                  if (ch == '&')
                  {
                    output.Write("&amp;");
                    continue;
                  }
                }
                else
                {
                  output.Write("&quot;");
                  continue;
                }
              }
              else if (ch != '\'')
              {
                if (ch != '<')
                {
                  if (ch == '>')
                  {
                    output.Write("&gt;");
                    continue;
                  }
                }
                else
                {
                  output.Write("&lt;");
                  continue;
                }
              }
              else
              {
                output.Write("&#39;");
                continue;
              }
              output.Write(ch);
            }
            else if (ch >= ' ' && ch < 'Ā')
            {
              output.Write("&#");
              output.Write(((int) ch).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
              output.Write(';');
            }
            else
              output.Write(ch);
          }
        }
      }
    }

    private static unsafe int IndexOfHtmlEncodingChars(string s, int startPos)
    {
      int num = s.Length - startPos;
      fixed (char* chPtr1 = s)
      {
        char* chPtr2 = chPtr1 + startPos;
        for (; num > 0; --num)
        {
          char ch = *chPtr2;
          if (ch <= '>')
          {
            if (ch <= '&')
            {
              if (ch != '"' && ch != '&')
                goto label_10;
            }
            else if (ch != '\'' && ch != '<' && ch != '>')
              goto label_10;
            return s.Length - num;
          }
          if (ch >= ' ' && ch < 'Ā')
            return s.Length - num;
label_10:
          ++chPtr2;
        }
      }
      return -1;
    }

    private static string UrlDecodeInternal(string value, Encoding encoding)
    {
      if (value == null)
        return (string) null;
      int length = value.Length;
      UriUtility.UrlDecoder urlDecoder = new UriUtility.UrlDecoder(length, encoding);
      for (int index = 0; index < length; ++index)
      {
        char ch1 = value[index];
        switch (ch1)
        {
          case '%':
            if (index < length - 2)
            {
              if (value[index + 1] == 'u' && index < length - 5)
              {
                int num1 = UriUtility.HexToInt(value[index + 2]);
                int num2 = UriUtility.HexToInt(value[index + 3]);
                int num3 = UriUtility.HexToInt(value[index + 4]);
                int num4 = UriUtility.HexToInt(value[index + 5]);
                if (num1 >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0)
                {
                  char ch2 = (char) (num1 << 12 | num2 << 8 | num3 << 4 | num4);
                  index += 5;
                  urlDecoder.AddChar(ch2);
                  break;
                }
                goto default;
              }
              else
              {
                int num5 = UriUtility.HexToInt(value[index + 1]);
                int num6 = UriUtility.HexToInt(value[index + 2]);
                if (num5 >= 0 && num6 >= 0)
                {
                  byte b = (byte) (num5 << 4 | num6);
                  index += 2;
                  urlDecoder.AddByte(b);
                  break;
                }
                goto default;
              }
            }
            else
              goto default;
          case '+':
            ch1 = ' ';
            goto default;
          default:
            if (((int) ch1 & 65408) == 0)
            {
              urlDecoder.AddByte((byte) ch1);
              break;
            }
            urlDecoder.AddChar(ch1);
            break;
        }
      }
      return urlDecoder.GetString();
    }

    private static byte[] UrlDecodeInternal(byte[] bytes, int offset, int count)
    {
      if (!UriUtility.ValidateUrlEncodingParameters(bytes, offset, count))
        return (byte[]) null;
      int length = 0;
      byte[] sourceArray = new byte[count];
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = offset + index1;
        byte num1 = bytes[index2];
        switch (num1)
        {
          case 37:
            if (index1 < count - 2)
            {
              int num2 = UriUtility.HexToInt((char) bytes[index2 + 1]);
              int num3 = UriUtility.HexToInt((char) bytes[index2 + 2]);
              if (num2 >= 0 && num3 >= 0)
              {
                num1 = (byte) (num2 << 4 | num3);
                index1 += 2;
                break;
              }
              break;
            }
            break;
          case 43:
            num1 = (byte) 32;
            break;
        }
        sourceArray[length++] = num1;
      }
      if (length < sourceArray.Length)
      {
        byte[] destinationArray = new byte[length];
        Array.Copy((Array) sourceArray, (Array) destinationArray, length);
        sourceArray = destinationArray;
      }
      return sourceArray;
    }

    private static string UrlDecodeInternal(
      byte[] bytes,
      int offset,
      int count,
      Encoding encoding)
    {
      if (!UriUtility.ValidateUrlEncodingParameters(bytes, offset, count))
        return (string) null;
      UriUtility.UrlDecoder urlDecoder = new UriUtility.UrlDecoder(count, encoding);
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = offset + index1;
        byte b = bytes[index2];
        switch (b)
        {
          case 37:
            if (index1 < count - 2)
            {
              if (bytes[index2 + 1] == (byte) 117 && index1 < count - 5)
              {
                int num1 = UriUtility.HexToInt((char) bytes[index2 + 2]);
                int num2 = UriUtility.HexToInt((char) bytes[index2 + 3]);
                int num3 = UriUtility.HexToInt((char) bytes[index2 + 4]);
                int num4 = UriUtility.HexToInt((char) bytes[index2 + 5]);
                if (num1 >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0)
                {
                  char ch = (char) (num1 << 12 | num2 << 8 | num3 << 4 | num4);
                  index1 += 5;
                  urlDecoder.AddChar(ch);
                  break;
                }
                goto default;
              }
              else
              {
                int num5 = UriUtility.HexToInt((char) bytes[index2 + 1]);
                int num6 = UriUtility.HexToInt((char) bytes[index2 + 2]);
                if (num5 >= 0 && num6 >= 0)
                {
                  b = (byte) (num5 << 4 | num6);
                  index1 += 2;
                  goto default;
                }
                else
                  goto default;
              }
            }
            else
              goto default;
          case 43:
            b = (byte) 32;
            goto default;
          default:
            urlDecoder.AddByte(b);
            break;
        }
      }
      return urlDecoder.GetString();
    }

    public static string UrlDecode(string str) => str == null ? (string) null : UriUtility.UrlDecode(str, Encoding.UTF8);

    public static string Base64Decode(string str) => str == null ? (string) null : Encoding.UTF8.GetString(Convert.FromBase64String(str));

    public static string UrlDecode(string str, Encoding e) => UriUtility.UrlDecodeInternal(str, e);

    public static string UrlDecode(byte[] bytes, Encoding e) => bytes == null ? (string) null : UriUtility.UrlDecode(bytes, 0, bytes.Length, e);

    public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e) => UriUtility.UrlDecodeInternal(bytes, offset, count, e);

    public static byte[] UrlDecodeToBytes(string str) => str == null ? (byte[]) null : UriUtility.UrlDecodeToBytes(str, Encoding.UTF8);

    public static byte[] UrlDecodeToBytes(string str, Encoding e) => str == null ? (byte[]) null : UriUtility.UrlDecodeToBytes(e.GetBytes(str));

    public static byte[] UrlDecodeToBytes(byte[] bytes) => bytes == null ? (byte[]) null : UriUtility.UrlDecodeToBytes(bytes, 0, bytes != null ? bytes.Length : 0);

    public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count) => UriUtility.UrlDecodeInternal(bytes, offset, count);

    public static int HexToInt(char h)
    {
      if (h >= '0' && h <= '9')
        return (int) h - 48;
      if (h >= 'a' && h <= 'f')
        return (int) h - 97 + 10;
      return h < 'A' || h > 'F' ? -1 : (int) h - 65 + 10;
    }

    public static char IntToHex(int n) => n <= 9 ? (char) (n + 48) : (char) (n - 10 + 97);

    public static bool IsUrlSafeChar(char ch)
    {
      if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch >= '0' && ch <= '9' || ch == '!')
        return true;
      switch (ch)
      {
        case '(':
        case ')':
        case '*':
        case '-':
        case '.':
        case '_':
          return true;
        default:
          return false;
      }
    }

    internal static string UrlEncodeSpaces(string str)
    {
      if (str != null && str.IndexOf(' ') >= 0)
        str = str.Replace(" ", "%20");
      return str;
    }

    private static bool ValidateUrlEncodingParameters(byte[] bytes, int offset, int count)
    {
      if (bytes == null && count == 0)
        return false;
      if (bytes == null)
        throw new ArgumentNullException(nameof (bytes));
      if (offset < 0 || offset > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0 || offset + count > bytes.Length)
        throw new ArgumentOutOfRangeException(nameof (count));
      return true;
    }

    private static bool IsNonAsciiByte(byte b) => b >= (byte) 127 || b < (byte) 32;

    public static string HtmlDecode(string value)
    {
      if (string.IsNullOrEmpty(value) || value.IndexOf('&') < 0)
        return value;
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        UriUtility.HtmlDecode(value, (TextWriter) output);
        return output.ToString();
      }
    }

    public static void HtmlDecode(string value, TextWriter output)
    {
      if (value == null)
        return;
      if (output == null)
        throw new ArgumentNullException(nameof (output));
      if (value.IndexOf('&') < 0)
      {
        output.Write(value);
      }
      else
      {
        int length = value.Length;
        for (int index1 = 0; index1 < length; ++index1)
        {
          char ch1 = value[index1];
          if (ch1 == '&')
          {
            int index2 = value.IndexOfAny(UriUtility._htmlEntityEndingChars, index1 + 1);
            if (index2 > 0 && value[index2] == ';')
            {
              string entity = value.Substring(index1 + 1, index2 - index1 - 1);
              if (entity.Length > 1 && entity[0] == '#')
              {
                ushort result;
                if (entity[1] == 'x' || entity[1] == 'X')
                  ushort.TryParse(entity.Substring(2), NumberStyles.AllowHexSpecifier, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result);
                else
                  ushort.TryParse(entity.Substring(1), NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result);
                if (result != (ushort) 0)
                {
                  ch1 = (char) result;
                  index1 = index2;
                }
              }
              else
              {
                index1 = index2;
                char ch2 = UriUtility.HtmlEntities.Lookup(entity);
                if (ch2 != char.MinValue)
                {
                  ch1 = ch2;
                }
                else
                {
                  output.Write('&');
                  output.Write(entity);
                  output.Write(';');
                  continue;
                }
              }
            }
          }
          output.Write(ch1);
        }
      }
    }

    private class _AbsoluteUriStringComparer : IEqualityComparer<Uri>
    {
      public bool Equals(Uri x, Uri y) => VssStringComparer.Url.Equals(x != (Uri) null ? UriUtility.GetInvariantAbsoluteUri(x) : (string) null, y != (Uri) null ? UriUtility.GetInvariantAbsoluteUri(y) : (string) null);

      public int GetHashCode(Uri obj) => UriUtility.GetInvariantAbsoluteUri(obj).GetHashCode();
    }

    private class _UrlPathIgnoreSeparatorsComparer : IEqualityComparer<string>
    {
      public bool Equals(string x, string y) => VssStringComparer.UrlPath.Equals(UriUtility.TrimPathSeparators(x), UriUtility.TrimPathSeparators(y));

      public int GetHashCode(string obj) => VssStringComparer.UrlPath.GetHashCode(UriUtility.TrimPathSeparators(obj));
    }

    [Serializable]
    internal class HttpValueCollection : NameValueCollection
    {
      internal HttpValueCollection()
        : base((IEqualityComparer) StringComparer.OrdinalIgnoreCase)
      {
      }

      internal HttpValueCollection(string str, bool readOnly, bool urlencoded, Encoding encoding)
        : base((IEqualityComparer) StringComparer.OrdinalIgnoreCase)
      {
        if (!string.IsNullOrEmpty(str))
          this.FillFromString(str, urlencoded, encoding);
        this.IsReadOnly = readOnly;
      }

      internal HttpValueCollection(int capacity)
        : base(capacity, (IEqualityComparer) StringComparer.OrdinalIgnoreCase)
      {
      }

      protected HttpValueCollection(SerializationInfo info, StreamingContext context)
        : base(info, context)
      {
      }

      internal void MakeReadOnly() => this.IsReadOnly = true;

      internal void MakeReadWrite() => this.IsReadOnly = false;

      internal void FillFromString(string s) => this.FillFromString(s, false, (Encoding) null);

      internal void FillFromString(string s, bool urlencoded, Encoding encoding)
      {
        int length = s != null ? s.Length : 0;
        for (int index = 0; index < length; ++index)
        {
          int startIndex = index;
          int num = -1;
          for (; index < length; ++index)
          {
            switch (s[index])
            {
              case '&':
                goto label_7;
              case '=':
                if (num < 0)
                {
                  num = index;
                  break;
                }
                break;
            }
          }
label_7:
          string str1 = (string) null;
          string str2;
          if (num >= 0)
          {
            str1 = s.Substring(startIndex, num - startIndex);
            str2 = s.Substring(num + 1, index - num - 1);
          }
          else
            str2 = s.Substring(startIndex, index - startIndex);
          if (urlencoded)
            this.Add(UriUtility.UrlDecode(str1, encoding), UriUtility.UrlDecode(str2, encoding));
          else
            this.Add(str1, str2);
          if (index == length - 1 && s[index] == '&')
            this.Add((string) null, string.Empty);
        }
      }

      internal void FillFromEncodedBytes(byte[] bytes, Encoding encoding)
      {
        int length = bytes != null ? bytes.Length : 0;
        for (int index = 0; index < length; ++index)
        {
          int offset = index;
          int num = -1;
          for (; index < length; ++index)
          {
            switch (bytes[index])
            {
              case 38:
                goto label_7;
              case 61:
                if (num < 0)
                {
                  num = index;
                  break;
                }
                break;
            }
          }
label_7:
          string name;
          string str;
          if (num >= 0)
          {
            name = UriUtility.UrlDecode(bytes, offset, num - offset, encoding);
            str = UriUtility.UrlDecode(bytes, num + 1, index - num - 1, encoding);
          }
          else
          {
            name = (string) null;
            str = UriUtility.UrlDecode(bytes, offset, index - offset, encoding);
          }
          this.Add(name, str);
          if (index == length - 1 && bytes[index] == (byte) 38)
            this.Add((string) null, string.Empty);
        }
      }

      internal void Reset() => this.Clear();

      public override string ToString() => this.ToString(true);

      internal virtual string ToString(bool urlencoded) => this.ToString(urlencoded, (IDictionary) null);

      internal virtual string ToString(bool urlencoded, IDictionary excludeKeys)
      {
        int count1 = this.Count;
        if (count1 == 0)
          return string.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        for (int index1 = 0; index1 < count1; ++index1)
        {
          string key = this.GetKey(index1);
          if (excludeKeys == null || key == null || excludeKeys[(object) key] == null)
          {
            if (urlencoded)
              key = UriUtility.UrlEncodeUnicode(key);
            string str1 = key != null ? key + "=" : string.Empty;
            ArrayList arrayList = (ArrayList) this.BaseGet(index1);
            int count2 = arrayList != null ? arrayList.Count : 0;
            if (stringBuilder.Length > 0)
              stringBuilder.Append('&');
            switch (count2)
            {
              case 0:
                stringBuilder.Append(str1);
                continue;
              case 1:
                stringBuilder.Append(str1);
                string str2 = (string) arrayList[0];
                if (urlencoded)
                  str2 = UriUtility.UrlEncodeUnicode(str2);
                stringBuilder.Append(str2);
                continue;
              default:
                for (int index2 = 0; index2 < count2; ++index2)
                {
                  if (index2 > 0)
                    stringBuilder.Append('&');
                  stringBuilder.Append(str1);
                  string str3 = (string) arrayList[index2];
                  if (urlencoded)
                    str3 = UriUtility.UrlEncodeUnicode(str3);
                  stringBuilder.Append(str3);
                }
                continue;
            }
          }
        }
        return stringBuilder.ToString();
      }
    }

    private class UrlDecoder
    {
      private int _bufferSize;
      private int _numChars;
      private char[] _charBuffer;
      private int _numBytes;
      private byte[] _byteBuffer;
      private Encoding _encoding;

      private void FlushBytes()
      {
        if (this._numBytes <= 0)
          return;
        this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
        this._numBytes = 0;
      }

      internal UrlDecoder(int bufferSize, Encoding encoding)
      {
        this._bufferSize = bufferSize;
        this._encoding = encoding;
        this._charBuffer = new char[bufferSize];
      }

      internal void AddChar(char ch)
      {
        if (this._numBytes > 0)
          this.FlushBytes();
        this._charBuffer[this._numChars++] = ch;
      }

      internal void AddByte(byte b)
      {
        if (this._byteBuffer == null)
          this._byteBuffer = new byte[this._bufferSize];
        this._byteBuffer[this._numBytes++] = b;
      }

      internal string GetString()
      {
        if (this._numBytes > 0)
          this.FlushBytes();
        return this._numChars > 0 ? new string(this._charBuffer, 0, this._numChars) : string.Empty;
      }
    }

    private static class HtmlEntities
    {
      private static string[] _entitiesList = new string[253]
      {
        "\"-quot",
        "&-amp",
        "'-apos",
        "<-lt",
        ">-gt",
        " -nbsp",
        "¡-iexcl",
        "¢-cent",
        "£-pound",
        "¤-curren",
        "¥-yen",
        "¦-brvbar",
        "§-sect",
        "¨-uml",
        "©-copy",
        "ª-ordf",
        "«-laquo",
        "¬-not",
        "\u00AD-shy",
        "®-reg",
        "¯-macr",
        "°-deg",
        "±-plusmn",
        "\u00B2-sup2",
        "\u00B3-sup3",
        "´-acute",
        "µ-micro",
        "¶-para",
        "·-middot",
        "¸-cedil",
        "\u00B9-sup1",
        "º-ordm",
        "»-raquo",
        "\u00BC-frac14",
        "\u00BD-frac12",
        "\u00BE-frac34",
        "¿-iquest",
        "À-Agrave",
        "Á-Aacute",
        "Â-Acirc",
        "Ã-Atilde",
        "Ä-Auml",
        "Å-Aring",
        "Æ-AElig",
        "Ç-Ccedil",
        "È-Egrave",
        "É-Eacute",
        "Ê-Ecirc",
        "Ë-Euml",
        "Ì-Igrave",
        "Í-Iacute",
        "Î-Icirc",
        "Ï-Iuml",
        "Ð-ETH",
        "Ñ-Ntilde",
        "Ò-Ograve",
        "Ó-Oacute",
        "Ô-Ocirc",
        "Õ-Otilde",
        "Ö-Ouml",
        "×-times",
        "Ø-Oslash",
        "Ù-Ugrave",
        "Ú-Uacute",
        "Û-Ucirc",
        "Ü-Uuml",
        "Ý-Yacute",
        "Þ-THORN",
        "ß-szlig",
        "à-agrave",
        "á-aacute",
        "â-acirc",
        "ã-atilde",
        "ä-auml",
        "å-aring",
        "æ-aelig",
        "ç-ccedil",
        "è-egrave",
        "é-eacute",
        "ê-ecirc",
        "ë-euml",
        "ì-igrave",
        "í-iacute",
        "î-icirc",
        "ï-iuml",
        "ð-eth",
        "ñ-ntilde",
        "ò-ograve",
        "ó-oacute",
        "ô-ocirc",
        "õ-otilde",
        "ö-ouml",
        "÷-divide",
        "ø-oslash",
        "ù-ugrave",
        "ú-uacute",
        "û-ucirc",
        "ü-uuml",
        "ý-yacute",
        "þ-thorn",
        "ÿ-yuml",
        "Œ-OElig",
        "œ-oelig",
        "Š-Scaron",
        "š-scaron",
        "Ÿ-Yuml",
        "ƒ-fnof",
        "ˆ-circ",
        "˜-tilde",
        "Α-Alpha",
        "Β-Beta",
        "Γ-Gamma",
        "Δ-Delta",
        "Ε-Epsilon",
        "Ζ-Zeta",
        "Η-Eta",
        "Θ-Theta",
        "Ι-Iota",
        "Κ-Kappa",
        "Λ-Lambda",
        "Μ-Mu",
        "Ν-Nu",
        "Ξ-Xi",
        "Ο-Omicron",
        "Π-Pi",
        "Ρ-Rho",
        "Σ-Sigma",
        "Τ-Tau",
        "Υ-Upsilon",
        "Φ-Phi",
        "Χ-Chi",
        "Ψ-Psi",
        "Ω-Omega",
        "α-alpha",
        "β-beta",
        "γ-gamma",
        "δ-delta",
        "ε-epsilon",
        "ζ-zeta",
        "η-eta",
        "θ-theta",
        "ι-iota",
        "κ-kappa",
        "λ-lambda",
        "μ-mu",
        "ν-nu",
        "ξ-xi",
        "ο-omicron",
        "π-pi",
        "ρ-rho",
        "ς-sigmaf",
        "σ-sigma",
        "τ-tau",
        "υ-upsilon",
        "φ-phi",
        "χ-chi",
        "ψ-psi",
        "ω-omega",
        "ϑ-thetasym",
        "ϒ-upsih",
        "ϖ-piv",
        " -ensp",
        " -emsp",
        " -thinsp",
        "\u200C-zwnj",
        "\u200D-zwj",
        "\u200E-lrm",
        "\u200F-rlm",
        "–-ndash",
        "—-mdash",
        "‘-lsquo",
        "’-rsquo",
        "‚-sbquo",
        "“-ldquo",
        "”-rdquo",
        "„-bdquo",
        "†-dagger",
        "‡-Dagger",
        "•-bull",
        "…-hellip",
        "‰-permil",
        "′-prime",
        "″-Prime",
        "‹-lsaquo",
        "›-rsaquo",
        "‾-oline",
        "⁄-frasl",
        "€-euro",
        "ℑ-image",
        "℘-weierp",
        "ℜ-real",
        "™-trade",
        "ℵ-alefsym",
        "←-larr",
        "↑-uarr",
        "→-rarr",
        "↓-darr",
        "↔-harr",
        "↵-crarr",
        "⇐-lArr",
        "⇑-uArr",
        "⇒-rArr",
        "⇓-dArr",
        "⇔-hArr",
        "∀-forall",
        "∂-part",
        "∃-exist",
        "∅-empty",
        "∇-nabla",
        "∈-isin",
        "∉-notin",
        "∋-ni",
        "∏-prod",
        "∑-sum",
        "−-minus",
        "∗-lowast",
        "√-radic",
        "∝-prop",
        "∞-infin",
        "∠-ang",
        "∧-and",
        "∨-or",
        "∩-cap",
        "∪-cup",
        "∫-int",
        "∴-there4",
        "∼-sim",
        "≅-cong",
        "≈-asymp",
        "≠-ne",
        "≡-equiv",
        "≤-le",
        "≥-ge",
        "⊂-sub",
        "⊃-sup",
        "⊄-nsub",
        "⊆-sube",
        "⊇-supe",
        "⊕-oplus",
        "⊗-otimes",
        "⊥-perp",
        "⋅-sdot",
        "⌈-lceil",
        "⌉-rceil",
        "⌊-lfloor",
        "⌋-rfloor",
        "〈-lang",
        "〉-rang",
        "◊-loz",
        "♠-spades",
        "♣-clubs",
        "♥-hearts",
        "♦-diams"
      };
      private static Dictionary<string, char> _lookupTable = UriUtility.HtmlEntities.GenerateLookupTable();

      private static Dictionary<string, char> GenerateLookupTable()
      {
        Dictionary<string, char> lookupTable = new Dictionary<string, char>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (string entities in UriUtility.HtmlEntities._entitiesList)
          lookupTable.Add(entities.Substring(2), entities[0]);
        return lookupTable;
      }

      public static char Lookup(string entity)
      {
        char ch;
        UriUtility.HtmlEntities._lookupTable.TryGetValue(entity, out ch);
        return ch;
      }
    }
  }
}
