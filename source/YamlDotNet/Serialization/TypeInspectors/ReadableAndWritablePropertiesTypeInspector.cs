// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.TypeInspectors.ReadableAndWritablePropertiesTypeInspector
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.TypeInspectors
{
  public sealed class ReadableAndWritablePropertiesTypeInspector : TypeInspectorSkeleton
  {
    private readonly ITypeInspector _innerTypeDescriptor;

    public ReadableAndWritablePropertiesTypeInspector(ITypeInspector innerTypeDescriptor) => this._innerTypeDescriptor = innerTypeDescriptor;

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) => this._innerTypeDescriptor.GetProperties(type, container).Where<IPropertyDescriptor>((Func<IPropertyDescriptor, bool>) (p => p.CanWrite));
  }
}
