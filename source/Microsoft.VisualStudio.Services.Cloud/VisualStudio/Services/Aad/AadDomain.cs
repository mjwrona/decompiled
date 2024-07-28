// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadDomain
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Aad
{
  public class AadDomain
  {
    private string name;
    private bool isDefault;

    public AadDomain()
    {
    }

    private AadDomain(string name, bool isDefault)
    {
      this.name = name;
      this.IsDefault = isDefault;
    }

    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    public bool IsDefault
    {
      get => this.isDefault;
      set => this.isDefault = value;
    }

    public class Factory
    {
      public AadDomain Create() => new AadDomain(this.Name, this.IsDefault);

      public string Name { get; set; }

      public bool IsDefault { get; set; }
    }
  }
}
