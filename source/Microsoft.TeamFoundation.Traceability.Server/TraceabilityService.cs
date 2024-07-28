// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Traceability.Server.TraceabilityService
// Assembly: Microsoft.TeamFoundation.Traceability.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C62AF110-A283-470F-B32B-FE03F2A1E0D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Traceability.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models;
using Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Providers;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Traceability.Server
{
  public class TraceabilityService : ITraceabilityService, IVssFrameworkService
  {
    private const string TraceLayer = "TraceabilityService";
    private const int MaxItems = 250;

    public TraceabilityChanges GetChanges(
      IVssRequestContext requestContext,
      ArtifactVersion currentArtifact,
      ArtifactVersion baseArtifact,
      TraceabilityContinuationToken continuationToken)
    {
      return TraceabilityService.GetChangesInternal(requestContext, currentArtifact, baseArtifact, continuationToken, out Guid _, out ArtifactSourceVersion _, out IList<string> _);
    }

    public IList<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      ArtifactVersion currentArtifact,
      ArtifactVersion baseArtifact)
    {
      Guid projectId;
      ArtifactSourceVersion currentSourceVersion;
      IList<string> workItemsProviderNames;
      TraceabilityChanges changesInternal = TraceabilityService.GetChangesInternal(requestContext, currentArtifact, baseArtifact, (TraceabilityContinuationToken) null, out projectId, out currentSourceVersion, out workItemsProviderNames);
      if ((workItemsProviderNames != null ? (!workItemsProviderNames.Any<string>() ? 1 : 0) : 1) == 0 && !(projectId == Guid.Empty))
      {
        int num1;
        if (changesInternal == null)
        {
          num1 = 1;
        }
        else
        {
          IList<Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change> changes = changesInternal.Changes;
          bool? nullable = changes != null ? new bool?(changes.Any<Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change>()) : new bool?();
          bool flag = true;
          num1 = !(nullable.GetValueOrDefault() == flag & nullable.HasValue) ? 1 : 0;
        }
        if (num1 == 0)
        {
          List<WorkItem> workItems1 = new List<WorkItem>();
          int num2 = 0;
          foreach (string providerName in (IEnumerable<string>) workItemsProviderNames)
          {
            int maxItems = 250 - num2;
            if (maxItems > 0)
            {
              try
              {
                IList<WorkItem> workItems2 = TraceabilityService.GetWorkItemsProvider(requestContext, providerName).GetWorkItems(requestContext, projectId, currentSourceVersion, (IEnumerable<Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change>) changesInternal.Changes, maxItems);
                if (workItems2 != null)
                {
                  if (workItems2.Any<WorkItem>())
                  {
                    num2 += workItems2.Count;
                    workItems1.AddRange((IEnumerable<WorkItem>) workItems2);
                  }
                }
              }
              catch (Exception ex)
              {
                requestContext.TraceException(34003405, "Traceability", nameof (TraceabilityService), ex);
              }
            }
            else
              break;
          }
          return (IList<WorkItem>) workItems1;
        }
      }
      return (IList<WorkItem>) Array.Empty<WorkItem>();
    }

    private static TraceabilityChanges GetChangesInternal(
      IVssRequestContext requestContext,
      ArtifactVersion currentArtifact,
      ArtifactVersion baseArtifact,
      TraceabilityContinuationToken inputContinuationToken,
      out Guid projectId,
      out ArtifactSourceVersion currentSourceVersion,
      out IList<string> workItemsProviderNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      workItemsProviderNames = (IList<string>) Array.Empty<string>();
      projectId = Guid.Empty;
      currentSourceVersion = (ArtifactSourceVersion) null;
      string message1 = "GetChangesInternal for ca_ui: " + currentArtifact?.UniqueResourceIdentifier + ", cv_version: " + currentArtifact?.ArtifactVersionId + ", ba_ui: " + baseArtifact?.UniqueResourceIdentifier + ", ba_version: " + baseArtifact?.ArtifactVersionId + ", ct_uid: " + inputContinuationToken?.UniqueIdentifier + ", ct_v: " + inputContinuationToken?.Version;
      TraceabilityService.TraceAlways(requestContext, 34003406, TraceLevel.Info, message1);
      try
      {
        ArtifactSourceVersion baseSourceVersion;
        if (TraceabilityService.TryValidateArtifacts(currentArtifact, baseArtifact, inputContinuationToken, out projectId, out currentSourceVersion, out baseSourceVersion))
        {
          if (projectId != Guid.Empty)
          {
            IChangesProvider changesProvider = TraceabilityService.GetChangesProvider(requestContext, currentSourceVersion.RepositoryType);
            ref IList<string> local = ref workItemsProviderNames;
            IList<string> itemsProviderTypes = changesProvider.GetDefaultWorkItemsProviderTypes();
            List<string> stringList = (itemsProviderTypes != null ? itemsProviderTypes.Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).ToList<string>() : (List<string>) null) ?? ((IEnumerable<string>) Array.Empty<string>()).ToList<string>();
            local = (IList<string>) stringList;
            string continuationTokenVersion = !string.IsNullOrWhiteSpace(inputContinuationToken?.UniqueIdentifier) ? inputContinuationToken?.Version : string.Empty;
            string message2 = "GetChangesInternal for csv_ui: " + currentSourceVersion.UniqueIdentifier + ", csv_version: " + currentSourceVersion.Version + ", bsv_ui: " + baseSourceVersion?.UniqueIdentifier + ", bsv_version: " + baseSourceVersion?.Version + ", ct_v: " + continuationTokenVersion + ", workItemsProviders: [" + string.Join(", ", (IEnumerable<string>) workItemsProviderNames) + "]";
            TraceabilityService.TraceAlways(requestContext, 34003406, TraceLevel.Info, message2);
            return changesProvider.GetChanges(requestContext, projectId, currentSourceVersion, baseSourceVersion, continuationTokenVersion: continuationTokenVersion);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(34003405, "Traceability", nameof (TraceabilityService), ex);
        throw;
      }
      return new TraceabilityChanges(string.Empty, (IList<Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change>) Array.Empty<Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.Change>(), (TraceabilityContinuationToken) null);
    }

    private static IChangesProvider GetChangesProvider(
      IVssRequestContext requestContext,
      string providerName)
    {
      IDisposableReadOnlyList<IChangesProvider> extensions = requestContext.GetExtensions<IChangesProvider>(ExtensionLifetime.Service);
      IEnumerable<IChangesProvider> source = extensions != null ? extensions.Where<IChangesProvider>((Func<IChangesProvider, bool>) (x => string.Equals(x.ProviderName, providerName, StringComparison.OrdinalIgnoreCase))) : (IEnumerable<IChangesProvider>) null;
      int num;
      if (source == null || (num = source.Count<IChangesProvider>()) == 0)
        throw new ChangesProviderNotFoundException(Resources.ChangesProviderNotFound((object) providerName));
      if (num > 1)
        TraceabilityService.TraceAlways(requestContext, 34003401, TraceLevel.Warning, Resources.MultipleProvidersWarning((object) providerName));
      return source.First<IChangesProvider>();
    }

    private static IWorkItemsProvider GetWorkItemsProvider(
      IVssRequestContext requestContext,
      string providerName)
    {
      IDisposableReadOnlyList<IWorkItemsProvider> extensions = requestContext.GetExtensions<IWorkItemsProvider>(ExtensionLifetime.Service);
      IEnumerable<IWorkItemsProvider> source = extensions != null ? extensions.Where<IWorkItemsProvider>((Func<IWorkItemsProvider, bool>) (x => string.Equals(x.ProviderName, providerName, StringComparison.OrdinalIgnoreCase))) : (IEnumerable<IWorkItemsProvider>) null;
      int num;
      if (source == null || (num = source.Count<IWorkItemsProvider>()) == 0)
        throw new WorkItemsProviderNotFoundException(Resources.WorkItemsProviderNotFound((object) providerName));
      if (num > 1)
        TraceabilityService.TraceAlways(requestContext, 34003401, TraceLevel.Warning, Resources.MultipleProvidersWarning((object) providerName));
      return source.First<IWorkItemsProvider>();
    }

    private static bool TryValidateArtifacts(
      ArtifactVersion currentArtifact,
      ArtifactVersion baseArtifact,
      TraceabilityContinuationToken inputContinuationToken,
      out Guid projectId,
      out ArtifactSourceVersion currentSourceVersion,
      out ArtifactSourceVersion baseSourceVersion)
    {
      baseSourceVersion = (ArtifactSourceVersion) null;
      IList<ArtifactSourceVersion> sourceVersions = currentArtifact?.GetSourceVersions();
      currentSourceVersion = TraceabilityService.GetArtifactSourceVersion(sourceVersions, inputContinuationToken?.UniqueIdentifier);
      TraceabilityService.CheckForNullArtifactSource(currentSourceVersion, "currentArtifact or currentArtifactSourceVersion");
      string empty = string.Empty;
      IDictionary<string, string> properties = currentSourceVersion.Properties;
      if ((properties != null ? (!properties.TryGetValue(ArtifactTraceabilityPropertyKeys.ProjectId, out empty) ? 1 : 0) : 1) != 0)
        currentArtifact.ArtifactVersionProperties?.TryGetValue(ArtifactTraceabilityPropertyKeys.ProjectId, out empty);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(empty, ArtifactTraceabilityPropertyKeys.ProjectId);
      projectId = new Guid(empty);
      ArgumentUtility.CheckForEmptyGuid(projectId, ArtifactTraceabilityPropertyKeys.ProjectId);
      if (baseArtifact != null)
      {
        baseSourceVersion = TraceabilityService.GetBaseArtifactSourceVersion(sourceVersions, baseArtifact.GetSourceVersions(), currentSourceVersion.UniqueIdentifier);
        try
        {
          TraceabilityService.CheckForNullArtifactSource(baseSourceVersion, "baseArtifactSourceVersion");
        }
        catch (ArgumentNullException ex)
        {
          throw new BaseArtifactSourceVersionNullException(Resources.BaseArtifactSourceVersionNullError((object) baseArtifact.UniqueResourceIdentifier, (object) currentSourceVersion.UniqueIdentifier));
        }
        if (!currentSourceVersion.RepositoryType.Equals(baseSourceVersion.RepositoryType, StringComparison.OrdinalIgnoreCase) || !currentSourceVersion.RepositoryId.Equals(baseSourceVersion.RepositoryId, StringComparison.OrdinalIgnoreCase))
          throw new ReposNotMatchingException(Resources.ReposNotMatchingError((object) currentSourceVersion.RepositoryId, (object) baseSourceVersion.RepositoryId, (object) currentArtifact.Alias));
      }
      return true;
    }

    public IList<WorkItem> GetPipelineArtifactWorkItems(
      IVssRequestContext requestContext,
      Guid projectId,
      out string exception,
      ArtifactVersion artifact)
    {
      exception = string.Empty;
      IList<ArtifactVersion> artifactVersionList1 = (IList<ArtifactVersion>) new List<ArtifactVersion>()
      {
        artifact
      };
      IList<WorkItem> first = (IList<WorkItem>) new List<WorkItem>();
      ArtifactVersion artifactVersion1 = artifact;
      IList<string> knownPipelines = (IList<string>) new List<string>();
      IArtifactTraceabilityService traceabilityService = requestContext.GetService<IArtifactTraceabilityService>()?.GetArtifactTraceabilityService(ArtifactTraceabilityConstants.ArtifactTraceabilityDeploymentService);
      for (; artifactVersion1 != null; artifactVersion1 = artifactVersionList1.Where<ArtifactVersion>((Func<ArtifactVersion, bool>) (x => !knownPipelines.Contains(x.ArtifactVersionId))).FirstOrDefault<ArtifactVersion>())
      {
        try
        {
          knownPipelines.Add(artifactVersion1.ArtifactVersionId);
          if (artifactVersion1.ArtifactCategory == ArtifactCategory.Pipeline)
          {
            IList<ArtifactVersion> source1 = traceabilityService?.GetArtifactTraceabilityDataForPipeline(requestContext, projectId, int.Parse(artifactVersion1.ArtifactVersionId), includeSourceDetails: true) ?? (IList<ArtifactVersion>) ((IEnumerable<ArtifactVersion>) Array.Empty<ArtifactVersion>()).ToList<ArtifactVersion>();
            artifactVersionList1 = (IList<ArtifactVersion>) artifactVersionList1.Concat<ArtifactVersion>((IEnumerable<ArtifactVersion>) source1.Where<ArtifactVersion>((Func<ArtifactVersion, bool>) (x => x.ArtifactCategory == ArtifactCategory.Pipeline)).ToList<ArtifactVersion>()).ToList<ArtifactVersion>();
            PipelineRunTraceabilitySnapshotObject traceabilitySnapshot = requestContext.GetService<IPipelineRunTraceabilitySnapshotService>().GetRunTraceabilitySnapshot(requestContext, projectId, int.Parse(artifactVersion1.ArtifactVersionId));
            List<ArtifactVersion> artifactVersionList2;
            if (traceabilitySnapshot == null)
            {
              artifactVersionList2 = (List<ArtifactVersion>) null;
            }
            else
            {
              IList<ArtifactVersion> artifactVersions = traceabilitySnapshot.BaseRunArtifactVersions;
              artifactVersionList2 = artifactVersions != null ? artifactVersions.ToList<ArtifactVersion>() : (List<ArtifactVersion>) null;
            }
            List<ArtifactVersion> source2 = artifactVersionList2;
            foreach (ArtifactVersion artifactVersion2 in source1.Where<ArtifactVersion>((Func<ArtifactVersion, bool>) (x => x.ArtifactCategory != ArtifactCategory.Pipeline)))
            {
              ArtifactVersion current = artifactVersion2;
              ArtifactVersion baseArtifact1 = (ArtifactVersion) null;
              if (source2 != null)
                baseArtifact1 = source2.Where<ArtifactVersion>((Func<ArtifactVersion, bool>) (baseArtifact => string.Equals(current.Alias, baseArtifact?.Alias, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ArtifactVersion>();
              try
              {
                IList<WorkItem> workItems = this.GetWorkItems(requestContext, current, baseArtifact1);
                first = (IList<WorkItem>) first.Union<WorkItem>((IEnumerable<WorkItem>) (workItems ?? (IList<WorkItem>) new List<WorkItem>()), (IEqualityComparer<WorkItem>) new TraceabilityService.WorkItemsComparer()).ToList<WorkItem>();
              }
              catch (Exception ex)
              {
                exception += ex.Message;
                requestContext.TraceException(34003405, "Traceability", nameof (TraceabilityService), ex);
              }
            }
          }
        }
        catch (Exception ex)
        {
          exception += ex.Message;
          requestContext.TraceException(34003405, "Traceability", nameof (TraceabilityService), ex);
        }
      }
      return first;
    }

    private static ArtifactSourceVersion GetArtifactSourceVersion(
      IList<ArtifactSourceVersion> sourceVersions,
      string sourceUniqueIdentifier)
    {
      if ((sourceVersions != null ? (!sourceVersions.Any<ArtifactSourceVersion>() ? 1 : 0) : 1) != 0)
        return (ArtifactSourceVersion) null;
      return !string.IsNullOrWhiteSpace(sourceUniqueIdentifier) ? sourceVersions.Where<ArtifactSourceVersion>((Func<ArtifactSourceVersion, bool>) (sv => sourceUniqueIdentifier.Equals(sv?.UniqueIdentifier, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<ArtifactSourceVersion>() : sourceVersions.FirstOrDefault<ArtifactSourceVersion>((Func<ArtifactSourceVersion, bool>) (sv => sv != null && !string.IsNullOrWhiteSpace(sv.RepositoryId) && !string.IsNullOrWhiteSpace(sv.RepositoryType)));
    }

    private static ArtifactSourceVersion GetBaseArtifactSourceVersion(
      IList<ArtifactSourceVersion> currentSourceVersions,
      IList<ArtifactSourceVersion> baseSourceVersions,
      string currentSourceUniqueIdentifier)
    {
      return currentSourceVersions.Count == 1 && baseSourceVersions.Count == 1 ? baseSourceVersions.FirstOrDefault<ArtifactSourceVersion>() : TraceabilityService.GetArtifactSourceVersion(baseSourceVersions, currentSourceUniqueIdentifier);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CheckForNullArtifactSource(
      ArtifactSourceVersion sourceVersion,
      string sourceVersionName)
    {
      ArgumentUtility.CheckForNull<ArtifactSourceVersion>(sourceVersion, sourceVersionName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(sourceVersion.RepositoryType, sourceVersionName + ".RepositoryType");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(sourceVersion.RepositoryId, sourceVersionName + ".RepositoryId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(sourceVersion.UniqueIdentifier, sourceVersionName + ".UniqueIdentifier");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void TraceAlways(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string message)
    {
      requestContext.TraceAlways(tracepoint, level, "Traceability", nameof (TraceabilityService), message);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private sealed class WorkItemsComparer : IEqualityComparer<WorkItem>
    {
      public bool Equals(WorkItem x, WorkItem y) => x?.Id == y?.Id;

      public int GetHashCode(WorkItem obj) => obj.Id.GetHashCode();
    }
  }
}
