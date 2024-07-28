// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.EuiiValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class EuiiValidator
  {
    private const string Layer = "Validation";
    private static EuiiValidator.FindReferencesVisitorSettings s_settings;

    private static void Initialize(
      IVssRequestContext requestContext,
      ODataQueryOptions queryOptions)
    {
      if (EuiiValidator.s_settings != null)
        return;
      EdmToClrEvaluator edmToClrEvaluator = new EdmToClrEvaluator((IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>>) null);
      EuiiValidator.s_settings = new EuiiValidator.FindReferencesVisitorSettings((IEnumerable<MemberInfo>) queryOptions.Context.Model.EntityContainer.Elements.OfType<EdmEntitySet>().SelectMany<EdmEntitySet, IEdmProperty>((Func<EdmEntitySet, IEnumerable<IEdmProperty>>) (e => ((IEdmStructuredType) ((EdmCollectionType) e.Type).ElementType.Definition).Properties())).Where<IEdmProperty>((Func<IEdmProperty, bool>) (p => p.VocabularyAnnotations(queryOptions.Context.Model).Any<IEdmVocabularyAnnotation>((Func<IEdmVocabularyAnnotation, bool>) (a => a.Term == EuiiAnnotationAttribute.EuiiNameTerm)))).Select<IEdmProperty, PropertyInfo>((Func<IEdmProperty, PropertyInfo>) (p => EdmTypeUtils.GetType((IEdmType) p.DeclaringType).GetProperty(p.Name))));
    }

    public static void Validate(
      IVssRequestContext requestContext,
      ODataQueryOptions queryOptions,
      ODataQuerySettings querySettings,
      IQueryable queryable)
    {
      EuiiValidator.Initialize(requestContext, queryOptions);
      if (requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/Analytics/Settings/OData/ODataDisableWhilelisting", false, false))
      {
        requestContext.TraceAlways(12013036, TraceLevel.Warning, "AnalyticsModel", "IODataValidator", "Validation skipped");
      }
      else
      {
        EuiiValidator.FindReferencesVisitor visitor = new EuiiValidator.FindReferencesVisitor(EuiiValidator.s_settings);
        visitor.Validate(queryable.Expression);
        if (visitor.MemberReferences.Count > 0 || visitor.TypeReferences.Count > 0)
        {
          requestContext.TraceConditionally(12013031, TraceLevel.Info, "AnalyticsModel", "Validation", (Func<string>) (() => new
          {
            MemberReferences = visitor.MemberReferences.Select(m => new
            {
              Type = m.ReflectedType.FullName,
              Member = m.Name
            }),
            TypeReferences = visitor.TypeReferences.Select(t => new
            {
              Type = t.FullName
            })
          }.Serialize()));
          throw new AnalyticsAccessCheckException(AnalyticsResources.QUERY_EUII_NOT_ALLOWED());
        }
      }
    }

    internal class FindReferencesVisitorSettings
    {
      public HashSet<MemberInfo> Members { get; }

      public HashSet<Type> Types { get; }

      public HashSet<Type> QueryableTypes { get; }

      public FindReferencesVisitorSettings(IEnumerable<MemberInfo> members)
      {
        this.Members = new HashSet<MemberInfo>(members);
        this.Types = new HashSet<Type>(members.Select<MemberInfo, Type>((Func<MemberInfo, Type>) (m => m.ReflectedType)));
        this.QueryableTypes = new HashSet<Type>(this.Types.Select<Type, Type>((Func<Type, Type>) (t => typeof (IQueryable<>).MakeGenericType(t))));
      }
    }

    internal class FindReferencesVisitor : ExpressionVisitor
    {
      public HashSet<Type> TypeReferences = new HashSet<Type>();

      public EuiiValidator.FindReferencesVisitorSettings Settings { get; }

      public HashSet<MemberInfo> MemberReferences { get; } = new HashSet<MemberInfo>();

      public FindReferencesVisitor(
        EuiiValidator.FindReferencesVisitorSettings settings)
      {
        this.Settings = settings;
      }

      public void Validate(Expression node)
      {
        if (this.Settings.QueryableTypes.Any<Type>((Func<Type, bool>) (t => t.IsAssignableFrom(node.Type))))
          this.TypeReferences.Add(node.Type.GenericTypeArguments[0]);
        this.Visit(node);
      }

      protected override Expression VisitMember(MemberExpression node)
      {
        if (this.Settings.Types.Contains(node.Expression?.Type))
        {
          if (this.Settings.Members.Contains(node.Member))
          {
            this.MemberReferences.Add(node.Member);
          }
          else
          {
            if (node.Expression.NodeType == ExpressionType.Parameter)
              return (Expression) node;
            if (node.Expression is MemberExpression expression && expression.Expression.NodeType == ExpressionType.Parameter)
              return (Expression) node;
          }
        }
        if (this.Settings.Types.Contains(node.Type))
          this.TypeReferences.Add(node.Type);
        return base.VisitMember(node);
      }

      protected override Expression VisitBinary(BinaryExpression node) => node.Right is ConstantExpression right && right.Value == null && node.Left is MemberExpression left && this.Settings.Types.Contains(left.Type) ? (Expression) node : base.VisitBinary(node);

      protected override Expression VisitLambda<T>(Expression<T> node)
      {
        this.Visit(node.Body);
        return (Expression) node;
      }

      protected override Expression VisitParameter(ParameterExpression node)
      {
        if (this.Settings.Types.Contains(node.Type))
          this.TypeReferences.Add(node.Type);
        return base.VisitParameter(node);
      }
    }
  }
}
