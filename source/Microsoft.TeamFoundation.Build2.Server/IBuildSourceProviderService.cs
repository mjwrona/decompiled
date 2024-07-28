// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildSourceProviderService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DefaultServiceImplementation(typeof (BuildSourceProviderService))]
  public interface IBuildSourceProviderService : IVssFrameworkService
  {
    IBuildSourceProvider GetSourceProvider(
      IVssRequestContext requestContext,
      string repositoryType,
      bool throwIfNotFound = true);

    IEnumerable<IBuildSourceProvider> GetSourceProviders(IVssRequestContext requestContext);
  }
}
