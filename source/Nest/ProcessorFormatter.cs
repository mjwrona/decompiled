// Decompiled with JetBrains decompiler
// Type: Nest.ProcessorFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Internal;
using Elasticsearch.Net.Utf8Json.Resolvers;
using System;

namespace Nest
{
  internal class ProcessorFormatter : IJsonFormatter<IProcessor>, IJsonFormatter
  {
    private static readonly AutomataDictionary Processors = new AutomataDictionary()
    {
      {
        "attachment",
        0
      },
      {
        "append",
        1
      },
      {
        "convert",
        2
      },
      {
        "date",
        3
      },
      {
        "date_index_name",
        4
      },
      {
        "dot_expander",
        5
      },
      {
        "fail",
        6
      },
      {
        "foreach",
        7
      },
      {
        "json",
        8
      },
      {
        "user_agent",
        9
      },
      {
        "kv",
        10
      },
      {
        "geoip",
        11
      },
      {
        "grok",
        12
      },
      {
        "gsub",
        13
      },
      {
        "join",
        14
      },
      {
        "lowercase",
        15
      },
      {
        "remove",
        16
      },
      {
        "rename",
        17
      },
      {
        "script",
        18
      },
      {
        "set",
        19
      },
      {
        "sort",
        20
      },
      {
        "split",
        21
      },
      {
        "trim",
        22
      },
      {
        "uppercase",
        23
      },
      {
        "urldecode",
        24
      },
      {
        "bytes",
        25
      },
      {
        "dissect",
        26
      },
      {
        "set_security_user",
        27
      },
      {
        "pipeline",
        28
      },
      {
        "drop",
        29
      },
      {
        "circle",
        30
      },
      {
        "enrich",
        31
      },
      {
        "csv",
        32
      },
      {
        "uri_parts",
        33
      },
      {
        "fingerprint",
        34
      },
      {
        "community_id",
        35
      },
      {
        "network_direction",
        36
      },
      {
        "registered_domain",
        37
      },
      {
        "inference",
        38
      }
    };

