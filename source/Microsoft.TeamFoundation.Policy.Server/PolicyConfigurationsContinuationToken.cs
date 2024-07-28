// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyConfigurationsContinuationToken
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

namespace Microsoft.TeamFoundation.Policy.Server
{
  public sealed class PolicyConfigurationsContinuationToken
  {
    public PolicyConfigurationsContinuationToken(int nextConfigurationId) => this.NextConfigurationId = nextConfigurationId;

    public static bool TryParseContinuationToken(
      string rawToken,
      out PolicyConfigurationsContinuationToken token)
    {
      token = (PolicyConfigurationsContinuationToken) null;
      int result;
      if (string.IsNullOrWhiteSpace(rawToken) || !int.TryParse(rawToken, out result) || result <= 0)
        return false;
      token = new PolicyConfigurationsContinuationToken(result);
      return true;
    }

    public override string ToString() => this.NextConfigurationId.ToString();

    public int NextConfigurationId { get; private set; }
  }
}
