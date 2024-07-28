// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.RunRetentionLease1Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.Server.Helpers;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "leases", ResourceVersion = 1)]
  [CheckWellFormedProject(Required = true)]
  public class RunRetentionLease1Controller : BuildApiController
  {
    private const char c_serializedLeaseSeparator = '|';

    [HttpGet]
    public virtual async Task<Microsoft.TeamFoundation.Build.WebApi.RetentionLease> GetRetentionLease(
      int leaseId)
    {
      RunRetentionLease1Controller lease1Controller = this;
      Microsoft.TeamFoundation.Build2.Server.RetentionLease retentionLease = await lease1Controller.BuildService.GetRetentionLease(lease1Controller.TfsRequestContext, lease1Controller.ProjectId, leaseId);
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (lease1Controller.ProjectInfo);
      TeamProjectReference projectReference = projectInfo != null ? projectInfo.ToTeamProjectReference(lease1Controller.TfsRequestContext) : (TeamProjectReference) null;
      return retentionLease.ToWebApiRetentionLease((ISecuredObject) projectReference);
    }

    [HttpGet]
    public virtual Task<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>> GetRetentionLeasesByUserId(
      Guid userOwnerId,
      int? definitionId = null,
      int? runId = null)
    {
      return this.GetRetentionLeasesByOwnerId(RetentionLeaseHelper.GetOwnerIdForUser(userOwnerId), definitionId, runId);
    }

    [HttpGet]
    public virtual async Task<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>> GetRetentionLeasesByOwnerId(
      string ownerId = null,
      int? definitionId = null,
      int? runId = null)
    {
      RunRetentionLease1Controller lease1Controller = this;
      if (string.IsNullOrWhiteSpace(ownerId) && !definitionId.HasValue && !runId.HasValue)
        throw new MissingRequiredParameterException(BuildServerResources.MustSpecifyAtLeastOneRetentionLeaseProperty());
      if (runId.HasValue && !definitionId.HasValue)
        throw new MissingRequiredParameterException(BuildServerResources.MustIncludeDefinitionIdWithRunId());
      IReadOnlyList<Microsoft.TeamFoundation.Build2.Server.RetentionLease> source;
      if (string.IsNullOrWhiteSpace(ownerId) && runId.HasValue)
        source = await lease1Controller.BuildService.GetRetentionLeasesForRuns(lease1Controller.TfsRequestContext, lease1Controller.ProjectId, (IEnumerable<int>) new int[1]
        {
          runId.Value
        });
      else
        source = await lease1Controller.BuildService.GetRetentionLeases(lease1Controller.TfsRequestContext, lease1Controller.ProjectId, ownerId, definitionId, runId);
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (lease1Controller.ProjectInfo);
      TeamProjectReference securedObject = projectInfo != null ? projectInfo.ToTeamProjectReference(lease1Controller.TfsRequestContext) : (TeamProjectReference) null;
      return source.Select<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>((Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>) (lease => lease.ToWebApiRetentionLease((ISecuredObject) securedObject)));
    }

    [HttpGet]
    public virtual async Task<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>> GetRetentionLeasesByMinimalRetentionLeases(
      [ClientParameterAsIEnumerable(typeof (Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease), '|')] string leasesToFetch)
    {
      RunRetentionLease1Controller lease1Controller = this;
      IEnumerable<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease> source = !string.IsNullOrWhiteSpace(leasesToFetch) ? lease1Controller.DeserializeAndValidateMinimalLeases(leasesToFetch, '|') : throw new MissingRequiredParameterException(BuildServerResources.MissingLeasesToFetchParameter());
      IReadOnlyList<Microsoft.TeamFoundation.Build2.Server.RetentionLease> retentionLeases = await lease1Controller.BuildService.GetRetentionLeases(lease1Controller.TfsRequestContext, lease1Controller.ProjectId, source.Select<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease, Microsoft.TeamFoundation.Build2.Server.MinimalRetentionLease>((Func<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease, Microsoft.TeamFoundation.Build2.Server.MinimalRetentionLease>) (l => l.ToServerMinimalRetentionLease())));
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (lease1Controller.ProjectInfo);
      TeamProjectReference securedObject = projectInfo != null ? projectInfo.ToTeamProjectReference(lease1Controller.TfsRequestContext) : (TeamProjectReference) null;
      Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease> selector = (Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>) (lease => lease.ToWebApiRetentionLease((ISecuredObject) securedObject));
      return retentionLeases.Select<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>(selector);
    }

    [HttpPost]
    public virtual async Task<IEnumerable<Microsoft.TeamFoundation.Build.WebApi.RetentionLease>> AddRetentionLeases(
      [FromBody] IReadOnlyList<NewRetentionLease> newLeases)
    {
      RunRetentionLease1Controller lease1Controller = this;
      lease1Controller.CheckRequestContent((object) newLeases);
      Exception[] array = newLeases.Select<NewRetentionLease, Exception>((Func<NewRetentionLease, Exception>) (lease =>
      {
        try
        {
          this.ValidateLease(lease);
        }
        catch (Exception ex)
        {
          return ex;
        }
        return (Exception) null;
      })).Where<Exception>((Func<Exception, bool>) (e => e != null)).ToArray<Exception>();
      if (array.Length != 0)
        throw new AggregateException(array);
      IReadOnlyList<Microsoft.TeamFoundation.Build2.Server.RetentionLease> source = await lease1Controller.BuildService.AddRetentionLeases(lease1Controller.TfsRequestContext, lease1Controller.ProjectId, (IList<Microsoft.TeamFoundation.Build2.Server.RetentionLease>) newLeases.Select<NewRetentionLease, Microsoft.TeamFoundation.Build2.Server.RetentionLease>((Func<NewRetentionLease, Microsoft.TeamFoundation.Build2.Server.RetentionLease>) (lease => lease.ToServerRetentionLease())).ToArray<Microsoft.TeamFoundation.Build2.Server.RetentionLease>());
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (lease1Controller.ProjectInfo);
      TeamProjectReference securedObject = projectInfo != null ? projectInfo.ToTeamProjectReference(lease1Controller.TfsRequestContext) : (TeamProjectReference) null;
      Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease> selector = (Func<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>) (lease => lease.ToWebApiRetentionLease((ISecuredObject) securedObject));
      return source.Select<Microsoft.TeamFoundation.Build2.Server.RetentionLease, Microsoft.TeamFoundation.Build.WebApi.RetentionLease>(selector);
    }

    [HttpDelete]
    public virtual async Task DeleteRetentionLeasesById([ClientParameterAsIEnumerable(typeof (int), ',')] string ids)
    {
      RunRetentionLease1Controller lease1Controller = this;
      lease1Controller.CheckRequestContent((object) ids);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      int[] array = ((IEnumerable<string>) ids.Split(',')).Select<string, int>(RunRetentionLease1Controller.\u003C\u003EO.\u003C0\u003E__Parse ?? (RunRetentionLease1Controller.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, int>(int.Parse))).ToArray<int>();
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) array, "leaseIds");
      foreach (int var in array)
        ArgumentUtility.CheckForNonPositiveInt(var, "leaseId");
      await lease1Controller.BuildService.RemoveRetentionLeases(lease1Controller.TfsRequestContext, lease1Controller.ProjectId, (IReadOnlyList<int>) array);
    }

    protected virtual IEnumerable<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease> DeserializeAndValidateMinimalLeases(
      string leasesToFetch,
      char separator)
    {
      string[] strArray = leasesToFetch.Split(new char[1]
      {
        separator
      }, StringSplitOptions.RemoveEmptyEntries);
      List<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease> minimalRetentionLeaseList = new List<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease>();
      foreach (string toDeserialize in strArray)
      {
        Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease lease = JsonUtility.FromString<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease>(toDeserialize);
        this.ValidateMinimalLease(lease);
        minimalRetentionLeaseList.Add(lease);
      }
      return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease>) minimalRetentionLeaseList;
    }

    protected virtual void ValidateLease(NewRetentionLease lease)
    {
      ArgumentUtility.CheckForNull<NewRetentionLease>(lease, nameof (lease));
      ArgumentUtility.CheckForNonPositiveInt(lease.DaysValid, "DaysValid");
      ArgumentUtility.CheckForNonPositiveInt(lease.RunId, "RunId");
      ArgumentUtility.CheckForNonPositiveInt(lease.DefinitionId, "DefinitionId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(lease.OwnerId, "OwnerId");
    }

    protected virtual void ValidateMinimalLease(Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease lease)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease>(lease, nameof (lease));
      ArgumentUtility.CheckForNonPositiveInt(lease.RunId, "RunId");
      ArgumentUtility.CheckForNonPositiveInt(lease.DefinitionId, "DefinitionId");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(lease.OwnerId, "OwnerId");
    }
  }
}
