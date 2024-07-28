// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WellKnownContainerArtifactVariables
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class WellKnownContainerArtifactVariables
  {
    public const string Registry = "registry";
    public const string Tag = "tag";
    public const string Digest = "digest";
    public const string Repository = "repository";
    public const string Location = "location";
    public const string Type = "type";
    public const string URI = "URI";
    private const string baseKey = "resources.container";

    public static string GetVariableKey(string alias, string key) => string.Format("{0}.{1}.{2}", (object) "resources.container", (object) alias, (object) key);
  }
}
