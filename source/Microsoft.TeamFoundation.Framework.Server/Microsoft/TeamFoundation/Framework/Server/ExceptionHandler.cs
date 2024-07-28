// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExceptionHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ExceptionHandler : IVssExceptionHandler
  {
    private static readonly HashSet<Type> s_fatalExceptionTypes = new HashSet<Type>((IEnumerable<Type>) new Type[1]
    {
      typeof (OutOfMemoryException)
    });
    private static readonly HashSet<Type> s_exceptionTypesNotReported = new HashSet<Type>((IEnumerable<Type>) new Type[9]
    {
      typeof (HttpException),
      typeof (CannotUnloadAppDomainException),
      typeof (ThreadAbortException),
      typeof (HttpRequestValidationException),
      typeof (HttpCompileException),
      typeof (TimeoutException),
      typeof (TaskCanceledException),
      typeof (HttpAntiForgeryException),
      typeof (SqlException)
    });
    private bool m_isHosted;
    private const string s_Area = "ExceptionHandling";
    private const string s_Layer = "ExceptionHandlingUtility";

    internal ExceptionHandler(bool isHosted = false) => this.m_isHosted = isHosted;

    public void HandleException(Exception exception)
    {
      try
      {
        bool flag = true;
        if (exception is AggregateException aggregateException1)
        {
          AggregateException aggregateException = aggregateException1.Flatten();
          int count = aggregateException.InnerExceptions.Count;
          if (count > 0)
          {
            int num = 0;
            foreach (Exception innerException in aggregateException.InnerExceptions)
            {
              if (ExceptionHandler.IsExceptionFiltered(innerException))
                ++num;
            }
            if (count == num)
              flag = false;
          }
        }
        else
          flag = !ExceptionHandler.IsExceptionFiltered(exception);
        this.InitiateShutdownIfNecessary(exception);
        if (!flag)
          return;
        this.ReportException("tfs", "General", exception, (string[]) null);
      }
      catch
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, "ExceptionHandling", "ExceptionHandlingUtility", "{0} Exception: {1}", (object) FrameworkResources.MessageBrief(), (object) exception);
      }
    }

    private static bool IsExceptionFiltered(Exception exception)
    {
      if (exception is VssException vssException)
        return !vssException.ReportException;
      if (ExceptionHandler.s_exceptionTypesNotReported.Contains(exception.GetType()))
        return true;
      return exception is InvalidOperationException && VssStringComparer.ExceptionSource.Equals("System.Web.Services", exception.Source);
    }

    public virtual void ReportException(
      string watsonReportingName,
      string eventCategory,
      Exception exception,
      string[] additionalInfo)
    {
      if (this.m_isHosted || !WatsonReport.VerifyConfiguration())
        return;
      TeamFoundationTracingService.TraceRaw(59003, TraceLevel.Error, "ExceptionHandling", "ExceptionHandlingUtility", "Filing a WER application report for exception: {0}", (object) exception);
      WatsonReport report = WatsonReport.CreateReport(WatsonReport.WatsonReportType.ServerDefault, eventCategory, exception);
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Server Version = {0}", (object) Assembly.GetCallingAssembly().FullName));
      stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Service Account = {0}", (object) UserNameUtil.CurrentUserName));
      stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Machine Name = {0}", (object) Environment.MachineName));
      if (HttpContext.Current != null)
      {
        HttpContext current = HttpContext.Current;
        HttpRequest request = current.Request;
        stringBuilder1.AppendLine("Request Details");
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Url={0} [method = {1}]", (object) request.Url.ToString(), (object) request.RequestType));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User Agent={0}", (object) request.UserAgent));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Headers={0}", (object) request.Headers));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Path={0}", (object) request.Path));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Local Request={0}", (object) request.IsLocal));
        stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User Host Address={0}", (object) request.UserHostAddress));
        if (current.User != null && current.User.Identity.IsAuthenticated)
          stringBuilder1.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User={0} [auth = {1}]", (object) current.User.Identity.Name, (object) current.User.Identity.AuthenticationType));
      }
      report.ApplicationInformation = stringBuilder1.ToString();
      report.ApplicationName = watsonReportingName;
      if (additionalInfo != null && additionalInfo.Length != 0)
      {
        StringBuilder stringBuilder2 = new StringBuilder();
        foreach (string str in additionalInfo)
        {
          if (!string.IsNullOrEmpty(str))
            stringBuilder2.AppendLine(str);
        }
        if (stringBuilder2.Length > 0)
        {
          string tempFileName = FileSpec.GetTempFileName();
          using (StreamWriter streamWriter = new StreamWriter(tempFileName, false, Encoding.Unicode))
            streamWriter.Write(stringBuilder2.ToString());
          report.ReportDataFiles.Add(tempFileName);
        }
      }
      report.FileReport();
    }

    private void InitiateShutdownIfNecessary(Exception ex)
    {
      if (!ExceptionHandler.s_fatalExceptionTypes.Contains(ex.GetType()))
        return;
      TeamFoundationEventLog.Default.LogException(FrameworkResources.ApplicationShutdownOnException(), ex);
      if (!HostingEnvironment.IsHosted)
        return;
      HostingEnvironment.InitiateShutdown();
    }
  }
}
