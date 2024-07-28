// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.IResourcePlugin
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  [InheritedExport]
  public interface IResourcePlugin
  {
    ResourceType Type { get; }

    IList<Resource> GetResourcesWithPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Resource> resources,
      ResourcePermission permission);

    IList<Resource> GetDefaultAuthorizedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId);

    void PopulateLinkedResources(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Resource> resources,
      out IList<Resource> publicLinkedResources,
      out IList<Resource> privateLinkedResources);

    bool HasProjectLevelAdminPermission(IVssRequestContext requestContext, Guid projectId);
  }
}
