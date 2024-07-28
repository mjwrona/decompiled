// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.KeyCredentialsHelper
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class KeyCredentialsHelper
  {
    public static PasswordCredential CreatePasswordCredential(
      DateTime startTime,
      DateTime endTime,
      string password)
    {
      Utils.ThrowIfNullOrEmpty((object) password, nameof (password));
      KeyCredentialsHelper.ValidateStartAndEndTime(startTime, endTime);
      return new PasswordCredential()
      {
        StartDate = new DateTime?(startTime),
        EndDate = new DateTime?(endTime),
        Value = password
      };
    }

    public static KeyCredential CreateSymmetricKeyCredential(
      DateTime startTime,
      DateTime endTime,
      KeyUsage keyUsage,
      byte[] credentialBlob)
    {
      Utils.ThrowIfNullOrEmpty((object) credentialBlob, nameof (credentialBlob));
      KeyCredentialsHelper.ValidateStartAndEndTime(startTime, endTime);
      return new KeyCredential()
      {
        StartDate = new DateTime?(startTime),
        EndDate = new DateTime?(endTime),
        Type = KeyType.Symmetric.ToString(),
        Usage = keyUsage.ToString(),
        Value = credentialBlob
      };
    }

    public static KeyCredential CreateSymmetricKeyCredential(
      DateTime startTime,
      DateTime endTime,
      KeyUsage keyUsage,
      string base64EncodedKeyValue)
    {
      Utils.ThrowIfNullOrEmpty((object) base64EncodedKeyValue, nameof (base64EncodedKeyValue));
      byte[] credentialBlob;
      try
      {
        credentialBlob = Convert.FromBase64String(base64EncodedKeyValue);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException("Input was not a valid base64 encoded value.", nameof (base64EncodedKeyValue));
      }
      return KeyCredentialsHelper.CreateSymmetricKeyCredential(startTime, endTime, keyUsage, credentialBlob);
    }

    public static KeyCredential CreateAsymmetricKeyCredential(
      DateTime startTime,
      DateTime endTime,
      X509Certificate2 certificate)
    {
      Utils.ThrowIfNullOrEmpty((object) certificate, nameof (certificate));
      KeyCredentialsHelper.ValidateStartAndEndTime(startTime, endTime);
      return new KeyCredential()
      {
        StartDate = new DateTime?(startTime),
        EndDate = new DateTime?(endTime),
        Type = KeyType.AsymmetricX509Cert.ToString(),
        Usage = KeyUsage.Verify.ToString(),
        Value = certificate.GetRawCertData()
      };
    }

    internal static void ValidateStartAndEndTime(DateTime startTime, DateTime endTime)
    {
      if (startTime.ToUniversalTime().CompareTo(endTime.ToUniversalTime()) > 0)
        throw new ArgumentException("startTime must be less than end time");
      if (endTime.ToUniversalTime().CompareTo(DateTime.UtcNow) < 0)
        throw new ArgumentException("endTime must be less than the current time.");
    }
  }
}
