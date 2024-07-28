// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatBinder3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatBinder3 : ExtensionDailyStatBinder2
  {
    private SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    private SqlColumnBinder statisticDateColumn = new SqlColumnBinder("StatisticDate");
    private SqlColumnBinder webPageViewsColumn = new SqlColumnBinder("WebPageViews");
    private SqlColumnBinder installCountColumn = new SqlColumnBinder("InstallCount");
    private SqlColumnBinder webDownloadCountColumn = new SqlColumnBinder("WebDownloadCount");
    private SqlColumnBinder uninstallCountColumn = new SqlColumnBinder("UninstallCount");
    private SqlColumnBinder buyCountColumn = new SqlColumnBinder("BuyCount");
    private SqlColumnBinder tryCountColumn = new SqlColumnBinder("TryCount");
    private SqlColumnBinder connectedInstallCountColumn = new SqlColumnBinder("ConnectedInstallCount");
    private SqlColumnBinder connectedBuyCountColumn = new SqlColumnBinder("ConnectedBuyCount");
    private SqlColumnBinder averageRatingColumn = new SqlColumnBinder("AverageRating");

    protected override ExtensionDailyStat Bind() => new ExtensionDailyStat()
    {
      Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
      StatisticDate = this.statisticDateColumn.GetDateTime((IDataReader) this.Reader),
      Counts = new EventCounts()
      {
        WebPageViews = this.webPageViewsColumn.GetInt64((IDataReader) this.Reader, 0L),
        InstallCount = this.installCountColumn.GetInt64((IDataReader) this.Reader, 0L),
        WebDownloadCount = this.webDownloadCountColumn.GetInt64((IDataReader) this.Reader, 0L),
        UninstallCount = this.uninstallCountColumn.GetInt32((IDataReader) this.Reader, 0),
        BuyCount = this.buyCountColumn.GetInt32((IDataReader) this.Reader, 0),
        TryCount = this.tryCountColumn.GetInt32((IDataReader) this.Reader, 0),
        ConnectedInstallCount = this.connectedInstallCountColumn.GetInt32((IDataReader) this.Reader, 0),
        ConnectedBuyCount = this.connectedBuyCountColumn.GetInt32((IDataReader) this.Reader, 0),
        AverageRating = Convert.ToSingle(this.averageRatingColumn.GetDouble((IDataReader) this.Reader, 0.0))
      }
    };
  }
}
