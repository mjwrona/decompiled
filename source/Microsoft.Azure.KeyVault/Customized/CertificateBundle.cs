// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.CertificateContentType
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault.Models
{
  public static class CertificateContentType
  {
    public const string Pfx = "application/x-pkcs12";
    public const string Pem = "application/x-pem-file";
    public static readonly string[] AllTypes = new string[2]
    {
      "application/x-pkcs12",
      "application/x-pem-file"
    };
  }
}
