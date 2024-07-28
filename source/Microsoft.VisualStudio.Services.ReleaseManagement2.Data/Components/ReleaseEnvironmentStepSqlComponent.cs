// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseEnvironmentStepSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseEnvironmentStepSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[18]
    {
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent3>(3),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent4>(4),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent5>(5),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent6>(6),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent7>(7),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent8>(8),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent9>(9),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent10>(10),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent11>(11),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent12>(12),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent13>(13),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent14>(14),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent15>(15),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent16>(16),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent17>(17),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent18>(18),
      (IComponentCreator) new ComponentCreator<ReleaseEnvironmentStepSqlComponent19>(19)
    }, "ReleaseManagementReleaseEnvironmentStep", "ReleaseManagement");

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseEnvironmentStepBinder GetReleaseEnvironmentStepBinder() => new ReleaseEnvironmentStepBinder((ReleaseManagementSqlResourceComponentBase) this);

    public virtual IEnumerable<ReleaseEnvironmentStep> AddReleaseEnvironmentSteps(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> environmentStepsToAdd,
      bool handleParallelApprovers,
      Guid changedBy)
    {
      if (environmentStepsToAdd == null)
        return (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
      List<ReleaseEnvironmentStep> releaseEnvironmentStepList = new List<ReleaseEnvironmentStep>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in environmentStepsToAdd)
      {
        this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_Add", projectId);
        this.BindInt("releaseId", releaseEnvironmentStep.ReleaseId);
        this.BindToReleaseEnvironmentStepTable("releaseEnvironmentStep", releaseEnvironmentStep);
        releaseEnvironmentStepList.Add(this.GetReleaseEnvironmentStepsObject().FirstOrDefault<ReleaseEnvironmentStep>());
      }
      return (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentStepList;
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> GetReleaseEnvironmentStep(
      Guid projectId,
      int releaseEnvironmentStepId,
      bool includeHistory)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_Get", projectId);
      this.BindInt(nameof (releaseEnvironmentStepId), releaseEnvironmentStepId);
      List<ReleaseEnvironmentStep> releaseEnvironmentStep = new List<ReleaseEnvironmentStep>();
      IEnumerable<ReleaseEnvironmentStep> environmentStepsObject = this.GetReleaseEnvironmentStepsObject();
      releaseEnvironmentStep.AddRange(environmentStepsObject);
      if (includeHistory)
      {
        IEnumerable<ReleaseEnvironmentStep> approvalHistory = this.GetApprovalHistory(projectId, (IEnumerable<int>) new List<int>()
        {
          releaseEnvironmentStepId
        });
        releaseEnvironmentStep.AddRange(approvalHistory);
      }
      return (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentStep;
    }

    public virtual ReleaseEnvironmentStep UpdateReleaseEnvironmentStep(
      Guid projectId,
      ReleaseEnvironmentStep environmentStep)
    {
      if (environmentStep == null)
        throw new ArgumentNullException(nameof (environmentStep));
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_Update", projectId);
      this.BindToReleaseEnvironmentStepTable("releaseEnvironmentStep", environmentStep);
      return this.GetReleaseEnvironmentStepsObject().FirstOrDefault<ReleaseEnvironmentStep>();
    }

    public virtual ReleaseEnvironmentStep HandleEnvironmentDeployJobStarted(
      Guid projectId,
      int releaseId,
      int releaseStepId,
      int releaseDeployPhaseId)
    {
      return (ReleaseEnvironmentStep) null;
    }

    public virtual IEnumerable<int> GetAllPendingApprovalIds(Guid projectId, int days) => (IEnumerable<int>) new List<int>();

    public virtual void HandleEnvironmentPipelineStatusUpdate(
      Guid projectId,
      int releaseId,
      int releaseStepId,
      DeploymentOperationStatus operationStatus)
    {
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> UpdatePendingReleaseEnvironmentStep(
      Guid projectId,
      ReleaseEnvironmentStep environmentStep)
    {
      if (environmentStep == null)
        throw new ArgumentNullException(nameof (environmentStep));
      this.PrepareStoredProcedure("Release.prc_ReleaseEnvironmentStep_UpdatePending", projectId);
      this.BindToReleaseEnvironmentStepTable("releaseEnvironmentStep", environmentStep);
      return this.GetReleaseEnvironmentStepsObject();
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> ListReleaseApprovalSteps(
      Guid projectId,
      IEnumerable<int> releaseIds,
      ReleaseEnvironmentStepStatus? status,
      Guid? approverId)
    {
      return this.ListReleaseApprovalSteps(projectId, releaseIds, status, approverId, EnvironmentStepType.All, new Guid?(), 0, 0, ReleaseQueryOrder.IdDescending, new DateTime?(), new DateTime?());
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> ListReleaseApprovalSteps(
      Guid projectId,
      IEnumerable<int> releaseIds,
      ReleaseEnvironmentStepStatus? status,
      Guid? approverId,
      EnvironmentStepType typeFilter,
      Guid? actualApproverId,
      int top,
      int continuationToken,
      ReleaseQueryOrder queryOrder,
      DateTime? minModifiedTime,
      DateTime? maxModifiedTime)
    {
      if (releaseIds == null || releaseIds.Count<int>() == 0)
        releaseIds = (IEnumerable<int>) new List<int>()
        {
          0
        };
      List<ReleaseEnvironmentStep> releaseEnvironmentStepList = new List<ReleaseEnvironmentStep>();
      foreach (int releaseId in releaseIds)
      {
        this.PrepareStoredProcedure("Release.prc_GetApprovals", projectId);
        this.BindInt("releaseId", releaseId);
        this.BindNullableGuid("approverIdFilter", approverId ?? Guid.Empty);
        this.BindInt("statusFilter", (int) status.GetValueOrDefault());
        releaseEnvironmentStepList.AddRange(this.GetReleaseEnvironmentStepsObject());
      }
      return (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentStepList;
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> GetApprovalHistory(
      Guid projectId,
      IEnumerable<int> approvalStepId)
    {
      throw new NotSupportedException();
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> GetReleaseEnvironmentApprovalSteps(
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int rank,
      int trialNumber)
    {
      Guid projectId1 = projectId;
      List<int> releaseIds = new List<int>();
      releaseIds.Add(releaseId);
      ReleaseEnvironmentStepStatus? status = new ReleaseEnvironmentStepStatus?();
      Guid? approverId = new Guid?();
      return this.ListReleaseApprovalSteps(projectId1, (IEnumerable<int>) releaseIds, status, approverId).Where<ReleaseEnvironmentStep>((System.Func<ReleaseEnvironmentStep, bool>) (s => s.ReleaseEnvironmentId == releaseEnvironmentId && s.Rank == rank && s.TrialNumber == trialNumber));
    }

    public virtual void BindToReleaseEnvironmentStepsTable(
      string parameterName,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      this.BindReleaseEnvironmentStepTable3(parameterName, releaseEnvironmentSteps);
    }

    public virtual void BindToReleaseEnvironmentStepTable(
      string parameterName,
      ReleaseEnvironmentStep releaseEnvironmentStep)
    {
      this.BindReleaseEnvironmentStepTable(parameterName, releaseEnvironmentStep);
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not required")]
    protected IEnumerable<ReleaseEnvironmentStep> GetReleaseEnvironmentStepsObject()
    {
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseEnvironmentStep>((ObjectBinder<ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        return (IEnumerable<ReleaseEnvironmentStep>) resultCollection.GetCurrent<ReleaseEnvironmentStep>().Items;
      }
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> UpdateReleaseEnvironmentSteps(
      Guid projectId,
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps)
    {
      if (releaseEnvironmentSteps == null)
        throw new ArgumentNullException(nameof (releaseEnvironmentSteps));
      List<ReleaseEnvironmentStep> releaseEnvironmentStepList = new List<ReleaseEnvironmentStep>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in releaseEnvironmentSteps)
      {
        try
        {
          releaseEnvironmentStepList.AddRange(this.UpdatePendingReleaseEnvironmentStep(projectId, releaseEnvironmentStep));
        }
        catch (ApprovalUpdateException ex)
        {
        }
        catch (DeploymentOperationStatusAlreadyUpdatedException ex)
        {
        }
      }
      return (IEnumerable<ReleaseEnvironmentStep>) releaseEnvironmentStepList;
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> GetPendingApprovalsForReleaseDefinitions(
      Guid projectId,
      IEnumerable<int> releaseDefinitionIds,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      return (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
    }

    public virtual IEnumerable<ReleaseEnvironmentStep> ListApprovalsForAnIdentity(
      Guid projectId,
      Guid approverId,
      ReleaseEnvironmentStepStatus statusFilter,
      int maxApprovals,
      DateTime minModifiedTime,
      DateTime maxModifiedTime)
    {
      return (IEnumerable<ReleaseEnvironmentStep>) new List<ReleaseEnvironmentStep>();
    }
  }
}
