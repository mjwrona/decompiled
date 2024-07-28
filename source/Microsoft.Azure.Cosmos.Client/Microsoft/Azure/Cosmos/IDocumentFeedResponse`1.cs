// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IDocumentFeedResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Azure.Cosmos
{
  internal interface IDocumentFeedResponse<T>
  {
    long DatabaseQuota { get; }

    long DatabaseUsage { get; }

    long CollectionQuota { get; }

    long CollectionUsage { get; }

    long UserQuota { get; }

    long UserUsage { get; }

    long PermissionQuota { get; }

    long PermissionUsage { get; }

    long CollectionSizeQuota { get; }

    long CollectionSizeUsage { get; }

    long StoredProceduresQuota { get; }

    long StoredProceduresUsage { get; }

    long TriggersQuota { get; }

    long TriggersUsage { get; }

    long UserDefinedFunctionsQuota { get; }

    long UserDefinedFunctionsUsage { get; }

    int Count { get; }

    string MaxResourceQuota { get; }

    string CurrentResourceQuotaUsage { get; }

    double RequestCharge { get; }

    string ActivityId { get; }

    string ResponseContinuation { get; }

    string SessionToken { get; }

    string ContentLocation { get; }

    NameValueCollection ResponseHeaders { get; }

    IEnumerator<T> GetEnumerator();
  }
}
