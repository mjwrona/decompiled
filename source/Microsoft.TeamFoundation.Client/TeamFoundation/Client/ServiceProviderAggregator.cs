// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ServiceProviderAggregator
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServiceProviderAggregator : IServiceProvider
  {
    private object[] m_providers;

    public ServiceProviderAggregator(params object[] list) => this.m_providers = list;

    public object GetService(Type type)
    {
      object service = (object) null;
      for (int index = 0; index < this.m_providers.Length && service == null; ++index)
      {
        if (this.m_providers[index] != null && this.m_providers[index] is IServiceProvider provider)
          service = provider.GetService(type);
      }
      return service;
    }

    [Conditional("DEBUG")]
    private void CheckList()
    {
    }
  }
}
