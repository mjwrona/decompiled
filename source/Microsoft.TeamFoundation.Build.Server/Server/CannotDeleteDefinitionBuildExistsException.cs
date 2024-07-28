// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.CannotDeleteDefinitionBuildExistsException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public class CannotDeleteDefinitionBuildExistsException : BuildServerException
  {
    public CannotDeleteDefinitionBuildExistsException(
      IVssRequestContext context,
      SqlException ex,
      SqlError err)
      : base(CannotDeleteDefinitionBuildExistsException.FormatMessage(context, err))
    {
    }

    private static string FormatMessage(IVssRequestContext context, SqlError err) => TeamFoundationServiceException.ExtractInt(err, "error") == 900028 ? ResourceStrings.CannotDeleteDefinitionBuildExists((object) BuildPath.Root(BuildServerException.GetTeamProjectName(context, err, "definitionProject"), DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "definitionPath")))) : ResourceStrings.CannotDeleteDefinitionQueuedBuildExists((object) BuildPath.Root(BuildServerException.GetTeamProjectName(context, TeamFoundationServiceException.ExtractString(err, "definitionProject")), DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "definitionPath"))), (object) BuildPath.Root(BuildServerException.GetTeamProjectName(context, TeamFoundationServiceException.ExtractString(err, "agentProject")), DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "agentPath"))));
  }
}
