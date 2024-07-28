// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStatusBadgeHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class CoverageStatusBadgeHelper
  {
    public static IList<string> GitRepoTypes = (IList<string>) new List<string>()
    {
      "TfsGit",
      "Git",
      "GitHub",
      "GitHubEnterprise"
    };

    public static HttpResponseMessage GenerateSvg(
      IVssRequestContext tfsRequestContext,
      CoverageResult coverageResult,
      string label,
      string rightText,
      object obj)
    {
      string content = CoverageStatusBadgeHelper.GetSVG(tfsRequestContext, coverageResult, label, rightText).ToString();
      HttpResponseMessage response = new HttpRequestMessage().CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerStringContent(content, Encoding.UTF8, "image/svg+xml", obj);
      return response;
    }

    private static XDocument GetSVG(
      IVssRequestContext requestContext,
      CoverageResult coverageResult,
      string leftText,
      string rightText)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string colorValue1 = CoverageStatusBadgeHelper.GetColorValue(requestContext, service, CoverageStatusBadgeHelper.DefaultSettings.s_DefinitionsColorMap[coverageResult]);
      string colorValue2 = CoverageStatusBadgeHelper.GetColorValue(requestContext, service, CoverageStatusBadgeHelper.DefaultSettings.LeftBackground);
      BadgeOptions options = new BadgeOptions(BadgeLogo.Pipelines, leftText, rightText, leftBackground: colorValue2, rightBackground: colorValue1);
      return BadgeSvgGenerator.CreateImage(requestContext, ref options);
    }

    private static string GetColorValue(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      Tuple<string, string> settings)
    {
      return registryService.GetValue<string>(requestContext, (RegistryQuery) settings.Item1, true, settings.Item2);
    }

    public struct PipelineGeneralSettings
    {
      public bool StatusBadgesArePublic;
      public static readonly CoverageStatusBadgeHelper.PipelineGeneralSettings Default = new CoverageStatusBadgeHelper.PipelineGeneralSettings()
      {
        StatusBadgesArePublic = true
      };
    }

    public static class SettingsKeys
    {
      public const string PipelinesGeneral = "Pipelines/General";
    }

    private static class DefaultSettings
    {
      public static readonly Dictionary<CoverageResult, Tuple<string, string>> s_DefinitionsColorMap = new Dictionary<CoverageResult, Tuple<string, string>>()
      {
        [CoverageResult.Succeeded] = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/RightBackgroundSucceeded", "#4EC820"),
        [CoverageResult.PartiallySucceeded] = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/RightBackgroundPartiallySucceeded", "#FEC006"),
        [CoverageResult.Failed] = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/RightBackgroundFailed", "#F34235"),
        [CoverageResult.NoData] = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/RightBackgroundNone", "#BBBBBB"),
        [CoverageResult.NoBuild] = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/RightBackgroundNone", "#BBBBBB"),
        [CoverageResult.NoDefinition] = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/RightBackgroundNone", "#BBBBBB")
      };

      public static Tuple<string, string> LeftBackground { get; } = new Tuple<string, string>("/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/LeftBackground", "#555555");
    }
  }
}
