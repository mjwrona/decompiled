// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.IObjectGraphVisitor`1
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Serialization
{
  public interface IObjectGraphVisitor<TContext>
  {
    bool Enter(IObjectDescriptor value, TContext context);

    bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, TContext context);

    bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, TContext context);

    void VisitScalar(IObjectDescriptor scalar, TContext context);

    void VisitMappingStart(
      IObjectDescriptor mapping,
      Type keyType,
      Type valueType,
      TContext context);

    void VisitMappingEnd(IObjectDescriptor mapping, TContext context);

    void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, TContext context);

    void VisitSequenceEnd(IObjectDescriptor sequence, TContext context);
  }
}
