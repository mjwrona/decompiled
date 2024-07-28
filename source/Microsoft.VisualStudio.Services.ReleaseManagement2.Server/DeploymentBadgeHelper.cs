// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DeploymentBadgeHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server
{
  public static class DeploymentBadgeHelper
  {
    private static readonly Dictionary<DeploymentStatus, Tuple<string, string>> DefinitionsColorMap = new Dictionary<DeploymentStatus, Tuple<string, string>>();
    private static readonly Tuple<string, string> DefinitionsRightBackgroundNoDeployment = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundNoEnvironment", "#4da2db");
    private static readonly Tuple<string, string> DefinitionsRightBackgroundColorNoDefinition = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundNoDefinition", "#007ACC");

    public static XDocument GetSVGForEnvironment(
      IVssRequestContext requestContext,
      Deployment releaseEnvironmentDeployment,
      bool isDefinitionConfigured = true)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      BadgeOptions options = new BadgeOptions(BadgeLogo.Pipelines, Resources.DeploymentBadgeLeftText);
      if (releaseEnvironmentDeployment != null)
      {
        DeploymentStatus status = releaseEnvironmentDeployment.Status;
        DeploymentOperationStatus operationStatus = releaseEnvironmentDeployment.OperationStatus;
        switch (status)
        {
          case DeploymentStatus.Succeeded:
            options.RightText = Resources.DeploymentBadgeSucceeded;
            break;
          case DeploymentStatus.PartiallySucceeded:
            options.RightText = Resources.DeploymentBadgePartiallySucceeded;
            break;
          case DeploymentStatus.Failed:
            options.RightText = operationStatus != DeploymentOperationStatus.Canceled ? Resources.DeploymentBadgeFailed : Resources.DeploymentBadgeCanceled;
            break;
          default:
            options.RightText = Resources.DeploymentBadgeNone;
            break;
        }
        options.RightBackground = DeploymentBadgeHelper.GetColorValue(requestContext, service, DeploymentBadgeHelper.GetDefinitionsRightBackgroundColor(status));
      }
      else if (!isDefinitionConfigured)
      {
        options.RightText = Resources.DeploymentBadgeNoDefinition;
        options.RightBackground = DeploymentBadgeHelper.GetColorValue(requestContext, service, DeploymentBadgeHelper.DefinitionsRightBackgroundColorNoDefinition);
      }
      else
      {
        options.RightText = Resources.DeploymentBadgeNoDeployment;
        options.RightBackground = DeploymentBadgeHelper.GetColorValue(requestContext, service, DeploymentBadgeHelper.DefinitionsRightBackgroundNoDeployment);
      }
      return BadgeSvgGenerator.CreateImage(requestContext, ref options);
    }

    static DeploymentBadgeHelper()
    {
      DeploymentBadgeHelper.DefinitionsColorMap[DeploymentStatus.NotDeployed] = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundSucceeded", "#FEC006");
      DeploymentBadgeHelper.DefinitionsColorMap[DeploymentStatus.Failed] = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundFailed", "#F34235");
      DeploymentBadgeHelper.DefinitionsColorMap[DeploymentStatus.Undefined] = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundNone", "#BBBBBB");
      DeploymentBadgeHelper.DefinitionsColorMap[DeploymentStatus.PartiallySucceeded] = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundPartiallySucceeded", "#FEC006");
      DeploymentBadgeHelper.DefinitionsColorMap[DeploymentStatus.Succeeded] = new Tuple<string, string>("/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundSucceeded", "#4EC820");
    }

    private static string GetColorValue(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      Tuple<string, string> settings)
    {
      return registryService.GetValue<string>(requestContext, (RegistryQuery) settings.Item1, true, settings.Item2);
    }

    private static Tuple<string, string> GetDefinitionsRightBackgroundColor(DeploymentStatus result)
    {
      Tuple<string, string> definitionsColor;
      if (!DeploymentBadgeHelper.DefinitionsColorMap.TryGetValue(result, out definitionsColor))
        definitionsColor = DeploymentBadgeHelper.DefinitionsColorMap[DeploymentStatus.Undefined];
      return definitionsColor;
    }

    private static class RegistryKeys
    {
      public const string BadgePath = "/Service/ReleaseManagement/Settings/Badges/";
      public const string DefinitionsPath = "/Service/ReleaseManagement/Settings/Badges/Definitions/";
      public const string LeftBackground = "/Service/ReleaseManagement/Settings/Badges/Definitions/LeftBackground";
      public const string RightBackgroundFailed = "/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundFailed";
      public const string RightBackgroundPartiallySucceeded = "/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundPartiallySucceeded";
      public const string RightBackgroundSucceeded = "/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundSucceeded";
      public const string RightBackgroundNone = "/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundNone";
      public const string RightBackgroundNoDefinition = "/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundNoDefinition";
      public const string RightBackgroundNoRuns = "/Service/ReleaseManagement/Settings/Badges/Definitions/RightBackgroundNoEnvironment";
    }
  }
}
