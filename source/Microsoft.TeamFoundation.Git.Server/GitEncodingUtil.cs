// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitEncodingUtil
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitEncodingUtil
  {
    public static readonly Encoding SafeAscii = Encoding.GetEncoding(Encoding.ASCII.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
    public static readonly Encoding SafeUtf8NoBom = (Encoding) new UTF8Encoding(false, true);
    public static readonly Encoding SafeUtf8WithBom = (Encoding) new UTF8Encoding(true, true);
    public static readonly Encoding BestEffortUtf8NoBom = (Encoding) new UTF8Encoding(false, false);

    public static Encoding GetSafeEncoding(int codePage, bool byteOrderMark)
    {
      if (codePage == Encoding.Unicode.CodePage)
        return (Encoding) new UnicodeEncoding(false, byteOrderMark, true);
      if (codePage == Encoding.BigEndianUnicode.CodePage)
        return (Encoding) new UnicodeEncoding(true, byteOrderMark, true);
      if (codePage == Encoding.UTF32.CodePage)
        return (Encoding) new UTF32Encoding(false, byteOrderMark, true);
      if (codePage != Encoding.UTF8.CodePage)
        return Encoding.GetEncoding(codePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
      return !byteOrderMark ? GitEncodingUtil.SafeUtf8NoBom : GitEncodingUtil.SafeUtf8WithBom;
    }
  }
}
