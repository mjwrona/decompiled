// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialActivityAggregatedMetricBinder
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialActivityAggregatedMetricBinder : ObjectBinder<SocialActivityAggregatedMetric>
  {
    private SqlColumnBinder providerIdColumn = new SqlColumnBinder("ProviderId");
    private SqlColumnBinder artifactTypeColumn = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder artifactIdColumn = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder aggregateDateTimeColumn = new SqlColumnBinder("AggregateDateTime");
    private SqlColumnBinder metaDataColumn = new SqlColumnBinder("MetaData");

    protected override SocialActivityAggregatedMetric Bind() => new SocialActivityAggregatedMetric()
    {
      ProviderId = this.providerIdColumn.GetGuid((IDataReader) this.Reader, false),
      ArtifactType = this.artifactTypeColumn.GetByte((IDataReader) this.Reader),
      ArtifactId = this.artifactIdColumn.GetString((IDataReader) this.Reader, false),
      AggregateDateTime = this.aggregateDateTimeColumn.GetDateTime((IDataReader) this.Reader),
      MetaData = this.metaDataColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
