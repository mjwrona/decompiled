// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeSelect
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeSelect : NodeList
  {
    public NodeSelect()
      : base(NodeType.Select, 7)
    {
    }

    public override DataType DataType => DataType.Void;

    public override Priority Priority => Priority.SelectOperator;

    public NodeFieldList Fields
    {
      get => (NodeFieldList) this[0];
      set => this[0] = (Node) value;
    }

    public NodeTableName From
    {
      get => (NodeTableName) this[1];
      set => this[1] = (Node) value;
    }

    public Node Where
    {
      get => this[2];
      set => this[2] = value;
    }

    public NodeFieldList GroupBy
    {
      get => (NodeFieldList) this[3];
      set => this[3] = (Node) value;
    }

    public NodeFieldList OrderBy
    {
      get => (NodeFieldList) this[4];
      set => this[4] = (Node) value;
    }

    public Node AsOf
    {
      get => this[5];
      set => this[5] = value;
    }

    public NodeMode Mode
    {
      get => (NodeMode) this[6];
      set => this[6] = (Node) value;
    }

    public override void AppendTo(StringBuilder builder)
    {
      string[] strArray = new string[7]
      {
        "{0}",
        " from {0}",
        " where {0}",
        "group by {0}",
        " order by {0}",
        " asof {0}",
        " mode ({0})"
      };
      builder.Append("select ");
      for (int i = 0; i < strArray.Length; ++i)
      {
        Node node = this[i];
        if (node == this.Where && node is NodeBoolConst && ((NodeBoolConst) node).Value)
          node = (Node) null;
        if (node != null)
        {
          string str = node.ToString();
          if (!string.IsNullOrEmpty(str))
            builder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, strArray[i], (object) str);
        }
        else if (i == 0)
          builder.Append("*");
      }
    }

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      Tools.EnsureSyntax(this.From != null, SyntaxError.FromIsNotSpecified, (Node) null);
      this.From.Bind(external, (NodeTableName) null, (NodeFieldName) null);
      Tools.EnsureSyntax(this.From.NodeType == NodeType.TableName, SyntaxError.ExpectingTableName, (Node) this.From);
      if (this.Mode != null)
        this.Mode.Bind(external, this.From, (NodeFieldName) null);
      if (this.Fields != null)
      {
        this.Fields.Bind(external, this.From, (NodeFieldName) null);
        Tools.EnsureSyntax(this.Fields.NodeType == NodeType.FieldList, SyntaxError.ExpectingFieldList, (Node) this.Fields);
      }
      if (this.GroupBy != null)
      {
        this.GroupBy.Bind(external, this.From, (NodeFieldName) null);
        Tools.EnsureSyntax(this.GroupBy.NodeType == NodeType.GroupFieldList, SyntaxError.ExpectingFieldList, (Node) this.GroupBy);
      }
      if (this.OrderBy != null)
      {
        this.OrderBy.Bind(external, this.From, (NodeFieldName) null);
        Tools.EnsureSyntax(this.OrderBy.NodeType == NodeType.OrderFieldList, SyntaxError.ExpectingFieldList, (Node) this.OrderBy);
      }
      if (this.Where != null)
      {
        try
        {
          this.Where.Bind(external, this.From, (NodeFieldName) null);
          Tools.EnsureSyntax(this.Where.DataType == DataType.Bool, SyntaxError.ExpectingCondition, this.Where);
        }
        catch (SyntaxException ex)
        {
          if (ex.SyntaxError == SyntaxError.FieldDoesNotExistInTheTable && this.From.Tag is LinkQueryMode && (LinkQueryMode) this.From.Tag > LinkQueryMode.WorkItems && ex.Node != null && !ex.Node.ToString().StartsWith("Source", StringComparison.OrdinalIgnoreCase) && !ex.Node.ToString().StartsWith("Target", StringComparison.OrdinalIgnoreCase))
            throw new SyntaxException(ex.Node, SyntaxError.InvalidFieldPrefixInLinkQueries);
          throw;
        }
      }
      if (this.AsOf != null)
      {
        this.AsOf.Bind(external, this.From, (NodeFieldName) null);
        if (external != null)
          Tools.EnsureSyntax(this.AsOf.IsScalar && this.AsOf.CanCastTo(DataType.Date, external.CultureInfo), SyntaxError.ExpectingDate, this.AsOf);
      }
      base.Bind(external, tableContext, fieldContext);
    }

    public override Node Optimize(
      IExternal e,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      if (this.Where != null)
        this.Where = this.Where.Optimize(e, this.From, (NodeFieldName) null);
      if (this.AsOf != null)
        this.AsOf = this.AsOf.Optimize(e, (NodeTableName) null, (NodeFieldName) null);
      return base.Optimize(e, tableContext, fieldContext);
    }

    public Dictionary<string, NodeAndOperator> GetWhereGroups()
    {
      Dictionary<string, NodeAndOperator> whereGroups = new Dictionary<string, NodeAndOperator>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Node where = this.Where;
      if (where is NodeAndOperator)
      {
        NodeAndOperator nodeAndOperator1 = (NodeAndOperator) where;
        for (int i = 0; i < nodeAndOperator1.Count; ++i)
        {
          Node node = nodeAndOperator1[i];
          string prefix = (string) null;
          node.CheckPrefix(ref prefix);
          if (prefix != null)
          {
            NodeAndOperator nodeAndOperator2;
            if (whereGroups.TryGetValue(prefix, out nodeAndOperator2))
            {
              nodeAndOperator2.Add(node);
            }
            else
            {
              nodeAndOperator2 = new NodeAndOperator();
              nodeAndOperator2.Add(node);
              whereGroups.Add(prefix, nodeAndOperator2);
            }
          }
        }
      }
      else if (where != null)
      {
        string prefix = (string) null;
        where.CheckPrefix(ref prefix);
        if (prefix != null)
        {
          NodeAndOperator nodeAndOperator = new NodeAndOperator();
          nodeAndOperator.Add(where);
          whereGroups.Add(prefix, nodeAndOperator);
        }
      }
      string[] array = new string[whereGroups.Count];
      whereGroups.Keys.CopyTo(array, 0);
      foreach (string key in array)
      {
        NodeAndOperator nodeAndOperator = whereGroups[key];
        if (nodeAndOperator.Count == 1 && nodeAndOperator[0] is NodeAndOperator)
          whereGroups[key] = (NodeAndOperator) nodeAndOperator[0];
      }
      return whereGroups;
    }
  }
}
