// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialEngagementRecordBinder
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialEngagementRecordBinder : ObjectBinder<SocialEngagementRecord>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder artifactTypeColumn = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder artifactIdColumn = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder socialEngagementTypeColumn = new SqlColumnBinder("SocialEngagementType");
    private SqlColumnBinder creationDateColumn = new SqlColumnBinder("CreationDate");
    private SqlColumnBinder impressionCountColumn = new SqlColumnBinder("ImpressionCount");

    protected override SocialEngagementRecord Bind()
    {
      SocialEngagementRecord engagementRecord = new SocialEngagementRecord()
      {
        SocialEngagementId = this.idColumn.GetGuid((IDataReader) this.Reader, true),
        ArtifactType = this.artifactTypeColumn.GetString((IDataReader) this.Reader, true),
        ArtifactId = this.artifactIdColumn.GetString((IDataReader) this.Reader, true),
        EngagementType = (SocialEngagementType) this.socialEngagementTypeColumn.GetByte((IDataReader) this.Reader),
        SocialEngagementStatistics = new SocialEngagementStatistics()
        {
          UserCount = this.impressionCountColumn.GetInt32((IDataReader) this.Reader)
        },
        CreationDate = new DateTime?(this.creationDateColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue))
      };
      if (engagementRecord.CreationDate.Equals((object) DateTime.MinValue))
        engagementRecord.CreationDate = new DateTime?();
      return engagementRecord;
    }
  }
}
