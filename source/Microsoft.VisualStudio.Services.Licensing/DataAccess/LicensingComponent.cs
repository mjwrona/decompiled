// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.LicensingComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  public class LicensingComponent : BaseLicenseComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<LicensingComponent>(1),
      (IComponentCreator) new ComponentCreator<LicensingComponent2>(2)
    }, "Licensing_Config");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = LicensingComponent.CreateSqlExceptionFactories();
    internal const int MachineIdColumnLength = 50;

    public LicensingComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    internal virtual IList<DateTimeOffset> GetVisualStudioTrialExpirations(
      string machineId,
      Guid userId,
      int majorVersion,
      int productFamilyId,
      int productEditionId,
      DateTimeOffset currentExpirationDate)
    {
      try
      {
        this.TraceEnter(1032000, nameof (GetVisualStudioTrialExpirations));
        this.PrepareStoredProcedure("prc_GetLicensingVSTrialExpirations");
        this.BindString("@machineId", machineId, 50, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@userId", userId);
        this.BindInt("@majorVersion", majorVersion);
        this.BindInt("@productFamilyId", productFamilyId);
        this.BindInt("@productEditionId", productEditionId);
        this.BindDateTime("@expirationDate", currentExpirationDate.UtcDateTime);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<LicensingComponent.VisualStudioTrialMachineRow>((ObjectBinder<LicensingComponent.VisualStudioTrialMachineRow>) new LicensingComponent.VisualStudioTrialMachineRowBinder());
        resultCollection.AddBinder<LicensingComponent.VisualStudioTrialUserRow>((ObjectBinder<LicensingComponent.VisualStudioTrialUserRow>) new LicensingComponent.VisualStudioTrialUserRowBinder());
        List<DateTimeOffset> trialExpirations = new List<DateTimeOffset>();
        List<LicensingComponent.VisualStudioTrialMachineRow> items1 = resultCollection.GetCurrent<LicensingComponent.VisualStudioTrialMachineRow>().Items;
        if (items1.Any<LicensingComponent.VisualStudioTrialMachineRow>())
          trialExpirations.Add(items1.First<LicensingComponent.VisualStudioTrialMachineRow>().ExpirationDate);
        else
          this.Trace(1032001, TraceLevel.Error, "Unexpected: No VS trial expiration info for machine:{0}, majorVersion:{1}, productFamilyId:{2}, productEditionId:{3}.", (object) machineId, (object) majorVersion, (object) productFamilyId, (object) productEditionId);
        resultCollection.NextResult();
        List<LicensingComponent.VisualStudioTrialUserRow> items2 = resultCollection.GetCurrent<LicensingComponent.VisualStudioTrialUserRow>().Items;
        if (items2.Any<LicensingComponent.VisualStudioTrialUserRow>())
          trialExpirations.Add(items2.First<LicensingComponent.VisualStudioTrialUserRow>().ExpirationDate);
        else
          this.Trace(1032002, TraceLevel.Error, "Unexpected: No VS trial expiration info for user:{0}, majorVersion:{1}, productFamilyId:{2}, productEditionId:{3}.", (object) userId, (object) majorVersion, (object) productFamilyId, (object) productEditionId);
        return (IList<DateTimeOffset>) trialExpirations;
      }
      catch (Exception ex)
      {
        this.TraceException(1032008, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032009, nameof (GetVisualStudioTrialExpirations));
      }
    }

    internal virtual IList<VisualStudioTrialUserInfo> GetVisualStudioTrialExpirations(
      DateTimeOffset minCreationTime,
      int batchSize)
    {
      try
      {
        this.TraceEnter(1032010, nameof (GetVisualStudioTrialExpirations));
        this.PrepareStoredProcedure("prc_GetLicensingVSTrialUserExpirationBatch");
        this.BindDateTime("@minCreationTime", minCreationTime.UtcDateTime);
        this.BindInt("@batchSize", batchSize);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<VisualStudioTrialUserInfo>((ObjectBinder<VisualStudioTrialUserInfo>) new LicensingComponent.VisualStudioTrialUserInfoBinder());
        return (IList<VisualStudioTrialUserInfo>) resultCollection.GetCurrent<VisualStudioTrialUserInfo>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032008, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032019, nameof (GetVisualStudioTrialExpirations));
      }
    }

    internal virtual void SetVisualStudioTrialExpirationMigrationStatus(
      IList<VisualStudioTrialUserInfo> updates)
    {
      this.PrepareStoredProcedure("prc_SetLicensingVSTrialUserMigrationStatus");
      this.BindLicensingVSTrialUserTable("@updates", (IEnumerable<VisualStudioTrialUserInfo>) updates);
      this.ExecuteNonQuery();
    }

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "LicensingComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) LicensingComponent.s_sqlExceptionFactories;

    private static Dictionary<int, SqlExceptionFactory> CreateSqlExceptionFactories() => new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1060001,
        new SqlExceptionFactory(typeof (LicensingInvalidMachineIdException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingInvalidMachineIdException()))
      },
      {
        1060002,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060003,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060004,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      }
    };

    protected new void TraceEnter(int tracepoint, [CallerMemberName] string methodName = null) => base.TraceEnter(tracepoint, methodName);

    protected new void TraceLeave(int tracePoint, [CallerMemberName] string methodName = null) => base.TraceLeave(tracePoint, methodName);

    internal class VisualStudioTrialMachineRow
    {
      internal DateTimeOffset ExpirationDate { get; set; }
    }

    internal class VisualStudioTrialUserRow
    {
      internal DateTimeOffset ExpirationDate { get; set; }
    }

    internal class VisualStudioTrialMachineRowBinder : 
      ObjectBinder<LicensingComponent.VisualStudioTrialMachineRow>
    {
      private SqlColumnBinder m_expirationDateColumn = new SqlColumnBinder("ExpirationDate");

      protected override LicensingComponent.VisualStudioTrialMachineRow Bind() => new LicensingComponent.VisualStudioTrialMachineRow()
      {
        ExpirationDate = (DateTimeOffset) this.m_expirationDateColumn.GetDateTime((IDataReader) this.Reader)
      };
    }

    internal class VisualStudioTrialUserRowBinder : 
      ObjectBinder<LicensingComponent.VisualStudioTrialUserRow>
    {
      private SqlColumnBinder m_expirationDateColumn = new SqlColumnBinder("ExpirationDate");

      protected override LicensingComponent.VisualStudioTrialUserRow Bind() => new LicensingComponent.VisualStudioTrialUserRow()
      {
        ExpirationDate = (DateTimeOffset) this.m_expirationDateColumn.GetDateTime((IDataReader) this.Reader)
      };
    }

    internal class VisualStudioTrialUserInfoBinder : ObjectBinder<VisualStudioTrialUserInfo>
    {
      private SqlColumnBinder m_userIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder m_majorVersionColumn = new SqlColumnBinder("MajorVersion");
      private SqlColumnBinder m_productFamilyIdColumn = new SqlColumnBinder("ProductFamilyId");
      private SqlColumnBinder m_productEditionIdColumn = new SqlColumnBinder("ProductEditionId");
      private SqlColumnBinder m_expirationDateColumn = new SqlColumnBinder("ExpirationDate");
      private SqlColumnBinder m_createdDateColumn = new SqlColumnBinder("CreatedDate");
      private SqlColumnBinder m_isMigratedColumn = new SqlColumnBinder("IsMigrated");

      protected override VisualStudioTrialUserInfo Bind() => new VisualStudioTrialUserInfo()
      {
        UserId = this.m_userIdColumn.GetGuid((IDataReader) this.Reader),
        MajorVersion = this.m_majorVersionColumn.GetInt32((IDataReader) this.Reader),
        ProductFamilyId = this.m_productFamilyIdColumn.GetInt32((IDataReader) this.Reader),
        ProductEditionId = this.m_productEditionIdColumn.GetInt32((IDataReader) this.Reader),
        ExpirationDate = (DateTimeOffset) this.m_expirationDateColumn.GetDateTime((IDataReader) this.Reader),
        CreatedDate = (DateTimeOffset) this.m_createdDateColumn.GetDateTime((IDataReader) this.Reader),
        IsMigrated = this.m_isMigratedColumn.GetBoolean((IDataReader) this.Reader, false)
      };
    }
  }
}
