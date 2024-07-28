// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryHierarchyItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class QueryHierarchyItem : WorkItemTrackingResource
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityReference CreatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityReference LastModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsFolder { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? HasChildren { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<QueryHierarchyItem> Children { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryType? QueryType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemFieldReference> Columns { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemQuerySortColumn> SortColumns { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Wiql { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsPublic { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsInvalidSyntax { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkItemQueryClause LinkClauses { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public LinkQueryMode? FilterOptions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryRecursionOption? QueryRecursionOption { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkItemQueryClause Clauses { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkItemQueryClause SourceClauses { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public WorkItemQueryClause TargetClauses { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityReference LastExecutedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastExecutedDate { get; set; }
  }
}
