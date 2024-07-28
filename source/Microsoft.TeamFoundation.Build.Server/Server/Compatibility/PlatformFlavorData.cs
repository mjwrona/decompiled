// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.PlatformFlavorData
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [SoapType("PlatformFlavorData")]
  [ClassVisibility(ClientVisibility.Internal)]
  public class PlatformFlavorData : IEquatable<PlatformFlavorData>
  {
    private string m_platformName;
    private string m_flavorName;

    public PlatformFlavorData()
    {
    }

    public PlatformFlavorData(string platformName, string flavorName)
    {
      this.m_platformName = platformName;
      this.m_flavorName = flavorName;
    }

    public string PlatformName
    {
      get => this.m_platformName;
      set => this.m_platformName = value;
    }

    public string FlavorName
    {
      get => this.m_flavorName;
      set => this.m_flavorName = value;
    }

    public override int GetHashCode() => TFStringComparer.BuildPlatformFlavor.GetHashCode(this.m_flavorName) ^ TFStringComparer.BuildPlatformFlavor.GetHashCode(this.m_platformName) * 31;

    public override bool Equals(object obj) => this.Equals(obj as PlatformFlavorData);

    public bool Equals(PlatformFlavorData other) => TFStringComparer.BuildPlatformFlavor.Equals(this.m_platformName, other.m_platformName) && TFStringComparer.BuildPlatformFlavor.Equals(this.m_flavorName, other.m_flavorName);
  }
}
