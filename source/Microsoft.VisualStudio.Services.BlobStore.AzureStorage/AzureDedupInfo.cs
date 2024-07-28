// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AzureDedupInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AzureDedupInfo : IDedupInfo
  {
    public DedupIdentifier Identifier { get; }

    public DateTime? KeepUntil { get; }

    public HealthStatus Status { get; }

    public AzureDedupInfo(DedupIdentifier id, DateTime keepUntil)
    {
      this.Identifier = id;
      this.Status = HealthStatus.Intact;
      this.KeepUntil = new DateTime?(keepUntil);
    }

    internal AzureDedupInfo(DedupIdentifier id, HealthStatus stat)
    {
      this.Identifier = id;
      this.Status = stat;
      this.KeepUntil = new DateTime?();
    }
  }
}
