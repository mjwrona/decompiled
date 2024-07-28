// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectGraphTraversalStrategies.RoundtripObjectGraphTraversalStrategy
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
  public class RoundtripObjectGraphTraversalStrategy : FullObjectGraphTraversalStrategy
  {
    private readonly IEnumerable<IYamlTypeConverter> converters;

    public RoundtripObjectGraphTraversalStrategy(
      IEnumerable<IYamlTypeConverter> converters,
      ITypeInspector typeDescriptor,
      ITypeResolver typeResolver,
      int maxRecursion)
      : base(typeDescriptor, typeResolver, maxRecursion, (INamingConvention) null)
    {
      this.converters = converters;
    }

    protected override void TraverseProperties<TContext>(
      IObjectDescriptor value,
      IObjectGraphVisitor<TContext> visitor,
      int currentDepth,
      TContext context)
    {
      if (!value.Type.HasDefaultConstructor() && !this.converters.Any<IYamlTypeConverter>((Func<IYamlTypeConverter, bool>) (c => c.Accepts(value.Type))))
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Type '{0}' cannot be deserialized because it does not have a default constructor or a type converter.", new object[1]
        {
          (object) value.Type
        }));
      base.TraverseProperties<TContext>(value, visitor, currentDepth, context);
    }
  }
}
