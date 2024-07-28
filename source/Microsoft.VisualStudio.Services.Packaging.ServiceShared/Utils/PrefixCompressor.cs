// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PrefixCompressor
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class PrefixCompressor
  {
    private bool firstCall = true;
    private string state = "";

    public string EncodeNext(string s)
    {
      if (s == null)
        throw new ArgumentNullException(nameof (s));
      if (this.firstCall)
      {
        this.firstCall = false;
        this.state = s;
        return s;
      }
      int prefixLength = PrefixCompressor.GetPrefixLength(this.state, s);
      this.state = s;
      return PrefixCompressor.EncodePrefixLength(prefixLength).ToString() + s.Substring(prefixLength);
    }

    private static int GetPrefixLength(string a, string b)
    {
      int prefixLength = Math.Min(61, Math.Min(a.Length, b.Length));
      for (int index = 0; index < prefixLength; ++index)
      {
        if ((int) a[index] != (int) b[index])
          return index;
      }
      return prefixLength;
    }

    private static char EncodePrefixLength(int length)
    {
      if (length >= 0 && length <= 9)
        return (char) (48 + length);
      if (length >= 10 && length <= 35)
        return (char) (65 + (length - 10));
      if (length >= 36 && length <= 61)
        return (char) (97 + (length - 36));
      throw new ArgumentOutOfRangeException(nameof (length), (object) length, "Cannot encode prefix length");
    }
  }
}
