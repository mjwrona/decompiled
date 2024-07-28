// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.GlobalConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Xml.Linq;

namespace WebGrease.Configuration
{
  public class GlobalConfig : INamedConfig
  {
    public GlobalConfig(XElement settingElement)
    {
      this.Name = (string) settingElement.Attribute((XName) "config") ?? string.Empty;
      bool? nullable = (bool?) settingElement.Attribute((XName) "treatWarningsAsErrors");
      this.TreatWarningsAsErrors = nullable.HasValue ? new bool?(nullable.GetValueOrDefault()) : (bool?) settingElement.Element((XName) nameof (TreatWarningsAsErrors));
    }

    public GlobalConfig()
    {
    }

    public bool? TreatWarningsAsErrors { get; private set; }

    public string Name { get; private set; }
  }
}
