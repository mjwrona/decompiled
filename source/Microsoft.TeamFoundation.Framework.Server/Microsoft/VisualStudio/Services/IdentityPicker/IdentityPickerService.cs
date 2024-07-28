// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.IdentityPickerService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.IdentityPicker.Operations;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  [DefaultServiceImplementation(typeof (FrameworkIdentityPickerService))]
  internal abstract class IdentityPickerService : IVssFrameworkService
  {
    public abstract void ServiceEnd(IVssRequestContext context);

    public abstract void ServiceStart(IVssRequestContext context);

    internal abstract SearchResponse Search(
      IVssRequestContext requestContext,
      SearchRequest request);

    internal abstract GetAvatarResponse GetAvatar(
      IVssRequestContext requestContext,
      GetAvatarRequest request);

    internal abstract GetConnectionsResponse GetConnections(
      IVssRequestContext requestContext,
      GetConnectionsRequest request);

    internal abstract GetMruResponse GetMru(
      IVssRequestContext requestContext,
      GetMruRequest request);

    internal abstract PatchMruResponse PatchMru(
      IVssRequestContext requestContext,
      PatchMruRequest request);
  }
}
