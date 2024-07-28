// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemSearchFieldsController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "workItemFields")]
  public class WorkItemSearchFieldsController : SearchApiController
  {
    [StaticSafe]
    private static readonly MultiPatternTextFilter s_workItemFieldSelector = new MultiPatternTextFilter("-WEF_????????????????????????????????_*,-System.IterationLevel*,-System.AreaLevel*,-System.Watermark");
    [StaticSafe]
    private static readonly ISet<string> s_identityFields = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "Microsoft.DevDiv.CssContact",
      "Microsoft.STO.DRIOnCall",
      "Microsoft.STO.Escalation1Name",
      "Microsoft.STO.Escalation2Name",
      "Microsoft.STO.Escalation3Name",
      "Microsoft.STO.Escalation4Name",
      "Microsoft.STO.LSROwner",
      "Microsoft.VSO.CS.SubmittedBy",
      "Microsoft.VSO.LSCR.ApprovedBy",
      "Microsoft.VSO.LSCR.DevContact",
      "Microsoft.VSO.LSCR.DevSignOff",
      "Microsoft.VSO.LSCR.PMContact",
      "Microsoft.VSO.LSCR.ServiceEngineer",
      "Microsoft.VSO.LSCR.ServiceEngineerPPE",
      "Microsoft.VSO.LSCR.SignOffSDLead",
      "Microsoft.VSO.LSCR.SignOffTestLead",
      "Microsoft.VSO.LSCR.TestContact",
      "Microsoft.VSO.LSCR.TestSignOff",
      "Microsoft.VSO.LSKB.ChangeApprover",
      "Microsoft.VSO.LSKB.KBOwner",
      "Microsoft.VSTS.CodeReview.AcceptedBy",
      "Microsoft.VSTS.Common.ActivatedBy",
      "Microsoft.VSTS.Common.ClosedBy",
      "Microsoft.VSTS.Common.ResolvedBy",
      "System.AssignedTo",
      "System.AuthorizedAs",
      "System.ChangedBy",
      "System.CreatedBy"
    };
    [StaticSafe("Grandfathered")]
    private static readonly Dictionary<string, string> s_fieldNameToDisplayNameMapper = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "System.History",
        "Discussion"
      }
    };
    internal static readonly IReadOnlyDictionary<FieldType, WorkItemFieldType> TfsToSearchFieldTypeMapping = (IReadOnlyDictionary<FieldType, WorkItemFieldType>) new Dictionary<FieldType, WorkItemFieldType>()
    {
      [FieldType.Boolean] = WorkItemFieldType.Boolean,
      [FieldType.DateTime] = WorkItemFieldType.DateTime,
      [FieldType.Double] = WorkItemFieldType.Double,
      [FieldType.PicklistDouble] = WorkItemFieldType.Double,
      [FieldType.History] = WorkItemFieldType.History,
      [FieldType.Html] = WorkItemFieldType.Html,
      [FieldType.Integer] = WorkItemFieldType.Integer,
      [FieldType.PicklistInteger] = WorkItemFieldType.Integer,
      [FieldType.TreePath] = WorkItemFieldType.TreePath,
      [FieldType.Guid] = WorkItemFieldType.Guid,
      [FieldType.PlainText] = WorkItemFieldType.PlainText,
      [FieldType.String] = WorkItemFieldType.String,
      [FieldType.PicklistString] = WorkItemFieldType.String,
      [FieldType.Identity] = WorkItemFieldType.Identity
    };

    [HttpGet]
    public IEnumerable<WorkItemFieldMetadata> WorkItemFields()
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080014, "REST-API", "REST-API", nameof (WorkItemFields));
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        IEnumerable<WorkItemFieldMetadata> itemFieldsMetadata = this.GetWorkItemFieldsMetadata();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetWorkItemTrackingFieldsTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        return itemFieldsMetadata;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080015, "REST-API", "REST-API", nameof (WorkItemFields));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal IEnumerable<WorkItemFieldMetadata> GetWorkItemFieldsMetadata()
    {
      IReadOnlyDictionary<string, ISet<string>> supportedAlternateNames = WorkItemAlternateFieldNames.AlternateFieldNames;
      IEnumerable<WorkItemFieldMetadata> itemFieldsMetadata = (IEnumerable<WorkItemFieldMetadata>) new List<WorkItemFieldMetadata>();
      try
      {
        itemFieldsMetadata = this.TfsRequestContext.GetService<IWorkItemFieldCacheService>().GetFieldsList(this.TfsRequestContext).Where<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField, bool>) (field => WorkItemSearchFieldsController.s_workItemFieldSelector.IsMatch(field.ReferenceName))).Select<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField, WorkItemFieldMetadata>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField, WorkItemFieldMetadata>) (field =>
        {
          IEnumerable<string> strings = !supportedAlternateNames.ContainsKey(field.ReferenceName) ? Enumerable.Empty<string>() : (IEnumerable<string>) supportedAlternateNames[field.ReferenceName];
          string str1;
          string str2 = WorkItemSearchFieldsController.s_fieldNameToDisplayNameMapper.TryGetValue(field.ReferenceName, out str1) ? str1 : field.Name;
          return new WorkItemFieldMetadata()
          {
            Name = str2,
            ReferenceName = field.ReferenceName,
            AlternateNames = strings,
            Type = WorkItemSearchFieldsController.GetWorkItemFieldType(field)
          };
        }));
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumOfFailedWorkItemFieldsRequests", "Query Pipeline", 1.0);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1080016, "REST-API", "REST-API", ex);
        if (!(ex is UnsupportedHostTypeException))
          throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
        ExceptionDispatchInfo.Capture(ex).Throw();
      }
      return itemFieldsMetadata;
    }

    private static WorkItemFieldType GetWorkItemFieldType(Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemField field) => !WorkItemSearchFieldsController.s_identityFields.Contains(field.ReferenceName) ? WorkItemSearchFieldsController.TfsToSearchFieldTypeMapping[field.Type] : WorkItemFieldType.Identity;
  }
}
