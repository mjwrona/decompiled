// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActivityLogParameterColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ActivityLogParameterColumns : ObjectBinder<ActivityLogParameter>
  {
    private SqlColumnBinder nameColumn = new SqlColumnBinder("ParameterName");
    private SqlColumnBinder valueColumn = new SqlColumnBinder("ParameterValue");
    private SqlColumnBinder indexColumn = new SqlColumnBinder("ParameterIndex");
    private IVssRequestContext m_requestContext;
    private static readonly string s_area = "CommandComponent";
    private static readonly string s_layer = "ObjectBinder";

    public ActivityLogParameterColumns(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(70345, ActivityLogParameterColumns.s_area, ActivityLogParameterColumns.s_layer, "ActivityLogParameterColumns.ctor");
      this.m_requestContext = requestContext;
      this.m_requestContext.TraceLeave(70350, ActivityLogParameterColumns.s_area, ActivityLogParameterColumns.s_layer, "ActivityLogParameterColumns.ctor");
    }

    protected override ActivityLogParameter Bind()
    {
      this.m_requestContext.TraceEnter(70355, ActivityLogParameterColumns.s_area, ActivityLogParameterColumns.s_layer, nameof (Bind));
      ActivityLogParameter activityLogParameter = new ActivityLogParameter();
      activityLogParameter.Name = this.nameColumn.GetString((IDataReader) this.Reader, false);
      activityLogParameter.Value = this.valueColumn.GetString((IDataReader) this.Reader, false);
      activityLogParameter.Index = this.indexColumn.GetInt32((IDataReader) this.Reader);
      this.m_requestContext.TraceLeave(70360, ActivityLogParameterColumns.s_area, ActivityLogParameterColumns.s_layer, nameof (Bind));
      return activityLogParameter;
    }
  }
}
