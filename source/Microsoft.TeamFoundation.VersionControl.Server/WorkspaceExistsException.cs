// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceExistsException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class WorkspaceExistsException : WorkspaceException
  {
    private string m_computerName = Resources.Get("UnknownString");

    public WorkspaceExistsException(string workspaceName, string ownerName)
      : base(workspaceName, ownerName)
    {
    }

    public WorkspaceExistsException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "workspace"), ServerException.ExtractIdentityName(requestContext, sqlError, "identity"))
    {
    }

    public override string Message => Resources.Format(nameof (WorkspaceExistsException), (object[]) new string[2]
    {
      WorkspaceSpec.Combine(this.m_workspaceName, this.m_workspaceOwner),
      this.m_computerName
    });

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      failure.ComputerName = this.m_computerName;
    }

    public string ComputerName
    {
      get => this.m_computerName;
      set => this.m_computerName = value;
    }
  }
}
