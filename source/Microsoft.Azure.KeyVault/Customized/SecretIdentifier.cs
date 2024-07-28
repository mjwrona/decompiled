// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.SecretIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault
{
  public sealed class SecretIdentifier : ObjectIdentifier
  {
    public static bool IsSecretIdentifier(string identifier) => ObjectIdentifier.IsObjectIdentifier("secrets", identifier);

    public SecretIdentifier(string vaultBaseUrl, string name, string version = "")
      : base(vaultBaseUrl, "secrets", name, version)
    {
    }

    public SecretIdentifier(string identifier)
      : base("secrets", identifier)
    {
    }
  }
}
