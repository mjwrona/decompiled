// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseHistoryService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseHistoryService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void SaveRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release definitionSnapshot,
      ReleaseHistoryChangeTypes changeType,
      string comment)
    {
      byte[] content = definitionSnapshot != null ? Encoding.UTF8.GetBytes(ServerModelUtility.ToString((object) definitionSnapshot)) : throw new ArgumentNullException(nameof (definitionSnapshot));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseHistoryService.SaveRevision", 1961217))
      {
        int num = requestContext.GetService<TeamFoundationFileService>().UploadFile(requestContext, content);
        releaseManagementTimer.RecordLap("Service", "TeamFoundationFilerService.UploadReleaseSnapshotFile", 1962012);
        ReleaseCreateOrUpdateChangeDetails updateChangeDetails = new ReleaseCreateOrUpdateChangeDetails()
        {
          ChangeType = changeType,
          ReleaseName = definitionSnapshot.Name
        };
        ReleaseRevision releaseRevision = new ReleaseRevision()
        {
          ChangedBy = definitionSnapshot.ModifiedBy,
          ChangedDate = definitionSnapshot.ModifiedOn,
          ChangeDetails = (ReleaseRevisionChangeDetails) updateChangeDetails,
          ChangeType = changeType,
          Comment = comment,
          DefinitionSnapshotRevision = definitionSnapshot.DefinitionSnapshotRevision,
          FileId = num,
          ReleaseId = definitionSnapshot.Id
        };
        ReleaseHistoryService.UpdateRevision(requestContext, projectId, releaseRevision);
        releaseManagementTimer.RecordLap("Service", "ReleaseHistoryService.SaveReleaseRevisionInSql", 1961218);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void SaveDefinitionSnapshotRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      ReleaseHistoryChangeTypes changeType,
      int definitionSnapshotRevision,
      string definitionSnapshot)
    {
      if (definitionSnapshot == null)
        throw new ArgumentNullException(nameof (definitionSnapshot));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseHistoryService.SaveRevision", 1961217))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(definitionSnapshot);
        int fileId = requestContext.GetService<TeamFoundationFileService>().UploadFile(requestContext, bytes);
        releaseManagementTimer.RecordLap("Service", "TeamFoundationFilerService.UploadReleaseSnapshotFile", 1962012);
        try
        {
          Action<ReleaseHistorySqlComponent> action = (Action<ReleaseHistorySqlComponent>) (component => component.UpdateRevision(projectId, releaseId, definitionSnapshotRevision, changeType, fileId));
          requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent>(action);
        }
        catch (ReleaseDefinitionHistoryUpdateException ex)
        {
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseRevisionFileIdUpdateFailureTraceMessage, (object) releaseId, (object) definitionSnapshotRevision, (object) changeType, (object) fileId, (object) ex.Message);
          requestContext.Trace(1961218, TraceLevel.Warning, "ReleaseManagementService", "Service", message);
          ReleaseHistoryService.DeleteFiles(requestContext, (IEnumerable<int>) new List<int>(fileId));
        }
        releaseManagementTimer.RecordLap("Service", "ReleaseHistoryService.SaveReleaseRevisionInSql", 1961218);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public Stream GetRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int definitionSnapshotRevision)
    {
      Func<ReleaseHistorySqlComponent, ReleaseRevision> action = (Func<ReleaseHistorySqlComponent, ReleaseRevision>) (component => component.GetRevision(projectId, releaseId, definitionSnapshotRevision));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseHistoryService.GetRevision", 1961219))
      {
        ReleaseRevision releaseRevision = requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent, ReleaseRevision>(action);
        releaseManagementTimer.RecordLap("Service", "ReleaseHistoryService.GetReleaseRevisionFromSql", 1961220);
        if (releaseRevision == null || releaseRevision.FileId <= 0)
          return Stream.Null;
        Stream revision = requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) releaseRevision.FileId, false, out byte[] _, out long _, out CompressionType _);
        releaseManagementTimer.RecordLap("Service", "ReleaseHistoryService.GetReleaseSnapshotUsingFileService", 1961221);
        return revision;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseRevision> GetHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseHistoryService.GetHistory", 1961222))
      {
        Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>> action = (Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>) (component => component.GetHistory(projectId, releaseId));
        return requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>(action).ResolveIdentityRefs(requestContext);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseRevision> GetHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attempt)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseHistoryService.GetHistory", 1961222))
      {
        Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>> action = (Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>) (component => component.GetHistory(projectId, releaseId, environmentId, attempt));
        return requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>(action).ResolveIdentityRefs(requestContext);
      }
    }

    public IEnumerable<ReleaseRevision> GetDeployTypeChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attempt,
      ReleaseEnvironmentStatus environmentStatus)
    {
      return (IEnumerable<ReleaseRevision>) this.GetHistory(requestContext, projectId, releaseId, environmentId, attempt).Where<ReleaseRevision>((Func<ReleaseRevision, bool>) (x => x.ChangeType == ReleaseHistoryChangeTypes.Deploy && x.ChangeDetails.GetReleaseEnvironmentStatus() == environmentStatus)).ToList<ReleaseRevision>();
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DeleteDefinitionSnapshots(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>> action = (Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>) (component => component.GetHistory(projectId, releaseId));
      IEnumerable<ReleaseRevision> source = requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>(action);
      if (source == null)
        return;
      IEnumerable<int> fileIds = source.Where<ReleaseRevision>((Func<ReleaseRevision, bool>) (revision => revision != null)).Select<ReleaseRevision, int>((Func<ReleaseRevision, int>) (revision => revision.FileId));
      ReleaseHistoryService.DeleteFiles(requestContext, fileIds);
    }

    private static void UpdateRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseRevision releaseRevision)
    {
      try
      {
        Action<ReleaseHistorySqlComponent> action = (Action<ReleaseHistorySqlComponent>) (component => component.SaveRevision(projectId, releaseRevision));
        requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent>(action);
      }
      catch (ReleaseDefinitionHistoryUpdateException ex)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ReleaseRevisionFileIdUpdateFailureTraceMessage, (object) releaseRevision.ReleaseId, (object) releaseRevision.DefinitionSnapshotRevision, (object) releaseRevision.ChangeType, (object) releaseRevision.FileId, (object) ex.Message);
        requestContext.Trace(1961218, TraceLevel.Warning, "ReleaseManagementService", "Service", message);
        ReleaseHistoryService.DeleteFiles(requestContext, (IEnumerable<int>) new List<int>(releaseRevision.FileId));
      }
    }

    private static void DeleteFiles(IVssRequestContext requestContext, IEnumerable<int> fileIds)
    {
      if (fileIds == null)
        return;
      List<int> list = fileIds.Where<int>((Func<int, bool>) (id => id > 0)).ToList<int>();
      if (!list.Any<int>())
        return;
      requestContext.GetService<TeamFoundationFileService>().DeleteFiles(requestContext, (IEnumerable<int>) list);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DeleteReleaseHistoryWithSecrets(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string secretPattern,
      int releasesCountToProcess)
    {
      List<int> intList1 = new List<int>();
      List<int> releaseIdsToBeDeleted = new List<int>();
      List<int> releaseSnapShotsToBeDeleted = new List<int>();
      List<int> intList2 = new List<int>();
      IEnumerable<ReleaseRevision> releaseRevisions = (IEnumerable<ReleaseRevision>) new List<ReleaseRevision>();
      int continuationToken = 0;
      int maxReleasesTobeRead = 500;
      int val1 = Math.Min(releasesCountToProcess, 500);
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      List<int> list;
      do
      {
        Func<ReleaseSqlComponent, IEnumerable<int>> action1 = (Func<ReleaseSqlComponent, IEnumerable<int>>) (component => component.GetReleasesForDefinition(projectId, definitionId, maxReleasesTobeRead, continuationToken));
        list = requestContext.ExecuteWithinUsingWithComponent<ReleaseSqlComponent, IEnumerable<int>>(action1).ToList<int>();
        int count = list.Count;
        if (count > 0)
        {
          continuationToken = list[count - 1] + 1;
          List<int> releasesIdListToProcess = new List<int>();
          for (int index = 0; index < count; index += val1)
          {
            releasesIdListToProcess = list.GetRange(index, Math.Min(val1, count - index));
            Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>> action2 = (Func<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>) (component => component.GetReleaseHistoryForReleases(projectId, (IEnumerable<int>) releasesIdListToProcess));
            IEnumerable<ReleaseRevision> releaseHistoryList = requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent, IEnumerable<ReleaseRevision>>(action2);
            this.DeleteReleaseHistoryWithSecrets(requestContext, releaseHistoryList, intList1, releaseIdsToBeDeleted, releaseSnapShotsToBeDeleted, secretPattern);
          }
        }
      }
      while (list.Count == maxReleasesTobeRead);
      if (!intList1.Any<int>())
        return;
      Action<ReleaseHistorySqlComponent> action = (Action<ReleaseHistorySqlComponent>) (component => component.DeleteReleaseHistoryForgivenReleaseAndSnapshot(projectId, (IEnumerable<int>) releaseIdsToBeDeleted, (IEnumerable<int>) releaseSnapShotsToBeDeleted));
      requestContext.ExecuteWithinUsingWithComponent<ReleaseHistorySqlComponent>(action);
      service.DeleteFiles(requestContext, (IEnumerable<int>) intList1);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    private void DeleteReleaseHistoryWithSecrets(
      IVssRequestContext requestContext,
      IEnumerable<ReleaseRevision> releaseHistoryList,
      List<int> fileIdsToBeDeleted,
      List<int> releaseIdsToBeDeleted,
      List<int> releaseSnapshotsToBeDeleted,
      string secretPattern)
    {
      string empty = string.Empty;
      bool flag = false;
      foreach (ReleaseRevision releaseHistory in releaseHistoryList)
      {
        int fileId = releaseHistory.FileId;
        try
        {
          flag = ReleaseAndRDSecretHandler.CheckForSecretinFileContent(ReleaseAndRDSecretHandler.RetrieveFile(requestContext, fileId), secretPattern);
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case RegexMatchTimeoutException _:
            case FileIdNotFoundException _:
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.RetrieveFileForRDAndCheckForSecretFailureMessage, (object) fileId, (object) ex.Message);
              requestContext.Trace(1961234, TraceLevel.Warning, "ReleaseManagementService", "Service", message);
              break;
            default:
              throw;
          }
        }
        if (flag)
        {
          releaseIdsToBeDeleted.Add(releaseHistory.ReleaseId);
          releaseSnapshotsToBeDeleted.Add(releaseHistory.DefinitionSnapshotRevision);
          fileIdsToBeDeleted.Add(fileId);
        }
      }
    }
  }
}
