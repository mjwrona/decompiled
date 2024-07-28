// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.NodeString
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class NodeString : NodeItem
  {
    public NodeString(string s)
      : base(NodeType.String, s)
    {
    }

    public override DataType DataType => DataType.String;

    public override bool IsConst => true;

    public override string ConstStringValue => this.Value;

    public override bool CanCastTo(DataType type, CultureInfo culture)
    {
      switch (type)
      {
        case DataType.Bool:
          return this.Value.Length == 0 || Tools.IsBoolString(this.Value);
        case DataType.Numeric:
          return this.Value.Length == 0 || Tools.IsNumericString(this.Value);
        case DataType.Date:
          return this.Value.Length == 0 || Tools.IsDateString(this.Value, culture);
        case DataType.String:
          return true;
        case DataType.Guid:
          return this.Value.Length == 0 || Tools.IsGuidString(this.Value);
        default:
          return false;
      }
    }

    public override void AppendTo(StringBuilder builder) => Tools.AppendString(builder, this.Value);
  }
}
