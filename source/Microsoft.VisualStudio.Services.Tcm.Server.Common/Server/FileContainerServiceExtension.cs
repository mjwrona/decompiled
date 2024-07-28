// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FileContainerServiceExtension
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class FileContainerServiceExtension
  {
    public Microsoft.VisualStudio.Services.FileContainer.FileContainer CreateContainerIfNotExists(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      Guid projectId,
      string containerName)
    {
      Uri artifactUri = this.GetArtifactUri(pipelineContext, containerName);
      ITeamFoundationFileContainerService service = requestContext.RequestContext.GetService<ITeamFoundationFileContainerService>();
      Microsoft.VisualStudio.Services.FileContainer.FileContainer containerIfNotExists = service.QueryContainers(requestContext.RequestContext, (IList<Uri>) new Uri[1]
      {
        artifactUri
      }, projectId)?.Find((Predicate<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) (fc => string.Equals(fc.Name, containerName, StringComparison.OrdinalIgnoreCase)));
      if (containerIfNotExists != null)
        return containerIfNotExists;
      try
      {
        requestContext.Logger.Verbose(1015183, "FileContainerServiceExtension.CreateContainerIfNotExists: Container with name " + containerName + " not found. Creating one.");
        string uri = requestContext.ProjectServiceHelper.GetProjectFromGuid(projectId).Uri;
        long container = service.CreateContainer(requestContext.RequestContext, artifactUri, uri, containerName, string.Empty, projectId);
        return service.GetContainer(requestContext.RequestContext, container, projectId, false);
      }
      catch (ContainerAlreadyExistsException ex)
      {
        requestContext.Logger.Info(1015136, "FileContainerServiceExtension.CreateContainerIfNotExists: Container already exists.");
        return service.QueryContainers(requestContext.RequestContext, (IList<Uri>) new Uri[1]
        {
          artifactUri
        }, projectId).Find((Predicate<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) (fc => string.Equals(fc.Name, containerName, StringComparison.OrdinalIgnoreCase)));
      }
    }

    public string PublishCoverageAttachment(
      TestManagementRequestContext requestContext,
      Guid projectId,
      Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer,
      string localCoveragePath,
      string containerCoveragePath)
    {
      try
      {
        ITeamFoundationFileContainerService service = requestContext.RequestContext.GetService<ITeamFoundationFileContainerService>();
        using (FileStream fileStream = File.OpenRead(localCoveragePath))
        {
          byte[] md5 = MD5Util.CalculateMD5((Stream) fileStream);
          fileStream.Position = 0L;
          FileContainerItem fileContainerItem1 = new FileContainerItem()
          {
            ContainerId = fileContainer.Id,
            Path = containerCoveragePath,
            ItemType = ContainerItemType.File,
            FileLength = fileStream.Length,
            FileHash = md5,
            FileEncoding = 1,
            FileType = 1
          };
          ITeamFoundationFileContainerService containerService = service;
          IVssRequestContext requestContext1 = requestContext.RequestContext;
          long id = fileContainer.Id;
          List<FileContainerItem> items = new List<FileContainerItem>();
          items.Add(fileContainerItem1);
          Guid scopeIdentifier = projectId;
          FileContainerItem fileContainerItem2 = containerService.CreateItems(requestContext1, id, (IList<FileContainerItem>) items, scopeIdentifier)[0];
          FileContainerItem fileContainerItem3 = service.UploadFile(requestContext.RequestContext, fileContainerItem2.ContainerId, fileContainerItem2, (Stream) fileStream, 0L, fileStream.Length, CompressionType.None, projectId);
          requestContext.Logger.Info(1015120, string.Format("FileContainerServiceExtension: Uploaded merged coverage file: {0}. Status: {1}.", (object) fileContainerItem3.Path, (object) fileContainerItem3.Status));
          return FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}/_apis/testresults/codecoverage/download/{2}/{3}", (object) requestContext.RequestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext.RequestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker), (object) Uri.EscapeDataString(requestContext.ProjectServiceHelper.GetProjectFromGuid(projectId).Name), (object) fileContainer.Id, (object) Uri.EscapeDataString(fileContainerItem3.Path)));
        }
      }
      catch (Exception ex)
      {
        requestContext.Logger.Error(1015128, string.Format("Error occurred while uploading merge coverage file. {0}", (object) ex));
        return string.Empty;
      }
    }

    public void DeleteContainer(
      TestManagementRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> buildIds,
      string containerName,
      Dictionary<string, object> ciData)
    {
      try
      {
        List<long> longList = new List<long>();
        List<string> stringList = new List<string>();
        ITeamFoundationFileContainerService service = requestContext.RequestContext.GetService<ITeamFoundationFileContainerService>();
        requestContext.Logger.Info(1015127, string.Format("FileContainerServiceExtension.DeleteContainer:Deleting container {0}. Project:{1}. List of deleted build IDs: {2}", (object) containerName, (object) projectId, (object) JsonConvert.SerializeObject((object) buildIds)));
        foreach (int buildId in buildIds)
        {
          string artifactUriFilter = "vstfs:///Build/Build/" + buildId.ToString();
          Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = service.FilterContainers(requestContext.RequestContext, artifactUriFilter, projectId)?.Find((Predicate<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) (fc => string.Equals(fc.Name, "ModuleCoverage", StringComparison.OrdinalIgnoreCase)));
          if (fileContainer != null)
          {
            requestContext.Logger.Info(1015128, "FileContainerServiceExtension.DeleteContainer:Deleting container " + containerName + " from build: " + artifactUriFilter);
            service.DeleteContainer(requestContext.RequestContext, fileContainer.Id, projectId);
            longList.Add(fileContainer.Id);
            stringList.Add(fileContainer.ArtifactUri.ToString());
          }
        }
        requestContext.Logger.Info(1015129, "Deletion completed");
        ciData.Add("DeletedModuleCoverageContainerIds", (object) longList);
        ciData.Add("DeletedModuleCoverageContainerArtifactUris", (object) stringList);
      }
      catch (DataspaceNotFoundException ex)
      {
        requestContext.Logger.Info(1015131, string.Format("FileContainerServiceExtension.DeleteContainer:Deleting container {0} lead to DataSpaceNotFoundException: {1}", (object) containerName, (object) ex));
        ciData.Add("KnowException", (object) ex);
      }
    }

    private Uri GetArtifactUri(PipelineContext pipelineContext, string containerName) => new Uri(pipelineContext.Uri);
  }
}
