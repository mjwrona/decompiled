// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Part
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  public class Part
  {
    public long From { get; set; }

    public long To { get; set; }

    public long TotalSize { get; set; }

    public DedupIdentifier RootId { get; set; }

    public long Size => this.To - this.From + 1L;
  }
}
