// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyConsistencySettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyConsistencySettings
  {
    public bool Repair { get; set; }

    public bool CheckContentMD5 { get; set; }

    public bool SyncSource { get; set; }

    public bool QueueCopies { get; set; }

    public string[] ContainerNames { get; set; }

    public DateTimeOffset? ExcludeBlobsBefore { get; set; }

    public bool CleanupTarget { get; set; }

    public TimeSpan? CleanupMinimumAge { get; set; }

    public void Validate()
    {
      if (this.SyncSource && this.CleanupTarget)
        throw new ArgumentException("Can not specify both SyncSource and CleanupTarget");
    }
  }
}
