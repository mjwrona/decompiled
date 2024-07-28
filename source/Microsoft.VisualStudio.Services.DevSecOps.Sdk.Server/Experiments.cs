// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Experiments
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class Experiments
  {
    public static bool IsInExperimentGroup(
      IReadOnlyCollection<string> experimentGroups,
      string experimentId)
    {
      string str = experimentId + "/user";
      return experimentGroups != null && experimentGroups.Contains<string>(str);
    }
  }
}
