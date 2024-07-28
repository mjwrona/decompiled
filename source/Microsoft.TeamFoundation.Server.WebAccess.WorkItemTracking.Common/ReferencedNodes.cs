// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ReferencedNodes
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class ReferencedNodes : BaseSecuredObjectModel
  {
    public ReferencedNodes(
      IEnumerable<ExtendedTreeNode> areaNodes,
      IEnumerable<ExtendedTreeNode> iterationNodes,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.AreaNodes = areaNodes;
      this.IterationNodes = iterationNodes;
    }

    [DataMember(Name = "areaNodes")]
    public IEnumerable<ExtendedTreeNode> AreaNodes { get; set; }

    [DataMember(Name = "iterationNodes")]
    public IEnumerable<ExtendedTreeNode> IterationNodes { get; set; }
  }
}
