// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeFieldName
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeFieldName : NodeItem
  {
    private DataType m_type;
    private object m_tag;
    private Direction m_direction;
    private bool? m_nullsFirst;
    private string m_prefix;

    public NodeFieldName(string s)
      : base(NodeType.FieldName, s)
    {
    }

    public NodeFieldName(string prefix, string s)
      : this(s)
    {
      this.m_prefix = prefix;
    }

    public NodeFieldName(NodeName prefix, NodeName n)
      : this(n.Value)
    {
      this.m_prefix = prefix.Value;
      this.StartOffset = n.StartOffset;
      this.EndOffset = n.EndOffset;
      if (prefix.StartOffset < 0 || prefix.StartOffset >= this.StartOffset)
        return;
      this.StartOffset = prefix.StartOffset;
    }

    public NodeFieldName(NodeName n)
      : this(n.Value)
    {
      this.StartOffset = n.StartOffset;
      this.EndOffset = n.EndOffset;
    }

    public object Tag
    {
      get => this.m_tag;
      set => this.m_tag = value;
    }

    public Direction Direction
    {
      get => this.m_direction;
      set => this.m_direction = value;
    }

    public bool? NullsFirst
    {
      get => this.m_nullsFirst;
      set => this.m_nullsFirst = value;
    }

    public string Prefix
    {
      get => this.m_prefix;
      set => this.m_prefix = value;
    }

    public override DataType DataType => this.m_type;

    public override bool IsConst => false;

    public override void AppendTo(StringBuilder builder)
    {
      if (!string.IsNullOrEmpty(this.m_prefix))
      {
        Tools.AppendName(builder, this.m_prefix);
        builder.Append(".");
      }
      Tools.AppendName(builder, this.Value);
      if (this.m_nullsFirst.HasValue)
      {
        builder.Append(" nulls ");
        builder.Append(this.m_nullsFirst.Value ? "first" : "last");
      }
      switch (this.m_direction)
      {
        case Direction.Ascending:
          builder.Append(" asc");
          break;
        case Direction.Descending:
          builder.Append(" desc");
          break;
      }
    }

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      if (external != null)
      {
        string name1 = this.Value;
        object tag = tableContext?.Tag;
        this.m_tag = external.FindField(name1, this.m_prefix, tag);
        if (this.m_tag == null && this.m_prefix == null && tag != null)
        {
          int length = name1.IndexOf('.');
          if (length > 0 && length + 1 < name1.Length)
          {
            string prefix = name1.Substring(0, length);
            string name2 = name1.Substring(length + 1);
            this.m_tag = external.FindField(name2, prefix, tag);
            if (this.m_tag != null)
            {
              this.m_prefix = prefix;
              this.Value = name2;
            }
          }
        }
        Tools.EnsureSyntax(this.m_tag != null, SyntaxError.FieldDoesNotExistInTheTable, (Node) this);
        this.m_type = external.GetFieldDataType(this.m_tag);
        Tools.EnsureSyntax(this.m_type != 0, SyntaxError.UnknownFieldType, (Node) this);
      }
      base.Bind(external, tableContext, fieldContext);
    }

    public override void CheckPrefix(ref string prefix)
    {
      string b = string.IsNullOrEmpty(this.m_prefix) ? string.Empty : this.m_prefix;
      if (prefix == null)
        prefix = b;
      else
        Tools.EnsureSyntax(string.Equals(prefix, b, StringComparison.OrdinalIgnoreCase), SyntaxError.MixedPrefixes, (Node) this);
    }

    public override void SetPrefix(string prefix) => this.m_prefix = prefix;
  }
}
