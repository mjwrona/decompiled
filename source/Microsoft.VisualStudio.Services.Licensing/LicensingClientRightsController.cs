// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingClientRightsController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.UserLicensing.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingClientRightsController : LicensingApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (UnauthorizedRequestException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (InvalidRightNameException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidClientVersionException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidClientRightsQueryContextException),
        HttpStatusCode.BadRequest
      }
    };
    private static readonly StringComparer QueryParamComparer = StringComparer.OrdinalIgnoreCase;
    private const string s_machineIdQueryParameterName = "machineId";
    private static readonly char[] s_machineIdDelimiter = new char[1]
    {
      ','
    };
    private const int s_MaxTelemetryItemsCount = 100;
    private const int s_MaxTelemetryKeyLength = 100;
    private const int s_MaxTelemetryValueLength = 1000;
    private const string s_layer = "LicensingClientRightsController";

    [HttpGet]
    [TraceFilter(1038400, 1038409)]
    [ClientResponseType(typeof (ClientRightsContainer), null, null)]
    public HttpResponseMessage GetClientRights(
      string rightName = null,
      string productVersion = null,
      string edition = null,
      string relType = null,
      bool includeCertificate = false,
      string canary = null,
      string machineId = null)
    {
      try
      {
        ClientRightsQueryContext queryContext = LicensingClientRightsController.DeserializeQueryContext(rightName, productVersion, edition, relType, includeCertificate, canary, machineId);
        ClientRightsTelemetryContext telemetryContext = this.DeserializeTelemetryContext(this.Request);
        try
        {
          return this.Request.CreateResponse<ClientRightsContainer>(HttpStatusCode.OK, this.TfsRequestContext.GetClient<UserLicensingHttpClient>(FrameworkServerConstants.UserExtensionPrincipal).GetClientRightsContainerAsync((string) LicensingApiController.GetUserSubjectDescriptor(this.TfsRequestContext), queryContext, telemetryContext, cancellationToken: this.TfsRequestContext.CancellationToken).SyncResult<ClientRightsContainer>());
        }
        catch (Exception ex)
        {
          this.TfsRequestContext.TraceException(141531, "VisualStudio.Services.LicensingService", nameof (LicensingClientRightsController), ex);
          throw;
        }
      }
      catch (Exception ex)
      {
        this.HandleException(ex, 1038408);
        throw;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LicensingClientRightsController.s_httpExceptions;

    private static ClientRightsQueryContext DeserializeQueryContext(
      string rightName,
      string productVersion,
      string edition,
      string relType,
      bool includeCertificate,
      string canary,
      string machineId)
    {
      ClientRightsQueryContext rightsQueryContext = new ClientRightsQueryContext()
      {
        ProductFamily = rightName,
        IncludeCertificate = includeCertificate,
        ProductVersion = !string.IsNullOrWhiteSpace(productVersion) ? productVersion : (string) null,
        ProductEdition = !string.IsNullOrWhiteSpace(edition) ? edition : (string) null,
        ReleaseType = !string.IsNullOrWhiteSpace(relType) ? relType : (string) null,
        Canary = !string.IsNullOrWhiteSpace(canary) ? canary : (string) null
      };
      if (string.IsNullOrWhiteSpace(machineId))
        return rightsQueryContext;
      string[] strArray = machineId.Split(LicensingClientRightsController.s_machineIdDelimiter, StringSplitOptions.RemoveEmptyEntries);
      rightsQueryContext.MachineId = strArray[0];
      return rightsQueryContext;
    }

    private ClientRightsTelemetryContext DeserializeTelemetryContext(HttpRequestMessage request)
    {
      ClientRightsTelemetryContext telemetryContext = new ClientRightsTelemetryContext()
      {
        Attributes = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) LicensingClientRightsController.QueryParamComparer)
      };
      foreach (KeyValuePair<string, string> keyValuePair in request.GetQueryNameValuePairs().Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (pair => LicensingClientRightsController.IsValidTelemetryPair(pair))))
      {
        if (telemetryContext.Attributes.Count >= 100)
        {
          this.TfsRequestContext.Trace(1038407, TraceLevel.Warning, "VisualStudio.Services.LicensingService", nameof (LicensingClientRightsController), "Telemetry items have exceeded the max allowed count ({0}).", (object) 100);
          break;
        }
        string key = keyValuePair.Key;
        string str = keyValuePair.Value ?? string.Empty;
        if (LicensingClientRightsController.QueryParamComparer.Equals(keyValuePair.Key, "machineId"))
        {
          key = "t-machineId";
          string[] strArray = str.Split(LicensingClientRightsController.s_machineIdDelimiter, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length > 1)
            str = strArray[1];
          else
            continue;
        }
        telemetryContext.Attributes[key] = str;
      }
      return telemetryContext;
    }

    private static bool IsValidTelemetryPair(KeyValuePair<string, string> pair)
    {
      if (string.IsNullOrEmpty(pair.Key) || pair.Key.Length > 100)
        return false;
      if (LicensingClientRightsController.QueryParamComparer.Equals(pair.Key, "machineId"))
        return true;
      if (!pair.Key.StartsWith("t-", StringComparison.OrdinalIgnoreCase) || pair.Value != null && pair.Value.Length > 1000)
        return false;
      for (int length = "t-".Length; length < pair.Key.Length; ++length)
      {
        char ch = pair.Key[length];
        if ((ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z'))
          return false;
      }
      return true;
    }

    private void HandleException(Exception ex, int tracepoint)
    {
      if (LicensingClientRightsController.s_httpExceptions.ContainsKey(ex.GetType()))
        return;
      this.TfsRequestContext.TraceException(tracepoint, "VisualStudio.Services.LicensingService", nameof (LicensingClientRightsController), ex);
      TeamFoundationEventLog.Default.LogException(ex.Message, ex);
    }
  }
}
