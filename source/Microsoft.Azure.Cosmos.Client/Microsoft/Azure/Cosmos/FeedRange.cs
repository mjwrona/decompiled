// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRange
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos
{
  [Serializable]
  public abstract class FeedRange
  {
    public abstract string ToJsonString();

    public static FeedRange FromJsonString(string toStringValue)
    {
      FeedRangeInternal feedRangeInternal;
      if (!FeedRangeInternal.TryParse(toStringValue, out feedRangeInternal))
        throw new ArgumentException(string.Format(ClientResources.FeedToken_UnknownFormat, (object) toStringValue));
      return (FeedRange) feedRangeInternal;
    }

    public static FeedRange FromPartitionKey(PartitionKey partitionKey) => (FeedRange) new FeedRangePartitionKey(partitionKey);
  }
}
