// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.BlobResultSegmentWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage.Blob;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public class BlobResultSegmentWrapper : IBlobResultSegmentWrapper
  {
    private BlobResultSegment m_result;

    public BlobResultSegmentWrapper(BlobResultSegment result) => this.m_result = result;

    public BlobContinuationToken ContinuationToken => this.m_result.ContinuationToken;

    public IEnumerable<IListBlobItem> Results => this.m_result.Results;
  }
}
