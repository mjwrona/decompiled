// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.EmissionPhaseObjectGraphVisitorArgs
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
  public sealed class EmissionPhaseObjectGraphVisitorArgs
  {
    private readonly IEnumerable<IObjectGraphVisitor<Nothing>> preProcessingPhaseVisitors;

    public IObjectGraphVisitor<IEmitter> InnerVisitor { get; private set; }

    public IEventEmitter EventEmitter { get; private set; }

    public ObjectSerializer NestedObjectSerializer { get; private set; }

    public IEnumerable<IYamlTypeConverter> TypeConverters { get; private set; }

    public EmissionPhaseObjectGraphVisitorArgs(
      IObjectGraphVisitor<IEmitter> innerVisitor,
      IEventEmitter eventEmitter,
      IEnumerable<IObjectGraphVisitor<Nothing>> preProcessingPhaseVisitors,
      IEnumerable<IYamlTypeConverter> typeConverters,
      ObjectSerializer nestedObjectSerializer)
    {
      this.InnerVisitor = innerVisitor != null ? innerVisitor : throw new ArgumentNullException(nameof (innerVisitor));
      this.EventEmitter = eventEmitter != null ? eventEmitter : throw new ArgumentNullException(nameof (eventEmitter));
      this.preProcessingPhaseVisitors = preProcessingPhaseVisitors != null ? preProcessingPhaseVisitors : throw new ArgumentNullException(nameof (preProcessingPhaseVisitors));
      this.TypeConverters = typeConverters != null ? typeConverters : throw new ArgumentNullException(nameof (typeConverters));
      this.NestedObjectSerializer = nestedObjectSerializer != null ? nestedObjectSerializer : throw new ArgumentNullException(nameof (nestedObjectSerializer));
    }

    public T GetPreProcessingPhaseObjectGraphVisitor<T>() where T : IObjectGraphVisitor<Nothing> => this.preProcessingPhaseVisitors.OfType<T>().Single<T>();
  }
}
