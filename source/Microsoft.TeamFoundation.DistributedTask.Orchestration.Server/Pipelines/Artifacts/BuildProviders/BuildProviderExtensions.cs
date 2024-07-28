// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders.BuildProviderExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders
{
  public static class BuildProviderExtensions
  {
    public static string ResolveVersion(
      this IBuildProvider buildProvider,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint)
    {
      string version = build.Version;
      string versionName1 = build.VersionName;
      if (string.IsNullOrEmpty(version))
      {
        string versionName2;
        (version, versionName2) = buildProvider.GetLatestVersionAndName(requestContext, projectId, build, endpoint);
        build.Version = !string.IsNullOrEmpty(version) ? version : throw new ExternalBuildProviderException("Unable to resolve latest version for build " + build.Alias);
        build.VersionName = versionName2;
      }
      return version;
    }
  }
}
