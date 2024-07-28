// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.JobsController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Servicing", ResourceName = "Jobs")]
  public class JobsController : BaseServicingJobsController
  {
    [HttpGet]
    public ServicingJobInfo Get(Guid jobId)
    {
      this.CheckDiagnosticPermission(this.TfsRequestContext);
      this.CheckHostType();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      IVssRequestContext requestContext = tfsRequestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationServicingService service = tfsRequestContext.GetService<TeamFoundationServicingService>();
      return tfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? service.GetServicingJobsInfo(requestContext, jobId).FirstOrDefault<ServicingJobInfo>() : service.GetServicingJobInfo(requestContext, tfsRequestContext.ServiceHost.InstanceId, jobId);
    }

    [HttpGet]
    public HttpResponseMessage Get(DateTime startTime)
    {
      this.CheckDiagnosticPermission(this.TfsRequestContext);
      this.CheckHostType();
      DateTime maxValue = DateTime.MaxValue;
      string operationClass = (string) null;
      ServicingJobResult? jobResult = new ServicingJobResult?();
      ServicingJobStatus? jobStatus = new ServicingJobStatus?();
      string databaseName = (string) null;
      int? databaseId = new int?();
      Guid? accountId = new Guid?();
      string poolName = (string) null;
      int? top = new int?();
      bool result1 = false;
      bool result2 = false;
      List<KeyValuePair<ServicingJobInfoColumn, SortOrder>> sortOrder = (List<KeyValuePair<ServicingJobInfoColumn, SortOrder>>) null;
      foreach (KeyValuePair<string, string> queryNameValuePair in this.Request.GetQueryNameValuePairs())
      {
        ServicingJobInfoColumn result3;
        if (System.Enum.TryParse<ServicingJobInfoColumn>(queryNameValuePair.Key, true, out result3))
        {
          switch (result3)
          {
            case ServicingJobInfoColumn.OperationClass:
              operationClass = queryNameValuePair.Value;
              continue;
            case ServicingJobInfoColumn.DatabaseId:
              int result4;
              if (int.TryParse(queryNameValuePair.Value, out result4))
              {
                databaseId = new int?(result4);
                continue;
              }
              continue;
            case ServicingJobInfoColumn.DatabaseName:
              databaseName = queryNameValuePair.Value;
              continue;
            case ServicingJobInfoColumn.PoolName:
              poolName = queryNameValuePair.Value;
              continue;
            case ServicingJobInfoColumn.AccountId:
              Guid result5;
              if (Guid.TryParse(queryNameValuePair.Value, out result5))
              {
                accountId = new Guid?(result5);
                continue;
              }
              continue;
            case ServicingJobInfoColumn.EndTime:
              maxValue = DateTime.Parse(queryNameValuePair.Value);
              continue;
            case ServicingJobInfoColumn.JobStatus:
              ServicingJobStatus result6;
              if (!System.Enum.TryParse<ServicingJobStatus>(queryNameValuePair.Value, true, out result6))
                throw new ArgumentException(HostingResources.InvalidFieldValue((object) "JobStatus", (object) queryNameValuePair.Value));
              jobStatus = new ServicingJobStatus?(result6);
              continue;
            case ServicingJobInfoColumn.JobResult:
              ServicingJobResult result7;
              if (!System.Enum.TryParse<ServicingJobResult>(queryNameValuePair.Value, true, out result7))
                throw new ArgumentException(HostingResources.InvalidFieldValue((object) "JobResult", (object) queryNameValuePair.Value));
              jobResult = new ServicingJobResult?(result7);
              continue;
            default:
              continue;
          }
        }
        else if (string.Equals("Top", queryNameValuePair.Key, StringComparison.OrdinalIgnoreCase))
        {
          int result8;
          if (int.TryParse(queryNameValuePair.Value, out result8))
            top = new int?(result8);
        }
        else if (string.Equals("SortOrder", queryNameValuePair.Key, StringComparison.OrdinalIgnoreCase))
        {
          sortOrder = new List<KeyValuePair<ServicingJobInfoColumn, SortOrder>>();
          this.ParseSortOrder(sortOrder, queryNameValuePair.Value);
          if (sortOrder.Count == 0)
            sortOrder.Add(new KeyValuePair<ServicingJobInfoColumn, SortOrder>(ServicingJobInfoColumn.QueueTime, SortOrder.Descending));
        }
        else if (string.Equals("WrapArray", queryNameValuePair.Key, StringComparison.OrdinalIgnoreCase))
        {
          bool.TryParse(queryNameValuePair.Value, out result1);
        }
        else
        {
          if (!string.Equals("WrapVssJsonCollection", queryNameValuePair.Key, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException(HostingResources.InvalidQueryParameter((object) queryNameValuePair.Key));
          bool.TryParse(queryNameValuePair.Value, out result2);
        }
      }
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<ServicingJobInfo> source = tfsRequestContext.GetService<TeamFoundationServicingService>().QueryServicingJobsInfo(tfsRequestContext, startTime, maxValue, operationClass, jobResult, jobStatus, databaseName, databaseId, accountId, poolName, top, (IList<KeyValuePair<ServicingJobInfoColumn, SortOrder>>) sortOrder);
      if (result1)
        return this.Request.CreateResponse<JsonArrayWrapper>(HttpStatusCode.OK, new JsonArrayWrapper()
        {
          __wrappedArray = (IEnumerable) source
        });
      return result2 ? this.Request.CreateResponse<VssJsonCollectionWrapper>(HttpStatusCode.OK, new VssJsonCollectionWrapper((IEnumerable) source)) : this.Request.CreateResponse<List<ServicingJobInfo>>(HttpStatusCode.OK, source);
    }

    private void ParseSortOrder(
      List<KeyValuePair<ServicingJobInfoColumn, SortOrder>> sortOrder,
      string sortClause)
    {
      string str1 = sortClause;
      char[] chArray1 = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ ' ' };
        string[] strArray = str2.Split(chArray2);
        if (strArray.Length < 1 || strArray.Length > 2)
          throw new ArgumentException(HostingResources.InvalidSortColumnExpression((object) sortClause));
        SortOrder sortOrder1 = SortOrder.Ascending;
        ServicingJobInfoColumn result = ServicingJobInfoColumn.None;
        if (!System.Enum.TryParse<ServicingJobInfoColumn>(strArray[0], true, out result))
          throw new ArgumentException(HostingResources.InvalidSortColumn((object) strArray[0]));
        if (strArray.Length == 2 && !strArray[1].Equals("asc", StringComparison.OrdinalIgnoreCase) && strArray[1].Trim().Equals("desc", StringComparison.OrdinalIgnoreCase))
          sortOrder1 = SortOrder.Descending;
        sortOrder.Add(new KeyValuePair<ServicingJobInfoColumn, SortOrder>(result, sortOrder1));
      }
    }
  }
}
