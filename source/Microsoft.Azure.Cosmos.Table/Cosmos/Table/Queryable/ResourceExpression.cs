// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ResourceExpression
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal abstract class ResourceExpression : Expression
  {
    protected InputReferenceExpression inputRef;
    private List<string> expandPaths;
    private CountOption countOption;
    private Dictionary<ConstantExpression, ConstantExpression> customQueryOptions;
    private ProjectionQueryOptionExpression projection;

    internal ResourceExpression(
      Expression source,
      ExpressionType nodeType,
      Type type,
      List<string> expandPaths,
      CountOption countOption,
      Dictionary<ConstantExpression, ConstantExpression> customQueryOptions,
      ProjectionQueryOptionExpression projection)
      : base(nodeType, type)
    {
      this.expandPaths = expandPaths ?? new List<string>();
      this.countOption = countOption;
      this.customQueryOptions = customQueryOptions ?? new Dictionary<ConstantExpression, ConstantExpression>((IEqualityComparer<ConstantExpression>) ReferenceEqualityComparer<ConstantExpression>.Instance);
      this.projection = projection;
      this.Source = source;
    }

    internal abstract ResourceExpression CreateCloneWithNewType(Type type);

    internal abstract bool HasQueryOptions { get; }

    internal abstract Type ResourceType { get; }

    internal abstract bool IsSingleton { get; }

    internal virtual List<string> ExpandPaths
    {
      get => this.expandPaths;
      set => this.expandPaths = value;
    }

    internal virtual CountOption CountOption
    {
      get => this.countOption;
      set => this.countOption = value;
    }

    internal virtual Dictionary<ConstantExpression, ConstantExpression> CustomQueryOptions
    {
      get => this.customQueryOptions;
      set => this.customQueryOptions = value;
    }

    internal ProjectionQueryOptionExpression Projection
    {
      get => this.projection;
      set => this.projection = value;
    }

    internal Expression Source { get; private set; }

    internal InputReferenceExpression CreateReference()
    {
      if (this.inputRef == null)
        this.inputRef = new InputReferenceExpression(this);
      return this.inputRef;
    }
  }
}
