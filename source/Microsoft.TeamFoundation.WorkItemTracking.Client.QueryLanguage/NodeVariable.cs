// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeVariable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeVariable : NodeItem
  {
    private DataType m_type;
    public bool m_doesMacroExtensionHandleOffset;
    private object m_tag;
    private NodeParameters m_parameters;

    public NodeVariable(string s)
      : base(NodeType.Variable, s)
    {
      this.m_parameters = new NodeParameters();
    }

    public NodeParameters Parameters => this.m_parameters;

    public object Tag => this.m_tag;

    public override DataType DataType => this.m_type;

    public bool DoesMacroExtensionHandleOffset => this.m_doesMacroExtensionHandleOffset;

    public override bool IsConst => true;

    public override void AppendTo(StringBuilder builder)
    {
      builder.AppendFormat("@{0}", (object) this.Value);
      IList<NodeItem> arguments = this.Parameters.Arguments;
      if (arguments.Count > 0)
      {
        builder.Append("(");
        for (int index = 0; index < arguments.Count; ++index)
        {
          arguments[index].AppendTo(builder);
          if (index + 1 < arguments.Count)
            builder.Append(", ");
        }
        builder.Append(")");
      }
      if (this.Parameters.Offset > 0.0)
      {
        builder.AppendFormat(" + {0}", (object) this.Parameters.Offset);
      }
      else
      {
        if (this.Parameters.Offset >= 0.0)
          return;
        builder.AppendFormat(" - {0}", (object) (0.0 - this.Parameters.Offset));
      }
    }

    public object GetVariableValue(IExternal external, IDictionary context)
    {
      if (this.m_tag == null || !context.Contains(this.m_tag))
      {
        this.m_tag = external.FindVariable(this.Value, this.Parameters);
        if (this.m_tag == null)
          return (object) null;
      }
      return context[this.m_tag];
    }

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      if (external != null)
      {
        this.m_type = external.GetVariableDataType(this.Value);
        this.m_doesMacroExtensionHandleOffset = external.DoesMacroExtensionHandleOffset(this.Value);
        if (this.m_type == DataType.Unknown)
        {
          if (string.Equals("project", this.Value, StringComparison.OrdinalIgnoreCase))
            throw new SyntaxException(ResourceStrings.Get("ProjectMacroUndefined"));
          throw new SyntaxException(ResourceStrings.Format("MacroUndefined", (object) this.Value));
        }
        external.ValidateParameters(this.Value, tableContext, fieldContext, this.Parameters);
      }
      base.Bind(external, tableContext, fieldContext);
    }
  }
}
