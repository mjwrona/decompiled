// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.FlightAssignmentsHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.ExP.TreatmentAssignmentClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public sealed class FlightAssignmentsHelper
  {
    private FlightAssignmentsHelper()
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Optional parameters.")]
    public static IList<string> GetFlightAssignments(
      IVssRequestContext requestContext,
      string treatmentAssignmentUri,
      string flightName = null,
      Dictionary<string, string> customHeaders = null,
      CancellationToken cancelToken = default (CancellationToken),
      string clientIP = null)
    {
      if (string.IsNullOrWhiteSpace(treatmentAssignmentUri))
        throw new ArgumentNullException(nameof (treatmentAssignmentUri));
      return FlightAssignmentsHelper.GetFlightAssignments(requestContext, new Uri(treatmentAssignmentUri), flightName, customHeaders, cancelToken, clientIP);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Optional parameters.")]
    public static IList<string> GetFlightAssignments(
      IVssRequestContext requestContext,
      Uri treatmentAssignmentUri,
      string flightName = null,
      Dictionary<string, string> customHeaders = null,
      CancellationToken cancelToken = default (CancellationToken),
      string clientIP = null)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (treatmentAssignmentUri == (Uri) null)
        throw new ArgumentNullException(nameof (treatmentAssignmentUri));
      string userId = requestContext.GetUserId().ToString("N").ToUpperInvariant();
      Microsoft.ExP.TreatmentAssignmentClient.TreatmentAssignmentClient treatmentAssignmentClient = new Microsoft.ExP.TreatmentAssignmentClient.TreatmentAssignmentClient(treatmentAssignmentUri);
      TreatmentAssignmentResponse result = Task.Run<TreatmentAssignmentResponse>((Func<Task<TreatmentAssignmentResponse>>) (() => treatmentAssignmentClient.GetTreatmentAssignmentsAsync(userId, customHeaders, cancelToken, clientIP))).GetResult<TreatmentAssignmentResponse>(requestContext.CancellationToken);
      List<string> flightAssignments = new List<string>();
      if (!result.Status.IsSuccess)
        return (IList<string>) flightAssignments;
      List<string> list = result.Flights.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Value.ToUpperInvariant())).ToList<string>();
      if (string.IsNullOrWhiteSpace(flightName))
        return (IList<string>) list;
      flightName = flightName.ToUpperInvariant();
      if (!list.Contains(flightName))
        return (IList<string>) new List<string>();
      return (IList<string>) new List<string>()
      {
        flightName
      };
    }
  }
}
