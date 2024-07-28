// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.ReferenceBatch
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [JsonObject(Description = "list of References, each reference is either a KeepUntilReference or an IdReference", MemberSerialization = MemberSerialization.OptIn)]
  public class ReferenceBatch
  {
    public ReferenceBatch()
    {
    }

    public ReferenceBatch(
      IEnumerable<KeyValuePair<BlobReference, BlobIdentifier>> referenceToBlobMap)
    {
      this.References = referenceToBlobMap.Select<KeyValuePair<BlobReference, BlobIdentifier>, Reference>((Func<KeyValuePair<BlobReference, BlobIdentifier>, Reference>) (rb => ReferenceFactory.MakeReference(rb.Key, rb.Value))).ToList<Reference>();
    }

    public ReferenceBatch(
      IEnumerable<KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>> referenceToBlobMap)
    {
      this.References = referenceToBlobMap.Select<KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>, Reference>((Func<KeyValuePair<BlobReference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>, Reference>) (rb => ReferenceFactory.MakeReference(rb.Key, rb.Value))).ToList<Reference>();
    }

    [JsonProperty(ItemConverterType = typeof (JsonReferenceConverter), PropertyName = "references")]
    public List<Reference> References { get; set; }
  }
}
