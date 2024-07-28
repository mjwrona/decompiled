// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDataProviderSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseDataProviderSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ReleaseDataProviderSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseDataProviderSqlComponent2>(2)
    }, "ReleaseManagementDataProviders", "ReleaseManagement");

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Has to be a method")]
    protected virtual ReleaseEnvironmentStepBinder GetReleaseEnvironmentStepBinder() => new ReleaseEnvironmentStepBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Has to be a method")]
    protected virtual ReleaseDefinitionBinder GetReleaseDefinitionBinder() => (ReleaseDefinitionBinder) new ReleaseDefinitionBinder5((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Has to be a method")]
    protected virtual DeploymentApiBinder GetDeploymentApiBinder() => (DeploymentApiBinder) new DeploymentApiBinder3((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Has to be a method")]
    protected virtual ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder6((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Override of base class method.")]
    protected virtual ReleaseDefinitionShallowReferenceBinder GetReleaseDefinitionShallowReferenceBinder() => new ReleaseDefinitionShallowReferenceBinder((ReleaseManagementSqlResourceComponentBase) this);

    public virtual ActiveDefinitionsData GetLandingPageDataWithPendingApprovals(
      Guid projectId,
      Guid userId,
      IList<int> favoriteDefinitions,
      int currentReleaseDefinitionId,
      int maxApprovalCount,
      int maxDeploymentCount,
      int activeDefinitionsCount,
      int recentDefinitionsCount,
      DateTime minModifiedTimePendingApproval,
      DateTime maxModifiedTimePendingApproval,
      DateTime minModifiedTimeCompletedApproval,
      DateTime maxModifiedTimeCompletedApproval,
      DateTime minDeploymentQueueTime,
      DateTime maxDeploymentQueueTime)
    {
      this.PrepareStoredProcedure("Release.prc_GetActiveDefinitionDataForIdentity", projectId);
      this.BindGuid(nameof (userId), userId);
      this.BindInt32Table(nameof (favoriteDefinitions), (IEnumerable<int>) favoriteDefinitions);
      this.BindInt(nameof (currentReleaseDefinitionId), currentReleaseDefinitionId);
      this.BindInt(nameof (maxApprovalCount), maxApprovalCount);
      this.BindInt(nameof (maxDeploymentCount), maxDeploymentCount);
      this.BindInt(nameof (activeDefinitionsCount), activeDefinitionsCount);
      this.BindInt(nameof (recentDefinitionsCount), recentDefinitionsCount);
      this.BindDateTime("minModifiedTimePendingApprovals", minModifiedTimePendingApproval);
      this.BindDateTime("maxModifiedTimePendingApprovals", maxModifiedTimePendingApproval);
      this.BindDateTime("minModifiedTimeCompletedApprovals", minModifiedTimeCompletedApproval);
      this.BindDateTime("maxModifiedTimeCompletedApprovals", maxModifiedTimeCompletedApproval);
      this.BindDateTime(nameof (minDeploymentQueueTime), minDeploymentQueueTime);
      this.BindDateTime(nameof (maxDeploymentQueueTime), maxDeploymentQueueTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => definitionId.GetInt32(reader))));
        SqlColumnBinder definitionId2 = new SqlColumnBinder("DefinitionId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => definitionId2.GetInt32(reader))));
        SqlColumnBinder definitionId3 = new SqlColumnBinder("DefinitionId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => definitionId3.GetInt32(reader))));
        SqlColumnBinder definitionId4 = new SqlColumnBinder("DefinitionId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => definitionId4.GetInt32(reader))));
        SqlColumnBinder definitionId5 = new SqlColumnBinder("DefinitionId");
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => definitionId5.GetInt32(reader))));
        resultCollection.AddBinder<ReleaseDefinition>((ObjectBinder<ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentApiBinder());
        List<int> items1 = resultCollection.GetCurrent<int>().Items;
        resultCollection.NextResult();
        List<int> items2 = resultCollection.GetCurrent<int>().Items;
        resultCollection.NextResult();
        List<int> items3 = resultCollection.GetCurrent<int>().Items;
        resultCollection.NextResult();
        List<int> items4 = resultCollection.GetCurrent<int>().Items;
        resultCollection.NextResult();
        List<int> items5 = resultCollection.GetCurrent<int>().Items;
        resultCollection.NextResult();
        List<ReleaseDefinition> items6 = resultCollection.GetCurrent<ReleaseDefinition>().Items;
        resultCollection.NextResult();
        List<ReleaseEnvironmentStep> items7 = resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
        resultCollection.NextResult();
        List<Deployment> items8 = resultCollection.GetCurrent<Deployment>().Items;
        return new ActiveDefinitionsData()
        {
          ActiveDefinitionIds = items1,
          RecentDefinitionIds = items2,
          DefinitionsWithApprovalPendingOnUser = items3,
          DefinitionsWithDeploymentRequestedByUser = items4,
          DefinitionsWithApprovalCompletedByUser = items5,
          Definitions = items6,
          PendingApprovals = items7,
          Deployments = items8
        };
      }
    }

    public virtual AllPipelinesViewData GetAllPipelinesViewData(
      Guid projectId,
      string queriedFolderPath)
    {
      return new AllPipelinesViewData();
    }
  }
}
