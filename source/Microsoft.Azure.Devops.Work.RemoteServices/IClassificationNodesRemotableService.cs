// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.IClassificationNodesRemotableService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  [DefaultServiceImplementation("Microsoft.Azure.Devops.Work.PlatformServices.PlatformClassificationNodesService, Microsoft.Azure.Devops.Work.PlatformServices")]
  public interface IClassificationNodesRemotableService : IVssFrameworkService
  {
    IEnumerable<WorkItemClassificationNode> GetRootNodes(
      IVssRequestContext requestContext,
      string projectName,
      int? depth = 0);

    IEnumerable<WorkItemClassificationNode> GetRootNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      int? depth = 0);

    WorkItemClassificationNode GetClassificationNode(
      IVssRequestContext requestContext,
      Guid ProjectId,
      TreeStructureGroup treeStructureGroup,
      string path = null,
      int depth = 0);

    WorkItemClassificationNode GetClassificationNode(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureGroup treeStructureGroup,
      string path = null,
      int depth = 0);

    IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<Guid> nodeIds);

    IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> nodeIds);
  }
}
