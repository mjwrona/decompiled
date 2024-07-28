// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IFileServiceContentValidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (FileServiceContentValidationService))]
  public interface IFileServiceContentValidationService : IVssFrameworkService
  {
    bool IsEnabled(IVssRequestContext requestContext);

    void SaveMetadata(
      IVssRequestContext requestContext,
      int fileId,
      int dataspaceId,
      Guid uploader,
      string ipAddress = null,
      string originalFilename = null,
      ContentValidationScanType? scanType = null);

    Task<FileServiceContentValidationResult> SubmitUnscannedFilesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      string dataspaceCategory,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> knownIdentities);

    void CleanupMetadata(IVssRequestContext requestContext);
  }
}
