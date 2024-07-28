// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.BlobMappings
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [DataContract]
  [CLSCompliant(false)]
  public class BlobMappings
  {
    public BlobMappings()
    {
    }

    public BlobMappings(IDictionary<ulong, PreauthenticatedUri> mappings) => this.Mappings = mappings.ToDictionary<KeyValuePair<ulong, PreauthenticatedUri>, ulong, string>((Func<KeyValuePair<ulong, PreauthenticatedUri>, ulong>) (kvp => kvp.Key), (Func<KeyValuePair<ulong, PreauthenticatedUri>, string>) (kvp => kvp.Value.NotNullUri.AbsoluteUri));

    [DataMember(EmitDefaultValue = false, Name = "mappings")]
    public Dictionary<ulong, string> Mappings { get; set; }

    public IDictionary<ulong, Uri> Deserialize() => (IDictionary<ulong, Uri>) this.Mappings.ToDictionary<KeyValuePair<ulong, string>, ulong, Uri>((Func<KeyValuePair<ulong, string>, ulong>) (kvp => kvp.Key), (Func<KeyValuePair<ulong, string>, Uri>) (kvp => new Uri(kvp.Value)));
  }
}
