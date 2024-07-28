// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.KeyIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault
{
  public sealed class KeyIdentifier : ObjectIdentifier
  {
    public static bool IsKeyIdentifier(string identifier) => ObjectIdentifier.IsObjectIdentifier("keys", identifier);

    public KeyIdentifier(string vaultBaseUrl, string name, string version = "")
      : base(vaultBaseUrl, "keys", name, version)
    {
    }

    public KeyIdentifier(string identifier)
      : base("keys", identifier)
    {
    }
  }
}
