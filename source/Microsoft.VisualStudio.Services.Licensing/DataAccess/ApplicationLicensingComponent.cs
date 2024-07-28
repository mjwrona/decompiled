// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.ApplicationLicensingComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class ApplicationLicensingComponent : 
    BaseLicenseComponent,
    ILicensingComponent,
    IDisposable
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[13]
    {
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent>(1),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent2>(2),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent3>(3),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent4>(4),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent5>(5),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent6>(6),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent7>(7),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent8>(8),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent9>(9),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent10>(10),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent11>(11),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent12>(12),
      (IComponentCreator) new ComponentCreator<ApplicationLicensingComponent13>(13)
    }, "Licensing_Partition");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = ApplicationLicensingComponent.CreateSqlExceptionFactories();

    public ApplicationLicensingComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual void CreateScope(Guid scopeId)
    {
    }

    internal virtual void CreateScopeAlias(Guid originalScopeId, Guid aliasScopeId) => throw new NotImplementedException();

    public virtual IList<Guid> GetScopes() => (IList<Guid>) new List<Guid>();

    internal virtual List<UserLicense> GetPreviousUserLicenses(Guid scopeId) => throw new ServiceVersionNotSupportedException("Licensing_Partition", this.Version, 8);

    public virtual List<UserLicense> GetPreviousUserLicenses(Guid scopeId, IList<Guid> userIds) => throw new ServiceVersionNotSupportedException("Licensing_Partition", this.Version, 8);

    public virtual UserLicense GetUserLicense(Guid scopeId, Guid userId)
    {
      try
      {
        this.TraceEnter(1032101, nameof (GetUserLicense));
        this.PrepareStoredProcedure("prc_GetUserLicense");
        this.BindGuid("@userId", userId);
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc, scopeId);
        return rc.GetCurrent<UserLicense>().Items.FirstOrDefault<UserLicense>();
      }
      catch (Exception ex)
      {
        this.TraceException(1032108, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032109, nameof (GetUserLicense));
      }
    }

    public virtual UserLicense SetUserLicense(
      Guid scopeId,
      Guid userId,
      LicensingSource source,
      int license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      AccountUserStatus statusIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity)
    {
      try
      {
        this.TraceEnter(1032111, nameof (SetUserLicense));
        this.PrepareStoredProcedure("prc_UpsertUserLicense");
        this.BindGuid("@userId", userId);
        this.BindByte("@source", (byte) source);
        this.BindByte("@license", (byte) license);
        this.BindByte("@statusIfAbsent", (byte) statusIfAbsent);
        this.BindEventData(licensingEvent);
        using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          rc.AddBinder<LicenseEventRow>((ObjectBinder<LicenseEventRow>) new Binders.LicenseEventRowBinder());
          rc.NextResult();
          this.AddUserLicenseRowBinder(rc, scopeId);
          return rc.GetCurrent<UserLicense>().Items.FirstOrDefault<UserLicense>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1032118, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032119, nameof (SetUserLicense));
      }
    }

    public virtual void DeleteUserLicense(
      Guid scopeId,
      Guid userId,
      ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032121, nameof (DeleteUserLicense));
        this.PrepareStoredProcedure("prc_DeleteUserLicense");
        this.BindGuid("@userId", userId);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032128, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032129, nameof (DeleteUserLicense));
      }
    }

    public virtual IList<UserLicense> GetUserLicenses(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032131, nameof (GetUserLicenses));
        this.PrepareStoredProcedure("prc_GetUserLicenses");
        ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        this.AddUserLicenseRowBinder(rc, scopeId);
        return (IList<UserLicense>) rc.GetCurrent<UserLicense>().Items;
      }
      catch (Exception ex)
      {
        this.TraceException(1032138, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032139, nameof (GetUserLicenses));
      }
    }

    public virtual IList<UserLicense> GetUserLicenses(Guid scopeId, IList<Guid> userIds) => throw new ServiceVersionNotSupportedException("Licensing_Partition", this.Version, 6);

    public virtual IList<UserLicense> GetUserLicenses(Guid scopeId, int top, int skip) => throw new ServiceVersionNotSupportedException("Licensing_Partition", 5, 5);

    public virtual IPagedList<UserLicense> GetUserLicenses(Guid scopeId, string continuationToken) => throw new NotImplementedException();

    public virtual void UpdateUserLastAccessed(
      Guid scopeId,
      Guid userId,
      DateTimeOffset lastAccessedDate)
    {
    }

    public virtual IList<AccountLicenseCount> GetUserLicensesDistribution(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032171, nameof (GetUserLicensesDistribution));
        this.PrepareStoredProcedure("prc_GetUserLicensesDistribution");
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<AccountLicenseCount>((ObjectBinder<AccountLicenseCount>) new Binders.AccountLicenseCountBinder());
        return (IList<AccountLicenseCount>) (resultCollection.GetCurrent<AccountLicenseCount>().Items ?? new List<AccountLicenseCount>());
      }
      catch (Exception ex)
      {
        this.TraceException(1032178, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032179, nameof (GetUserLicensesDistribution));
      }
    }

    public virtual IList<UserLicenseCount> GetUserLicenseUsage(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032271, nameof (GetUserLicenseUsage));
        this.PrepareStoredProcedure("prc_GetUserLicenseUsage");
        this.BindGuid("@scopeId", scopeId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<UserLicenseCount>((ObjectBinder<UserLicenseCount>) new Binders.UserLicenseCountBinder());
        return (IList<UserLicenseCount>) (resultCollection.GetCurrent<UserLicenseCount>().Items ?? new List<UserLicenseCount>());
      }
      catch (Exception ex)
      {
        this.TraceException(1032278, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032279, nameof (GetUserLicenseUsage));
      }
    }

    public virtual void TransferUserLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
    }

    public virtual void ImportScope(
      Guid scopeId,
      List<UserLicense> userLicenses,
      List<UserLicense> previousUserLicenses,
      List<UserExtensionLicense> userExtensionLicenses,
      ILicensingEvent licensingEvent)
    {
      throw new ServiceVersionNotSupportedException("Licensing_Partition", 7, 7);
    }

    public virtual void DeleteScope(Guid scopeId, ILicensingEvent licensingEvent) => throw new ServiceVersionNotSupportedException("Licensing_Partition", 7, 7);

    public virtual void AddUser(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      License licenseIfAbsent,
      AssignmentSource assignmentSourceIfAbsent,
      LicensingOrigin originIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity)
    {
      try
      {
        this.TraceEnter(1032141, nameof (AddUser));
        this.PrepareStoredProcedure("prc_UpsertUserStatus");
        this.BindGuid("@userId", userId);
        this.BindByte("@status", (byte) status);
        this.BindByte("@sourceIfAbsent", (byte) licenseIfAbsent.Source);
        this.BindByte("@licenseIfAbsent", (byte) licenseIfAbsent.GetLicenseAsInt32());
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032148, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032149, nameof (AddUser));
      }
    }

    public virtual void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      ILicensingEvent licensingEvent)
    {
      try
      {
        this.TraceEnter(1032151, nameof (UpdateUserStatus));
        this.PrepareStoredProcedure("prc_UpdateUserStatus");
        this.BindGuid("@userId", userId);
        this.BindByte("@status", (byte) status);
        this.BindEventData(licensingEvent);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1032158, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032159, nameof (UpdateUserStatus));
      }
    }

    public virtual IList<Guid> MigrateTestManagerToBasicPlusTestPlans(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032272, nameof (MigrateTestManagerToBasicPlusTestPlans));
        this.PrepareStoredProcedure("prc_MigrateTestManagerToBasicPlusTestPlans");
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new Binders.MigratedUser());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1032280, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032281, nameof (MigrateTestManagerToBasicPlusTestPlans));
      }
    }

    public virtual IList<Guid> RollbackMigrateTestManagerToBasicPlusTestPlans(Guid scopeId)
    {
      try
      {
        this.TraceEnter(1032273, nameof (RollbackMigrateTestManagerToBasicPlusTestPlans));
        this.PrepareStoredProcedure("prc_RollbackMigrateTestManagerToBasicPlusTestPlans");
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new Binders.MigratedUser());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1032282, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1032283, nameof (RollbackMigrateTestManagerToBasicPlusTestPlans));
      }
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ApplicationLicensingComponent.s_sqlExceptionFactories;

    private static Dictionary<int, SqlExceptionFactory> CreateSqlExceptionFactories() => new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1060101,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060106,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060102,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060104,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060105,
        new SqlExceptionFactory(typeof (AccountUserNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AccountUserNotFoundException(sqEr.ExtractString("UserId"), string.Empty)))
      },
      {
        1060107,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060108,
        new SqlExceptionFactory(typeof (AccountUserNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new AccountUserNotFoundException(sqEr.ExtractString("UserId"), string.Empty)))
      },
      {
        1060109,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060401,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060402,
        new SqlExceptionFactory(typeof (LicensingOperationFailException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicensingOperationFailException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      },
      {
        1060110,
        new SqlExceptionFactory(typeof (LicenseScopeNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new LicenseScopeNotFoundException(LicensingResources.LicensingOperationFailed(), (Exception) sqEx)))
      }
    };

    protected virtual void AddUserLicenseRowBinder(ResultCollection rc, Guid accountId) => rc.AddBinder<UserLicense>((ObjectBinder<UserLicense>) new Binders.UserLicenseRowBinder(accountId));

    public IPagedList<UserLicenseCosmosSerializableDocument> GetPagedUserLicenseCosmosDocuments(
      string continuation)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<UserLicenseCosmosSerializableDocument> GetUserLicenseCosmosDocuments() => throw new NotImplementedException();

    public IEnumerable<UserLicenseCosmosSerializableDocument> GetUserLicenseCosmosDocuments(
      IEnumerable<Guid> userIds)
    {
      throw new NotImplementedException();
    }

    public int GetUserLicenseCount() => throw new NotImplementedException();

    public int GetUserLicenseCount(LicensingSource source, int license) => throw new NotImplementedException();

    public IPagedList<UserLicense> GetFilteredUserLicenses(
      string continuationToken,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort)
    {
      throw new NotImplementedException();
    }

    public IPagedList<UserLicenseCosmosSerializableDocument> GetFilteredPagedUserLicenseCosmosDocuments(
      string continuation,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort)
    {
      throw new NotImplementedException();
    }

    public UserLicenseCosmosSerializableDocument UpdateUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      string documentId = null,
      bool withOptimisticConcurrency = false)
    {
      throw new NotImplementedException();
    }

    public UserLicenseCosmosSerializableDocument UpsertUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      bool withOptimisticConcurrency = false)
    {
      throw new NotImplementedException();
    }

    public UserLicenseCosmosSerializableDocument GetUserLicenseCosmosDocument(Guid userId) => throw new NotImplementedException();

    public void DeleteUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      bool withOptimisticConcurrency = false)
    {
      throw new NotImplementedException();
    }

    public UserLicenseCosmosSerializableDocument GetUserLicenseCosmosDocumentByIdAndPreviousId(
      Guid userId)
    {
      throw new NotImplementedException();
    }
  }
}
