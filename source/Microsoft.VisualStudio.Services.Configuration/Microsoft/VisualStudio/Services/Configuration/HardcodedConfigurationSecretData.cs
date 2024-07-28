// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.HardcodedConfigurationSecretData
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class HardcodedConfigurationSecretData : ConfigurationSecretData
  {
    public HardcodedConfigurationSecretData(string secretName, object value)
      : base(secretName, false)
    {
      this.Values = new object[1]{ value };
    }

    public HardcodedConfigurationSecretData(string secretName, object[] values)
      : base(secretName, true)
    {
      this.Values = values;
    }

    public object[] Values { get; private set; }

    public string CredentialName { get; set; }
  }
}
