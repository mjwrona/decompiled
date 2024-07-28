// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders.ISourceProviderExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.SourceProviders
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ISourceProviderExtensions
  {
    public static bool SupportsCapability(this ISourceProvider sourceProvider, string capability)
    {
      bool flag = false;
      IDictionary<string, bool> supportedCapabilities = sourceProvider.Attributes.SupportedCapabilities;
      return ((supportedCapabilities != null ? (supportedCapabilities.TryGetValue(capability, out flag) ? 1 : 0) : 0) & (flag ? 1 : 0)) != 0;
    }
  }
}
