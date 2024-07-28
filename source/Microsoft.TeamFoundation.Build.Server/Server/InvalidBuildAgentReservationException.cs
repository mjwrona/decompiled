// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.InvalidBuildAgentReservationException
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data.SqlClient;
using System.Text;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class InvalidBuildAgentReservationException : AdministrationException
  {
    public InvalidBuildAgentReservationException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
      : base(InvalidBuildAgentReservationException.FormatMessage(requestContext, ex, err))
    {
    }

    private static string FormatMessage(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError err)
    {
      switch (TeamFoundationServiceException.ExtractInt(err, "error"))
      {
        case 900540:
          return AdministrationResources.BuildAgentReservationInvalidBuildStatus((object) DBHelper.CreateArtifactUri("Build", TeamFoundationServiceException.ExtractInt(err, "buildId")));
        case 900544:
          return AdministrationResources.BuildAgentReservationCannotBeSatisfiedVersionMismatch((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "agentName")));
        default:
          string str = TeamFoundationServiceException.ExtractString(err, "requiredTags");
          TagComparison tagComparison = (TagComparison) TeamFoundationServiceException.ExtractInt(err, "tagComparison");
          if (string.IsNullOrEmpty(str))
            return tagComparison == TagComparison.MatchAtLeast ? AdministrationResources.BuildAgentReservationCannotBeSatisfied((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "agentName"))) : AdministrationResources.BuildAgentReservationCannotBeSatisfiedMatchExactly((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "agentName")));
          StringBuilder stringBuilder = new StringBuilder();
          string[] strArray = str.Split(new string[1]
          {
            ";-|-;"
          }, StringSplitOptions.RemoveEmptyEntries);
          stringBuilder.Append(strArray[0]);
          for (int index = 1; index < strArray.Length; ++index)
            stringBuilder.Append(AdministrationResources.ListSeparatorFormat((object) strArray[index]));
          return tagComparison == TagComparison.MatchAtLeast ? AdministrationResources.BuildAgentReservationCannotBeSatisfiedWithTags((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "agentName")), (object) stringBuilder) : AdministrationResources.BuildAgentReservationCannotBeSatisfiedWithTagsMatchExactly((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "controllerName")), (object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "agentName")), (object) stringBuilder);
      }
    }
  }
}
