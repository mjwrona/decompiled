// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces.ILegacyCssUtilsService
// Assembly: Microsoft.Azure.Boards.LegacyInterfaces, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C0E0C41-D39C-453E-A6CF-32A7C57153EE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.LegacyInterfaces.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsCssUtilsService, Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations")]
  public interface ILegacyCssUtilsService : IVssFrameworkService
  {
    object GetTreeValues(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureType nodeType,
      out int nodeCount);

    object CreateNode(
      string nodeId,
      string nodeName,
      string parentId,
      ICollection<object> values,
      IEnumerable<object> children);

    List<CommonStructureNodeInfo> GetNodes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> nodeIds);
  }
}
