// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.StatusBadgeHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class StatusBadgeHelper
  {
    public static XDocument GetSVG(
      IVssRequestContext requestContext,
      bool isDefinitionConfigured,
      BuildResult? buildResult,
      string leftText)
    {
      if (string.IsNullOrWhiteSpace(leftText))
        leftText = BuildServerResources.DefinitionBadgeLeftText();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      BadgeOptions options = new BadgeOptions(BadgeLogo.Pipelines, leftText);
      if (!isDefinitionConfigured)
      {
        options.RightText = BuildServerResources.DefinitionBadgeNoDefinition();
        options.RightBackground = StatusBadgeHelper.GetColorValue(requestContext, service, StatusBadgeHelper.DefaultSettings.RightBackgroundNoDefinition);
      }
      else
      {
        if (buildResult.HasValue)
        {
          switch (buildResult.GetValueOrDefault())
          {
            case BuildResult.Succeeded:
              options.RightText = BuildServerResources.DefinitionBadgeSucceeded();
              break;
            case BuildResult.PartiallySucceeded:
              options.RightText = BuildServerResources.DefinitionBadgePartiallySucceeded();
              break;
            case BuildResult.Failed:
              options.RightText = BuildServerResources.DefinitionBadgeFailed();
              break;
            case BuildResult.Canceled:
              options.RightText = BuildServerResources.DefinitionBadgeCanceled();
              break;
            default:
              options.RightText = BuildServerResources.DefinitionBadgeNone();
              break;
          }
        }
        else
          options.RightText = BuildServerResources.DefinitionBadgeNoBuilds();
        options.RightBackground = StatusBadgeHelper.GetColorValue(requestContext, service, StatusBadgeHelper.DefaultSettings.GetBuildResultBackgroundColor(buildResult));
      }
      return BadgeSvgGenerator.CreateImage(requestContext, ref options);
    }

    private static string GetColorValue(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      Tuple<string, string> settings)
    {
      return registryService.GetValue<string>(requestContext, (RegistryQuery) settings.Item1, true, settings.Item2);
    }

    private static class RegistryKeys
    {
      public const string BadgePath = "/Service/Build/Settings/Badges/";
      public const string DefinitionsPath = "/Service/Build/Settings/Badges/Definitions/";
      public const string LeftBackground = "/Service/Build/Settings/Badges/Definitions/LeftBackground";
      public const string RightBackgroundFailed = "/Service/Build/Settings/Badges/Definitions/RightBackgroundFailed";
      public const string RightBackgroundPartiallySucceeded = "/Service/Build/Settings/Badges/Definitions/RightBackgroundPartiallySucceeded";
      public const string RightBackgroundSucceeded = "/Service/Build/Settings/Badges/Definitions/RightBackgroundSucceeded";
      public const string RightBackgroundNone = "/Service/Build/Settings/Badges/Definitions/RightBackgroundNone";
      public const string RightBackgroundNoDefinition = "/Service/Build/Settings/Badges/Definitions/RightBackgroundNoDefinition";
      public const string RightBackgroundNoRuns = "/Service/Build/Settings/Badges/Definitions/RightBackgroundNoBuilds";
    }

    private static class DefaultSettings
    {
      private static readonly Tuple<string, string> s_RightBackgroundNoBuilds = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundNoBuilds", "#4da2db");
      private static readonly Dictionary<BuildResult, Tuple<string, string>> s_DefinitionsColorMap = new Dictionary<BuildResult, Tuple<string, string>>()
      {
        [BuildResult.Canceled] = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundSucceeded", "#FEC006"),
        [BuildResult.Failed] = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundFailed", "#F34235"),
        [BuildResult.None] = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundNone", "#BBBBBB"),
        [BuildResult.PartiallySucceeded] = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundPartiallySucceeded", "#FEC006"),
        [BuildResult.Succeeded] = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundSucceeded", "#4EC820")
      };

      public static Tuple<string, string> LeftBackground { get; } = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/LeftBackground", "#555555");

      public static Tuple<string, string> RightBackgroundNoDefinition { get; } = new Tuple<string, string>("/Service/Build/Settings/Badges/Definitions/RightBackgroundNoDefinition", "#007ACC");

      public static Tuple<string, string> GetBuildResultBackgroundColor(BuildResult? buildResult) => !buildResult.HasValue ? StatusBadgeHelper.DefaultSettings.s_RightBackgroundNoBuilds : StatusBadgeHelper.DefaultSettings.s_DefinitionsColorMap[buildResult.Value];
    }
  }
}
