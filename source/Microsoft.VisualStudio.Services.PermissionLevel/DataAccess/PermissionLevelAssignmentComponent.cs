// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.DataAccess.PermissionLevelAssignmentComponent
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PermissionLevel.DataAccess
{
  public class PermissionLevelAssignmentComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PermissionLevelAssignmentComponent>(1)
    }, "PermissionLevel");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        3890005,
        new SqlExceptionFactory(typeof (PermissionLevelAssignmentAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelAssignmentAlreadyExistsException(Resources.PermissionLevelAssignmentAlreadyExists((object) sqEr.ExtractString("PermissionLevelAssignmentDefinitionId"), (object) sqEr.ExtractString("ResourceId"), (object) sqEr.ExtractString("SubjectId")))))
      },
      {
        3890006,
        new SqlExceptionFactory(typeof (PermissionLevelAssignmentNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelAssignmentNotFoundException(Resources.PermissionLevelAssignmentNotFound((object) sqEr.ExtractString("PermissionLevelAssignmentDefinitionId"), (object) sqEr.ExtractString("ResourceId"), (object) sqEr.ExtractString("SubjectId")))))
      },
      {
        3890007,
        new SqlExceptionFactory(typeof (PermissionLevelBadRequestException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelBadRequestException(Resources.PermissionLevelAssignmentUpdateScopeMismatch((object) sqEr.ExtractString("PermissionLevelDefinitionId"), (object) sqEr.ExtractInt("ScopeId"), (object) sqEr.ExtractString("NewPermissionLevelDefinitionId"), (object) sqEr.ExtractInt("NewScopeId")))))
      }
    };
    private const string c_area = "PermissionLevel";
    private const string c_layer = "PermissionLevelAssignmentComponent";
    private const string c_serviceName = "PermissionLevel";
    private const int c_maxResourceLength = 1024;

    public PermissionLevelAssignmentComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void Initialize()
    {
      this.PrepareStoredProcedure("PermissionLevel.prc_InstallPermissionLevelAssignmentData");
      this.ExecuteNonQuery();
    }

    public virtual PermissionLevelAssignmentStoreItem CreatePermissionLevelAssignment(
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem)
    {
      ArgumentUtility.CheckForNull<PermissionLevelAssignmentStoreItem>(permissionLevelAssignmentStoreItem, nameof (permissionLevelAssignmentStoreItem));
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignmentStoreItem.HostId, "HostId");
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId, "PermissionLevelDefinitionId");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionLevelAssignmentStoreItem.ResourceId, "ResourceId");
      this.PrepareStoredProcedure("PermissionLevel.prc_CreatePermissionLevelAssignment");
      this.BindGuid("@HostId", permissionLevelAssignmentStoreItem.HostId);
      this.BindGuid("@PermissionLevelDefinitionId", permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId);
      this.BindString("@ResourceId", permissionLevelAssignmentStoreItem.ResourceId, 1024, false, SqlDbType.VarChar);
      this.BindGuid("@SubjectId", permissionLevelAssignmentStoreItem.SubjectId);
      this.BindInt("@ScopeId", permissionLevelAssignmentStoreItem.ScopeId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().FirstOrDefault<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual PermissionLevelAssignmentStoreItem UpdatePermissionLevelAssignment(
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem,
      Guid newDefinitionId,
      int newScopeId)
    {
      ArgumentUtility.CheckForNull<PermissionLevelAssignmentStoreItem>(permissionLevelAssignmentStoreItem, nameof (permissionLevelAssignmentStoreItem));
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignmentStoreItem.HostId, "HostId");
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId, "PermissionLevelDefinitionId");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionLevelAssignmentStoreItem.ResourceId, "ResourceId");
      ArgumentUtility.CheckForEmptyGuid(newDefinitionId, nameof (newDefinitionId));
      this.PrepareStoredProcedure("PermissionLevel.prc_UpdatePermissionLevelAssignment");
      this.BindGuid("@HostId", permissionLevelAssignmentStoreItem.HostId);
      this.BindGuid("@PermissionLevelDefinitionId", permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId);
      this.BindString("@ResourceId", permissionLevelAssignmentStoreItem.ResourceId, 1024, false, SqlDbType.VarChar);
      this.BindGuid("@SubjectId", permissionLevelAssignmentStoreItem.SubjectId);
      this.BindGuid("@NewPermissionLevelDefinitionId", newDefinitionId);
      this.BindInt("@NewScopeId", newScopeId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().FirstOrDefault<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual PermissionLevelAssignmentStoreItem QueryPermissionLevelAssignment(
      Guid hostId,
      Guid permissionLevelDefinitionId,
      string resourceId,
      Guid subjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckForEmptyGuid(permissionLevelDefinitionId, nameof (permissionLevelDefinitionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      ArgumentUtility.CheckForEmptyGuid(subjectId, nameof (subjectId));
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelAssignment");
      this.BindGuid("@HostId", hostId);
      this.BindGuid("@PermissionLevelDefinitionId", permissionLevelDefinitionId);
      this.BindString("@ResourceId", resourceId, 1024, false, SqlDbType.VarChar);
      this.BindGuid("@SubjectId", subjectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().FirstOrDefault<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual IEnumerable<PermissionLevelAssignmentStoreItem> QueryPermissionLevelAssignmentByDefinitionId(
      Guid hostId,
      Guid permissionLevelDefinitionId,
      string resourceId,
      int startSequenceId,
      int pageSize,
      out int? sequenceId)
    {
      sequenceId = new int?();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckForEmptyGuid(permissionLevelDefinitionId, nameof (permissionLevelDefinitionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelAssignmentByDefinitionId");
      this.BindGuid("@HostId", hostId);
      this.BindGuid("@PermissionLevelDefinitionId", permissionLevelDefinitionId);
      this.BindString("@ResourceId", resourceId, 1024, false, SqlDbType.VarChar);
      this.BindInt("@startSequenceId", startSequenceId);
      this.BindInt("@pageSize", pageSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return (IEnumerable<PermissionLevelAssignmentStoreItem>) resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().ToList<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual IEnumerable<PermissionLevelAssignmentStoreItem> QueryPermissionLevelAssignmentByScope(
      Guid hostId,
      string resourceId,
      int scopeId,
      out int? sequenceId,
      int startSequenceId,
      int pageSize)
    {
      sequenceId = new int?();
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelAssignmentByScope");
      this.BindGuid("@HostId", hostId);
      this.BindString("@ResourceId", resourceId, 1024, false, SqlDbType.VarChar);
      this.BindInt("@ScopeId", scopeId);
      this.BindInt("@startSequenceId", startSequenceId);
      this.BindInt("@pageSize", pageSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return (IEnumerable<PermissionLevelAssignmentStoreItem>) resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().ToList<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual IEnumerable<PermissionLevelAssignmentStoreItem> QueryPermissionLevelAssignmentByScope(
      Guid hostId,
      string resourceId,
      int scopeId,
      Guid subjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceId, nameof (resourceId));
      ArgumentUtility.CheckForEmptyGuid(subjectId, nameof (subjectId));
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelAssignmentByScopeAndSubject");
      this.BindGuid("@HostId", hostId);
      this.BindString("@ResourceId", resourceId, 1024, false, SqlDbType.VarChar);
      this.BindInt("@ScopeId", scopeId);
      this.BindGuid("@SubjectId", subjectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return (IEnumerable<PermissionLevelAssignmentStoreItem>) resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().ToList<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual IEnumerable<PermissionLevelAssignmentStoreItem> QueryPermissionLevelAssignmentBySubject(
      Guid hostId,
      Guid subjectId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      ArgumentUtility.CheckForEmptyGuid(subjectId, nameof (subjectId));
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelAssignmentBySubjectId");
      this.BindGuid("@HostId", hostId);
      this.BindGuid("@SubjectId", subjectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelAssignmentStoreItem>(this.CreatePermissionLevelAssignmentColumns());
        return (IEnumerable<PermissionLevelAssignmentStoreItem>) resultCollection.GetCurrent<PermissionLevelAssignmentStoreItem>().ToList<PermissionLevelAssignmentStoreItem>();
      }
    }

    public virtual bool DeletePermissionLevelAssignment(
      PermissionLevelAssignmentStoreItem permissionLevelAssignmentStoreItem)
    {
      ArgumentUtility.CheckForNull<PermissionLevelAssignmentStoreItem>(permissionLevelAssignmentStoreItem, nameof (permissionLevelAssignmentStoreItem));
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignmentStoreItem.HostId, "HostId");
      ArgumentUtility.CheckForEmptyGuid(permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId, "PermissionLevelDefinitionId");
      ArgumentUtility.CheckStringForNullOrEmpty(permissionLevelAssignmentStoreItem.ResourceId, "ResourceId");
      this.PrepareStoredProcedure("PermissionLevel.prc_DeletePermissionLevelAssignment");
      this.BindGuid("@HostId", permissionLevelAssignmentStoreItem.HostId);
      this.BindGuid("@PermissionLevelDefinitionId", permissionLevelAssignmentStoreItem.PermissionLevelDefinitionId);
      this.BindString("@ResourceId", permissionLevelAssignmentStoreItem.ResourceId, 1024, false, SqlDbType.VarChar);
      this.BindGuid("@SubjectId", permissionLevelAssignmentStoreItem.SubjectId);
      this.ExecuteNonQuery();
      return true;
    }

    protected virtual ObjectBinder<PermissionLevelAssignmentStoreItem> CreatePermissionLevelAssignmentColumns() => (ObjectBinder<PermissionLevelAssignmentStoreItem>) new PermissionLevelAssignmentBinder();

    internal static void ValidatePermissionLevelAssignmentResource(string resource)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckForOutOfRange(resource.Length, nameof (resource), 1, 1024);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PermissionLevelAssignmentComponent.s_sqlExceptionFactories;
  }
}
