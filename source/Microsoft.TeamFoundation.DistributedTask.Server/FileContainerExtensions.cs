// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.FileContainerExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class FileContainerExtensions
  {
    private const string TaskDefinitionSecurityToken = "default";
    private const string TaskDefinitionContainerName = "TaskDefinitions";

    internal static long GetTaskContainerId(
      this ITeamFoundationFileContainerService service,
      IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = service.QueryContainers(requestContext, (IList<Uri>) new Uri[1]
      {
        TaskContainer.Uri
      }, Guid.Empty).FirstOrDefault<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
      long taskContainerId;
      if (fileContainer == null)
      {
        try
        {
          taskContainerId = service.CreateContainer(requestContext, TaskContainer.Uri, "default", "TaskDefinitions", (string) null, Guid.Empty);
        }
        catch (ContainerAlreadyExistsException ex)
        {
          taskContainerId = service.QueryContainers(requestContext, (IList<Uri>) new Uri[1]
          {
            TaskContainer.Uri
          }, Guid.Empty).FirstOrDefault<Microsoft.VisualStudio.Services.FileContainer.FileContainer>().Id;
        }
      }
      else
        taskContainerId = fileContainer.Id;
      return taskContainerId;
    }

    internal static FileContainerItem UploadFile(
      this ITeamFoundationFileContainerService service,
      IVssRequestContext requestContext,
      long containerId,
      string filePath,
      Stream stream)
    {
      return service.UploadFile(requestContext, containerId, filePath, stream, stream.Length);
    }

    internal static FileContainerItem UploadFile(
      this ITeamFoundationFileContainerService service,
      IVssRequestContext requestContext,
      long containerId,
      string filePath,
      Stream stream,
      long length)
    {
      FileContainerItem fileContainerItem1 = new FileContainerItem()
      {
        Path = filePath,
        FileLength = length,
        ItemType = ContainerItemType.File,
        FileType = 1,
        FileEncoding = 1
      };
      FileContainerItem fileContainerItem2 = service.CreateItems(requestContext, containerId, (IList<FileContainerItem>) new FileContainerItem[1]
      {
        fileContainerItem1
      }, Guid.Empty).FirstOrDefault<FileContainerItem>();
      if (length > 0L)
        service.UploadFile(requestContext, containerId, fileContainerItem2, stream, 0L, length, CompressionType.None, Guid.Empty);
      return fileContainerItem2;
    }
  }
}
