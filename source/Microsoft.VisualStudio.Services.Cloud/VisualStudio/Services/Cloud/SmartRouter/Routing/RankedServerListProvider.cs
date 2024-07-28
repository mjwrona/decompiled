// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing.RankedServerListProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Routing
{
  public class RankedServerListProvider : SmartRouterBase, IRankedServerListProvider
  {
    public RankedServerListProvider()
      : base(SmartRouterBase.TraceLayer.Routing)
    {
    }

    public MinHeap<RankedServerNode> GetRankedServerList(
      string affinityKey,
      IReadOnlyList<ServerNodeWithHash> servers)
    {
      servers = servers.CheckArgumentIsNotNull<IReadOnlyList<ServerNodeWithHash>>(nameof (servers));
      affinityKey = affinityKey.CheckArgumentIsNotNullOrEmpty(nameof (affinityKey));
      MinHeap<RankedServerNode> rankedServerList = new MinHeap<RankedServerNode>(servers.Count);
      using (HashAlgorithm hasher = RankedServerListProvider.CreateHasher())
      {
        byte[] inputHash = RankedServerListProvider.HashString(affinityKey, hasher);
        foreach (ServerNodeWithHash server in (IEnumerable<ServerNodeWithHash>) servers)
          rankedServerList.Add(RankedServerListProvider.RankServer(server, inputHash, hasher));
        return rankedServerList;
      }
    }

    private static HashAlgorithm CreateHasher() => (HashAlgorithm) SHA256.Create();

    private static RankedServerNode RankServer(
      ServerNodeWithHash server,
      byte[] inputHash,
      HashAlgorithm hasher)
    {
      byte[] numArray = new byte[8];
      Array.Copy((Array) inputHash, 0, (Array) numArray, 0, 4);
      Array.Copy((Array) server.Hash.ToArray(), 0, (Array) numArray, 4, 4);
      return new RankedServerNode(BitConverter.ToUInt32(hasher.ComputeHash(numArray), 0), server);
    }

    private static byte[] HashString(string? input, HashAlgorithm hasher) => hasher.ComputeHash(input == null ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(input));
  }
}
