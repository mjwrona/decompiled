// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.TransportExceptions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal static class TransportExceptions
  {
    internal static GoneException GetGoneException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null,
      TransportRequestStats transportRequestStats = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      GoneException goneException = inner != null ? (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress) : new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress, RntbdConnection.LocalIpv4Address)) : (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress) : new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.Gone), inner, SubStatusCodes.TransportGenerated410, targetAddress, RntbdConnection.LocalIpv4Address));
      goneException.Headers.Set("x-ms-activity-id", activityId.ToString());
      goneException.TransportRequestStats = transportRequestStats;
      return goneException;
    }

    internal static RequestTimeoutException GetRequestTimeoutException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null,
      TransportRequestStats transportRequestStats = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      RequestTimeoutException timeoutException = inner != null ? (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress) : new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress, RntbdConnection.LocalIpv4Address)) : (!RntbdConnection.AddSourceIpAddressInNetworkExceptionMessage ? new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress) : new RequestTimeoutException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ExceptionMessage, (object) RMResources.RequestTimeout), inner, targetAddress, RntbdConnection.LocalIpv4Address));
      timeoutException.Headers.Add("x-ms-request-validation-failure", "1");
      timeoutException.TransportRequestStats = transportRequestStats;
      return timeoutException;
    }

    internal static ServiceUnavailableException GetServiceUnavailableException(
      Uri targetAddress,
      Guid activityId,
      Exception inner = null,
      TransportRequestStats transportRequestStats = null)
    {
      Trace.CorrelationManager.ActivityId = activityId;
      ServiceUnavailableException unavailableException = inner != null ? ServiceUnavailableException.Create(new SubStatusCodes?(SubStatusCodes.Channel_Closed), inner, requestUri: targetAddress) : ServiceUnavailableException.Create(new SubStatusCodes?(SubStatusCodes.Channel_Closed), requestUri: targetAddress);
      unavailableException.Headers.Add("x-ms-request-validation-failure", "1");
      unavailableException.TransportRequestStats = transportRequestStats;
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
