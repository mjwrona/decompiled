// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Providers.IWorkItemsProvider
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Providers
{
  [InheritedExport]
  public interface IWorkItemsProvider
  {
    string ProviderName { get; }

    IList<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactSourceVersion sourceVersion,
      IEnumerable<Change> changes,
      int maxItems = 250);
  }
}
