// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Base64UrlStringCodec
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  public class Base64UrlStringCodec
  {
    public static string ToBase64UrlString(byte[] bytes) => Convert.ToBase64String(bytes).Replace('+', '-').Replace('/', '_').Replace("=", string.Empty);

    public static byte[] FromBase64UrlString(string s)
    {
      int count = 4 - s.Length % 4;
      if (count != 4)
        s += new string('=', count);
      return Convert.FromBase64String(s.Replace('_', '/').Replace('-', '+'));
    }
  }
}
