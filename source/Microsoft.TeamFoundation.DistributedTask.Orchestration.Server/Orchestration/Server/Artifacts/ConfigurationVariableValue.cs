// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ConfigurationVariableValue
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class ConfigurationVariableValue
  {
    public ConfigurationVariableValue()
    {
    }

    public ConfigurationVariableValue(string value, bool isSecret)
      : this(value, isSecret, false)
    {
    }

    public ConfigurationVariableValue(string value, bool isSecret, bool allowOverride)
    {
      this.Value = value;
      this.IsSecret = isSecret;
      this.AllowOverride = allowOverride;
    }

    public string Value { get; set; }

    public bool IsSecret { get; set; }

    public bool AllowOverride { get; set; }
  }
}
