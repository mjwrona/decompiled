// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IdentityManagement.IdentitiesSearchRequestModel
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Client.IdentityManagement
{
  [DataContract]
  internal sealed class IdentitiesSearchRequestModel
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
  }
}
