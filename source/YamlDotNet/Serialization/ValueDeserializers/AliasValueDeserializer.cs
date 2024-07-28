// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.ValueDeserializers.AliasValueDeserializer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
  public sealed class AliasValueDeserializer : IValueDeserializer
  {
    private readonly IValueDeserializer innerDeserializer;

    public AliasValueDeserializer(IValueDeserializer innerDeserializer) => this.innerDeserializer = innerDeserializer != null ? innerDeserializer : throw new ArgumentNullException(nameof (innerDeserializer));

    public object DeserializeValue(
      IParser parser,
      Type expectedType,
      SerializerState state,
      IValueDeserializer nestedObjectDeserializer)
    {
      AnchorAlias alias = parser.Allow<AnchorAlias>();
      if (alias != null)
      {
        AliasValueDeserializer.AliasState aliasState = state.Get<AliasValueDeserializer.AliasState>();
        AliasValueDeserializer.ValuePromise valuePromise;
        if (!aliasState.TryGetValue(alias.Value, out valuePromise))
        {
          valuePromise = new AliasValueDeserializer.ValuePromise(alias);
          aliasState.Add(alias.Value, valuePromise);
        }
        return !valuePromise.HasValue ? (object) valuePromise : valuePromise.Value;
      }
      string key = (string) null;
      NodeEvent nodeEvent = parser.Peek<NodeEvent>();
      if (nodeEvent != null && !string.IsNullOrEmpty(nodeEvent.Anchor))
        key = nodeEvent.Anchor;
      object obj = this.innerDeserializer.DeserializeValue(parser, expectedType, state, nestedObjectDeserializer);
      if (key != null)
      {
        AliasValueDeserializer.AliasState aliasState = state.Get<AliasValueDeserializer.AliasState>();
        AliasValueDeserializer.ValuePromise valuePromise;
        if (!aliasState.TryGetValue(key, out valuePromise))
          aliasState.Add(key, new AliasValueDeserializer.ValuePromise(obj));
        else if (!valuePromise.HasValue)
          valuePromise.Value = obj;
        else
          aliasState[key] = new AliasValueDeserializer.ValuePromise(obj);
      }
      return obj;
    }

    private sealed class AliasState : 
      Dictionary<string, AliasValueDeserializer.ValuePromise>,
      IPostDeserializationCallback
    {
      public void OnDeserialization()
      {
        foreach (AliasValueDeserializer.ValuePromise valuePromise in this.Values)
        {
          if (!valuePromise.HasValue)
            throw new AnchorNotFoundException(valuePromise.Alias.Start, valuePromise.Alias.End, string.Format("Anchor '{0}' not found", (object) valuePromise.Alias.Value));
        }
      }
    }

    private sealed class ValuePromise : IValuePromise
    {
      private object value;
      public readonly AnchorAlias Alias;

      public event Action<object> ValueAvailable;

      public bool HasValue { get; private set; }

      public ValuePromise(AnchorAlias alias) => this.Alias = alias;

      public ValuePromise(object value)
      {
        this.HasValue = true;
        this.value = value;
      }

      public object Value
      {
        get
        {
          if (!this.HasValue)
            throw new InvalidOperationException("Value not set");
          return this.value;
        }
        set
        {
          this.HasValue = !this.HasValue ? true : throw new InvalidOperationException("Value already set");
          this.value = value;
          if (this.ValueAvailable == null)
            return;
          this.ValueAvailable(value);
        }
      }
    }
  }
}
