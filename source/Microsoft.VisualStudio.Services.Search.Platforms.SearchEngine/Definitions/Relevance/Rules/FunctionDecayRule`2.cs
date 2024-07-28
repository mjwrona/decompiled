// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules.FunctionDecayRule`2
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules
{
  public class FunctionDecayRule<TOrigin, TScale> : RelevanceRule
  {
    private TOrigin m_origin;
    private TScale m_scale;
    private TScale m_offset;
    private float m_decay = 0.5f;

    public DecayFunction Function { get; set; }

    public TOrigin Origin
    {
      get => this.m_origin;
      set => this.m_origin = value;
    }

    public TScale Scale
    {
      get => this.m_scale;
      set => this.m_scale = value;
    }

    public TScale Offset
    {
      get => this.m_offset;
      set => this.m_offset = value;
    }

    public float Decay
    {
      get => this.m_decay;
      set => this.m_decay = (double) value > 0.0 && (double) value <= 1.0 ? value : throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid value [{0}] for Decay. Decay should be greater than 0 and less than/equal to 1", (object) value)));
    }

    public string Field { get; set; }

    public override IRelevanceExpression Evaluate(IExpression expression)
    {
      IRelevanceExpression relevanceExpression = base.Evaluate(expression);
      if (relevanceExpression != null)
        return relevanceExpression;
      if (EqualityComparer<TScale>.Default.Equals(this.Scale, default (TScale)))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Missing required parameter Scale for rule: {0} ", (object) this.Description)));
      return (IRelevanceExpression) new FunctionDecayExpression<TOrigin, TScale>()
      {
        Function = this.Function,
        Decay = this.Decay,
        Origin = this.Origin,
        Scale = this.Scale,
        Field = this.Field,
        Offset = this.Offset
      };
    }

    public FunctionDecayRule()
    {
      TypeCode typeCode1 = Type.GetTypeCode(typeof (TOrigin));
      TypeCode typeCode2 = Type.GetTypeCode(typeof (TScale));
      if (FunctionDecayExpression<TOrigin, TScale>.s_ValidTypesCombos.ContainsKey(typeCode1))
      {
        if (typeCode2 != FunctionDecayExpression<TOrigin, TScale>.s_ValidTypesCombos[typeCode1])
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Span of type  {0} is not supported for field type {1}", (object) typeof (TScale).FullName, (object) typeof (TOrigin).FullName)));
      }
      else
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Field type {0} is not supported", (object) typeof (TOrigin).FullName)));
    }
  }
}
