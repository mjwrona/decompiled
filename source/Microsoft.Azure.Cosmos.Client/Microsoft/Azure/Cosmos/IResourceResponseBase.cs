// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IResourceResponseBase
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal interface IResourceResponseBase
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
