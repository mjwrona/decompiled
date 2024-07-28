// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.ServerFingerprintModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class ServerFingerprintModel
  {
    public string Fingerprint { get; }

    public string HashAlgorithm { get; }

    public string Encryption { get; }

    public ServerFingerprintModel(string fingerprint, string hashAlgorithm, string encryption)
    {
      this.Fingerprint = fingerprint;
      this.HashAlgorithm = hashAlgorithm;
      this.Encryption = encryption;
    }
  }
}
