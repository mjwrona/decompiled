// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DestroyFailureColumns15
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class DestroyFailureColumns15 : DestroyFailureColumns
  {
    internal DestroyFailureColumns15(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(requestContext, component)
    {
    }

    protected override Failure Bind()
    {
      Failure failure = new Failure();
      int num = (int) this.type.GetByte((IDataReader) this.Reader);
      this.versionFrom.GetInt32((IDataReader) this.Reader);
      string itemProjectNamePath = this.BestEffortGetServerItemProjectNamePath(this.serverItem.GetServerItem(this.Reader, false));
      string workspace = this.workspaceName.GetString((IDataReader) this.Reader, false);
      string identityDisplayName = VersionControlObjectBinder<Failure>.GetIdentityDisplayName(this.m_requestContext, this.m_identityService, this.workspaceOwnerId.GetGuid((IDataReader) this.Reader));
      this.errorCode.GetInt32((IDataReader) this.Reader);
      failure.Code = "DestroyFailure";
      failure.Severity = SeverityType.Error;
      if (num == 1)
        failure.Message = Resources.Format("CannotDestroyItemInUseByShelf", (object) itemProjectNamePath, (object) WorkspaceSpec.Combine(workspace, identityDisplayName));
      else
        failure.Message = Resources.Format("CannotDestroyItemInUseByWorkspace", (object) itemProjectNamePath, (object) WorkspaceSpec.Combine(workspace, identityDisplayName));
      return failure;
    }
  }
}
