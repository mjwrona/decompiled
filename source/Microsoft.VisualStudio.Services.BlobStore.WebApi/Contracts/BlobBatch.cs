// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.BlobBatch
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [DataContract]
  public class BlobBatch
  {
    public BlobBatch()
    {
    }

    public BlobBatch(IEnumerable<BlobIdentifier> blobs) => this.Blobs = blobs.Select<BlobIdentifier, Blob>((Func<BlobIdentifier, Blob>) (b => new Blob(b))).ToList<Blob>();

    public BlobBatch(IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks> blobs) => this.Blobs = blobs.Select<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, Blob>((Func<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, Blob>) (b => new Blob(b))).ToList<Blob>();

    [DataMember(EmitDefaultValue = false, Name = "blobs")]
    public List<Blob> Blobs { get; set; }
  }
}
