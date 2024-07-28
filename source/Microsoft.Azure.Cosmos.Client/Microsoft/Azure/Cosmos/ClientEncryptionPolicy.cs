// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ClientEncryptionPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos
{
  public sealed class ClientEncryptionPolicy
  {
    public ClientEncryptionPolicy(
      IEnumerable<ClientEncryptionIncludedPath> includedPaths)
      : this(includedPaths, 1)
    {
    }

    public ClientEncryptionPolicy(
      IEnumerable<ClientEncryptionIncludedPath> includedPaths,
      int policyFormatVersion)
    {
      this.PolicyFormatVersion = policyFormatVersion <= 2 && policyFormatVersion >= 1 ? policyFormatVersion : throw new ArgumentException("Supported versions of client encryption policy are 1 and 2. ");
      ClientEncryptionPolicy.ValidateIncludedPaths(includedPaths, policyFormatVersion);
      this.IncludedPaths = includedPaths;
    }

    [JsonConstructor]
    private ClientEncryptionPolicy()
    {
    }

    [JsonProperty(PropertyName = "includedPaths")]
    public IEnumerable<ClientEncryptionIncludedPath> IncludedPaths { get; private set; }

    [JsonProperty(PropertyName = "policyFormatVersion")]
    public int PolicyFormatVersion { get; private set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    internal void ValidatePartitionKeyPathsIfEncrypted(
      IReadOnlyList<IReadOnlyList<string>> partitionKeyPathTokens)
    {
      foreach (IReadOnlyList<string> partitionKeyPathToken in (IEnumerable<IReadOnlyList<string>>) partitionKeyPathTokens)
      {
        if (partitionKeyPathToken.Count > 0)
        {
          string topLevelToken = partitionKeyPathToken.First<string>();
          IEnumerable<ClientEncryptionIncludedPath> source = this.IncludedPaths.Where<ClientEncryptionIncludedPath>((Func<ClientEncryptionIncludedPath, bool>) (p => p.Path.Substring(1).Equals(topLevelToken)));
          if (source.Any<ClientEncryptionIncludedPath>())
          {
            if (this.PolicyFormatVersion < 2)
              throw new ArgumentException(string.Format("Path: /{0} which is part of the partition key cannot be encrypted with PolicyFormatVersion: {1}. Please use PolicyFormatVersion: 2. ", (object) topLevelToken, (object) this.PolicyFormatVersion));
            if (source.Select<ClientEncryptionIncludedPath, string>((Func<ClientEncryptionIncludedPath, string>) (et => et.EncryptionType)).FirstOrDefault<string>() != "Deterministic")
              throw new ArgumentException("Path: /" + topLevelToken + " which is part of the partition key has to be encrypted with Deterministic type Encryption.");
          }
        }
      }
    }

    private static void ValidateIncludedPaths(
      IEnumerable<ClientEncryptionIncludedPath> clientEncryptionIncludedPath,
      int policyFormatVersion)
    {
      List<string> stringList = new List<string>();
      foreach (ClientEncryptionIncludedPath clientEncryptionIncludedPath1 in clientEncryptionIncludedPath)
      {
        ClientEncryptionPolicy.ValidateClientEncryptionIncludedPath(clientEncryptionIncludedPath1, policyFormatVersion);
        if (stringList.Contains(clientEncryptionIncludedPath1.Path))
          throw new ArgumentException("Duplicate Path found: " + clientEncryptionIncludedPath1.Path + ".");
        stringList.Add(clientEncryptionIncludedPath1.Path);
      }
    }

    private static void ValidateClientEncryptionIncludedPath(
      ClientEncryptionIncludedPath clientEncryptionIncludedPath,
      int policyFormatVersion)
    {
      if (clientEncryptionIncludedPath == null)
        throw new ArgumentNullException(nameof (clientEncryptionIncludedPath));
      if (string.IsNullOrWhiteSpace(clientEncryptionIncludedPath.Path))
        throw new ArgumentNullException("Path");
      if (clientEncryptionIncludedPath.Path[0] != '/' || clientEncryptionIncludedPath.Path.LastIndexOf('/') != 0)
        throw new ArgumentException("Invalid path '" + (clientEncryptionIncludedPath.Path ?? string.Empty) + "'.");
      if (string.IsNullOrWhiteSpace(clientEncryptionIncludedPath.ClientEncryptionKeyId))
        throw new ArgumentNullException("ClientEncryptionKeyId");
      if (string.IsNullOrWhiteSpace(clientEncryptionIncludedPath.EncryptionType))
        throw new ArgumentNullException("EncryptionType");
      if (string.Equals(clientEncryptionIncludedPath.Path.Substring(1), "id"))
      {
        if (policyFormatVersion < 2)
          throw new ArgumentException(string.Format("Path: {0} cannot be encrypted with PolicyFormatVersion: {1}. Please use PolicyFormatVersion: 2. ", (object) clientEncryptionIncludedPath.Path, (object) policyFormatVersion));
        if (clientEncryptionIncludedPath.EncryptionType != "Deterministic")
          throw new ArgumentException("Only Deterministic encryption type is supported for path: " + clientEncryptionIncludedPath.Path + ". ");
      }
      if (!string.Equals(clientEncryptionIncludedPath.EncryptionType, "Deterministic") && !string.Equals(clientEncryptionIncludedPath.EncryptionType, "Randomized"))
        throw new ArgumentException("EncryptionType should be either 'Deterministic' or 'Randomized'. ", nameof (clientEncryptionIncludedPath));
      if (string.IsNullOrWhiteSpace(clientEncryptionIncludedPath.EncryptionAlgorithm))
        throw new ArgumentNullException("EncryptionAlgorithm");
      if (!string.Equals(clientEncryptionIncludedPath.EncryptionAlgorithm, "AEAD_AES_256_CBC_HMAC_SHA256"))
        throw new ArgumentException("EncryptionAlgorithm should be 'AEAD_AES_256_CBC_HMAC_SHA256'. ", nameof (clientEncryptionIncludedPath));
    }
  }
}
