// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BisQuery
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal sealed class BisQuery
  {
    internal static Artifact[] GetV1Artifacts(
      IVssRequestContext requestContext,
      string[] artifactUriList)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return BisQuery.GetArtifacts(requestContext, artifactUriList, BisQuery.\u003C\u003EO.\u003C0\u003E__CreateV1Artifact ?? (BisQuery.\u003C\u003EO.\u003C0\u003E__CreateV1Artifact = new BisQuery.CreateArtifact(BisQuery.CreateV1Artifact)));
    }

    internal static Artifact[] GetV2Artifacts(
      IVssRequestContext requestContext,
      string[] artifactUriList)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return BisQuery.GetArtifacts(requestContext, artifactUriList, BisQuery.\u003C\u003EO.\u003C1\u003E__CreateV2Artifact ?? (BisQuery.\u003C\u003EO.\u003C1\u003E__CreateV2Artifact = new BisQuery.CreateArtifact(BisQuery.CreateV2Artifact)));
    }

    private static Artifact[] GetArtifacts(
      IVssRequestContext requestContext,
      string[] artifactUriList,
      BisQuery.CreateArtifact createArtifact)
    {
      requestContext.TraceEnter(0, "Build", "Notification", nameof (GetArtifacts));
      if (artifactUriList == null || artifactUriList.Length == 0)
      {
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Exiting due to no input artifact uris");
        return Array.Empty<Artifact>();
      }
      Artifact[] artifacts = new Artifact[artifactUriList.Length];
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationBuildService>().QueryBuildsByUri(requestContext, (IList<string>) artifactUriList, (IList<string>) null, QueryOptions.Definitions, QueryDeletedOption.ExcludeDeleted, new Guid(), false))
      {
        int index = 0;
        foreach (BuildDetail build in foundationDataReader.Current<BuildQueryResult>().Builds)
        {
          if (build != null && LinkingUtilities.IsUriWellFormed(artifactUriList[index]))
          {
            ArtifactId artifactId = LinkingUtilities.DecodeUri(artifactUriList[index]);
            if (VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "Build"))
            {
              artifacts[index] = createArtifact(build);
              requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Created artifact '{0}'", (object) artifacts[index].Uri);
            }
            else
              requestContext.Trace(0, TraceLevel.Warning, "Build", "Notification", "Artifact type '{0}' is not build", (object) artifactId.ArtifactType);
          }
          else
            requestContext.Trace(0, TraceLevel.Warning, "Build", "Notification", "Build not found or artifact uri '{0}' not well-formed", (object) artifactUriList[index]);
          ++index;
        }
      }
      requestContext.TraceLeave(0, "Build", "Notification", nameof (GetArtifacts));
      return artifacts;
    }

    internal static Artifact CreateV1Artifact(BuildDetail build) => new Artifact()
    {
      Uri = build.Uri,
      ArtifactTitle = build.BuildNumber,
      ExtendedAttributes = new List<ExtendedAttribute>()
      {
        BisQuery.CreateExtendedAttribute("BuildType", build.Definition?.Name),
        BisQuery.CreateExtendedAttribute("BuildQuality", build.Quality),
        BisQuery.CreateExtendedAttribute("BuildStatus", EnumHelper.ToString(build.Status)),
        BisQuery.CreateExtendedAttribute("Team Project", build.Definition?.TeamProject.Name),
        BisQuery.CreateExtendedAttribute("StartTime", build.StartTime.ToString()),
        BisQuery.CreateExtendedAttribute("FinishTime", build.FinishTime.ToString()),
        BisQuery.CreateExtendedAttribute("DropLocation", build.DropLocation)
      }.ToArray()
    };

    private static Artifact CreateV2Artifact(BuildDetail build) => new Artifact()
    {
      Uri = build.Uri,
      ArtifactTitle = ResourceStrings.BuildArtifactTitle((object) BuildPath.Combine(build.Definition.FullPath, build.BuildNumber))
    };

    private static ExtendedAttribute CreateExtendedAttribute(string name, string value) => new ExtendedAttribute()
    {
      Name = name,
      Value = value
    };

    private delegate Artifact CreateArtifact(BuildDetail build);
  }
}
