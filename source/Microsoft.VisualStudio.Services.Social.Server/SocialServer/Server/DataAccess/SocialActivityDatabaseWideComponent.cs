// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialActivityDatabaseWideComponent
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialActivityDatabaseWideComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<SocialActivityDatabaseWideComponent>(1350)
    }, "SocialActivityDatabaseWide");

    public SocialActivityDatabaseWideComponent() => this.ContainerErrorCode = 50000;

    internal IEnumerable<PartitionRecord> DeleteSocialActivityRecords(int dataAge)
    {
      if (this.Version < 1350)
        return Enumerable.Empty<PartitionRecord>();
      this.PrepareStoredProcedure("Social.prc_DeleteSocialActivity2");
      this.BindInt("@dataAge", dataAge);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PartitionRecord>((ObjectBinder<PartitionRecord>) new PartitionRecordBinder());
        return (IEnumerable<PartitionRecord>) resultCollection.GetCurrent<PartitionRecord>().Items;
      }
    }

    internal DateTime CreateNewSocialActivityPartition(int minPartitionSpanInSec)
    {
      if (this.Version < 1350)
        return DateTime.MinValue;
      this.PrepareStoredProcedure("Social.prc_CreateNewSocialActivityPartition");
      this.BindInt("@minPartitionSpanInSec", minPartitionSpanInSec);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder createdPartitionBoundary = new SqlColumnBinder("CreatedPartitionBoundary");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => createdPartitionBoundary.GetDateTime(reader, DateTime.MinValue))));
        return resultCollection.GetCurrent<DateTime>().FirstOrDefault<DateTime>();
      }
    }
  }
}
