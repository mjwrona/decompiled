// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.IGalleryDomainInfo
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  [InheritedExport]
  public interface IGalleryDomainInfo
  {
    GalleryUserDomainInfo GetDomainInfo(IVssRequestContext requestContext);
  }
}
