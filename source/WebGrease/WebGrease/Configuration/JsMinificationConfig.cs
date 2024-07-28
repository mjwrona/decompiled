// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.JsMinificationConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  internal sealed class JsMinificationConfig : INamedConfig
  {
    public JsMinificationConfig()
    {
      this.ShouldMinify = true;
      this.GlobalsToIgnore = "jQuery";
      this.MinificationArugments = "";
    }

    internal JsMinificationConfig(XElement element)
      : this()
    {
      this.Name = (string) element.Attribute((XName) "config") ?? string.Empty;
      foreach (XElement descendant in element.Descendants())
      {
        string str1 = descendant.Name.ToString();
        string str2 = descendant.Value;
        switch (str1)
        {
          case "Minify":
            this.ShouldMinify = str2.TryParseBool();
            continue;
          case nameof (GlobalsToIgnore):
            this.GlobalsToIgnore = !str2.IsNullOrWhitespace() ? str2 : "jQuery";
            continue;
          case "MinifyArguments":
            this.MinificationArugments = !str2.IsNullOrWhitespace() ? str2 : "";
            continue;
          default:
            continue;
        }
      }
    }

    public string Name { get; set; }

    internal bool ShouldMinify { get; set; }

    internal string GlobalsToIgnore { get; set; }

    internal string MinificationArugments { get; set; }
  }
}
