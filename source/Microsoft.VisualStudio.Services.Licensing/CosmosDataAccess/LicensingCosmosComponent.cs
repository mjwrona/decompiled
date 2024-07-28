// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.LicensingCosmosComponent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DocDB;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess.Model;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Licensing.CosmosDataAccess
{
  internal class LicensingCosmosComponent : 
    DocDBResourceComponent<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>,
    ILicensingComponent,
    IDisposable,
    IExtensionLicensingComponent
  {
    private const string EnableBulkExecutorFeatureName = "AzureDevOps.Services.Licensing.EnableBulkExecutor";
    private const string EnableScopeIdFilterFeatureName = "AzureDevOps.Services.Licensing.EnableScopeIdFilter";

    protected virtual string CollectionId => "Licensing";

    protected LicensingCosmosBulkExecutorService BulkExecutor => ((DocDBResourceComponentBase) this).RequestContext.ToDeployment().GetService<LicensingCosmosBulkExecutorService>();

    private IQueryable<UserLicenseCosmosSerializableDocument> CreateDocumentQuery(
      string continuationToken = null,
      int maxItemCount = -1)
    {
      FeedOptions feedOptions = new FeedOptions()
      {
        RequestContinuation = continuationToken,
        MaxItemCount = new int?(maxItemCount),
        ResponseContinuationTokenLimitInKb = new int?(1)
      };
      SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem> documentMethodOptions = new SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>();
      ((DocDBMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) documentMethodOptions).FeedOptions = feedOptions;
      return this.CreateQuery(documentMethodOptions);
    }

    private IQueryable<UserLicenseCosmosSerializableDocument> CreateDocumentQuery(
      IEnumerable<Guid> userIds,
      string continuationToken = null)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LicensingCosmosComponent.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass5_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.userIds = userIds;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      return this.CreateDocumentQuery(continuationToken).Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.Call((Expression) null, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Enumerable.Contains)), new Expression[2]
      {
        cDisplayClass50.userIds,
        (Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_UserId)))
      }), parameterExpression));
    }

    private UserLicenseCosmosSerializableDocument GetDocument(Guid userId) => ((DocDBResourceComponent) this).ReadDocument<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>(userId.ToString(), (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);

    private IEnumerable<UserLicenseCosmosSerializableDocument> GetDocuments() => ((DocDBResourceComponent) this).Get<UserLicenseCosmosSerializableDocument>(this.CreateDocumentQuery(), true);

    private IEnumerable<UserLicenseCosmosSerializableDocument> GetDocuments(
      IEnumerable<Guid> userIds)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LicensingCosmosComponent.\u003C\u003Ec__DisplayClass8_0 cDisplayClass80 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass8_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass80.userIds = userIds;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      return ((DocDBResourceComponent) this).Get<UserLicenseCosmosSerializableDocument>(this.CreateDocumentQuery().Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.Call((Expression) null, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Enumerable.Contains)), new Expression[2]
      {
        cDisplayClass80.userIds,
        (Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_UserId)))
      }), parameterExpression)), true);
    }

    private SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem> BuildSerializableDocumentMethodOptions(
      UserLicenseCosmosSerializableDocument document,
      string documentId = null,
      bool withOptimisticConcurrency = false)
    {
      if (documentId == null && !withOptimisticConcurrency)
        return (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null;
      SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem> documentMethodOptions1 = new SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>();
      ((DocDBMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) documentMethodOptions1).DocumentId = documentId;
      SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem> documentMethodOptions2 = documentMethodOptions1;
      if (withOptimisticConcurrency)
        ((DocDBMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) documentMethodOptions2).RequestOptions = new RequestOptions()
        {
          AccessCondition = new AccessCondition()
          {
            Condition = ((Resource) document).ETag,
            Type = (AccessConditionType) 0
          }
        };
      return documentMethodOptions2;
    }

    public void CreateScope(Guid scopeId)
    {
    }

    public IList<Guid> GetScopes() => (IList<Guid>) Enumerable.Empty<Guid>().ToList<Guid>();

    public List<UserLicense> GetPreviousUserLicenses(Guid scopeId, IList<Guid> userIds) => this.GetDocuments((IEnumerable<Guid>) userIds).Select<UserLicenseCosmosSerializableDocument, UserLicense>((Func<UserLicenseCosmosSerializableDocument, UserLicense>) (x => x.Document.PreviousLicense)).Where<UserLicense>((Func<UserLicense, bool>) (x => x != null)).ToList<UserLicense>();

    public UserLicense GetUserLicense(Guid scopeId, Guid userId) => this.GetDocument(userId)?.Document?.License;

    public UserLicenseCosmosSerializableDocument GetUserLicenseCosmosDocumentByIdAndPreviousId(
      Guid userId)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LicensingCosmosComponent.\u003C\u003Ec__DisplayClass14_0 cDisplayClass140 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass14_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass140.userId = userId;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      return ((DocDBResourceComponent) this).Get<UserLicenseCosmosSerializableDocument>(this.CreateDocumentQuery().Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.OrElse((Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_UserId))), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass140, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass14_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass14_0.userId))), false, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Guid.op_Equality))), (Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_PreviousUserId))), (Expression) Expression.Convert((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass140, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass14_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass14_0.userId))), typeof (Guid?)), false, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Guid.op_Equality)))), parameterExpression)), false).FirstOrDefault<UserLicenseCosmosSerializableDocument>();
    }

    public UserLicense SetUserLicense(
      Guid scopeId,
      Guid userId,
      LicensingSource source,
      int license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      AccountUserStatus statusIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity)
    {
      this.TraceLicensingEvent(userId, licensingEvent);
      UserLicenseCosmosSerializableDocument document = this.GetDocument(userId);
      if (document == null)
      {
        UserLicense license1 = new UserLicense(scopeId, userId, assignmentSource, license, origin, source, statusIfAbsent);
        return this.Create(new UserLicenseCosmosSerializableDocument(userId, license1, licensedIdentity), (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null).Document.License;
      }
      document.Document.LicensedIdentity = licensedIdentity;
      if (document.Document.License == null)
      {
        UserLicense userLicense = new UserLicense(scopeId, userId, assignmentSource, license, origin, source, statusIfAbsent);
        document.Document.License = userLicense;
        return this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null).Document.License;
      }
      UserLicense license2 = document.Document.License;
      if (license2.Source == source && license2.License == license && license2.AssignmentSource == assignmentSource)
        return license2;
      if (license2.AssignmentSource != AssignmentSource.GroupRule && assignmentSource == AssignmentSource.GroupRule)
        document.Document.PreviousLicense = license2.ToPreviousUserLicense();
      license2.Source = source;
      license2.License = license;
      license2.AssignmentDate = DateTimeOffset.UtcNow;
      license2.LastUpdated = DateTimeOffset.UtcNow;
      if (assignmentSource != AssignmentSource.None)
        license2.AssignmentSource = assignmentSource;
      return this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null).Document.License;
    }

    public void DeleteUserLicense(Guid scopeId, Guid userId, ILicensingEvent licensingEvent)
    {
      UserLicenseCosmosSerializableDocument document = this.GetDocument(userId);
      if (document == null)
        return;
      if (document.Document.ExtensionLicenses == null || document.Document.ExtensionLicenses.Count == 0)
      {
        this.Delete(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      }
      else
      {
        if (document.Document.License != null)
          document.Document.License = (UserLicense) null;
        if (document.Document.PreviousLicense != null)
          document.Document.PreviousLicense = (UserLicense) null;
        this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      }
      this.TraceLicensingEvent(userId, licensingEvent);
    }

    public IList<UserLicense> GetUserLicenses(Guid scopeId) => (IList<UserLicense>) ((DocDBResourceComponent) this).Get<UserLicense>(this.CreateQuery().Select<UserLicenseCosmosItem, UserLicense>((Expression<Func<UserLicenseCosmosItem, UserLicense>>) (x => x.License)).Where<UserLicense>((Expression<Func<UserLicense, bool>>) (x => x != default (object))), true).ToList<UserLicense>();

    public IList<UserLicense> GetUserLicenses(Guid scopeId, IList<Guid> userIds) => (IList<UserLicense>) this.GetDocuments((IEnumerable<Guid>) userIds).Select<UserLicenseCosmosSerializableDocument, UserLicense>((Func<UserLicenseCosmosSerializableDocument, UserLicense>) (x => x.Document.License)).Where<UserLicense>((Func<UserLicense, bool>) (x => x != null)).ToList<UserLicense>();

    public IList<UserLicense> GetUserLicenses(Guid scopeId, int top, int skip) => (IList<UserLicense>) this.GetDocuments().Select<UserLicenseCosmosSerializableDocument, UserLicense>((Func<UserLicenseCosmosSerializableDocument, UserLicense>) (x => x.Document.License)).Where<UserLicense>((Func<UserLicense, bool>) (x => x != null)).Skip<UserLicense>(skip).Take<UserLicense>(top).ToList<UserLicense>();

    public IPagedList<UserLicense> GetUserLicenses(Guid scopeId, string continuationToken)
    {
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      FeedResponse<UserLicense> page = ((DocDBResourceComponent) this).GetPage<UserLicense>(this.CreateDocumentQuery(continuationToken).Select<UserLicenseCosmosSerializableDocument, UserLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, UserLicense>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), parameterExpression)).Where<UserLicense>((Expression<Func<UserLicense, bool>>) (x => x != default (object))));
      return (IPagedList<UserLicense>) new PagedList<UserLicense>((IEnumerable<UserLicense>) page, page.ResponseContinuation);
    }

    public IPagedList<UserLicense> GetFilteredUserLicenses(
      string continuation,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort)
    {
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      FeedResponse<UserLicense> page = ((DocDBResourceComponent) this).GetPage<UserLicense>(this.GetFilterAndSortDocumentQuery(continuation, maxPageSize, filter, sort).Select<UserLicenseCosmosSerializableDocument, UserLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, UserLicense>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), parameterExpression)));
      return (IPagedList<UserLicense>) new PagedList<UserLicense>((IEnumerable<UserLicense>) page, page.ResponseContinuation);
    }

    public IPagedList<UserLicense> GetProfessionalLicenseUsers(string continuation)
    {
      ParameterExpression parameterExpression1;
      ParameterExpression parameterExpression2;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      FeedResponse<UserLicense> page = ((DocDBResourceComponent) this).GetPage<UserLicense>(this.CreateDocumentQuery(continuation).Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.AndAlso((Expression) Expression.Equal((Expression) Expression.Convert((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression1, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_Source))), typeof (int)), (Expression) Expression.Constant((object) 1, typeof (int))), (Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression1, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_License))), (Expression) Expression.Constant((object) 3, typeof (int)))), parameterExpression1)).Select<UserLicenseCosmosSerializableDocument, UserLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, UserLicense>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression2, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), parameterExpression2)));
      return (IPagedList<UserLicense>) new PagedList<UserLicense>((IEnumerable<UserLicense>) page, page.ResponseContinuation);
    }

    public void UpdateUserLastAccessed(Guid scopeId, Guid userId, DateTimeOffset lastAccessedDate)
    {
      UserLicenseCosmosSerializableDocument document = this.GetDocument(userId);
      if (document == null)
        return;
      if (document.Document.License != null)
        document.Document.License.LastAccessed = lastAccessedDate;
      this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
    }

    public IList<AccountLicenseCount> GetUserLicensesDistribution(Guid scopeId) => (IList<AccountLicenseCount>) this.GetDocuments().Select<UserLicenseCosmosSerializableDocument, UserLicense>((Func<UserLicenseCosmosSerializableDocument, UserLicense>) (x => x.Document.License)).Where<UserLicense>((Func<UserLicense, bool>) (x => x != null)).GroupBy(x => new
    {
      Source = x.Source,
      License = x.License
    }).Select<IGrouping<\u003C\u003Ef__AnonymousType0<LicensingSource, int>, UserLicense>, AccountLicenseCount>(x => new AccountLicenseCount()
    {
      License = new AccountUserLicense(x.Key.Source, x.Key.License),
      Count = x.Count<UserLicense>()
    }).ToList<AccountLicenseCount>();

    public int GetUserLicenseCount(LicensingSource source, int license)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LicensingCosmosComponent.\u003C\u003Ec__DisplayClass25_0 cDisplayClass250 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass25_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass250.source = source;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass250.license = license;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: field reference
      return this.CreateDocumentQuery().Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.AndAlso((Expression) Expression.Equal((Expression) Expression.Convert((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_Source))), typeof (int)), (Expression) Expression.Convert((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass250, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass25_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass25_0.source))), typeof (int))), (Expression) Expression.Equal((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_License))), (Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass250, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass25_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass25_0.license))))), parameterExpression)).Count<UserLicenseCosmosSerializableDocument>();
    }

    public IList<UserLicenseCount> GetUserLicenseUsage(Guid scopeId)
    {
      ParameterExpression parameterExpression1;
      ParameterExpression parameterExpression2;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: type reference
      return (IList<UserLicenseCount>) ((DocDBResourceComponent) this).Get(this.CreateDocumentQuery().Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.NotEqual((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression1, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (Expression) Expression.Constant((object) null, typeof (object))), parameterExpression1)).Select(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, \u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>>>((Expression) Expression.New((ConstructorInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>.\u002Ector), __typeref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>)), (IEnumerable<Expression>) new Expression[3]
      {
        (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression2, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_Source))),
        (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression2, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_License))),
        (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression2, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_Status)))
      }, (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>.get_Source), __typeref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>.get_License), __typeref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>.get_Status), __typeref (\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>))), parameterExpression2)), true).ToList().GroupBy(x => new
      {
        Source = x.Source,
        License = x.License,
        Status = x.Status
      }).Select<IGrouping<\u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>, \u003C\u003Ef__AnonymousType1<LicensingSource, int, AccountUserStatus>>, UserLicenseCount>(x =>
      {
        return new UserLicenseCount()
        {
          License = new AccountUserLicense(x.Key.Source, x.Key.License),
          UserStatus = x.Key.Status,
          Count = x.Count()
        };
      }).ToList<UserLicenseCount>();
    }

    public void ImportScope(
      Guid scopeId,
      List<UserLicense> userLicenses,
      List<UserLicense> previousUserLicenses,
      List<UserExtensionLicense> userExtensionLicenses,
      ILicensingEvent licensingEvent)
    {
      Dictionary<Guid, UserLicense> snapshotLicenses = userLicenses.ToDictionary<UserLicense, Guid>((Func<UserLicense, Guid>) (x => x.UserId));
      Dictionary<Guid, UserLicense> snapshotPreviousLicenses = previousUserLicenses.Where<UserLicense>((Func<UserLicense, bool>) (x => x != null)).Select<UserLicense, UserLicense>((Func<UserLicense, UserLicense>) (x => x.ToPreviousUserLicense())).ToDictionary<UserLicense, Guid>((Func<UserLicense, Guid>) (x => x.UserId));
      ILookup<Guid, UserExtensionLicense> snapshotExtensionLicenses = userExtensionLicenses.ToLookup<UserExtensionLicense, Guid>((Func<UserExtensionLicense, Guid>) (x => x.UserId));
      List<Guid> list1 = snapshotLicenses.Select<KeyValuePair<Guid, UserLicense>, Guid>((Func<KeyValuePair<Guid, UserLicense>, Guid>) (x => x.Key)).Union<Guid>(previousUserLicenses.Select<UserLicense, Guid>((Func<UserLicense, Guid>) (x => x.UserId))).Union<Guid>(userExtensionLicenses.Select<UserExtensionLicense, Guid>((Func<UserExtensionLicense, Guid>) (x => x.UserId))).ToList<Guid>();
      IEnumerable<UserLicenseCosmosSerializableDocument> serializableDocuments = this.GetDocuments((IEnumerable<Guid>) list1).Select<UserLicenseCosmosSerializableDocument, UserLicenseCosmosSerializableDocument>((Func<UserLicenseCosmosSerializableDocument, UserLicenseCosmosSerializableDocument>) (user =>
      {
        user.Document.License = snapshotLicenses.GetValueOrDefault<Guid, UserLicense>(user.Document.UserId, user.Document.License);
        user.Document.PreviousLicense = snapshotPreviousLicenses.GetValueOrDefault<Guid, UserLicense>(user.Document.UserId, user.Document.PreviousLicense);
        user.Document.ExtensionLicenses = snapshotExtensionLicenses[user.Document.UserId].ToList<UserExtensionLicense>();
        return user;
      }));
      IEnumerable<UserLicenseCosmosSerializableDocument> second = list1.Except<Guid>(serializableDocuments.Select<UserLicenseCosmosSerializableDocument, Guid>((Func<UserLicenseCosmosSerializableDocument, Guid>) (x => x.Document.UserId))).Select<Guid, UserLicenseCosmosSerializableDocument>((Func<Guid, UserLicenseCosmosSerializableDocument>) (userId => new UserLicenseCosmosSerializableDocument(userId, snapshotLicenses.GetValueOrDefault<Guid, UserLicense>(userId, (UserLicense) null), new LicensedIdentity(), snapshotPreviousLicenses.GetValueOrDefault<Guid, UserLicense>(userId, (UserLicense) null), snapshotExtensionLicenses[userId].ToList<UserExtensionLicense>())));
      List<UserLicenseCosmosSerializableDocument> list2 = serializableDocuments.Union<UserLicenseCosmosSerializableDocument>(second).ToList<UserLicenseCosmosSerializableDocument>();
      if (((DocDBResourceComponentBase) this).RequestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableBulkExecutor"))
      {
        this.BulkExecutor.Upsert<UserLicenseCosmosSerializableDocument>(((DocDBResourceComponentBase) this).RequestContext, (IEnumerable<UserLicenseCosmosSerializableDocument>) list2);
      }
      else
      {
        foreach (UserLicenseCosmosSerializableDocument serializableDocument in list2)
          this.Upsert(serializableDocument, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      }
      this.TraceLicensingEvent(Guid.Empty, licensingEvent);
    }

    public void DeleteScope(Guid scopeId, ILicensingEvent licensingEvent)
    {
      if (((DocDBResourceComponentBase) this).RequestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableBulkExecutor"))
      {
        this.BulkExecutor.DeletePartition(((DocDBResourceComponentBase) this).RequestContext);
      }
      else
      {
        foreach (UserLicenseCosmosSerializableDocument document in this.GetDocuments())
          this.Delete(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      }
      this.TraceLicensingEvent(Guid.Empty, licensingEvent);
    }

    public void AddUser(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      License licenseIfAbsent,
      AssignmentSource assignmentSourceIfAbsent,
      LicensingOrigin originIfAbsent,
      ILicensingEvent licensingEvent,
      LicensedIdentity licensedIdentity)
    {
      UserLicenseCosmosSerializableDocument serializableDocument = this.GetDocument(userId) ?? new UserLicenseCosmosSerializableDocument(userId);
      if (licensedIdentity != null && !licensedIdentity.IsEmpty())
        serializableDocument.Document.LicensedIdentity = licensedIdentity;
      if (serializableDocument.Document.License == null)
      {
        serializableDocument.Document.License = new UserLicense(scopeId, userId, assignmentSourceIfAbsent, licenseIfAbsent.GetLicenseAsInt32(), originIfAbsent, licenseIfAbsent.Source, status);
      }
      else
      {
        serializableDocument.Document.License.Status = status;
        serializableDocument.Document.License.LastUpdated = DateTimeOffset.UtcNow;
      }
      this.Upsert(serializableDocument, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      this.TraceLicensingEvent(userId, licensingEvent);
    }

    public void UpdateUserStatus(
      Guid scopeId,
      Guid userId,
      AccountUserStatus status,
      ILicensingEvent licensingEvent)
    {
      UserLicenseCosmosSerializableDocument document = this.GetDocument(userId);
      if (document == null)
        return;
      if (document.Document.License != null)
      {
        document.Document.License.Status = status;
        document.Document.License.LastUpdated = DateTimeOffset.UtcNow;
      }
      this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      this.TraceLicensingEvent(userId, licensingEvent);
    }

    private void DeleteExistingTargetDocuments(ICollection<Guid> userIds)
    {
      foreach (UserLicenseCosmosSerializableDocument document in this.GetDocuments((IEnumerable<Guid>) userIds))
        this.DeleteUserLicenseCosmosDocument(document, false);
    }

    public void TransferUserLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
      this.DeleteExistingTargetDocuments((ICollection<Guid>) userIdTransferMap.Select<KeyValuePair<Guid, Guid>, Guid>((Func<KeyValuePair<Guid, Guid>, Guid>) (x => x.Value)).ToList<Guid>());
      foreach (UserLicenseCosmosSerializableDocument document in this.GetDocuments(userIdTransferMap.Select<KeyValuePair<Guid, Guid>, Guid>((Func<KeyValuePair<Guid, Guid>, Guid>) (x => x.Key))).Where<UserLicenseCosmosSerializableDocument>((Func<UserLicenseCosmosSerializableDocument, bool>) (x => x?.Document?.License != null)).Join<UserLicenseCosmosSerializableDocument, KeyValuePair<Guid, Guid>, Guid, UserLicenseCosmosSerializableDocument>(userIdTransferMap, (Func<UserLicenseCosmosSerializableDocument, Guid>) (left => left.Document.License.UserId), (Func<KeyValuePair<Guid, Guid>, Guid>) (right => right.Key), (Func<UserLicenseCosmosSerializableDocument, KeyValuePair<Guid, Guid>, UserLicenseCosmosSerializableDocument>) ((left, right) => left.Transfer(right.Value))))
      {
        try
        {
          this.Update(document, this.BuildSerializableDocumentMethodOptions(document, document.Document.PreviousUserId.ToString()));
        }
        catch (Exception ex)
        {
          ((DocDBResourceComponentBase) this).Trace(1035300, TraceLevel.Error, string.Format("Failed during ReplaceDocument for {0} with account id {1}, ", (object) nameof (TransferUserLicenses), (object) scopeId) + string.Format("sourceId {0}, and targetId {1}. Exception: {2}", (object) document.Document.PreviousUserId, (object) document.Document.UserId, (object) ex.Message));
        }
      }
    }

    public void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      foreach (UserLicenseCosmosSerializableDocument document in this.GetDocuments(userIds))
      {
        this.BuildExtensionAssignment(document, scopeId, extensionId, source, assignmentSource, collectionId);
        this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      }
    }

    private void BuildExtensionAssignment(
      UserLicenseCosmosSerializableDocument user,
      Guid scopedId,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      UserExtensionLicense extensionLicense = user.Document.ExtensionLicenses.SingleOrDefault<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (x => x.ExtensionId == extensionId && x.Source == source));
      if (extensionLicense != null)
      {
        if (extensionLicense.Status == UserExtensionLicenseStatus.NotActive)
          extensionLicense.CollectionId = collectionId;
        extensionLicense.Status = UserExtensionLicenseStatus.Active;
        extensionLicense.LastUpdated = new DateTime?(DateTime.UtcNow);
        if (assignmentSource == AssignmentSource.None)
          return;
        extensionLicense.AssignmentSource = assignmentSource;
      }
      else
        user.Document.ExtensionLicenses.Add(new UserExtensionLicense()
        {
          ExtensionId = extensionId,
          AssignmentDate = (DateTimeOffset) DateTime.UtcNow,
          AssignmentSource = assignmentSource != AssignmentSource.None ? assignmentSource : AssignmentSource.Unknown,
          CollectionId = collectionId,
          Source = source,
          Status = UserExtensionLicenseStatus.Active,
          UserId = user.Document.License.UserId
        });
    }

    public IList<UserExtensionLicense> FilterUsersWithExtensionBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId)
    {
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      return (IList<UserExtensionLicense>) ((DocDBResourceComponent) this).Get<UserExtensionLicense>(this.CreateDocumentQuery(userIds).SelectMany<UserLicenseCosmosSerializableDocument, UserExtensionLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, IEnumerable<UserExtensionLicense>>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_ExtensionLicenses))), parameterExpression)).Where<UserExtensionLicense>((Expression<Func<UserExtensionLicense, bool>>) (x => x.ExtensionId == extensionId && (int) x.Status == 1)), true).ToList<UserExtensionLicense>();
    }

    public IList<UserExtensionLicense> GetUserExtensionLicenses(
      Guid scopeId,
      Guid userId,
      UserExtensionLicenseStatus status)
    {
      UserLicenseCosmosSerializableDocument document = this.GetDocument(userId);
      return document == null ? (IList<UserExtensionLicense>) new List<UserExtensionLicense>() : (IList<UserExtensionLicense>) document.Document.ExtensionLicenses.Where<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (x => x.Status == status)).ToList<UserExtensionLicense>();
    }

    public IDictionary<Guid, IList<ExtensionSource>> GetExtensionsForUsersBatch(
      Guid scopeId,
      IList<Guid> userIds)
    {
      return (IDictionary<Guid, IList<ExtensionSource>>) this.GetDocuments((IEnumerable<Guid>) userIds).ToDictionary<UserLicenseCosmosSerializableDocument, Guid, IList<ExtensionSource>>((Func<UserLicenseCosmosSerializableDocument, Guid>) (x => x.Document.UserId), (Func<UserLicenseCosmosSerializableDocument, IList<ExtensionSource>>) (x => (IList<ExtensionSource>) x.Document.ExtensionLicenses.Where<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (y => y.Status == UserExtensionLicenseStatus.Active)).Select<UserExtensionLicense, ExtensionSource>((Func<UserExtensionLicense, ExtensionSource>) (y => new ExtensionSource()
      {
        AssignmentSource = y.AssignmentSource,
        ExtensionGalleryId = y.ExtensionId,
        LicensingSource = y.Source
      })).ToList<ExtensionSource>()));
    }

    public IList<UserExtensionLicense> GetUserExtensionLicenses(Guid scopeId)
    {
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      return (IList<UserExtensionLicense>) ((DocDBResourceComponent) this).Get<UserExtensionLicense>(this.CreateDocumentQuery().SelectMany<UserLicenseCosmosSerializableDocument, UserExtensionLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, IEnumerable<UserExtensionLicense>>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_ExtensionLicenses))), parameterExpression)), true).ToList<UserExtensionLicense>();
    }

    public int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      List<string> list = extensionIds.ToList<string>();
      UserLicenseCosmosSerializableDocument document = this.GetDocument(userId);
      if (document == null)
        throw new ArgumentException(string.Format("No document found for userId {0} in host {1}", (object) userId, (object) scopeId), nameof (userId));
      IEnumerable<\u003C\u003Ef__AnonymousType2<UserExtensionLicense, string>> datas = document.Document.ExtensionLicenses.GroupJoin((IEnumerable<string>) list, (Func<UserExtensionLicense, string>) (x => x.ExtensionId), (Func<string, string>) (y => y), (x, y) => new
      {
        ExistingLicense = x,
        NewLicense = y.SingleOrDefault<string>()
      });
      int userBatchWithCount = 0;
      foreach (var data in datas)
      {
        if (data.NewLicense == null && data.ExistingLicense.Source == source || data.NewLicense != null)
          data.ExistingLicense.LastUpdated = new DateTime?(DateTime.UtcNow);
        if (data.NewLicense == null && data.ExistingLicense.Source == source || data.NewLicense != null && data.ExistingLicense.Source != source)
          data.ExistingLicense.Status = UserExtensionLicenseStatus.NotActive;
        if (data.NewLicense != null && data.ExistingLicense.Source == source)
          data.ExistingLicense.Status = UserExtensionLicenseStatus.Active;
        if (data.ExistingLicense.Status == UserExtensionLicenseStatus.NotActive && data.NewLicense != null)
          data.ExistingLicense.CollectionId = collectionId;
        if (assignmentSource != AssignmentSource.None)
          data.ExistingLicense.AssignmentSource = assignmentSource;
        ++userBatchWithCount;
      }
      foreach (string extensionId in list.Except<string>(document.Document.ExtensionLicenses.Where<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (x => x.Source == source)).Select<UserExtensionLicense, string>((Func<UserExtensionLicense, string>) (x => x.ExtensionId))))
      {
        this.BuildExtensionAssignment(document, scopeId, extensionId, source, assignmentSource, collectionId);
        ++userBatchWithCount;
      }
      this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
      return userBatchWithCount;
    }

    public void UpdateUserStatusBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      UserExtensionLicenseStatus status,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      foreach (UserLicenseCosmosSerializableDocument document in this.GetDocuments(userIds))
      {
        UserExtensionLicense extensionLicense = document.Document.ExtensionLicenses.SingleOrDefault<UserExtensionLicense>((Func<UserExtensionLicense, bool>) (y => y.ExtensionId == extensionId && y.Source == source));
        if (extensionLicense != null)
        {
          if (extensionLicense.Status == UserExtensionLicenseStatus.NotActive && status == UserExtensionLicenseStatus.Active)
            extensionLicense.CollectionId = collectionId;
          extensionLicense.Status = status;
          extensionLicense.LastUpdated = new DateTime?(DateTime.UtcNow);
          if (assignmentSource != AssignmentSource.None)
            extensionLicense.AssignmentSource = assignmentSource;
          this.Update(document, (SerializableDocumentMethodOptions<UserLicenseCosmosSerializableDocument, UserLicenseCosmosItem>) null);
        }
      }
    }

    public IList<AccountExtensionCount> GetAccountExtensionCount(
      Guid scopeId,
      UserExtensionLicenseStatus status)
    {
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      return (IList<AccountExtensionCount>) ((DocDBResourceComponent) this).Get(this.CreateDocumentQuery().SelectMany<UserLicenseCosmosSerializableDocument, UserExtensionLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, IEnumerable<UserExtensionLicense>>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_ExtensionLicenses))), parameterExpression)).Where<UserExtensionLicense>((Expression<Func<UserExtensionLicense, bool>>) (x => (int) x.Status == (int) status)).Select(x => new
      {
        ExtensionId = x.ExtensionId,
        Source = x.Source
      }), true).ToList().GroupBy(x => new
      {
        ExtensionId = x.ExtensionId,
        Source = x.Source
      }).Select<IGrouping<\u003C\u003Ef__AnonymousType3<string, LicensingSource>, \u003C\u003Ef__AnonymousType3<string, LicensingSource>>, AccountExtensionCount>(x => new AccountExtensionCount()
      {
        ExtensionId = x.Key.ExtensionId,
        Source = x.Key.Source,
        Assigned = x.Count()
      }).ToList<AccountExtensionCount>();
    }

    public int GetExtensionUsageCountInAccount(Guid scopeId, string extensionId)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LicensingCosmosComponent.\u003C\u003Ec__DisplayClass42_0 cDisplayClass420 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass42_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass420.scopeId = scopeId;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass420.extensionId = extensionId;
      if (((DocDBResourceComponentBase) this).RequestContext.IsFeatureEnabled("AzureDevOps.Services.Licensing.EnableScopeIdFilter"))
      {
        ParameterExpression parameterExpression1;
        ParameterExpression parameterExpression2;
        // ISSUE: method reference
        // ISSUE: field reference
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: reference to a compiler-generated field
        return this.CreateDocumentQuery().Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.Equal((Expression) Expression.Property((Expression) parameterExpression1, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument.get_HostIdUsedInPartitionKey))), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass420, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass42_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass42_0.scopeId))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (object.ToString)), Array.Empty<Expression>())), parameterExpression1)).SelectMany<UserLicenseCosmosSerializableDocument, UserExtensionLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, IEnumerable<UserExtensionLicense>>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression2, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_ExtensionLicenses))), parameterExpression2)).Count<UserExtensionLicense>((Expression<Func<UserExtensionLicense, bool>>) (x => x.ExtensionId == cDisplayClass420.extensionId && (int) x.Status == 1 && (int) x.Source == 1));
      }
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: type reference
      // ISSUE: method reference
      // ISSUE: reference to a compiler-generated field
      return this.CreateDocumentQuery().SelectMany<UserLicenseCosmosSerializableDocument, UserExtensionLicense>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, IEnumerable<UserExtensionLicense>>>((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_ExtensionLicenses))), parameterExpression)).Count<UserExtensionLicense>((Expression<Func<UserExtensionLicense, bool>>) (x => x.ExtensionId == cDisplayClass420.extensionId && (int) x.Status == 1 && (int) x.Source == 1));
    }

    public void TransferUserExtensionLicenses(
      Guid scopeId,
      IEnumerable<KeyValuePair<Guid, Guid>> userIdTransferMap)
    {
    }

    public void DeleteUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      bool withOptimisticConcurrency = false)
    {
      if (document == null)
        return;
      if (document.Document != null)
      {
        LicensingEvent licensingEvent = new LicensingEvent()
        {
          EventData = (ILicensingEventData) CommandDataFactory.CreateDeleteLicenseCommandData(document.Document)
        };
        this.TraceLicensingEvent(document.Document.UserId, (ILicensingEvent) licensingEvent);
      }
      this.Delete(document, this.BuildSerializableDocumentMethodOptions(document, withOptimisticConcurrency: withOptimisticConcurrency));
    }

    public virtual IPagedList<UserLicenseCosmosSerializableDocument> GetPagedUserLicenseCosmosDocuments(
      string continuation)
    {
      FeedResponse<UserLicenseCosmosSerializableDocument> page = ((DocDBResourceComponent) this).GetPage<UserLicenseCosmosSerializableDocument>(this.CreateDocumentQuery(continuation));
      return (IPagedList<UserLicenseCosmosSerializableDocument>) new PagedList<UserLicenseCosmosSerializableDocument>((IEnumerable<UserLicenseCosmosSerializableDocument>) page, page.ResponseContinuation);
    }

    public IPagedList<UserLicenseCosmosSerializableDocument> GetFilteredPagedUserLicenseCosmosDocuments(
      string continuation,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort)
    {
      FeedResponse<UserLicenseCosmosSerializableDocument> page = ((DocDBResourceComponent) this).GetPage<UserLicenseCosmosSerializableDocument>(this.GetFilterAndSortDocumentQuery(continuation, maxPageSize, filter, sort));
      return (IPagedList<UserLicenseCosmosSerializableDocument>) new PagedList<UserLicenseCosmosSerializableDocument>((IEnumerable<UserLicenseCosmosSerializableDocument>) page, page.ResponseContinuation);
    }

    public virtual UserLicenseCosmosSerializableDocument GetUserLicenseCosmosDocument(Guid userId) => this.GetDocument(userId);

    public virtual IEnumerable<UserLicenseCosmosSerializableDocument> GetUserLicenseCosmosDocuments() => this.GetDocuments();

    public virtual IEnumerable<UserLicenseCosmosSerializableDocument> GetUserLicenseCosmosDocuments(
      IEnumerable<Guid> userIds)
    {
      return this.GetDocuments(userIds);
    }

    public UserLicenseCosmosSerializableDocument UpdateUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      string documentId = null,
      bool withOptimisticConcurrency = false)
    {
      if (document?.Document != null)
      {
        LicensingEvent licensingEvent = new LicensingEvent()
        {
          EventData = (ILicensingEventData) CommandDataFactory.CreateUpdateLicenseCommandData(document.Document)
        };
        this.TraceLicensingEvent(document.Document.UserId, (ILicensingEvent) licensingEvent);
      }
      return this.Update(document, this.BuildSerializableDocumentMethodOptions(document, documentId, withOptimisticConcurrency));
    }

    public UserLicenseCosmosSerializableDocument UpsertUserLicenseCosmosDocument(
      UserLicenseCosmosSerializableDocument document,
      bool withOptimisticConcurrency = false)
    {
      if (document?.Document != null)
      {
        LicensingEvent licensingEvent = new LicensingEvent()
        {
          EventData = (ILicensingEventData) CommandDataFactory.CreateUpsertLicenseCommandData(document.Document)
        };
        this.TraceLicensingEvent(document.Document.UserId, (ILicensingEvent) licensingEvent);
      }
      return this.Upsert(document, this.BuildSerializableDocumentMethodOptions(document, withOptimisticConcurrency: withOptimisticConcurrency));
    }

    public int GetUserLicenseCount() => this.CreateDocumentQuery().Count<UserLicenseCosmosSerializableDocument>();

    private IQueryable<UserLicenseCosmosSerializableDocument> GetFilterAndSortDocumentQuery(
      string continuation,
      int maxPageSize,
      AccountEntitlementFilter filter,
      AccountEntitlementSort sort)
    {
      return this.SortUsersQuery(this.FilterUsersQuery(this.CreateDocumentQuery(continuation, maxPageSize), filter), sort);
    }

    private IQueryable<UserLicenseCosmosSerializableDocument> FilterUsersQuery(
      IQueryable<UserLicenseCosmosSerializableDocument> documentQuery,
      AccountEntitlementFilter filter)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      LicensingCosmosComponent.\u003C\u003Ec__DisplayClass54_0 cDisplayClass540 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass54_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass540.filter = filter;
      // ISSUE: reference to a compiler-generated field
      if (cDisplayClass540.filter != null)
      {
        // ISSUE: reference to a compiler-generated field
        if (!cDisplayClass540.filter.Licenses.IsNullOrEmpty<LicenseFilter>())
        {
          // ISSUE: reference to a compiler-generated field
          documentQuery = this.FilterLicenseQuery(documentQuery, cDisplayClass540.filter.Licenses);
        }
        // ISSUE: reference to a compiler-generated field
        if (!cDisplayClass540.filter.UserTypes.IsNullOrEmpty<IdentityMetaType>())
        {
          ParameterExpression parameterExpression;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: method reference
          documentQuery = documentQuery.Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.Call(cDisplayClass540.filter.UserTypes, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ICollection<IdentityMetaType>.Contains), __typeref (ICollection<IdentityMetaType>)), new Expression[1]
          {
            (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_LicensedIdentity))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (LicensedIdentity.get_UserType)))
          }), parameterExpression));
        }
        // ISSUE: reference to a compiler-generated field
        if (!cDisplayClass540.filter.AssignmentSources.IsNullOrEmpty<AssignmentSource>())
        {
          ParameterExpression parameterExpression;
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: method reference
          documentQuery = documentQuery.Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.Call(cDisplayClass540.filter.AssignmentSources, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ICollection<AssignmentSource>.Contains), __typeref (ICollection<AssignmentSource>)), new Expression[1]
          {
            (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_AssignmentSource)))
          }), parameterExpression));
        }
        // ISSUE: reference to a compiler-generated field
        if (!string.IsNullOrEmpty(cDisplayClass540.filter.NameOrEmail))
        {
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: field reference
          // ISSUE: method reference
          // ISSUE: method reference
          documentQuery = documentQuery.Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.OrElse((Expression) Expression.Call((Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_LicensedIdentity))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (LicensedIdentity.get_Name))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Contains)), (Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass540, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass54_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass54_0.filter))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (AccountEntitlementFilter.get_NameOrEmail))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>())), (Expression) Expression.Call((Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_LicensedIdentity))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (LicensedIdentity.get_Email))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Contains)), (Expression) Expression.Call((Expression) Expression.Property((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass540, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass54_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass54_0.filter))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (AccountEntitlementFilter.get_NameOrEmail))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()))), parameterExpression));
        }
      }
      return documentQuery;
    }

    private IQueryable<UserLicenseCosmosSerializableDocument> FilterLicenseQuery(
      IQueryable<UserLicenseCosmosSerializableDocument> documentQuery,
      IList<LicenseFilter> filter)
    {
      if (filter.Any<LicenseFilter>((Func<LicenseFilter, bool>) (x => x.Status.HasValue)))
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        LicensingCosmosComponent.\u003C\u003Ec__DisplayClass55_0 cDisplayClass550 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass55_0();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass550.licenseNameFilters = new List<string>();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass550.licenseNameAndStatusFilters = new List<string>();
        // ISSUE: reference to a compiler-generated method
        filter.ForEach<LicenseFilter>(new Action<LicenseFilter>(cDisplayClass550.\u003CFilterLicenseQuery\u003Eb__1));
        ParameterExpression parameterExpression;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: method reference
        // ISSUE: field reference
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: method reference
        documentQuery = documentQuery.Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.OrElse((Expression) Expression.Call(cDisplayClass550.licenseNameFilters, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (List<string>.Contains), __typeref (List<string>)), new Expression[1]
        {
          (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_LicenseName)))
        }), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass550, typeof (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass55_0)), FieldInfo.GetFieldFromHandle(__fieldref (LicensingCosmosComponent.\u003C\u003Ec__DisplayClass55_0.licenseNameAndStatusFilters))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (List<string>.Contains), __typeref (List<string>)), (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_LicenseAndStatusName))))), parameterExpression));
      }
      else
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        LicensingCosmosComponent.\u003C\u003Ec__DisplayClass55_1 cDisplayClass551 = new LicensingCosmosComponent.\u003C\u003Ec__DisplayClass55_1();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass551.licenseNames = filter.Select<LicenseFilter, string>((Func<LicenseFilter, string>) (x => x.Name));
        ParameterExpression parameterExpression;
        // ISSUE: method reference
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: method reference
        documentQuery = documentQuery.Where<UserLicenseCosmosSerializableDocument>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, bool>>((Expression) Expression.Call((Expression) null, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Enumerable.Contains)), new Expression[2]
        {
          cDisplayClass551.licenseNames,
          (Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_LicenseName)))
        }), parameterExpression));
      }
      return documentQuery;
    }

    private IQueryable<UserLicenseCosmosSerializableDocument> SortUsersQuery(
      IQueryable<UserLicenseCosmosSerializableDocument> documentQuery,
      AccountEntitlementSort sort)
    {
      if (sort != null)
      {
        bool flag = sort.SortOrder == SortOrder.Descending;
        switch (sort.SortColumn)
        {
          case "name":
            if (!flag)
            {
              ParameterExpression parameterExpression;
              // ISSUE: method reference
              // ISSUE: type reference
              // ISSUE: method reference
              // ISSUE: method reference
              return (IQueryable<UserLicenseCosmosSerializableDocument>) documentQuery.OrderBy<UserLicenseCosmosSerializableDocument, string>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, string>>((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_LicensedIdentity))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (LicensedIdentity.get_Name))), parameterExpression));
            }
            ParameterExpression parameterExpression1;
            // ISSUE: method reference
            // ISSUE: type reference
            // ISSUE: method reference
            // ISSUE: method reference
            return (IQueryable<UserLicenseCosmosSerializableDocument>) documentQuery.OrderByDescending<UserLicenseCosmosSerializableDocument, string>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, string>>((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression1, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_LicensedIdentity))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (LicensedIdentity.get_Name))), parameterExpression1));
          case "lastAccessed":
            if (!flag)
            {
              ParameterExpression parameterExpression2;
              // ISSUE: method reference
              // ISSUE: type reference
              // ISSUE: method reference
              // ISSUE: method reference
              return (IQueryable<UserLicenseCosmosSerializableDocument>) documentQuery.OrderBy<UserLicenseCosmosSerializableDocument, DateTimeOffset>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, DateTimeOffset>>((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression2, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_LastAccessed))), parameterExpression2));
            }
            ParameterExpression parameterExpression3;
            // ISSUE: method reference
            // ISSUE: type reference
            // ISSUE: method reference
            // ISSUE: method reference
            return (IQueryable<UserLicenseCosmosSerializableDocument>) documentQuery.OrderByDescending<UserLicenseCosmosSerializableDocument, DateTimeOffset>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, DateTimeOffset>>((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression3, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_LastAccessed))), parameterExpression3));
          case "dateCreated":
            if (!flag)
            {
              ParameterExpression parameterExpression4;
              // ISSUE: method reference
              // ISSUE: type reference
              // ISSUE: method reference
              // ISSUE: method reference
              return (IQueryable<UserLicenseCosmosSerializableDocument>) documentQuery.OrderBy<UserLicenseCosmosSerializableDocument, DateTimeOffset>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, DateTimeOffset>>((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression4, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_DateCreated))), parameterExpression4));
            }
            ParameterExpression parameterExpression5;
            // ISSUE: method reference
            // ISSUE: type reference
            // ISSUE: method reference
            // ISSUE: method reference
            return (IQueryable<UserLicenseCosmosSerializableDocument>) documentQuery.OrderByDescending<UserLicenseCosmosSerializableDocument, DateTimeOffset>(Expression.Lambda<Func<UserLicenseCosmosSerializableDocument, DateTimeOffset>>((Expression) Expression.Property((Expression) Expression.Property((Expression) Expression.Property((Expression) parameterExpression5, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (DocDBSerializableDocument<UserLicenseCosmosItem>.get_Document), __typeref (DocDBSerializableDocument<UserLicenseCosmosItem>))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicenseCosmosItem.get_License))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserLicense.get_DateCreated))), parameterExpression5));
        }
      }
      return documentQuery;
    }

    private void TraceLicensingEvent(Guid userId, ILicensingEvent licensingEvent) => TeamFoundationTracingService.TraceLicensingEvent(((DocDBResourceComponentBase) this).RequestContext.ServiceHost.InstanceId, userId, licensingEvent.EventTypeFamily, licensingEvent.EventTypeDescriptor, (object) licensingEvent.EventData);
  }
}
