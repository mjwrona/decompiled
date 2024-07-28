// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseDefinitionHistoryService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
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
  public class ReleaseDefinitionHistoryService : ReleaseManagement2ServiceBase
  {
    public const string DefaultApiVersionValue = "4.0-preview.3";

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void SaveRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      string apiVersion,
      AuditAction changeType)
    {
      if (apiVersion == null)
        apiVersion = "4.0-preview.3";
      JsonSerializerSettings serializerSettings = new VssJsonMediaTypeFormatter().SerializerSettings;
      byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) releaseDefinition, serializerSettings));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseDefinitionHistoryService.SaveRevision", 1961204))
      {
        int fileId = requestContext.GetService<TeamFoundationFileService>().UploadFile(requestContext, bytes);
        releaseManagementTimer.RecordLap("Service", "TeamFoundationFilerService.UploadFile", 1962012);
        Action<ReleaseDefinitionHistorySqlComponent> action = (Action<ReleaseDefinitionHistorySqlComponent>) (component => component.SaveRevision(projectId, releaseDefinition, fileId, apiVersion, changeType));
        requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionHistorySqlComponent>(action);
        releaseManagementTimer.RecordLap("Service", "ReleaseDefinitionHistoryService.SaveRevisionSql", 1961205);
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DeleteRevisionHistoryWithSecrets(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string secretPattern)
    {
      IEnumerable<ReleaseDefinitionRevision> history = this.GetHistory(requestContext, projectId, definitionId);
      List<int> intList = new List<int>();
      List<int> revisionsToBeDeleted = new List<int>();
      string empty = string.Empty;
      bool flag = false;
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      foreach (ReleaseDefinitionRevision definitionRevision in history)
      {
        int fileId = definitionRevision.FileId;
        int revision = definitionRevision.Revision;
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
              string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties.Resources.RetrieveFileForRDAndCheckForSecretFailureMessage, (object) fileId, (object) ex.Message);
              requestContext.Trace(1961234, TraceLevel.Warning, "ReleaseManagementService", "Service", message);
              break;
            default:
              throw;
          }
        }
        if (flag)
        {
          revisionsToBeDeleted.Add(revision);
          intList.Add(fileId);
        }
      }
      if (!intList.Any<int>())
        return;
      Action<ReleaseDefinitionHistorySqlComponent> action = (Action<ReleaseDefinitionHistorySqlComponent>) (component => component.DeleteReleaseDefinitionRevisions(projectId, definitionId, (IEnumerable<int>) revisionsToBeDeleted));
      requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionHistorySqlComponent>(action);
      service.DeleteFiles(requestContext, (IEnumerable<int>) intList);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public Stream GetRevision(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int revision)
    {
      Func<ReleaseDefinitionHistorySqlComponent, ReleaseDefinitionRevision> action = (Func<ReleaseDefinitionHistorySqlComponent, ReleaseDefinitionRevision>) (component => component.GetRevision(projectId, definitionId, revision));
      ReleaseDefinitionRevision definitionRevision = requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionHistorySqlComponent, ReleaseDefinitionRevision>(action);
      return definitionRevision != null ? requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) definitionRevision.FileId, false, out byte[] _, out long _, out CompressionType _) : Stream.Null;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void CheckReleaseDefinitionRevisionStream(
      IVssRequestContext requestContext,
      int definitionId,
      int revision,
      Stream content)
    {
      if (content.Length > 0L)
        return;
      if (requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.Return404WhenNoRevisionFound"))
        throw new ReleaseDefinitionRevisionNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.ReleaseDefinitionRevisionNotFound, (object) definitionId, (object) revision));
      requestContext.Trace(1900050, TraceLevel.Info, "ReleaseManagementService", "Service", Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.TraceMessageForGetRevision);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseDefinitionRevision> GetHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      Func<ReleaseDefinitionHistorySqlComponent, IEnumerable<ReleaseDefinitionRevision>> action = (Func<ReleaseDefinitionHistorySqlComponent, IEnumerable<ReleaseDefinitionRevision>>) (component => component.GetHistory(projectId, definitionId));
      return (IEnumerable<ReleaseDefinitionRevision>) requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionHistorySqlComponent, IEnumerable<ReleaseDefinitionRevision>>(action).ResolveIdentityRefs(requestContext).OrderBy<ReleaseDefinitionRevision, DateTime>((Func<ReleaseDefinitionRevision, DateTime>) (historyEntry => historyEntry.ChangedDate));
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public void DeleteHistory(IVssRequestContext requestContext, Guid projectId, int definitionId)
    {
      Func<ReleaseDefinitionHistorySqlComponent, IEnumerable<ReleaseDefinitionRevision>> action1 = (Func<ReleaseDefinitionHistorySqlComponent, IEnumerable<ReleaseDefinitionRevision>>) (component => component.GetHistory(projectId, definitionId));
      IEnumerable<int> fileIds = requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionHistorySqlComponent, IEnumerable<ReleaseDefinitionRevision>>(action1).Select<ReleaseDefinitionRevision, int>((Func<ReleaseDefinitionRevision, int>) (revision => revision.FileId));
      requestContext.GetService<TeamFoundationFileService>().DeleteFiles(requestContext, fileIds);
      Action<ReleaseDefinitionHistorySqlComponent> action2 = (Action<ReleaseDefinitionHistorySqlComponent>) (component => component.DeleteHistory(projectId, definitionId));
      requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionHistorySqlComponent>(action2);
    }
  }
}
