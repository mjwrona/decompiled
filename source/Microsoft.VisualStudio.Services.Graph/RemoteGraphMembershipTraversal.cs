// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.RemoteGraphMembershipTraversal
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class RemoteGraphMembershipTraversal
  {
    public SubjectDescriptor SubjectDescriptor { get; }

    public IDictionary<Guid, SubjectDescriptor> TraversedRemoteIdToDescriptorDict { get; }

    public bool IsComplete { get; }

    public string IncompletenessReason { get; }

    public RemoteGraphMembershipTraversal(
      SubjectDescriptor subjectDescriptor,
      IDictionary<Guid, SubjectDescriptor> traversedRemoteIdToDescriptorDict,
      bool isComplete,
      string incompletenessReason = null)
    {
      ArgumentUtility.CheckForNull<IDictionary<Guid, SubjectDescriptor>>(traversedRemoteIdToDescriptorDict, nameof (traversedRemoteIdToDescriptorDict));
      foreach (KeyValuePair<Guid, SubjectDescriptor> keyValuePair in (IEnumerable<KeyValuePair<Guid, SubjectDescriptor>>) traversedRemoteIdToDescriptorDict)
      {
        if (keyValuePair.Value == new SubjectDescriptor())
          throw new ArgumentException(CommonResources.SubjectDescriptorEmpty((object) "remoteIdToDescriptor"));
      }
      this.SubjectDescriptor = subjectDescriptor;
      this.IsComplete = isComplete;
      this.IncompletenessReason = incompletenessReason;
      this.TraversedRemoteIdToDescriptorDict = traversedRemoteIdToDescriptorDict;
    }
  }
}
