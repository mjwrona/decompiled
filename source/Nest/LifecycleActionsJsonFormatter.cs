// Decompiled with JetBrains decompiler
// Type: Nest.LifecycleActionsJsonFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class LifecycleActionsJsonFormatter : IJsonFormatter<ILifecycleActions>, IJsonFormatter
  {
    private static readonly AutomataDictionary LifeCycleActions = new AutomataDictionary()
    {
      {
        "allocate",
        0
      },
      {
        "delete",
        1
      },
      {
        "forcemerge",
        2
      },
      {
        "freeze",
        3
      },
      {
        "readonly",
        4
      },
      {
        "rollover",
        5
      },
      {
        "set_priority",
        6
      },
      {
        "shrink",
        7
      },
      {
        "unfollow",
        8
      },
      {
        "wait_for_snapshot",
        9
      },
      {
        "searchable_snapshot",
        10
      }
    };

    public ILifecycleActions Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (ILifecycleActions) null;
      }
      Dictionary<string, ILifecycleAction> container = new Dictionary<string, ILifecycleAction>();
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> segment = reader.ReadPropertyNameSegmentRaw();
        ILifecycleAction lifecycleAction = (ILifecycleAction) null;
        int num;
        if (LifecycleActionsJsonFormatter.LifeCycleActions.TryGetValue(segment, out num))
        {
          switch (num)
          {
            case 0:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<AllocateLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 1:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<DeleteLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 2:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<ForceMergeLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 3:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<FreezeLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 4:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<ReadOnlyLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 5:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<RolloverLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 6:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<SetPriorityLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 7:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<ShrinkLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 8:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<UnfollowLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 9:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<WaitForSnapshotLifecycleAction>().Deserialize(ref reader, formatterResolver);
              break;
            case 10:
              lifecycleAction = (ILifecycleAction) formatterResolver.GetFormatter<SearchableSnapshotAction>().Deserialize(ref reader, formatterResolver);
              break;
          }
          container.Add(segment.Utf8String(), lifecycleAction);
        }
        else
          reader.ReadNextBlock();
      }
      return (ILifecycleActions) new LifecycleActions((IDictionary<string, ILifecycleAction>) container);
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ILifecycleActions value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        int num = 0;
        foreach (KeyValuePair<string, ILifecycleAction> keyValuePair in (IEnumerable<KeyValuePair<string, ILifecycleAction>>) value)
        {
          if (num > 0)
            writer.WriteValueSeparator();
          writer.WritePropertyName(keyValuePair.Key);
          switch (keyValuePair.Key)
          {
            case "allocate":
              this.Serialize<IAllocateLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "delete":
              this.Serialize<IDeleteLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "forcemerge":
              this.Serialize<IForceMergeLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "freeze":
              this.Serialize<IFreezeLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "readonly":
              this.Serialize<IReadOnlyLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "rollover":
              this.Serialize<IRolloverLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "searchable_snapshot":
              this.Serialize<ISearchableSnapshotAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "set_priority":
              this.Serialize<ISetPriorityLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "shrink":
              this.Serialize<IShrinkLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "unfollow":
              this.Serialize<IUnfollowLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            case "wait_for_snapshot":
              this.Serialize<IWaitForSnapshotLifecycleAction>(ref writer, keyValuePair.Value, formatterResolver);
              break;
            default:
              DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<ILifecycleAction>().Serialize(ref writer, keyValuePair.Value, formatterResolver);
              break;
          }
          ++num;
        }
        writer.WriteEndObject();
      }
    }

    private void Serialize<TLifecycleAction>(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      ILifecycleAction value,
      IJsonFormatterResolver formatterResolver)
      where TLifecycleAction : ILifecycleAction
    {
      formatterResolver.GetFormatter<TLifecycleAction>().Serialize(ref writer, (TLifecycleAction) value, formatterResolver);
    }
  }
}
