// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Hit
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public class Hit : SearchSecuredV2Object, IComparable<Hit>
  {
    [DataMember(Name = "charOffset")]
    public int CharOffset { get; set; }

    [DataMember(Name = "length")]
    public int Length { get; set; }

    [DataMember(Name = "line")]
    public int Line { get; set; }

    [DataMember(Name = "column")]
    public int Column { get; set; }

    [DataMember(Name = "codeSnippet")]
    public string CodeSnippet { get; set; }

    [DataMember(Name = "type")]
    public string Type { get; set; }

    public int CompareTo(Hit other)
    {
      if (this.CharOffset < other.CharOffset)
        return -1;
      if (this.CharOffset > other.CharOffset)
        return 1;
      if (this.Length > other.Length)
        return -1;
      return this.Length < other.Length ? 1 : 0;
    }

    public override bool Equals(object obj) => obj is Hit hit && this.CharOffset == hit.CharOffset && this.Length == hit.Length;

    public override int GetHashCode() => this.CharOffset;
  }
}
