// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.AggregatedArtifactsRecordBinder
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class AggregatedArtifactsRecordBinder : ObjectBinder<AggregatedArtifactsRecord>
  {
    private SqlColumnBinder providerIdColumn = new SqlColumnBinder("ProviderId");
    private SqlColumnBinder artifactTypeColumn = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder artifactsColumn = new SqlColumnBinder("Artifacts");
    private SqlColumnBinder lastUpdatedTimeColumn = new SqlColumnBinder("LastUpdatedTime");

    protected override AggregatedArtifactsRecord Bind() => new AggregatedArtifactsRecord()
    {
      ProviderId = this.providerIdColumn.GetGuid((IDataReader) this.Reader, false),
      ArtifactType = this.artifactTypeColumn.GetByte((IDataReader) this.Reader),
      Artifacts = this.artifactsColumn.GetString((IDataReader) this.Reader, false),
      LastUpdatedTime = new DateTime?(this.lastUpdatedTimeColumn.GetDateTime((IDataReader) this.Reader))
    };
  }
}
