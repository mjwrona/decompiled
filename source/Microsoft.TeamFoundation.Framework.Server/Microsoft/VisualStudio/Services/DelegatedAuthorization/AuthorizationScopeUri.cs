// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationScopeUri
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationScopeUri
  {
    private const string HostComponentWildcard = "_";
    private const char HostComponentDelimiterCharacter = '.';
    private const char FragmentSeparatorCharacter = '#';
    private const char MethodDelimiterCharacter = '+';
    private const char PathComponentDelimiterCharacter = '/';
    private const string PathComponentWildcard = "*";
    private const string QuerySeparator = "?";
    private const string DefaultCollectionPattern = "DefaultCollection";
    private static readonly HttpMethod[] ValidMethods = new HttpMethod[7]
    {
      HttpMethod.Options,
      HttpMethod.Delete,
      HttpMethod.Post,
      HttpMethod.Head,
      HttpMethod.Put,
      HttpMethod.Get,
      new HttpMethod("PATCH")
    };
    private static readonly Regex WellKnownScopePattern = new Regex("^[a-zA-Z0-9.-_]+$", RegexOptions.Compiled);

    internal AuthorizationScopeUri(Uri uri, params HttpMethod[] methods)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException("URI");
      this.Initialize(uri, methods);
    }

    internal AuthorizationScopeUri(string scopeUri)
    {
      if (scopeUri == null)
        throw new ArgumentNullException("scope URI");
      if (string.IsNullOrWhiteSpace(scopeUri))
        throw new ArgumentException("Scope URI is required: '" + scopeUri + "'");
      Uri result = (Uri) null;
      if (!Uri.TryCreate(scopeUri, UriKind.RelativeOrAbsolute, out result))
        throw new ArgumentException("Scope URI must be a valid absolute or relative URI: '" + scopeUri + "'");
      string str1 = (string) null;
      if (result.IsAbsoluteUri)
      {
        str1 = result.Fragment;
      }
      else
      {
        string[] strArray = result.OriginalString.Split('#');
        if (strArray.Length == 2)
        {
          str1 = '#'.ToString() + strArray[1];
          if (!Uri.TryCreate(strArray[0], UriKind.RelativeOrAbsolute, out result))
            throw new ArgumentException("Scope URI without fragment must be a valid absolute or relative URI: '" + scopeUri + "'");
        }
      }
      HttpMethod[] methods = (HttpMethod[]) null;
      if (!string.IsNullOrWhiteSpace(str1))
      {
        string str2 = str1.Substring(1).Trim();
        if (str2 == "*")
        {
          methods = AuthorizationScopeUri.ValidMethods;
        }
        else
        {
          IEnumerable<HttpMethod> source = ((IEnumerable<string>) str2.Split(new char[1]
          {
            '+'
          }, StringSplitOptions.RemoveEmptyEntries)).Select<string, HttpMethod>((Func<string, HttpMethod>) (method => ((IEnumerable<HttpMethod>) AuthorizationScopeUri.ValidMethods).FirstOrDefault<HttpMethod>((Func<HttpMethod, bool>) (validMethod => string.Equals(validMethod.ToString(), method, StringComparison.OrdinalIgnoreCase))))).Where<HttpMethod>((Func<HttpMethod, bool>) (method => method != (HttpMethod) null));
          if (source.Any<HttpMethod>())
            methods = source.Distinct<HttpMethod>().ToArray<HttpMethod>();
        }
      }
      this.Initialize(result, methods);
    }

    private void Initialize(Uri uri, HttpMethod[] methods)
    {
      string input = Uri.UnescapeDataString(uri.OriginalString);
      if (input.Contains("?"))
        throw new ArgumentException("Scope URI must not contain a query: '" + input + "'");
      if (!uri.IsAbsoluteUri)
      {
        if (input[0] == '/')
        {
          this.IsAbsolutePath = true;
        }
        else
        {
          if (!AuthorizationScopeUri.WellKnownScopePattern.IsMatch(input))
            throw new ArgumentException("Relative scope URI must be an absolute path or well-known scope: '" + input + "'");
          this.IsWellKnownScope = true;
        }
      }
      else
      {
        if (!Uri.UriSchemeHttp.Equals(uri.Scheme) && !Uri.UriSchemeHttps.Equals(uri.Scheme))
          throw new ArgumentException("Absolute scope URI must use the 'http' or 'https' scheme: '" + input + "'");
        if (!uri.Authority.Equals(uri.Host))
          throw new ArgumentException("Absolute scope URI must not specify a port: '" + input + "'");
        string[] array = ((IEnumerable<string>) uri.Host.Split('.')).Reverse<string>().ToArray<string>();
        if (((IEnumerable<string>) array).Contains<string>("_"))
          this.HostPatternParts = (IReadOnlyList<string>) array;
      }
      if (this.IsAbsolutePath || uri.IsAbsoluteUri)
        this.PathPatternParts = (IReadOnlyList<string>) ((IEnumerable<string>) (this.IsAbsolutePath ? uri.OriginalString : uri.AbsolutePath).Substring(1).Split('/')).Select<string, string>((Func<string, string>) (part => !(part == "*") ? Uri.UnescapeDataString(part) : part)).ToArray<string>();
      this.Methods = (IReadOnlyList<HttpMethod>) methods;
      this.Uri = uri;
    }

    public IReadOnlyList<string> HostPatternParts { get; protected set; }

    public bool IsAbsolutePath { get; protected set; }

    public bool IsWellKnownScope { get; protected set; }

    public IReadOnlyList<HttpMethod> Methods { get; protected set; }

    public IReadOnlyList<string> PathPatternParts { get; protected set; }

    public Uri Uri { get; protected set; }

    public bool Match(string matchUri) => this.Match(matchUri, (HttpMethod) null);

    public bool Match(string matchUri, HttpMethod method)
    {
      if (matchUri == null)
        throw new ArgumentNullException("match URI");
      if (string.IsNullOrWhiteSpace(matchUri))
        throw new ArgumentException("Match URI is required: '" + matchUri + "'");
      Uri result = (Uri) null;
      if (!Uri.TryCreate(matchUri, UriKind.RelativeOrAbsolute, out result))
        throw new ArgumentException("Match URI must be a valid absolute or relative URI: '" + matchUri + "'");
      return this.Match(result, method);
    }

    public bool Match(Uri uri, HttpMethod method)
    {
      if (uri == (Uri) null)
        throw new ArgumentNullException("URI");
      if (method != (HttpMethod) null)
      {
        int num = this.Methods != null ? 0 : (method == HttpMethod.Get ? 1 : 0);
        bool flag = this.Methods != null && this.Methods.Count > 0;
        if (num == 0 && (!flag || !this.Methods.Contains<HttpMethod>(method)))
          return false;
      }
      if (this.IsWellKnownScope)
        return !uri.IsAbsoluteUri && string.Equals(this.Uri.OriginalString, uri.OriginalString, StringComparison.OrdinalIgnoreCase);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string[] array = ((IEnumerable<string>) (uri.IsAbsoluteUri ? uri.AbsolutePath : uri.OriginalString).Substring(1).Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>(AuthorizationScopeUri.\u003C\u003EO.\u003C0\u003E__UnescapeDataString ?? (AuthorizationScopeUri.\u003C\u003EO.\u003C0\u003E__UnescapeDataString = new Func<string, string>(Uri.UnescapeDataString))).ToArray<string>();
      int count = this.PathPatternParts.Count;
      int num1 = array.Length - 1;
      int index1 = 0;
      int index2 = 0;
      while (index1 < count)
      {
        string pathPatternPart = this.PathPatternParts[index1];
        if (!(pathPatternPart == "*"))
        {
          if (index2 > num1)
            return false;
          Guid result = new Guid();
          if (index1 == 0 && string.Equals(pathPatternPart, "DefaultCollection", StringComparison.Ordinal))
          {
            if ((char.ToUpperInvariant(array[index2][0]) != 'A' || !Guid.TryParse(array[index2].Substring(1), out result)) && index2 == 0 && !string.Equals(array[index2], "DefaultCollection", StringComparison.OrdinalIgnoreCase))
              --index2;
          }
          else if (index1 == 0 && char.ToUpperInvariant(array[index2][0]) == 'A' && Guid.TryParse(array[index2].Substring(1), out result))
            --index1;
          else if (!string.Equals(pathPatternPart, array[index2], StringComparison.OrdinalIgnoreCase))
            return false;
        }
        ++index1;
        ++index2;
      }
      if (this.IsAbsolutePath)
        return true;
      return uri.IsAbsoluteUri && (Uri.UriSchemeHttps.Equals(uri.Scheme) || Uri.UriSchemeHttp.Equals(this.Uri.Scheme)) && this.MatchHost(uri.Host);
    }

    public bool MatchHost(string host)
    {
      if (this.IsWellKnownScope || this.IsAbsolutePath)
        return true;
      if (string.IsNullOrWhiteSpace(host))
        return false;
      if (this.HostPatternParts == null)
        return string.Equals(this.Uri.Host, host, StringComparison.OrdinalIgnoreCase) || host.EndsWith('.'.ToString() + this.Uri.Host, StringComparison.OrdinalIgnoreCase);
      string[] array = ((IEnumerable<string>) host.Split('.')).Reverse<string>().ToArray<string>();
      int count = this.HostPatternParts.Count;
      int num = array.Length - 1;
      for (int index = 0; index < count; ++index)
      {
        string hostPatternPart = this.HostPatternParts[index];
        if (!(hostPatternPart == "_") && (index > num || !string.Equals(hostPatternPart, array[index], StringComparison.OrdinalIgnoreCase)))
          return false;
      }
      return true;
    }

    public override string ToString() => this.Uri.OriginalString;

    public static bool TryCreate(string uri, out AuthorizationScopeUri scopeUri)
    {
      scopeUri = (AuthorizationScopeUri) null;
      try
      {
        scopeUri = new AuthorizationScopeUri(uri);
      }
      catch
      {
        return false;
      }
      return true;
    }
  }
}
