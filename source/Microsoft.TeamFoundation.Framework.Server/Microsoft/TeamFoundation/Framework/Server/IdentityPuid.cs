// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentityPuid
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IdentityPuid
  {
    public const string AadPuidPrefix = "aad:";

    public string Value { get; private set; }

    public IdentityPuidType PuidType { get; private set; }

    public IdentityPuid(string rawPuid)
    {
      this.Value = string.Empty;
      this.PuidType = IdentityPuidType.Msa;
      if (string.IsNullOrEmpty(rawPuid))
        return;
      if (rawPuid.StartsWith("aad:"))
      {
        this.Value = rawPuid.Substring("aad:".Length);
        this.PuidType = IdentityPuidType.OrgId;
      }
      else
      {
        this.Value = rawPuid;
        this.PuidType = IdentityPuidType.Msa;
      }
    }
  }
}
