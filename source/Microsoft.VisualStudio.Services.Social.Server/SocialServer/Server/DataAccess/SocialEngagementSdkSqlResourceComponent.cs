// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess.SocialEngagementSdkSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Social.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6878458A-724A-4C44-954E-B2170F10219E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Social.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.SocialEngagement.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SocialServer.Server.DataAccess
{
  internal class SocialEngagementSdkSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<SocialEngagementSdkSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<SocialEngagementSdkSqlResourceComponent>(1430)
    }, "SocialEngagementSdkService");

    public SocialEngagementSdkSqlResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public int DeleteOldAggregatedSocialEngagementMetrics(
      int hoursToRetain,
      SocialEngagementType engagementType,
      string artifactType)
    {
      this.PrepareStoredProcedure("Social.prc_DeleteSocialEngagementAggregateMetric");
      this.BindInt("@hoursToRetain", hoursToRetain);
      this.BindInt("@socialEngagementType", (int) (byte) engagementType);
      this.BindString("@artifactType", artifactType, 256, true, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder deletedRecordCount = new SqlColumnBinder("DeletedRecordsCount");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => deletedRecordCount.GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    public IList<Guid> GetUsersForSocialEngagementRecord(
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      int top,
      int skip)
    {
      this.PrepareStoredProcedure("Social.prc_GetUsersForSocialEngagement");
      this.BindNullableInt("@dataSpaceId", this.IdentifyDataspace(socialEngagementCreateParameter.ArtifactScope));
      this.BindString("@artifactType", socialEngagementCreateParameter.ArtifactType, 256, true, SqlDbType.NVarChar);
      this.BindString("@artifactId", socialEngagementCreateParameter.ArtifactId, 256, true, SqlDbType.NVarChar);
      this.BindByte("@socialEngagementType", (byte) socialEngagementCreateParameter.EngagementType);
      this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder ownerIdentity = new SqlColumnBinder("OwnerIdentity");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => ownerIdentity.GetGuid(reader))));
        return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }

    public SocialEngagementRecord CreateSocialEngagementRecord(
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      Guid ownerId,
      AggregationType aggregationType)
    {
      this.PrepareStoredProcedure("Social.prc_CreateSocialEngagement");
      this.BindNullableInt("@dataSpaceId", this.IdentifyDataspace(socialEngagementCreateParameter.ArtifactScope));
      this.BindGuid("@ownerIdentity", ownerId);
      this.BindString("@artifactType", socialEngagementCreateParameter.ArtifactType, 256, true, SqlDbType.NVarChar);
      this.BindString("@artifactId", socialEngagementCreateParameter.ArtifactId, 256, true, SqlDbType.NVarChar);
      this.BindByte("@socialEngagementType", (byte) socialEngagementCreateParameter.EngagementType);
      this.BindBoolean("@isProcessed", false);
      this.BindByte("@aggregationType", (byte) aggregationType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialEngagementRecord>((ObjectBinder<SocialEngagementRecord>) new SocialEngagementRecordBinder());
        return resultCollection.GetCurrent<SocialEngagementRecord>().FirstOrDefault<SocialEngagementRecord>();
      }
    }

    public SocialEngagementRecord GetSocialEngagementRecord(
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      Guid ownerId)
    {
      this.PrepareStoredProcedure("Social.prc_GetSocialEngagement");
      this.BindNullableInt("@dataSpaceId", this.IdentifyDataspace(socialEngagementCreateParameter.ArtifactScope));
      this.BindGuid("@ownerIdentity", ownerId);
      this.BindString("@artifactType", socialEngagementCreateParameter.ArtifactType, 256, true, SqlDbType.NVarChar);
      this.BindString("@artifactId", socialEngagementCreateParameter.ArtifactId, 256, true, SqlDbType.NVarChar);
      this.BindByte("@socialEngagementType", (byte) socialEngagementCreateParameter.EngagementType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialEngagementRecord>((ObjectBinder<SocialEngagementRecord>) new SocialEngagementRecordBinder());
        return resultCollection.GetCurrent<SocialEngagementRecord>().FirstOrDefault<SocialEngagementRecord>();
      }
    }

    public SocialEngagementAggregateMetric GetGetSocialEngagementAggregateMetric(
      SocialEngagementCreateParameter socialEngagementCreateParameter)
    {
      this.PrepareStoredProcedure("Social.prc_GetSocialEngagementAggregateMetric");
      this.BindNullableInt("@dataSpaceId", this.IdentifyDataspace(socialEngagementCreateParameter.ArtifactScope));
      this.BindString("@artifactType", socialEngagementCreateParameter.ArtifactType, 256, true, SqlDbType.NVarChar);
      this.BindString("@artifactId", socialEngagementCreateParameter.ArtifactId, 256, true, SqlDbType.NVarChar);
      this.BindByte("@socialEngagementType", (byte) socialEngagementCreateParameter.EngagementType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialEngagementAggregateAndImpressionCount>((ObjectBinder<SocialEngagementAggregateAndImpressionCount>) new SocialEngagementAggregateAndImpressionCountBinder());
        List<SocialEngagementAggregateAndImpressionCount> items = resultCollection.GetCurrent<SocialEngagementAggregateAndImpressionCount>().Items;
        return new SocialEngagementAggregateMetric()
        {
          SocialEngagementAggregateData = (IList<SocialEngagementAggregate>) SocialEngagementAggregateAndImpressionCount.GetSocialEngagementAggregateList(items),
          SocialEngagementStatistics = SocialEngagementAggregateAndImpressionCount.GetSocialEngagementStatistics(items)
        };
      }
    }

    public SocialEngagementRecord DeleteSocialEngagement(
      SocialEngagementCreateParameter socialEngagementCreateParameter,
      Guid ownerId,
      AggregationType aggregationType)
    {
      this.PrepareStoredProcedure("Social.prc_DeleteSocialEngagement");
      this.BindNullableInt("@dataSpaceId", this.IdentifyDataspace(socialEngagementCreateParameter.ArtifactScope));
      this.BindGuid("@ownerIdentity", ownerId);
      this.BindString("@artifactType", socialEngagementCreateParameter.ArtifactType, 256, true, SqlDbType.NVarChar);
      this.BindString("@artifactId", socialEngagementCreateParameter.ArtifactId, 256, true, SqlDbType.NVarChar);
      this.BindByte("@socialEngagementType", (byte) socialEngagementCreateParameter.EngagementType);
      this.BindByte("@aggregationType", (byte) aggregationType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialEngagementRecord>((ObjectBinder<SocialEngagementRecord>) new SocialEngagementRecordBinder());
        return resultCollection.GetCurrent<SocialEngagementRecord>().FirstOrDefault<SocialEngagementRecord>();
      }
    }

    public IEnumerable<SocialEngagementRecord> GetSocialEngagementRecords(
      ArtifactScope artifactScope,
      Guid ownerId,
      string artifactType,
      ISet<string> artifactIds,
      IEnumerable<SocialEngagementType> socialEngagementTypes)
    {
      this.EnforceMinimalVersion(1430);
      this.PrepareStoredProcedure("Social.prc_GetSocialEngagementBatch");
      this.BindNullableInt("@dataSpaceId", this.IdentifyDataspace(artifactScope));
      this.BindGuid("@ownerIdentity", ownerId);
      this.BindString("@artifactType", artifactType, 256, true, SqlDbType.NVarChar);
      this.BindStringTable("@artifactIds", (IEnumerable<string>) artifactIds);
      this.BindTinyIntTable("@socialEngagementTypes", socialEngagementTypes.Select<SocialEngagementType, byte>((System.Func<SocialEngagementType, byte>) (set => (byte) set)));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SocialEngagementRecord>((ObjectBinder<SocialEngagementRecord>) new SocialEngagementRecordBinder());
        return (IEnumerable<SocialEngagementRecord>) resultCollection.GetCurrent<SocialEngagementRecord>().Items ?? Enumerable.Empty<SocialEngagementRecord>();
      }
    }

    protected int? IdentifyDataspace(ArtifactScope artifactScope)
    {
      int dataspaceId;
      if (artifactScope.Type == "Project")
      {
        dataspaceId = this.GetDataspaceId(Guid.Parse(artifactScope.Id));
      }
      else
      {
        if (!(artifactScope.Type == "Collection"))
          throw new NotSupportedException();
        dataspaceId = this.GetDataspaceId(Guid.Empty);
      }
      return new int?(dataspaceId);
    }

    private void EnforceMinimalVersion(int version)
    {
      if (this.Version < version)
        throw new ServiceVersionNotSupportedException(SocialEngagementSdkSqlResourceComponent.ComponentFactory.ServiceName, this.Version, version);
    }
  }
}
