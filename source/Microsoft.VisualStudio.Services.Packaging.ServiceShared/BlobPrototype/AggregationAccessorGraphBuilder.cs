// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AggregationAccessorGraphBuilder
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AggregationAccessorGraphBuilder : IGraphBuilder<IAggregationAccessor>
  {
    public IEnumerable<GraphNode<IAggregationAccessor>> Build(
      IEnumerable<IAggregationAccessor> aggregationAccessors)
    {
      return this.CreateAccessorDependencyGraph(aggregationAccessors);
    }

    private IEnumerable<GraphNode<IAggregationAccessor>> CreateAccessorDependencyGraph(
      IEnumerable<IAggregationAccessor> aggregationAccessors)
    {
      if (!(aggregationAccessors is IList<IAggregationAccessor> aggregationAccessorList))
        aggregationAccessorList = (IList<IAggregationAccessor>) aggregationAccessors.ToList<IAggregationAccessor>();
      IList<IAggregationAccessor> aggAccessors = aggregationAccessorList;
      Dictionary<string, GraphNode<IAggregationAccessor>> graphMap = new Dictionary<string, GraphNode<IAggregationAccessor>>();
      IDictionary<AggregationDefinition, ISet<IAggregationAccessor>> defToAggAccessors = this.CreateMapOfAggDefToAggAccessors((IEnumerable<IAggregationAccessor>) aggAccessors);
      foreach (AggregationDefinition key in (IEnumerable<AggregationDefinition>) defToAggAccessors.Keys)
      {
        foreach (IAggregationAccessor aggregationAccessor1 in this.GetAccessorsForAggDef(defToAggAccessors, key))
        {
          GraphNode<IAggregationAccessor> updateDictionary1 = this.CreateGraphNodeIfNeededAndUpdateDictionary(aggregationAccessor1, (IDictionary<string, GraphNode<IAggregationAccessor>>) graphMap);
          foreach (AggregationDefinition aggDef in key.DependsOn)
          {
            foreach (IAggregationAccessor aggregationAccessor2 in this.GetAccessorsForAggDef(defToAggAccessors, aggDef))
            {
              GraphNode<IAggregationAccessor> updateDictionary2 = this.CreateGraphNodeIfNeededAndUpdateDictionary(aggregationAccessor2, (IDictionary<string, GraphNode<IAggregationAccessor>>) graphMap);
              updateDictionary1.Edges.Add(updateDictionary2);
            }
          }
        }
      }
      return (IEnumerable<GraphNode<IAggregationAccessor>>) graphMap.Values;
    }

    private IEnumerable<IAggregationAccessor> GetAccessorsForAggDef(
      IDictionary<AggregationDefinition, ISet<IAggregationAccessor>> aggDefToAccessors,
      AggregationDefinition aggDef)
    {
      return aggDef == null || !aggDefToAccessors.ContainsKey(aggDef) ? Enumerable.Empty<IAggregationAccessor>() : (IEnumerable<IAggregationAccessor>) aggDefToAccessors[aggDef];
    }

    private IDictionary<AggregationDefinition, ISet<IAggregationAccessor>> CreateMapOfAggDefToAggAccessors(
      IEnumerable<IAggregationAccessor> aggAccessors)
    {
      Dictionary<AggregationDefinition, ISet<IAggregationAccessor>> defToAggAccessors = new Dictionary<AggregationDefinition, ISet<IAggregationAccessor>>();
      foreach (IAggregationAccessor aggAccessor in aggAccessors)
      {
        AggregationDefinition definition = aggAccessor.Aggregation.Definition;
        if (!defToAggAccessors.ContainsKey(definition))
          defToAggAccessors.Add(definition, (ISet<IAggregationAccessor>) new HashSet<IAggregationAccessor>());
        defToAggAccessors[definition].Add(aggAccessor);
      }
      return (IDictionary<AggregationDefinition, ISet<IAggregationAccessor>>) defToAggAccessors;
    }

    private GraphNode<IAggregationAccessor> CreateGraphNodeIfNeededAndUpdateDictionary(
      IAggregationAccessor aggregationAccessor,
      IDictionary<string, GraphNode<IAggregationAccessor>> graphMap)
    {
      string key = this.GetKey(aggregationAccessor);
      if (graphMap.ContainsKey(key))
        return graphMap[key];
      GraphNode<IAggregationAccessor> updateDictionary = new GraphNode<IAggregationAccessor>(aggregationAccessor);
      graphMap.Add(key, updateDictionary);
      return updateDictionary;
    }

    private string GetKey(IAggregationAccessor aggregationAccessor)
    {
      IAggregation aggregation = aggregationAccessor.Aggregation;
      return aggregation.Definition.Name + "." + aggregation.VersionName;
    }
  }
}
