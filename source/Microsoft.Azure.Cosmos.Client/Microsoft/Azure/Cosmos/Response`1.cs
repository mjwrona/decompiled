// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Response`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Net;

namespace Microsoft.Azure.Cosmos
{
  public abstract class Response<T>
  {
    public abstract Headers Headers { get; }

    public abstract T Resource { get; }

    public static implicit operator T(Response<T> response) => response.Resource;

    public abstract HttpStatusCode StatusCode { get; }

    public abstract double RequestCharge { get; }

    public abstract string ActivityId { get; }

    public abstract string ETag { get; }

    public abstract CosmosDiagnostics Diagnostics { get; }

    internal virtual RequestMessage RequestMessage { get; }
  }
}
