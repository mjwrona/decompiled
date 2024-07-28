// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CannotMergeWithWorkspaceSpecAndPendingDeleteException
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
  public class CannotMergeWithWorkspaceSpecAndPendingDeleteException : ServerException
  {
    private string m_message;

    public CannotMergeWithWorkspaceSpecAndPendingDeleteException(
      string operation,
      string workspaceSpec,
      string serverItem)
      : base(string.Empty)
    {
      if (string.CompareOrdinal(operation, "branch") == 0)
        this.m_message = Resources.Format("CannotBranchWithWorkspaceSpecAndPendingDeleteException", (object) workspaceSpec, (object) serverItem);
      else if (string.CompareOrdinal(operation, "merge") == 0)
        this.m_message = Resources.Format(nameof (CannotMergeWithWorkspaceSpecAndPendingDeleteException), (object) workspaceSpec, (object) serverItem);
      else
        this.m_message = Resources.Format(nameof (CannotMergeWithWorkspaceSpecAndPendingDeleteException), (object) workspaceSpec, (object) serverItem);
    }

    public CannotMergeWithWorkspaceSpecAndPendingDeleteException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "operation"), WorkspaceSpec.Combine(TeamFoundationServiceException.ExtractString(sqlError, "workspaceName"), TeamFoundationServiceException.ExtractString(sqlError, "workspaceOwner")), ServerException.ExtractDataspaceServerItem(requestContext, sqlError, "serverItem"))
    {
    }

    public override string Message => this.m_message;
  }
}
