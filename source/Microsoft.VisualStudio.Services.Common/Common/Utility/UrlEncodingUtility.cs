// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Utility.UrlEncodingUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.Utility
{
  public class UrlEncodingUtility
  {
    public static string UrlTokenDecodeToString(string input) => Encoding.UTF8.GetString(UrlEncodingUtility.UrlTokenDecode(input));

    public static byte[] UrlTokenDecode(string input)
    {
      int num1 = input != null ? input.Length : throw new ArgumentNullException(nameof (input));
      if (num1 < 1)
        return Array.Empty<byte>();
      int num2 = (int) input[num1 - 1] - 48;
      if (num2 < 0 || num2 > 10)
        return (byte[]) null;
      char[] inArray = new char[num1 - 1 + num2];
      for (int index = 0; index < num1 - 1; ++index)
      {
        char ch = input[index];
        switch (ch)
        {
          case '-':
            inArray[index] = '+';
            break;
          case '_':
            inArray[index] = '/';
            break;
          default:
            inArray[index] = ch;
            break;
        }
      }
      for (int index = num1 - 1; index < inArray.Length; ++index)
        inArray[index] = '=';
      return Convert.FromBase64CharArray(inArray, 0, inArray.Length);
    }

    public static string UrlTokenEncode(string input) => input != null ? UrlEncodingUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(input)) : throw new ArgumentNullException(nameof (input));

    public static string UrlTokenEncode(byte[] input)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      if (input.Length < 1)
        return string.Empty;
      string base64String = Convert.ToBase64String(input);
      if (base64String == null)
        return (string) null;
      int length = base64String.Length;
      while (length > 0 && base64String[length - 1] == '=')
        --length;
      char[] chArray = new char[length + 1];
      chArray[length] = (char) (48 + base64String.Length - length);
      for (int index = 0; index < length; ++index)
      {
        char ch = base64String[index];
        switch (ch)
        {
          case '+':
            chArray[index] = '-';
            break;
          case '/':
            chArray[index] = '_';
            break;
          case '=':
            chArray[index] = ch;
            break;
          default:
            chArray[index] = ch;
            break;
        }
      }
      return new string(chArray);
    }
  }
}
