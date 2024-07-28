// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.CertificateOperationIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;

namespace Microsoft.Azure.KeyVault
{
  public sealed class CertificateOperationIdentifier : ObjectIdentifier
  {
    public static bool IsCertificateOperationIdentifier(string identifier)
    {
      bool flag = ObjectIdentifier.IsObjectIdentifier("certificates", identifier);
      Uri uri = new Uri(identifier, UriKind.Absolute);
      if (uri.Segments.Length != 4)
        flag = false;
      if (!string.Equals(uri.Segments[3], "pending"))
        flag = false;
      return flag;
    }

    public CertificateOperationIdentifier(string vaultBaseUrl, string name)
      : base(vaultBaseUrl, "certificates", name, "pending")
    {
      this.BaseIdentifier = this.Identifier;
      this.Version = string.Empty;
    }

    public CertificateOperationIdentifier(string identifier)
      : base("certificates", identifier)
    {
      this.BaseIdentifier = this.Identifier;
      this.Version = string.Empty;
    }
  }
}
