// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ResourceSetExpression
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  [DebuggerDisplay("ResourceSetExpression {Source}.{MemberExpression}")]
  internal class ResourceSetExpression : ResourceExpression
  {
    private readonly Type resourceType;
    private readonly Expression member;
    private Dictionary<PropertyInfo, ConstantExpression> keyFilter;
    private List<QueryOptionExpression> sequenceQueryOptions;
    private ResourceSetExpression.TransparentAccessors transparentScope;

    internal ResourceSetExpression(
      Type type,
      Expression source,
      Expression memberExpression,
      Type resourceType,
      List<string> expandPaths,
      CountOption countOption,
      Dictionary<ConstantExpression, ConstantExpression> customQueryOptions,
      ProjectionQueryOptionExpression projection)
      : base(source, source != null ? (ExpressionType) 10001 : (ExpressionType) 10000, type, expandPaths, countOption, customQueryOptions, projection)
    {
      this.member = memberExpression;
      this.resourceType = resourceType;
      this.sequenceQueryOptions = new List<QueryOptionExpression>();
    }

    internal Expression MemberExpression => this.member;

    internal override Type ResourceType => this.resourceType;

    internal bool HasTransparentScope => this.transparentScope != null;

    internal ResourceSetExpression.TransparentAccessors TransparentScope
    {
      get => this.transparentScope;
      set => this.transparentScope = value;
    }

    internal bool HasKeyPredicate => this.keyFilter != null;

    internal Dictionary<PropertyInfo, ConstantExpression> KeyPredicate
    {
      get => this.keyFilter;
      set => this.keyFilter = value;
    }

    internal override bool IsSingleton => this.HasKeyPredicate;

    internal override bool HasQueryOptions => this.sequenceQueryOptions.Count > 0 || this.ExpandPaths.Count > 0 || this.CountOption == CountOption.InlineAll || this.CustomQueryOptions.Count > 0 || this.Projection != null;

    internal FilterQueryOptionExpression Filter => this.sequenceQueryOptions.OfType<FilterQueryOptionExpression>().SingleOrDefault<FilterQueryOptionExpression>();

    internal RequestOptionsQueryOptionExpression RequestOptions => this.sequenceQueryOptions.OfType<RequestOptionsQueryOptionExpression>().SingleOrDefault<RequestOptionsQueryOptionExpression>();

    internal TakeQueryOptionExpression Take => this.sequenceQueryOptions.OfType<TakeQueryOptionExpression>().SingleOrDefault<TakeQueryOptionExpression>();

    internal IEnumerable<QueryOptionExpression> SequenceQueryOptions => (IEnumerable<QueryOptionExpression>) this.sequenceQueryOptions.ToList<QueryOptionExpression>();

    internal bool HasSequenceQueryOptions => this.sequenceQueryOptions.Count > 0;

    internal override ResourceExpression CreateCloneWithNewType(Type type) => (ResourceExpression) new ResourceSetExpression(type, this.Source, this.MemberExpression, TypeSystem.GetElementType(type), this.ExpandPaths.ToList<string>(), this.CountOption, this.CustomQueryOptions.ToDictionary<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression, ConstantExpression>((Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Key), (Func<KeyValuePair<ConstantExpression, ConstantExpression>, ConstantExpression>) (kvp => kvp.Value)), this.Projection)
    {
      keyFilter = this.keyFilter,
      sequenceQueryOptions = this.sequenceQueryOptions,
      transparentScope = this.transparentScope
    };

    internal void AddSequenceQueryOption(QueryOptionExpression qoe)
    {
      QueryOptionExpression previous = this.sequenceQueryOptions.Where<QueryOptionExpression>((Func<QueryOptionExpression, bool>) (o => o.GetType() == qoe.GetType())).FirstOrDefault<QueryOptionExpression>();
      if (previous != null)
      {
        qoe = qoe.ComposeMultipleSpecification(previous);
        this.sequenceQueryOptions.Remove(previous);
      }
      this.sequenceQueryOptions.Add(qoe);
    }

    internal void OverrideInputReference(ResourceSetExpression newInput)
    {
      InputReferenceExpression inputRef = newInput.inputRef;
      if (inputRef == null)
        return;
      this.inputRef = inputRef;
      inputRef.OverrideTarget(this);
    }

    [DebuggerDisplay("{ToString()}")]
    internal class TransparentAccessors
    {
      internal readonly string Accessor;
      internal readonly Dictionary<string, Expression> SourceAccessors;

      internal TransparentAccessors(string acc, Dictionary<string, Expression> sourceAccesors)
      {
        this.Accessor = acc;
        this.SourceAccessors = sourceAccesors;
      }

      public override string ToString() => "SourceAccessors=[" + string.Join(",", this.SourceAccessors.Keys.ToArray<string>()) + "] ->* Accessor=" + this.Accessor;
    }
  }
}
