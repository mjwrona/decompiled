// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ISecureFileServiceExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ISecureFileServiceExtensions
  {
    public static SecureFile GetSecureFile(
      this ISecureFileService service,
      IVssRequestContext requestContext,
      Guid projectId,
      Guid secureFileId,
      bool includeDownloadTicket = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      return requestContext.RunSynchronously<SecureFile>((Func<Task<SecureFile>>) (() => service.GetSecureFileAsync(requestContext, projectId, secureFileId, includeDownloadTicket, actionFilter)));
    }

    public static IList<SecureFile> GetSecureFiles(
      this ISecureFileService service,
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> secureFileIds,
      bool includeDownloadTickets = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      return requestContext.RunSynchronously<IList<SecureFile>>((Func<Task<IList<SecureFile>>>) (() => service.GetSecureFilesAsync(requestContext, projectId, secureFileIds, includeDownloadTickets, actionFilter)));
    }

    public static IList<SecureFile> GetSecureFiles(
      this ISecureFileService service,
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> secureFileNames,
      bool includeDownloadTicket = false,
      SecureFileActionFilter actionFilter = SecureFileActionFilter.None)
    {
      return requestContext.RunSynchronously<IList<SecureFile>>((Func<Task<IList<SecureFile>>>) (() => service.GetSecureFilesAsync(requestContext, projectId, secureFileNames, includeDownloadTicket, actionFilter)));
    }
  }
}