    public IProcessor Deserialize(ref Elasticsearch.Net.Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
      if (reader.GetCurrentJsonToken() != JsonToken.BeginObject)
      {
        reader.ReadNextBlock();
        return (IProcessor) null;
      }
      reader.ReadNext();
      IProcessor processor = (IProcessor) null;
      ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
      int num;
      if (ProcessorFormatter.Processors.TryGetValue(bytes, out num))
      {
        switch (num)
        {
          case 0:
            processor = (IProcessor) ProcessorFormatter.Deserialize<AttachmentProcessor>(ref reader, formatterResolver);
            break;
          case 1:
            processor = (IProcessor) ProcessorFormatter.Deserialize<AppendProcessor>(ref reader, formatterResolver);
            break;
          case 2:
            processor = (IProcessor) ProcessorFormatter.Deserialize<ConvertProcessor>(ref reader, formatterResolver);
            break;
          case 3:
            processor = (IProcessor) ProcessorFormatter.Deserialize<DateProcessor>(ref reader, formatterResolver);
            break;
          case 4:
            processor = (IProcessor) ProcessorFormatter.Deserialize<DateIndexNameProcessor>(ref reader, formatterResolver);
            break;
          case 5:
            processor = (IProcessor) ProcessorFormatter.Deserialize<DotExpanderProcessor>(ref reader, formatterResolver);
            break;
          case 6:
            processor = (IProcessor) ProcessorFormatter.Deserialize<FailProcessor>(ref reader, formatterResolver);
            break;
          case 7:
            processor = (IProcessor) ProcessorFormatter.Deserialize<ForeachProcessor>(ref reader, formatterResolver);
            break;
          case 8:
            processor = (IProcessor) ProcessorFormatter.Deserialize<JsonProcessor>(ref reader, formatterResolver);
            break;
          case 9:
            processor = (IProcessor) ProcessorFormatter.Deserialize<UserAgentProcessor>(ref reader, formatterResolver);
            break;
          case 10:
            processor = (IProcessor) ProcessorFormatter.Deserialize<KeyValueProcessor>(ref reader, formatterResolver);
            break;
          case 11:
            processor = (IProcessor) ProcessorFormatter.Deserialize<GeoIpProcessor>(ref reader, formatterResolver);
            break;
          case 12:
            processor = (IProcessor) ProcessorFormatter.Deserialize<GrokProcessor>(ref reader, formatterResolver);
            break;
          case 13:
            processor = (IProcessor) ProcessorFormatter.Deserialize<GsubProcessor>(ref reader, formatterResolver);
            break;
          case 14:
            processor = (IProcessor) ProcessorFormatter.Deserialize<JoinProcessor>(ref reader, formatterResolver);
            break;
          case 15:
            processor = (IProcessor) ProcessorFormatter.Deserialize<LowercaseProcessor>(ref reader, formatterResolver);
            break;
          case 16:
            processor = (IProcessor) ProcessorFormatter.Deserialize<RemoveProcessor>(ref reader, formatterResolver);
            break;
          case 17:
            processor = (IProcessor) ProcessorFormatter.Deserialize<RenameProcessor>(ref reader, formatterResolver);
            break;
          case 18:
            processor = (IProcessor) ProcessorFormatter.Deserialize<ScriptProcessor>(ref reader, formatterResolver);
            break;
          case 19:
            processor = (IProcessor) ProcessorFormatter.Deserialize<SetProcessor>(ref reader, formatterResolver);
            break;
          case 20:
            processor = (IProcessor) ProcessorFormatter.Deserialize<SortProcessor>(ref reader, formatterResolver);
            break;
          case 21:
            processor = (IProcessor) ProcessorFormatter.Deserialize<SplitProcessor>(ref reader, formatterResolver);
            break;
          case 22:
            processor = (IProcessor) ProcessorFormatter.Deserialize<TrimProcessor>(ref reader, formatterResolver);
            break;
          case 23:
            processor = (IProcessor) ProcessorFormatter.Deserialize<UppercaseProcessor>(ref reader, formatterResolver);
            break;
          case 24:
            processor = (IProcessor) ProcessorFormatter.Deserialize<UrlDecodeProcessor>(ref reader, formatterResolver);
            break;
          case 25:
            processor = (IProcessor) ProcessorFormatter.Deserialize<BytesProcessor>(ref reader, formatterResolver);
            break;
          case 26:
            processor = (IProcessor) ProcessorFormatter.Deserialize<DissectProcessor>(ref reader, formatterResolver);
            break;
          case 27:
            processor = (IProcessor) ProcessorFormatter.Deserialize<SetSecurityUserProcessor>(ref reader, formatterResolver);
            break;
          case 28:
            processor = (IProcessor) ProcessorFormatter.Deserialize<PipelineProcessor>(ref reader, formatterResolver);
            break;
          case 29:
            processor = (IProcessor) ProcessorFormatter.Deserialize<DropProcessor>(ref reader, formatterResolver);
            break;
          case 30:
            processor = (IProcessor) ProcessorFormatter.Deserialize<CircleProcessor>(ref reader, formatterResolver);
            break;
          case 31:
            processor = (IProcessor) ProcessorFormatter.Deserialize<EnrichProcessor>(ref reader, formatterResolver);
            break;
          case 32:
            processor = (IProcessor) ProcessorFormatter.Deserialize<CsvProcessor>(ref reader, formatterResolver);
            break;
          case 33:
            processor = (IProcessor) ProcessorFormatter.Deserialize<UriPartsProcessor>(ref reader, formatterResolver);
            break;
          case 34:
            processor = (IProcessor) ProcessorFormatter.Deserialize<FingerprintProcessor>(ref reader, formatterResolver);
            break;
          case 35:
            processor = (IProcessor) ProcessorFormatter.Deserialize<NetworkCommunityIdProcessor>(ref reader, formatterResolver);
            break;
          case 36:
            processor = (IProcessor) ProcessorFormatter.Deserialize<NetworkDirectionProcessor>(ref reader, formatterResolver);
            break;
          case 37:
            processor = (IProcessor) ProcessorFormatter.Deserialize<RegisteredDomainProcessor>(ref reader, formatterResolver);
            break;
          case 38:
            processor = (IProcessor) ProcessorFormatter.Deserialize<InferenceProcessor>(ref reader, formatterResolver);
            break;
        }
      }
      else
        reader.ReadNextBlock();
      reader.ReadIsEndObjectWithVerify();
      return processor;
    }

