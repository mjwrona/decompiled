// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTrackingResourceComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  [SupportedSqlAccessIntent(SqlAccessIntent.ReadWrite | SqlAccessIntent.ReadOnly | SqlAccessIntent.Dbo, null)]
  internal abstract class WorkItemTrackingResourceComponent : TeamFoundationSqlResourceComponent
  {
    private int m_remainingRetries = 2;

    public WorkItemTrackingResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected override bool HandleCustomException(SqlException ex)
    {
      bool flag = false;
      if (this.m_remainingRetries-- > 0)
      {
        switch (WorkItemTrackingResourceComponent.GetErrorNumber(this, ex))
        {
          case 600047:
            if (this.RequestContext.GetAccessIntent(this.DataspaceCategory) == AccessIntent.Read)
              throw new WorkItemTrackingElevateAccessIntentException((Exception) ex);
            this.RequestContext.Trace(900406, TraceLevel.Info, "ResourceComponent", nameof (WorkItemTrackingResourceComponent), "User is not in system. On best effort trying to pull down his membership info from BIS");
            try
            {
              using (this.AcquireExemptionLock())
              {
                ITeamFoundationWorkItemTrackingMetadataService service = this.RequestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
                Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.RequestContext.GetUserIdentity();
                this.RequestContext.Trace(900427, TraceLevel.Info, "ResourceComponent", nameof (WorkItemTrackingResourceComponent), string.Format("RequestContext identity: {0}", (object) userIdentity));
                IVssRequestContext requestContext = this.RequestContext;
                Microsoft.VisualStudio.Services.Identity.Identity identity = userIdentity;
                flag = service.SyncIdentity(requestContext, identity, nameof (HandleCustomException));
                break;
              }
            }
            catch (Exception ex1)
            {
              this.RequestContext.TraceException(900407, "ResourceComponent", nameof (WorkItemTrackingResourceComponent), ex1);
              break;
            }
          case 600172:
            flag = true;
            break;
        }
      }
      return flag;
    }

    private static int GetErrorNumber(WorkItemTrackingResourceComponent component, SqlException ex)
    {
      foreach (SqlError error in ex.Errors)
      {
        if (component.ContainerErrorCode != 0 && error.Number == component.ContainerErrorCode)
          return TeamFoundationServiceException.ExtractInt(error, "error");
        if (!error.IsInformational())
          return error.Number;
      }
      return ex.Number;
    }

    protected virtual IDataReader ExecuteReader() => (IDataReader) base.ExecuteReader();

    protected override object ExecuteUnknown(SqlDataReader reader, object parameter) => parameter is System.Func<IDataReader, object> func ? func((IDataReader) reader) : (object) null;

    protected virtual TResult ExecuteUnknown<TResult>(System.Func<IDataReader, TResult> binder) => (TResult) this.ExecuteUnknown((object) (System.Func<IDataReader, object>) (reader => (object) binder(reader)));

    internal static IEnumerable<T> Bind<T>(IDataReader reader, System.Func<IDataReader, T> binder)
    {
      while (reader.Read())
        yield return binder(reader);
    }

    protected SqlParameter BindBasicTvp<T>(
      WorkItemTrackingResourceComponent.TvpRecordBinder<T> recordBinder,
      string parameterName,
      IEnumerable<T> rows)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingResourceComponent.TvpRecordBinder<T>>(recordBinder, nameof (recordBinder));
      ArgumentUtility.CheckForNull<string>(parameterName, nameof (parameterName));
      ArgumentUtility.CheckForNull<IEnumerable<T>>(rows, nameof (rows));
      return this.BindTable(parameterName, recordBinder.TypeName, (IEnumerable<SqlDataRecord>) rows.Select<T, WorkItemTrackingSqlDataRecord>(new System.Func<T, WorkItemTrackingSqlDataRecord>(recordBinder.Bind)));
    }

    protected virtual SqlParameter BindWorkItemIdRevPairs(
      string paramName,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs)
    {
      return this.BindBasicTvp<WorkItemIdRevisionPair>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemIdRevisionPair>) new WorkItemTrackingResourceComponent.WorkItemIdRevPairBinder(), paramName, workItemIdRevPairs);
    }

    protected virtual SqlParameter BindWorkItemIdRevPairsAsKeyValuePairInt32Int32(
      string paramName,
      IEnumerable<WorkItemIdRevisionPair> workItemIdRevPairs)
    {
      return this.BindKeyValuePairInt32Int32Table(paramName, workItemIdRevPairs.Select<WorkItemIdRevisionPair, KeyValuePair<int, int>>((System.Func<WorkItemIdRevisionPair, KeyValuePair<int, int>>) (p => new KeyValuePair<int, int>(p.Id, p.Revision))));
    }

    protected static SqlExceptionFactory CreateFactory<T>() where T : Exception, new() => new SqlExceptionFactory(typeof (T), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((_1, _2, _3, _4) => (Exception) new T()));

    protected virtual object ExecuteScalarEx() => this.ExecuteScalar();

    protected virtual int ExecuteNonQueryEx() => this.ExecuteNonQuery();

    protected SqlCommandTimeouts GetEffectiveCommandTimeout() => this.CommandTimeoutOverrideAnonymousUser.HasValue ? new SqlCommandTimeouts(this.CommandTimeoutOverride.GetValueOrDefault(this.CommandTimeout), this.CommandTimeoutOverrideAnonymousUser.Value) : new SqlCommandTimeouts(this.CommandTimeoutOverride.GetValueOrDefault(this.CommandTimeout));

    protected virtual SqlCommand PrepareDynamicProcedure(
      string procedureName,
      string sqlStatement,
      bool bindPartitionId = true,
      bool addStatementIndex = true)
    {
      SqlCommand sqlCommand = this.PrepareSqlBatch(sqlStatement.Length, bindPartitionId, this.GetEffectiveCommandTimeout());
      this.AddStatement(sqlStatement, 0, true, addStatementIndex);
      return sqlCommand;
    }

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure)
    {
      this.EnforceDataspaceRestrictionsOnProject();
      return this.PrepareStoredProcedure(storedProcedure, this.GetEffectiveCommandTimeout());
    }

    protected override SqlCommand PrepareStoredProcedure(
      string storedProcedure,
      bool bindPartitionId)
    {
      return this.PrepareStoredProcedure(storedProcedure, bindPartitionId, this.GetEffectiveCommandTimeout());
    }

    protected override SqlCommand PrepareStoredProcedure(string storedProcedure, int commandTimeout)
    {
      this.EnforceDataspaceRestrictionsOnProject();
      return base.PrepareStoredProcedure(storedProcedure, commandTimeout);
    }

    protected override SqlCommand PrepareSqlBatch(int lengthEstimate) => this.PrepareSqlBatch(lengthEstimate, this.GetEffectiveCommandTimeout());

    protected override SqlParameter BindDateTime(string parameterName, DateTime parameterValue)
    {
      if (parameterValue < SqlDateTime.MinValue.Value)
        parameterValue = SqlDateTime.MinValue.Value;
      return base.BindDateTime(parameterName, parameterValue);
    }

    protected override SqlParameter BindNullableDateTime(
      string parameterName,
      DateTime? parameterValue)
    {
      if (parameterValue.HasValue)
      {
        DateTime? nullable = parameterValue;
        SqlDateTime minValue = SqlDateTime.MinValue;
        DateTime dateTime1 = minValue.Value;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < dateTime1 ? 1 : 0) : 0) != 0)
        {
          ref DateTime? local = ref parameterValue;
          minValue = SqlDateTime.MinValue;
          DateTime dateTime2 = minValue.Value;
          local = new DateTime?(dateTime2);
        }
      }
      return base.BindNullableDateTime(parameterName, parameterValue);
    }

    protected void EnforceDataspaceRestrictionsOnProject()
    {
      if (this.RequestContext != null && !this.RequestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.BypassDataspaceRestrictionForMembers))
        this.RequestContext.RootContext.Items.Add(RequestContextItemsKeys.BypassDataspaceRestrictionForMembers, (object) true);
      if (!this.m_isAnonymousOrPublicRequest)
        return;
      this.EnforceDataspaceRestrictions();
    }

    public int? CommandTimeoutOverride { get; set; }

    public int? CommandTimeoutOverrideAnonymousUser { get; set; }

    private class WorkItemIdRevPairBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemIdRevisionPair>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("Rev", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitIdRevTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrackingResourceComponent.WorkItemIdRevPairBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemIdRevisionPair row)
      {
        record.SetInt32(0, row.Id);
        record.SetInt32(1, row.Revision);
      }
    }

    protected abstract class TvpRecordBinder<T>
    {
      public WorkItemTrackingSqlDataRecord Bind(T entry)
      {
        WorkItemTrackingSqlDataRecord record = new WorkItemTrackingSqlDataRecord(this.TvpMetadata);
        this.SetRecordValues(record, entry);
        return record;
      }

      protected abstract SqlMetaData[] TvpMetadata { get; }

      public abstract string TypeName { get; }

      public abstract void SetRecordValues(WorkItemTrackingSqlDataRecord record, T entry);
    }

    private class ComponentErrorData
    {
      private ComponentErrorData()
      {
      }

      public static WorkItemTrackingResourceComponent.ComponentErrorData Create(
        WorkItemTrackingResourceComponent component,
        SqlException sqlException)
      {
        int errorNumber = WorkItemTrackingResourceComponent.GetErrorNumber(component, sqlException);
        string dataspaceCategory = component.DataspaceCategory;
        return new WorkItemTrackingResourceComponent.ComponentErrorData()
        {
          SqlException = sqlException,
          ErrorNumber = errorNumber,
          DataspaceCategory = dataspaceCategory
        };
      }

      public ComponentErrorData(
        SqlException sqlException,
        int errorNumber,
        string dataspaceCategory)
      {
        this.SqlException = sqlException;
        this.ErrorNumber = errorNumber;
        this.DataspaceCategory = dataspaceCategory;
      }

      public SqlException SqlException { get; private set; }

      public int ErrorNumber { get; private set; }

      public string DataspaceCategory { get; private set; }
    }
  }
}
