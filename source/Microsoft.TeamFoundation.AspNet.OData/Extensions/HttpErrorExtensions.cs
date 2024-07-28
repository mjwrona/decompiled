// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Extensions.HttpErrorExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.Http;

namespace Microsoft.AspNet.OData.Extensions
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HttpErrorExtensions
  {
    public static ODataError CreateODataError(this HttpError httpError)
    {
      if (httpError == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (httpError));
      return new ODataError()
      {
        Message = httpError.GetPropertyValue<string>(HttpErrorKeys.MessageKey),
        ErrorCode = httpError.GetPropertyValue<string>(HttpErrorKeys.ErrorCodeKey),
        InnerError = HttpErrorExtensions.ToODataInnerError(httpError)
      };
    }

    private static ODataInnerError ToODataInnerError(HttpError httpError)
    {
      string propertyValue1 = httpError.GetPropertyValue<string>(HttpErrorKeys.ExceptionMessageKey);
      if (propertyValue1 == null)
      {
        string propertyValue2 = httpError.GetPropertyValue<string>(HttpErrorKeys.MessageDetailKey);
        if (propertyValue2 == null)
        {
          HttpError propertyValue3 = httpError.GetPropertyValue<HttpError>(HttpErrorKeys.ModelStateKey);
          if (propertyValue3 == null)
            return (ODataInnerError) null;
          return new ODataInnerError()
          {
            Message = HttpErrorExtensions.ConvertModelStateErrors(propertyValue3)
          };
        }
        return new ODataInnerError()
        {
          Message = propertyValue2
        };
      }
      ODataInnerError odataInnerError = new ODataInnerError();
      odataInnerError.Message = propertyValue1;
      odataInnerError.TypeName = httpError.GetPropertyValue<string>(HttpErrorKeys.ExceptionTypeKey);
      odataInnerError.StackTrace = httpError.GetPropertyValue<string>(HttpErrorKeys.StackTraceKey);
      HttpError propertyValue4 = httpError.GetPropertyValue<HttpError>(HttpErrorKeys.InnerExceptionKey);
      if (propertyValue4 != null)
        odataInnerError.InnerError = HttpErrorExtensions.ToODataInnerError(propertyValue4);
      return odataInnerError;
    }

    private static string ConvertModelStateErrors(HttpError error)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) error)
      {
        if (keyValuePair.Value != null)
        {
          stringBuilder.Append(keyValuePair.Key);
          stringBuilder.Append(" : ");
          if (keyValuePair.Value is IEnumerable<string> strings)
          {
            foreach (string str in strings)
              stringBuilder.AppendLine(str);
          }
          else
            stringBuilder.AppendLine(keyValuePair.Value.ToString());
        }
      }
      return stringBuilder.ToString();
    }
  }
}
