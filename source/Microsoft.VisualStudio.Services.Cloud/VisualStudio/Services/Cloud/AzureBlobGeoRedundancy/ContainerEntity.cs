// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.ContainerEntity
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public abstract class ContainerEntity : TableEntity
  {
    public ContainerEntity(string containerName)
    {
      this.PartitionKey = ContainerEntity.GetPartitionKey(containerName);
      this.RowKey = containerName;
    }

    public ContainerEntity()
    {
    }

    public string ContainerName => this.RowKey;

    public string Status { get; set; }

    public DateTime? QueuedDate { get; set; }

    public DateTime? StartedDate { get; set; }

    public static string GetPartitionKey(string containerName) => containerName.Substring(0, 1);
  }
}
