// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.QueryResult
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  [DataContract]
  public class QueryResult : PackagingSecuredObject
  {
    [DataMember(Name = "@context")]
    public Context Context { get; set; }

    [DataMember]
    public IEnumerable<QueryResultPackage> Data { get; set; }

    [DataMember]
    public string LastReopen { get; set; }

    [DataMember]
    public string Index { get; set; }

    [DataMember]
    public string TotalHits { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.Context?.SetSecuredObject(securedObject);
      IEnumerable<QueryResultPackage> data = this.Data;
      this.Data = data != null ? data.ToSecuredObject<QueryResultPackage>(securedObject) : (IEnumerable<QueryResultPackage>) null;
    }
  }
}
