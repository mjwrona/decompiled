// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.Client.MicrosoftGraphClientException
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MSGraph.Client
{
  [Serializable]
  public class MicrosoftGraphClientException : VssServiceException
  {
    public MicrosoftGraphClientException()
    {
    }

    public MicrosoftGraphClientException(string message)
      : base(message)
    {
    }

    public MicrosoftGraphClientException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected MicrosoftGraphClientException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public MicrosoftGraphClientException(
      HttpStatusCode statusCode,
      ErrorResult errorResult,
      Exception innerException = null,
      RetryConditionHeaderValue retryAfter = null)
      : base(MicrosoftGraphClientException.GetMessage(statusCode, errorResult, retryAfter), innerException)
    {
      this.ErrorResult = errorResult;
      this.StatusCode = statusCode;
      this.RetryAfter = retryAfter;
    }

    public MicrosoftGraphClientException(
      HttpStatusCode statusCode,
      string errorMessage,
      Exception innerException = null,
      RetryConditionHeaderValue retryAfter = null)
      : base(MicrosoftGraphClientException.GetMessage(statusCode, errorMessage, retryAfter), innerException)
    {
      this.StatusCode = statusCode;
      this.RetryAfter = retryAfter;
    }

    public HttpStatusCode StatusCode { get; }

    public ErrorResult ErrorResult { get; }

    public RetryConditionHeaderValue RetryAfter { get; }

    private static string GetMessage(
      HttpStatusCode statusCode,
      ErrorResult errorResult,
      RetryConditionHeaderValue retryAfter)
    {
      return MicrosoftGraphClientException.GetMessage(statusCode, MicrosoftGraphClientException.GetMessage(errorResult), retryAfter);
    }

    private static string GetMessage(
      HttpStatusCode statusCode,
      string errorMessage,
      RetryConditionHeaderValue retryAfter)
    {
      return string.Format("{0} {1} {2} {3}:{4}", (object) (int) statusCode, (object) statusCode, (object) errorMessage, (object) nameof (retryAfter), (object) MicrosoftGraphClientException.GetMessage(retryAfter));
    }

    private static string GetMessage(ErrorResult errorResult)
    {
      if (errorResult == null)
        return "<null>";
      try
      {
        return JsonConvert.SerializeObject((object) errorResult);
      }
      catch (Exception ex)
      {
        return errorResult.Error?.Message;
      }
    }

    private static string GetMessage(RetryConditionHeaderValue retryAfter)
    {
      if (retryAfter == null)
        return "<null>";
      try
      {
        return JsonConvert.SerializeObject((object) retryAfter);
      }
      catch (Exception ex)
      {
        return string.Format("{{\"date\":\"{0}\",\"delta\":\"{1}\"}}", (object) retryAfter.Date, (object) retryAfter.Delta);
      }
    }
  }
}
