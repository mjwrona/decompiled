// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.RegistryManagerV2
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class RegistryManagerV2 : RegistryManager
  {
    public RegistryManagerV2()
    {
    }

    public RegistryManagerV2(IVssRequestContext requestContext, string traceLayer)
      : base(requestContext, traceLayer)
    {
    }

    public override string GetRegistryPath(string featureName, string entityPathId) => this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) featureName, (object) this.CollectionId, (object) entityPathId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}", (object) featureName, (object) entityPathId);

    public override string GetRegistryPathPattern(string featureName) => this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/*", (object) featureName, (object) this.CollectionId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/*", (object) featureName);
  }
}
