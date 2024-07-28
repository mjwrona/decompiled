// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.AnalyticsSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class AnalyticsSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsSqlComponent>(1)
    }, "ReleaseManagementAnalytics", "ReleaseManagement");

    public virtual IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release> QueryReleasesByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleasesByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>((ObjectBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>) this.GetReleaseBinder());
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release>().Items.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release>((System.Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release>) (r => r.ToAnalyticsModel())).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release>();
      }
    }

    public virtual IList<ReleaseArtifactSource> QueryReleaseArtifactSourcesByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseArtifactSourcesByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PipelineArtifactSource>((ObjectBinder<PipelineArtifactSource>) this.GetReleaseArtifactSourceBinder());
        return (IList<ReleaseArtifactSource>) resultCollection.GetCurrent<PipelineArtifactSource>().Items.Select<PipelineArtifactSource, ReleaseArtifactSource>((System.Func<PipelineArtifactSource, ReleaseArtifactSource>) (ras => ras.ToAnalyticsModel())).ToList<ReleaseArtifactSource>();
      }
    }

    public virtual IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition> QueryReleaseDefinitionsByChangedDate(
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseDefinitionsByChangedDate");
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>((ObjectBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition>().Items.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition>((System.Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition>) (rd => rd.ToAnalyticsModel())).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseDefinition>();
      }
    }

    public virtual IList<ReleaseDeployment> QueryReleaseDeploymentsByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseDeploymentsByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Deployment>((ObjectBinder<Deployment>) this.GetDeploymentBinder());
        return (IList<ReleaseDeployment>) resultCollection.GetCurrent<Deployment>().Items.Select<Deployment, ReleaseDeployment>((System.Func<Deployment, ReleaseDeployment>) (d => d.ToAnalyticsModel())).ToList<ReleaseDeployment>();
      }
    }

    public virtual IList<ReleaseDeploymentGate> QueryReleaseDeploymentGatesByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseDeploymentGatesByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DeploymentGate>((ObjectBinder<DeploymentGate>) this.GetDeploymentGateBinder());
        return (IList<ReleaseDeploymentGate>) resultCollection.GetCurrent<DeploymentGate>().Items.Select<DeploymentGate, ReleaseDeploymentGate>((System.Func<DeploymentGate, ReleaseDeploymentGate>) (dg => dg.ToAnalyticsModel())).ToList<ReleaseDeploymentGate>();
      }
    }

    public virtual IList<ReleaseDeploymentRunPlan> QueryReleaseDeploymentRunPlansByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseDeploymentRunPlansByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDeployPhase>((ObjectBinder<ReleaseDeployPhase>) this.GetDeployPhaseBinder());
        return (IList<ReleaseDeploymentRunPlan>) resultCollection.GetCurrent<ReleaseDeployPhase>().Items.Select<ReleaseDeployPhase, ReleaseDeploymentRunPlan>((System.Func<ReleaseDeployPhase, ReleaseDeploymentRunPlan>) (d => d.ToAnalyticsModel())).ToList<ReleaseDeploymentRunPlan>();
      }
    }

    public virtual IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment> QueryReleaseEnvironmentsByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseEnvironmentsByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>((ObjectBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>) this.GetReleaseEnvironmentBinder());
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment>().Items.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment>((System.Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment>) (re => re.ToAnalyticsModel())).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironment>();
      }
    }

    public virtual IList<ReleaseEnvironmentDefinition> QueryReleaseEnvironmentDefinitionsByChangedDate(
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseEnvironmentDefinitionsByChangedDate");
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentBinder());
        return (IList<ReleaseEnvironmentDefinition>) resultCollection.GetCurrent<DefinitionEnvironment>().Items.Select<DefinitionEnvironment, ReleaseEnvironmentDefinition>((System.Func<DefinitionEnvironment, ReleaseEnvironmentDefinition>) (de => de.ToAnalyticsModel())).ToList<ReleaseEnvironmentDefinition>();
      }
    }

    public virtual IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep> QueryReleaseEnvironmentStepsByChangedDate(
      int dataspaceId,
      int batchSize,
      DateTime fromDate)
    {
      this.PrepareStoredProcedure("Release.prc_QueryReleaseEnvironmentStepsByChangedDate");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (batchSize), batchSize);
      this.BindDateTime("fromChangedDate", fromDate, true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep>((ObjectBinder<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep>) this.GetReleaseEnvironmentStepBinder());
        return (IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep>().Items.Select<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep>((System.Func<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironmentStep, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep>) (s => s.ToAnalyticsModel())).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.ReleaseEnvironmentStep>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual ReleaseBinder GetReleaseBinder() => (ReleaseBinder) new ReleaseBinder2(this.RequestContext, (ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual ReleaseArtifactSourceBinder GetReleaseArtifactSourceBinder() => (ReleaseArtifactSourceBinder) new ReleaseArtifactSourceBinder6((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual ReleaseDefinitionBinder GetReleaseDefinitionBinder() => (ReleaseDefinitionBinder) new ReleaseDefinitionBinder5((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual DeploymentBinder GetDeploymentBinder() => (DeploymentBinder) new DeploymentBinder4((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual DeploymentGateBinder GetDeploymentGateBinder() => (DeploymentGateBinder) new DeploymentGateBinder2((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual ReleaseDeployPhaseBinder GetDeployPhaseBinder() => new ReleaseDeployPhaseBinder((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual DefinitionEnvironmentBinder GetDefinitionEnvironmentBinder() => (DefinitionEnvironmentBinder) new DefinitionEnvironmentBinder3((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual ReleaseEnvironmentBinder GetReleaseEnvironmentBinder() => (ReleaseEnvironmentBinder) new ReleaseEnvironmentBinder3((ReleaseManagementSqlResourceComponentBase) this);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual ReleaseEnvironmentStepBinder GetReleaseEnvironmentStepBinder() => new ReleaseEnvironmentStepBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
