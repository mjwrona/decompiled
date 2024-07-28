// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.DalWorkItemLinksService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer
{
  public class DalWorkItemLinksService : IDalWorkItemLinksService, IVssFrameworkService
  {
    private const char WatermarkSeparator = ';';
    private const string ContinuationTokenParameterName = "continuationToken";
    internal const string ChangedDateTimeFormat = "yyyyMMddHHmmssfff";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItemLinkChange[] GetWorkItemLinkChanges(
      IVssRequestContext requestContext,
      IDataAccessLayer dal,
      Guid? projectId,
      IEnumerable<string> types,
      IEnumerable<string> linkTypes,
      int batchSize,
      DateTime? startDateTime,
      string continuationToken,
      out string nextContinuationToken,
      out int totalRowCount)
    {
      dal = dal ?? (IDataAccessLayer) new DataAccessLayerImpl(requestContext);
      long rowVersion = -1;
      DateTime? createdDateWatermark = new DateTime?(SqlDateTime.MinValue.Value);
      DateTime? removedDateWatermark = new DateTime?(SqlDateTime.MinValue.Value);
      Dictionary<int, Guid> cache = new Dictionary<int, Guid>();
      if (!string.IsNullOrEmpty(continuationToken))
      {
        if (startDateTime.HasValue)
          throw new ArgumentConflictException(new string[2]
          {
            nameof (continuationToken),
            nameof (startDateTime)
          });
        Tuple<long?, long?> continuationToken1 = this.ParseDateTimeWatermarkFromContinuationToken(continuationToken, nameof (continuationToken));
        if (continuationToken1.Item2.HasValue)
        {
          createdDateWatermark = new DateTime?(new DateTime(continuationToken1.Item1.Value, DateTimeKind.Utc));
          removedDateWatermark = new DateTime?(new DateTime(continuationToken1.Item2.Value, DateTimeKind.Utc));
        }
        else
        {
          Tuple<DateTime, DateTime> timeForTimeStamp = dal.GetLinkDateTimeForTimeStamp(continuationToken1.Item1.Value);
          createdDateWatermark = new DateTime?(timeForTimeStamp.Item1);
          removedDateWatermark = new DateTime?(timeForTimeStamp.Item2);
        }
      }
      else if (startDateTime.HasValue)
      {
        removedDateWatermark = new DateTime?(startDateTime.Value.ToUniversalTime());
        createdDateWatermark = removedDateWatermark;
      }
      WorkItemLinkChange[] workItemLinkChanges = dal.GetWorkItemLinkChanges(requestContext, rowVersion, batchSize, projectId, types, linkTypes, ref createdDateWatermark, ref removedDateWatermark, out long _, out totalRowCount);
      IDataspaceService service = requestContext.GetService<IDataspaceService>();
      foreach (WorkItemLinkChange linkChange in workItemLinkChanges)
      {
        linkChange.SourceProjectId = this.GetProjectIdentifier(requestContext, service, (IDictionary<int, Guid>) cache, linkChange.SourceDataspaceId, linkChange);
        linkChange.TargetProjectId = this.GetProjectIdentifier(requestContext, service, (IDictionary<int, Guid>) cache, linkChange.TargetDataspaceId, linkChange);
      }
      nextContinuationToken = this.BuildContinuationToken(createdDateWatermark.Value, removedDateWatermark.Value);
      return workItemLinkChanges;
    }

    private Guid GetProjectIdentifier(
      IVssRequestContext requestContext,
      IDataspaceService dataspaceService,
      IDictionary<int, Guid> cache,
      int dataspaceId,
      WorkItemLinkChange linkChange)
    {
      Guid projectIdentifier = Guid.Empty;
      if (dataspaceId >= 0)
      {
        if (!cache.TryGetValue(dataspaceId, out projectIdentifier))
        {
          try
          {
            if (dataspaceId == 0 && linkChange.RemoteProjectId.HasValue)
            {
              projectIdentifier = linkChange.RemoteProjectId.Value;
            }
            else
            {
              Dataspace dataspace = dataspaceService.QueryDataspace(requestContext, dataspaceId);
              if (dataspace != null)
                projectIdentifier = cache[dataspaceId] = dataspace.DataspaceIdentifier;
            }
          }
          catch (DataspaceNotFoundException ex)
          {
            projectIdentifier = cache[dataspaceId] = Guid.Empty;
          }
        }
      }
      return projectIdentifier;
    }

    private string BuildContinuationToken(
      DateTime createdDateWaterMark,
      DateTime removedDateWatermark)
    {
      return string.Format("{0}{1}{2}", (object) createdDateWaterMark.Ticks, (object) ';', (object) removedDateWatermark.Ticks);
    }

    private Tuple<long?, long?> ParseDateTimeWatermarkFromContinuationToken(
      string continuationToken,
      string propertyName)
    {
      long result1 = 0;
      long result2 = 0;
      long result3 = 0;
      string[] strArray = !string.IsNullOrEmpty(continuationToken) ? continuationToken.Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentException("cannot parse null or empty continuation token", continuationToken);
      switch (strArray.Length)
      {
        case 1:
          return long.TryParse(strArray[0], out result3) && result3 >= 0L ? new Tuple<long?, long?>(new long?(result3), new long?()) : throw new VssPropertyValidationException(propertyName, DalResourceStrings.Format("QueryParameterOutOfRange", (object) propertyName));
        case 2:
          if (!long.TryParse(strArray[0], out result1) || result1 < 0L || result1 > SqlDateTime.MaxValue.Value.Ticks)
            throw new VssPropertyValidationException(propertyName, DalResourceStrings.Format("QueryParameterOutOfRange", (object) propertyName));
          if (!long.TryParse(strArray[1], out result2) || result2 < 0L || result2 > SqlDateTime.MaxValue.Value.Ticks)
            throw new VssPropertyValidationException(propertyName, DalResourceStrings.Format("QueryParameterOutOfRange", (object) propertyName));
          SqlDateTime minValue;
          DateTime dateTime1;
          if (result1 < SqlDateTime.MinValue.Value.Ticks)
          {
            minValue = SqlDateTime.MinValue;
            dateTime1 = minValue.Value;
            result1 = dateTime1.Ticks;
          }
          long num = result2;
          minValue = SqlDateTime.MinValue;
          dateTime1 = minValue.Value;
          long ticks = dateTime1.Ticks;
          if (num < ticks)
          {
            minValue = SqlDateTime.MinValue;
            dateTime1 = minValue.Value;
            result2 = dateTime1.Ticks;
          }
          return new Tuple<long?, long?>(new long?(result1), new long?(result2));
        case 5:
          long dateTime2 = DalWorkItemLinksService.ParseDateTime(strArray[0]);
          return new Tuple<long?, long?>(new long?(dateTime2), new long?(dateTime2));
        default:
          throw new VssPropertyValidationException(propertyName, DalResourceStrings.Format("QueryParameterOutOfRange", (object) propertyName));
      }
    }

    private static void AssertForContinuationToken(bool assertionValue)
    {
      if (!assertionValue)
        throw new VssPropertyValidationException("continuationToken", DalResourceStrings.Format("QueryParameterOutOfRange", (object) "continuationToken"));
    }

    private static long ParseDateTime(string dateTimeString)
    {
      DateTimeOffset result;
      DalWorkItemLinksService.AssertForContinuationToken(DateTimeOffset.TryParseExact(dateTimeString, "yyyyMMddHHmmssfff", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) && result.Ticks >= 0L && result.Ticks <= SqlDateTime.MaxValue.Value.Ticks);
      TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(result.DateTime);
      return Math.Max(new DateTimeOffset(result.DateTime, utcOffset).Ticks, SqlDateTime.MinValue.Value.Ticks);
    }
  }
}
