// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestSettings
  {
    public TestSettings(int settingsId) => this.TestSettingsId = settingsId;

    public TestSettings(string name, string content, string areaPath)
    {
      this.TestSettingsName = name;
      this.TestSettingsContent = content;
      this.AreaPath = areaPath;
    }

    public TestSettings()
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int TestSettingsId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string TestSettingsName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string TestSettingsContent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AreaPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MachineRoles { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsPublic { get; set; }
  }
}
