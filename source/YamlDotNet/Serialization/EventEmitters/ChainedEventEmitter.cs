// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.EventEmitters.ChainedEventEmitter
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.EventEmitters
{
  public abstract class ChainedEventEmitter : IEventEmitter
  {
    protected readonly IEventEmitter nextEmitter;

    protected ChainedEventEmitter(IEventEmitter nextEmitter) => this.nextEmitter = nextEmitter != null ? nextEmitter : throw new ArgumentNullException(nameof (nextEmitter));

    public virtual void Emit(AliasEventInfo eventInfo, IEmitter emitter) => this.nextEmitter.Emit(eventInfo, emitter);

    public virtual void Emit(ScalarEventInfo eventInfo, IEmitter emitter) => this.nextEmitter.Emit(eventInfo, emitter);

    public virtual void Emit(MappingStartEventInfo eventInfo, IEmitter emitter) => this.nextEmitter.Emit(eventInfo, emitter);

    public virtual void Emit(MappingEndEventInfo eventInfo, IEmitter emitter) => this.nextEmitter.Emit(eventInfo, emitter);

    public virtual void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter) => this.nextEmitter.Emit(eventInfo, emitter);

    public virtual void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter) => this.nextEmitter.Emit(eventInfo, emitter);
  }
}
