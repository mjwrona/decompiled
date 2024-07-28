// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingChangeConflictColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendingChangeConflictColumns : VersionControlObjectBinder<PendingChangeConflict>
  {
    protected SqlColumnBinder cause = new SqlColumnBinder("Cause");
    protected SqlColumnBinder targetServerItem = new SqlColumnBinder("TargetServerItem");
    protected SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");
    protected SqlColumnBinder workspaceName = new SqlColumnBinder("WorkspaceName");
    protected SqlColumnBinder offendingServerItem = new SqlColumnBinder("OffendingServerItem");
    protected IdentityService m_identityService;
    protected IVssRequestContext m_requestContext;

    public PendingChangeConflictColumns(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    public PendingChangeConflictColumns(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    protected override PendingChangeConflict Bind() => new PendingChangeConflict(this.m_requestContext)
    {
      ErrorCode = this.cause.GetInt32((IDataReader) this.Reader),
      ServerItem = this.targetServerItem.GetServerItem(this.Reader, true),
      OffendingServerItem = this.offendingServerItem.GetServerItem(this.Reader, true),
      OwnerName = VersionControlObjectBinder<PendingChangeConflict>.GetIdentityDisplayName(this.m_requestContext, this.m_identityService, this.ownerId.GetGuid((IDataReader) this.Reader, true)),
      LockedWorkspace = this.workspaceName.GetString((IDataReader) this.Reader, true)
    };
  }
}
