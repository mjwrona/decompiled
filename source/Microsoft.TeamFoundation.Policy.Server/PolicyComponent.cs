// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyComponent
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class PolicyComponent : PolicyBaseComponent, IPolicyComponent, IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<PolicyComponent>(1380),
      (IComponentCreator) new ComponentCreator<PolicyComponent>(1610),
      (IComponentCreator) new ComponentCreator<PolicyComponent>(2310)
    }, "Policy");
    private static readonly SqlMetaData[] typ_EvaluationRecordUpdate = new SqlMetaData[4]
    {
      new SqlMetaData("PolicyConfigurationId", SqlDbType.Int),
      new SqlMetaData("PolicyConfigurationReviewer", SqlDbType.Int),
      new SqlMetaData("PolicyStatus", SqlDbType.TinyInt),
      new SqlMetaData("Context", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_ConfigurationScope = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, 436L)
    };
    private static readonly SqlMetaData[] typ_ConfigurationScopesByVersion2 = new SqlMetaData[3]
    {
      new SqlMetaData("PolicyConfigurationId", SqlDbType.Int),
      new SqlMetaData("VersionId", SqlDbType.Int),
      new SqlMetaData("Scope", SqlDbType.NVarChar, 436L)
    };
    protected const string c_serviceName = "Policy";
    protected const int c_maxCommentLength = 4000;
    protected const int c_maxArtifactIdLength = 400;

    public PolicyComponent()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public SqlCommand PrepareStoredProcedure(string storedProcedure, Guid projectId)
    {
      SqlCommand sqlCommand = this.PrepareStoredProcedure(storedProcedure);
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      return sqlCommand;
    }

    PolicyConfigurationRecord IPolicyComponent.UpdatePolicyConfiguration(
      PolicyConfigurationRecord updatedConfiguration,
      System.Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      int expectedVersionId)
    {
      if (updatedConfiguration.Settings.Length > 50000)
        throw new PolicySettingsTooLargeException(updatedConfiguration.Settings.Length);
      this.PrepareStoredProcedure("prc_UpdatePolicyConfiguration", updatedConfiguration.ProjectId);
      this.BindInt("@policyConfigurationId", updatedConfiguration.ConfigurationId);
      this.BindScopes("@scopes", scopes);
      this.BindBoolean("@isEnabled", updatedConfiguration.IsEnabled);
      this.BindBoolean("@isBlocking", updatedConfiguration.IsBlocking);
      this.BindNullValue("@isDeleted", SqlDbType.Bit);
      this.BindString("@settings", updatedConfiguration.Settings, -1, false, SqlDbType.NVarChar);
      if (this.Version >= 1610)
        this.BindBoolean("@isEnterpriseManaged", updatedConfiguration.IsEnterpriseManaged);
      this.BindGuid("@modifiedById", updatedConfiguration.CreatorId);
      this.BindGuid("@policyTypeId", updatedConfiguration.TypeId);
      this.BindInt("@expectedVersionId", expectedVersionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(updatedConfiguration.ProjectId));
        return resultCollection.GetCurrent<PolicyConfigurationRecord>().Items.Single<PolicyConfigurationRecord>();
      }
    }

    PolicyConfigurationRecord IPolicyComponent.DeletePolicyConfiguration(
      Guid projectId,
      int policyConfigurationId,
      Guid modifiedById,
      System.Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      int expectedVersionId)
    {
      this.PrepareStoredProcedure("prc_UpdatePolicyConfiguration", projectId);
      this.BindInt("@policyConfigurationId", policyConfigurationId);
      this.BindScopes("@scopes", scopes);
      this.BindGuid("@modifiedById", modifiedById);
      this.BindNullValue("@isEnabled", SqlDbType.Bit);
      this.BindNullValue("@isBlocking", SqlDbType.Bit);
      this.BindBoolean("@isDeleted", true);
      this.BindNullValue("@settings", SqlDbType.NVarChar);
      if (this.Version >= 1610)
        this.BindNullValue("@isEnterpriseManaged", SqlDbType.Bit);
      this.BindNullValue("@policyTypeId", SqlDbType.UniqueIdentifier);
      this.BindInt("@expectedVersionId", expectedVersionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
        return resultCollection.GetCurrent<PolicyConfigurationRecord>().Items.Single<PolicyConfigurationRecord>();
      }
    }

    protected PolicyEvaluationRecord SavePolicyEvaluationRecord(
      Guid projectId,
      PolicyEvaluationRecord toSave)
    {
      this.PrepareStoredProcedure("prc_SavePolicyEvaluationRecord", projectId);
      this.BindInt("@policyConfigurationId", toSave.Configuration.Id);
      this.BindInt("@policyConfigurationVersion", toSave.Configuration.Revision);
      this.BindString("@artifactId", toSave.ArtifactId, 400, false, SqlDbType.NVarChar);
      if (toSave.Status.HasValue)
        this.BindByte("@policyStatus", (byte) toSave.Status.Value);
      if (toSave.Context != null)
        this.BindString("@context", toSave.Context.ToString(), 4000, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyEvaluationRecord>((ObjectBinder<PolicyEvaluationRecord>) new PolicyEvaluationRecordBinder());
        return resultCollection.GetCurrent<PolicyEvaluationRecord>().Items.FirstOrDefault<PolicyEvaluationRecord>();
      }
    }

    public PolicyConfigurationRecord GetPolicyConfiguration(
      Guid projectId,
      int configurationId,
      int? revisionId)
    {
      if (!revisionId.HasValue)
        return this.GetLatestPolicyConfiguration(projectId, configurationId);
      this.PrepareStoredProcedure("prc_QueryPolicyConfigurations", projectId);
      this.BindInt("@configurationId", configurationId);
      this.BindInt("@versionId", revisionId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
        return resultCollection.GetCurrent<PolicyConfigurationRecord>().Items.FirstOrDefault<PolicyConfigurationRecord>();
      }
    }

    public PolicyConfigurationRecord GetLatestPolicyConfiguration(
      Guid projectId,
      int configurationId)
    {
      this.PrepareStoredProcedure("prc_QueryPolicyConfigurations", projectId);
      this.BindInt("@configurationId", configurationId);
      this.BindInt("@top", 1);
      this.BindInt("@skip", 0);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
        return resultCollection.GetCurrent<PolicyConfigurationRecord>().Items.FirstOrDefault<PolicyConfigurationRecord>();
      }
    }

    public VirtualResultCollection<PolicyConfigurationRecord> GetPolicyConfigurationRevisions(
      Guid projectId,
      int configurationId,
      int top,
      int skip)
    {
      this.PrepareStoredProcedure("prc_QueryPolicyConfigurations", projectId);
      this.BindInt("@configurationId", configurationId);
      this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
      return new VirtualResultCollection<PolicyConfigurationRecord>(rc);
    }

    public VirtualResultCollection<PolicyEvaluationRecord> GetPolicyEvaluationRecords(
      Guid projectId,
      int? policyConfigurationId,
      int? policyConfigurationRevision,
      ArtifactId artifactId,
      bool includeNotApplicable,
      int top,
      int skip)
    {
      this.PrepareStoredProcedure("prc_QueryPolicyEvaluationRecords", projectId);
      if (policyConfigurationId.HasValue)
        this.BindInt("@policyConfigurationId", policyConfigurationId.Value);
      if (policyConfigurationRevision.HasValue)
        this.BindInt("@policyConfigurationVersion", policyConfigurationRevision.Value);
      if (artifactId != null)
        this.BindString("@artifactId", LinkingUtilities.EncodeUri(artifactId), 400, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeNotApplicable", includeNotApplicable);
      this.BindInt("@top", top);
      this.BindInt("@skip", skip);
      ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      rc.AddBinder<PolicyEvaluationRecord>((ObjectBinder<PolicyEvaluationRecord>) new PolicyEvaluationRecordBinder(PolicySecuredObjectFactory.CreateReadOnlyInstance(projectId)));
      return new VirtualResultCollection<PolicyEvaluationRecord>(rc);
    }

    protected IList<PolicyEvaluationRecord> DeletePolicyEvaluationRecords(
      Guid projectId,
      ArtifactId artifactId,
      int? policyConfigurationId = null)
    {
      this.PrepareStoredProcedure("prc_DeletePolicyEvaluationRecords", projectId);
      if (policyConfigurationId.HasValue)
        this.BindInt("@policyConfigurationId", policyConfigurationId.Value);
      this.BindString("@artifactId", LinkingUtilities.EncodeUri(artifactId), 400, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<PolicyEvaluationRecord>((ObjectBinder<PolicyEvaluationRecord>) new PolicyEvaluationRecordBinder());
      return (IList<PolicyEvaluationRecord>) resultCollection.GetCurrent<PolicyEvaluationRecord>().Items;
    }

    public IList<PolicyConfigurationRecord> GetLatestPolicyConfigurations(
      Guid projectId,
      int top,
      int firstConfigurationId,
      Guid? policyType = null)
    {
      this.PrepareStoredProcedure("prc_GetLatestPolicyConfigurations", projectId);
      this.BindInt("@top", top);
      this.BindInt("@firstConfigurationId", firstConfigurationId);
      if (policyType.HasValue)
        this.BindGuid("@policyType", policyType.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
        return (IList<PolicyConfigurationRecord>) resultCollection.GetCurrent<PolicyConfigurationRecord>().Items;
      }
    }

    public IList<PolicyConfigurationRecord> GetLatestPolicyConfigurationsByScope(
      Guid projectId,
      IEnumerable<string> scopes,
      System.Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      int top,
      int firstConfigurationId,
      Guid? policyType = null,
      bool includeHidden = false,
      bool userVersion2 = false)
    {
      this.PrepareStoredProcedure("prc_GetLatestPolicyConfigurationsByScope", projectId);
      this.BindScopes("@scopes", scopes);
      if (policyType.HasValue)
        this.BindGuid("@policyType", policyType.Value);
      this.BindInt("@top", top);
      this.BindInt("@firstConfigurationId", firstConfigurationId);
      if (userVersion2 && this.Version >= 2310)
        this.BindInt("@featureFlagVersion", 2);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        long milliseconds = stopwatch.ElapsedMilliseconds;
        this.RequestContext.TraceConditionally(1391001, TraceLevel.Verbose, "Policy", nameof (PolicyComponent), (Func<string>) (() =>
        {
          string str = string.Join(", ", scopes.Take<string>(20));
          return string.Format("Getting latest policy configs by scope from DB took {0} ms. ", (object) milliseconds) + string.Format("Total scopes: {0}. First 20 scopes: {1}", (object) scopes.Count<string>(), (object) str);
        }));
        stopwatch.Restart();
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
        List<PolicyConfigurationRecord> policies = resultCollection.GetCurrent<PolicyConfigurationRecord>().Items;
        milliseconds = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        this.RequestContext.TraceConditionally(1391002, TraceLevel.Verbose, "Policy", nameof (PolicyComponent), (Func<string>) (() =>
        {
          string str = string.Join(", ", scopes.Take<string>(20));
          return string.Format("Binding latest policy configs by scope took {0} ms. Number of policies: {1}. ", (object) milliseconds, (object) policies.Count) + string.Format("Total scopes: {0}. First 20 scopes: {1}", (object) scopes.Count<string>(), (object) str);
        }));
        return (IList<PolicyConfigurationRecord>) policies;
      }
    }

    public IDictionary<string, int> GetPolicyConfigurationsCountByScope(
      Guid projectId,
      IEnumerable<string> scopes)
    {
      this.PrepareStoredProcedure("prc_GetPolicyConfigurationsCountByScope", projectId);
      this.BindScopes("@scopes", scopes);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      Dictionary<string, int> dictionary;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        long milliseconds = stopwatch.ElapsedMilliseconds;
        this.RequestContext.TraceConditionally(1391011, TraceLevel.Verbose, "Policy", nameof (PolicyComponent), (Func<string>) (() =>
        {
          string str = string.Join(", ", scopes.Take<string>(20));
          return string.Format("Getting policy configurations count by scope from DB took {0} ms. ", (object) milliseconds) + string.Format("Total scopes: {0}. First 20 scopes: {1}", (object) scopes.Count<string>(), (object) str);
        }));
        stopwatch.Restart();
        resultCollection.AddBinder<PolicyCountByScopeRecord>((ObjectBinder<PolicyCountByScopeRecord>) new PolicyCountByScopeBinder());
        dictionary = resultCollection.GetCurrent<PolicyCountByScopeRecord>().Items.ToDictionary<PolicyCountByScopeRecord, string, int>((System.Func<PolicyCountByScopeRecord, string>) (rec => rec.Scope), (System.Func<PolicyCountByScopeRecord, int>) (rec => rec.PolicyCount));
        milliseconds = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();
        this.RequestContext.TraceConditionally(1391012, TraceLevel.Verbose, "Policy", nameof (PolicyComponent), (Func<string>) (() =>
        {
          string str = string.Join(", ", scopes.Take<string>(20));
          return string.Format("Binding policy configurations count by scope took {0} ms.", (object) milliseconds) + string.Format("Total scopes: {0}. First 20 scopes: {1}", (object) scopes.Count<string>(), (object) str);
        }));
      }
      return (IDictionary<string, int>) dictionary;
    }

    public PolicyConfigurationRecord CreatePolicyConfiguration(
      PolicyConfigurationRecord newConfiguration,
      System.Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes)
    {
      if (newConfiguration.Settings.Length > 50000)
        throw new PolicySettingsTooLargeException(newConfiguration.Settings.Length);
      IEnumerable<string> scopes = determineScopes(newConfiguration);
      this.PrepareStoredProcedure("prc_CreatePolicyConfiguration", newConfiguration.ProjectId);
      this.BindGuid("@policyType", newConfiguration.TypeId);
      this.BindScopes("@scopes", scopes);
      this.BindBoolean("@isEnabled", newConfiguration.IsEnabled);
      this.BindBoolean("@isBlocking", newConfiguration.IsBlocking);
      this.BindString("@settings", newConfiguration.Settings, -1, false, SqlDbType.NVarChar);
      this.BindGuid("@createdById", newConfiguration.CreatorId);
      if (this.Version >= 1610)
        this.BindBoolean("@isEnterpriseManaged", newConfiguration.IsEnterpriseManaged);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      sqlDataReader.Read();
      int int32_1 = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("PolicyConfigurationId"));
      int int32_2 = sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("VersionId"));
      DateTime dateTime = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("DateCreated"));
      int configurationId = int32_1;
      return new PolicyConfigurationRecord(int32_2, configurationId, newConfiguration.TypeId, newConfiguration.ProjectId, newConfiguration.IsEnabled, newConfiguration.IsBlocking, newConfiguration.IsEnterpriseManaged, newConfiguration.Settings, newConfiguration.CreatorId, dateTime, newConfiguration.IsDeleted);
    }

    public IList<PolicyConfigurationRecord> QueryPolicyConfigurationVersionsToBackfill(
      Guid projectId,
      int firstConfigurationId,
      int configurationVersionsToFetch)
    {
      this.PrepareStoredProcedure("prc_QueryPolicyConfigurationVersionsToBackfill", projectId);
      this.BindInt("@firstConfigurationId", firstConfigurationId);
      this.BindInt("@configurationVersionsToFetch", configurationVersionsToFetch);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyConfigurationRecord>((ObjectBinder<PolicyConfigurationRecord>) new PolicyConfigurationBinder(projectId));
        return (IList<PolicyConfigurationRecord>) resultCollection.GetCurrent<PolicyConfigurationRecord>().Items;
      }
    }

    public void BackfillPolicyConfigurationScopes(
      Guid projectId,
      IList<ConfigurationScopesByVersion> scopesByVersion)
    {
      this.PrepareStoredProcedure("prc_BackfillPolicyConfigurationScopes", projectId);
      this.BindConfigurationScopes("@configurationScopes", (IEnumerable<ConfigurationScopesByVersion>) scopesByVersion);
      this.ExecuteNonQuery();
    }

    public void UpdatePolicyEvaluationRecords(
      Guid projectId,
      ArtifactId artifactId,
      IEnumerable<PolicyEvaluationRecord> recordsToUpdate,
      IEnumerable<int> idsOfRecordsToDelete)
    {
      this.PrepareStoredProcedure("prc_UpdatePolicyEvaluationRecords", projectId);
      this.BindString("@artifactId", LinkingUtilities.EncodeUri(artifactId), 400, false, SqlDbType.NVarChar);
      this.BindReviewerUpdateTable("@recordsToUpdate", recordsToUpdate);
      this.BindInt32Table("@idsOfRecordsToDelete", idsOfRecordsToDelete);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindReviewerUpdateTable(
      string name,
      IEnumerable<PolicyEvaluationRecord> updates)
    {
      System.Func<PolicyEvaluationRecord, SqlDataRecord> selector = (System.Func<PolicyEvaluationRecord, SqlDataRecord>) (update =>
      {
        SqlDataRecord record = new SqlDataRecord(PolicyComponent.typ_EvaluationRecordUpdate);
        record.SetInt32(0, update.Configuration.Id);
        record.SetInt32(1, update.Configuration.Revision);
        record.SetByte(2, (byte) update.Status.Value);
        record.SetNullableString(3, update.Context == null ? (string) null : update.Context.ToString());
        return record;
      });
      return this.BindTable(name, "typ_EvaluationRecordUpdate", updates.Select<PolicyEvaluationRecord, SqlDataRecord>(selector));
    }

    protected SqlParameter BindScopes(string name, IEnumerable<string> scopes)
    {
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (scope =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PolicyComponent.typ_ConfigurationScope);
        sqlDataRecord.SetString(0, scope);
        return sqlDataRecord;
      });
      return this.BindTable(name, "typ_ConfigurationScope", scopes.Select<string, SqlDataRecord>(selector));
    }

    public PolicyEvaluationRecord GetPolicyEvaluationRecord(Guid projectId, Guid evaluationRecordId)
    {
      this.PrepareStoredProcedure("prc_GetPolicyEvaluationRecord", projectId);
      this.BindGuid("@policyEvaluationRecordId", evaluationRecordId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PolicyEvaluationRecord>((ObjectBinder<PolicyEvaluationRecord>) new PolicyEvaluationRecordBinder(PolicySecuredObjectFactory.CreateReadOnlyInstance(projectId)));
        return resultCollection.GetCurrent<PolicyEvaluationRecord>().Items.FirstOrDefault<PolicyEvaluationRecord>();
      }
    }

    public SqlParameter BindConfigurationScopes(
      string name,
      IEnumerable<ConfigurationScopesByVersion> scopes)
    {
      System.Func<ConfigurationScopesByVersion, SqlDataRecord> selector = (System.Func<ConfigurationScopesByVersion, SqlDataRecord>) (scope =>
      {
        SqlDataRecord record = new SqlDataRecord(PolicyComponent.typ_ConfigurationScopesByVersion2);
        record.SetInt32(0, scope.PolicyConfigurationId);
        record.SetInt32(1, scope.VersionId);
        record.SetNullableString(2, scope.Scope);
        return record;
      });
      return this.BindTable(name, "typ_ConfigurationScopesByVersion2", scopes.Select<ConfigurationScopesByVersion, SqlDataRecord>(selector));
    }

    public static IList<PolicyConfigurationRecord> FilterByScope(
      System.Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      IList<PolicyConfigurationRecord> records)
    {
      if (scopes == null || !scopes.Any<string>())
        return records;
      List<string> scopeList = scopes.ToList<string>();
      List<PolicyConfigurationRecord> configurationRecordList = new List<PolicyConfigurationRecord>();
      foreach (PolicyConfigurationRecord record in (IEnumerable<PolicyConfigurationRecord>) records)
      {
        if (determineScopes(record).Any<string>((System.Func<string, bool>) (s => scopeList.Contains(s))))
          configurationRecordList.Add(record);
      }
      return (IList<PolicyConfigurationRecord>) configurationRecordList;
    }
  }
}
