// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CannotChangeWorkspaceOwnerException
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
  public class CannotChangeWorkspaceOwnerException : ServerException
  {
    public CannotChangeWorkspaceOwnerException(
      string workspaceName,
      string oldOwner,
      string newOwner)
      : base(Resources.Format(nameof (CannotChangeWorkspaceOwnerException), (object) WorkspaceSpec.Combine(workspaceName, oldOwner), (object) newOwner))
    {
    }

    public CannotChangeWorkspaceOwnerException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "workspace"), ServerException.ExtractIdentityName(requestContext, sqlError, "oldIdentity"), ServerException.ExtractIdentityName(requestContext, sqlError, "newIdentity"))
    {
    }
  }
}
