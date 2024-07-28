// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemQueryResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemQueryResult : BaseSecuredObject
  {
    [DataMember]
    public QueryType QueryType { get; set; }

    [DataMember]
    public QueryResultType QueryResultType { get; set; }

    [DataMember]
    public DateTime AsOf { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemFieldReference> Columns { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemQuerySortColumn> SortColumns { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemReference> WorkItems { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemLink> WorkItemRelations { get; set; }

    public WorkItemQueryResult()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public WorkItemQueryResult(ISecuredObject securedObject)
      : base(securedObject)
    {
    }
  }
}
