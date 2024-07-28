// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Commerce.CommerceRequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Commerce;

namespace Microsoft.TeamFoundation.Framework.Server.Commerce
{
  public static class CommerceRequestContextExtensions
  {
    public static bool IsInfrastructureHost(this IVssRequestContext requestContext)
    {
      ServiceHostTags serviceHostTags = ServiceHostTags.FromString(requestContext.To(TeamFoundationHostType.Application).ServiceHost.Description);
      return serviceHostTags != ServiceHostTags.EmptyServiceHostTags && serviceHostTags.HasTag(WellKnownServiceHostTags.IsInfrastructureHost);
    }
  }
}
