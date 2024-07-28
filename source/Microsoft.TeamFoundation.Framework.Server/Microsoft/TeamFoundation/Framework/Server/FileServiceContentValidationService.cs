// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileServiceContentValidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FileServiceContentValidationService : 
    IFileServiceContentValidationService,
    IVssFrameworkService
  {
    private FileServiceContentValidationService.ContentValidationSettings m_settings;
    private const string c_area = "FileServiceContentValidationService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), "/Service/Framework/Settings/ContentValidation/*");
      Interlocked.CompareExchange<FileServiceContentValidationService.ContentValidationSettings>(ref this.m_settings, new FileServiceContentValidationService.ContentValidationSettings(systemRequestContext), (FileServiceContentValidationService.ContentValidationSettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    public bool IsEnabled(IVssRequestContext requestContext) => requestContext.GetService<IContentValidationService>().IsEnabled(requestContext);

    public void SaveMetadata(
      IVssRequestContext requestContext,
      int fileId,
      int dataspaceId,
      Guid uploader,
      string ipAddress = null,
      string originalFilename = null,
      ContentValidationScanType? scanType = null)
    {
      using (FileServiceContentValidationComponent component = requestContext.CreateComponent<FileServiceContentValidationComponent>())
        component.SaveMetadata(new FileServiceContentValidationMetadata()
        {
          FileId = fileId,
          DataspaceId = dataspaceId,
          Uploader = uploader,
          IPAddress = FileServiceContentValidationService.ExtractIPAddress(ipAddress),
          FileName = originalFilename,
          ScanType = scanType
        });
    }

    public async Task<FileServiceContentValidationResult> SubmitUnscannedFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string dataspaceCategory,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> knownIdentities)
    {
      Stopwatch totalTime = new Stopwatch();
      FileServiceContentValidationResult result = new FileServiceContentValidationResult();
      FileServiceContentValidationService.ContentValidationSettings settings = this.m_settings;
      TeamFoundationFileService fileService = requestContext.GetService<TeamFoundationFileService>();
      totalTime.Start();
      try
      {
        await this.SubmitUnscannedFilesInternalAsync(requestContext, projectId, dataspaceCategory, knownIdentities, result, settings, false);
        if (fileService.IsUsingSecondaryRange(requestContext, TeamFoundationFileService.GetOwnerIdFromCategory(dataspaceCategory)))
          await this.SubmitUnscannedFilesInternalAsync(requestContext, projectId, dataspaceCategory, knownIdentities, result, settings, true);
      }
      catch (DataspaceNotFoundException ex)
      {
        requestContext.Trace(15289521, TraceLevel.Info, nameof (FileServiceContentValidationService), nameof (SubmitUnscannedFilesAsync), "Dataspace {0} not found for project {1}", (object) dataspaceCategory, (object) projectId);
      }
      catch (Exception ex)
      {
        string message = string.Format("Caught exception while processing files. ServiceHost: {0}, PartitionId: {1}, ProjectId: {2}, Dataspace: {3}, Exception: {4}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.PartitionId, (object) projectId, (object) dataspaceCategory, (object) ex);
        requestContext.Trace(15289517, TraceLevel.Error, nameof (FileServiceContentValidationService), nameof (SubmitUnscannedFilesAsync), message);
        result.ErrorMessage = message;
      }
      totalTime.Stop();
      result.TotalTime = totalTime.ElapsedMilliseconds;
      FileServiceContentValidationResult validationResult = result;
      totalTime = (Stopwatch) null;
      result = (FileServiceContentValidationResult) null;
      settings = (FileServiceContentValidationService.ContentValidationSettings) null;
      fileService = (TeamFoundationFileService) null;
      return validationResult;
    }

    internal async Task SubmitUnscannedFilesInternalAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string dataspaceCategory,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> knownIdentities,
      FileServiceContentValidationResult result,
      FileServiceContentValidationService.ContentValidationSettings settings,
      bool scanNegativeFileIdRange)
    {
      Stopwatch submissionTime = new Stopwatch();
      IEnumerable<FileServiceContentValidationMetadata> entries = (IEnumerable<FileServiceContentValidationMetadata>) null;
      IContentValidationService contentValidationService = requestContext.GetService<IContentValidationService>();
      Uri baseUri = this.GetFileDownloadBaseUri(requestContext);
      int dataspaceId = this.GetProjectScopedDataspaceId(requestContext, projectId, dataspaceCategory);
      do
      {
        requestContext.CancellationToken.ThrowIfCancellationRequested();
        int fileIdWatermark;
        using (FileServiceContentValidationComponent component = requestContext.CreateComponent<FileServiceContentValidationComponent>())
        {
          if (component is FileServiceContentValidationComponent4 validationComponent4)
          {
            fileIdWatermark = validationComponent4.GetWatermark(dataspaceId, scanNegativeFileIdRange);
          }
          else
          {
            if (scanNegativeFileIdRange)
            {
              submissionTime = (Stopwatch) null;
              entries = (IEnumerable<FileServiceContentValidationMetadata>) null;
              contentValidationService = (IContentValidationService) null;
              baseUri = (Uri) null;
              return;
            }
            fileIdWatermark = component.GetWatermark(dataspaceId);
          }
          entries = (IEnumerable<FileServiceContentValidationMetadata>) component.QueryUnvalidatedFiles(dataspaceId, fileIdWatermark, settings.BatchSize);
        }
        if (!entries.Any<FileServiceContentValidationMetadata>())
        {
          requestContext.Trace(15289513, TraceLevel.Info, nameof (FileServiceContentValidationService), "SubmitUnscannedFilesAsync", "Skipping, no files to scan. ServiceHost: {0}, PartitionId: {1}, ProjectId: {2} DataspaceId: {3}, Watermark: {4}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.PartitionId, (object) projectId, (object) dataspaceId, (object) fileIdWatermark);
          break;
        }
        requestContext.Trace(15289514, TraceLevel.Info, nameof (FileServiceContentValidationService), "SubmitUnscannedFilesAsync", "Processing batch of {4} files. ServiceHost: {0}, PartitionId: {1}, ProjectId: {2}, DataspaceId: {3}, Watermark: {5}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.PartitionId, (object) projectId, (object) dataspaceId, (object) entries.Count<FileServiceContentValidationMetadata>(), (object) fileIdWatermark);
        foreach (FileServiceContentValidationMetadata entry in entries)
          this.DetermineScanType(requestContext, entry, result);
        IEnumerable<FileServiceContentValidationMetadata> validationMetadatas = entries.Where<FileServiceContentValidationMetadata>((Func<FileServiceContentValidationMetadata, bool>) (e => e.ScanType.HasValue && e.ScanType.Value != 0));
        if (validationMetadatas.Any<FileServiceContentValidationMetadata>())
        {
          SignableCollection<FileServiceContentValidationMetadata> fileCollection = new SignableCollection<FileServiceContentValidationMetadata>((SignableCollection<FileServiceContentValidationMetadata>.GetFileIdDelegate) (e => e.FileId));
          fileCollection.AddRange(validationMetadatas);
          fileCollection.Sign(requestContext, DateTime.UtcNow.AddDays((double) settings.ExpirationInDays));
          this.UpdateIdentities(requestContext, validationMetadatas.Select<FileServiceContentValidationMetadata, Guid>((Func<FileServiceContentValidationMetadata, Guid>) (e => e.Uploader)), knownIdentities, result);
          foreach (IGrouping<Tuple<Guid, string>, FileServiceContentValidationMetadata> group in validationMetadatas.GroupBy<FileServiceContentValidationMetadata, Tuple<Guid, string>>((Func<FileServiceContentValidationMetadata, Tuple<Guid, string>>) (e => Tuple.Create<Guid, string>(e.Uploader, e.IPAddress))))
          {
            Microsoft.VisualStudio.Services.Identity.Identity contentCreator = (Microsoft.VisualStudio.Services.Identity.Identity) null;
            if (group.Key.Item1 != Guid.Empty)
              knownIdentities.TryGetValue(group.Key.Item1, out contentCreator);
            List<ContentValidationKey> submissions = new List<ContentValidationKey>();
            foreach (FileServiceContentValidationMetadata validationMetadata in (IEnumerable<FileServiceContentValidationMetadata>) group)
              submissions.Add(new ContentValidationKey(new UriBuilder(baseUri)
              {
                Query = fileCollection.GetSignature(validationMetadata)
              }.Uri, validationMetadata.ScanType.Value, validationMetadata.FileName));
            if (submissions.Any<ContentValidationKey>())
            {
              requestContext.Trace(15289518, TraceLevel.Info, nameof (FileServiceContentValidationService), "SubmitUnscannedFilesAsync", "Submitting {3} files. PartitionId: {0}, ProjectId: {1}, DataspaceId: {2}", (object) requestContext.ServiceHost.PartitionId, (object) projectId, (object) dataspaceId, (object) submissions.Count);
              submissionTime.Start();
              await contentValidationService.SubmitAsync(requestContext, projectId, (IEnumerable<ContentValidationKey>) submissions, contentCreator, group.Key.Item2);
              submissionTime.Stop();
              result.SubmittedFiles += submissions.Count;
              ++result.CallsToCVS;
            }
            result.ProcessedFiles += group.Count<FileServiceContentValidationMetadata>();
            submissions = (List<ContentValidationKey>) null;
          }
          fileCollection = (SignableCollection<FileServiceContentValidationMetadata>) null;
        }
        fileIdWatermark = entries.Last<FileServiceContentValidationMetadata>().FileId;
        requestContext.Trace(15289515, TraceLevel.Info, nameof (FileServiceContentValidationService), "SubmitUnscannedFilesAsync", "Updating the watermark. ServiceHost: {0}, PartitionId: {1}, ProjectId: {2}, DataspaceId: {3}, NewWatermark: {4}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.PartitionId, (object) projectId, (object) dataspaceId, (object) fileIdWatermark);
        using (FileServiceContentValidationComponent component = requestContext.CreateComponent<FileServiceContentValidationComponent>())
          component.SetWatermark(dataspaceId, fileIdWatermark);
        requestContext.Trace(15289516, TraceLevel.Info, nameof (FileServiceContentValidationService), "SubmitUnscannedFilesAsync", "Finished processing batch of {4} files. ServiceHost: {0}, PartitionId: {1}, ProjectId: {2}, DataspaceId: {3}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.PartitionId, (object) projectId, (object) dataspaceId, (object) entries.Count<FileServiceContentValidationMetadata>());
        ++result.ProcessedBatches;
      }
      while (entries != null && entries.Count<FileServiceContentValidationMetadata>() == settings.BatchSize);
      result.SubmissionTime += submissionTime.ElapsedMilliseconds;
      submissionTime = (Stopwatch) null;
      entries = (IEnumerable<FileServiceContentValidationMetadata>) null;
      contentValidationService = (IContentValidationService) null;
      baseUri = (Uri) null;
    }

    public void CleanupMetadata(IVssRequestContext requestContext)
    {
      FileServiceContentValidationService.ContentValidationSettings settings = this.m_settings;
      using (FileServiceContentValidationComponent component = requestContext.CreateComponent<FileServiceContentValidationComponent>())
        component.CleanupMetadata(settings.RetentionPeriodInDays);
    }

    internal void DetermineScanType(
      IVssRequestContext requestContext,
      FileServiceContentValidationMetadata entry,
      FileServiceContentValidationResult result)
    {
      if (entry.ScanType.HasValue)
        return;
      ++result.ScanTypeQueries;
      using (Stream content = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) entry.FileId, false, out byte[] _, out long _, out CompressionType _))
        entry.ScanType = new ContentValidationScanType?(ContentValidationUtil.DetectScanTypeFromStream(content));
    }

    internal void UpdateIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Guid> unknownIdentities,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> knownIdentities,
      FileServiceContentValidationResult result)
    {
      unknownIdentities = unknownIdentities.Distinct<Guid>().Where<Guid>((Func<Guid, bool>) (id => id != Guid.Empty && !knownIdentities.ContainsKey(id)));
      if (!unknownIdentities.Any<Guid>())
        return;
      IdentityService service = requestContext.GetService<IdentityService>();
      requestContext.Trace(15289519, TraceLevel.Info, nameof (FileServiceContentValidationService), nameof (UpdateIdentities), "Resolving {2} identities. ServiceHost: {0}, PartitionId: {1}", (object) requestContext.ServiceHost.InstanceId, (object) requestContext.ServiceHost.PartitionId, (object) unknownIdentities.Count<Guid>());
      IVssRequestContext requestContext1 = requestContext;
      List<Guid> list = unknownIdentities.ToList<Guid>();
      IEnumerable<string> propertyNameFilters = Enumerable.Empty<string>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext1, (IList<Guid>) list, QueryMembership.None, propertyNameFilters))
      {
        knownIdentities.Add(readIdentity.Id, readIdentity);
        ++result.ResolvedIdentities;
      }
      result.UnresolvedIdentities += unknownIdentities.Where<Guid>((Func<Guid, bool>) (i => !knownIdentities.ContainsKey(i))).Count<Guid>();
    }

    internal static string ExtractIPAddress(string ipAddress)
    {
      if (ipAddress == null)
        return (string) null;
      if (ipAddress.Contains<char>(','))
        ipAddress = ipAddress.Split(',')[0].Trim();
      if (ipAddress.Length > 45)
        ipAddress = ipAddress.Substring(0, 45);
      return ipAddress;
    }

    internal Uri GetFileDownloadBaseUri(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "CvsFileDownload", CvsFileDownloadResourceIds.LocationId, (object) new object[0]);

    internal int GetProjectScopedDataspaceId(
      IVssRequestContext requestContext,
      Guid projectId,
      string dataspaceCategory)
    {
      return requestContext.GetService<IDataspaceService>().QueryDataspace(requestContext, dataspaceCategory, projectId, true).DataspaceId;
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection chanedEntries)
    {
      requestContext.TraceAlways(15289520, TraceLevel.Info, nameof (FileServiceContentValidationService), nameof (OnRegistrySettingsChanged), "Registry settings changed!");
      Volatile.Write<FileServiceContentValidationService.ContentValidationSettings>(ref this.m_settings, new FileServiceContentValidationService.ContentValidationSettings(requestContext));
    }

    internal class ContentValidationSettings
    {
      public readonly int BatchSize;
      public readonly int ExpirationInDays;
      public readonly int RetentionPeriodInDays;
      public const string RegistryPath = "/Service/Framework/Settings/ContentValidation";
      public const string BatchSizeRegistryName = "BatchSize";
      public const string ExpirationInDaysRegistryName = "ExpirationInDays";
      public const string RetentionPeriodInDaysRegistryName = "RetentionPeriodInDays";
      public const string RegistryFilter = "/Service/Framework/Settings/ContentValidation/*";
      private const int c_defaultBatchSize = 1000;
      private const int c_defaultExpirationInDays = 14;
      private const int c_defaultRetentionPeriodInDays = 14;

      public ContentValidationSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/Framework/Settings/ContentValidation/*");
        this.BatchSize = registryEntryCollection.GetValueFromPath<int>(nameof (BatchSize), 1000);
        this.ExpirationInDays = registryEntryCollection.GetValueFromPath<int>(nameof (ExpirationInDays), 14);
        this.RetentionPeriodInDays = registryEntryCollection.GetValueFromPath<int>(nameof (RetentionPeriodInDays), 14);
        if (this.BatchSize < 1)
        {
          requestContext.Trace(15289510, TraceLevel.Error, nameof (FileServiceContentValidationService), nameof (ContentValidationSettings), string.Format("Invalid batch size. Must be >= 1. Changing from {0} to {1}. Please correct the registry value at {2}/{3}.", (object) this.BatchSize, (object) 1000, (object) "/Service/Framework/Settings/ContentValidation", (object) nameof (BatchSize)));
          this.BatchSize = 1000;
        }
        if (this.ExpirationInDays < 1)
        {
          requestContext.Trace(15289511, TraceLevel.Error, nameof (FileServiceContentValidationService), nameof (ContentValidationSettings), string.Format("Invalid expiration days. Must be >= 1. Changing from {0} to {1}. Please correct the registry value at {2}/{3}.", (object) this.ExpirationInDays, (object) 14, (object) "/Service/Framework/Settings/ContentValidation", (object) nameof (ExpirationInDays)));
          this.ExpirationInDays = 14;
        }
        if (this.RetentionPeriodInDays >= 1)
          return;
        requestContext.Trace(15289612, TraceLevel.Error, nameof (FileServiceContentValidationService), nameof (ContentValidationSettings), string.Format("Invalid retention period. Must be >= 1. Changing from {0} to {1}. Please correct the registry value at {2}/{3}.", (object) this.RetentionPeriodInDays, (object) 14, (object) "/Service/Framework/Settings/ContentValidation", (object) nameof (RetentionPeriodInDays)));
        this.RetentionPeriodInDays = 14;
      }
    }
  }
}
