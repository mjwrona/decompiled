// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.WikiQueryResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class WikiQueryResponse : EntitySearchResponse
  {
    [DataMember(Name = "query")]
    public WikiSearchQuery Query { get; set; }

    [DataMember(Name = "results")]
    public WikiResults Results { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.AppendLine(indentSpacing, "Search query:").Append(this.Query.ToString(indentLevel + 1));
      sb.AppendLine(indentSpacing, "Results:").Append(this.Results.ToString(indentLevel + 1));
      sb.AppendLine(indentSpacing, "Filter categories:");
      if (this.FilterCategories != null)
      {
        foreach (FilterCategory filterCategory in this.FilterCategories)
          sb.Append(filterCategory.ToString(indentLevel + 1));
      }
      if (this.Errors != null)
      {
        sb.AppendLine(indentSpacing, "Errors:");
        foreach (ErrorData error in this.Errors)
          sb.Append(error.ToString(indentLevel + 1));
      }
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
