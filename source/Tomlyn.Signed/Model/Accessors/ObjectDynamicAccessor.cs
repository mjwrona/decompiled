// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.Accessors.ObjectDynamicAccessor
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using Tomlyn.Syntax;


#nullable enable
namespace Tomlyn.Model.Accessors
{
  internal abstract class ObjectDynamicAccessor : DynamicAccessor
  {
    protected ObjectDynamicAccessor(
      DynamicModelReadContext context,
      Type targetType,
      ReflectionObjectKind kind)
      : base(context, targetType, kind)
    {
    }

    public abstract IEnumerable<KeyValuePair<string, object?>> GetProperties(object obj);

    public abstract bool TryGetPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      out object? value);

    public abstract bool TrySetPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      object? value);

    public abstract bool TryGetPropertyType(SourceSpan span, string name, out Type? propertyType);

    public abstract bool TryCreateAndSetDefaultPropertyValue(
      SourceSpan span,
      object obj,
      string name,
      ObjectKind kind,
      out object? instance);
  }
}
