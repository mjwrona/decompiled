// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IStoredProcedureResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Specialized;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  internal interface IStoredProcedureResponse<TValue>
  {
    string ActivityId { get; }

    string CurrentResourceQuotaUsage { get; }

    string MaxResourceQuota { get; }

    double RequestCharge { get; }

    TValue Response { get; }

    NameValueCollection ResponseHeaders { get; }

    string SessionToken { get; }

    string ScriptLog { get; }

    HttpStatusCode StatusCode { get; }
  }
}
