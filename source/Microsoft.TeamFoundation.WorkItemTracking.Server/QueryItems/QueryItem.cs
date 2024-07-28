// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  public abstract class QueryItem : ICloneable
  {
    internal static IEnumerable<QueryItem> Create(IEnumerable<QueryItemEntry> queryEntries) => queryEntries.Select<QueryItemEntry, QueryItem>((Func<QueryItemEntry, QueryItem>) (qe => QueryItem.Create(qe)));

    internal static QueryItem Create(QueryItemEntry queryEntry)
    {
      QueryItem withoutDescendents = QueryItem.CreateWithoutDescendents(queryEntry);
      if (!(withoutDescendents is QueryFolder folder))
        return withoutDescendents;
      QueryItem.PopulateDescendents(folder, queryEntry);
      return withoutDescendents;
    }

    private static QueryItem CreateWithoutDescendents(QueryItemEntry queryEntry) => queryEntry.IsFolder ? (QueryItem) new QueryFolder(queryEntry) : (QueryItem) new Query(queryEntry);

    internal static void PopulateDescendents(QueryFolder folder, QueryItemEntry entry)
    {
      Stack<KeyValuePair<QueryItemEntry, QueryFolder>> keyValuePairStack = new Stack<KeyValuePair<QueryItemEntry, QueryFolder>>(entry.Children.Select<QueryItemEntry, KeyValuePair<QueryItemEntry, QueryFolder>>((Func<QueryItemEntry, KeyValuePair<QueryItemEntry, QueryFolder>>) (child => new KeyValuePair<QueryItemEntry, QueryFolder>(child, folder))));
      while (keyValuePairStack.Count > 0)
      {
        KeyValuePair<QueryItemEntry, QueryFolder> keyValuePair = keyValuePairStack.Pop();
        QueryItem withoutDescendents = QueryItem.CreateWithoutDescendents(keyValuePair.Key);
        keyValuePair.Value.Children.Add(withoutDescendents);
        if (withoutDescendents is QueryFolder queryFolder)
        {
          foreach (QueryItemEntry child in keyValuePair.Key.Children)
            keyValuePairStack.Push(new KeyValuePair<QueryItemEntry, QueryFolder>(child, queryFolder));
        }
      }
    }

    public abstract object Clone();

    internal QueryItem()
    {
    }

    internal QueryItem(QueryItemEntry queryEntry)
    {
      this.Id = queryEntry.Id;
      this.ProjectId = queryEntry.ProjectId;
      this.Name = queryEntry.Name;
      this.CreatedById = queryEntry.CreatedById;
      this.CreatedByName = queryEntry.CreatedByName;
      this.CreatedDate = queryEntry.CreatedDate;
      this.ModifiedDate = queryEntry.ModifiedDate;
      this.ModifiedById = queryEntry.ModifiedById;
      this.ModifiedByName = queryEntry.ModifiedByName;
      this.ParentId = queryEntry.ParentId;
      this.SecurityToken = queryEntry.SecurityToken;
      this.Path = queryEntry.Path;
      this.IsPublic = queryEntry.IsPublic;
      this.IsDeleted = queryEntry.IsDeleted;
    }

    internal virtual void DeepCopyFrom(QueryItem queryItem)
    {
      this.Id = queryItem.Id;
      this.ProjectId = queryItem.ProjectId;
      this.Name = queryItem.Name;
      this.CreatedById = queryItem.CreatedById;
      this.CreatedByName = queryItem.CreatedByName;
      this.CreatedDate = queryItem.CreatedDate;
      this.ModifiedDate = queryItem.ModifiedDate;
      this.ModifiedById = queryItem.ModifiedById;
      this.ModifiedByName = queryItem.ModifiedByName;
      this.ParentId = queryItem.ParentId;
      this.SecurityToken = queryItem.SecurityToken;
      this.Path = queryItem.Path;
      this.IsPublic = queryItem.IsPublic;
      this.IsDeleted = queryItem.IsDeleted;
    }

    public Guid Id { get; internal set; }

    public Guid ProjectId { get; internal set; }

    public string Name { get; internal set; }

    public bool IsPublic { get; internal set; }

    public Guid CreatedById { get; internal set; }

    public string CreatedByName { get; internal set; }

    public DateTime CreatedDate { get; internal set; }

    public Guid ModifiedById { get; internal set; }

    public string ModifiedByName { get; internal set; }

    public DateTime ModifiedDate { get; internal set; }

    public Guid ParentId { get; internal set; }

    public string SecurityToken { get; internal set; }

    public string Path { get; internal set; }

    public bool IsDeleted { get; internal set; }
  }
}
