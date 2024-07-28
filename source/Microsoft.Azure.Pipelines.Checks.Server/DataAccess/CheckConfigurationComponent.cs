// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckConfigurationComponent
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckConfigurationComponent : ChecksSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[8]
    {
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent>(1),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent2>(2),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent3>(3),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent4>(4),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent5>(5),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent6>(6),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent7>(7),
      (IComponentCreator) new ComponentCreator<CheckConfigurationComponent8>(8)
    }, "PolicyAssignment", "PipelinePolicy");

    public CheckConfigurationComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual CheckConfiguration AddCheckConfiguration(
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      ArgumentUtility.CheckForEmptyGuid(checkConfiguration.Type.Id, "checkTypeId", "Pipeline.Checks");
      object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
      string parameterValue = configurationSettings != null ? JsonUtility.ToString(configurationSettings) : string.Empty;
      string scopeString = checkConfiguration.Resource.GetScopeString();
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (AddCheckConfiguration)))
      {
        this.PrepareForAuditingAction("CheckConfiguration.Created", this.GetAuditLogData(checkConfiguration), projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@config"
        });
        this.PrepareStoredProcedure("PipelinePolicy.prc_AddPolicyAssignment");
        this.BindGuid("@typeId", checkConfiguration.Type.Id);
        this.BindString("@scope", scopeString, 256, false, SqlDbType.NVarChar);
        this.BindString("@config", parameterValue, parameterValue.Length, false, SqlDbType.NVarChar);
        this.BindGuid("@createdBy", new Guid(checkConfiguration.CreatedBy.Id));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().FirstOrDefault<CheckConfiguration>();
        }
      }
    }

    public virtual CheckConfiguration AddNewCheckConfigurationVersion(
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      return (CheckConfiguration) null;
    }

    public virtual IList<CheckConfiguration> AddCheckConfigurations(
      IList<CheckConfiguration> checkConfigurations)
    {
      return (IList<CheckConfiguration>) new List<CheckConfiguration>();
    }

    public virtual CheckConfiguration GetCheckConfigurationByIdVersion(
      int id,
      int version,
      bool includeSettings = false)
    {
      return this.GetCheckConfigurationsByIdVersion((IEnumerable<CheckConfigurationRef>) new CheckConfigurationRef[1]
      {
        new CheckConfigurationRef() { Id = id, Version = version }
      }, includeSettings).FirstOrDefault<CheckConfiguration>();
    }

    public virtual List<CheckConfiguration> GetCheckConfigurationsByIdVersion(
      IEnumerable<CheckConfigurationRef> checkParams,
      bool includeSettings = false,
      bool includeDeletedChecks = false)
    {
      return new List<CheckConfiguration>();
    }

    public virtual CheckConfiguration GetCheckConfiguration(int id, bool includeSettings = false) => this.GetCheckConfigurations((IEnumerable<int>) new int[1]
    {
      id
    }, includeSettings).FirstOrDefault<CheckConfiguration>();

    public virtual List<CheckConfiguration> GetCheckConfigurations(
      IEnumerable<int> ids,
      bool includeSettings = false)
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckConfigurations)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyAssignmentById");
        this.BindUniqueInt32Table("@assignmentIds", ids);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().Items;
        }
      }
    }

    public virtual List<CheckConfiguration> GetCheckConfigurationsOnResource(
      Resource resource,
      bool includeSettings = false)
    {
      return this.GetCheckConfigurationsOnResources(new List<Resource>()
      {
        resource
      }, true, includeSettings);
    }

    public virtual List<CheckConfiguration> GetCheckConfigurationsOnResources(
      List<Resource> resources,
      bool includeDisabledChecks,
      bool includeSettings = false,
      bool includeDeletedChecks = false)
    {
      IEnumerable<string> rows = resources.Select<Resource, string>((System.Func<Resource, string>) (resource => resource.GetScopeString()));
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (GetCheckConfigurationsOnResources)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_GetPolicyAssignmentsByScopes");
        this.BindStringTable("@scopesTable", rows, true, 256);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().Items;
        }
      }
    }

    public virtual void DeleteCheckConfiguration(
      int id,
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (DeleteCheckConfiguration)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_DeletePolicyAssignment");
        this.BindInt("@assignmentId", id);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
      }
    }

    public virtual CheckConfiguration UpdateCheckConfiguration(
      int assignmentId,
      CheckConfiguration checkConfiguration,
      Guid projectId = default (Guid))
    {
      object configurationSettings = checkConfiguration.GetCheckConfigurationSettings();
      string parameterValue = configurationSettings != null ? JsonUtility.ToString(configurationSettings) : string.Empty;
      using (new ChecksSqlComponentBase.SqlMethodScope((ChecksSqlComponentBase) this, method: nameof (UpdateCheckConfiguration)))
      {
        this.PrepareForAuditingAction("CheckConfiguration.Updated", this.GetAuditLogData(checkConfiguration), projectId, excludeParameters: (IEnumerable<string>) new string[1]
        {
          "@config"
        });
        this.PrepareStoredProcedure("PipelinePolicy.prc_UpdatePolicyAssignment");
        this.BindInt("@assignmentId", assignmentId);
        this.BindString("@config", parameterValue, parameterValue.Length, false, SqlDbType.NVarChar);
        this.BindGuid("@modifiedBy", new Guid(checkConfiguration.ModifiedBy.Id));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<CheckConfiguration>((ObjectBinder<CheckConfiguration>) this.GetCheckConfigurationBinder());
          return resultCollection.GetCurrent<CheckConfiguration>().FirstOrDefault<CheckConfiguration>();
        }
      }
    }

    protected Dictionary<string, object> GetAuditLogData(CheckConfiguration checkConfiguration) => new Dictionary<string, object>()
    {
      ["CheckId"] = (object) checkConfiguration.Id,
      ["Type"] = (object) checkConfiguration.Type.Name,
      ["ResourceType"] = (object) checkConfiguration.Resource.Type,
      ["ResourceName"] = (object) checkConfiguration.Resource.Name,
      ["ResourceId"] = (object) checkConfiguration.Resource.Id
    };

    protected virtual CheckConfigurationBinder GetCheckConfigurationBinder() => new CheckConfigurationBinder();

    public virtual int DeleteUnusedSoftDeletedCheckConfigurations(
      int batchSize,
      int deleteOffsetInHours)
    {
      return 0;
    }

    public virtual int DeleteHistoricalCheckConfigurationVersions(
      int batchSize,
      int deleteOffsetInHours)
    {
      return 0;
    }
  }
}
