// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.IPipelineTfsVersionControlService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  [DefaultServiceImplementation(typeof (FrameworkTfsVersionControlService))]
  public interface IPipelineTfsVersionControlService : IVssFrameworkService
  {
    string GetItemContent(IVssRequestContext requestContext, string path, int changesetId);

    CommitInfo GetChangeset(
      IVssRequestContext requestContext,
      IList<TfvcMappingFilter> pathFilters,
      int changesetId);

    CommitInfo GetLatestChangeset(
      IVssRequestContext requestContext,
      IList<TfvcMappingFilter> pathFilters);
  }
}
