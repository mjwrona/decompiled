// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CannotStartBuildException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public class CannotStartBuildException : BuildServerException
  {
    internal CannotStartBuildException(string message)
      : base(message)
    {
      this.SetExceptionProperties();
    }

    public CannotStartBuildException(IVssRequestContext context, SqlException ex, SqlError err)
      : this(CannotStartBuildException.FormatMessage(context, err))
    {
    }

    protected void SetExceptionProperties() => this.FaultCode = Soap12FaultCodes.SenderFaultCode;

    private static string FormatMessage(IVssRequestContext requestContext, SqlError err)
    {
      switch (TeamFoundationServiceException.ExtractInt(err, "error"))
      {
        case 900535:
          string teamProjectName = BuildServerException.GetTeamProjectName(requestContext, err, "teamProject");
          string serverPath = DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "definitionName"));
          return BuildTypeResource.CannotStartBuildForActiveDefinition((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) BuildPath.Root(teamProjectName, serverPath));
        case 900536:
          return BuildTypeResource.CannotStartBuildForUnavailableController((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        case 900537:
          return BuildTypeResource.CannotStartBuildControllerOverloaded((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        case 900539:
          return BuildTypeResource.CannotStartBuildGatedInProgress((object) TeamFoundationServiceException.ExtractInt(err, "queueId"), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")));
        default:
          return string.Empty;
      }
    }
  }
}
