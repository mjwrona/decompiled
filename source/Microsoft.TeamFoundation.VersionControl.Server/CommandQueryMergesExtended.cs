// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryMergesExtended
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryMergesExtended : VersionControlCommand
  {
    private ObjectBinder<ExtendedMerge> m_mergeBinder;
    private StreamingCollection<ExtendedMerge> m_extendedMerges;
    private ResultCollection m_results;
    private VersionedItemComponent m_db;
    private UrlSigner m_urlSigner;

    public CommandQueryMergesExtended(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public StreamingCollection<ExtendedMerge> Execute(
      string workspaceName,
      string workspaceOwner,
      ItemSpec target,
      VersionSpec versionTarget,
      VersionSpec versionFrom,
      VersionSpec versionTo,
      QueryMergesExtendedOptions options)
    {
      this.m_versionControlRequestContext.Validation.checkIdentity(ref workspaceOwner, nameof (workspaceOwner), true);
      this.m_versionControlRequestContext.Validation.checkWorkspaceName(workspaceName, nameof (workspaceName), true);
      this.m_versionControlRequestContext.Validation.check((IValidatable) target, nameof (target), false);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(versionTarget, nameof (versionTarget), false, false);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(versionFrom, nameof (versionFrom), true, true);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(versionTo, nameof (versionTo), true, false);
      Workspace localWorkspace = Workspace.QueryWorkspace(this.m_versionControlRequestContext, workspaceName, workspaceOwner);
      int changeset = versionTarget.ToChangeset(this.m_versionControlRequestContext.RequestContext);
      if (changeset == VersionSpec.UnknownChangeset)
        throw new NotSupportedException(nameof (versionTarget));
      int versionFrom1;
      if (versionFrom != null)
      {
        versionFrom1 = versionFrom.ToChangeset(this.m_versionControlRequestContext.RequestContext);
        if (versionFrom1 == VersionSpec.UnknownChangeset)
          throw new NotSupportedException(nameof (versionFrom));
      }
      else
        versionFrom1 = changeset;
      int versionTo1;
      if (versionTo != null)
      {
        versionTo1 = versionTo.ToChangeset(this.m_versionControlRequestContext.RequestContext);
        if (versionTo1 == VersionSpec.UnknownChangeset)
          throw new NotSupportedException(nameof (versionTo));
      }
      else
        versionTo1 = changeset;
      ItemPathPair serverItem = target.toServerItem(this.m_versionControlRequestContext.RequestContext, localWorkspace);
      this.m_db = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext);
      this.m_results = this.m_db.QueryMergesExtended(serverItem, changeset, target.DeletionId, versionFrom1, versionTo1, options);
      if ((options & QueryMergesExtendedOptions.IncludeDownloadInfo) == QueryMergesExtendedOptions.IncludeDownloadInfo)
        this.m_urlSigner = new UrlSigner(this.m_versionControlRequestContext.RequestContext);
      this.m_extendedMerges = new StreamingCollection<ExtendedMerge>((Command) this)
      {
        HandleExceptions = false
      };
      this.m_mergeBinder = this.m_results.GetCurrent<ExtendedMerge>();
      this.ContinueExecution();
      if (this.IsCacheFull)
        this.RequestContext.PartialResultsReady();
      return this.m_extendedMerges;
    }

    public override void ContinueExecution()
    {
      bool flag;
      while (flag = this.m_mergeBinder.MoveNext())
      {
        ExtendedMerge current = this.m_mergeBinder.Current;
        if (this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.SourceItem.Item.ItemPathPair) && this.SecurityWrapper.HasItemPermission(this.m_versionControlRequestContext, VersionedItemPermissions.Read, current.TargetItem.ItemPathPair))
        {
          current.SourceChangeset.LookupDisplayNames(this.m_versionControlRequestContext);
          if (this.m_urlSigner != null)
            this.m_urlSigner.SignObject((ISignable) current.SourceItem.Item);
          this.m_extendedMerges.Enqueue(current);
          if (this.IsCacheFull)
            break;
        }
      }
      if (this.m_urlSigner != null)
        this.m_urlSigner.FlushDeferredSignatures();
      if (flag)
        return;
      this.m_extendedMerges.IsComplete = true;
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.m_db != null)
      {
        this.m_db.Dispose();
        this.m_db = (VersionedItemComponent) null;
      }
      if (this.m_results != null)
      {
        this.m_results.Dispose();
        this.m_results = (ResultCollection) null;
      }
      if (this.m_urlSigner == null)
        return;
      this.m_urlSigner.Dispose();
      this.m_urlSigner = (UrlSigner) null;
    }
  }
}
