// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.KeyUsageType
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault.Models
{
  public static class KeyUsageType
  {
    public const string DigitalSignature = "digitalSignature";
    public const string NonRepudiation = "nonRepudiation";
    public const string KeyEncipherment = "keyEncipherment";
    public const string DataEncipherment = "dataEncipherment";
    public const string KeyAgreement = "keyAgreement";
    public const string KeyCertSign = "keyCertSign";
    public const string CRLSign = "cRLSign";
    public const string EncipherOnly = "encipherOnly";
    public const string DecipherOnly = "decipherOnly";
  }
}
