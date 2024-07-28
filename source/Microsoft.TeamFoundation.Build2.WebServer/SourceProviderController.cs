// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.SourceProviderController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "sourceProviders")]
  public class SourceProviderController : BuildApiController
  {
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes> ListSourceProviders() => this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvidersForExecutionEnvironment(this.TfsRequestContext).Select<IBuildSourceProvider, Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes>((Func<IBuildSourceProvider, Microsoft.TeamFoundation.Build.WebApi.SourceProviderAttributes>) (sp => sp.GetAttributes(this.TfsRequestContext).ToWebApiSourceProviderAttributes()));
  }
}
