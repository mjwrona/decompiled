// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.DocDBErrors.ExceptionClassifier
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Net;

namespace Microsoft.Azure.Cosmos.ChangeFeed.DocDBErrors
{
  internal static class ExceptionClassifier
  {
    public static DocDbError ClassifyStatusCodes(HttpStatusCode statusCode, int subStatusCode) => statusCode == HttpStatusCode.Gone && (subStatusCode == 1002 || subStatusCode == 1007) ? DocDbError.PartitionSplit : DocDbError.Undefined;
  }
}
