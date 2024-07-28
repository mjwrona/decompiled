// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation.GraphValidator
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation
{
  internal static class GraphValidator
  {
    internal static void Validate<T>(
      PipelineBuildContext context,
      ValidationResult result,
      Func<object, string> getBaseRefName,
      string graphName,
      IList<T> nodes,
      GraphValidator.ErrorFormatter formatError)
      where T : class, IGraphNode
    {
      List<T> objList = new List<T>();
      List<T> collection = new List<T>();
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool flag = false;
      foreach (T node in (IEnumerable<T>) nodes)
      {
        if (!string.IsNullOrEmpty(node.Name))
        {
          if (!NameValidation.IsValid(node.Name, context.BuildOptions.AllowHyphenNames))
            result.Errors.Add(new PipelineValidationError("NameInvalid", formatError("NameInvalid", new object[2]
            {
              (object) graphName,
              (object) node.Name
            })));
          else if (!stringSet1.Add(node.Name))
          {
            flag = true;
            result.Errors.Add(new PipelineValidationError("NameNotUnique", formatError("NameNotUnique", new object[2]
            {
              (object) graphName,
              (object) node.Name
            })));
          }
        }
        else
          objList.Add(node);
        if (node.DependsOn.Count == 0)
          collection.Add(node);
      }
      int num = 1;
      foreach (T obj in objList)
      {
        string str;
        for (str = getBaseRefName((object) num); !stringSet1.Add(str); str = getBaseRefName((object) num))
          ++num;
        ++num;
        obj.Name = str;
      }
      foreach (T node in (IEnumerable<T>) nodes)
        node.Validate(context, result);
      if (collection.Count == 0)
      {
        result.Errors.Add(new PipelineValidationError("StartingPointNotFound", formatError("StartingPointNotFound", new object[1]
        {
          (object) graphName
        })));
      }
      else
      {
        if (flag)
          return;
        Queue<T> objQueue = new Queue<T>((IEnumerable<T>) collection);
        Dictionary<string, T> dictionary1 = nodes.ToDictionary<T, string>((Func<T, string>) (x => x.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<string>> dictionary2 = nodes.ToDictionary<T, string, List<string>>((Func<T, string>) (x => x.Name), (Func<T, List<string>>) (x => new List<string>((IEnumerable<string>) x.DependsOn)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        while (objQueue.Count > 0)
        {
          T obj = objQueue.Dequeue();
          stringSet2.Add(obj.Name);
          foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary2)
          {
            for (int index = keyValuePair.Value.Count - 1; index >= 0; --index)
            {
              if (keyValuePair.Value[index].Equals(obj.Name, StringComparison.OrdinalIgnoreCase))
              {
                keyValuePair.Value.RemoveAt(index);
                if (keyValuePair.Value.Count == 0)
                  objQueue.Enqueue(dictionary1[keyValuePair.Key]);
              }
            }
          }
        }
        if (nodes.Count - stringSet2.Count <= 0)
          return;
        foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary2.Where<KeyValuePair<string, List<string>>>((Func<KeyValuePair<string, List<string>>, bool>) (x => x.Value.Count > 0)))
        {
          foreach (string key in keyValuePair.Value)
          {
            if (!dictionary1.ContainsKey(key))
              result.Errors.Add(new PipelineValidationError("DependencyNotFound", formatError("DependencyNotFound", new object[3]
              {
                (object) graphName,
                (object) keyValuePair.Key,
                (object) key
              })));
            else
              result.Errors.Add(new PipelineValidationError("GraphContainsCycle", formatError("GraphContainsCycle", new object[3]
              {
                (object) graphName,
                (object) keyValuePair.Key,
                (object) key
              })));
          }
        }
      }
    }

    internal static void Traverse<T>(IList<T> nodes, Action<T, ISet<string>> handleNode) where T : class, IGraphNode
    {
      Dictionary<string, GraphValidator.GraphTraversalState<T>> dictionary1 = nodes.ToDictionary<T, string, GraphValidator.GraphTraversalState<T>>((Func<T, string>) (x => x.Name), (Func<T, GraphValidator.GraphTraversalState<T>>) (x => new GraphValidator.GraphTraversalState<T>(x)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<string>> dictionary2 = nodes.ToDictionary<T, string, List<string>>((Func<T, string>) (x => x.Name), (Func<T, List<string>>) (x => new List<string>((IEnumerable<string>) x.DependsOn)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Queue<GraphValidator.GraphTraversalState<T>> graphTraversalStateQueue = new Queue<GraphValidator.GraphTraversalState<T>>(nodes.Where<T>((Func<T, bool>) (x => x.DependsOn.Count == 0)).Select<T, GraphValidator.GraphTraversalState<T>>((Func<T, GraphValidator.GraphTraversalState<T>>) (x => new GraphValidator.GraphTraversalState<T>(x))));
      while (graphTraversalStateQueue.Count > 0)
      {
        GraphValidator.GraphTraversalState<T> graphTraversalState1 = graphTraversalStateQueue.Dequeue();
        handleNode(graphTraversalState1.Node, graphTraversalState1.Dependencies);
        foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary2)
        {
          for (int index = keyValuePair.Value.Count - 1; index >= 0; --index)
          {
            if (keyValuePair.Value[index].Equals(graphTraversalState1.Node.Name, StringComparison.OrdinalIgnoreCase))
            {
              keyValuePair.Value.RemoveAt(index);
              GraphValidator.GraphTraversalState<T> graphTraversalState2 = dictionary1[keyValuePair.Key];
              graphTraversalState2.Dependencies.Add(graphTraversalState1.Node.Name);
              graphTraversalState2.Dependencies.UnionWith((IEnumerable<string>) graphTraversalState1.Dependencies);
              if (keyValuePair.Value.Count == 0)
                graphTraversalStateQueue.Enqueue(graphTraversalState2);
            }
          }
        }
      }
    }

    internal delegate string ErrorFormatter(string code, params object[] values);

    private class GraphTraversalState<T> where T : class, IGraphNode
    {
      public GraphTraversalState(T node) => this.Node = node;

      public T Node { get; }

      public ISet<string> Dependencies { get; } = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
