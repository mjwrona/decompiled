// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.UriSafeLocatorCodec
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class UriSafeLocatorCodec
  {
    private const int AllowedCharTableSize = 127;
    private static readonly ImmutableArray<bool> AllowedCharTable = Enumerable.Range(0, (int) sbyte.MaxValue).Select<int, bool>((Func<int, bool>) (idx => UriSafeLocatorCodec.IsCharAllowedUnescapedInternal((char) idx))).ToImmutableArray<bool>();

    public UriSafeEncodedLocator Encode(Locator locator) => new UriSafeEncodedLocator(locator.PathSegments.Select<string, string>(UriSafeLocatorCodec.\u003C\u003EO.\u003C0\u003E__EncodeSegment ?? (UriSafeLocatorCodec.\u003C\u003EO.\u003C0\u003E__EncodeSegment = new Func<string, string>(UriSafeLocatorCodec.EncodeSegment))));

    public Locator Decode(UriSafeEncodedLocator encodedLocator) => new Locator(encodedLocator.PathSegments.Select<string, string>(UriSafeLocatorCodec.\u003C\u003EO.\u003C1\u003E__DecodeSegment ?? (UriSafeLocatorCodec.\u003C\u003EO.\u003C1\u003E__DecodeSegment = new Func<string, string>(UriSafeLocatorCodec.DecodeSegment))));

    private static string EncodeSegment(string s) => !UriSafeLocatorCodec.NeedsEncoding(s) ? s : "~" + Base64UrlStringCodec.ToBase64UrlString(Encoding.UTF8.GetBytes(s));

    private static string DecodeSegment(string s) => !UriSafeLocatorCodec.NeedsDecoding(s) ? s : Encoding.UTF8.GetString(Base64UrlStringCodec.FromBase64UrlString(s.Substring(1)));

    private static bool IsCharRfc2234Alpha(char ch)
    {
      if (ch >= 'A' && ch <= 'Z')
        return true;
      return ch >= 'a' && ch <= 'z';
    }

    private static bool IsCharRfc2234Digit(char ch) => ch >= '0' && ch <= '9';

    private static bool IsCharUriUnreserved(char ch) => UriSafeLocatorCodec.IsCharRfc2234Alpha(ch) || UriSafeLocatorCodec.IsCharRfc2234Digit(ch) || ch == '-' || ch == '.' || ch == '_' || ch == '~';

    private static bool IsCharAllowedUnescapedInternal(char ch) => UriSafeLocatorCodec.IsCharUriUnreserved(ch) || ch == '@';

    private static bool IsCharAllowedUnescaped(char ch) => ch < '\u007F' && UriSafeLocatorCodec.AllowedCharTable[(int) ch];

    private static bool NeedsEncoding(string s) => s.StartsWith("~") || s.Any<char>((Func<char, bool>) (ch => !UriSafeLocatorCodec.IsCharAllowedUnescaped(ch)));

    private static bool NeedsDecoding(string s) => s.StartsWith("~");
  }
}
