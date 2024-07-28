// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.NavigationPropertySingletonExpression
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class NavigationPropertySingletonExpression : ResourceExpression
  {
    private readonly Expression memberExpression;
    private readonly Type resourceType;

    internal NavigationPropertySingletonExpression(
      Type type,
      Expression source,
      Expression memberExpression,
      Type resourceType,
      List<string> expandPaths,
      CountOption countOption,
      Dictionary<ConstantExpression, ConstantExpression> customQueryOptions,
      ProjectionQueryOptionExpression projection)
      : base(source, (ExpressionType) 10002, type, expandPaths, countOption, customQueryOptions, projection)
    {
      this.memberExpression = memberExpression;
      this.resourceType = resourceType;
    }

    internal MemberExpression MemberExpression => (MemberExpression) this.memberExpression;

    internal override Type ResourceType => this.resourceType;

    internal override bool IsSingleton => true;

    internal override bool HasQueryOptions => this.ExpandPaths.Count > 0 || this.CountOption == CountOption.InlineAll || this.CustomQueryOptions.Count > 0 || this.Projection != null;

    internal override ResourceExpression CreateCloneWithNewType(Type type) => (ResourceExpression) new NavigationPropertySingletonExpression(type, this.Source, (Expression) this.MemberExpression, TypeSystem.GetElementType(type), this.ExpandPaths.ToList<string>(), this.CountOption, this.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>((Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Key), (Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Value)), this.Projection);
  }
}
