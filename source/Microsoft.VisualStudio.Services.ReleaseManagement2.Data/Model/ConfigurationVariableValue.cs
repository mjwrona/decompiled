// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
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
