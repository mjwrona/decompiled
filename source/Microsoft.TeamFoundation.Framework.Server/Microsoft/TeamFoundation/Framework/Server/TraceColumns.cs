// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TraceColumns : ObjectBinder<Microsoft.VisualStudio.Services.WebApi.TraceFilter>
  {
    private SqlColumnBinder m_traceIdColumn = new SqlColumnBinder("TraceId");
    private SqlColumnBinder m_enabledColumn = new SqlColumnBinder("IsEnabled");
    private SqlColumnBinder m_processNameColumn = new SqlColumnBinder("ProcessName");
    private SqlColumnBinder m_userLoginColumn = new SqlColumnBinder("UserLogin");
    private SqlColumnBinder m_serviceColumn = new SqlColumnBinder("Service");
    private SqlColumnBinder m_methodColumn = new SqlColumnBinder("Method");
    private SqlColumnBinder m_levelColumn = new SqlColumnBinder("Level");
    private SqlColumnBinder m_areaColumn = new SqlColumnBinder("Area");
    private SqlColumnBinder m_userAgentColumn = new SqlColumnBinder("UserAgent");
    private SqlColumnBinder m_layerColumn = new SqlColumnBinder("Layer");
    private SqlColumnBinder m_serviceHostColumn = new SqlColumnBinder("ServiceHost");
    private SqlColumnBinder m_userDefinedColumn = new SqlColumnBinder("UserDefined");
    private SqlColumnBinder m_urlColumn = new SqlColumnBinder("Uri");
    private SqlColumnBinder m_pathColumn = new SqlColumnBinder("Path");
    private SqlColumnBinder m_tracePointColumn = new SqlColumnBinder("TracePoint");
    private SqlColumnBinder m_ownerIdColumn = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder m_timeCreatedColumn = new SqlColumnBinder("TimeCreated");

    protected override Microsoft.VisualStudio.Services.WebApi.TraceFilter Bind()
    {
      Microsoft.VisualStudio.Services.WebApi.TraceFilter traceFilter = new Microsoft.VisualStudio.Services.WebApi.TraceFilter();
      traceFilter.TraceId = this.m_traceIdColumn.GetGuid((IDataReader) this.Reader);
      traceFilter.IsEnabled = this.m_enabledColumn.GetBoolean((IDataReader) this.Reader);
      traceFilter.ProcessName = this.m_processNameColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.UserLogin = this.m_userLoginColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.Service = this.m_serviceColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.Method = this.m_methodColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.Level = (TraceLevel) this.m_levelColumn.GetByte((IDataReader) this.Reader, (byte) 0);
      traceFilter.Area = this.m_areaColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.UserAgent = this.m_userAgentColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.Layer = this.m_layerColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.ServiceHost = this.m_serviceHostColumn.GetGuid((IDataReader) this.Reader, false);
      traceFilter.Tracepoint = this.m_tracePointColumn.GetInt32((IDataReader) this.Reader, 0);
      traceFilter.Uri = this.m_urlColumn.GetString((IDataReader) this.Reader, true);
      traceFilter.Path = this.m_pathColumn.GetString((IDataReader) this.Reader, true);
      string str = this.m_userDefinedColumn.GetString((IDataReader) this.Reader, true);
      if (string.IsNullOrEmpty(str))
      {
        traceFilter.Tags = (string[]) null;
      }
      else
      {
        traceFilter.Tags = str.Split(new char[2]{ ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
        Array.Sort<string>(traceFilter.Tags);
      }
      traceFilter.OwnerId = this.m_ownerIdColumn.GetGuid((IDataReader) this.Reader, false, Guid.Empty);
      traceFilter.TimeCreated = this.m_timeCreatedColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue);
      return traceFilter;
    }
  }
}
