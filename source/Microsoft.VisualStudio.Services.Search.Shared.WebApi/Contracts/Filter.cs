// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public class Filter : SearchSecuredV2Object
  {
    public Filter(string name, string id, int resultCount)
    {
      this.Name = name;
      this.Id = id;
      this.ResultCount = resultCount;
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "resultCount")]
    public int ResultCount { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(Extensions.GetIndentSpacing(indentLevel), "Name: ").Append(this.Name).Append(", #results: ").Append(this.ResultCount);
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
