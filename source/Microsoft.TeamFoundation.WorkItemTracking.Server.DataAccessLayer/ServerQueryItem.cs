// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ServerQueryItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ServerQueryItem
  {
    private string m_securityToken;
    private Details m_new;
    private Details m_existing;

    public ServerQueryItem()
      : this(Guid.Empty)
    {
    }

    public ServerQueryItem(Guid id)
    {
      this.Id = id;
      this.Action = PersistenceAction.None;
      this.HasStartedProcessing = false;
      this.HasFinishedProcessing = false;
      this.Flag = false;
      this.IsLoaded = false;
    }

    public Guid Id { get; set; }

    public PersistenceAction Action { get; set; }

    public bool HasStartedProcessing { get; set; }

    public bool HasFinishedProcessing { get; set; }

    public bool Flag { get; set; }

    public PayloadTable.PayloadRow PayloadRow { get; set; }

    public DateTime UpdateTime { get; set; }

    public bool IsLoaded { get; set; }

    public bool IsBackCompat { get; set; }

    public ExtendedAccessControlListData AccessControlList { get; set; }

    public string SecurityToken
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_securityToken))
        {
          if (this.Action == PersistenceAction.Insert && this.m_new != null && this.m_new.Parent != null)
          {
            string securityToken = this.m_new.Parent.SecurityToken;
            if (!string.IsNullOrEmpty(securityToken))
              this.m_securityToken = securityToken + QueryItemSecurityConstants.PathSeparator.ToString() + this.Id.ToString().ToUpperInvariant();
          }
          else if (this.m_existing != null && this.m_existing.Parent != null)
          {
            string securityToken = this.m_existing.Parent.SecurityToken;
            if (!string.IsNullOrEmpty(securityToken))
              this.m_securityToken = securityToken + QueryItemSecurityConstants.PathSeparator.ToString() + this.Id.ToString().ToUpperInvariant();
          }
        }
        return this.m_securityToken;
      }
      set => this.m_securityToken = value;
    }

    public string QueryName
    {
      get
      {
        if (this.New != null && !string.IsNullOrEmpty(this.New.QueryName))
          return this.New.QueryName;
        return this.Existing != null && !string.IsNullOrEmpty(this.Existing.QueryName) ? this.Existing.QueryName : "";
      }
    }

    public Details New
    {
      get
      {
        if (this.m_new == null)
          this.m_new = new Details();
        return this.m_new;
      }
    }

    public Details Existing
    {
      get
      {
        if (this.m_existing == null)
          this.m_existing = new Details();
        return this.m_existing;
      }
    }

    public void PropagateFlagToExistingParents()
    {
      for (ServerQueryItem parent = this.Existing.Parent; parent != null && parent.Flag != this.Flag; parent = parent.Existing.Parent)
        parent.Flag = this.Flag;
    }
  }
}
