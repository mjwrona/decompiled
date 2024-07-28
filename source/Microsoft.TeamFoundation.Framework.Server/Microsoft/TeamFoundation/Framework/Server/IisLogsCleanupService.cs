// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IisLogsCleanupService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class IisLogsCleanupService : IVssFrameworkService
  {
    private static readonly string s_area = "IisLogsCleanup";
    private static readonly string s_layer = "BusinessLogic";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(9450, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, nameof (ServiceStart));
      try
      {
        if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
        if (!HostingEnvironment.IsHosted)
          throw new InvalidOperationException("This service can only be used in ASP.net application");
        if (!systemRequestContext.ExecutionEnvironment.IsCloudDeployment)
          throw new InvalidOperationException("This service can only be used in hosted service.");
        TeamFoundationTaskService service = systemRequestContext.GetService<TeamFoundationTaskService>();
        TeamFoundationTask teamFoundationTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.CleanupLogs), (object) null, DateTime.UtcNow.AddSeconds(15.0), 900000);
        IVssRequestContext requestContext = systemRequestContext;
        TeamFoundationTask task = teamFoundationTask;
        service.AddTask(requestContext, task);
      }
      finally
      {
        systemRequestContext.TraceLeave(9450, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void CleanupLogs(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(9460, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, nameof (CleanupLogs));
      try
      {
        string environmentVariable = Environment.GetEnvironmentVariable("APP_POOL_CONFIG");
        string siteName = HostingEnvironment.SiteName;
        requestContext.Trace(9461, TraceLevel.Info, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, "App Pool Config: '{0}', Site Name: '{1}'", (object) environmentVariable, (object) siteName);
        string logFilesDirectory = IisLogsCleanupService.GetLogFilesDirectory(environmentVariable, siteName);
        if (string.IsNullOrEmpty(logFilesDirectory))
        {
          requestContext.Trace(9462, TraceLevel.Error, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, "Could not find log files directory.");
        }
        else
        {
          int num1 = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.RetainIisLogsHours, 4);
          requestContext.Trace(9463, TraceLevel.Info, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, "Log Files Directory: '{0}'.", (object) logFilesDirectory);
          FileInfo[] array = ((IEnumerable<FileSystemInfo>) new DirectoryInfo(logFilesDirectory).GetFileSystemInfos("*.log", SearchOption.TopDirectoryOnly)).Where<FileSystemInfo>((Func<FileSystemInfo, bool>) (fs => fs is FileInfo)).Cast<FileInfo>().OrderBy<FileInfo, DateTime>((Func<FileInfo, DateTime>) (f => f.LastWriteTimeUtc)).ToArray<FileInfo>();
          long num2 = ((IEnumerable<FileInfo>) array).Sum<FileInfo>((Func<FileInfo, long>) (fi => fi.Length));
          requestContext.Trace(9464, TraceLevel.Info, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, "Found {0} log file(s). Total size: {1:#.##} MB.", (object) array.Length, (object) ((double) num2 / 1048576.0));
          DateTime dateTime = DateTime.UtcNow - TimeSpan.FromHours((double) num1);
          for (int index = 0; index < array.Length - 1; ++index)
          {
            FileInfo fileInfo = array[index];
            long length = fileInfo.Length;
            if (!(fileInfo.LastWriteTimeUtc <= dateTime) && num2 < 10737418240L)
              break;
            requestContext.Trace(9465, TraceLevel.Info, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, "Deleting '{0}'.", (object) fileInfo.FullName);
            try
            {
              fileInfo.Delete();
              num2 -= length;
            }
            catch (UnauthorizedAccessException ex)
            {
              requestContext.Trace(9467, TraceLevel.Info, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, ex.Message);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(9466, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, ex);
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9469, IisLogsCleanupService.s_area, IisLogsCleanupService.s_layer, ex);
      }
    }

    private static string GetLogFilesDirectory(string appPoolConfig, string siteName)
    {
      XDocument node = XDocument.Load(appPoolConfig);
      XElement xelement = ((IEnumerable<XElement>) node.XPathSelectElements("/configuration/system.applicationHost/sites/site").ToArray<XElement>()).FirstOrDefault<XElement>((Func<XElement, bool>) (s => s.Attribute((XName) "name").Value.Equals(siteName, StringComparison.Ordinal)));
      string name = xelement.Element((XName) "logFile")?.Attribute((XName) "directory")?.Value ?? node.XPathSelectElement("/configuration/system.applicationHost/sites/siteDefaults/logFile")?.Attribute((XName) "directory")?.Value;
      if (name != null)
        name = Path.Combine(Environment.ExpandEnvironmentVariables(name), "W3SVC" + xelement.Attribute((XName) "id").Value);
      return name;
    }
  }
}
