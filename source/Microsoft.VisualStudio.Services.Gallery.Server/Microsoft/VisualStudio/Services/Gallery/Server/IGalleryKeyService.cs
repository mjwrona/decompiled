// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IGalleryKeyService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (GalleryKeyService))]
  public interface IGalleryKeyService : IVssFrameworkService
  {
    void CreateKey(
      IVssRequestContext requestContext,
      string keyName,
      int keyLength,
      int expireCurrentSeconds = -1);

    string ReadKey(IVssRequestContext requestContext, string keyName, bool allowCachedRead = true);

    IEnumerable<string> ReadKeys(
      IVssRequestContext requestContext,
      string keyName,
      bool allowCachedRead = true);
  }
}
