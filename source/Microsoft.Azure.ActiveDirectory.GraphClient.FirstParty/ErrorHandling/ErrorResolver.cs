// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.ErrorHandling.ErrorResolver
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace Microsoft.Azure.ActiveDirectory.GraphClient.ErrorHandling
{
  public class ErrorResolver
  {
    public static GraphException ParseWebException(WebException webException)
    {
      GraphException webException1 = (GraphException) null;
      HttpStatusCode statusCode = HttpStatusCode.Unused;
      string empty = string.Empty;
      if (webException == null)
        throw new ArgumentNullException(nameof (webException));
      if (webException.Response != null)
      {
        statusCode = ((HttpWebResponse) webException.Response).StatusCode;
        empty = webException.Response.ResponseUri.ToString();
        Stream responseStream = webException.Response.GetResponseStream();
        if (responseStream != null)
        {
          string end;
          using (StreamReader streamReader = new StreamReader(responseStream))
            end = streamReader.ReadToEnd();
          webException1 = ErrorResolver.ParseErrorMessageString(statusCode, end);
          if (webException.Response.Headers != null)
          {
            Utils.LogResponseHeaders(webException.Response.Headers);
            webException1.ResponseHeaders = webException.Response.Headers;
          }
          webException.Response.Close();
        }
      }
      if (webException1 == null)
        webException1 = new GraphException(statusCode, webException.Message);
      webException1.ResponseUri = empty;
      return webException1;
    }

    public static GraphException ParseErrorMessageString(
      HttpStatusCode statusCode,
      string errorMessage)
    {
      try
      {
        ODataError oDataError = JsonConvert.DeserializeObject<ODataError>(errorMessage);
        if (oDataError != null)
        {
          if (oDataError.Error != null)
            return ErrorResolver.ResolveErrorCode(statusCode, oDataError);
        }
      }
      catch (JsonReaderException ex)
      {
        Logger.Instance.Error("Invalid JSON response: {0}. {1}", (object) ex.Message, (object) errorMessage);
      }
      return new GraphException(statusCode, "Unknown", errorMessage);
    }

    public static GraphException ResolveErrorCode(HttpStatusCode statusCode, ODataError oDataError)
    {
      GraphException graphException = (GraphException) null;
      string str = string.Empty;
      string message = string.Empty;
      List<ExtendedErrorValue> extendedErrorValueList = (List<ExtendedErrorValue>) null;
      if (oDataError != null && oDataError.Error != null)
      {
        str = oDataError.Error.Code;
        if (oDataError.Error.Message != null)
          message = oDataError.Error.Message.MessageValue;
        extendedErrorValueList = oDataError.Error.Values;
      }
      if (ErrorCodes.ExceptionErrorCodeMap.ContainsKey(str))
      {
        ConstructorInfo constructor = ErrorCodes.ExceptionErrorCodeMap[str].GetConstructor(new Type[2]
        {
          typeof (HttpStatusCode),
          typeof (string)
        });
        if (constructor != (ConstructorInfo) null)
        {
          if (!(constructor.Invoke(new object[2]
          {
            (object) statusCode,
            (object) message
          }) is GraphException graphException1))
            graphException1 = new GraphException(statusCode, str, message);
          graphException = graphException1;
          graphException.Code = str;
          graphException.HttpStatusCode = statusCode;
          graphException.ErrorMessage = message;
        }
      }
      if (graphException == null)
        graphException = new GraphException(statusCode, str, message);
      if (extendedErrorValueList != null)
      {
        graphException.ExtendedErrors = new Dictionary<string, string>();
        extendedErrorValueList.ForEach((Action<ExtendedErrorValue>) (x => graphException.ExtendedErrors[x.Item] = x.Value));
      }
      graphException.ErrorResponse = oDataError;
      return graphException;
    }
  }
}
