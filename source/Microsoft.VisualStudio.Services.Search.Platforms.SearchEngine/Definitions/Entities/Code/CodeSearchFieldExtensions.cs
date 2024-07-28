// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code.CodeSearchFieldExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code
{
  public static class CodeSearchFieldExtensions
  {
    public static string InlineFilterName(
      this CodeFileContract.CodeContractQueryableElement value)
    {
      CodeSearchFieldExtensions.InlineFilterNameAttribute[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.InlineFilterNameAttribute), false) as CodeSearchFieldExtensions.InlineFilterNameAttribute[];
      return customAttributes.Length == 0 ? value.ToString() : customAttributes[0].StringValue;
    }

    public static string FacetFilterName(this Enum value)
    {
      CodeSearchFieldExtensions.FacetFilterNameAttribute[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.FacetFilterNameAttribute), false) as CodeSearchFieldExtensions.FacetFilterNameAttribute[];
      return customAttributes.Length == 0 ? (string) null : customAttributes[0].StringValue;
    }

    public static bool IsStoredField(this Enum value) => (value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.IsStoredFieldAttribute), false) as CodeSearchFieldExtensions.IsStoredFieldAttribute[]).Length != 0;

    public static string ElasticsearchFieldName(this Enum value)
    {
      CodeSearchFieldExtensions.ElasticsearchFieldNameAttribute[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.ElasticsearchFieldNameAttribute), false) as CodeSearchFieldExtensions.ElasticsearchFieldNameAttribute[];
      return customAttributes.Length == 0 ? (string) null : customAttributes[0].StringValue;
    }

    public static CodeContractField.CodeSearchFieldDesc StoredToField(this Enum value)
    {
      CodeSearchFieldExtensions.StoredToFieldAttribute[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.StoredToFieldAttribute), false) as CodeSearchFieldExtensions.StoredToFieldAttribute[];
      return (CodeContractField.CodeSearchFieldDesc) (customAttributes.Length != 0 ? (Enum) customAttributes[0].RelatedField : (value.IsStoredField() ? value : (Enum) CodeContractField.CodeSearchFieldDesc.None));
    }

    public static CodeContractField.CodeSearchFieldDesc StoredForField(this Enum value)
    {
      CodeSearchFieldExtensions.StoredForFieldAttribute[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.StoredForFieldAttribute), false) as CodeSearchFieldExtensions.StoredForFieldAttribute[];
      return (CodeContractField.CodeSearchFieldDesc) (customAttributes.Length != 0 ? (Enum) customAttributes[0].RelatedField : (value.IsStoredField() ? value : (Enum) CodeContractField.CodeSearchFieldDesc.None));
    }

    public static CodeFileContract.CodeContractQueryableElement QueryableElement(this Enum value)
    {
      CodeSearchFieldExtensions.CodeContractQueryableElementAttribute[] customAttributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (CodeSearchFieldExtensions.CodeContractQueryableElementAttribute), false) as CodeSearchFieldExtensions.CodeContractQueryableElementAttribute[];
      return customAttributes.Length == 0 ? CodeFileContract.CodeContractQueryableElement.None : customAttributes[0].QueryableElement;
    }

    public static IEnumerable<CodeContractField.CodeSearchFieldDesc> GetCodeSearchFieldDescsValues() => Enum.GetValues(typeof (CodeContractField.CodeSearchFieldDesc)).Cast<CodeContractField.CodeSearchFieldDesc>();

    public static IEnumerable<CodeFileContract.CodeContractQueryableElement> GetQueryableElementValues() => Enum.GetValues(typeof (CodeFileContract.CodeContractQueryableElement)).Cast<CodeFileContract.CodeContractQueryableElement>();

    public static bool IsOfType(
      this TermExpression termExpression,
      CodeFileContract.CodeContractQueryableElement type)
    {
      return termExpression.Type.Equals(type.InlineFilterName(), StringComparison.OrdinalIgnoreCase);
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InlineFilterNameAttribute : Attribute
    {
      public string StringValue { get; }

      public InlineFilterNameAttribute(string value) => this.StringValue = value;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FacetFilterNameAttribute : Attribute
    {
      public string StringValue { get; }

      public FacetFilterNameAttribute(string value) => this.StringValue = value;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ElasticsearchFieldNameAttribute : Attribute
    {
      public string StringValue { get; }

      public ElasticsearchFieldNameAttribute(string value) => this.StringValue = value;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StoredToFieldAttribute : Attribute
    {
      public CodeContractField.CodeSearchFieldDesc RelatedField { get; }

      public StoredToFieldAttribute(CodeContractField.CodeSearchFieldDesc value) => this.RelatedField = value;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StoredForFieldAttribute : Attribute
    {
      public CodeContractField.CodeSearchFieldDesc RelatedField { get; }

      public StoredForFieldAttribute(CodeContractField.CodeSearchFieldDesc value) => this.RelatedField = value;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class IsStoredFieldAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodeContractQueryableElementAttribute : Attribute
    {
      public CodeFileContract.CodeContractQueryableElement QueryableElement { get; }

      public CodeContractQueryableElementAttribute(
        CodeFileContract.CodeContractQueryableElement value)
      {
        this.QueryableElement = value;
      }
    }
  }
}
