// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class Node : IEnumerable
  {
    private NodeType m_nodeType;
    private int m_startOffset = -1;
    private int m_endOffset = -1;

    protected Node(NodeType type) => this.m_nodeType = type;

    public NodeType NodeType => this.m_nodeType;

    public bool HasParentheses { get; set; }

    public abstract DataType DataType { get; }

    public abstract bool IsConst { get; }

    public abstract Priority Priority { get; }

    public abstract bool IsScalar { get; }

    public abstract int Count { get; }

    public abstract Node this[int i] { get; set; }

    public abstract void AppendTo(StringBuilder builder);

    public virtual bool CanCastTo(DataType type, CultureInfo culture) => this.DataType == type;

    public virtual string ConstStringValue => (string) null;

    public virtual void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      external?.VerifyNode(this, tableContext, fieldContext);
    }

    public virtual Node Optimize(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      return external == null ? this : external.OptimizeNode(this, tableContext, fieldContext);
    }

    public virtual void CheckPrefix(ref string prefix)
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        this[i]?.CheckPrefix(ref prefix);
    }

    public virtual void SetPrefix(string prefix)
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        this[i].SetPrefix(prefix);
    }

    public int StartOffset
    {
      get => this.m_startOffset;
      set => this.m_startOffset = value;
    }

    public int EndOffset
    {
      get => this.m_endOffset;
      set => this.m_endOffset = value;
    }

    public Node FindNodeByOffset(int offset)
    {
      if (offset >= 0 && (this.m_startOffset == -1 || offset >= this.m_startOffset && offset < this.m_endOffset))
      {
        int count = this.Count;
        for (int i = 0; i < count; ++i)
        {
          if (this[i] != null)
          {
            Node nodeByOffset = this[i].FindNodeByOffset(offset);
            if (nodeByOffset != null)
              return nodeByOffset;
          }
        }
        if (this.m_startOffset != -1)
          return this;
      }
      return (Node) null;
    }

    public void Walk(Action<Node> visit)
    {
      Queue<Node> source = new Queue<Node>();
      source.Enqueue(this);
      while (source.Any<Node>())
      {
        Node node1 = source.Dequeue();
        visit(node1);
        foreach (object obj in node1)
        {
          if (obj is Node node2)
            source.Enqueue(node2);
        }
      }
    }

    public override string ToString()
    {
      StringBuilder builder = new StringBuilder();
      this.AppendTo(builder);
      return builder.ToString();
    }

    protected void AppendChildren(StringBuilder builder, string sep)
    {
      int length = builder.Length;
      int count = this.Count;
      for (int i = 0; i < count; ++i)
      {
        Node node = this[i];
        if (builder.Length != length)
          builder.Append(sep);
        if (node != null)
        {
          if (node.Priority >= this.Priority || node.HasParentheses)
          {
            builder.Append("(");
            node.AppendTo(builder);
            builder.Append(")");
          }
          else
            node.AppendTo(builder);
        }
      }
    }

    protected DataType GetChildrenDataType()
    {
      int count = this.Count;
      if (count == 0)
        return DataType.Void;
      DataType dataType = this[0].DataType;
      for (int i = 1; i < count; ++i)
      {
        if (this[i].DataType != dataType)
          return DataType.Unknown;
      }
      return dataType;
    }

    protected bool GetChildrenIsConst()
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
      {
        if (!this[i].IsConst)
          return false;
      }
      return true;
    }

    protected bool GetChildrenCanCastTo(DataType type, CultureInfo culture)
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
      {
        if (!this[i].CanCastTo(type, culture))
          return false;
      }
      return true;
    }

    protected void BindChildren(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        this[i].Bind(external, tableContext, fieldContext);
    }

    protected void OptimizeChildren(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        this[i] = this[i].Optimize(external, tableContext, fieldContext);
    }

    public IEnumerator GetEnumerator() => (IEnumerator) new NodeEnumerator(this);
  }
}
