// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Provenance
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [DataContract]
  public class Provenance : FeedSecuredObject
  {
    [DataMember]
    public Guid PublisherUserIdentity { get; set; }

    [DataMember]
    public string UserAgent { get; set; }

    [DataMember]
    public string ProvenanceSource { get; set; }

    [DataMember]
    public IDictionary<string, string> Data { get; set; }
  }
}
