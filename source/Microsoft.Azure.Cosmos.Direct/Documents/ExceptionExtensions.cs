// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ExceptionExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal static class ExceptionExtensions
  {
    public static string ToLoggingString(this Exception exception)
    {
      IEnumerable<string> values = ((IEnumerable<PropertyInfo>) exception.GetType().GetProperties()).Select(property => new
      {
        Name = property.Name,
        Value = property.GetValue((object) exception, (object[]) null)
      }).Select(x => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) x.Name, x.Value != null ? (object) x.Value.ToString() : (object) string.Empty));
      return exception.GetType().ToString() + " : " + string.Join(",", values);
    }

    public static string ToStringWithData(this Exception exception)
    {
      StringBuilder stringBuilder = new StringBuilder(exception.ToString());
      List<string> stringList = new List<string>();
      ExceptionExtensions.CaptureExceptionData(exception, stringList);
      if (stringList.Count<string>() > 0)
      {
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append("AdditionalData:");
        foreach (string str in stringList)
        {
          stringBuilder.Append(Environment.NewLine);
          stringBuilder.Append(str);
        }
      }
      return stringBuilder.ToString();
    }

    public static string ToStringWithMessageAndData(this Exception exception)
    {
      StringBuilder stringBuilder = new StringBuilder(exception.Message);
      List<string> stringList = new List<string>();
      ExceptionExtensions.CaptureExceptionData(exception, stringList);
      if (stringList.Count<string>() > 0)
      {
        stringBuilder.Append(Environment.NewLine);
        stringBuilder.Append("AdditionalData:");
        foreach (string str in stringList)
        {
          stringBuilder.Append(Environment.NewLine);
          stringBuilder.Append(str);
        }
      }
      return stringBuilder.ToString();
    }

    public static DocumentClientException GetTranslatedStoredProcedureException(
      DocumentClientException dce)
    {
      if (dce == null)
        return dce;
      HttpStatusCode? statusCode = dce.StatusCode;
      if (!statusCode.HasValue)
        return dce;
      SubStatusCodes subStatus = dce.GetSubStatus();
      statusCode = dce.StatusCode;
      if (statusCode.Value != HttpStatusCode.BadRequest)
        return dce;
      switch ((HttpStatusCode) subStatus)
      {
        case HttpStatusCode.BadRequest:
          return (DocumentClientException) new BadRequestException(dce.Message);
        case HttpStatusCode.Forbidden:
          return (DocumentClientException) new ForbiddenException(dce.Message);
        case HttpStatusCode.NotFound:
          return (DocumentClientException) new NotFoundException(dce.Message);
        case HttpStatusCode.RequestTimeout:
          return (DocumentClientException) new RequestTimeoutException(dce.Message);
        case HttpStatusCode.Conflict:
          return (DocumentClientException) new ConflictException(dce.Message);
        case HttpStatusCode.Gone:
          return (DocumentClientException) new GoneException(dce.Message, SubStatusCodes.Unknown);
        case HttpStatusCode.PreconditionFailed:
          return (DocumentClientException) new PreconditionFailedException(dce.Message);
        case HttpStatusCode.RequestEntityTooLarge:
          return (DocumentClientException) new RequestEntityTooLargeException(dce.Message);
        case (HttpStatusCode) 449:
          return (DocumentClientException) new RetryWithException(dce.Message);
        case HttpStatusCode.ServiceUnavailable:
          return (DocumentClientException) new ServiceUnavailableException(dce.Message, SubStatusCodes.Unknown);
        case (HttpStatusCode) 1004:
          return (DocumentClientException) new NotFoundException((Exception) dce, SubStatusCodes.CrossPartitionQueryNotServable);
        case (HttpStatusCode) 3001:
          return (DocumentClientException) new InternalServerErrorException(dce.Message);
        case (HttpStatusCode) 3207:
          return (DocumentClientException) new ConflictException(dce.Message, SubStatusCodes.ConfigurationNameAlreadyExists);
        default:
          return dce;
      }
    }

    private static void CaptureExceptionData(Exception exception, List<string> exceptionData)
    {
      if (exception.Data != null && exception.Data.Count > 0)
      {
        foreach (object key in (IEnumerable) exception.Data.Keys)
          exceptionData.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", (object) key.ToString(), (object) exception.Data[key].ToString()));
      }
      if (exception.InnerException == null)
        return;
      ExceptionExtensions.CaptureExceptionData(exception.InnerException, exceptionData);
    }
  }
}
