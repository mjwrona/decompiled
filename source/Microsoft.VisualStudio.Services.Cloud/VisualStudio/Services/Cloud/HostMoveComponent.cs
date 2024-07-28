// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMoveComponent
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
  internal class HostMoveComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<HostMoveComponent>(1)
    }, "HostMove");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800026,
        new SqlExceptionFactory(typeof (HostDoesNotExistException))
      },
      {
        800103,
        new SqlExceptionFactory(typeof (HostMoveRequestAlreadyExistsException))
      }
    };

    public HostMoveComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.None;

    public void AddHostMoveRequest(HostMoveRequest hostMoveRequest)
    {
      ArgumentUtility.CheckForNull<HostMoveRequest>(hostMoveRequest, nameof (hostMoveRequest));
      this.PrepareStoredProcedure("prc_AddHostMoveRequest");
      this.BindGuid("@hostId", hostMoveRequest.HostId);
      this.BindInt("@targetDatabaseId", hostMoveRequest.TargetDatabaseId);
      this.BindByte("@priority", hostMoveRequest.Priority);
      this.BindByte("@options", (byte) hostMoveRequest.Options);
      this.BindNullableGuid("@jobId", hostMoveRequest.JobId);
      this.ExecuteNonQuery();
    }

    public HostMoveRequest GetHostMoveRequest(Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      this.PrepareStoredProcedure("prc_GetHostMoveRequest");
      this.BindGuid("@hostId", hostId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMoveRequest>((ObjectBinder<HostMoveRequest>) new HostMoveRequestBinder());
        return resultCollection.GetCurrent<HostMoveRequest>().SingleOrDefault<HostMoveRequest>();
      }
    }

    public List<HostMoveRequest> QuerySchedulableHostMoveRequests(
      int count,
      DateTime maxLastUserAccess,
      IEnumerable<int> excludeDatabaseIds)
    {
      this.PrepareStoredProcedure("prc_QuerySchedulableHostMoveRequests");
      this.BindInt("@count", count);
      this.BindDateTime("@maxLastUserAccess", maxLastUserAccess);
      this.BindInt32Table("@excludeDatabaseIds", excludeDatabaseIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMoveRequest>((ObjectBinder<HostMoveRequest>) new HostMoveRequestBinder());
        return resultCollection.GetCurrent<HostMoveRequest>().Items;
      }
    }

    public List<HostMoveRequest> QueryQueuedHostMoveRequests()
    {
      this.PrepareStoredProcedure("prc_QueryQueuedHostMoveRequests");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<HostMoveRequest>((ObjectBinder<HostMoveRequest>) new HostMoveRequestBinder());
        return resultCollection.GetCurrent<HostMoveRequest>().Items;
      }
    }

    public bool SetHostMoveRequestJobId(Guid hostId, Guid jobId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      this.PrepareStoredProcedure("prc_SetHostMoveRequestJobId");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@jobId", jobId);
      return (int) this.ExecuteScalar() == 1;
    }

    public bool DeleteHostMoveRequest(Guid hostId)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      this.PrepareStoredProcedure("prc_DeleteHostMoveRequest");
      this.BindGuid("@hostId", hostId);
      return (int) this.ExecuteScalar() == 1;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) HostMoveComponent.s_sqlExceptionFactories;
  }
}
