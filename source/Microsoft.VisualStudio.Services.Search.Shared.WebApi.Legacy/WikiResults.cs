// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.WikiResults
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class WikiResults
  {
    public WikiResults(int count, IEnumerable<WikiResult> values)
    {
      this.Count = count;
      this.Values = values;
    }

    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "values")]
    public IEnumerable<WikiResult> Values { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Total Count: ").AppendLine(this.Count.ToString());
      sb.Append(indentSpacing, "Count: ").AppendLine(this.Values.Count<WikiResult>().ToString());
      foreach (WikiResult wikiResult in this.Values)
        sb.AppendFormat(indentSpacing, (object) "{0}: ").Append(wikiResult.ToString(indentLevel + 1));
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
