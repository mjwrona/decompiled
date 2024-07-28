// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingChangeConflictColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class PendingChangeConflictColumns15 : PendingChangeConflictColumns
  {
    public PendingChangeConflictColumns15(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(requestContext, component)
    {
    }

    protected override PendingChangeConflict Bind() => new PendingChangeConflict(this.m_requestContext)
    {
      ErrorCode = this.cause.GetInt32((IDataReader) this.Reader),
      ServerItem = this.BestEffortGetServerItemProjectNamePath(this.targetServerItem.GetServerItem(this.Reader, true)),
      OffendingServerItem = this.BestEffortGetServerItemProjectNamePath(this.offendingServerItem.GetServerItem(this.Reader, true)),
      OwnerName = VersionControlObjectBinder<PendingChangeConflict>.GetIdentityDisplayName(this.m_requestContext, this.m_identityService, this.ownerId.GetGuid((IDataReader) this.Reader, true)),
      LockedWorkspace = this.workspaceName.GetString((IDataReader) this.Reader, true)
    };
  }
}
