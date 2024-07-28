// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ISecureFileService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkSecureFileService))]
  public interface ISecureFileService : IVssFrameworkService
  {
    SecureFile UploadSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      string secureFileName,
      Stream content,
      bool authorizePipelines = false);

    Stream DownloadSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      string ticket);

    Task<SecureFile> GetSecureFileAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      bool includeDownloadTicket = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None);

    Task<IList<SecureFile>> GetSecureFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> secureFileIds,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None);

    Task<IList<SecureFile>> GetSecureFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> secureFileNames,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None);

    IList<SecureFile> GetSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      string secureFileNamePattern,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None);

    void DeleteSecureFile(IVssRequestContext requestContext, Guid projectId, Guid secureFileId);

    SecureFile UpdateSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      SecureFile secureFile);

    IList<SecureFile> UpdateSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<SecureFile> secureFiles);

    IList<SecureFile> QuerySecureFilesByProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      string condition,
      string secureFileNamePattern);
  }
}
