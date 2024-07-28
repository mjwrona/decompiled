// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureJob
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FeatureJob
  {
    public FeatureJob(string featureName, string jobName, string areaPath)
    {
      this.FeatureName = featureName;
      this.JobName = jobName;
      this.AreaPath = areaPath;
    }

    public string FeatureName { get; }

    public string JobName { get; }

    public string AreaPath { get; }
  }
}
