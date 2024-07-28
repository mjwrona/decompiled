// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.DeletedCertificateIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault
{
  public sealed class DeletedCertificateIdentifier : ObjectIdentifier
  {
    public static bool IsDeletedCertificateIdentifier(string identifier) => ObjectIdentifier.IsObjectIdentifier("deletedcertificates", identifier);

    public DeletedCertificateIdentifier(string vaultBaseUrl, string name)
      : base(vaultBaseUrl, "deletedcertificates", name, string.Empty)
    {
      this.Identifier = this.BaseIdentifier;
    }

    public DeletedCertificateIdentifier(string identifier)
      : base("deletedcertificates", identifier)
    {
      this.Version = string.Empty;
      this.Identifier = this.BaseIdentifier;
    }
  }
}
