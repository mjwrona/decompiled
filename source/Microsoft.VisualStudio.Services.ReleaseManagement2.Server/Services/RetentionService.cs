// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.RetentionService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
  public class RetentionService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual IEnumerable<string> GetBuildsToStopRetaining(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      IEnumerable<int> releaseIds,
      bool isDeleted)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseIds == null)
        throw new ArgumentNullException(nameof (releaseIds));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.GetBuildsToStopRetaining", 1971053))
      {
        Func<RetentionSqlComponent, IEnumerable<string>> action = (Func<RetentionSqlComponent, IEnumerable<string>>) (component => component.GetBuildsToStopRetaining(projectId, definitionId, releaseIds, isDeleted));
        return context.ExecuteWithinUsingWithComponent<RetentionSqlComponent, IEnumerable<string>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual void UpdateRetainBuildForReleaseArtifactSources(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<int> releaseIds,
      bool updatedStateForRetainBuild,
      string artifactType)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseIds == null)
        throw new ArgumentNullException(nameof (releaseIds));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.UpdateRetainBuildForReleaseArtifactSources", 1971048))
      {
        Action<RetentionSqlComponent> action = (Action<RetentionSqlComponent>) (component => component.UpdateRetainBuildForReleaseArtifactSources(projectId, releaseIds, updatedStateForRetainBuild, artifactType));
        context.ExecuteWithinUsingWithComponent<RetentionSqlComponent>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "Necessary to convey usage")]
    public virtual IEnumerable<Release> GetReleasesToUpdateRetention(
      IVssRequestContext context,
      Guid projectId,
      int definitionId,
      IEnumerable<int> definitionEnvironments,
      int minReleaseId,
      int maxReleases,
      string artifactTypeId,
      bool stopRetainBuild,
      bool isRdDeleted)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.GetReleasesToUpdateRetention", 1971052))
      {
        Func<RetentionSqlComponent, IEnumerable<Release>> action = (Func<RetentionSqlComponent, IEnumerable<Release>>) (component => component.GetReleasesToUpdateRetention(projectId, definitionId, definitionEnvironments, stopRetainBuild, minReleaseId, maxReleases, artifactTypeId, isRdDeleted));
        return context.ExecuteWithinUsingWithComponent<RetentionSqlComponent, IEnumerable<Release>>(action);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "These messages are for tracing, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Logs dont need to be localized")]
    public virtual void UpdateBuildRetention(
      IVssRequestContext context,
      IDictionary<Guid, IList<string>> artifactVersions,
      bool retainBuildState,
      StringBuilder results)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (artifactVersions == null)
        throw new ArgumentNullException(nameof (artifactVersions));
      if (results == null)
        throw new ArgumentNullException(nameof (results));
      if (!artifactVersions.Any<KeyValuePair<Guid, IList<string>>>())
      {
        context.Trace(1971048, TraceLevel.Info, "ReleaseManagementService", "Service", "No Builds to update retention.");
      }
      else
      {
        using (ReleaseManagementTimer.Create(context, "DataAccessLayer", "Service.UpdateBuildRetention", 1971048))
        {
          foreach (KeyValuePair<Guid, IList<string>> artifactVersion in (IEnumerable<KeyValuePair<Guid, IList<string>>>) artifactVersions)
          {
            IList<int> source = (IList<int>) new List<int>();
            foreach (string s in (IEnumerable<string>) artifactVersion.Value)
            {
              int result;
              if (int.TryParse(s, out result))
                source.Add(result);
            }
            IList<int> list = (IList<int>) source.Distinct<int>().ToList<int>();
            string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Update retainedByRelease status to {0} for buildIds : {1}", (object) retainBuildState, (object) string.Join<int>(",", (IEnumerable<int>) list));
            context.Trace(1971048, TraceLevel.Info, "ReleaseManagementService", "Service", message);
            results.AppendLine(message);
            RetentionService.ValidateAndUpdateBuilds(context, artifactVersion.Key, list, retainBuildState, results);
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is because of dependency on many classes.")]
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "ArgumentUtility does this.")]
    public virtual void DeleteRetentionLeases(
      IVssRequestContext requestContext,
      Guid releaseProjectId,
      IDictionary<Guid, IList<RetentionLeaseData>> leasesToDeleteProjectMap,
      StringBuilder results)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<Guid, IList<RetentionLeaseData>>>(leasesToDeleteProjectMap, nameof (leasesToDeleteProjectMap));
      ArgumentUtility.CheckForNull<StringBuilder>(results, nameof (results));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "Service.DeleteRetentionLeases", 1971048))
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        BuildHttpClient buildClient = vssRequestContext.GetClient<BuildHttpClient>();
        foreach (KeyValuePair<Guid, IList<RetentionLeaseData>> leasesToDeleteProject in (IEnumerable<KeyValuePair<Guid, IList<RetentionLeaseData>>>) leasesToDeleteProjectMap)
        {
          Guid projectId = leasesToDeleteProject.Key;
          IList<RetentionLeaseData> retentionLeaseDataList = leasesToDeleteProject.Value;
          MinimalRetentionLease[] minimalLeasesToDelete = retentionLeaseDataList.Select<RetentionLeaseData, MinimalRetentionLease>((Func<RetentionLeaseData, MinimalRetentionLease>) (l => this.GetMinimalRetentionLease(l.BuildId, l.BuildDefinitionId, l.OwnerId))).ToArray<MinimalRetentionLease>();
          if (RetentionService.IsValidProject(vssRequestContext, projectId, results))
          {
            int num1 = 0;
            int num2 = 3;
            while (num1 < num2)
            {
              ++num1;
              try
              {
                int[] leaseIdsToDelete = vssRequestContext.RunSynchronously<List<RetentionLease>>(closure_0 ?? (closure_0 = (Func<Task<List<RetentionLease>>>) (() => buildClient.GetRetentionLeasesByMinimalRetentionLeasesAsync(projectId.ToString(), (IEnumerable<MinimalRetentionLease>) minimalLeasesToDelete)))).Select<RetentionLease, int>((Func<RetentionLease, int>) (l => l.LeaseId)).ToArray<int>();
                StringBuilder stringBuilder = new StringBuilder();
                if (((IEnumerable<int>) leaseIdsToDelete).Any<int>())
                {
                  string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Calling DeleteRetentionLeasesByIdAsync for projectId : {0} and for lease ids : {1}", (object) projectId, (object) string.Join<int>(",", (IEnumerable<int>) leaseIdsToDelete));
                  stringBuilder.AppendLine(str);
                  results.AppendLine(str);
                  vssRequestContext.RunSynchronously((Func<Task>) (() => buildClient.DeleteRetentionLeasesByIdAsync(projectId.ToString(), (IEnumerable<int>) leaseIdsToDelete)));
                }
                else
                {
                  string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found no retention leases for projectId : {0} and for owner ids : {1}", (object) projectId, (object) string.Join<string>(",", retentionLeaseDataList.Select<RetentionLeaseData, string>((Func<RetentionLeaseData, string>) (l => l.OwnerId))));
                  stringBuilder.AppendLine(str);
                  results.AppendLine(str);
                }
                requestContext.Trace(1971048, TraceLevel.Info, "ReleaseManagementService", "Service", stringBuilder.ToString());
                break;
              }
              catch (Exception ex)
              {
                string errorMessage = RetentionService.ProcessException(requestContext, ex, results);
                if (num1 >= num2)
                  RetentionService.QueueRetryJob(requestContext, projectId, releaseProjectId, ManageBuildRetentionLeaseActionType.DeleteLease, (IEnumerable<RetentionLeaseData>) retentionLeaseDataList, errorMessage);
              }
            }
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required for complexity of type")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is because of dependency on many classes.")]
    [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "ArgumentUtility does this.")]
    public virtual void AddRetentionLeases(
      IVssRequestContext requestContext,
      Guid releaseProjectId,
      IDictionary<Guid, IList<RetentionLeaseData>> leasesToAddProjectMap,
      StringBuilder results)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<Guid, IList<RetentionLeaseData>>>(leasesToAddProjectMap, nameof (leasesToAddProjectMap));
      ArgumentUtility.CheckForNull<StringBuilder>(results, nameof (results));
      using (ReleaseManagementTimer.Create(requestContext, "DataAccessLayer", "Service.AddRetentionLeases", 1971048))
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        BuildHttpClient buildClient = vssRequestContext.GetClient<BuildHttpClient>();
        foreach (KeyValuePair<Guid, IList<RetentionLeaseData>> leasesToAddProject in (IEnumerable<KeyValuePair<Guid, IList<RetentionLeaseData>>>) leasesToAddProjectMap)
        {
          Guid projectId = leasesToAddProject.Key;
          IList<RetentionLeaseData> retentionLeaseDataList = leasesToAddProject.Value;
          NewRetentionLease[] array1 = retentionLeaseDataList.Select<RetentionLeaseData, NewRetentionLease>((Func<RetentionLeaseData, NewRetentionLease>) (l => this.GetNewRetentionLease(l.BuildId, l.BuildDefinitionId, l.OwnerId))).ToArray<NewRetentionLease>();
          MinimalRetentionLease[] minimalLeases = retentionLeaseDataList.Select<RetentionLeaseData, MinimalRetentionLease>((Func<RetentionLeaseData, MinimalRetentionLease>) (l => this.GetMinimalRetentionLease(l.BuildId, l.BuildDefinitionId, l.OwnerId))).ToArray<MinimalRetentionLease>();
          if (RetentionService.IsValidProject(vssRequestContext, projectId, results))
          {
            int num1 = 0;
            int num2 = 3;
            while (num1 < num2)
            {
              ++num1;
              try
              {
                RetentionLease[] array2 = vssRequestContext.RunSynchronously<List<RetentionLease>>(closure_0 ?? (closure_0 = (Func<Task<List<RetentionLease>>>) (() => buildClient.GetRetentionLeasesByMinimalRetentionLeasesAsync(projectId.ToString(), (IEnumerable<MinimalRetentionLease>) minimalLeases)))).ToArray();
                NewRetentionLease[] filteredNewRetentionLeases = array1;
                if (((IEnumerable<RetentionLease>) array2).Any<RetentionLease>())
                {
                  NewRetentionLease[] array3 = ((IEnumerable<NewRetentionLease>) array1).Join((IEnumerable<RetentionLease>) array2, newLease => new
                  {
                    DefinitionId = newLease.DefinitionId,
                    OwnerId = newLease.OwnerId,
                    RunId = newLease.RunId
                  }, dupLease => new
                  {
                    DefinitionId = dupLease.DefinitionId,
                    OwnerId = dupLease.OwnerId,
                    RunId = dupLease.RunId
                  }, (Func<NewRetentionLease, RetentionLease, NewRetentionLease>) ((newLease, dupLease) => newLease)).ToArray<NewRetentionLease>();
                  filteredNewRetentionLeases = ((IEnumerable<NewRetentionLease>) filteredNewRetentionLeases).Except<NewRetentionLease>((IEnumerable<NewRetentionLease>) array3).ToArray<NewRetentionLease>();
                }
                StringBuilder stringBuilder = new StringBuilder();
                if (((IEnumerable<NewRetentionLease>) filteredNewRetentionLeases).Any<NewRetentionLease>())
                {
                  int[] array4 = ((IEnumerable<NewRetentionLease>) filteredNewRetentionLeases).Select<NewRetentionLease, int>((Func<NewRetentionLease, int>) (l => l.RunId)).ToArray<int>();
                  string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Calling AddRetentionLeasesAsync for projectId : {0} and for buildIds : {1}", (object) projectId, (object) string.Join<int>(",", (IEnumerable<int>) array4));
                  stringBuilder.AppendLine(str);
                  results.AppendLine(str);
                  vssRequestContext.RunSynchronously<List<RetentionLease>>((Func<Task<List<RetentionLease>>>) (() => buildClient.AddRetentionLeasesAsync((IReadOnlyList<NewRetentionLease>) filteredNewRetentionLeases, projectId)));
                }
                else
                {
                  string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found no retention leases to add for projectId : {0} and for owner ids : {1}", (object) projectId, (object) string.Join<string>(",", retentionLeaseDataList.Select<RetentionLeaseData, string>((Func<RetentionLeaseData, string>) (l => l.OwnerId))));
                  stringBuilder.AppendLine(str);
                  results.AppendLine(str);
                }
                requestContext.Trace(1971048, TraceLevel.Info, "ReleaseManagementService", "Service", stringBuilder.ToString());
                break;
              }
              catch (Exception ex)
              {
                string errorMessage = RetentionService.ProcessException(requestContext, ex, results);
                if (num1 >= num2)
                  RetentionService.QueueRetryJob(requestContext, projectId, releaseProjectId, ManageBuildRetentionLeaseActionType.AddLease, (IEnumerable<RetentionLeaseData>) retentionLeaseDataList, errorMessage);
              }
            }
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public NewRetentionLease GetNewRetentionLease(int buildId, int definitionId, string ownerId) => new NewRetentionLease()
    {
      DaysValid = 365000,
      DefinitionId = definitionId,
      OwnerId = ownerId,
      ProtectPipeline = true,
      RunId = buildId
    };

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public MinimalRetentionLease GetMinimalRetentionLease(
      int buildId,
      int definitionId,
      string ownerId)
    {
      return new MinimalRetentionLease()
      {
        DefinitionId = definitionId,
        OwnerId = ownerId,
        RunId = buildId
      };
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public string GetRMLeaseOwnerId(Guid projectId, int releaseId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "RM:{0}:{1}", (object) projectId, (object) releaseId);

    private static string ProcessException(
      IVssRequestContext requestContext,
      Exception ex,
      StringBuilder results)
    {
      requestContext.TraceCatch(1979015, "ReleaseManagementService", "Service", ex);
      string str = TeamFoundationExceptionFormatter.FormatException(ex, false);
      results.AppendLine(str);
      return str;
    }

    private static void QueueRetryJob(
      IVssRequestContext requestContext,
      Guid buildProjectId,
      Guid releaseProjectId,
      ManageBuildRetentionLeaseActionType actionType,
      IEnumerable<RetentionLeaseData> leaseData,
      string errorMessage)
    {
      ActionRequestService service = requestContext.GetService<ActionRequestService>();
      foreach (RetentionLeaseData retentionLeaseData in leaseData)
        ActionRequestsProcessorHelper.AddManageBuildRetentionLeaseActionRequest(requestContext, buildProjectId, service, actionType, errorMessage, retentionLeaseData.BuildId, releaseProjectId, retentionLeaseData.BuildDefinitionId, retentionLeaseData.OwnerId, retentionLeaseData.ReleaseId);
      service.QueueActionRequestsProcessorJob(requestContext, ActionRequestType.ManageBuildRetentionLease, true);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:CyclomaticComplexity", Justification = "This is because of dependency on many classes.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is because of dependency on many classes.")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Required to catch all exception")]
    private static void ValidateAndUpdateBuilds(
      IVssRequestContext context,
      Guid projectId,
      IList<int> buildIds,
      bool retainBuildState,
      StringBuilder results)
    {
      IVssRequestContext context1 = context.Elevate();
      BuildHttpClient buildClient = context1.GetClient<BuildHttpClient>();
      int num1 = 0;
      int num2 = 3;
      while (num1 < num2)
      {
        ++num1;
        try
        {
          if (!RetentionService.IsValidProject(context1, projectId, results))
            break;
          Func<Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>> getBuildAsyncFunc = closure_0 ?? (closure_0 = (Func<Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) (() =>
          {
            BuildHttpClient buildHttpClient = buildClient;
            Guid project = projectId;
            IEnumerable<int> ints = (IEnumerable<int>) buildIds;
            DateTime? minTime = new DateTime?();
            DateTime? maxTime = new DateTime?();
            BuildReason? reasonFilter = new BuildReason?();
            BuildStatus? statusFilter = new BuildStatus?();
            BuildResult? resultFilter = new BuildResult?();
            int? top = new int?();
            int? maxBuildsPerDefinition = new int?();
            QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
            BuildQueryOrder? queryOrder = new BuildQueryOrder?();
            IEnumerable<int> buildIds1 = ints;
            CancellationToken cancellationToken = new CancellationToken();
            return buildHttpClient.GetBuildsAsync(project, (IEnumerable<int>) null, (IEnumerable<int>) null, (string) null, minTime, maxTime, (string) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, (IEnumerable<string>) null, top, (string) null, maxBuildsPerDefinition, deletedFilter, queryOrder, (string) null, buildIds1, (string) null, (string) null, (object) null, cancellationToken);
          }));
          IList<Microsoft.TeamFoundation.Build.WebApi.Build> buildsToUpdate = (IList<Microsoft.TeamFoundation.Build.WebApi.Build>) (!context.IsFeatureEnabled("AzureDevOps.ReleaseManagement.MakeBuildClientCallSync") ? (IList<Microsoft.TeamFoundation.Build.WebApi.Build>) context.ExecuteAsyncAndGetResult<List<Microsoft.TeamFoundation.Build.WebApi.Build>>(getBuildAsyncFunc) : (IList<Microsoft.TeamFoundation.Build.WebApi.Build>) context.RunSynchronously<List<Microsoft.TeamFoundation.Build.WebApi.Build>>((Func<Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) (async () => await getBuildAsyncFunc())).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>()).Where<Microsoft.TeamFoundation.Build.WebApi.Build>(closure_1 ?? (closure_1 = (Func<Microsoft.TeamFoundation.Build.WebApi.Build, bool>) (b =>
          {
            bool? retainedByRelease = b.RetainedByRelease;
            bool flag = retainBuildState;
            return !(retainedByRelease.GetValueOrDefault() == flag & retainedByRelease.HasValue);
          }))).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>();
          if (!buildsToUpdate.Any<Microsoft.TeamFoundation.Build.WebApi.Build>())
            break;
          buildsToUpdate.ForEach<Microsoft.TeamFoundation.Build.WebApi.Build>(closure_2 ?? (closure_2 = (Action<Microsoft.TeamFoundation.Build.WebApi.Build>) (b =>
          {
            b.RetainedByRelease = new bool?(retainBuildState);
            b.KeepForever = new bool?();
          })));
          buildsToUpdate = (IList<Microsoft.TeamFoundation.Build.WebApi.Build>) buildsToUpdate.OrderBy<Microsoft.TeamFoundation.Build.WebApi.Build, int>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, int>) (o => o.Id)).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>();
          List<int> list1 = buildsToUpdate.Select<Microsoft.TeamFoundation.Build.WebApi.Build, int>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, int>) (b => b.Id)).ToList<int>();
          StringBuilder stringBuilder = new StringBuilder();
          string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Calling updateBuilds for projectId : {0} and for buildIds : {1}", (object) projectId, (object) string.Join<int>(",", (IEnumerable<int>) list1));
          stringBuilder.AppendLine(str1);
          results.AppendLine(str1);
          List<int> list2 = context.RunSynchronously<List<Microsoft.TeamFoundation.Build.WebApi.Build>>((Func<Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>>>) (() => RetentionService.UpdateBuildsInternal(buildClient, projectId, buildsToUpdate))).Select<Microsoft.TeamFoundation.Build.WebApi.Build, int>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, int>) (b => b.Id)).ToList<int>();
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Update happened for buildIds : {0}", (object) string.Join<int>(",", (IEnumerable<int>) list2));
          stringBuilder.AppendLine(str2);
          results.AppendLine(str2);
          if (list2.Count != buildsToUpdate.Count)
          {
            string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to update to new status for buildIds : {0}", (object) string.Join<int>(",", (IEnumerable<int>) list1.Except<int>((IEnumerable<int>) list2).ToList<int>()));
            stringBuilder.AppendLine(str3);
            results.AppendLine(str3);
          }
          string message = stringBuilder.ToString();
          context.Trace(1971048, list2.Count != buildsToUpdate.Count ? TraceLevel.Error : TraceLevel.Info, "ReleaseManagementService", "Service", message);
          break;
        }
        catch (Exception ex)
        {
          context.TraceCatch(1979015, "ReleaseManagementService", "Service", ex);
          string errorMessage = TeamFoundationExceptionFormatter.FormatException(ex, false);
          results.AppendLine(errorMessage);
          if (num1 >= num2)
          {
            ActionRequestService service = context.GetService<ActionRequestService>();
            foreach (int buildId in (IEnumerable<int>) buildIds)
              ActionRequestsProcessorHelper.AddRetainBuildActionRequest(context, projectId, service, buildId, retainBuildState, errorMessage);
            service.QueueActionRequestsProcessorJob(context, ActionRequestType.RetainBuild, true);
          }
        }
      }
    }

    private static async Task<List<Microsoft.TeamFoundation.Build.WebApi.Build>> UpdateBuildsInternal(
      BuildHttpClient buildClient,
      Guid projectId,
      IList<Microsoft.TeamFoundation.Build.WebApi.Build> buildsToUpdate)
    {
      return await buildClient.UpdateBuildsAsync((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>) buildsToUpdate, projectId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Should catch all exceptions")]
    private static bool IsValidProject(
      IVssRequestContext context,
      Guid projectId,
      StringBuilder results)
    {
      bool flag = false;
      try
      {
        ProjectHelper.GetProject(context, projectId);
        flag = true;
      }
      catch (Exception ex)
      {
        if (ex is ProjectDoesNotExistException || ex.InnerException is ProjectDoesNotExistException)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find the build's project {0}. Failed with exception : {1}", (object) projectId, (object) ex.Message);
          results.Append(message);
          context.Trace(1971048, TraceLevel.Warning, "ReleaseManagementService", "Service", message);
        }
        else
          throw;
      }
      return flag;
    }
  }
}
