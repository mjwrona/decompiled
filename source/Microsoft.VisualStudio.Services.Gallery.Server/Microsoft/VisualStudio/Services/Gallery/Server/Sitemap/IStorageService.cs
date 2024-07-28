// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Sitemap.IStorageService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Sitemap
{
  [DefaultServiceImplementation(typeof (BlobStorageService))]
  internal interface IStorageService : IVssFrameworkService
  {
    Stream RetrieveFile(IVssRequestContext requestContext, int fileId);

    int UploadFile(IVssRequestContext requestContext, Stream content, string fileName);

    void DeleteFile(IVssRequestContext requestContext, int fileId);
  }
}
