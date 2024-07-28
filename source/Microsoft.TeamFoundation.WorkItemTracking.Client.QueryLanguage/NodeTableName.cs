// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeTableName
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeTableName : NodeItem
  {
    private object m_tag;

    public NodeTableName(string s)
      : base(NodeType.TableName, s)
    {
    }

    public NodeTableName(NodeName n)
      : base(NodeType.TableName, n.Value)
    {
      this.StartOffset = n.StartOffset;
      this.EndOffset = n.EndOffset;
    }

    public object Tag
    {
      get => this.m_tag;
      set => this.m_tag = value;
    }

    public override DataType DataType => DataType.Table;

    public override bool IsConst => true;

    public override void AppendTo(StringBuilder builder) => Tools.AppendName(builder, this.Value);

    public override void Bind(
      IExternal external,
      NodeTableName tableContext,
      NodeFieldName fieldContext)
    {
      if (external != null)
      {
        this.m_tag = external.FindTable(this.Value);
        Tools.EnsureSyntax(this.m_tag != null, SyntaxError.TableDoesNotExist, (Node) this);
      }
      base.Bind(external, tableContext, fieldContext);
    }
  }
}
