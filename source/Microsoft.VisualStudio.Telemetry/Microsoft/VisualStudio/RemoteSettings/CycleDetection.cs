// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.CycleDetection
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class CycleDetection : ICycleDetection
  {
    private static Regex scopeRegex = new Regex("Scope\\.[a-zA-Z_0-9]+");

    public bool HasCycles(IEnumerable<Scope> scopes)
    {
      CycleDetection.Graph graph = new CycleDetection.Graph();
      foreach (Scope scope in scopes)
      {
        CycleDetection.Node node1 = graph.AddNode(scope.Name);
        foreach (object scopeMatch in CycleDetection.GetScopeMatches(scope.ScopeString))
        {
          string[] strArray = scopeMatch.ToString().Split('.');
          CycleDetection.Node node2 = graph.AddNode(strArray[1]);
          node1.AddDirected(node2);
        }
      }
      return graph.HasCycles();
    }

    internal static MatchCollection GetScopeMatches(string scopeString) => CycleDetection.scopeRegex.Matches(scopeString);

    internal class Node
    {
      private string value;
      private HashSet<CycleDetection.Node> edges = new HashSet<CycleDetection.Node>();

      public List<CycleDetection.Node> Edges => this.edges.ToList<CycleDetection.Node>();

      public Node(string value) => this.value = value;

      public void AddDirected(CycleDetection.Node node)
      {
        if (this.edges.Contains(node))
          return;
        this.edges.Add(node);
      }

      public override string ToString() => this.value;

      public override bool Equals(object obj) => obj is CycleDetection.Node && this.value.Equals(((CycleDetection.Node) obj).value);

      public override int GetHashCode() => this.value.GetHashCode();
    }

    internal class Graph
    {
      private Dictionary<string, CycleDetection.Node> nodes = new Dictionary<string, CycleDetection.Node>();

      public List<CycleDetection.Node> Nodes => this.nodes.Select<KeyValuePair<string, CycleDetection.Node>, CycleDetection.Node>((Func<KeyValuePair<string, CycleDetection.Node>, CycleDetection.Node>) (keyValue => keyValue.Value)).ToList<CycleDetection.Node>();

      public CycleDetection.Node AddNode(string value)
      {
        if (this.nodes.ContainsKey(value))
          return this.nodes[value];
        CycleDetection.Node node = new CycleDetection.Node(value);
        this.nodes[value] = node;
        return node;
      }

      public bool HasCycles()
      {
        Dictionary<CycleDetection.Node, CycleDetection.Color> colors = new Dictionary<CycleDetection.Node, CycleDetection.Color>();
        foreach (CycleDetection.Node node in this.Nodes)
        {
          if (!colors.ContainsKey(node) && this.ColorToFindCycles(node, (IDictionary<CycleDetection.Node, CycleDetection.Color>) colors))
            return true;
        }
        return false;
      }

      private bool ColorToFindCycles(
        CycleDetection.Node node,
        IDictionary<CycleDetection.Node, CycleDetection.Color> colors)
      {
        colors[node] = CycleDetection.Color.Gray;
        foreach (CycleDetection.Node edge in node.Edges)
        {
          if (!colors.ContainsKey(edge) && this.ColorToFindCycles(edge, colors) || colors[edge] == CycleDetection.Color.Gray)
            return true;
        }
        colors[node] = CycleDetection.Color.Black;
        return false;
      }
    }

    internal enum Color
    {
      Gray,
      Black,
    }
  }
}
