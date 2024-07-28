// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationFileContainerService
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationFileContainerService))]
  public interface ITeamFoundationFileContainerService : IVssFrameworkService
  {
    List<FileContainerItem> CopyFiles(
      IVssRequestContext requestContext,
      long containerId,
      IList<Tuple<string, string>> sourcesAndTargets,
      Guid scopeIdentifier,
      bool ignoreMissingSources = false,
      bool overwriteTargets = false);

    List<FileContainerItem> CopyFolder(
      IVssRequestContext requestContext,
      long containerId,
      string folderSourcePath,
      string folderTargetPath,
      Guid scopeIdentifier);

    long CreateContainer(
      IVssRequestContext requestContext,
      Uri artifactUri,
      string securityToken,
      string name,
      string description,
      Guid scopeIdentifier,
      ContainerOptions options = ContainerOptions.None);

    List<FileContainerItem> CreateItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<FileContainerItem> items,
      Guid scopeIdentifier);

    void DeleteContainer(IVssRequestContext requestContext, long containerId, Guid scopeIdentifier);

    void DeleteContainers(
      IVssRequestContext requestContext,
      IList<long> containerIds,
      Guid scopeIdentifier);

    void DeleteItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<string> paths,
      Guid scopeIdentifier);

    List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> FilterContainers(
      IVssRequestContext requestContext,
      string artifactUriFilter,
      Guid scopeIdentifier);

    Microsoft.VisualStudio.Services.FileContainer.FileContainer GetContainer(
      IVssRequestContext requestContext,
      long containerId,
      Guid scopeIdentifier,
      bool returnSize = true);

    List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IVssRequestContext requestContext,
      Guid scopeIdentifier);

    List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IVssRequestContext requestContext,
      IList<Uri> artifactUris,
      Guid scopeIdentifier);

    List<FileContainerItem> QueryItems(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Guid scopeIdentifier,
      bool isShallow = false,
      bool includeBlobMetadata = false);

    List<FileContainerItem> QuerySpecificItems(
      IVssRequestContext requestContext,
      long containerId,
      IEnumerable<string> paths,
      Guid scopeIdentifier);

    List<FileContainerItem> RenameFiles(
      IVssRequestContext requestContext,
      long containerId,
      IList<Tuple<string, string>> sourcesAndTargets,
      Guid scopeIdentifier,
      bool ignoreMissingSources = false,
      bool overwriteTargets = false);

    List<FileContainerItem> RenameFolder(
      IVssRequestContext requestContext,
      long containerId,
      string folderSourcePath,
      string folderTargetPath,
      Guid scopeIdentifier);

    FileContainerItem UploadFile(
      IVssRequestContext requestContext,
      long containerId,
      FileContainerItem item,
      Stream fileStream,
      long offsetFrom,
      long compressedLength,
      CompressionType compressionType,
      Guid scopeIdentifier,
      byte[] clientContentId = null);

    void WriteContents(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Stream outputStream,
      Guid scopeIdentifier,
      bool preserveServerTimestamp = false,
      bool saveAbsolutePath = true);

    (int numberOfFilesDeleted, int numberOfFailures, bool batchLimitReached) CleanupBlobReferences(
      IVssRequestContext requestContext);

    Stream RetrieveFile(
      IVssRequestContext requestContext,
      long containerId,
      Guid? scopeIdentifier,
      FileContainerItem item,
      bool allowCompression,
      out CompressionType compressionType);
  }
}
