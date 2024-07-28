// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DockerfileParser
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class DockerfileParser
  {
    private const string c_layer = "DockerfileParser";
    private const int c_maxBuildStages = 10;
    private const int c_maxExposedPortsPerStage = 50;

    public static IDockerfileInfo Parse(IVssRequestContext requestContext, string contents)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(contents, nameof (contents));
      return (IDockerfileInfo) new DockerfileParser.DockerfileInfo((IReadOnlyList<IDockerBuildStage>) DockerfileParser.ParseInternal(requestContext, contents));
    }

    private static List<IDockerBuildStage> ParseInternal(
      IVssRequestContext requestContext,
      string contents)
    {
      using (StringReader stringReader = new StringReader(contents))
      {
        DockerfileParser.ParserContext parserContext = new DockerfileParser.ParserContext();
        for (string str = stringReader.ReadLine(); str != null; str = stringReader.ReadLine())
        {
          string line = str.Trim();
          if (line.Length > 0 && !line.StartsWith("#"))
          {
            DockerfileParser.ParseLine(line, parserContext);
            if (parserContext.BuildStages.Count >= 10)
            {
              requestContext.TraceWarning(TracePoints.BuildFrameworkDetection.DockerfileParserBuildStagesLimitReached, nameof (DockerfileParser), "Read more than {0} build stages, stopping parsing", (object) 10);
              return parserContext.BuildStages;
            }
            DockerfileParser.DockerBuildStage currentBuildStage = parserContext.CurrentBuildStage;
            if ((currentBuildStage != null ? (currentBuildStage.ExposedPorts.Count >= 50 ? 1 : 0) : 0) != 0)
            {
              requestContext.TraceWarning(TracePoints.BuildFrameworkDetection.DockerfileParserExposedPortsPerStageLimitReached, nameof (DockerfileParser), "Read more than {0} exposed ports in a single stage, stopping parsing", (object) 50);
              return parserContext.BuildStages;
            }
          }
        }
        return parserContext.BuildStages;
      }
    }

    private static void ParseLine(string line, DockerfileParser.ParserContext parserContext)
    {
      Match match1 = Regex.Match(line, "^FROM\\s+(?<image>[^:@\\s]+)(?:(?::(?<tag>\\S+))|(?:@(?<digest>\\S+)))?(?:\\s+AS\\s+(?<name>.+))?$", RegexOptions.IgnoreCase);
      if (match1.Success)
        parserContext.NewStage(new BaseImage(GetValueOrNull(match1.Groups["image"]), GetValueOrNull(match1.Groups["tag"]), GetValueOrNull(match1.Groups["digest"]), GetValueOrNull(match1.Groups["name"])));
      Match match2 = Regex.Match(line, "^EXPOSE(?:\\s+(\\d+(?:\\/\\w+)?))+$", RegexOptions.IgnoreCase);
      if (!match2.Success || parserContext.CurrentBuildStage == null)
        return;
      parserContext.CurrentBuildStage.ExposedPorts.AddRange(match2.Groups[1].Captures.Cast<Capture>().Select<Capture, string>((Func<Capture, string>) (c => c.Value)));

      static string GetValueOrNull(Group group) => !group.Success ? (string) null : group.Value;
    }

    private class ParserContext
    {
      public List<IDockerBuildStage> BuildStages { get; } = new List<IDockerBuildStage>();

      public DockerfileParser.DockerBuildStage CurrentBuildStage { get; private set; }

      public void NewStage(BaseImage baseImage)
      {
        this.CurrentBuildStage = new DockerfileParser.DockerBuildStage()
        {
          BaseImage = baseImage
        };
        this.BuildStages.Add((IDockerBuildStage) this.CurrentBuildStage);
      }
    }

    private class DockerfileInfo : IDockerfileInfo
    {
      public IReadOnlyList<IDockerBuildStage> BuildStages { get; }

      public DockerfileInfo(IReadOnlyList<IDockerBuildStage> buildStages) => this.BuildStages = buildStages;
    }

    private class DockerBuildStage : IDockerBuildStage
    {
      public BaseImage BaseImage { get; set; }

      public List<string> ExposedPorts { get; set; } = new List<string>();

      IReadOnlyList<string> IDockerBuildStage.ExposedPorts => (IReadOnlyList<string>) this.ExposedPorts;
    }
  }
}
