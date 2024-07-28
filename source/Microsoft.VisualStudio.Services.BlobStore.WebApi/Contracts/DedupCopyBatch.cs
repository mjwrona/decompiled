// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.DedupCopyBatch
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [DataContract]
  public class DedupCopyBatch
  {
    [DataMember]
    public ISet<DedupIdentifier> DedupIds { get; set; }

    [DataMember]
    [JsonConverter(typeof (DomainIdJsonConverter))]
    public IDomainId SourceDomainId { get; set; }

    [DataMember]
    public Guid SourceHostId { get; set; }
  }
}
