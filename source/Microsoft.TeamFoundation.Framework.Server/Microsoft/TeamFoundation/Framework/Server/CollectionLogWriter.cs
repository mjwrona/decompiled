// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionLogWriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CollectionLogWriter : ServicingLogWriter
  {
    private string m_collectionName;

    public CollectionLogWriter()
    {
    }

    public CollectionLogWriter(string collectionName) => this.m_collectionName = collectionName;

    public static string CollectionLogBaseFileName(
      TeamProjectCollectionProperties collectionProperties,
      ServicingJobDetail servicingJobDetail)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TPC_{0}_{1}", (object) collectionProperties.Name, (object) servicingJobDetail.OperationClass);
    }

    public static DateTime CollectionLogTimeStamp(ServicingJobDetail servicingJobDetail) => servicingJobDetail.QueueTime.ToLocalTime();

    public string Write(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collectionProperties,
      ServicingJobDetail servicingJobDetail,
      bool overwriteIfExists)
    {
      TeamFoundationServicingService service = requestContext.GetService<TeamFoundationServicingService>();
      string collectionLogPath;
      using (IDisposableReadOnlyList<IAdminUtilityExtension> extensions = requestContext.GetExtensions<IAdminUtilityExtension>())
      {
        if (extensions == null || extensions.Count <= 0)
          throw new TeamFoundationExtensionNotFoundException("IAdminUtilityExtension", requestContext.ServiceHost.PlugInDirectory);
        collectionLogPath = extensions[0].GetCollectionLogPath(CollectionLogWriter.CollectionLogBaseFileName(collectionProperties, servicingJobDetail), CollectionLogWriter.CollectionLogTimeStamp(servicingJobDetail));
      }
      if (File.Exists(collectionLogPath))
      {
        if (!overwriteIfExists)
          return collectionLogPath;
        File.Delete(collectionLogPath);
      }
      List<ServicingStepDetail> servicingDetails = service.GetServicingDetails(requestContext, servicingJobDetail.HostId, servicingJobDetail.JobId, ServicingStepDetailFilterOptions.AllStepDetails, out ServicingJobDetail _);
      this.m_collectionName = collectionProperties.Name;
      return this.Write(servicingJobDetail, servicingDetails, collectionLogPath);
    }

    public void Write(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collectionProperties,
      ServicingJobDetail servicingJobDetail,
      StreamWriter streamWriter)
    {
      List<ServicingStepDetail> servicingDetails = requestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(requestContext, servicingJobDetail.HostId, servicingJobDetail.JobId, ServicingStepDetailFilterOptions.AllStepDetails, out ServicingJobDetail _);
      this.m_collectionName = collectionProperties.Name;
      this.Write(servicingJobDetail, servicingDetails, streamWriter);
    }

    public void Write(
      IVssRequestContext requestContext,
      TeamProjectCollectionProperties collectionProperties,
      ServicingJobDetail servicingJobDetail,
      IServicingLogEntryHandler handler)
    {
      List<ServicingStepDetail> servicingDetails = requestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(requestContext, servicingJobDetail.HostId, servicingJobDetail.JobId, ServicingStepDetailFilterOptions.AllStepDetails, out ServicingJobDetail _);
      this.m_collectionName = collectionProperties.Name;
      this.Write(servicingJobDetail, servicingDetails, handler);
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
      handler.HandleEntry(ServicingLogWriter.StringToInfo("===================================================================="));
      if (!string.IsNullOrEmpty(this.m_collectionName))
        handler.HandleEntry(ServicingLogWriter.FormatInfo("Collection name : {0}", (object) this.m_collectionName));
      if (!string.IsNullOrEmpty(operationClass))
        handler.HandleEntry(ServicingLogWriter.FormatInfo("Operation class : {0}", (object) operationClass));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Time Zone       : {0}", (object) str));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Queue time      : {0}", (object) queueTime.ToLocalTime()));
      handler.HandleEntry(ServicingLogWriter.FormatInfo("Start time      : {0}", (object) startTime.ToLocalTime()));
      if (jobResult != ServicingJobResult.None)
      {
        handler.HandleEntry(ServicingLogWriter.FormatInfo("End time        : {0}", (object) endTime.ToLocalTime()));
        handler.HandleEntry(ServicingLogWriter.FormatInfo("Execution time  : {0:d\\:hh\\:mm\\:ss}", (object) (endTime - startTime)));
        handler.HandleEntry(ServicingLogWriter.FormatInfo("Job result      : {0}", (object) jobResult));
      }
      handler.HandleEntry(ServicingLogWriter.StringToInfo("===================================================================="));
    }
  }
}
