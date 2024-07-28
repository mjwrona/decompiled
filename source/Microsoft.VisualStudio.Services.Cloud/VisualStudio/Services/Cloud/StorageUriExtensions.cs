// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageUriExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class StorageUriExtensions
  {
    public static Microsoft.Azure.Storage.StorageUri ToBlobStorageUri(this Microsoft.Azure.Cosmos.Table.StorageUri storageUri) => new Microsoft.Azure.Storage.StorageUri(storageUri.PrimaryUri, storageUri.SecondaryUri);

    public static Microsoft.Azure.Cosmos.Table.StorageUri ToTableStorageUri(
      this Microsoft.Azure.Storage.StorageUri storageUri)
    {
      return new Microsoft.Azure.Cosmos.Table.StorageUri(storageUri.PrimaryUri, storageUri.SecondaryUri);
    }
  }
}
