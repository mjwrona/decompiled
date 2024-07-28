// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureResponseException
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [Serializable]
  public class AzureResponseException : VssServiceException
  {
    public string CorrelationId { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public AzureResponseException(HttpStatusCode statusCode, string message, string correlationId = null)
      : base(string.Format("{0}: {1}", (object) statusCode, (object) message))
    {
      this.CorrelationId = correlationId;
      this.StatusCode = statusCode;
    }

    public AzureResponseException(string message, string correlationId)
      : base(message)
    {
      this.CorrelationId = correlationId;
    }
  }
}
