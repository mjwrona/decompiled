// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDefinitionUpdateException
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
  public class BuildDefinitionUpdateException : BuildServerException
  {
    internal BuildDefinitionUpdateException(string message)
      : base(message)
    {
      this.FaultCode = Soap12FaultCodes.SenderFaultCode;
    }

    public BuildDefinitionUpdateException(BuildDefinition definition)
      : base(ResourceStrings.BuildContainerDropMustUseVirtual((object) definition.Name))
    {
    }

    public BuildDefinitionUpdateException(BuildController controller)
      : base(ResourceStrings.UnableToFindControllersServiceHost((object) controller.ServiceHostUri, (object) controller.Name))
    {
    }

    public BuildDefinitionUpdateException(BuildDefinition definition, BuildController controller)
      : base(ResourceStrings.BuildContainerDropNotSupported((object) definition.Name, (object) controller.Name))
    {
    }

    public BuildDefinitionUpdateException(
      IVssRequestContext context,
      SqlException ex,
      SqlError err)
      : this(BuildDefinitionUpdateException.Format(context, ex, err))
    {
    }

    private static string Format(IVssRequestContext context, SqlException ex, SqlError err)
    {
      switch (TeamFoundationServiceException.ExtractInt(err, "error"))
      {
        case 900011:
          return ResourceStrings.BuildDefinitionNameInvalid((object) DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "definitionName")));
        case 900043:
          string teamProjectName = BuildServerException.GetTeamProjectName(context, err, "teamProject");
          string serverPath = DBHelper.DBPathToServerPath(TeamFoundationServiceException.ExtractString(err, "buildGroup"));
          string versionControlPath = DBHelper.DBPathToVersionControlPath(TeamFoundationServiceException.ExtractString(err, "processTemplatePath"));
          string relativePath = serverPath;
          return ResourceStrings.UpgradeTemplateDoesNotSupportBatching((object) BuildPath.RootNoCanonicalize(teamProjectName, relativePath), (object) versionControlPath);
        case 900542:
          return ResourceStrings.BuildDefinitionCanOnlyBeSpecifiedOnce((object) DBHelper.CreateArtifactUri("Definition", TeamFoundationServiceException.ExtractInt(err, "definitionId")));
        default:
          return string.Empty;
      }
    }
  }
}
