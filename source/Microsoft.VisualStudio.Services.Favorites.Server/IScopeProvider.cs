// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.IScopeProvider
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Favorites.Server
{
  [InheritedExport]
  public interface IScopeProvider
  {
    IEnumerable<Favorite> AppendScopeInfo(
      IVssRequestContext requestContext,
      IEnumerable<Favorite> favorites);
  }
}
