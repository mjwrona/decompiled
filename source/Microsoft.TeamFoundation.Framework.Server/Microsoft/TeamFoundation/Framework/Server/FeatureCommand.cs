// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureCommand
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FeatureCommand
  {
    public FeatureCommand(string featureName, string application, string command, string areaPath)
    {
      this.FeatureName = featureName;
      this.Application = application;
      this.Command = command;
      this.AreaPath = areaPath;
    }

    public string FeatureName { get; }

    public string Application { get; }

    public string Command { get; }

    public string AreaPath { get; }
  }
}
