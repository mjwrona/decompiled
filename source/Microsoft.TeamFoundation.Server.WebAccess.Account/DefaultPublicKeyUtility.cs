// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.DefaultPublicKeyUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class DefaultPublicKeyUtility : IPublicKeyUtility
  {
    private const string SSH2KeyMarkerStart = "---- BEGIN SSH2 PUBLIC KEY ----";
    private const string SSH2KeyMarkerEnd = "---- END SSH2 PUBLIC KEY ----";
    private const string Base64Regex = "^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";

    public string CalculatePublicKeyFingerprint(string publicKey) => this.CalculatePublicKeyFingerprint(this.ExtractPublicKey(publicKey, out KeyFormat _));

    public string CalculatePublicKeyFingerprint(byte[] publicKeyData) => publicKeyData != null ? BitConverter.ToString(DefaultPublicKeyUtility.CalculateMD5Hash(publicKeyData)).ToLower().Replace("-", ":") : (string) null;

    private static byte[] CalculateMD5Hash(byte[] data) => MD5.Create().ComputeHash(data);

    private static string NormalizeNewLines(string data) => data.Replace("\r", "");

    public byte[] ExtractPublicKey(string data, out KeyFormat format)
    {
      data = data.Trim();
      format = KeyFormat.Unknown;
      string s = (string) null;
      if (data.StartsWith("ssh-"))
      {
        format = KeyFormat.OpenSSH;
        string[] strArray = data.Split(' ');
        if (strArray.Length >= 2)
          s = strArray[1];
      }
      else if (data.StartsWith("---- BEGIN SSH2 PUBLIC KEY ----"))
      {
        format = KeyFormat.RFC4716;
        StringBuilder stringBuilder = new StringBuilder();
        data = DefaultPublicKeyUtility.NormalizeNewLines(data);
        string[] strArray = data.Split('\n');
        bool flag = false;
        foreach (string source in strArray)
        {
          if (!source.Equals("---- BEGIN SSH2 PUBLIC KEY ----"))
          {
            if (flag)
            {
              if (!source.EndsWith("\\"))
                flag = false;
            }
            else if (source.Contains<char>(':'))
            {
              if (source.EndsWith("\\"))
                flag = true;
            }
            else if (!source.Equals("---- END SSH2 PUBLIC KEY ----"))
              stringBuilder.Append(source);
            else
              break;
          }
        }
        s = stringBuilder.ToString();
      }
      else if (Regex.IsMatch(data, "^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$"))
      {
        format = KeyFormat.Raw;
        s = data;
      }
      if (string.IsNullOrEmpty(s))
        return (byte[]) null;
      try
      {
        return Convert.FromBase64String(s);
      }
      catch (FormatException ex)
      {
        return (byte[]) null;
      }
    }

    public bool HasSshRsaHeader(string publickeyData)
    {
      if (string.IsNullOrEmpty(publickeyData))
        return false;
      publickeyData = publickeyData.Trim();
      return publickeyData.StartsWith("ssh-rsa");
    }
  }
}
