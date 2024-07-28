// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Client.InvalidResponseCodeException
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D55F5C7-EE6B-4E5B-8407-D17F3B35057D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Client.dll

using System;
using System.Net;

namespace Microsoft.Azure.Pipelines.PoolProvider.Client
{
  public class InvalidResponseCodeException : Exception
  {
    public InvalidResponseCodeException(HttpStatusCode statusCode)
      : base(string.Format("Received invalid response code from Agent Cloud API: {0} ({1})", (object) statusCode.ToString(), (object) (int) statusCode))
    {
      this.StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; set; }

    public DateTime? RetryAfter { get; set; }
  }
}
