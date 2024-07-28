// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.WidgetBinder
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  internal class WidgetBinder : ObjectBinder<Widget>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dashboardId = new SqlColumnBinder("DashboardId");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder rowNumber = new SqlColumnBinder("RowNumber");
    private SqlColumnBinder columnNumber = new SqlColumnBinder("ColumnNumber");
    private SqlColumnBinder rowLength = new SqlColumnBinder("RowLength");
    private SqlColumnBinder columnLength = new SqlColumnBinder("ColumnLength");
    private SqlColumnBinder widgetTypeId = new SqlColumnBinder("WidgetTypeId");
    private SqlColumnBinder settings = new SqlColumnBinder("Settings");
    private SqlColumnBinder artifactId = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder widgetETag = new SqlColumnBinder("WidgetETag");
    private SqlColumnBinder dashboardETag = new SqlColumnBinder("DashboardETag");

    protected override Widget Bind() => new Widget()
    {
      Id = new Guid?(this.id.GetGuid((IDataReader) this.Reader)),
      Name = this.name.GetString((IDataReader) this.Reader, false),
      Position = new WidgetPosition()
      {
        Row = this.rowNumber.GetInt32((IDataReader) this.Reader),
        Column = this.columnNumber.GetInt32((IDataReader) this.Reader)
      },
      Size = new WidgetSize()
      {
        RowSpan = this.rowLength.GetInt32((IDataReader) this.Reader),
        ColumnSpan = this.columnLength.GetInt32((IDataReader) this.Reader)
      },
      ContributionId = this.widgetTypeId.GetString((IDataReader) this.Reader, false),
      Settings = this.settings.GetString((IDataReader) this.Reader, true),
      ArtifactId = this.artifactId.GetString((IDataReader) this.Reader, true),
      ETag = this.widgetETag.GetString((IDataReader) this.Reader, "")
    };
  }
}
