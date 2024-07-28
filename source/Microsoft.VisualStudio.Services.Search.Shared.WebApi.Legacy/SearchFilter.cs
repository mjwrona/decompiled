// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchFilter
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class SearchFilter : SearchSecuredObject, ICloneable
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "values")]
    public IEnumerable<string> Values { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Name: ").AppendLine(this.Name).Append(indentSpacing, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "FilterCount: {0}", (object) this.Values.Count<string>())).AppendLine();
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);

    public object Clone()
    {
      SearchFilter searchFilter = new SearchFilter();
      searchFilter.Name = this.Name;
      if (this.Values != null)
        searchFilter.Values = (IEnumerable<string>) new List<string>(this.Values);
      return (object) searchFilter;
    }
  }
}
