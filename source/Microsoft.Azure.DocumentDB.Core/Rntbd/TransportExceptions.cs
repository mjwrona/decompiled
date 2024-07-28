// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.TransportExceptions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal static class TransportExceptions
  {
    internal static GoneException GetGoneException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      GoneException goneException = inner != null ? (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, targetAddress) : new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, targetAddress, RntbdConnection.LocalIpv4Address)) : (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, targetAddress) : new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, targetAddress, RntbdConnection.LocalIpv4Address));
      goneException.Headers.Set("x-ms-activity-id", activityId.ToString());
      return goneException;
    }

    internal static RequestTimeoutException GetRequestTimeoutException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      RequestTimeoutException timeoutException = inner != null ? (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress) : new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress, RntbdConnection.LocalIpv4Address)) : (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress) : new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress, RntbdConnection.LocalIpv4Address));
      timeoutException.Headers.Add("x-ms-request-validation-failure", "1");
      return timeoutException;
    }

    internal static ServiceUnavailableException GetServiceUnavailableException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      ServiceUnavailableException unavailableException = inner != null ? new ServiceUnavailableException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.ChannelClosed), inner, targetAddress) : new ServiceUnavailableException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.ChannelClosed), targetAddress);
      unavailableException.Headers.Add("x-ms-request-validation-failure", "1");
      return unavailableException;
    }

    internal static InternalServerErrorException GetInternalServerErrorException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      InternalServerErrorException serverErrorException = inner != null ? new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.ChannelClosed), inner, targetAddress) : new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.ChannelClosed), targetAddress);
      serverErrorException.Headers.Add("x-ms-request-validation-failure", "1");
      return serverErrorException;
    }

    internal static InternalServerErrorException GetInternalServerErrorException(
      Uri targetAddress,
      string exceptionMessage)
    {
      InternalServerErrorException serverErrorException = new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) exceptionMessage), targetAddress);
      serverErrorException.Headers.Add("x-ms-request-validation-failure", "1");
      return serverErrorException;
    }
  }
}
