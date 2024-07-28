// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CodeIndexShardSizeReductionJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class CodeIndexShardSizeReductionJob : AbstractShardSizeReductionJob
  {
    private IDictionary<string, string> m_vcTypeIndexingUnitTyeMap;

    public CodeIndexShardSizeReductionJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance(), Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal CodeIndexShardSizeReductionJob(
      IDataAccessFactory dataAccessFactory,
      ISearchPlatformFactory searchPlatformFactory)
      : base(dataAccessFactory, searchPlatformFactory)
    {
    }

    protected internal override void Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      base.Initialize(requestContext, jobDefinition);
      this.EntityType = (IEntityType) CodeEntityType.GetInstance();
      this.IndicesToBeMonitored = requestContext.GetConfigValue("/Service/ALMSearch/Settings/IndexPatternForShardReduction");
      this.m_lastJobStatus = requestContext.GetConfigValue<MaintenanceJobStatus>("/Service/ALMSearch/Settings/CodeShardReductionState", TeamFoundationHostType.Deployment, MaintenanceJobStatus.Succeeded);
      this.MonitoringMode = this.m_lastJobStatus == MaintenanceJobStatus.InProgress;
      this.m_vcTypeIndexingUnitTyeMap = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "git",
          "Git_Repository"
        },
        {
          "tfvc",
          "TFVC_Repository"
        },
        {
          "custom",
          "CustomRepository"
        }
      };
    }

    protected internal override void UpdateMaintenanceJobStatus(
      IVssRequestContext requestContext,
      MaintenanceJobStatus maintenanceJobStatus)
    {
      this.m_lastJobStatus = maintenanceJobStatus;
      requestContext.SetConfigValue<MaintenanceJobStatus>("/Service/ALMSearch/Settings/CodeShardReductionState", this.m_lastJobStatus);
    }

    [Info("InternalForTestPurpose")]
    internal override Guid GetCollectionToBeMigratedFromTheShard(
      IVssRequestContext requestContext,
      string indexName,
      DocumentContractType contractType,
      string routingId)
    {
      string key = this.SearchClusterStateService.GetFieldWiseDocumentCount(this.ExecutionContext, indexName, contractType, "repositoryId", routingId).First<KeyValuePair<string, long>>().Key;
      IDictionary<string, string> fieldValues = this.SearchClusterStateService.GetFieldValues("repositoryId", key, new string[2]
      {
        "collectionId",
        "vcType"
      }, indexName, contractType);
      Guid guid = new Guid(fieldValues["collectionId"]);
      return !this.VerifyCollectionRepo(requestContext, fieldValues, key) ? Guid.Empty : guid;
    }

    [Info("InternalForTestPurpose")]
    internal bool VerifyCollectionRepo(
      IVssRequestContext requestContext,
      IDictionary<string, string> metaData,
      string repoId)
    {
      string g = metaData["collectionId"];
      string typeIndexingUnitTye = this.m_vcTypeIndexingUnitTyeMap[metaData["vcType"]];
      ITeamFoundationHostManagementService service = requestContext.GetService<ITeamFoundationHostManagementService>();
      bool flag = false;
      IVssRequestContext requestContext1 = requestContext;
      Guid instanceId = new Guid(g);
      using (IVssRequestContext requestContext2 = service.BeginRequest(requestContext1, instanceId, RequestContextType.SystemContext))
        flag = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(requestContext2, new Guid(repoId), typeIndexingUnitTye, (IEntityType) CodeEntityType.GetInstance()) != null;
      if (!flag)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083041, "Indexing Pipeline", "IndexingOperation", "Collection Id ,repo mismatch, inconsistent indexing");
      return flag;
    }
  }
}
