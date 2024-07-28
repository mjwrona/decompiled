// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.SerializationExtensionMethods
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl
{
  public static class SerializationExtensionMethods
  {
    public static Version GetEdmxVersion(this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue<Version>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "EdmxVersion");
    }

    public static void SetEdmxVersion(this IEdmModel model, Version version)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "EdmxVersion", (object) version);
    }

    public static void SetNamespacePrefixMappings(
      this IEdmModel model,
      IEnumerable<KeyValuePair<string, string>> mappings)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "NamespacePrefix", (object) mappings);
    }

    public static IEnumerable<KeyValuePair<string, string>> GetNamespacePrefixMappings(
      this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue<IEnumerable<KeyValuePair<string, string>>>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "NamespacePrefix");
    }

    public static void SetSerializationLocation(
      this IEdmVocabularyAnnotation annotation,
      IEdmModel model,
      EdmVocabularyAnnotationSerializationLocation? location)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) annotation, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "AnnotationSerializationLocation", (object) location);
    }

    public static EdmVocabularyAnnotationSerializationLocation? GetSerializationLocation(
      this IEdmVocabularyAnnotation annotation,
      IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue((IEdmElement) annotation, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "AnnotationSerializationLocation") as EdmVocabularyAnnotationSerializationLocation?;
    }

    public static void SetSchemaNamespace(
      this IEdmVocabularyAnnotation annotation,
      IEdmModel model,
      string schemaNamespace)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) annotation, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "SchemaNamespace", (object) schemaNamespace);
    }

    public static string GetSchemaNamespace(
      this IEdmVocabularyAnnotation annotation,
      IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmVocabularyAnnotation>(annotation, nameof (annotation));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue<string>((IEdmElement) annotation, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "SchemaNamespace");
    }

    public static void SetIsValueExplicit(
      this IEdmEnumMember member,
      IEdmModel model,
      bool? isExplicit)
    {
      EdmUtil.CheckArgumentNull<IEdmEnumMember>(member, nameof (member));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      model.SetAnnotationValue((IEdmElement) member, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "IsEnumMemberValueExplicit", (object) isExplicit);
    }

    public static bool? IsValueExplicit(this IEdmEnumMember member, IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmEnumMember>(member, nameof (member));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue((IEdmElement) member, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "IsEnumMemberValueExplicit") as bool?;
    }

    public static void SetIsSerializedAsElement(
      this IEdmValue value,
      IEdmModel model,
      bool isSerializedAsElement)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(value, nameof (value));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      EdmError error;
      if (isSerializedAsElement && !ValidationHelper.ValidateValueCanBeWrittenAsXmlElementAnnotation(value, (string) null, (string) null, out error))
        throw new InvalidOperationException(error.ToString());
      model.SetAnnotationValue((IEdmElement) value, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "IsSerializedAsElement", (object) isSerializedAsElement);
    }

    public static bool IsSerializedAsElement(this IEdmValue value, IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmValue>(value, nameof (value));
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue((IEdmElement) value, "http://schemas.microsoft.com/ado/2011/04/edm/internal", nameof (IsSerializedAsElement)) as bool? ?? false;
    }

    public static void SetNamespaceAlias(this IEdmModel model, string namespaceName, string alias)
    {
      VersioningDictionary<string, string> versioningDictionary = model.GetAnnotationValue<VersioningDictionary<string, string>>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "NamespaceAlias") ?? VersioningDictionary<string, string>.Create(new Func<string, string, int>(string.CompareOrdinal));
      if (EdmUtil.IsNullOrWhiteSpaceInternal(alias))
      {
        if (versioningDictionary.TryGetValue(namespaceName, out string _))
          versioningDictionary = versioningDictionary.Remove(namespaceName);
      }
      else
        versioningDictionary = versioningDictionary.Set(namespaceName, alias);
      model.SetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "NamespaceAlias", (object) versioningDictionary);
      VersioningList<string> source = model.GetAnnotationValue<VersioningList<string>>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "UsedNamespaces") ?? VersioningList<string>.Create();
      if (!string.IsNullOrEmpty(namespaceName) && !source.Contains<string>(namespaceName))
        source = source.Add(namespaceName);
      model.SetAnnotationValue((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "UsedNamespaces", (object) source);
    }

    public static string GetNamespaceAlias(this IEdmModel model, string namespaceName)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      VersioningDictionary<string, string> annotationValue = model.GetAnnotationValue<VersioningDictionary<string, string>>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "NamespaceAlias");
      string str;
      return annotationValue != null && annotationValue.TryGetValue(namespaceName, out str) ? str : (string) null;
    }

    internal static VersioningDictionary<string, string> GetNamespaceAliases(this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue<VersioningDictionary<string, string>>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "NamespaceAlias");
    }

    internal static VersioningList<string> GetUsedNamespacesHavingAlias(this IEdmModel model)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(model, nameof (model));
      return model.GetAnnotationValue<VersioningList<string>>((IEdmElement) model, "http://schemas.microsoft.com/ado/2011/04/edm/internal", "UsedNamespaces");
    }

    internal static bool IsInline(this IEdmVocabularyAnnotation annotation, IEdmModel model)
    {
      EdmVocabularyAnnotationSerializationLocation? serializationLocation1 = annotation.GetSerializationLocation(model);
      EdmVocabularyAnnotationSerializationLocation serializationLocation2 = EdmVocabularyAnnotationSerializationLocation.Inline;
      return serializationLocation1.GetValueOrDefault() == serializationLocation2 & serializationLocation1.HasValue || annotation.TargetString() == null;
    }

    internal static string TargetString(this IEdmVocabularyAnnotation annotation) => EdmUtil.FullyQualifiedName(annotation.Target);
  }
}
