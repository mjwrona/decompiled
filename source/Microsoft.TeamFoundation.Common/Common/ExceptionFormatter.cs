// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ExceptionFormatter
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.Win32;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;

namespace Microsoft.TeamFoundation.Common
{
  public static class ExceptionFormatter
  {
    private static bool s_showDetailedExceptions = ExceptionFormatter.ShowDetails();

    public static bool ShowDetailedExceptions => ExceptionFormatter.s_showDetailedExceptions;

    private static bool ShowDetails()
    {
      try
      {
        if (string.Equals(Environment.GetEnvironmentVariable("TfsDetailedErrors"), "1", StringComparison.OrdinalIgnoreCase))
          return true;
        string name = "SOFTWARE\\Microsoft\\TeamFoundationServerSetup";
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
        {
          if (registryKey != null)
          {
            if (registryKey.GetValue("TfsDetailedErrors") is int num)
            {
              if (num == 1)
                return true;
            }
          }
        }
        return false;
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine("Error while trying to determine if error details should be printed. Default to not printing details.");
        Console.Error.WriteLine(ex.ToString());
        return false;
      }
    }

    public static string FormatExceptionForDisplay(Exception ex)
    {
      if (ex == null)
        return string.Empty;
      if (!ExceptionFormatter.ShowDetailedExceptions)
        return ex.Message;
      try
      {
        using (StringWriter stringWriter1 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          string str = ">";
          Exception exception = ex;
          while (true)
          {
            switch (exception)
            {
              case null:
                goto label_33;
              case WebException _:
                WebException webException = (WebException) exception;
                stringWriter1.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Status Code: {0}", (object) webException.Status));
                if (webException.Response is HttpWebResponse response)
                {
                  stringWriter1.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HTTP Response Code: {0}", (object) response.StatusCode));
                  break;
                }
                break;
              case SqlException _:
                SqlException sqlException = (SqlException) exception;
                using (StringWriter stringWriter2 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
                {
                  int num = 1;
                  foreach (SqlError error in sqlException.Errors)
                  {
                    stringWriter2.WriteLine("SQL Error #" + num.ToString());
                    stringWriter2.WriteLine("SQL Message: " + error.Message);
                    stringWriter2.WriteLine("SQL LineNumber: " + error.LineNumber.ToString());
                    stringWriter2.WriteLine("SQL Source: " + error.Source);
                    stringWriter2.WriteLine("SQL Procedure: " + error.Procedure);
                    ++num;
                  }
                  stringWriter1.WriteLine(stringWriter2.ToString());
                  break;
                }
              case Win32Exception _:
                Win32Exception win32Exception = (Win32Exception) exception;
                stringWriter1.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HRESULT Error Code: {0:X8}", (object) win32Exception.ErrorCode));
                stringWriter1.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Native Error Code: {0}", (object) win32Exception.NativeErrorCode));
                break;
            }
            stringWriter1.WriteLine(exception.ToString());
            if (exception.Data != null && exception.Data.Count > 0)
            {
              stringWriter1.WriteLine("Exception Data:");
              foreach (DictionaryEntry dictionaryEntry in exception.Data)
                stringWriter1.WriteLine("Key: {0}, Value: {1}", dictionaryEntry.Key, dictionaryEntry.Value);
            }
            exception = exception.InnerException;
            if (exception != null)
              stringWriter1.WriteLine(str + " Inner Exception:");
            str += ">";
          }
label_33:
          return stringWriter1.ToString();
        }
      }
      catch (Exception ex1)
      {
        return "Error formatting exception: " + ex1.ToString();
      }
    }
  }
}
