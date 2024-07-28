// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Copy.DedupContentCopyRequest
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Copy
{
  public class DedupContentCopyRequest
  {
    public Guid CopyFromHostId { get; set; }

    [JsonConverter(typeof (DomainIdJsonConverter))]
    public IDomainId CopyFromDomainId { get; set; }

    [JsonConverter(typeof (DomainIdJsonConverter))]
    public IDomainId CopyToDomainId { get; set; }

    public ISet<DedupIdentifier> DedupIds { get; set; }
  }
}
