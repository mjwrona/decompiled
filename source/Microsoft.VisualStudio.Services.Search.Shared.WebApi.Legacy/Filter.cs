// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class Filter : SearchSecuredObject
  {
    public Filter(string name, string id, int resultCount, bool selected)
    {
      this.Name = name;
      this.Id = id;
      this.ResultCount = resultCount;
      this.Selected = selected;
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "resultCount")]
    public int ResultCount { get; set; }

    [DataMember(Name = "selected")]
    public bool Selected { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(Extensions.GetIndentSpacing(indentLevel), "Name: ").Append(this.Name).Append(", #results: ").Append(this.ResultCount).Append(", IsSelected: ").Append(this.Selected);
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
