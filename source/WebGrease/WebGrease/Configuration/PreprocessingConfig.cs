// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.PreprocessingConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace WebGrease.Configuration
{
  public class PreprocessingConfig : INamedConfig
  {
    public PreprocessingConfig() => this.PreprocessingEngines = new Collection<string>();

    public PreprocessingConfig(XElement element)
      : this()
    {
      this.Name = (string) element.Attribute((XName) "config") ?? string.Empty;
      string str1 = (string) element.Element((XName) "Engines") ?? (string) element.Attribute((XName) "Engines") ?? (string) element.Attribute((XName) "engines");
      if (!string.IsNullOrWhiteSpace(str1))
      {
        string str2 = str1;
        char[] separator = new char[1]{ ';' };
        foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        {
          this.Enabled = true;
          this.PreprocessingEngines.Add(str3);
        }
      }
      this.Element = element;
    }

    public XElement Element { get; private set; }

    public bool Enabled { get; private set; }

    public Collection<string> PreprocessingEngines { get; private set; }

    public string Name { get; private set; }
  }
}
