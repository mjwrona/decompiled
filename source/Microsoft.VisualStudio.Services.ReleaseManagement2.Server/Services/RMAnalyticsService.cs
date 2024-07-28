// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.RMAnalyticsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class RMAnalyticsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<Release> QueryReleasesByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleasesByChangedDate", 1971074))
      {
        Func<AnalyticsSqlComponent, IList<Release>> action = (Func<AnalyticsSqlComponent, IList<Release>>) (component => component.QueryReleasesByChangedDate(dataspaceId, batchSize, fromDate));
        IList<Release> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<Release>>(action);
        Release release = source.OrderBy<Release, DateTime?>((Func<Release, DateTime?>) (r => r.ModifiedOn)).LastOrDefault<Release>();
        if (release != null)
          toDate = release.ModifiedOn.Value;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<Release> QueryReleaseKeysOnlyByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      return (IList<Release>) this.QueryReleasesByChangedDate(requestContext, dataspaceId, batchSize, fromDate, out toDate).Select<Release, Release>((Func<Release, Release>) (r => new Release()
      {
        ProjectGuid = r.ProjectGuid,
        ReleaseId = r.ReleaseId
      })).ToList<Release>();
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseArtifactSource> QueryReleaseArtifactSourcesByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseArtifactSourcesByChangedDate", 1971085))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseArtifactSource>> action = (Func<AnalyticsSqlComponent, IList<ReleaseArtifactSource>>) (component => component.QueryReleaseArtifactSourcesByChangedDate(dataspaceId, batchSize, fromDate));
        IList<ReleaseArtifactSource> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseArtifactSource>>(action);
        ReleaseArtifactSource releaseArtifactSource = source.OrderBy<ReleaseArtifactSource, DateTime>((Func<ReleaseArtifactSource, DateTime>) (r => r.CreatedOn)).LastOrDefault<ReleaseArtifactSource>();
        if (releaseArtifactSource != null)
          toDate = releaseArtifactSource.CreatedOn;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseDefinition> QueryReleaseDefinitionsByChangedDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseDefinitionsByChangedDate", 1971070))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseDefinition>> action = (Func<AnalyticsSqlComponent, IList<ReleaseDefinition>>) (component => component.QueryReleaseDefinitionsByChangedDate(batchSize, fromDate));
        IList<ReleaseDefinition> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseDefinition>>(action);
        ReleaseDefinition releaseDefinition = source.OrderBy<ReleaseDefinition, DateTime?>((Func<ReleaseDefinition, DateTime?>) (rd => rd.ModifiedOn)).LastOrDefault<ReleaseDefinition>();
        if (releaseDefinition != null)
          toDate = releaseDefinition.ModifiedOn.Value;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseDefinition> QueryReleaseDefinitionKeysOnlyByChangedDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      return (IList<ReleaseDefinition>) this.QueryReleaseDefinitionsByChangedDate(requestContext, batchSize, fromDate, out toDate).Select<ReleaseDefinition, ReleaseDefinition>((Func<ReleaseDefinition, ReleaseDefinition>) (r => new ReleaseDefinition()
      {
        ProjectGuid = r.ProjectGuid,
        ReleaseDefinitionId = r.ReleaseDefinitionId
      })).ToList<ReleaseDefinition>();
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseDeployment> QueryReleaseDeploymentsByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseDeploymentsByChangedDate", 1971075))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseDeployment>> action = (Func<AnalyticsSqlComponent, IList<ReleaseDeployment>>) (component => component.QueryReleaseDeploymentsByChangedDate(dataspaceId, batchSize, fromDate));
        IList<ReleaseDeployment> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseDeployment>>(action);
        ReleaseDeployment releaseDeployment = source.OrderBy<ReleaseDeployment, DateTime>((Func<ReleaseDeployment, DateTime>) (d => d.LastModifiedOn)).LastOrDefault<ReleaseDeployment>();
        if (releaseDeployment != null)
          toDate = releaseDeployment.LastModifiedOn;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseDeploymentGate> QueryReleaseDeploymentGatesByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseDeploymentGatesByChangedDate", 1971086))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseDeploymentGate>> action = (Func<AnalyticsSqlComponent, IList<ReleaseDeploymentGate>>) (component => component.QueryReleaseDeploymentGatesByChangedDate(dataspaceId, batchSize, fromDate));
        IList<ReleaseDeploymentGate> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseDeploymentGate>>(action);
        ReleaseDeploymentGate releaseDeploymentGate = source.OrderBy<ReleaseDeploymentGate, DateTime?>((Func<ReleaseDeploymentGate, DateTime?>) (g => g.DeploymentLastModifiedOn)).LastOrDefault<ReleaseDeploymentGate>();
        if (releaseDeploymentGate != null && releaseDeploymentGate.DeploymentLastModifiedOn.HasValue)
          toDate = releaseDeploymentGate.DeploymentLastModifiedOn.Value;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseDeploymentRunPlan> QueryReleaseDeploymentRunPlansByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseDeploymentRunPlansByChangedDate", 1971087))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseDeploymentRunPlan>> action = (Func<AnalyticsSqlComponent, IList<ReleaseDeploymentRunPlan>>) (component => component.QueryReleaseDeploymentRunPlansByChangedDate(dataspaceId, batchSize, fromDate));
        IList<ReleaseDeploymentRunPlan> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseDeploymentRunPlan>>(action);
        ReleaseDeploymentRunPlan deploymentRunPlan = source.OrderBy<ReleaseDeploymentRunPlan, DateTime?>((Func<ReleaseDeploymentRunPlan, DateTime?>) (p => p.DeploymentLastModifiedOn)).LastOrDefault<ReleaseDeploymentRunPlan>();
        if (deploymentRunPlan != null && deploymentRunPlan.DeploymentLastModifiedOn.HasValue)
          toDate = deploymentRunPlan.DeploymentLastModifiedOn.Value;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseEnvironment> QueryReleaseEnvironmentsByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseEnvironmentsByChangedDate", 1971088))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseEnvironment>> action = (Func<AnalyticsSqlComponent, IList<ReleaseEnvironment>>) (component => component.QueryReleaseEnvironmentsByChangedDate(dataspaceId, batchSize, fromDate));
        IList<ReleaseEnvironment> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseEnvironment>>(action);
        ReleaseEnvironment releaseEnvironment = source.OrderBy<ReleaseEnvironment, DateTime?>((Func<ReleaseEnvironment, DateTime?>) (e => e.DeploymentLastModifiedOn)).LastOrDefault<ReleaseEnvironment>();
        if (releaseEnvironment != null && releaseEnvironment.DeploymentLastModifiedOn.HasValue)
          toDate = releaseEnvironment.DeploymentLastModifiedOn.Value;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "3#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseEnvironmentDefinition> QueryReleaseEnvironmentDefinitionsByChangedDate(
      IVssRequestContext requestContext,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseEnvironmentDefinitionsByChangedDate", 1971076))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseEnvironmentDefinition>> action = (Func<AnalyticsSqlComponent, IList<ReleaseEnvironmentDefinition>>) (component => component.QueryReleaseEnvironmentDefinitionsByChangedDate(batchSize, fromDate));
        IList<ReleaseEnvironmentDefinition> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseEnvironmentDefinition>>(action);
        ReleaseEnvironmentDefinition environmentDefinition = source.OrderBy<ReleaseEnvironmentDefinition, DateTime>((Func<ReleaseEnvironmentDefinition, DateTime>) (e => e.ModifiedOn)).LastOrDefault<ReleaseEnvironmentDefinition>();
        if (environmentDefinition != null)
          toDate = environmentDefinition.ModifiedOn;
        return source;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Common pattern for Analytics.")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "The name toDate is not hungarian notation")]
    public virtual IList<ReleaseEnvironmentStep> QueryReleaseEnvironmentStepsByChangedDate(
      IVssRequestContext requestContext,
      int dataspaceId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      toDate = fromDate;
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "RMAnalyticsService.QueryReleaseEnvironmentStepsByChangedDate", 1971077))
      {
        Func<AnalyticsSqlComponent, IList<ReleaseEnvironmentStep>> action = (Func<AnalyticsSqlComponent, IList<ReleaseEnvironmentStep>>) (component => component.QueryReleaseEnvironmentStepsByChangedDate(dataspaceId, batchSize, fromDate));
        IList<ReleaseEnvironmentStep> source = requestContext.ExecuteWithinUsingWithComponent<AnalyticsSqlComponent, IList<ReleaseEnvironmentStep>>(action);
        bool flag = source.All<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.DeploymentLastModifiedOn.HasValue));
        ReleaseEnvironmentStep releaseEnvironmentStep = flag ? source.OrderBy<ReleaseEnvironmentStep, DateTime?>((Func<ReleaseEnvironmentStep, DateTime?>) (s => s.DeploymentLastModifiedOn)).LastOrDefault<ReleaseEnvironmentStep>() : source.OrderBy<ReleaseEnvironmentStep, DateTime>((Func<ReleaseEnvironmentStep, DateTime>) (s => s.ModifiedOn)).LastOrDefault<ReleaseEnvironmentStep>();
        if (releaseEnvironmentStep != null)
          toDate = flag ? releaseEnvironmentStep.DeploymentLastModifiedOn.Value : releaseEnvironmentStep.ModifiedOn;
        return source;
      }
    }
  }
}
