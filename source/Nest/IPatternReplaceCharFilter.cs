// Decompiled with JetBrains decompiler
// Type: Nest.IPatternReplaceCharFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public interface IPatternReplaceCharFilter : ICharFilter
  {
    [DataMember(Name = "flags")]
    string Flags { get; set; }

    [DataMember(Name = "pattern")]
    string Pattern { get; set; }

    [DataMember(Name = "replacement")]
    string Replacement { get; set; }
  }
}
