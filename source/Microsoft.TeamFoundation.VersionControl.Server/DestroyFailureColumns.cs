// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DestroyFailureColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class DestroyFailureColumns : VersionControlObjectBinder<Failure>
  {
    protected SqlColumnBinder versionFrom = new SqlColumnBinder("VersionFrom");
    protected SqlColumnBinder serverItem = new SqlColumnBinder("ServerItem");
    protected SqlColumnBinder workspaceName = new SqlColumnBinder("WorkspaceName");
    protected SqlColumnBinder type = new SqlColumnBinder("Type");
    protected SqlColumnBinder workspaceOwnerId = new SqlColumnBinder("WorkspaceOwnerId");
    protected SqlColumnBinder errorCode = new SqlColumnBinder("Error");
    protected IdentityService m_identityService;
    protected IVssRequestContext m_requestContext;

    internal DestroyFailureColumns(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    internal DestroyFailureColumns(
      IVssRequestContext requestContext,
      VersionControlSqlResourceComponent component)
      : base(component)
    {
      this.m_requestContext = requestContext;
      this.m_identityService = requestContext.GetService<IdentityService>();
    }

    protected override Failure Bind()
    {
      Failure failure = new Failure();
      int num = (int) this.type.GetByte((IDataReader) this.Reader);
      this.versionFrom.GetInt32((IDataReader) this.Reader);
      string serverItem = this.serverItem.GetServerItem(this.Reader, false);
      string workspace = this.workspaceName.GetString((IDataReader) this.Reader, false);
      string identityDisplayName = VersionControlObjectBinder<Failure>.GetIdentityDisplayName(this.m_requestContext, this.m_identityService, this.workspaceOwnerId.GetGuid((IDataReader) this.Reader));
      this.errorCode.GetInt32((IDataReader) this.Reader);
      failure.Code = "DestroyFailure";
      failure.Severity = SeverityType.Error;
      if (num == 1)
        failure.Message = Resources.Format("CannotDestroyItemInUseByShelf", (object) serverItem, (object) WorkspaceSpec.Combine(workspace, identityDisplayName));
      else
        failure.Message = Resources.Format("CannotDestroyItemInUseByWorkspace", (object) serverItem, (object) WorkspaceSpec.Combine(workspace, identityDisplayName));
      return failure;
    }
  }
}
