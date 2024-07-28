// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ManualInterventionSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ManualInterventionSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ManualInterventionSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ManualInterventionSqlComponent2>(2)
    }, "ReleaseManagementManualIntervention", "ReleaseManagement");

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ManualInterventionBinder GetManualInterventionBinder() => new ManualInterventionBinder();

    public virtual ManualIntervention CreateManualIntervention(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      TaskActivityData taskActivityData,
      string instructions,
      Guid changedBy)
    {
      this.PrepareStoredProcedure("Release.prc_CreateManualIntervention", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (releaseEnvironmentId), releaseEnvironmentId);
      this.BindInt(nameof (releaseDeployPhaseId), releaseDeployPhaseId);
      this.BindString("TaskActivityData", ServerModelUtility.ToString((object) taskActivityData), 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionBinder());
        return resultCollection.GetCurrent<ManualIntervention>().Items.FirstOrDefault<ManualIntervention>();
      }
    }

    public virtual ManualIntervention GetManualIntervention(
      Guid projectId,
      int releaseId,
      int manualInterventionId)
    {
      this.PrepareStoredProcedure("Release.prc_GetManualIntervention", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (manualInterventionId), manualInterventionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionBinder());
        return resultCollection.GetCurrent<ManualIntervention>().Items.FirstOrDefault<ManualIntervention>();
      }
    }

    public virtual ManualIntervention UpdateManualIntervention(
      Guid projectId,
      int releaseId,
      int manualInterventionId,
      Guid approvedBy,
      ManualInterventionStatus status,
      string comments)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateManualIntervention", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (manualInterventionId), manualInterventionId);
      this.BindGuid(nameof (approvedBy), approvedBy);
      this.BindByte("environmentStatus", (byte) status);
      this.BindString(nameof (comments), comments, 4000, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionBinder());
        return resultCollection.GetCurrent<ManualIntervention>().Items.FirstOrDefault<ManualIntervention>();
      }
    }

    public virtual IList<ManualIntervention> GetManualInterventionsForRelease(
      Guid projectId,
      int releaseId)
    {
      this.PrepareStoredProcedure("Release.prc_GetManualInterventionsForRelease", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ManualIntervention>((ObjectBinder<ManualIntervention>) this.GetManualInterventionBinder());
        return (IList<ManualIntervention>) resultCollection.GetCurrent<ManualIntervention>().Items;
      }
    }
  }
}
