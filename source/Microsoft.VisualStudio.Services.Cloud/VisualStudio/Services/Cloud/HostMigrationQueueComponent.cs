// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationQueueComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationQueueComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<HostMigrationQueueComponent>(1),
      (IComponentCreator) new ComponentCreator<HostMigrationQueueComponent2>(2),
      (IComponentCreator) new ComponentCreator<HostMigrationQueueComponent3>(3),
      (IComponentCreator) new ComponentCreator<HostMigrationQueueComponent4>(4),
      (IComponentCreator) new ComponentCreator<HostMigrationQueueComponent5>(5)
    }, "HostMigrationQueue");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800308,
        new SqlExceptionFactory(typeof (HostMigrationRequestAlreadyExistsException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) HostMigrationQueueComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "HostMigrationQueue";

    public void AddQueueRequest(HostMigrationRequest hostMigrationRequest)
    {
      ArgumentUtility.CheckForNull<HostMigrationRequest>(hostMigrationRequest, nameof (hostMigrationRequest));
      this.PrepareStoredProcedure("Migration.prc_AddQueueRequest");
      this.BindAddQueueRequest(hostMigrationRequest);
      this.ExecuteNonQuery();
    }

    public virtual void AddQueueRequests(
      IEnumerable<HostMigrationRequest> hostMigrationRequests)
    {
      ArgumentUtility.CheckForNull<IEnumerable<HostMigrationRequest>>(hostMigrationRequests, nameof (hostMigrationRequests));
      foreach (HostMigrationRequest migrationRequest in hostMigrationRequests)
        this.AddQueueRequest(migrationRequest);
    }

    protected virtual void BindAddQueueRequest(HostMigrationRequest hostMigrationRequest)
    {
      this.BindGuid("@hostId", hostMigrationRequest.HostId);
      this.BindString("@targetInstanceName", hostMigrationRequest.TargetInstanceName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("@priority", hostMigrationRequest.Priority);
      this.BindByte("@options", (byte) hostMigrationRequest.Options);
      if (hostMigrationRequest.TargetDatabaseId <= 0)
        return;
      this.BindInt("@targetDatabaseId", hostMigrationRequest.TargetDatabaseId);
    }

    public HostMigrationRequest GetQueueRequest(Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      this.PrepareStoredProcedure("Migration.prc_GetQueueRequest");
      this.BindGuid("@hostId", hostId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMigrationRequest>((ObjectBinder<HostMigrationRequest>) new HostMigrationRequestBinder());
        return resultCollection.GetCurrent<HostMigrationRequest>().SingleOrDefault<HostMigrationRequest>();
      }
    }

    public virtual List<HostMigrationRequest> GetSchedulableQueueRequest(
      DateTime maxLastUserAccess,
      int numberOfRequests,
      string[] excludedTargetInstances)
    {
      return new List<HostMigrationRequest>();
    }

    public virtual List<HostMigrationRequest> GetSchedulableQueueRequestsSegmented(
      DateTime maxLastUserAccess,
      int numberOfRequests,
      int maxPriority,
      Guid minHostId,
      string[] excludedTargetInstances)
    {
      return this.GetSchedulableQueueRequest(maxLastUserAccess, numberOfRequests, excludedTargetInstances);
    }

    public List<HostMigrationRequest> QueryQueueRequests()
    {
      this.PrepareStoredProcedure("Migration.prc_QueryQueueRequests");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMigrationRequest>((ObjectBinder<HostMigrationRequest>) new HostMigrationRequestBinder());
        return resultCollection.GetCurrent<HostMigrationRequest>().Items;
      }
    }

    public virtual List<HostMigrationRequest> GetRunningRequests() => new List<HostMigrationRequest>();

    public bool SetQueueRequestMigrationId(Guid hostId, Guid migrationId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      this.PrepareStoredProcedure("Migration.prc_SetQueueRequestMigrationId");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@migrationId", migrationId);
      return (int) this.ExecuteScalar() == 1;
    }

    public bool SetQueueRequestDriverJobId(Guid hostId, Guid? driverJobId)
    {
      if (driverJobId.HasValue)
        ArgumentUtility.CheckForEmptyGuid(driverJobId.Value, nameof (driverJobId));
      this.PrepareStoredProcedure("Migration.prc_SetQueueRequestDriverJobId");
      this.BindGuid("@hostId", hostId);
      this.BindNullableGuid("@driverJobId", driverJobId);
      return (int) this.ExecuteScalar() == 1;
    }

    public bool DeleteQueueRequest(Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      this.PrepareStoredProcedure("Migration.prc_DeleteQueueRequest");
      this.BindGuid("@hostId", hostId);
      return (int) this.ExecuteScalar() == 1;
    }
  }
}
