// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.NamedWebSessionTokenProviderService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  internal class NamedWebSessionTokenProviderService : IVssFrameworkService
  {
    private IDictionary<string, INamedWebSessionTokenProvider> m_providers;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      using (IDisposableReadOnlyList<INamedWebSessionTokenProvider> extensions = systemRequestContext.GetExtensions<INamedWebSessionTokenProvider>(throwOnError: true))
        this.m_providers = (IDictionary<string, INamedWebSessionTokenProvider>) extensions.ToDictionary<INamedWebSessionTokenProvider, string>((Func<INamedWebSessionTokenProvider, string>) (provider => provider.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public INamedWebSessionTokenProvider GetTokenProvider(string namedTokenId)
    {
      INamedWebSessionTokenProvider tokenProvider = (INamedWebSessionTokenProvider) null;
      this.m_providers.TryGetValue(namedTokenId, out tokenProvider);
      return tokenProvider;
    }
  }
}
