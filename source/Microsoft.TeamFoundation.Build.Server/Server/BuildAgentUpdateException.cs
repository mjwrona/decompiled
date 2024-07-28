// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildAgentUpdateException
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
  public sealed class BuildAgentUpdateException : AdministrationException
  {
    public BuildAgentUpdateException(string property, string agentName)
      : base(AdministrationResources.ElasticBuildAgentCannotBeUpdated((object) property, (object) agentName))
    {
      this.FaultCode = Soap12FaultCodes.SenderFaultCode;
    }

    public BuildAgentUpdateException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(BuildAgentUpdateException.FormatMessage(requestContext, ex, err))
    {
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
    {
      switch (TeamFoundationServiceException.ExtractInt(err, "error"))
      {
        case 900523:
          return AdministrationResources.BuildAgentCannotBeMovedWhileReserved((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "displayName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        case 900525:
          return AdministrationResources.ElasticBuildAgentCannotBeAdded((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "displayName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        case 900526:
          return AdministrationResources.BuildAgentVersionMismatch((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "displayName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        default:
          return string.Empty;
      }
    }
  }
}
