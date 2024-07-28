// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ResourceTypeNames
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class ResourceTypeNames
  {
    public const string ServiceEndpoint = "endpoint";
    public const string Queue = "queue";
    public const string SecureFile = "securefile";
    public const string VariableGroup = "variablegroup";
    private static readonly Dictionary<string, ResourceType> s_nameToValueMap = new Dictionary<string, ResourceType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "endpoint",
        ResourceType.ServiceEndpoint
      },
      {
        "queue",
        ResourceType.Queue
      },
      {
        "securefile",
        ResourceType.SecureFile
      },
      {
        "variablegroup",
        ResourceType.VariableGroup
      }
    };

    public static bool TryParse(string name, out ResourceType result)
    {
      result = (ResourceType) 0;
      return !string.IsNullOrEmpty(name) && ResourceTypeNames.s_nameToValueMap.TryGetValue(name, out result);
    }
  }
}
