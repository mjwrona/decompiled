// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmUtil
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.OData.Edm
{
  public static class EdmUtil
  {
    private const string StartCharacterExp = "[\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Lm}\\p{Nl}]";
    private const string OtherCharacterExp = "[\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Lm}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]";
    private const string NameExp = "[\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Lm}\\p{Nl}][\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Lm}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]{0,}";
    private static Regex UndottedNameValidator = PlatformHelper.CreateCompiled("^[\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Lm}\\p{Nl}][\\p{Ll}\\p{Lu}\\p{Lt}\\p{Lo}\\p{Lm}\\p{Nl}\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}\\p{Cf}]{0,}$", RegexOptions.Singleline);

    public static string GetMimeType(this IEdmModel model, IEdmProperty annotatableProperty) => model.GetStringAnnotationValue<IEdmProperty>(annotatableProperty, "MimeType", (Func<string>) (() => Strings.EdmUtil_NullValueForMimeTypeAnnotation));

    public static void SetMimeType(
      this IEdmModel model,
      IEdmProperty annotatableProperty,
      string mimeType)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmProperty>(annotatableProperty, nameof (annotatableProperty));
      model.SetAnnotation((IEdmElement) annotatableProperty, "MimeType", mimeType);
    }

    public static string GetMimeType(this IEdmModel model, IEdmOperation annotatableOperation) => model.GetStringAnnotationValue<IEdmOperation>(annotatableOperation, "MimeType", (Func<string>) (() => Strings.EdmUtil_NullValueForMimeTypeAnnotation));

    public static string GetSymbolicString(this IEdmVocabularyAnnotatable annotatedElement)
    {
      switch (annotatedElement)
      {
        case IEdmSchemaElement edmSchemaElement:
          if (edmSchemaElement.SchemaElementKind != EdmSchemaElementKind.TypeDefinition)
            return edmSchemaElement.SchemaElementKind.ToString();
          switch (((IEdmType) edmSchemaElement).TypeKind)
          {
            case EdmTypeKind.Entity:
              return "EntityType";
            case EdmTypeKind.Complex:
              return "ComplexType";
            case EdmTypeKind.Enum:
              return "EnumType";
            case EdmTypeKind.TypeDefinition:
              return "TypeDefinition";
            default:
              return (string) null;
          }
        case IEdmEntityContainerElement containerElement:
          return containerElement.ContainerElementKind.ToString();
        case IEdmProperty edmProperty:
          switch (edmProperty.PropertyKind)
          {
            case EdmPropertyKind.Structural:
              return "Property";
            case EdmPropertyKind.Navigation:
              return "NavigationProperty";
            default:
              return (string) null;
          }
        case IEdmExpression edmExpression:
          switch (edmExpression.ExpressionKind)
          {
            case EdmExpressionKind.Null:
            case EdmExpressionKind.Record:
            case EdmExpressionKind.Collection:
            case EdmExpressionKind.If:
            case EdmExpressionKind.Cast:
              return edmExpression.ExpressionKind.ToString();
            case EdmExpressionKind.IsType:
              return "IsOf";
            case EdmExpressionKind.FunctionApplication:
              return "Apply";
            case EdmExpressionKind.Labeled:
              return "LabeledElement";
            default:
              return (string) null;
          }
        case IEdmOperationParameter _:
          return "Parameter";
        case IEdmOperationReturn _:
          return "ReturnType";
        case IEdmReference _:
          return "Reference";
        case IEdmInclude _:
          return "Include";
        case IEdmReferentialConstraint _:
          return "ReferentialConstraint";
        case IEdmEnumMember _:
          return "Member";
        case IEdmVocabularyAnnotation _:
          return "Annotation";
        case IEdmPropertyConstructor _:
          return "PropertyValue";
        default:
          return (string) null;
      }
    }

    public static void SetMimeType(
      this IEdmModel model,
      IEdmOperation annotatableOperation,
      string mimeType)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<IEdmOperation>(annotatableOperation, nameof (annotatableOperation));
      model.SetAnnotation((IEdmElement) annotatableOperation, "MimeType", mimeType);
    }

    internal static bool TryParseContainerQualifiedElementName(
      string containerQualifiedElementName,
      out string containerName,
      out string containerElementName)
    {
      containerName = (string) null;
      containerElementName = (string) null;
      int length = containerQualifiedElementName.LastIndexOf('.');
      if (length < 0)
        return false;
      containerName = containerQualifiedElementName.Substring(0, length);
      containerElementName = containerQualifiedElementName.Substring(length + 1);
      return !string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(containerElementName);
    }

    internal static bool IsNullOrWhiteSpaceInternal(string value) => value == null || ((IEnumerable<char>) value.ToCharArray()).All<char>(new Func<char, bool>(char.IsWhiteSpace));

    internal static string JoinInternal<T>(string separator, IEnumerable<T> values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (separator == null)
        separator = string.Empty;
      using (IEnumerator<T> enumerator = values.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return string.Empty;
        StringBuilder stringBuilder = new StringBuilder();
        if ((object) enumerator.Current != null)
        {
          string str = enumerator.Current.ToString();
          if (str != null)
            stringBuilder.Append(str);
        }
        while (enumerator.MoveNext())
        {
          stringBuilder.Append(separator);
          if ((object) enumerator.Current != null)
          {
            string str = enumerator.Current.ToString();
            if (str != null)
              stringBuilder.Append(str);
          }
        }
        return stringBuilder.ToString();
      }
    }

    internal static bool IsQualifiedName(string name)
    {
      string[] source = name.Split('.');
      if (((IEnumerable<string>) source).Count<string>() < 2)
        return false;
      foreach (string str in source)
      {
        if (EdmUtil.IsNullOrWhiteSpaceInternal(str))
          return false;
      }
      return true;
    }

    internal static bool IsValidUndottedName(string name) => !string.IsNullOrEmpty(name) && EdmUtil.UndottedNameValidator.IsMatch(name);

    internal static bool IsValidDottedName(string name) => ((IEnumerable<string>) name.Split('.')).All<string>(new Func<string, bool>(EdmUtil.IsValidUndottedName));

    internal static string ParameterizedName(IEdmOperation operation)
    {
      int num1 = 0;
      int num2 = operation.Parameters.Count<IEdmOperationParameter>();
      StringBuilder stringBuilder = new StringBuilder();
      if (operation is UnresolvedOperation unresolvedOperation)
      {
        stringBuilder.Append(unresolvedOperation.Namespace);
        stringBuilder.Append("/");
        stringBuilder.Append(unresolvedOperation.Name);
        return stringBuilder.ToString();
      }
      IEdmSchemaElement edmSchemaElement = (IEdmSchemaElement) operation;
      if (edmSchemaElement != null)
      {
        stringBuilder.Append(edmSchemaElement.Namespace);
        stringBuilder.Append(".");
      }
      stringBuilder.Append(operation.Name);
      stringBuilder.Append("(");
      foreach (IEdmOperationParameter parameter in operation.Parameters)
      {
        string str = parameter.Type != null ? (!parameter.Type.IsCollection() ? (!parameter.Type.IsEntityReference() ? parameter.Type.FullName() : "Ref(" + parameter.Type.AsEntityReference().EntityType().FullName() + ")") : "Collection(" + parameter.Type.AsCollection().ElementType().FullName() + ")") : "Edm.Untyped";
        stringBuilder.Append(str);
        ++num1;
        if (num1 < num2)
          stringBuilder.Append(", ");
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    internal static bool TryGetNamespaceNameFromQualifiedName(
      string qualifiedName,
      out string namespaceName,
      out string name,
      out string fullName)
    {
      bool fromQualifiedName = EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out namespaceName, out name);
      fullName = EdmUtil.GetFullNameForSchemaElement(namespaceName, name);
      return fromQualifiedName;
    }

    internal static bool TryGetNamespaceNameFromQualifiedName(
      string qualifiedName,
      out string namespaceName,
      out string name)
    {
      int length1 = qualifiedName.LastIndexOf('/');
      if (length1 < 0)
      {
        int length2 = qualifiedName.LastIndexOf('.');
        if (length2 < 0)
        {
          namespaceName = string.Empty;
          name = qualifiedName;
          return false;
        }
        namespaceName = qualifiedName.Substring(0, length2);
        name = qualifiedName.Substring(length2 + 1);
        return true;
      }
      namespaceName = qualifiedName.Substring(0, length1);
      name = qualifiedName.Substring(length1 + 1);
      return true;
    }

    internal static string FullyQualifiedName(IEdmVocabularyAnnotatable element)
    {
      switch (element)
      {
        case IEdmSchemaElement element1:
          return element1 is IEdmOperation operation ? EdmUtil.ParameterizedName(operation) : element1.FullName();
        case IEdmEntityContainerElement containerElement:
          return containerElement.Container.FullName() + "/" + containerElement.Name;
        case IEdmProperty edmProperty:
          if (edmProperty.DeclaringType is IEdmSchemaType declaringType)
          {
            string str = EdmUtil.FullyQualifiedName((IEdmVocabularyAnnotatable) declaringType);
            if (str != null)
              return str + "/" + edmProperty.Name;
            break;
          }
          break;
        case IEdmOperationParameter operationParameter:
          string str1 = EdmUtil.FullyQualifiedName((IEdmVocabularyAnnotatable) operationParameter.DeclaringOperation);
          if (str1 != null)
            return str1 + "/" + operationParameter.Name;
          break;
        case IEdmEnumMember edmEnumMember:
          string str2 = EdmUtil.FullyQualifiedName((IEdmVocabularyAnnotatable) edmEnumMember.DeclaringType);
          if (str2 != null)
            return str2 + "/" + edmEnumMember.Name;
          break;
        case IEdmOperationReturn edmOperationReturn:
          string str3 = EdmUtil.FullyQualifiedName((IEdmVocabularyAnnotatable) edmOperationReturn.DeclaringOperation);
          if (str3 != null)
            return str3 + "/$ReturnType";
          break;
      }
      return (string) null;
    }

    internal static T CheckArgumentNull<T>([EdmUtil.ValidatedNotNull] T value, string parameterName) where T : class => (object) value != null ? value : throw new ArgumentNullException(parameterName);

    internal static bool EqualsOrdinal(this string string1, string string2) => string.Equals(string1, string2, StringComparison.Ordinal);

    internal static bool EqualsOrdinalIgnoreCase(this string string1, string string2) => string.Equals(string1, string2, StringComparison.OrdinalIgnoreCase);

    internal static void SetAnnotation(
      this IEdmModel model,
      IEdmElement annotatable,
      string localName,
      string value)
    {
      IEdmStringValue edmStringValue = (IEdmStringValue) null;
      if (value != null)
        edmStringValue = (IEdmStringValue) new EdmStringConstant(EdmCoreModel.Instance.GetString(true), value);
      model.SetAnnotationValue(annotatable, "http://docs.oasis-open.org/odata/ns/metadata", localName, (object) edmStringValue);
    }

    internal static bool TryGetAnnotation(
      this IEdmModel model,
      IEdmElement annotatable,
      string localName,
      out string value)
    {
      object annotationValue = model.GetAnnotationValue(annotatable, "http://docs.oasis-open.org/odata/ns/metadata", localName);
      if (annotationValue == null)
      {
        value = (string) null;
        return false;
      }
      value = annotationValue is IEdmStringValue edmStringValue ? edmStringValue.Value : throw new InvalidOperationException(Strings.EdmUtil_InvalidAnnotationValue((object) localName, (object) annotationValue.GetType().FullName));
      return true;
    }

    internal static TValue DictionaryGetOrUpdate<TKey, TValue>(
      IDictionary<TKey, TValue> dictionary,
      TKey key,
      Func<TKey, TValue> computeValue)
    {
      EdmUtil.CheckArgumentNull<IDictionary<TKey, TValue>>(dictionary, nameof (dictionary));
      EdmUtil.CheckArgumentNull<Func<TKey, TValue>>(computeValue, nameof (computeValue));
      TValue orUpdate;
      lock (dictionary)
      {
        if (dictionary.TryGetValue(key, out orUpdate))
          return orUpdate;
      }
      TValue obj = computeValue(key);
      lock (dictionary)
      {
        if (!dictionary.TryGetValue(key, out orUpdate))
        {
          orUpdate = obj;
          dictionary.Add(key, obj);
        }
      }
      return orUpdate;
    }

    internal static TValue DictionarySafeGet<TKey, TValue>(
      IDictionary<TKey, TValue> dictionary,
      TKey key)
    {
      EdmUtil.CheckArgumentNull<IDictionary<TKey, TValue>>(dictionary, nameof (dictionary));
      TValue obj;
      lock (dictionary)
        dictionary.TryGetValue(key, out obj);
      return obj;
    }

    internal static string GetFullNameForSchemaElement(string elementNamespace, string elementName)
    {
      if (elementName == null)
        return string.Empty;
      return elementNamespace == null ? elementName : elementNamespace + "." + elementName;
    }

    private static string GetStringAnnotationValue<TEdmElement>(
      this IEdmModel model,
      TEdmElement annotatable,
      string localName,
      Func<string> getFoundAnnotationValueErrorString)
      where TEdmElement : class, IEdmElement
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmUtil.CheckArgumentNull<TEdmElement>(annotatable, nameof (annotatable));
      string str;
      if (!model.TryGetAnnotation((IEdmElement) annotatable, localName, out str))
        return (string) null;
      return str != null ? str : throw new InvalidOperationException(getFoundAnnotationValueErrorString());
    }

    private sealed class ValidatedNotNullAttribute : Attribute
    {
    }
  }
}
