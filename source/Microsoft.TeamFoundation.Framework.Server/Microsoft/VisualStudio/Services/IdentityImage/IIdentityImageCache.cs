// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.IIdentityImageCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  public interface IIdentityImageCache
  {
    bool TryGetValue(
      IVssRequestContext requestContext,
      Guid identityId,
      out bool isContainer,
      out Guid imageId);

    bool Remove(IVssRequestContext requestContext, string dependencyKey);

    bool Remove(IVssRequestContext requestContext, Guid identityId);

    void Add(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid imageId,
      string dependencyKey);
  }
}
