// Decompiled with JetBrains decompiler
// Type: Nest.NodeHotThreadsResponseBuilder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class NodeHotThreadsResponseBuilder : CustomResponseBuilderBase
  {
    private static readonly Regex NodeRegex = new Regex("^\\s\\{(?<name>.+?)\\}\\{(?<id>.+?)\\}(?<hosts>.+)\\n");

    public static NodeHotThreadsResponseBuilder Instance { get; } = new NodeHotThreadsResponseBuilder();

    private static NodesHotThreadsResponse Parse(string plainTextResponse)
    {
      if (!plainTextResponse.StartsWith(":::", StringComparison.Ordinal))
        return new NodesHotThreadsResponse();
      return new NodesHotThreadsResponse((IReadOnlyCollection<HotThreadInformation>) ((IEnumerable<string>) plainTextResponse.Split(new string[1]
      {
        ":::"
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (section => section.Split(new string[1]
      {
        "\n   \n"
      }, StringSplitOptions.None))).Where<string[]>((Func<string[], bool>) (sectionLines => sectionLines.Length != 0)).Select(sectionLines => new
      {
        sectionLines = sectionLines,
        nodeLine = ((IEnumerable<string>) sectionLines).FirstOrDefault<string>()
      }).Where(_param1 => _param1.nodeLine != null).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        matches = NodeHotThreadsResponseBuilder.NodeRegex.Match(_param1.nodeLine)
      }).Where(_param1 => _param1.matches.Success).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier1 = _param1,
        node = _param1.matches.Groups["name"].Value
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier2 = _param1,
        nodeId = _param1.\u003C\u003Eh__TransparentIdentifier1.matches.Groups["id"].Value
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier3 = _param1,
        hosts = _param1.\u003C\u003Eh__TransparentIdentifier2.\u003C\u003Eh__TransparentIdentifier1.matches.Groups["hosts"].Value.Split(new char[2]
        {
          '{',
          '}'
        }, StringSplitOptions.RemoveEmptyEntries)
      }).Select(_param1 => new
      {
        \u003C\u003Eh__TransparentIdentifier4 = _param1,
        threads = ((IEnumerable<string>) _param1.\u003C\u003Eh__TransparentIdentifier3.\u003C\u003Eh__TransparentIdentifier2.\u003C\u003Eh__TransparentIdentifier1.\u003C\u003Eh__TransparentIdentifier0.sectionLines).Skip<string>(1).Take<string>(_param1.\u003C\u003Eh__TransparentIdentifier3.\u003C\u003Eh__TransparentIdentifier2.\u003C\u003Eh__TransparentIdentifier1.\u003C\u003Eh__TransparentIdentifier0.sectionLines.Length - 1).ToList<string>()
      }).Select(_param1 => new HotThreadInformation()
      {
        NodeName = _param1.\u003C\u003Eh__TransparentIdentifier4.\u003C\u003Eh__TransparentIdentifier3.\u003C\u003Eh__TransparentIdentifier2.node,
        NodeId = _param1.\u003C\u003Eh__TransparentIdentifier4.\u003C\u003Eh__TransparentIdentifier3.nodeId,
        Threads = (IReadOnlyCollection<string>) _param1.threads,
        Hosts = (IReadOnlyCollection<string>) _param1.\u003C\u003Eh__TransparentIdentifier4.hosts
      }).ToList<HotThreadInformation>());
    }

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      if (!response.Success)
        return (object) new NodesHotThreadsResponse();
      using (stream)
      {
        using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
          return (object) NodeHotThreadsResponseBuilder.Parse(streamReader.ReadToEnd());
      }
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      if (!response.Success)
        return (object) new NodesHotThreadsResponse();
      using (stream)
      {
        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
          return (object) NodeHotThreadsResponseBuilder.Parse(await sr.ReadToEndAsync().ConfigureAwait(false));
      }
    }
  }
}
