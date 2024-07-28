// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.IdentitiesSearchRequestModel
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.IdentityPicker.Operations;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [DataContract]
  public sealed class IdentitiesSearchRequestModel
  {
    [DataMember(EmitDefaultValue = false, Name = "identityTypes")]
    public IList<string> IdentityTypes { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "operationScopes")]
    public IList<string> OperationScopes { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "options")]
    public SearchOptions Options { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "query")]
    public string Query { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "filterByAncestorEntityIds")]
    public IList<string> FilterByAncestorEntityIds { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "filterByEntityIds")]
    public IList<string> FilterByEntityIds { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "queryTypeHint")]
    public string QueryTypeHint { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "properties")]
    public IList<string> Properties { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "pagingToken")]
    public string PagingToken { get; set; }

    public IdentitiesSearchRequestModel()
    {
    }

    public IdentitiesSearchRequestModel(IdentitiesSearchRequestModel identitiesRequest)
    {
      this.IdentityTypes = identitiesRequest.IdentityTypes;
      this.OperationScopes = identitiesRequest.OperationScopes;
      this.Options = identitiesRequest.Options;
      this.Query = identitiesRequest.Query;
      this.FilterByAncestorEntityIds = identitiesRequest.FilterByAncestorEntityIds;
      this.FilterByEntityIds = identitiesRequest.FilterByEntityIds;
      this.Properties = identitiesRequest.Properties;
      this.PagingToken = identitiesRequest.PagingToken;
    }
  }
}
