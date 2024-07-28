// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.HashUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public static class HashUtils
  {
    public static string ComputeUnsecureHashForString(string value) => value != null ? HashUtils.ComputeUnsecureHashForBytes(Encoding.UTF8.GetBytes(value)) : throw new ArgumentNullException(nameof (value));

    [SuppressMessage("Microsoft.Cryptographic.Standard", "CA5354:SHA1CannotBeUsed", Target = "sha1", Justification = "Content Id generation uses this method and changing the hashing algorithm can result in re-indexing. Do not use this method for implementing security critical code.")]
    public static string ComputeUnsecureHashForBytes(byte[] byteContent)
    {
      if (byteContent == null)
        throw new ArgumentNullException(nameof (byteContent));
      byte[] hash;
      using (SHA1 shA1 = SHA1.Create())
        hash = shA1.ComputeHash(byteContent);
      return HashUtils.HexStringFromBytes(hash);
    }

    private static string HexStringFromBytes(byte[] byteArray)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in byteArray)
      {
        string str = num.ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder.Append(str);
      }
      return stringBuilder.ToString();
    }
  }
}
