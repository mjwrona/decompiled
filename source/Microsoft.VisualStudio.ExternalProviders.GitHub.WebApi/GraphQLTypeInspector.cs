// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GraphQLTypeInspector
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  internal class GraphQLTypeInspector
  {
    public GraphQLTypeInspector(IReadOnlyList<Type> types = null)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      types = types ?? (IReadOnlyList<Type>) ((IEnumerable<Type>) typeof (GitHubData.V4).GetNestedTypes()).Where<Type>(GraphQLTypeInspector.\u003C\u003EO.\u003C0\u003E__IsObjectType ?? (GraphQLTypeInspector.\u003C\u003EO.\u003C0\u003E__IsObjectType = new Func<Type, bool>(GraphQLTypeInspector.IsObjectType))).ToList<Type>();
      this.TypeInfoMap = GraphQLTypeInspector.BuildTypeInfoMap(types);
    }

    public IDictionary<Type, DeclaredGraphQLTypeInfo> TypeInfoMap { get; }

    public static bool IsObjectType(Type type) => GraphQLTypeInspector.GetElementalType(type).GetCustomAttributes<GitHubData.V4.GraphQLObjectAttribute>(true).Any<GitHubData.V4.GraphQLObjectAttribute>();

    public string GetAllFields<T>(int depth)
    {
      StringBuilder builder = new StringBuilder();
      this.AppendAllFields(builder, typeof (T), depth);
      return builder.ToString();
    }

    public string GetScalarFields<T>()
    {
      StringBuilder builder = new StringBuilder();
      this.AppendAllFields(builder, typeof (T), 0);
      return builder.ToString();
    }

    private void AppendAllFields(StringBuilder builder, Type type, int depth)
    {
      GraphQLTypeInfo typeInfo = (GraphQLTypeInfo) this.GetTypeInfo(type);
      if (typeInfo == null)
        return;
      this.AppendAllFields(builder, typeInfo, depth);
    }

    private void AppendAllFields(StringBuilder builder, GraphQLTypeInfo typeInfo, int depth)
    {
      this.AppendDeclaredScalarFields(builder, typeInfo);
      if (depth > 0)
      {
        builder.Append(" ");
        this.AppendDeclaredObjectFields(builder, typeInfo, depth);
      }
      if (!(typeInfo is DeclaredGraphQLTypeInfo typeInfo1))
        return;
      builder.Append(" ");
      this.AppendInlineFragments(builder, typeInfo1, depth);
    }

    private void AppendInlineFragments(
      StringBuilder builder,
      DeclaredGraphQLTypeInfo typeInfo,
      int depth)
    {
      foreach (DistinctGraphQLTypeInfo implementation in (IEnumerable<DistinctGraphQLTypeInfo>) typeInfo.Implementations)
      {
        StringBuilder builder1 = new StringBuilder();
        this.AppendAllFields(builder1, (GraphQLTypeInfo) implementation, depth);
        string str = builder1.ToString();
        if (!string.IsNullOrWhiteSpace(str))
          builder.Append(" ...on " + implementation.Type.Name + " { " + str + " }");
      }
    }

    private void AppendDeclaredObjectFields(
      StringBuilder builder,
      GraphQLTypeInfo typeInfo,
      int depth)
    {
      foreach (PropertyInfo objectProperty in (IEnumerable<PropertyInfo>) typeInfo.ObjectProperties)
      {
        builder.Append(" ");
        this.AppendObjectProperty(builder, objectProperty, depth - 1);
      }
    }

    private void AppendDeclaredScalarFields(StringBuilder builder, GraphQLTypeInfo typeInfo)
    {
      if (typeInfo is DeclaredGraphQLTypeInfo declaredGraphQlTypeInfo && declaredGraphQlTypeInfo.Implementations.Count > 0)
        builder.Append("__typename ");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      foreach (string str in typeInfo.ScalarProperties.Select<PropertyInfo, string>(GraphQLTypeInspector.\u003C\u003EO.\u003C1\u003E__GetFieldName ?? (GraphQLTypeInspector.\u003C\u003EO.\u003C1\u003E__GetFieldName = new Func<PropertyInfo, string>(GraphQLTypeInspector.GetFieldName))))
      {
        builder.Append(" ");
        builder.Append(str);
      }
    }

    private void AppendObjectProperty(StringBuilder builder, PropertyInfo propertyInfo, int depth)
    {
      builder.Append(" " + GraphQLTypeInspector.GetFieldName(propertyInfo) + " { ");
      this.AppendAllFields(builder, propertyInfo.PropertyType, depth);
      builder.Append(" }");
    }

    private DeclaredGraphQLTypeInfo GetTypeInfo(Type type)
    {
      type = GraphQLTypeInspector.GetElementalType(type);
      DeclaredGraphQLTypeInfo declaredGraphQlTypeInfo;
      return this.TypeInfoMap.TryGetValue(type, out declaredGraphQlTypeInfo) ? declaredGraphQlTypeInfo : (DeclaredGraphQLTypeInfo) null;
    }

    private static IDictionary<Type, DeclaredGraphQLTypeInfo> BuildTypeInfoMap(
      IReadOnlyList<Type> objectTypes)
    {
      ArgumentUtility.CheckForNull<IReadOnlyList<Type>>(objectTypes, nameof (objectTypes));
      Dictionary<Type, DeclaredGraphQLTypeInfo> dictionary = new Dictionary<Type, DeclaredGraphQLTypeInfo>();
      foreach (Type objectType in (IEnumerable<Type>) objectTypes)
      {
        IReadOnlyList<PropertyInfo> source = GraphQLTypeInspector.IsObjectType(objectType) ? GraphQLTypeInspector.GetQueryFragmentGenerationProperties(objectType) : throw new ArgumentException(string.Format("Type {0} doesn't represent a GraphQL object.", (object) objectType), nameof (objectTypes));
        DeclaredGraphQLTypeInfo declaredGraphQlTypeInfo1 = new DeclaredGraphQLTypeInfo();
        declaredGraphQlTypeInfo1.Type = objectType;
        declaredGraphQlTypeInfo1.ScalarProperties = (IReadOnlyList<PropertyInfo>) source.Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !GraphQLTypeInspector.IsObjectType(p.PropertyType))).ToArray<PropertyInfo>();
        declaredGraphQlTypeInfo1.ObjectProperties = (IReadOnlyList<PropertyInfo>) source.Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => GraphQLTypeInspector.IsObjectType(p.PropertyType))).ToArray<PropertyInfo>();
        declaredGraphQlTypeInfo1.Implementations = (IReadOnlyList<DistinctGraphQLTypeInfo>) Array.Empty<DistinctGraphQLTypeInfo>();
        DeclaredGraphQLTypeInfo declaredGraphQlTypeInfo2 = declaredGraphQlTypeInfo1;
        dictionary.Add(objectType, declaredGraphQlTypeInfo2);
      }
      foreach (Type type1 in objectTypes.Where<Type>((Func<Type, bool>) (t => t.IsInterface)))
      {
        Type interfaceType = type1;
        DeclaredGraphQLTypeInfo declaredGraphQlTypeInfo = dictionary[interfaceType];
        List<Type> list1 = objectTypes.Where<Type>((Func<Type, bool>) (t => ((IEnumerable<Type>) t.GetInterfaces()).Contains<Type>(interfaceType))).ToList<Type>();
        List<DistinctGraphQLTypeInfo> distinctGraphQlTypeInfoList = new List<DistinctGraphQLTypeInfo>();
        foreach (Type type2 in (IEnumerable<Type>) list1)
        {
          IReadOnlyList<PropertyInfo> list2 = (IReadOnlyList<PropertyInfo>) GraphQLTypeInspector.GetQueryFragmentGenerationProperties(type2).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => interfaceType.GetProperty(p.Name) == (PropertyInfo) null)).ToList<PropertyInfo>();
          DistinctGraphQLTypeInfo distinctGraphQlTypeInfo1 = new DistinctGraphQLTypeInfo();
          distinctGraphQlTypeInfo1.Type = type2;
          distinctGraphQlTypeInfo1.DeclaredType = (GraphQLTypeInfo) declaredGraphQlTypeInfo;
          distinctGraphQlTypeInfo1.ScalarProperties = (IReadOnlyList<PropertyInfo>) list2.Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !GraphQLTypeInspector.IsObjectType(p.PropertyType))).ToArray<PropertyInfo>();
          distinctGraphQlTypeInfo1.ObjectProperties = (IReadOnlyList<PropertyInfo>) list2.Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => GraphQLTypeInspector.IsObjectType(p.PropertyType))).ToArray<PropertyInfo>();
          DistinctGraphQLTypeInfo distinctGraphQlTypeInfo2 = distinctGraphQlTypeInfo1;
          distinctGraphQlTypeInfoList.Add(distinctGraphQlTypeInfo2);
        }
        declaredGraphQlTypeInfo.Implementations = (IReadOnlyList<DistinctGraphQLTypeInfo>) distinctGraphQlTypeInfoList;
      }
      return (IDictionary<Type, DeclaredGraphQLTypeInfo>) dictionary;
    }

    private static IReadOnlyList<PropertyInfo> GetQueryFragmentGenerationProperties(Type type) => (IReadOnlyList<PropertyInfo>) ((IEnumerable<PropertyInfo>) type.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.GetCustomAttributes<DataMemberAttribute>(true).Any<DataMemberAttribute>())).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.GetCustomAttributes<GitHubData.V4.GraphQLQueryGenerationIgnoreAttribute>().Any<GitHubData.V4.GraphQLQueryGenerationIgnoreAttribute>())).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.PropertyType.IsGenericType || p.PropertyType.GetGenericTypeDefinition() != typeof (GitHubData.V4.Connection<>))).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => !p.GetCustomAttributes<GitHubData.V4.GraphQLArgumentAttribute>(true).Where<GitHubData.V4.GraphQLArgumentAttribute>((Func<GitHubData.V4.GraphQLArgumentAttribute, bool>) (att => att.IsRequired)).Any<GitHubData.V4.GraphQLArgumentAttribute>())).ToList<PropertyInfo>();

    private static string GetFieldName(PropertyInfo propertyInfo) => propertyInfo.GetCustomAttributes<DataMemberAttribute>(true).First<DataMemberAttribute>().Name;

    private static Type GetElementalType(Type type)
    {
      if (type.IsArray)
        return GraphQLTypeInspector.GetElementalType(type.GetElementType());
      if (type.IsGenericType)
      {
        Type[] genericArguments = type.GetGenericArguments();
        if (genericArguments.Length == 1)
        {
          Type genericTypeDefinition = type.GetGenericTypeDefinition();
          if (genericTypeDefinition == typeof (IEnumerable<>) || genericTypeDefinition == typeof (ICollection<>) || genericTypeDefinition == typeof (IReadOnlyList<>) || genericTypeDefinition == typeof (IList<>) || genericTypeDefinition == typeof (List<>))
            return GraphQLTypeInspector.GetElementalType(genericArguments[0]);
        }
      }
      return type;
    }
  }
}
