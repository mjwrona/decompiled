// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryComparisonExpressionNode
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryComparisonExpressionNode : QueryExpressionNode
  {
    private FieldEntry[] m_fields;

    public QueryExpressionOperator Operator { get; set; }

    public QueryExpressionValue Value { get; set; }

    public bool ExpandConstant { get; set; }

    public FieldEntry Field
    {
      get => this.Fields.FirstOrDefault<FieldEntry>();
      set
      {
        if (this.m_fields != null && this.m_fields.Length != 0)
          this.m_fields[0] = value;
        else
          this.m_fields = new FieldEntry[1]{ value };
      }
    }

    public IEnumerable<FieldEntry> Fields => (IEnumerable<FieldEntry>) this.m_fields ?? Enumerable.Empty<FieldEntry>();

    public ResolvedIdentityNamesInfo IdentityNamesInfo { get; internal set; }

    internal void SetFields(FieldEntry[] fields) => this.m_fields = fields;
  }
}
