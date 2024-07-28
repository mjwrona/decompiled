// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.CustomArtifactFilter
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  public class CustomArtifactFilter
  {
    private IDictionary<string, string> m_filters;

    public CustomArtifactFilter(IDictionary<string, string> filters = null) => this.m_filters = filters;

    public bool IsFilterMatches(WebHookEventPayloadInputMapper inputMapper)
    {
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (this.m_filters ?? (IDictionary<string, string>) new Dictionary<string, string>()))
      {
        string fromEventPaylaod = inputMapper.GetValueFromEventPaylaod(keyValuePair.Key);
        if (!string.Equals(keyValuePair.Value, fromEventPaylaod, StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }
  }
}
