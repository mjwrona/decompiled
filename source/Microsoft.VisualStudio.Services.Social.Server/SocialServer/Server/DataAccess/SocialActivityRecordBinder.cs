// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialActivityRecordBinder
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialActivityRecordBinder : ObjectBinder<SocialActivityRecord>
  {
    private SqlColumnBinder activityTypeColumn = new SqlColumnBinder("ActivityType");
    private SqlColumnBinder activityIdColumn = new SqlColumnBinder("ActivityId");
    private SqlColumnBinder activityTimeStampColumn = new SqlColumnBinder("ActivityTimeStamp");
    private SqlColumnBinder creationTimeColumn = new SqlColumnBinder("CreationTime");
    private SqlColumnBinder userIdColumn = new SqlColumnBinder("UserId");
    private SqlColumnBinder dataColumn = new SqlColumnBinder("Data");
    private SqlColumnBinder extendedDataColumn = new SqlColumnBinder("ExtendedData");

    protected override SocialActivityRecord Bind() => new SocialActivityRecord()
    {
      ActivityType = this.activityTypeColumn.GetString((IDataReader) this.Reader, false),
      ActivityId = this.activityIdColumn.GetGuid((IDataReader) this.Reader, false),
      ActivityTimeStamp = this.activityTimeStampColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue),
      CreationTime = new DateTime?(this.creationTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue)),
      UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader),
      Data = this.dataColumn.GetString((IDataReader) this.Reader, true),
      ExtendedData = this.extendedDataColumn.ColumnExists((IDataReader) this.Reader) ? this.extendedDataColumn.GetString((IDataReader) this.Reader, true) : (string) null
    };
  }
}
