// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.StorageAccountIdentifier
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault
{
  public sealed class StorageAccountIdentifier : ObjectIdentifier
  {
    public static bool IsStorageAccountIdentifier(string identifier) => ObjectIdentifier.IsObjectIdentifier("storage", identifier);

    public StorageAccountIdentifier(string vaultBaseUrl, string name)
      : base(vaultBaseUrl, "storage", name)
    {
    }

    public StorageAccountIdentifier(string identifier)
      : base("storage", identifier)
    {
    }
  }
}
