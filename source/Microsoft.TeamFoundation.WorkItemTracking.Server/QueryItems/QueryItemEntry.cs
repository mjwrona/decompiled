// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemEntry
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemEntry
  {
    private List<QueryItemEntry> m_queryChildren;
    private string m_path;
    private string m_securityToken;

    public QueryItemEntry() => this.m_queryChildren = new List<QueryItemEntry>();

    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string Name { get; set; }

    public Guid CreatedById { get; set; }

    public string CreatedByName { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid ModifiedById { get; set; }

    public string ModifiedByName { get; set; }

    public DateTime ModifiedDate { get; set; }

    public Guid ParentId { get; set; }

    public string Wiql { get; set; }

    public bool IsFolder { get; set; }

    public bool IsPublic { get; set; }

    public Guid? LastExecutedById { get; internal set; }

    public DateTime? LastExecutedDate { get; internal set; }

    public virtual bool HasChildren { get; set; }

    public virtual bool IsDeleted { get; set; }

    public int? QueryType { get; set; }

    public IEnumerable<QueryItemEntry> Children => (IEnumerable<QueryItemEntry>) this.m_queryChildren;

    public QueryItemEntry Parent { get; set; }

    public string Path
    {
      get
      {
        if (this.m_path == null)
          this.m_path = this.Parent == null ? this.Name : this.Parent.Path + "/" + this.Name;
        return this.m_path;
      }
      set => this.m_path = value;
    }

    public string SecurityToken
    {
      get
      {
        if (this.m_securityToken == null)
        {
          if (this.Parent != null)
          {
            this.m_securityToken = this.Parent.SecurityToken + "/" + this.Id.ToString();
          }
          else
          {
            Guid guid = this.ProjectId;
            string str1 = guid.ToString();
            guid = this.Id;
            string str2 = guid.ToString();
            this.m_securityToken = "$/" + str1 + "/" + str2;
          }
        }
        return this.m_securityToken;
      }
      set => this.m_securityToken = value;
    }

    public void AddChild(QueryItemEntry child)
    {
      this.m_queryChildren.Add(child);
      child.Parent = this;
    }

    public void RemoveChild(QueryItemEntry child) => this.m_queryChildren.Remove(child);
  }
}
