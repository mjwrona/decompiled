// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders.IBuildProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders
{
  [InheritedExport]
  public interface IBuildProvider
  {
    string GetLatestVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint);

    (string version, string versionName) GetLatestVersionAndName(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint);

    void Validate(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint);
  }
}
