// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SUMoveComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class SUMoveComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<SUMoveComponent>(1),
      (IComponentCreator) new ComponentCreator<SUMoveComponent2>(2),
      (IComponentCreator) new ComponentCreator<SUMoveComponent3>(3)
    }, "SUMove");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800311,
        new SqlExceptionFactory(typeof (ScaleUnitDataTransferAlreadyExistsException))
      },
      {
        800310,
        new SqlExceptionFactory(typeof (RoleInstanceSynchronizationAlreadyExistsException))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SUMoveComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "SUMove";

    public virtual ScaleUnitDataTransfer CreateScaleUnitDataTransfer(
      ScaleUnitDataTransfer suDataTransfer)
    {
      this.PrepareStoredProcedure("SUMove.prc_CreateScaleUnitDataTransfer");
      this.BindString("@sourceStorageAccount", suDataTransfer.SourceStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@targetStorageAccount", suDataTransfer.TargetStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@primaryId", suDataTransfer.PrimaryId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@subsetId", suDataTransfer.SubsetId, 250, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@itemType", suDataTransfer.ItemType, 10, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindNullableGuid("@jobThread", suDataTransfer.JobThread);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScaleUnitDataTransfer>((ObjectBinder<ScaleUnitDataTransfer>) new ScaleUnitDataTransferColumns());
        return resultCollection.GetCurrent<ScaleUnitDataTransfer>().Items.FirstOrDefault<ScaleUnitDataTransfer>();
      }
    }

    public virtual ScaleUnitDataTransfer GetScaleUnitDataTransfer(
      ScaleUnitDataTransfer suDataTransfer)
    {
      this.PrepareStoredProcedure("SUMove.prc_GetScaleUnitDataTransfer");
      this.BindString("@sourceStorageAccount", suDataTransfer.SourceStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@targetStorageAccount", suDataTransfer.TargetStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@primaryId", suDataTransfer.PrimaryId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@subsetId", suDataTransfer.SubsetId, 250, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@itemType", suDataTransfer.ItemType, 10, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScaleUnitDataTransfer>((ObjectBinder<ScaleUnitDataTransfer>) new ScaleUnitDataTransferColumns());
        return resultCollection.GetCurrent<ScaleUnitDataTransfer>().Items.FirstOrDefault<ScaleUnitDataTransfer>();
      }
    }

    public virtual RoleInstanceSynchronization CreateRoleInstanceSynchronization(
      RoleInstanceSynchronization roleInstanceSynchronization)
    {
      this.PrepareStoredProcedure("SUMove.prc_CreateRoleInstanceSynchronization");
      this.BindString("@roleInstance", roleInstanceSynchronization.RoleInstance, 50, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindNullableGuid("@hostId", roleInstanceSynchronization.HostId);
      this.BindString("@currentStatus", roleInstanceSynchronization.CurrentStatus, 500, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@previousStatus", roleInstanceSynchronization.PreviousStatus, 500, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RoleInstanceSynchronization>((ObjectBinder<RoleInstanceSynchronization>) new RoleInstanceSynchronizationColumns());
        return resultCollection.GetCurrent<RoleInstanceSynchronization>().Items.FirstOrDefault<RoleInstanceSynchronization>();
      }
    }

    public virtual void UpdateScaleUnitDataTransfer(ScaleUnitDataTransfer scaleUnitDataTransfer)
    {
      this.PrepareStoredProcedure("SUMove.prc_UpdateScaleUnitDataTransfer");
      this.BindString("@sourceStorageAccount", scaleUnitDataTransfer.SourceStorageAccount, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@targetStorageAccount", scaleUnitDataTransfer.TargetStorageAccount, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@primaryId", scaleUnitDataTransfer.PrimaryId, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@subsetId", scaleUnitDataTransfer.SubsetId, 250, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@itemType", scaleUnitDataTransfer.ItemType, 10, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@itemId", scaleUnitDataTransfer.ItemId, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindNullableLong("@totalEntriesTransferred", scaleUnitDataTransfer.TotalEntriesTransferred);
      this.BindNullableDateTime("@startedProcessing", scaleUnitDataTransfer.StartedProcessing);
      this.BindNullableGuid("@jobThread", scaleUnitDataTransfer.JobThread);
      this.BindBoolean("@completedProcessing", scaleUnitDataTransfer.CompletedProcessing);
      this.ExecuteNonQuery();
    }

    public virtual void UpdateRoleInstanceSynchronization(
      RoleInstanceSynchronization roleInstanceSynchronization)
    {
      this.PrepareStoredProcedure("SUMove.prc_UpdateRoleInstanceSychronization");
      this.BindString("@roleInstance", roleInstanceSynchronization.RoleInstance, 50, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindNullableGuid("@hostId", roleInstanceSynchronization.HostId);
      this.BindString("@currentStatus", roleInstanceSynchronization.CurrentStatus, 500, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual bool DeleteScaleUnitDataTransfer(ScaleUnitDataTransfer suDataTransfer)
    {
      this.PrepareStoredProcedure("SUMove.prc_DeleteScaleUnitDataTransfer");
      this.BindString("@sourceStorageAccount", suDataTransfer.SourceStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@targetStorageAccount", suDataTransfer.TargetStorageAccount, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@primaryId", suDataTransfer.PrimaryId, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@subsetId", suDataTransfer.SubsetId, 250, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindString("@itemType", suDataTransfer.ItemType, 10, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      return (int) this.ExecuteScalar() > 0;
    }

    public virtual bool DeleteRoleInstanceSynchronization(
      RoleInstanceSynchronization roleInstanceSynchronization)
    {
      this.PrepareStoredProcedure("SUMove.prc_DeleteRoleInstanceSynchronization");
      this.BindString("@roleInstance", roleInstanceSynchronization.RoleInstance, 50, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
      this.BindNullableGuid("@hostId", roleInstanceSynchronization.HostId);
      return (int) this.ExecuteScalar() > 0;
    }

    public virtual void CleanCompletedScaleUnitDataTransfer()
    {
      this.PrepareStoredProcedure("SUMove.prc_CleanCompletedScaleUnitDataTransfer");
      this.ExecuteNonQuery();
    }

    public virtual void CleanRoleInstanceSynchronization(Guid? hostId)
    {
      this.PrepareStoredProcedure("SUMove.prc_CleanRoleInstanceSynchronization");
      this.BindNullableGuid("@hostId", hostId);
      this.ExecuteNonQuery();
    }

    public virtual List<RoleInstanceSynchronization> GetAllRoleInstanceSynchronization(Guid? hostId)
    {
      this.PrepareStoredProcedure("SUMove.prc_GetAllRoleInstanceSynchronization");
      this.BindNullableGuid("@hostId", hostId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RoleInstanceSynchronization>((ObjectBinder<RoleInstanceSynchronization>) new RoleInstanceSynchronizationColumns());
        return resultCollection.GetCurrent<RoleInstanceSynchronization>().Items;
      }
    }

    public virtual List<RoleInstanceSynchronization> GetTopRoleInstanceSynchronization()
    {
      this.PrepareStoredProcedure("SUMove.prc_GetTopRoleInstanceSynchronization");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RoleInstanceSynchronization>((ObjectBinder<RoleInstanceSynchronization>) new RoleInstanceSynchronizationColumns());
        return resultCollection.GetCurrent<RoleInstanceSynchronization>().Items;
      }
    }

    public virtual List<ScaleUnitDataTransfer> GetNonRunningScaleUnitDataTransfers(
      int heartbeatTimeout)
    {
      this.PrepareStoredProcedure("SUMove.prc_GetNonRunningScaleUnitDataTransfers");
      this.BindInt("@heartbeatTimeout", heartbeatTimeout * -1);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScaleUnitDataTransfer>((ObjectBinder<ScaleUnitDataTransfer>) new ScaleUnitDataTransferColumns());
        return resultCollection.GetCurrent<ScaleUnitDataTransfer>().Items;
      }
    }

    public virtual List<ScaleUnitDataTransfer> GetTopScaleUnitDataTransfers()
    {
      this.PrepareStoredProcedure("SUMove.prc_GetTopScaleUnitDataTransfers");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ScaleUnitDataTransfer>((ObjectBinder<ScaleUnitDataTransfer>) new ScaleUnitDataTransferColumns());
        return resultCollection.GetCurrent<ScaleUnitDataTransfer>().Items;
      }
    }
  }
}
