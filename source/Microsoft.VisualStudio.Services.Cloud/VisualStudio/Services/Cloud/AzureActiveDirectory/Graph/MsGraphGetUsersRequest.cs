// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetUsersRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MsGraphGetUsersRequest : MicrosoftGraphClientPagedRequest<MsGraphGetUsersResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetUsersRequest";

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> SurnamePrefixes { get; set; }

    public IEnumerable<string> MailPrefixes { get; set; }

    public IEnumerable<string> MailNicknamePrefixes { get; set; }

    public IEnumerable<string> UserPrincipalNamePrefixes { get; set; }

    public IEnumerable<string> OnPremiseSecurityIdentifiers { get; set; }

    public IEnumerable<string> ImmutableIds { get; set; }

    public string ExpandProperty { get; set; }

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.SurnamePrefixes, this.MailPrefixes, this.MailNicknamePrefixes, this.UserPrincipalNamePrefixes, this.OnPremiseSecurityIdentifiers, this.ImmutableIds);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override MsGraphGetUsersResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        IGraphServiceUsersCollectionRequest usersRequest;
        if (this.PagingToken == null)
        {
          string expression = this.BuildFilterExpression();
          ParameterExpression parameterExpression;
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          usersRequest = graphServiceClient.Users.Request().Filter(expression).Select(Expression.Lambda<Func<User, object>>((Expression) Expression.New((ConstructorInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.\u002Ector), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (IEnumerable<Expression>) new Expression[20]
          {
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Entity.get_Id))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_DisplayName))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_Mail))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_OtherMails))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_MailNickname))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_UserPrincipalName))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_JobTitle))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_Department))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_Manager))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_DirectReports))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_OfficeLocation))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_Surname))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_UserType))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_ExternalUserState))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_OnPremisesSecurityIdentifier))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_OnPremisesImmutableId))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_BusinessPhones))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_Country))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_UsageLocation))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (User.get_AccountEnabled)))
          }, (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_Id), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_DisplayName), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_Mail), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_OtherMails), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_MailNickname), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_UserPrincipalName), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_JobTitle), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_Department), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_Manager), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_DirectReports), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_OfficeLocation), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_Surname), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_UserType), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_ExternalUserState), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_OnPremisesSecurityIdentifier), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_OnPremisesImmutableId), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_BusinessPhones), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_Country), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_UsageLocation), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>.get_AccountEnabled), __typeref (\u003C\u003Ef__AnonymousType2<string, string, string, IEnumerable<string>, string, string, string, string, DirectoryObject, IUserDirectReportsCollectionWithReferencesPage, string, string, string, string, string, string, IEnumerable<string>, string, string, bool?>))), parameterExpression));
          if (this.PageSize.HasValue)
            usersRequest = usersRequest.Top(this.PageSize.Value);
          if (this.ExpandProperty == "Manager")
          {
            usersRequest = usersRequest.Expand("manager");
            context.Trace(44750024, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUsersRequest), "Usage of 'Manager' expand on 'GetUsers' Request");
          }
          context.TraceConditionally(44750020, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUsersRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Users with filter = '" + expression + "'"));
        }
        else
        {
          usersRequest = (IGraphServiceUsersCollectionRequest) new GraphServiceUsersCollectionRequest(this.GetSecureNextRequestUrlFromPageToken(graphServiceClient), (IBaseClient) graphServiceClient, (IEnumerable<Option>) null);
          context.TraceConditionally(44750021, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUsersRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Users with pagingToken = '" + MicrosoftGraphUtils.GetSkipTokenFromGraphRequest((IBaseRequest) usersRequest) + "'"));
        }
        IGraphServiceUsersCollectionPage source = context.RunSynchronously<IGraphServiceUsersCollectionPage>((Func<Task<IGraphServiceUsersCollectionPage>>) (() => usersRequest.GetAsync(new CancellationToken())));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<AadUser> aadUsers = source != null ? ((IEnumerable<User>) source).Select<User, AadUser>(MsGraphGetUsersRequest.\u003C\u003EO.\u003C0\u003E__ConvertUser ?? (MsGraphGetUsersRequest.\u003C\u003EO.\u003C0\u003E__ConvertUser = new Func<User, AadUser>(MicrosoftGraphConverters.ConvertUser))) : throw new MicrosoftGraphException("Microsoft Graph API Get Users call returned null");
        MsGraphGetUsersResponse getUsersResponse = new MsGraphGetUsersResponse();
        getUsersResponse.Users = aadUsers;
        getUsersResponse.PagingToken = MicrosoftGraphUtils.GetGraphPageNextLink(context, (IBaseRequest) usersRequest, (IBaseRequest) source.NextPageRequest);
        return getUsersResponse;
      }
      finally
      {
        context.Trace(44750023, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetUsersRequest), "Leaving Microsoft Graph API for Get Users");
      }
    }

    private string BuildFilterExpression()
    {
      MsGraphFilterBuilder graphFilterBuilder = new MsGraphFilterBuilder();
      if (this.DisplayNamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.DisplayNamePrefixes, "displayName");
      if (this.SurnamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.SurnamePrefixes, "surname");
      if (this.MailPrefixes != null)
      {
        graphFilterBuilder.WithSearchPrefix(this.MailPrefixes, "mail");
        graphFilterBuilder.WithCustomSearchParam(this.MailPrefixes, (Func<string, string>) (searchPrefix => "startswith(mail,'" + AadQueryUtils.SanitizeInput(searchPrefix) + "')"));
      }
      if (this.MailNicknamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.MailNicknamePrefixes, "mailNickname");
      if (this.UserPrincipalNamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.UserPrincipalNamePrefixes, "userPrincipalName");
      if (this.OnPremiseSecurityIdentifiers != null)
        graphFilterBuilder.WithSearchEqualByString(this.OnPremiseSecurityIdentifiers, "onPremisesSecurityIdentifier");
      if (this.ImmutableIds != null)
        graphFilterBuilder.WithSearchEqualByString(this.ImmutableIds, "onPremisesImmutableId");
      return graphFilterBuilder.BuildFilter();
    }

    public override string ToString() => string.Format("GetUsersRequest{{DisplayNamePrefixes={0},SurnamePrefixes={1},MailPrefixes={2},MailNicknamePrefixes={3},UserPrincipalNamePrefixes={4},OnPremiseSecurityIdentifiers={5},ImmutableIds={6},ExpandProperty={7}}}", (object) AadQueryUtils.ToString(this.DisplayNamePrefixes), (object) AadQueryUtils.ToString(this.SurnamePrefixes), (object) AadQueryUtils.ToString(this.MailPrefixes), (object) AadQueryUtils.ToString(this.MailNicknamePrefixes), (object) AadQueryUtils.ToString(this.UserPrincipalNamePrefixes), (object) AadQueryUtils.ToString(this.OnPremiseSecurityIdentifiers), (object) AadQueryUtils.ToString(this.ImmutableIds), (object) this.ExpandProperty);
  }
}
