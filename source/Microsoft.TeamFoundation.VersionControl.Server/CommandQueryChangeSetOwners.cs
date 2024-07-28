// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryChangeSetOwners
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryChangeSetOwners : VersionControlCommand
  {
    private StreamingCollection<ChangeSetOwner> m_owners;
    private ObjectBinder<ChangeSetOwner> m_ownersBinder;
    private VersionedItemComponent m_db;
    private ResultCollection m_rc;
    private IdentityService m_identityService;

    public CommandQueryChangeSetOwners(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public void Execute(bool includeCounts)
    {
      this.m_owners = new StreamingCollection<ChangeSetOwner>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_rc = this.m_db.QueryChangesetOwners(includeCounts);
      this.m_ownersBinder = this.m_rc.GetCurrent<ChangeSetOwner>();
      this.m_identityService = this.RequestContext.GetService<IdentityService>();
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      if (this.m_ownersBinder != null)
      {
        List<ChangeSetOwner> source = new List<ChangeSetOwner>();
        while (!this.IsCacheFull && this.m_ownersBinder.MoveNext())
          source.Add(this.m_ownersBinder.Current);
        if (source.Count > 0)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_identityService.ReadIdentities(this.RequestContext, (IList<Guid>) source.Select<ChangeSetOwner, Guid>((Func<ChangeSetOwner, Guid>) (o => o.TeamFoundationId)).ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null);
          for (int index = 0; index < source.Count; ++index)
          {
            source[index].Identity = identityList[index];
            this.m_owners.Enqueue(source[index]);
          }
          if (this.IsCacheFull)
            return;
        }
        this.m_ownersBinder = (ObjectBinder<ChangeSetOwner>) null;
      }
      this.m_owners.IsComplete = true;
    }

    public StreamingCollection<ChangeSetOwner> Owners => this.m_owners;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_rc != null)
      {
        this.m_rc.Dispose();
        this.m_rc = (ResultCollection) null;
      }
      if (this.m_db == null)
        return;
      this.m_db.Dispose();
      this.m_db = (VersionedItemComponent) null;
    }
  }
}
