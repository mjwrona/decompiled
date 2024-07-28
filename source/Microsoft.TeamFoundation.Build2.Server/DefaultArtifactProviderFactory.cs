// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DefaultArtifactProviderFactory
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class DefaultArtifactProviderFactory : IArtifactProviderFactory, IDisposable
  {
    private IDictionary<string, IArtifactProvider> m_artifactProviderMap;
    private IDisposableReadOnlyList<IArtifactProvider> m_artifactProviders;

    public bool TryGetArtifactProvider(
      IVssRequestContext requestContext,
      string type,
      out IArtifactProvider artifactProvider)
    {
      if (this.m_artifactProviderMap == null)
      {
        Dictionary<string, IArtifactProvider> dictionary = new Dictionary<string, IArtifactProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_artifactProviders = requestContext.GetExtensions<IArtifactProvider>();
        foreach (IArtifactProvider artifactProvider1 in (IEnumerable<IArtifactProvider>) this.m_artifactProviders)
        {
          foreach (string supportedArtifactType in artifactProvider1.SupportedArtifactTypes)
          {
            if (!dictionary.ContainsKey(supportedArtifactType))
              dictionary.Add(supportedArtifactType, artifactProvider1);
            else
              requestContext.TraceWarning("Service", "Multiple artifact providers are present for artifactType {0}. The first one found will win.");
          }
        }
        this.m_artifactProviderMap = (IDictionary<string, IArtifactProvider>) dictionary;
      }
      artifactProvider = (IArtifactProvider) null;
      return type != null && this.m_artifactProviderMap.TryGetValue(type, out artifactProvider);
    }

    void IDisposable.Dispose()
    {
      if (this.m_artifactProviderMap != null)
      {
        this.m_artifactProviderMap.Clear();
        this.m_artifactProviderMap = (IDictionary<string, IArtifactProvider>) null;
      }
      if (this.m_artifactProviders == null)
        return;
      this.m_artifactProviders.Dispose();
      this.m_artifactProviders = (IDisposableReadOnlyList<IArtifactProvider>) null;
    }
  }
}