    public void Serialize(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IProcessor value,
      IJsonFormatterResolver formatterResolver)
    {
      if (value == null || value.Name == null)
      {
        writer.WriteNull();
      }
      else
      {
        writer.WriteBeginObject();
        writer.WritePropertyName(value.Name);
        switch (value.Name)
        {
          case "append":
            ProcessorFormatter.Serialize<IAppendProcessor>(ref writer, value, formatterResolver);
            break;
          case "attachment":
            ProcessorFormatter.Serialize<IAttachmentProcessor>(ref writer, value, formatterResolver);
            break;
          case "bytes":
            ProcessorFormatter.Serialize<IBytesProcessor>(ref writer, value, formatterResolver);
            break;
          case "circle":
            ProcessorFormatter.Serialize<ICircleProcessor>(ref writer, value, formatterResolver);
            break;
          case "community_id":
            ProcessorFormatter.Serialize<INetworkCommunityIdProcessor>(ref writer, value, formatterResolver);
            break;
          case "convert":
            ProcessorFormatter.Serialize<IConvertProcessor>(ref writer, value, formatterResolver);
            break;
          case "csv":
            ProcessorFormatter.Serialize<ICsvProcessor>(ref writer, value, formatterResolver);
            break;
          case "date":
            ProcessorFormatter.Serialize<IDateProcessor>(ref writer, value, formatterResolver);
            break;
          case "date_index_name":
            ProcessorFormatter.Serialize<IDateIndexNameProcessor>(ref writer, value, formatterResolver);
            break;
          case "dissect":
            ProcessorFormatter.Serialize<IDissectProcessor>(ref writer, value, formatterResolver);
            break;
          case "dot_expander":
            ProcessorFormatter.Serialize<IDotExpanderProcessor>(ref writer, value, formatterResolver);
            break;
          case "drop":
            ProcessorFormatter.Serialize<IDropProcessor>(ref writer, value, formatterResolver);
            break;
          case "enrich":
            ProcessorFormatter.Serialize<IEnrichProcessor>(ref writer, value, formatterResolver);
            break;
          case "fail":
            ProcessorFormatter.Serialize<IFailProcessor>(ref writer, value, formatterResolver);
            break;
          case "fingerprint":
            ProcessorFormatter.Serialize<IFingerprintProcessor>(ref writer, value, formatterResolver);
            break;
          case "foreach":
            ProcessorFormatter.Serialize<IForeachProcessor>(ref writer, value, formatterResolver);
            break;
          case "geoip":
            ProcessorFormatter.Serialize<IGeoIpProcessor>(ref writer, value, formatterResolver);
            break;
          case "grok":
            ProcessorFormatter.Serialize<IGrokProcessor>(ref writer, value, formatterResolver);
            break;
          case "gsub":
            ProcessorFormatter.Serialize<IGsubProcessor>(ref writer, value, formatterResolver);
            break;
          case "inference":
            ProcessorFormatter.Serialize<IInferenceProcessor>(ref writer, value, formatterResolver);
            break;
          case "join":
            ProcessorFormatter.Serialize<IJoinProcessor>(ref writer, value, formatterResolver);
            break;
          case "json":
            ProcessorFormatter.Serialize<IJsonProcessor>(ref writer, value, formatterResolver);
            break;
          case "kv":
            ProcessorFormatter.Serialize<IKeyValueProcessor>(ref writer, value, formatterResolver);
            break;
          case "lowercase":
            ProcessorFormatter.Serialize<ILowercaseProcessor>(ref writer, value, formatterResolver);
            break;
          case "network_direction":
            ProcessorFormatter.Serialize<INetworkDirectionProcessor>(ref writer, value, formatterResolver);
            break;
          case "pipeline":
            ProcessorFormatter.Serialize<IPipelineProcessor>(ref writer, value, formatterResolver);
            break;
          case "registered_domain":
            ProcessorFormatter.Serialize<IRegisteredDomainProcessor>(ref writer, value, formatterResolver);
            break;
          case "remove":
            ProcessorFormatter.Serialize<IRemoveProcessor>(ref writer, value, formatterResolver);
            break;
          case "rename":
            ProcessorFormatter.Serialize<IRenameProcessor>(ref writer, value, formatterResolver);
            break;
          case "script":
            ProcessorFormatter.Serialize<IScriptProcessor>(ref writer, value, formatterResolver);
            break;
          case "set":
            ProcessorFormatter.Serialize<ISetProcessor>(ref writer, value, formatterResolver);
            break;
          case "set_security_user":
            ProcessorFormatter.Serialize<ISetSecurityUserProcessor>(ref writer, value, formatterResolver);
            break;
          case "sort":
            ProcessorFormatter.Serialize<ISortProcessor>(ref writer, value, formatterResolver);
            break;
          case "split":
            ProcessorFormatter.Serialize<ISplitProcessor>(ref writer, value, formatterResolver);
            break;
          case "trim":
            ProcessorFormatter.Serialize<ITrimProcessor>(ref writer, value, formatterResolver);
            break;
          case "uppercase":
            ProcessorFormatter.Serialize<IUppercaseProcessor>(ref writer, value, formatterResolver);
            break;
          case "uri_parts":
            ProcessorFormatter.Serialize<IUriPartsProcessor>(ref writer, value, formatterResolver);
            break;
          case "urldecode":
            ProcessorFormatter.Serialize<IUrlDecodeProcessor>(ref writer, value, formatterResolver);
            break;
          case "user_agent":
            ProcessorFormatter.Serialize<IUserAgentProcessor>(ref writer, value, formatterResolver);
            break;
          default:
            DynamicObjectResolver.ExcludeNullCamelCase.GetFormatter<IProcessor>().Serialize(ref writer, value, formatterResolver);
            break;
        }
        writer.WriteEndObject();
      }
    }

    private static TProcessor Deserialize<TProcessor>(
      ref Elasticsearch.Net.Utf8Json.JsonReader reader,
      IJsonFormatterResolver formatterResolver)
      where TProcessor : IProcessor
    {
      return formatterResolver.GetFormatter<TProcessor>().Deserialize(ref reader, formatterResolver);
    }

    private static void Serialize<TProcessor>(
      ref Elasticsearch.Net.Utf8Json.JsonWriter writer,
      IProcessor value,
      IJsonFormatterResolver formatterResolver)
      where TProcessor : class, IProcessor
    {
      formatterResolver.GetFormatter<TProcessor>().Serialize(ref writer, value as TProcessor, formatterResolver);
    }
  }
}
