// Decompiled with JetBrains decompiler
// Type: Nest.IMappingVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface IMappingVisitor
  {
    int Depth { get; set; }

    void Visit(ITypeMapping mapping);

    void Visit(ITextProperty property);

    void Visit(IKeywordProperty property);

    void Visit(IDateProperty property);

    void Visit(IDateNanosProperty property);

    void Visit(IBooleanProperty property);

    void Visit(IBinaryProperty property);

    void Visit(IObjectProperty property);

    void Visit(INestedProperty property);

    void Visit(IIpProperty property);

    void Visit(IGeoPointProperty property);

    void Visit(IGeoShapeProperty property);

    void Visit(IShapeProperty property);

    void Visit(IPointProperty property);

    void Visit(INumberProperty property);

    void Visit(ICompletionProperty property);

    void Visit(IMurmur3HashProperty property);

    void Visit(ITokenCountProperty property);

    void Visit(IPercolatorProperty property);

    void Visit(IIntegerRangeProperty property);

    void Visit(IFloatRangeProperty property);

    void Visit(ILongRangeProperty property);

    void Visit(IDoubleRangeProperty property);

    void Visit(IDateRangeProperty property);

    void Visit(IIpRangeProperty property);

    void Visit(IJoinProperty property);

    void Visit(IRankFeatureProperty property);

    void Visit(IRankFeaturesProperty property);

    void Visit(ISearchAsYouTypeProperty property);

    void Visit(IFlattenedProperty property);

    void Visit(IHistogramProperty property);

    void Visit(IConstantKeywordProperty property);

    void Visit(IVersionProperty property);

    void Visit(IDenseVectorProperty property);

    void Visit(IMatchOnlyTextProperty property);
  }
}
