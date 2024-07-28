// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.PipelineResourceExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts
{
  public static class PipelineResourceExtension
  {
    public static IDictionary<string, string> GetResourceInputs(this Resource resource)
    {
      IDictionary<string, string> resourceInputs = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, JToken> keyValuePair in (IEnumerable<KeyValuePair<string, JToken>>) resource.Properties.GetItems())
      {
        if (keyValuePair.Value.Type == JTokenType.String)
          resourceInputs[keyValuePair.Key] = keyValuePair.Value.ToString();
      }
      return resourceInputs;
    }
  }
}
