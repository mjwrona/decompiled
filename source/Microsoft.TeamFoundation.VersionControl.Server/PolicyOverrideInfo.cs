// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PolicyOverrideInfo
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Private)]
  public class PolicyOverrideInfo : IValidatable
  {
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Comment { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public PolicyFailureInfo[] PolicyFailures { get; set; }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.check((IValidatable[]) this.PolicyFailures, "PolicyFailures", true);
    }
  }
}
