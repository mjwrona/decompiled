// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ServerQueryProvisioningHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ServerQueryProvisioningHelper : IQueryProvisioningHelper
  {
    private ServerQueryItem m_cachedQueryItem;
    private Dictionary<Guid, ServerQueryItem> m_queries;
    private IVssRequestContext m_requestContext;

    public ServerQueryProvisioningHelper(
      IVssRequestContext requestContext,
      Uri projectUri,
      Dictionary<Guid, ServerQueryItem> queries)
    {
      this.m_requestContext = requestContext;
      this.m_queries = queries;
    }

    public IEnumerable<Guid> GetDirtyQueryItems() => (IEnumerable<Guid>) this.m_queries.Keys.ToList<Guid>();

    public bool IsQueryDeleted(Guid id) => false;

    public bool IsQueryNew(Guid id) => !this[id].IsLoaded;

    public bool IsQueryDirtyShallow(Guid id)
    {
      CultureInfo culture = this.m_requestContext.ServiceHost.GetCulture(this.m_requestContext);
      return this[id].IsLoaded && ServerResources.Manager.GetString("SharedQueries", culture).Equals(this[id].Existing.QueryName);
    }

    public string GetOwnerIdentifier(Guid id, bool onlyIfChanged = false)
    {
      string identifier1 = this[id].New.Owner == (IdentityDescriptor) null ? (string) null : this[id].New.Owner.Identifier;
      string identifier2 = this[id].Existing.Owner == (IdentityDescriptor) null ? (string) null : this[id].Existing.Owner.Identifier;
      string str = !this[id].IsLoaded ? identifier1 : (identifier1 != identifier2 || !string.IsNullOrEmpty(identifier1) && identifier1.Equals(identifier2) ? identifier1 : (onlyIfChanged ? (string) null : identifier2));
      return !string.IsNullOrEmpty(str) ? str : this.m_requestContext.UserContext.Identifier;
    }

    public string GetIdentityType(Guid id, bool onlyIfChanged = false)
    {
      string identityType1 = this[id].New.Owner == (IdentityDescriptor) null ? (string) null : this[id].New.Owner.IdentityType;
      string identityType2 = this[id].Existing.Owner == (IdentityDescriptor) null ? (string) null : this[id].Existing.Owner.IdentityType;
      string str = !this[id].IsLoaded ? identityType1 : (identityType1 != identityType2 || !string.IsNullOrEmpty(identityType1) && identityType1.Equals(identityType2) ? identityType1 : (onlyIfChanged ? (string) null : identityType2));
      return !string.IsNullOrEmpty(str) ? str : this.m_requestContext.UserContext.IdentityType;
    }

    public string GetQueryText(Guid id, bool onlyIfChanged = false)
    {
      if (!this[id].IsLoaded || !string.IsNullOrEmpty(this[id].New.QueryText) && string.Equals(this[id].Existing.QueryText, this[id].New.QueryText))
        return this[id].New.QueryText;
      return !onlyIfChanged ? this[id].Existing.QueryText : (string) null;
    }

    public Guid GetParentId(Guid id, bool onlyIfChanged = false)
    {
      if (!this[id].IsLoaded || this[id].New.ParentId != Guid.Empty && this[id].Existing.ParentId != this[id].New.ParentId)
        return this[id].New.ParentId;
      return !onlyIfChanged ? this[id].Existing.ParentId : Guid.Empty;
    }

    public string GetName(Guid id, bool onlyIfChanged = false)
    {
      if (!this[id].IsLoaded || !string.IsNullOrEmpty(this[id].New.QueryName) && string.Equals(this[id].Existing.QueryName, this[id].New.QueryName))
        return this[id].New.QueryName;
      return !onlyIfChanged ? this[id].Existing.QueryName : (string) null;
    }

    public IEnumerable<QueryAccessControlEntry> GetAccessControlEntries(Guid id) => (IEnumerable<QueryAccessControlEntry>) null;

    public bool GetInheritPermissions(Guid id) => false;

    public bool IsAccessControlListDirty(Guid id) => false;

    private ServerQueryItem this[Guid id] => this.m_cachedQueryItem != null && this.m_cachedQueryItem.Id.Equals(id) ? this.m_cachedQueryItem : (this.m_cachedQueryItem = this.m_queries[id]);
  }
}
