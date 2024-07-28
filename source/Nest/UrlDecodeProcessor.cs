// Decompiled with JetBrains decompiler
// Type: Nest.UrlDecodeProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class UrlDecodeProcessor : ProcessorBase, IUrlDecodeProcessor, IProcessor
  {
    [DataMember(Name = "field")]
    public Field Field { get; set; }

    [DataMember(Name = "ignore_missing")]
    public bool? IgnoreMissing { get; set; }

    [DataMember(Name = "target_field")]
    public Field TargetField { get; set; }

    protected override string Name => "urldecode";
  }
}
