// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkSecureFileService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class FrameworkSecureFileService : ISecureFileService, IVssFrameworkService
  {
    private const string c_layer = "FrameworkSecureFileService";

    public SecureFile UploadSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      string secureFileName,
      Stream content,
      bool authorizePipelines = false)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (UploadSecureFile)))
        return requestContext.GetClient<TaskAgentHttpClient>().UploadSecureFileAsync(projectId, content, secureFileName, new bool?(authorizePipelines)).SyncResult<SecureFile>();
    }

    public Stream DownloadSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      string ticket)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (DownloadSecureFile)))
        return requestContext.GetClient<TaskAgentHttpClient>().DownloadSecureFileAsync(projectId, secureFileId, ticket).SyncResult<Stream>();
    }

    public async Task<SecureFile> GetSecureFileAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      bool includeDownloadTicket = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      SecureFile secureFileAsync;
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (GetSecureFileAsync)))
        secureFileAsync = await requestContext.GetClient<TaskAgentHttpClient>().GetSecureFileAsync(projectId, secureFileId, new bool?(includeDownloadTicket), new SecureFileActionFilter?(actionFilter), (object) null, new CancellationToken());
      return secureFileAsync;
    }

    public async Task<IList<SecureFile>> GetSecureFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> secureFileIds,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      IList<SecureFile> secureFilesByIdsAsync;
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (GetSecureFilesAsync)))
        secureFilesByIdsAsync = (IList<SecureFile>) await requestContext.GetClient<TaskAgentHttpClient>().GetSecureFilesByIdsAsync(projectId, secureFileIds, new bool?(includeDownloadTickets), new SecureFileActionFilter?(actionFilter), (object) null, new CancellationToken());
      return secureFilesByIdsAsync;
    }

    public async Task<IList<SecureFile>> GetSecureFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> secureFileNames,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      IList<SecureFile> filesByNamesAsync;
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (GetSecureFilesAsync)))
        filesByNamesAsync = (IList<SecureFile>) await requestContext.GetClient<TaskAgentHttpClient>().GetSecureFilesByNamesAsync(projectId, secureFileNames, new bool?(includeDownloadTickets), new SecureFileActionFilter?(actionFilter));
      return filesByNamesAsync;
    }

    public IList<SecureFile> GetSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      string secureFileNamePattern,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (GetSecureFiles)))
        return (IList<SecureFile>) requestContext.GetClient<TaskAgentHttpClient>().GetSecureFilesAsync(projectId, secureFileNamePattern, new bool?(includeDownloadTickets), new SecureFileActionFilter?(actionFilter), (object) null, new CancellationToken()).SyncResult<List<SecureFile>>();
    }

    public void DeleteSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (DeleteSecureFile)))
        requestContext.GetClient<TaskAgentHttpClient>().DeleteSecureFileAsync(projectId, secureFileId).SyncResult();
    }

    public SecureFile UpdateSecureFile(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      SecureFile secureFile)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (UpdateSecureFile)))
        return requestContext.GetClient<TaskAgentHttpClient>().UpdateSecureFileAsync(projectId, secureFileId, secureFile).SyncResult<SecureFile>();
    }

    public IList<SecureFile> UpdateSecureFiles(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<SecureFile> secureFiles)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (UpdateSecureFiles)))
        return (IList<SecureFile>) requestContext.GetClient<TaskAgentHttpClient>().UpdateSecureFilesAsync(projectId, (IEnumerable<SecureFile>) secureFiles).SyncResult<List<SecureFile>>();
    }

    public IList<SecureFile> QuerySecureFilesByProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      string condition,
      string secureFileNamePattern = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkSecureFileService), nameof (QuerySecureFilesByProperties)))
        return (IList<SecureFile>) requestContext.GetClient<TaskAgentHttpClient>().QuerySecureFilesByPropertiesAsync(projectId, condition, secureFileNamePattern).SyncResult<List<SecureFile>>();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
