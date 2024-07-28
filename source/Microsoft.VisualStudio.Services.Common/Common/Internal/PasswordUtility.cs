// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.PasswordUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  public static class PasswordUtility
  {
    private static readonly char[] punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();
    private static readonly char[] startingChars = new char[2]
    {
      '<',
      '&'
    };

    public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
    {
      if (length < 1 || length > 128)
        throw new ArgumentOutOfRangeException(nameof (length), "Password length specified must be between 1 and 128 characters.");
      if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
        throw new ArgumentOutOfRangeException(nameof (numberOfNonAlphanumericCharacters), "The value specified in parameter 'numberOfNonAlphanumericCharacters' should be in the range from zero to the value specified in the password length parameter");
      string s;
      do
      {
        byte[] data = new byte[length];
        char[] chArray = new char[length];
        int num1 = 0;
        using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
          cryptoServiceProvider.GetBytes(data);
        for (int index = 0; index < length; ++index)
        {
          int num2 = (int) data[index] % 87;
          if (num2 < 10)
            chArray[index] = (char) (48 + num2);
          else if (num2 < 36)
            chArray[index] = (char) (65 + num2 - 10);
          else if (num2 < 62)
          {
            chArray[index] = (char) (97 + num2 - 36);
          }
          else
          {
            chArray[index] = PasswordUtility.punctuations[num2 - 62];
            ++num1;
          }
        }
        if (num1 < numberOfNonAlphanumericCharacters)
        {
          Random random = new Random();
          for (int index1 = 0; index1 < numberOfNonAlphanumericCharacters - num1; ++index1)
          {
            int index2;
            do
            {
              index2 = random.Next(0, length);
            }
            while (!char.IsLetterOrDigit(chArray[index2]));
            chArray[index2] = PasswordUtility.punctuations[random.Next(0, PasswordUtility.punctuations.Length)];
          }
        }
        s = new string(chArray);
      }
      while (PasswordUtility.IsDangerousString(s, out int _));
      return s;
    }

    internal static bool IsDangerousString(string s, out int matchIndex)
    {
      matchIndex = 0;
      int startIndex = 0;
      while (true)
      {
        int index = s.IndexOfAny(PasswordUtility.startingChars, startIndex);
        if (index >= 0 && index != s.Length - 1)
        {
          matchIndex = index;
          switch (s[index])
          {
            case '&':
              if (s[index + 1] != '#')
                break;
              goto label_7;
            case '<':
              if (PasswordUtility.IsAtoZ(s[index + 1]) || s[index + 1] == '!' || s[index + 1] == '/' || s[index + 1] == '?')
                goto label_5;
              else
                break;
          }
          startIndex = index + 1;
        }
        else
          break;
      }
      return false;
label_5:
      return true;
label_7:
      return true;
    }

    private static bool IsAtoZ(char c)
    {
      if (c >= 'a' && c <= 'z')
        return true;
      return c >= 'A' && c <= 'Z';
    }
  }
}
