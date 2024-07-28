// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DiagnosticBlobUploader
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public class DiagnosticBlobUploader : IDiagnosticBlobUploader, IVssFrameworkService
  {
    private IVssRequestContext requestContext;

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.requestContext = systemRequestContext;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public bool TryUploadBlobToDiagnosticStorageAccount(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      IEnumerable<string> uploadPayload,
      string blobName,
      string containerName)
    {
      return containerName != null ? StorageAccountUtilities.TryUploadBlobToDiagnosticStorageAccount(requestContext ?? this.requestContext, tracer, uploadPayload, blobName, containerName) : StorageAccountUtilities.TryUploadBlobToDiagnosticStorageAccount(requestContext ?? this.requestContext, tracer, uploadPayload, blobName);
    }
  }
}
