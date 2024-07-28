// Decompiled with JetBrains decompiler
// Type: Nest.EnrichProcessor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class EnrichProcessor : ProcessorBase, IEnrichProcessor, IProcessor
  {
    public Field Field { get; set; }

    public string PolicyName { get; set; }

    public Field TargetField { get; set; }

    public bool? IgnoreMissing { get; set; }

    public bool? Override { get; set; }

    public int? MaxMatches { get; set; }

    public GeoShapeRelation? ShapeRelation { get; set; }

    protected override string Name => "enrich";
  }
}
