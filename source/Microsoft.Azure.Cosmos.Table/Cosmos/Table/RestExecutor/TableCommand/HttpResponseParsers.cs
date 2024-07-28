// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.HttpResponseParsers
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class HttpResponseParsers
  {
    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode expectedStatusCode,
      HttpResponseMessage resp,
      T retVal,
      RESTCommand<T> cmd,
      Exception ex)
    {
      return HttpResponseParsers.ProcessExpectedStatusCodeNoException<T>(expectedStatusCode, resp != null ? resp.StatusCode : HttpStatusCode.Unused, retVal, cmd, ex);
    }

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode[] expectedStatusCodes,
      HttpResponseMessage resp,
      T retVal,
      RESTCommand<T> cmd,
      Exception ex)
    {
      return HttpResponseParsers.ProcessExpectedStatusCodeNoException<T>(expectedStatusCodes, resp != null ? resp.StatusCode : HttpStatusCode.Unused, retVal, cmd, ex);
    }

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode expectedStatusCode,
      HttpStatusCode actualStatusCode,
      T retVal,
      RESTCommand<T> cmd,
      Exception ex)
    {
      if (ex != null)
        throw ex;
      if (actualStatusCode != expectedStatusCode)
        throw new StorageException(cmd.CurrentResult, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected response code, Expected:{0}, Received:{1}", (object) expectedStatusCode, (object) actualStatusCode), (Exception) null);
      return retVal;
    }

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode[] expectedStatusCodes,
      HttpStatusCode actualStatusCode,
      T retVal,
      RESTCommand<T> cmd,
      Exception ex)
    {
      if (ex != null)
        throw ex;
      if (!((IEnumerable<HttpStatusCode>) expectedStatusCodes).Contains<HttpStatusCode>(actualStatusCode))
      {
        string str = string.Join<HttpStatusCode>(",", (IEnumerable<HttpStatusCode>) expectedStatusCodes);
        throw new StorageException(cmd.CurrentResult, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected response code, Expected:{0}, Received:{1}", (object) str, (object) actualStatusCode.ToString()), (Exception) null);
      }
      return retVal;
    }
  }
}
