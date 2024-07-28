// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DataImportControllerBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class DataImportControllerBase : TfsApiController
  {
    public static readonly ReadOnlyDictionary<Type, HttpStatusCode> KnownException = new ReadOnlyDictionary<Type, HttpStatusCode>((IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ActionDeniedBySubscriberException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (BlobsCopiedExceededThresholdException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DataImportClientVersionNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DataImportConfigurationException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (DataImportEntryAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (DataImportEntryDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (DataImportInProgressException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DataImportRegionMismatchException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (EmptyImportSourceException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ImportSourceConnectionTimeoutException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDataImportDacpacException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidDataImportPropertyValueException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidSASKeyExpirationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidImportSourceConnectionStringException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidSourceExtendedPropertyValueException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidAccessException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (MaxDacpacSizeExceededException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MilestoneNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MissingDataImportPropertyException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MissingSourceExtendedPropertyException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PackageLocationNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SourceContainsExportedDataException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SourceIsMissingSnapshotTablesException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SourceIsNotADetachedDatabaseException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SourceIsTFSConfigurationDatabaseException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (SqlPackageVersionNotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnableToExtractDacpacInformationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UserNotInDatabaseRoleException),
        HttpStatusCode.BadRequest
      }
    });
    private const string c_layer = "DataImportController";

    [HttpPut]
    [ClientResponseType(typeof (bool), null, null)]
    public HttpResponseMessage QueueRequest(FrameworkDataImportRequest request, bool validateOnly)
    {
      this.TfsRequestContext.TraceAlways(15080318, TraceLevel.Info, this.TraceArea, "DataImportController", string.Format("{0}:{1}, {2}:{3}, {4}:{5}, {6}:{7}", (object) nameof (validateOnly), (object) validateOnly, (object) "JobPluginName", (object) request?.JobPluginName, (object) "RequestId", (object) request?.RequestId, (object) "ServicingJobId", (object) request?.ServicingJobId));
      try
      {
        bool flag;
        HttpStatusCode statusCode;
        if (validateOnly)
        {
          flag = this.ValidateRequest(request);
          statusCode = HttpStatusCode.OK;
        }
        else
        {
          this.QueueRequest(request);
          flag = true;
          statusCode = HttpStatusCode.Created;
        }
        return this.Request.CreateResponse<bool>(statusCode, flag);
      }
      finally
      {
        if (this.TfsRequestContext.IsCanceled)
          this.TfsRequestContext.TraceAlways(15080303, TraceLevel.Error, this.TraceArea, "DataImportController", string.Format("Queue Import Request was Canceled. Validation Only: {0}", (object) validateOnly));
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (DataImportRequestStatus), null, null)]
    public HttpResponseMessage GetDataImportRequestStatus(Guid requestId)
    {
      DataImportRequestStatus importInfoFromId = this.TfsRequestContext.GetService<IDataImportService>().GetDataImportInfoFromId(this.TfsRequestContext, requestId);
      return importInfoFromId == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<DataImportRequestStatus>(HttpStatusCode.OK, importInfoFromId);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage CancelDataImportJob(Guid requestId)
    {
      this.TfsRequestContext.GetService<IDataImportService>().CancelDataImportJob(this.TfsRequestContext, requestId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    public override string TraceArea => "DataImportOrchestration";

    public override string ActivityLogArea => "Framework";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DataImportControllerBase.KnownException;

    private bool ValidateRequest(FrameworkDataImportRequest request)
    {
      ArgumentUtility.CheckForNull<FrameworkDataImportRequest>(request, nameof (request));
      ArgumentUtility.CheckBoundsInclusive(request.ImportVersion, 1, 4, "ImportVersion");
      return this.TfsRequestContext.GetService<IDataImportService>().ValidateRequest(this.TfsRequestContext, request);
    }

    private void QueueRequest(FrameworkDataImportRequest request)
    {
      ArgumentUtility.CheckForNull<FrameworkDataImportRequest>(request, nameof (request));
      ArgumentUtility.CheckBoundsInclusive(request.ImportVersion, 1, 4, "ImportVersion");
      this.TfsRequestContext.GetService<IDataImportService>().QueueRequest(this.TfsRequestContext, request);
    }
  }
}
