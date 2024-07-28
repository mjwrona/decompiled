// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.CountResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts
{
  [DataContract]
  public class CountResponse : SearchSecuredObject
  {
    private List<ErrorData> m_errors = new List<ErrorData>();

    public CountResponse()
    {
      this.Count = 0;
      this.Relation = RelationFromExactCount.Equals;
    }

    public CountResponse(int count, RelationFromExactCount relation, IEnumerable<ErrorData> errors)
    {
      this.Count = count;
      this.Relation = relation;
      foreach (ErrorData error in errors)
        this.m_errors.Add(error);
    }

    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "relationFromExactCount")]
    public RelationFromExactCount Relation { get; set; }

    [DataMember(Name = "errors")]
    public IEnumerable<ErrorData> Errors => (IEnumerable<ErrorData>) this.m_errors;

    public override string ToString() => JsonConvert.SerializeObject((object) new
    {
      count = this.Count,
      relationFromExactCount = this.Relation,
      errors = this.Errors
    }, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });

    public void AddError(ErrorData errorData) => this.m_errors.Add(errorData);

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      List<ErrorData> errors = this.m_errors;
      this.m_errors = errors != null ? errors.Select<ErrorData, ErrorData>((Func<ErrorData, ErrorData>) (e =>
      {
        e.SetSecuredObject(namespaceId, requiredPermissions, token);
        return e;
      })).ToList<ErrorData>() : (List<ErrorData>) null;
    }
  }
}
