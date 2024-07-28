// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.IRemoteGraphMembershipTraversalExtension
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Graph
{
  [InheritedExport]
  public interface IRemoteGraphMembershipTraversalExtension
  {
    RemoteGraphMembershipTraversal TraverseDescendants(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor);

    RemoteDirectoryName RemoteDirectoryName { get; }
  }
}
