// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Details
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class Details
  {
    private bool? m_isPublic;

    public Guid ParentId { get; set; }

    public int ProjectId { get; set; }

    public string QueryName { get; set; }

    public ServerQueryItem Parent { get; set; }

    public ServerQueryItem NearestPersistedParent { get; set; }

    public bool? IsPublic
    {
      get
      {
        if (!this.m_isPublic.HasValue && !this.ParentId.Equals(Guid.Empty) && this.Parent != null && this.Parent.Action != PersistenceAction.Insert)
          this.m_isPublic = this.Parent.Existing.IsPublic;
        return this.m_isPublic;
      }
      set => this.m_isPublic = value;
    }

    public bool IsFolder { get; set; }

    public bool IsDeleted { get; set; }

    public string QueryText { get; set; }

    public string Description { get; set; }

    public IdentityDescriptor Owner { get; set; }

    public Guid OwnerTeamFoundationId { get; set; }
  }
}
