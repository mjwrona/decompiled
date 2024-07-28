// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FileContainerExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class FileContainerExtensions
  {
    internal static FileContainerItem UploadFile(
      this ITeamFoundationFileContainerService service,
      IVssRequestContext requestContext,
      long containerId,
      string filePath,
      Stream stream,
      Guid scopeIdentifier)
    {
      return service.UploadFile(requestContext, containerId, filePath, stream, stream.Length, scopeIdentifier);
    }

    internal static FileContainerItem UploadFile(
      this ITeamFoundationFileContainerService service,
      IVssRequestContext requestContext,
      long containerId,
      string filePath,
      Stream stream,
      long length,
      Guid scopeIdentifier)
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
      }, scopeIdentifier).FirstOrDefault<FileContainerItem>();
      if (length > 0L)
        service.UploadFile(requestContext, containerId, fileContainerItem2, stream, 0L, length, CompressionType.None, scopeIdentifier);
      return fileContainerItem2;
    }

    internal static FileContainerItem UploadFileFromArtifact(
      this TeamFoundationFileContainerService service,
      IVssRequestContext requestContext,
      long containerId,
      string filePath,
      string artifactHash,
      long length,
      Guid scopeIdentifier)
    {
      FileContainerItem fileContainerItem = new FileContainerItem()
      {
        Path = filePath,
        FileLength = length,
        ItemType = ContainerItemType.File,
        FileType = 1,
        FileEncoding = 1,
        Status = ContainerItemStatus.Created
      };
      return service.CreateItemFromArtifact(requestContext, containerId, fileContainerItem, artifactHash, new Guid?(scopeIdentifier)).FirstOrDefault<FileContainerItem>();
    }
  }
}
