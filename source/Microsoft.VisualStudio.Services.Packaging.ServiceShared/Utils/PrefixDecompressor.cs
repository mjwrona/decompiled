// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PrefixDecompressor
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class PrefixDecompressor
  {
    private bool firstCall = true;
    private char[] state = new char[100];
    private int stateValidLength;

    public string DecodeNext(string s)
    {
      if (s == null)
        throw new ArgumentNullException(nameof (s));
      if (this.firstCall)
      {
        this.firstCall = false;
        s.CopyTo(0, this.state, 0, s.Length);
        this.stateValidLength = s.Length;
        return s;
      }
      int num = s.Length >= 1 ? PrefixDecompressor.DecodePrefixLength(s[0]) : throw new ArgumentException("String must contain a prefix-length character");
      if (num > this.stateValidLength)
        num = this.stateValidLength;
      int length = num + s.Length - 1;
      if (this.state.Length < length)
      {
        char[] destinationArray = new char[length];
        Array.Copy((Array) this.state, (Array) destinationArray, num);
        this.state = destinationArray;
      }
      s.CopyTo(1, this.state, num, s.Length - 1);
      this.stateValidLength = length;
      return new string(this.state, 0, this.stateValidLength);
    }

    private static int DecodePrefixLength(char prefixChar)
    {
      if (prefixChar >= '0' && prefixChar <= '9')
        return (int) prefixChar - 48;
      if (prefixChar >= 'A' && prefixChar <= 'Z')
        return 10 + ((int) prefixChar - 65);
      if (prefixChar >= 'a' && prefixChar <= 'z')
        return 36 + ((int) prefixChar - 97);
      throw new ArgumentException(string.Format("String contains unknown prefix length character {0}", (object) prefixChar));
    }
  }
}
