// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamIterationsCollection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class TeamIterationsCollection : 
    ITeamIterationsCollection,
    IEnumerable<ITeamIteration>,
    IEnumerable
  {
    private IDictionary<Guid, ITeamIteration> m_iterations;

    public TeamIterationsCollection() => this.m_iterations = (IDictionary<Guid, ITeamIteration>) new Dictionary<Guid, ITeamIteration>();

    public IEnumerable<ITeamIteration> Values => this.m_iterations.Values.Cast<ITeamIteration>();

    public ITeamIteration GetIteration(Guid iterationId)
    {
      ITeamIteration iteration;
      this.m_iterations.TryGetValue(iterationId, out iteration);
      return iteration;
    }

    public void DeleteIteration(ITeamIteration iteration)
    {
      ArgumentUtility.CheckForNull<ITeamIteration>(iteration, nameof (iteration));
      if (!this.m_iterations.ContainsKey(iteration.IterationId))
        throw new InvalidOperationException(Resources.TeamIterationsCollection_DeleteInvalidIteration);
      this.m_iterations.Remove(iteration.IterationId);
    }

    public ITeamIteration CreateIteration(Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode iterationNode)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode>(iterationNode, nameof (iterationNode));
      if (iterationNode.Type != TreeStructureType.Iteration)
        throw new InvalidOperationException(Resources.TeamIterationsCollection_CreateForNonIteration);
      if (iterationNode.IsProject || iterationNode.IsStructureSpecifier)
        throw new InvalidOperationException(Resources.TeamIterationsCollection_CreateTopLevelIterationError);
      if (this.m_iterations.ContainsKey(iterationNode.CssNodeId))
        throw new InvalidOperationException(Resources.TeamIterationsCollection_CreateAlreadyExists);
      TeamIteration iteration = new TeamIteration(iterationNode.CssNodeId);
      this.m_iterations.Add(iteration.IterationId, (ITeamIteration) iteration);
      return (ITeamIteration) iteration;
    }

    public IEnumerator<ITeamIteration> GetEnumerator() => this.m_iterations.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_iterations.Values.GetEnumerator();

    internal void AddIteration(ITeamIteration iteration) => this.m_iterations.Add(iteration.IterationId, iteration);
  }
}
