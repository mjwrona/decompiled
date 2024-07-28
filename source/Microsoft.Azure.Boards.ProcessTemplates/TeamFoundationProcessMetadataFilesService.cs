// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.TeamFoundationProcessMetadataFilesService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class TeamFoundationProcessMetadataFilesService : 
    ITeamFoundationProcessMetadataFilesService,
    IVssFrameworkService
  {
    private const string s_Area = "TeamFoundationProcessMetadataFilesService";
    private const string s_Layer = "Service";

    public bool CreateOrUpdateProcessMetadataFiles(
      IVssRequestContext requestContext,
      ProcessMetadataFile[] files)
    {
      IVssRequestContext deploymentLevelContext = this.GetDeploymentLevelContext(requestContext);
      ITeamFoundationFileService service = deploymentLevelContext.GetService<ITeamFoundationFileService>();
      Dictionary<ProcessMetadataFile, ProcessMetadataFileEntry> source = new Dictionary<ProcessMetadataFile, ProcessMetadataFileEntry>();
      foreach (ProcessMetadataFile file in files)
        source.Add(file, new ProcessMetadataFileEntry()
        {
          ProcessTypeId = file.ProcessTypeId,
          ResourceName = file.ResourceName,
          ResourceType = file.ResourceType.ToString()
        });
      IEnumerable<ProcessMetadataFileEntry> processMetadataFiles;
      using (ProcessMetadataFilesComponent metadataFilesComponent = this.CreateProcessMetadataFilesComponent(deploymentLevelContext))
        processMetadataFiles = metadataFilesComponent.GetProcessMetadataFiles(source.Select<KeyValuePair<ProcessMetadataFile, ProcessMetadataFileEntry>, ProcessMetadataFileEntry>((Func<KeyValuePair<ProcessMetadataFile, ProcessMetadataFileEntry>, ProcessMetadataFileEntry>) (r => r.Value)).ToArray<ProcessMetadataFileEntry>());
      List<ProcessMetadataFileEntry> metadataFileEntryList = new List<ProcessMetadataFileEntry>();
      foreach (KeyValuePair<ProcessMetadataFile, ProcessMetadataFileEntry> keyValuePair in source)
      {
        KeyValuePair<ProcessMetadataFile, ProcessMetadataFileEntry> candidate = keyValuePair;
        byte[] numArray = (byte[]) null;
        ProcessMetadataFileEntry metadataFileEntry1 = processMetadataFiles.SingleOrDefault<ProcessMetadataFileEntry>((Func<ProcessMetadataFileEntry, bool>) (er => er.ProcessTypeId == candidate.Value.ProcessTypeId && er.ResourceName == candidate.Value.ResourceName && er.ResourceType == candidate.Value.ResourceType));
        if (metadataFileEntry1 != null)
        {
          candidate.Key.ResourceStream.Seek(0L, SeekOrigin.Begin);
          numArray = MD5Util.CalculateMD5(candidate.Key.ResourceStream, true);
          if (((IEnumerable<byte>) service.GetFileStatistics(deploymentLevelContext, (long) metadataFileEntry1.FileId).HashValue).SequenceEqual<byte>((IEnumerable<byte>) numArray))
            continue;
        }
        candidate.Key.ResourceStream.Seek(0L, SeekOrigin.Begin);
        if (numArray == null)
          numArray = MD5Util.CalculateMD5(candidate.Key.ResourceStream, true);
        int fileId = 0;
        long length = candidate.Key.ResourceStream.Length;
        service.UploadFile(requestContext, ref fileId, candidate.Key.ResourceStream, numArray, length, length, 0L, CompressionType.None, OwnerId.ProcessTemplate, Guid.Empty, "", false, deploymentLevelContext.ExecutionEnvironment.IsCloudDeployment);
        ProcessMetadataFileEntry metadataFileEntry2 = candidate.Value;
        metadataFileEntry2.FileId = fileId;
        metadataFileEntryList.Add(metadataFileEntry2);
      }
      if (metadataFileEntryList.Count <= 0)
        return false;
      using (ProcessMetadataFilesComponent metadataFilesComponent = this.CreateProcessMetadataFilesComponent(deploymentLevelContext))
        metadataFilesComponent.AddUpdateMetadataFiles(metadataFileEntryList.ToArray());
      return true;
    }

    public Stream GetProcessMetadataFile(
      IVssRequestContext requestContext,
      Guid processTypeId,
      ProcessMetadataResourceType resourceType,
      string resourceName)
    {
      return (Stream) requestContext.TraceBlock<FileStreamWrapper>(100052010, 100052011, "ProcessTemplate", nameof (TeamFoundationProcessMetadataFilesService), nameof (GetProcessMetadataFile), (Func<FileStreamWrapper>) (() =>
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        try
        {
          int fileId = this.GetFileId(vssRequestContext, processTypeId, resourceType, resourceName);
          ITeamFoundationFileCacheService service = vssRequestContext.GetService<ITeamFoundationFileCacheService>();
          using (FileStreamDownloadState streamDownloadState = new FileStreamDownloadState(vssRequestContext))
          {
            FileInformation fileInfo = new FileInformation(vssRequestContext.ServiceHost.InstanceId, fileId, (byte[]) null);
            service.RetrieveFile<FileInformation>(vssRequestContext, fileInfo, (IDownloadState<FileInformation>) streamDownloadState, false);
            return new FileStreamWrapper((Stream) streamDownloadState.FileStream);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(100052008, nameof (TeamFoundationProcessMetadataFilesService), "Service", ex);
          throw;
        }
      }));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private ProcessMetadataFilesComponent CreateProcessMetadataFilesComponent(
      IVssRequestContext requestContext)
    {
      return this.GetDeploymentLevelContext(requestContext).CreateComponent<ProcessMetadataFilesComponent>();
    }

    protected virtual IVssRequestContext GetDeploymentLevelContext(IVssRequestContext requestContext) => !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.To(TeamFoundationHostType.Deployment) : requestContext;

    public int GetFileId(
      IVssRequestContext deploymentContext,
      Guid processTypeId,
      ProcessMetadataResourceType resourceType,
      string resourceName)
    {
      int fileId;
      if (!deploymentContext.GetService<ProcessMetadataFileIdCache>().TryGetProcessMetadataFileId(deploymentContext, processTypeId, resourceType.ToString(), resourceName, out fileId))
      {
        deploymentContext.Trace(100052009, TraceLevel.Error, nameof (TeamFoundationProcessMetadataFilesService), "GetProcessMetadataFile", string.Format("FileId is not found for processId: {0}, ResourceType: {1}, ResourceName: {2}", (object) processTypeId, (object) resourceType, (object) resourceName));
        throw new UnableToFindMetadataResourceForProcess(resourceType.ToString(), resourceName, processTypeId);
      }
      return fileId;
    }
  }
}
