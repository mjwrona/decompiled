// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance.FunctionDecayExpression`2
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance
{
  public class FunctionDecayExpression<TOrigin, TScale> : IRelevanceExpression
  {
    [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "It is similar to a const dictionary and needs to be used in other classes")]
    public static readonly IReadOnlyDictionary<TypeCode, TypeCode> s_ValidTypesCombos = (IReadOnlyDictionary<TypeCode, TypeCode>) new Dictionary<TypeCode, TypeCode>()
    {
      [TypeCode.Int64] = TypeCode.Int64,
      [TypeCode.String] = TypeCode.String,
      [TypeCode.Single] = TypeCode.Single,
      [TypeCode.Double] = TypeCode.Double,
      [TypeCode.DateTime] = TypeCode.String
    };
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

    public FunctionDecayExpression()
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
