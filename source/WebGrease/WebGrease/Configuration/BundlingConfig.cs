// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.BundlingConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  public class BundlingConfig : INamedConfig
  {
    public BundlingConfig() => this.ShouldBundleFiles = true;

    internal BundlingConfig(XElement element)
      : this()
    {
      this.Name = (string) element.Attribute((XName) "config") ?? string.Empty;
      foreach (XElement descendant in element.Descendants())
      {
        string str = descendant.Name.ToString();
        string textToParse = descendant.Value;
        switch (str)
        {
          case "AssembleFiles":
            this.ShouldBundleFiles = textToParse.TryParseBool();
            continue;
          case nameof (MinimalOutput):
            this.MinimalOutput = textToParse.TryParseBool();
            continue;
          default:
            continue;
        }
      }
    }

    public bool ShouldBundleFiles { get; private set; }

    public bool MinimalOutput { get; private set; }

    public string Name { get; private set; }
  }
}
