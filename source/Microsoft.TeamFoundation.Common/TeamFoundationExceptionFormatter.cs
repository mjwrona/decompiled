// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationExceptionFormatter
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation
{
  public static class TeamFoundationExceptionFormatter
  {
    public static string FormatException(Exception exception, bool useBriefFormat)
    {
      if (exception == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder(TeamFoundationExceptionFormatter.formatOneException(exception, useBriefFormat));
      if (!useBriefFormat)
      {
        for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
        {
          stringBuilder.AppendLine();
          stringBuilder.AppendLine(TFCommonResources.InnerException());
          stringBuilder.Append(TeamFoundationExceptionFormatter.formatOneException(innerException, false));
        }
      }
      return stringBuilder.ToString();
    }

    private static string formatOneException(Exception exception, bool useBriefFormat)
    {
      StringBuilder stringBuilder = new StringBuilder(TFCommonResources.LogExceptionHeader((object) exception.Message, (object) exception.GetType().Name));
      if (!useBriefFormat)
      {
        switch (exception)
        {
          case WebException _:
            WebException webException = exception as WebException;
            if (webException.Response is HttpWebResponse response)
            {
              try
              {
                string header = response.Headers["X-TFS-ServiceError"];
                string str = string.IsNullOrEmpty(header) ? response.StatusDescription : UriUtility.UrlDecode(header);
                stringBuilder.AppendLine(TFCommonResources.WebExceptionReport((object) webException.Status, (object) response.StatusCode, (object) str));
                break;
              }
              catch (ObjectDisposedException ex)
              {
                stringBuilder.AppendLine(TFCommonResources.WebExceptionReport_ResponseDisposed((object) webException.Status));
                break;
              }
            }
            else
              break;
          case SqlException _:
            SqlException exception1 = exception as SqlException;
            stringBuilder.AppendLine(TFCommonResources.SqlExceptionReport((object) exception1.Class, (object) exception1.Number, (object) exception1.Procedure, (object) exception1.LineNumber, (object) exception1.Server, (object) exception1.State, (object) TeamFoundationExceptionFormatter.formatSqlErrors(exception1)));
            break;
          case SoapException _:
            SoapException soapException = exception as SoapException;
            if (soapException.Detail != null)
            {
              stringBuilder.AppendLine(TFCommonResources.SoapExceptionReport((object) soapException.Detail.OuterXml));
              break;
            }
            break;
          default:
            stringBuilder.AppendLine();
            break;
        }
        string str1 = TeamFoundationExceptionFormatter.formatDataDictionary(exception);
        if (!string.IsNullOrEmpty(str1))
          stringBuilder.AppendLine(str1);
        stringBuilder.AppendLine(TFCommonResources.ExceptionStackTrace((object) exception.StackTrace));
      }
      return stringBuilder.ToString();
    }

    internal static string formatSqlErrors(SqlException exception)
    {
      if (exception.Errors.Count == 1)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 1; index < exception.Errors.Count; ++index)
        stringBuilder.AppendLine(TFCommonResources.SqlExceptionError((object) index, (object) exception.Errors[index].ToString(), (object) exception.Errors[index].Class, (object) exception.Errors[index].Number, (object) exception.Errors[index].Server, (object) exception.Errors[index].Source, (object) exception.Errors[index].State, (object) exception.Errors[index].Procedure, (object) exception.Errors[index].LineNumber));
      return stringBuilder.ToString();
    }

    private static string formatDataDictionary(Exception exception)
    {
      if (exception.Data == null || exception.Data.Count <= 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (DictionaryEntry dictionaryEntry in exception.Data)
        stringBuilder.AppendLine(TFCommonResources.ExceptionDataDictionaryReport(dictionaryEntry.Key, dictionaryEntry.Value));
      return TFCommonResources.LogExceptionDataDictionary((object) stringBuilder.ToString());
    }
  }
}
