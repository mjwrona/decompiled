// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildServiceHostOwnershipException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class BuildServiceHostOwnershipException : AdministrationException
  {
    public BuildServiceHostOwnershipException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(BuildServiceHostOwnershipException.FormatErrorMessage(requestContext, ex, err))
    {
    }

    private static string FormatErrorMessage(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
    {
      string str = TeamFoundationServiceException.ExtractString(err, "ownerName");
      string serverPath = DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "computerName"));
      return !string.IsNullOrEmpty(str) && !str.Equals("(null)", StringComparison.Ordinal) ? AdministrationResources.BuildServiceHostOwnershipExceptionWithOwner((object) serverPath, (object) str) : AdministrationResources.BuildServiceHostOwnershipException((object) serverPath);
    }
  }
}
