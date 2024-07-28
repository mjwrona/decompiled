// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestSignatures
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RequestSignatures
  {
    public static string RegenerateUrl(NameValueCollection parameters)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < parameters.Count; ++index)
        stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, index == 0 ? "{0}={1}" : "&{0}={1}", (object) parameters.Keys[index], (object) Uri.EscapeDataString(parameters[index])));
      return stringBuilder.ToString();
    }

    internal static string NormalizeQueryString(int[] fileIds, long timestamp)
    {
      StringBuilder stringBuilder = new StringBuilder(fileIds.Length * 8);
      for (int index = 0; index < fileIds.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(',');
        stringBuilder.Append(fileIds[index]);
      }
      return RequestSignatures.NormalizeQueryString(stringBuilder.ToString(), timestamp.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal static string NormalizeQueryString(string fileIds, string timestamp) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}={1}&{2}={3}", (object) "sfid", (object) fileIds, (object) "ts", (object) timestamp);

    internal static int DefaultKeyLength => 1024;

    internal static bool IsValidKeyLength(int keyLength) => keyLength == 1024 || keyLength == 2048;

    internal static byte[] GenerateQueryStringHash(string query)
    {
      using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
      {
        byte[] bytes = Encoding.ASCII.GetBytes(query);
        return cryptoServiceProvider.ComputeHash(bytes);
      }
    }

    internal static void GetKeyInformation(
      byte[] privateKey,
      out int keyLength,
      out byte[] publicKey)
    {
      using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
      {
        Flags = CspProviderFlags.UseMachineKeyStore
      }))
      {
        cryptoServiceProvider.ImportCspBlob(privateKey);
        keyLength = cryptoServiceProvider.KeySize;
        publicKey = cryptoServiceProvider.ExportCspBlob(false);
      }
    }

    internal static int GetKeyLength(byte[] key)
    {
      using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
      {
        Flags = CspProviderFlags.UseMachineKeyStore
      }))
      {
        cryptoServiceProvider.ImportCspBlob(key);
        return cryptoServiceProvider.KeySize;
      }
    }

    internal static byte[] GenerateNewPrivateKey(int keyLength)
    {
      if (!RequestSignatures.IsValidKeyLength(keyLength))
        throw new ArgumentOutOfRangeException(nameof (keyLength));
      using (RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(keyLength, new CspParameters()
      {
        Flags = CspProviderFlags.UseMachineKeyStore
      }))
        return cryptoServiceProvider.ExportCspBlob(true);
    }
  }
}
