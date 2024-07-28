// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.Traditional.Binders.ColorConfigurationBinder
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.Traditional.Binders
{
  internal class ColorConfigurationBinder : ObjectBinder<ColorConfiguration>
  {
    internal SqlColumnBinder chartId = new SqlColumnBinder("ChartId");
    internal SqlColumnBinder value = new SqlColumnBinder("Value");
    internal SqlColumnBinder backgroundColor = new SqlColumnBinder("BackgroundColor");

    protected override ColorConfiguration Bind() => new ColorConfiguration()
    {
      ChartId = new Guid?(this.chartId.GetGuid((IDataReader) this.Reader)),
      Value = this.value.GetString((IDataReader) this.Reader, false),
      BackgroundColor = this.backgroundColor.GetString((IDataReader) this.Reader, false)
    };
  }
}
