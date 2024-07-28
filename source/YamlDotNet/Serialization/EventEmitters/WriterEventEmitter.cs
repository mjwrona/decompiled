// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.EventEmitters.WriterEventEmitter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.EventEmitters
{
  public sealed class WriterEventEmitter : IEventEmitter
  {
    void IEventEmitter.Emit(AliasEventInfo eventInfo, IEmitter emitter) => emitter.Emit((ParsingEvent) new AnchorAlias(eventInfo.Alias));

    void IEventEmitter.Emit(ScalarEventInfo eventInfo, IEmitter emitter) => emitter.Emit((ParsingEvent) new Scalar(eventInfo.Anchor, eventInfo.Tag, eventInfo.RenderedValue, eventInfo.Style, eventInfo.IsPlainImplicit, eventInfo.IsQuotedImplicit));

    void IEventEmitter.Emit(MappingStartEventInfo eventInfo, IEmitter emitter) => emitter.Emit((ParsingEvent) new MappingStart(eventInfo.Anchor, eventInfo.Tag, eventInfo.IsImplicit, eventInfo.Style));

    void IEventEmitter.Emit(MappingEndEventInfo eventInfo, IEmitter emitter) => emitter.Emit((ParsingEvent) new MappingEnd());

    void IEventEmitter.Emit(SequenceStartEventInfo eventInfo, IEmitter emitter) => emitter.Emit((ParsingEvent) new SequenceStart(eventInfo.Anchor, eventInfo.Tag, eventInfo.IsImplicit, eventInfo.Style));

    void IEventEmitter.Emit(SequenceEndEventInfo eventInfo, IEmitter emitter) => emitter.Emit((ParsingEvent) new SequenceEnd());
  }
}
