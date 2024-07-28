// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.ConfigurationScopesByVersion
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ConfigurationScopesByVersion
  {
    public ConfigurationScopesByVersion(int policyConfigurationId, int versionId, string scope)
    {
      this.PolicyConfigurationId = policyConfigurationId;
      this.VersionId = versionId;
      this.Scope = scope;
    }

    public int PolicyConfigurationId { get; set; }

    public int VersionId { get; set; }

    public string Scope { get; set; }
  }
}
