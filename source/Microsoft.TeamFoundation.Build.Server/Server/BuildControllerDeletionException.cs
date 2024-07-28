// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildControllerDeletionException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class BuildControllerDeletionException : AdministrationException
  {
    public BuildControllerDeletionException(string controllerName)
      : base(AdministrationResources.ElasticBuildControllerCannotBeDeleted((object) controllerName))
    {
      this.FaultCode = Soap12FaultCodes.SenderFaultCode;
    }

    public BuildControllerDeletionException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(BuildControllerDeletionException.FormatMessage(requestContext, ex, err))
    {
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
    {
      switch (TeamFoundationServiceException.ExtractInt(err, "error"))
      {
        case 900513:
          return AdministrationResources.BuildControllerCannotBeDeletedBuildsInProgress((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "displayName")));
        case 900514:
          return AdministrationResources.BuildControllerCannotBeDeletedAgentsAssociated((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "displayName")));
        default:
          return string.Empty;
      }
    }
  }
}
