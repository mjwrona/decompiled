// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.LexicalBase64Converter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class LexicalBase64Converter : 
    IConverter<string, string>,
    IHaveInputType<string>,
    IHaveOutputType<string>,
    IConverter<ArraySegment<byte>, string>,
    IHaveInputType<ArraySegment<byte>>
  {
    public string Convert(string input) => this.Convert(Encoding.UTF8.GetBytes(input).AsArraySegment());

    public string Convert(ArraySegment<byte> bytes)
    {
      if (bytes.Array == null)
        throw new ArgumentNullException(nameof (bytes));
      int length1 = bytes.Count > 0 ? 1 + (4 * bytes.Count - 1) / 3 : 0;
      int length2 = length1;
      if (length2 % 4 != 0)
        length2 = length2 + 4 - length2 % 4;
      char[] chArray = new char[length2];
      System.Convert.ToBase64CharArray(bytes.Array, bytes.Offset, bytes.Count, chArray, 0);
      this.TransformBase64ToLexicalBase64(chArray);
      return new string(chArray, 0, length1);
    }

    internal void TransformBase64ToLexicalBase64(char[] base64Chars)
    {
      for (int index = 0; index < base64Chars.Length; ++index)
      {
        char base64Char = base64Chars[index];
        if (base64Char == 'A' || base64Char == 'B')
          base64Chars[index] -= '\u0014';
        else if (base64Char >= 'C' && base64Char <= 'L')
          base64Chars[index] -= '\u0013';
        else if (base64Char >= 'M' && base64Char <= 'Z')
          base64Chars[index] -= '\f';
        else if (base64Char >= 'a' && base64Char <= 'l')
          base64Chars[index] -= '\u0012';
        else if (base64Char >= 'm' && base64Char <= 'z')
          base64Chars[index] -= '\f';
        else if (base64Char >= '0' && base64Char <= '9')
        {
          base64Chars[index] += '?';
        }
        else
        {
          switch (base64Char)
          {
            case '+':
              base64Chars[index] = 'y';
              continue;
            case '/':
              base64Chars[index] = 'z';
              continue;
            default:
              continue;
          }
        }
      }
    }
  }
}
