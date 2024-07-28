// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.InputSpec
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.IO;
using System.Xml.Linq;

namespace WebGrease.Configuration
{
  public class InputSpec
  {
    public InputSpec() => this.SearchOption = SearchOption.AllDirectories;

    internal InputSpec(XElement element, string sourceDirectory)
    {
      XAttribute xattribute1 = element.Attribute((XName) "optional");
      bool result1;
      if (xattribute1 != null && bool.TryParse(xattribute1.Value, out result1))
        this.IsOptional = result1;
      XAttribute xattribute2 = element.Attribute((XName) "searchPattern");
      this.SearchPattern = xattribute2 != null ? xattribute2.Value : string.Empty;
      XAttribute xattribute3 = element.Attribute((XName) "searchOption");
      SearchOption result2;
      this.SearchOption = xattribute3 == null ? SearchOption.AllDirectories : (Enum.TryParse<SearchOption>(xattribute3.Value, out result2) ? result2 : SearchOption.AllDirectories);
      if (string.IsNullOrWhiteSpace(element.Value))
        return;
      this.Path = System.IO.Path.GetFullPath(System.IO.Path.Combine(sourceDirectory, element.Value));
    }

    public string Path { get; set; }

    public string SearchPattern { get; set; }

    public SearchOption SearchOption { get; set; }

    public bool IsOptional { get; set; }

    public override bool Equals(object obj) => obj is InputSpec inputSpec && inputSpec.Path == this.Path && inputSpec.SearchOption == this.SearchOption && inputSpec.SearchPattern == this.SearchPattern && inputSpec.IsOptional == this.IsOptional;

    public override int GetHashCode() => (((17 * 23 + InputSpec.GetObjectHashCode((object) this.Path)) * 23 + InputSpec.GetObjectHashCode((object) this.SearchOption)) * 23 + InputSpec.GetObjectHashCode((object) this.SearchPattern)) * 23 + this.IsOptional.GetHashCode();

    private static int GetObjectHashCode(object obj) => obj == null ? 0 : obj.GetHashCode();
  }
}
