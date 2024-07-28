// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GetRequest
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class GetRequest : IValidatable
  {
    private ItemSpec m_itemSpec;
    private VersionSpec m_versionSpec;

    public ItemSpec ItemSpec
    {
      get => this.m_itemSpec;
      set => this.m_itemSpec = value;
    }

    public VersionSpec VersionSpec
    {
      get => this.m_versionSpec;
      set => this.m_versionSpec = value;
    }

    public override string ToString()
    {
      string str = string.Empty;
      if (this.ItemSpec != null)
        str = str + this.ItemSpec.ToString() + ";";
      if (this.VersionSpec != null)
        str += this.VersionSpec.ToString();
      return str;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.check((IValidatable) this.m_itemSpec, "ItemSpec", true);
      versionControlRequestContext.Validation.checkVersionSpec(this.m_versionSpec, "VersionSpec", false);
    }
  }
}
