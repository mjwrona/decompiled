// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IRepositorySigningService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.SigningTool;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (RepositorySigningService))]
  internal interface IRepositorySigningService : IVssFrameworkService
  {
    ReturnCode GenerateSigningManifestFile(
      IVssRequestContext requestContext,
      string packageFilePath,
      string manifestFilePath);

    ReturnCode GenerateSignatureArchive(
      IVssRequestContext requestContext,
      string manifestFilePath,
      string signatureFilePath,
      string signatureArchiveFilePath);

    ReturnCode VerifyPackageSignature(
      IVssRequestContext requestContext,
      string packageFilePath,
      string signatureArchiveFilePath);

    bool DownloadFile(IVssRequestContext requestContext, string fileUrl, string filePath);

    void DeleteRepositorySigningFiles(List<string> filesToBeDeleted);
  }
}
