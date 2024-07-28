// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.ISourceProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  [InheritedExport]
  public interface ISourceProvider
  {
    SourceProviderAttributes Attributes { get; }

    string GetDirectoryName(string path);

    string GetFileName(string path);

    string ResolvePath(string defaultRoot, string path);

    string GetFileContent(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource resource,
      ServiceEndpoint endpoint,
      string path);

    void ResolveVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint);

    void Validate(
      IVssRequestContext requestContext,
      Guid projectId,
      RepositoryResource repository,
      ServiceEndpoint endpoint);

    void PopulateJobData(
      IVssRequestContext requestContext,
      JobExecutionContext context,
      RepositoryResource repository,
      ServiceEndpoint endpoint);
  }
}
