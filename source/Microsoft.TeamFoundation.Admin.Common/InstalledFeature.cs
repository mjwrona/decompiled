// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.InstalledFeature
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;

namespace Microsoft.TeamFoundation.Admin
{
  internal class InstalledFeature : IFeature
  {
    private readonly string m_name;
    private readonly string m_displayName;
    private readonly string m_installPath;
    private readonly Version m_version;
    private readonly FeatureType m_type;

    public InstalledFeature(
      Version version,
      string name,
      string displayName,
      string installPath,
      bool isConfigured)
    {
      this.m_version = version;
      this.m_name = name;
      this.m_displayName = displayName;
      this.m_installPath = installPath;
      this.IsConfigured = isConfigured;
      this.m_type = (FeatureType) Enum.Parse(typeof (FeatureType), this.m_name);
    }

    public FeatureType Type => this.m_type;

    public Version Version => this.m_version;

    public string Name => this.m_name;

    public string DisplayName => this.m_displayName;

    public string InstallPath => this.m_installPath;

    public bool IsConfigured { get; set; }
  }
}
