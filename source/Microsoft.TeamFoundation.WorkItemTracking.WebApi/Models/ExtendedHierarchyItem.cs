// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ExtendedHierarchyItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [ClientIgnore]
  [DataContract]
  public class ExtendedHierarchyItem : QueryHierarchyItem
  {
    public ExtendedHierarchyItem(QueryHierarchyItem item)
    {
      this.Id = item.Id;
      IList<QueryHierarchyItem> children = item.Children;
      this.Children = children != null ? (IList<QueryHierarchyItem>) children.Select<QueryHierarchyItem, ExtendedHierarchyItem>((Func<QueryHierarchyItem, ExtendedHierarchyItem>) (hierarchyItem => new ExtendedHierarchyItem(hierarchyItem))).OrderBy<ExtendedHierarchyItem, string>((Func<ExtendedHierarchyItem, string>) (q => q.Name), (IComparer<string>) TFStringComparer.WorkItemQueryName).ToArray<ExtendedHierarchyItem>() : (IList<QueryHierarchyItem>) (ExtendedHierarchyItem[]) null;
      this.Clauses = item.Clauses;
      this.Columns = item.Columns;
      this.CreatedBy = item.CreatedBy;
      this.CreatedDate = item.CreatedDate;
      this.FilterOptions = item.FilterOptions;
      this.HasChildren = item.HasChildren;
      this.IsDeleted = item.IsDeleted;
      this.IsFolder = item.IsFolder;
      this.IsInvalidSyntax = item.IsInvalidSyntax;
      this.IsPublic = item.IsPublic;
      this.LastExecutedBy = item.LastExecutedBy;
      this.LastExecutedDate = item.LastExecutedDate;
      this.LastModifiedBy = item.LastModifiedBy;
      this.LastModifiedDate = item.LastModifiedDate;
      this.LinkClauses = item.LinkClauses;
      this.Links = item.Links;
      this.Name = item.Name;
      this.Path = item.Path;
      this.QueryRecursionOption = item.QueryRecursionOption;
      this.QueryType = item.QueryType;
      this.SortColumns = item.SortColumns;
      this.SourceClauses = item.SourceClauses;
      this.TargetClauses = item.TargetClauses;
      this.Url = item.Url;
      this.Wiql = item.Wiql;
      this.IsFavorite = false;
    }

    [DataMember(EmitDefaultValue = false)]
    public bool IsFavorite { get; set; }
  }
}
