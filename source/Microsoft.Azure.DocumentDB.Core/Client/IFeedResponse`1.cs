// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.IFeedResponse`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Client
{
  public interface IFeedResponse<T>
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
