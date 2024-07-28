// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseLogWriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DatabaseLogWriter : ServicingLogWriter
  {
    private int m_databaseId;

    public DatabaseLogWriter(int databaseId) => this.m_databaseId = databaseId;

    public string Write(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      bool overwriteIfExists)
    {
      ServicingJobDetail jobDetails;
      List<ServicingStepDetail> servicingDetails = requestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(requestContext, hostId, jobId, ServicingStepDetailFilterOptions.AllStepDetails, out jobDetails);
      int databaseId = TeamFoundationServicingService.GetDatabaseId(jobDetails.HostId);
      string collectionLogPath;
      using (IDisposableReadOnlyList<IAdminUtilityExtension> extensions = requestContext.GetExtensions<IAdminUtilityExtension>())
      {
        if (extensions == null || extensions.Count <= 0)
          throw new TeamFoundationExtensionNotFoundException("IAdminUtilityExtension", requestContext.ServiceHost.PlugInDirectory);
        collectionLogPath = extensions[0].GetCollectionLogPath(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DB_{0}_{1}", (object) databaseId, (object) jobDetails.OperationClass), jobDetails.QueueTime.ToLocalTime());
      }
      if (File.Exists(collectionLogPath))
      {
        if (!overwriteIfExists)
          return collectionLogPath;
        File.Delete(collectionLogPath);
      }
      this.m_databaseId = databaseId;
      return this.Write(jobDetails, servicingDetails, collectionLogPath);
    }

    public void Write(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      StreamWriter streamWriter)
    {
      ServicingJobDetail jobDetails;
      List<ServicingStepDetail> servicingDetails = requestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(requestContext, hostId, jobId, ServicingStepDetailFilterOptions.AllStepDetails, out jobDetails);
      this.m_databaseId = TeamFoundationServicingService.GetDatabaseId(jobDetails.HostId);
      this.Write(jobDetails, servicingDetails, streamWriter);
    }

    protected override void WriteHeader(
      string operationClass,
      DateTime queueTime,
      DateTime startTime,
      DateTime endTime,
      ServicingJobResult jobResult,
      IServicingLogEntryHandler handler)
    {
      TimeZoneInfo local = TimeZoneInfo.Local;
      string str = local.IsDaylightSavingTime(startTime.ToLocalTime()) ? local.DaylightName : local.StandardName;
      handler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, "===================================================================="));
      handler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Database Id     : {0}", (object) this.m_databaseId)));
      if (!string.IsNullOrEmpty(operationClass))
        handler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Operation class : {0}", (object) operationClass)));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Queue time      : {0}", (object) queueTime.ToLocalTime()));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Start time      : {0}", (object) startTime.ToLocalTime()));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("End time        : {0}", (object) endTime.ToLocalTime()));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Execution time  : {0:d\\:hh\\:mm\\:ss}", (object) (endTime - startTime)));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Time Zone       : {0}", (object) str));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Job result      : {0}", (object) jobResult));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("===================================================================="));
    }
  }
}
