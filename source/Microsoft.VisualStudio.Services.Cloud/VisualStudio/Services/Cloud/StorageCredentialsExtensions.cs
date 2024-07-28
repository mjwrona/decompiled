// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.StorageCredentialsExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class StorageCredentialsExtensions
  {
    public static Microsoft.Azure.Cosmos.Table.StorageCredentials ToTableCredentials(
      this Microsoft.Azure.Storage.Auth.StorageCredentials storageCredentials)
    {
      Microsoft.Azure.Cosmos.Table.StorageCredentials tableCredentials;
      if (storageCredentials == null)
        tableCredentials = (Microsoft.Azure.Cosmos.Table.StorageCredentials) null;
      else if (storageCredentials.IsSAS)
      {
        tableCredentials = new Microsoft.Azure.Cosmos.Table.StorageCredentials(storageCredentials.SASToken);
      }
      else
      {
        if (storageCredentials.IsToken)
          throw new ArgumentException("Cannot convert token credential.");
        tableCredentials = new Microsoft.Azure.Cosmos.Table.StorageCredentials(storageCredentials.AccountName, storageCredentials.ExportBase64EncodedKey(), storageCredentials.KeyName);
      }
      return tableCredentials;
    }

    public static Microsoft.Azure.Storage.Auth.StorageCredentials ToBlobStorageCredentials(
      this Microsoft.Azure.Cosmos.Table.StorageCredentials tableCredentials)
    {
      return tableCredentials != null ? (!tableCredentials.IsSAS ? new Microsoft.Azure.Storage.Auth.StorageCredentials(tableCredentials.AccountName, tableCredentials.Key, tableCredentials.KeyName) : new Microsoft.Azure.Storage.Auth.StorageCredentials(tableCredentials.SASToken)) : (Microsoft.Azure.Storage.Auth.StorageCredentials) null;
    }
  }
}
