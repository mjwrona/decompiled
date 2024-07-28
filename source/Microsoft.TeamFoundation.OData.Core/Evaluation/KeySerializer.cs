// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Evaluation.KeySerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Evaluation
{
  internal abstract class KeySerializer
  {
    private static readonly KeySerializer.DefaultKeySerializer DefaultInstance = new KeySerializer.DefaultKeySerializer();
    private static readonly KeySerializer.SegmentKeySerializer SegmentInstance = new KeySerializer.SegmentKeySerializer();

    internal static KeySerializer Create(bool enableKeyAsSegment) => enableKeyAsSegment ? (KeySerializer) KeySerializer.SegmentInstance : (KeySerializer) KeySerializer.DefaultInstance;

    internal abstract void AppendKeyExpression<TProperty>(
      StringBuilder builder,
      ICollection<TProperty> keyProperties,
      Func<TProperty, string> getPropertyName,
      Func<TProperty, object> getPropertyValue);

    private static string GetKeyValueAsString<TProperty>(
      Func<TProperty, object> getPropertyValue,
      TProperty property,
      LiteralFormatter literalFormatter)
    {
      object obj = getPropertyValue(property);
      return literalFormatter.Format(obj);
    }

    private static void AppendKeyWithParentheses<TProperty>(
      StringBuilder builder,
      ICollection<TProperty> keyProperties,
      Func<TProperty, string> getPropertyName,
      Func<TProperty, object> getPropertyValue)
    {
      LiteralFormatter literalFormatter = LiteralFormatter.ForKeys(false);
      builder.Append('(');
      bool flag = true;
      foreach (TProperty keyProperty in (IEnumerable<TProperty>) keyProperties)
      {
        if (flag)
          flag = false;
        else
          builder.Append(',');
        if (keyProperties.Count != 1)
        {
          builder.Append(getPropertyName(keyProperty));
          builder.Append('=');
        }
        string keyValueAsString = KeySerializer.GetKeyValueAsString<TProperty>(getPropertyValue, keyProperty, literalFormatter);
        builder.Append(keyValueAsString);
      }
      builder.Append(')');
    }

    private sealed class DefaultKeySerializer : KeySerializer
    {
      internal override void AppendKeyExpression<TProperty>(
        StringBuilder builder,
        ICollection<TProperty> keyProperties,
        Func<TProperty, string> getPropertyName,
        Func<TProperty, object> getPropertyValue)
      {
        KeySerializer.AppendKeyWithParentheses<TProperty>(builder, keyProperties, getPropertyName, getPropertyValue);
      }
    }

    private sealed class SegmentKeySerializer : KeySerializer
    {
      internal SegmentKeySerializer()
      {
      }

      internal override void AppendKeyExpression<TProperty>(
        StringBuilder builder,
        ICollection<TProperty> keyProperties,
        Func<TProperty, string> getPropertyName,
        Func<TProperty, object> getPropertyValue)
      {
        if (keyProperties.Count > 1)
          KeySerializer.AppendKeyWithParentheses<TProperty>(builder, keyProperties, getPropertyName, getPropertyValue);
        else
          KeySerializer.SegmentKeySerializer.AppendKeyWithSegments<TProperty>(builder, keyProperties, getPropertyValue);
      }

      private static void AppendKeyWithSegments<TProperty>(
        StringBuilder builder,
        ICollection<TProperty> keyProperties,
        Func<TProperty, object> getPropertyValue)
      {
        builder.Append('/');
        builder.Append(KeySerializer.GetKeyValueAsString<TProperty>(getPropertyValue, keyProperties.Single<TProperty>(), LiteralFormatter.ForKeys(true)));
      }
    }
  }
}
