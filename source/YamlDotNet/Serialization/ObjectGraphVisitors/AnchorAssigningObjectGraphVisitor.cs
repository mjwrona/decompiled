// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ObjectGraphVisitors.AnchorAssigningObjectGraphVisitor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
  public sealed class AnchorAssigningObjectGraphVisitor : ChainedObjectGraphVisitor
  {
    private readonly IEventEmitter eventEmitter;
    private readonly IAliasProvider aliasProvider;
    private readonly HashSet<string> emittedAliases = new HashSet<string>();

    public AnchorAssigningObjectGraphVisitor(
      IObjectGraphVisitor<IEmitter> nextVisitor,
      IEventEmitter eventEmitter,
      IAliasProvider aliasProvider)
      : base(nextVisitor)
    {
      this.eventEmitter = eventEmitter;
      this.aliasProvider = aliasProvider;
    }

    public override bool Enter(IObjectDescriptor value, IEmitter context)
    {
      string alias = this.aliasProvider.GetAlias(value.Value);
      if (alias == null || this.emittedAliases.Add(alias))
        return base.Enter(value, context);
      IEventEmitter eventEmitter = this.eventEmitter;
      AliasEventInfo eventInfo = new AliasEventInfo(value);
      eventInfo.Alias = alias;
      IEmitter emitter = context;
      eventEmitter.Emit(eventInfo, emitter);
      return false;
    }

    public override void VisitMappingStart(
      IObjectDescriptor mapping,
      Type keyType,
      Type valueType,
      IEmitter context)
    {
      IEventEmitter eventEmitter = this.eventEmitter;
      MappingStartEventInfo eventInfo = new MappingStartEventInfo(mapping);
      eventInfo.Anchor = this.aliasProvider.GetAlias(mapping.Value);
      IEmitter emitter = context;
      eventEmitter.Emit(eventInfo, emitter);
    }

    public override void VisitSequenceStart(
      IObjectDescriptor sequence,
      Type elementType,
      IEmitter context)
    {
      IEventEmitter eventEmitter = this.eventEmitter;
      SequenceStartEventInfo eventInfo = new SequenceStartEventInfo(sequence);
      eventInfo.Anchor = this.aliasProvider.GetAlias(sequence.Value);
      IEmitter emitter = context;
      eventEmitter.Emit(eventInfo, emitter);
    }

    public override void VisitScalar(IObjectDescriptor scalar, IEmitter context)
    {
      IEventEmitter eventEmitter = this.eventEmitter;
      ScalarEventInfo eventInfo = new ScalarEventInfo(scalar);
      eventInfo.Anchor = this.aliasProvider.GetAlias(scalar.Value);
      IEmitter emitter = context;
      eventEmitter.Emit(eventInfo, emitter);
    }
  }
}
