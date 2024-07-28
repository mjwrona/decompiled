// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IBlobStoreLogService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (BlobStoreLogService))]
  public interface IBlobStoreLogService : IVssFrameworkService
  {
    Task<BlobIdentifier> AddLogReferenceAsync(
      IVssRequestContext requestContext,
      Guid planId,
      int logId,
      int pageId,
      string blobFileId);

    Task<Stream> GetLogStreamAsync(IVssRequestContext requestContext, string blobFileId);

    Task DeleteLogReferencesAsync(
      IVssRequestContext requestContext,
      IEnumerable<LogBlobIdentifier> logBlobs);

    Task<string> UploadLogAsync(
      IVssRequestContext requestContext,
      Guid planId,
      int logId,
      int pageId,
      Stream stream);
  }
}
