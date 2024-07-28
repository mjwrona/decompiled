// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityBatchController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "IdentityBatch")]
  public class IdentityBatchController : IdentitiesControllerBase, IOverrideLoggingMethodNames
  {
    private const string s_identityIds = "identityIds";
    private const string s_identityDescriptors = "identityDescriptors";
    private const string s_subjectDescriptors = "subjectDescriptors";
    private const string s_socialDescriptors = "socialDescriptors";
    private const string s_area = "Identity";
    private const string s_layer = "IdentityBatchController";
    private const string s_methodReadIdentityBatch = "ReadIdentityBatch";

    [HttpPost]
    [PublicCollectionRequestRestrictions(true, true, null)]
    [ClientResponseType(typeof (IQueryable<Microsoft.VisualStudio.Services.Identity.Identity>), null, null)]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage ReadIdentityBatch(IdentityBatchInfo batchInfo)
    {
      this.TfsRequestContext.TraceEnter(850550, "Identity", nameof (IdentityBatchController), nameof (ReadIdentityBatch));
      try
      {
        if (batchInfo == null || batchInfo.SubjectDescriptors == null && batchInfo.Descriptors == null && batchInfo.IdentityIds == null && batchInfo.SocialDescriptors == null)
          throw new ArgumentException(HostingResources.OneOfThreeRequiredArgumentsIsMissing((object) "identityIds", (object) "identityDescriptors", (object) "subjectDescriptors"));
        IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable;
        using (this.TfsRequestContext.SetDisposableContextItem(RequestContextItemsKeys.ReadOnlyReplicaReadEnabled, (object) true))
          queryable = batchInfo.IdentityIds == null ? (batchInfo.SubjectDescriptors == null ? (batchInfo.SocialDescriptors == null ? this.ReadIdentitiesByDescriptors(batchInfo.Descriptors, batchInfo.QueryMembership, batchInfo.PropertyNames) : this.ReadIdentitiesBySocialDescriptors(batchInfo.SocialDescriptors, batchInfo.QueryMembership, batchInfo.PropertyNames)) : this.ReadIdentitiesBySubjectDescriptors(batchInfo.SubjectDescriptors, batchInfo.QueryMembership, batchInfo.PropertyNames)) : this.ReadIdentitiesByIds(batchInfo.IdentityIds, batchInfo.QueryMembership, batchInfo.PropertyNames);
        this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
        return this.Request.CreateResponse<IQueryable<Microsoft.VisualStudio.Services.Identity.Identity>>(HttpStatusCode.OK, queryable);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(850558, "Identity", nameof (IdentityBatchController), ex);
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(850559, "Identity", nameof (IdentityBatchController), nameof (ReadIdentityBatch));
      }
    }

    private IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByIds(
      List<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> properties)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identityIds, nameof (identityIds));
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      this.TfsRequestContext.GetService<IPlatformIdentityServiceInternal>().CheckForLeakedMasterIds(this.TfsRequestContext, (IEnumerable<Guid>) identityIds);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<Guid> identityIds1 = identityIds;
      int num = (int) queryMembership;
      IEnumerable<string> propertyNameFilters = properties;
      IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable = service.ReadIdentities(tfsRequestContext, (IList<Guid>) identityIds1, (QueryMembership) num, propertyNameFilters).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
      return queryable;
    }

    private IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDescriptors(
      List<IdentityDescriptor> identityDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> properties)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identityDescriptors, nameof (identityDescriptors));
      IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) identityDescriptors, queryMembership, properties).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
      return queryable;
    }

    private IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesBySubjectDescriptors(
      List<SubjectDescriptor> subjectDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> properties)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) subjectDescriptors, "identityDescriptors");
      IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<SubjectDescriptor>) subjectDescriptors, queryMembership, properties).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
      return queryable;
    }

    private IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesBySocialDescriptors(
      List<SocialDescriptor> socialDescriptors,
      QueryMembership queryMembership,
      IEnumerable<string> properties)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) socialDescriptors, "identityDescriptors");
      IQueryable<Microsoft.VisualStudio.Services.Identity.Identity> queryable = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<SocialDescriptor>) socialDescriptors, queryMembership, properties).AsQueryable<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.ScrubMasterId((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) queryable);
      return queryable;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    string IOverrideLoggingMethodNames.GetLoggingMethodName(
      string methodName,
      HttpActionContext actionContext)
    {
      if (methodName.EndsWith("ReadIdentityBatch", StringComparison.Ordinal))
      {
        NameValueCollection queryNameValueCollection = HttpUtility.ParseQueryString(actionContext?.Request?.RequestUri?.Query ?? string.Empty);
        Dictionary<string, string> dictionary = ((IEnumerable<string>) queryNameValueCollection.AllKeys).ToDictionary<string, string, string>((Func<string, string>) (key => key), (Func<string, string>) (key => queryNameValueCollection[key]));
        methodName = IdentitiesControllerLoggingNameGenerator.GetReadIdentityBatchLoggingName(methodName, (IReadOnlyDictionary<string, string>) dictionary);
        IdentitiesControllerLoggingNameGenerator.RenameCurrentServiceIfRequestIsMembershipRelated(this.TfsRequestContext.RootContext, methodName);
      }
      return methodName;
    }
  }
}
