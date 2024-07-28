// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.IClientToolStorageProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface IClientToolStorageProvider
  {
    PreauthenticatedUri? GenerateUri(
      IVssRequestContext requestContext,
      string fileName,
      ClientTool toolname,
      EdgeCache edgeCache);
  }
}
