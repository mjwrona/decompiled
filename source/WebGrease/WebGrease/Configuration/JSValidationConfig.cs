// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.JSValidationConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  public class JSValidationConfig : INamedConfig
  {
    public JSValidationConfig()
    {
      this.ShouldAnalyze = true;
      this.AnalyzeArguments = "-analyze -WARN:4";
    }

    internal JSValidationConfig(XElement element)
      : this()
    {
      this.Name = (string) element.Attribute((XName) "config") ?? string.Empty;
      foreach (XElement descendant in element.Descendants())
      {
        string str1 = descendant.Name.ToString();
        string str2 = descendant.Value;
        switch (str1)
        {
          case "Analyze":
            this.ShouldAnalyze = str2.TryParseBool();
            continue;
          case "AnalayzeArguments":
          case nameof (AnalyzeArguments):
            this.AnalyzeArguments = !str2.IsNullOrWhitespace() ? str2 : "-analyze -WARN:4";
            continue;
          default:
            continue;
        }
      }
    }

    public string Name { get; set; }

    internal bool ShouldAnalyze { get; set; }

    internal string AnalyzeArguments { get; set; }
  }
}
