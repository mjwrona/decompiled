// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.SemanticVersion
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class SemanticVersion : DashboardSecuredObject
  {
    [DataMember]
    public int Major { get; set; }

    [DataMember]
    public int Minor { get; set; }

    [DataMember]
    public int Patch { get; set; }

    public SemanticVersion()
    {
    }

    public SemanticVersion(int major, int minor, int patch)
    {
      this.Major = major;
      this.Minor = minor;
      this.Patch = patch;
    }

    public SemanticVersion(string version)
    {
      string[] strArray = version.Split('.');
      this.Major = int.Parse(strArray[0]);
      this.Minor = int.Parse(strArray[1]);
      this.Patch = int.Parse(strArray[2]);
    }

    public static SemanticVersion Default => new SemanticVersion(1, 0, 0);

    public override string ToString() => string.Format("{0}.{1}.{2}", (object) this.Major, (object) this.Minor, (object) this.Patch);
  }
}
