// Decompiled with JetBrains decompiler
// Type: Nest.ClusterRerouteCommandFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class ClusterRerouteCommandFormatter : 
    IJsonFormatter<IClusterRerouteCommand>,
    IJsonFormatter
  {
    private static readonly AutomataDictionary Commands = new AutomataDictionary()
    {
      {
        "allocate_replica",
        0
      },
      {
        "allocate_empty_primary",
        1
      },
      {
        "allocate_stale_primary",
        2
      },
      {
        "move",
        3
      },
      {
        "cancel",
        4
      }
    };

    public IClusterRerouteCommand Deserialize(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      IClusterRerouteCommand clusterRerouteCommand = (IClusterRerouteCommand) null;
      int count = 0;
      while (reader.ReadIsInObject(ref count))
      {
        ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
        int num;
        if (ClusterRerouteCommandFormatter.Commands.TryGetValue(bytes, out num))
        {
          switch (num)
          {
            case 0:
              clusterRerouteCommand = (IClusterRerouteCommand) ClusterRerouteCommandFormatter.Deserialize<AllocateReplicaClusterRerouteCommand>(ref reader, formatterResolver);
              continue;
            case 1:
              clusterRerouteCommand = (IClusterRerouteCommand) ClusterRerouteCommandFormatter.Deserialize<AllocateEmptyPrimaryRerouteCommand>(ref reader, formatterResolver);
              continue;
            case 2:
              clusterRerouteCommand = (IClusterRerouteCommand) ClusterRerouteCommandFormatter.Deserialize<AllocateStalePrimaryRerouteCommand>(ref reader, formatterResolver);
              continue;
            case 3:
              clusterRerouteCommand = (IClusterRerouteCommand) ClusterRerouteCommandFormatter.Deserialize<MoveClusterRerouteCommand>(ref reader, formatterResolver);
              continue;
            case 4:
              clusterRerouteCommand = (IClusterRerouteCommand) ClusterRerouteCommandFormatter.Deserialize<CancelClusterRerouteCommand>(ref reader, formatterResolver);
              continue;
            default:
              continue;
          }
        }
        else
          reader.ReadNext();
      }
      return clusterRerouteCommand;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IClusterRerouteCommand value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName(value.Name);
        switch (value.Name)
        {
          case "allocate_replica":
            ClusterRerouteCommandFormatter.Serialize<IAllocateReplicaClusterRerouteCommand>(ref writer, value, formatterResolver);
            break;
          case "allocate_empty_primary":
            ClusterRerouteCommandFormatter.Serialize<IAllocateEmptyPrimaryRerouteCommand>(ref writer, value, formatterResolver);
            break;
          case "allocate_stale_primary":
            ClusterRerouteCommandFormatter.Serialize<IAllocateStalePrimaryRerouteCommand>(ref writer, value, formatterResolver);
            break;
          case "move":
            ClusterRerouteCommandFormatter.Serialize<IMoveClusterRerouteCommand>(ref writer, value, formatterResolver);
            break;
          case "cancel":
            ClusterRerouteCommandFormatter.Serialize<ICancelClusterRerouteCommand>(ref writer, value, formatterResolver);
            break;
          default:
            DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IClusterRerouteCommand>().Serialize(ref writer, value, formatterResolver);
            break;
        }
        writer.WriteEndObject();
      }
    }

    private static void Serialize<TCommand>(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IClusterRerouteCommand value,
      IJsonFormatterResolver formatterResolver)
      where TCommand : class, IClusterRerouteCommand
    {
      formatterResolver.GetFormatter<TCommand>().Serialize(ref writer, value as TCommand, formatterResolver);
    }

    private static TCommand Deserialize<TCommand>(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TCommand : IClusterRerouteCommand
    {
      return formatterResolver.GetFormatter<TCommand>().Deserialize(ref reader, formatterResolver);
    }
  }
}
