// Decompiled with JetBrains decompiler
// Type: Nest.NoopMappingVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class NoopMappingVisitor : IMappingVisitor
  {
    public virtual int Depth { get; set; }

    public virtual void Visit(ITypeMapping mapping)
    {
    }

    public virtual void Visit(ITextProperty property)
    {
    }

    public virtual void Visit(IKeywordProperty property)
    {
    }

    public virtual void Visit(IDateProperty property)
    {
    }

    public virtual void Visit(IDateNanosProperty property)
    {
    }

    public virtual void Visit(IBooleanProperty property)
    {
    }

    public virtual void Visit(IBinaryProperty property)
    {
    }

    public virtual void Visit(IPointProperty property)
    {
    }

    public virtual void Visit(INumberProperty property)
    {
    }

    public virtual void Visit(IObjectProperty property)
    {
    }

    public virtual void Visit(INestedProperty property)
    {
    }

    public virtual void Visit(IIpProperty property)
    {
    }

    public virtual void Visit(IGeoPointProperty property)
    {
    }

    public virtual void Visit(IGeoShapeProperty property)
    {
    }

    public virtual void Visit(IShapeProperty property)
    {
    }

    public virtual void Visit(ICompletionProperty property)
    {
    }

    public virtual void Visit(IMurmur3HashProperty property)
    {
    }

    public virtual void Visit(ITokenCountProperty property)
    {
    }

    public virtual void Visit(IPercolatorProperty property)
    {
    }

    public virtual void Visit(IIntegerRangeProperty property)
    {
    }

    public virtual void Visit(IFloatRangeProperty property)
    {
    }

    public virtual void Visit(ILongRangeProperty property)
    {
    }

    public virtual void Visit(IDoubleRangeProperty property)
    {
    }

    public virtual void Visit(IDateRangeProperty property)
    {
    }

    public virtual void Visit(IIpRangeProperty property)
    {
    }

    public virtual void Visit(IJoinProperty property)
    {
    }

    public virtual void Visit(IRankFeatureProperty property)
    {
    }

    public virtual void Visit(IRankFeaturesProperty property)
    {
    }

    public virtual void Visit(ISearchAsYouTypeProperty property)
    {
    }

    public virtual void Visit(IFlattenedProperty property)
    {
    }

    public virtual void Visit(IHistogramProperty property)
    {
    }

    public virtual void Visit(IConstantKeywordProperty property)
    {
    }

    public virtual void Visit(IVersionProperty property)
    {
    }

    public virtual void Visit(IDenseVectorProperty property)
    {
    }

    public virtual void Visit(IMatchOnlyTextProperty property)
    {
    }
  }
}
