// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectGraphVisitors.EmittingObjectGraphVisitor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
  public sealed class EmittingObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
  {
    private readonly IEventEmitter eventEmitter;

    public EmittingObjectGraphVisitor(IEventEmitter eventEmitter) => this.eventEmitter = eventEmitter;

    bool IObjectGraphVisitor<IEmitter>.Enter(IObjectDescriptor value, IEmitter context) => true;

    bool IObjectGraphVisitor<IEmitter>.EnterMapping(
      IObjectDescriptor key,
      IObjectDescriptor value,
      IEmitter context)
    {
      return true;
    }

    bool IObjectGraphVisitor<IEmitter>.EnterMapping(
      IPropertyDescriptor key,
      IObjectDescriptor value,
      IEmitter context)
    {
      return true;
    }

    void IObjectGraphVisitor<IEmitter>.VisitScalar(IObjectDescriptor scalar, IEmitter context) => this.eventEmitter.Emit(new ScalarEventInfo(scalar), context);

    void IObjectGraphVisitor<IEmitter>.VisitMappingStart(
      IObjectDescriptor mapping,
      Type keyType,
      Type valueType,
      IEmitter context)
    {
      this.eventEmitter.Emit(new MappingStartEventInfo(mapping), context);
    }

    void IObjectGraphVisitor<IEmitter>.VisitMappingEnd(IObjectDescriptor mapping, IEmitter context) => this.eventEmitter.Emit(new MappingEndEventInfo(mapping), context);

    void IObjectGraphVisitor<IEmitter>.VisitSequenceStart(
      IObjectDescriptor sequence,
      Type elementType,
      IEmitter context)
    {
      this.eventEmitter.Emit(new SequenceStartEventInfo(sequence), context);
    }

    void IObjectGraphVisitor<IEmitter>.VisitSequenceEnd(
      IObjectDescriptor sequence,
      IEmitter context)
    {
      this.eventEmitter.Emit(new SequenceEndEventInfo(sequence), context);
    }
  }
}
