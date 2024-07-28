// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.VersionedAndRidCheckedCompositeToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal readonly struct VersionedAndRidCheckedCompositeToken
  {
    public VersionedAndRidCheckedCompositeToken(
      VersionedAndRidCheckedCompositeToken.Version version,
      CosmosElement continuationToken,
      string rid)
    {
      this.VersionNumber = version;
      this.ContinuationToken = continuationToken ?? throw new ArgumentNullException(nameof (continuationToken));
      this.Rid = rid ?? throw new ArgumentNullException(nameof (rid));
    }

    public VersionedAndRidCheckedCompositeToken.Version VersionNumber { get; }

    public CosmosElement ContinuationToken { get; }

    public string Rid { get; }

    public static TryCatch<VersionedAndRidCheckedCompositeToken> MonadicCreateFromCosmosElement(
      CosmosElement cosmosElement)
    {
      if (cosmosElement == (CosmosElement) null)
        throw new ArgumentNullException(nameof (cosmosElement));
      if (!(cosmosElement is CosmosObject cosmosObject))
        return TryCatch<VersionedAndRidCheckedCompositeToken>.FromException((Exception) new FormatException(string.Format("Expected object for {0}: {1}.", (object) nameof (VersionedAndRidCheckedCompositeToken), (object) cosmosElement)));
      CosmosNumber typedCosmosElement1;
      if (!cosmosObject.TryGetValue<CosmosNumber>("V", out typedCosmosElement1))
        return TryCatch<VersionedAndRidCheckedCompositeToken>.FromException((Exception) new FormatException(string.Format("expected number {0} property for {1}: {2}.", (object) "Version", (object) nameof (VersionedAndRidCheckedCompositeToken), (object) cosmosElement)));
      CosmosElement continuationToken;
      if (!cosmosObject.TryGetValue("Continuation", out continuationToken))
        return TryCatch<VersionedAndRidCheckedCompositeToken>.FromException((Exception) new FormatException(string.Format("expected number {0} property for {1}: {2}.", (object) "Continuation", (object) nameof (VersionedAndRidCheckedCompositeToken), (object) cosmosElement)));
      CosmosString typedCosmosElement2;
      return !cosmosObject.TryGetValue<CosmosString>("Rid", out typedCosmosElement2) ? TryCatch<VersionedAndRidCheckedCompositeToken>.FromException((Exception) new FormatException(string.Format("expected number {0} property for {1}: {2}.", (object) "Version", (object) nameof (VersionedAndRidCheckedCompositeToken), (object) cosmosElement))) : TryCatch<VersionedAndRidCheckedCompositeToken>.FromResult(new VersionedAndRidCheckedCompositeToken((VersionedAndRidCheckedCompositeToken.Version) Number64.ToLong(typedCosmosElement1.Value), continuationToken, UtfAnyString.op_Implicit(typedCosmosElement2.Value)));
    }

    public static CosmosElement ToCosmosElement(VersionedAndRidCheckedCompositeToken token) => (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
    {
      {
        "V",
        (CosmosElement) CosmosNumber64.Create((Number64) (long) token.VersionNumber)
      },
      {
        "Rid",
        (CosmosElement) CosmosString.Create(token.Rid)
      },
      {
        "Continuation",
        token.ContinuationToken
      }
    });

    private static class PropertyNames
    {
      public const string Version = "V";
      public const string Rid = "Rid";
      public const string Continuation = "Continuation";
    }

    public enum Version
    {
      V1 = 0,
      V2 = 2,
    }
  }
}
