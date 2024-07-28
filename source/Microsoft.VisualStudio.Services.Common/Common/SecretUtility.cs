// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.SecretUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class SecretUtility
  {
    internal const string PasswordMask = "******";
    internal const string SecretMask = "<secret removed>";
    internal const string SignatureMask = "<signature removed>";
    internal const string PasswordRemovedMask = "**password-removed**";
    internal const string PwdRemovedMask = "**pwd-removed**";
    internal const string PasswordSpaceRemovedMask = "**password-space-removed**";
    internal const string PwdSpaceRemovedMask = "**pwd-space-removed**";
    internal const string AccountKeyRemovedMask = "**account-key-removed**";
    private const string c_passwordToken = "Password=";
    private const string c_passwordTokenSpaced = "-Password ";
    private const string c_pwdToken = "Pwd=";
    private const string c_pwdTokenSpaced = "-Pwd ";
    private const string c_accountKeyToken = "AccountKey=";
    private const string c_authBearerToken = "Bearer ";
    private const string c_jwtTypToken = "eyJ0eXAiOi";
    private const string c_jwtAlgToken = "eyJhbGciOi";
    private const string c_jwtX5tToken = "eyJ4NXQiOi";
    private const string c_jwtKidToken = "eyJraWQiOi";
    private const string c_urlSignatureAssignment = "urlSignature=";
    private static readonly char[] s_validPasswordEnding = new char[3]
    {
      ';',
      '\'',
      '"'
    };

    public static bool ContainsUnmaskedSecret(string message) => !string.Equals(message, SecretUtility.ScrubSecrets(message, false), StringComparison.Ordinal);

    public static bool ContainsUnmaskedSecret(string message, out bool onlyJwtsFound)
    {
      if (string.IsNullOrEmpty(message))
      {
        onlyJwtsFound = false;
        return false;
      }
      string b1 = SecretUtility.ScrubJwts(message, false);
      bool flag1 = !string.Equals(message, b1, StringComparison.Ordinal);
      string b2 = SecretUtility.ScrubTraditionalSecrets(message, false);
      bool flag2 = !string.Equals(message, b2, StringComparison.Ordinal);
      string b3 = SecretUtility.ScrubUrlSignatures(message, false);
      bool flag3 = !string.Equals(message, b3, StringComparison.Ordinal);
      onlyJwtsFound = ((flag3 ? 0 : (!flag2 ? 1 : 0)) & (flag1 ? 1 : 0)) != 0;
      return flag2 | flag1 | flag3;
    }

    public static string ScrubSecrets(string message, bool assertOnDetection = true)
    {
      if (string.IsNullOrEmpty(message))
        return message;
      message = SecretUtility.ScrubTraditionalSecrets(message, assertOnDetection);
      message = SecretUtility.ScrubJwts(message, assertOnDetection);
      message = SecretUtility.ScrubUrlSignatures(message, assertOnDetection);
      return message;
    }

    public static string ScrubUrlSignatures(string message, bool assertOnDetection)
    {
      message = SecretUtility.ScrubSecret(message, "urlSignature=", "<signature removed>", assertOnDetection);
      return message;
    }

    private static string ScrubTraditionalSecrets(string message, bool assertOnDetection)
    {
      message = SecretUtility.ScrubSecret(message, "Password=", "**password-removed**", assertOnDetection);
      message = SecretUtility.ScrubSecret(message, "Pwd=", "**pwd-removed**", assertOnDetection);
      message = SecretUtility.ScrubSecret(message, "-Password ", "**password-space-removed**", assertOnDetection);
      message = SecretUtility.ScrubSecret(message, "-Pwd ", "**pwd-space-removed**", assertOnDetection);
      message = SecretUtility.ScrubSecret(message, "AccountKey=", "**account-key-removed**", assertOnDetection);
      message = SecretUtility.ScrubSecret(message, "Bearer ", "<secret removed>", assertOnDetection);
      return message;
    }

    private static string ScrubJwts(string message, bool assertOnDetection)
    {
      message = SecretUtility.ScrubSecret(message, "eyJ0eXAiOi", "<secret removed>", assertOnDetection, true);
      message = SecretUtility.ScrubSecret(message, "eyJhbGciOi", "<secret removed>", assertOnDetection, true);
      message = SecretUtility.ScrubSecret(message, "eyJ4NXQiOi", "<secret removed>", assertOnDetection, true);
      message = SecretUtility.ScrubSecret(message, "eyJraWQiOi", "<secret removed>", assertOnDetection, true);
      return message;
    }

    private static string ScrubSecret(
      string message,
      string token,
      string mask,
      bool assertOnDetection,
      bool maskToken = false)
    {
      int num1 = -1;
      do
      {
        num1 = message.IndexOf(token, num1 < 0 ? 0 : num1, StringComparison.OrdinalIgnoreCase);
        if (num1 >= 0)
        {
          if (!maskToken && (message.IndexOf(token + mask, StringComparison.OrdinalIgnoreCase) == num1 || message.IndexOf(token + "******", StringComparison.OrdinalIgnoreCase) == num1))
          {
            num1 += token.Length + mask.Length;
          }
          else
          {
            try
            {
              if (!maskToken)
                num1 += token.Length;
              int num2 = message.Length - 1;
              if (message[num1] == '"' || message[num1] == '\'')
              {
                for (int index = num1 + 1; index < message.Length - 1; ++index)
                {
                  if ((int) message[num1] == (int) message[index])
                  {
                    if ((int) message[num1] == (int) message[index + 1])
                    {
                      ++index;
                    }
                    else
                    {
                      num2 = index;
                      break;
                    }
                  }
                }
              }
              else
              {
                for (int index = num1 + 1; index < message.Length; ++index)
                {
                  if (char.IsWhiteSpace(message[index]) || ((ICollection<char>) SecretUtility.s_validPasswordEnding).Contains(message[index]))
                  {
                    num2 = index - 1;
                    break;
                  }
                }
              }
              message = message.Substring(0, num1) + mask + message.Substring(num2 + 1);
              int num3 = assertOnDetection ? 1 : 0;
            }
            catch (Exception ex)
            {
            }
            finally
            {
              num1 += mask.Length;
            }
          }
        }
        else
          break;
      }
      while (num1 < message.Length);
      return message;
    }
  }
}
