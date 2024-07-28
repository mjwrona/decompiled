// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.IResourceResponseBase
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Documents.Client
{
  public interface IResourceResponseBase
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

    long DocumentQuota { get; }

    long DocumentUsage { get; }

    long StoredProceduresQuota { get; }

    long StoredProceduresUsage { get; }

    long TriggersQuota { get; }

    long TriggersUsage { get; }

    long UserDefinedFunctionsQuota { get; }

    long UserDefinedFunctionsUsage { get; }

    string ActivityId { get; }

    string SessionToken { get; }

    HttpStatusCode StatusCode { get; }

    string MaxResourceQuota { get; }

    string CurrentResourceQuotaUsage { get; }

    Stream ResponseStream { get; }

    double RequestCharge { get; }

    NameValueCollection ResponseHeaders { get; }

    string ContentLocation { get; }

    long IndexTransformationProgress { get; }

    long LazyIndexingProgress { get; }
  }
}
