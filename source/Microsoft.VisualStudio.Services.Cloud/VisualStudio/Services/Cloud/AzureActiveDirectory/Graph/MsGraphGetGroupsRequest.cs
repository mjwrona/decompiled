// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetGroupsRequest
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
  public class MsGraphGetGroupsRequest : MicrosoftGraphClientPagedRequest<MsGraphGetGroupsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetGroupsRequest";
    private const string TrueString = "true";

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> MailNicknamePrefixes { get; set; }

    public IEnumerable<string> MailPrefixes { get; set; }

    public IEnumerable<string> OnPremisesSecurityIdentifiers { get; set; }

    public bool IncludeDistributionGroups { get; set; } = true;

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.MailNicknamePrefixes, this.MailPrefixes, this.OnPremisesSecurityIdentifiers);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override MsGraphGetGroupsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        IGraphServiceGroupsCollectionRequest groupsRequest;
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
          groupsRequest = graphServiceClient.Groups.Request().Filter(expression).Select(Expression.Lambda<Func<Group, object>>((Expression) Expression.New((ConstructorInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.\u002Ector), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>)), (IEnumerable<Expression>) new Expression[6]
          {
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Entity.get_Id))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Group.get_Description))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Group.get_DisplayName))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Group.get_MailNickname))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Group.get_Mail))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Group.get_OnPremisesSecurityIdentifier)))
          }, (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.get_Id), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.get_Description), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.get_DisplayName), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.get_MailNickname), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.get_Mail), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>.get_OnPremisesSecurityIdentifier), __typeref (\u003C\u003Ef__AnonymousType1<string, string, string, string, string, string>))), parameterExpression));
          if (this.PageSize.HasValue)
            groupsRequest = groupsRequest.Top(this.PageSize.Value);
          context.TraceConditionally(44750030, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetGroupsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Groups with filter = '" + expression + "'"));
        }
        else
        {
          groupsRequest = (IGraphServiceGroupsCollectionRequest) new GraphServiceGroupsCollectionRequest(this.GetSecureNextRequestUrlFromPageToken(graphServiceClient), (IBaseClient) graphServiceClient, (IEnumerable<Option>) null);
          context.TraceConditionally(44750031, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetGroupsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Groups with pagingToken = '" + MicrosoftGraphUtils.GetSkipTokenFromGraphRequest((IBaseRequest) groupsRequest) + "'"));
        }
        IGraphServiceGroupsCollectionPage source = context.RunSynchronously<IGraphServiceGroupsCollectionPage>((Func<Task<IGraphServiceGroupsCollectionPage>>) (() => groupsRequest.GetAsync(new CancellationToken())));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<AadGroup> aadGroups = source != null ? ((IEnumerable<Group>) source).Select<Group, AadGroup>(MsGraphGetGroupsRequest.\u003C\u003EO.\u003C0\u003E__ConvertGroup ?? (MsGraphGetGroupsRequest.\u003C\u003EO.\u003C0\u003E__ConvertGroup = new Func<Group, AadGroup>(MicrosoftGraphConverters.ConvertGroup))) : throw new MicrosoftGraphException("Microsoft Graph API Get Groups call returned null");
        MsGraphGetGroupsResponse getGroupsResponse = new MsGraphGetGroupsResponse();
        getGroupsResponse.Groups = aadGroups;
        getGroupsResponse.PagingToken = MicrosoftGraphUtils.GetGraphPageNextLink(context, (IBaseRequest) groupsRequest, (IBaseRequest) source.NextPageRequest);
        return getGroupsResponse;
      }
      finally
      {
        context.Trace(44750032, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetGroupsRequest), "Leaving Microsoft Graph API for Get Groups");
      }
    }

    private string BuildFilterExpression()
    {
      MsGraphFilterBuilder graphFilterBuilder = new MsGraphFilterBuilder();
      if (this.DisplayNamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.DisplayNamePrefixes, "displayName");
      if (this.MailNicknamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.MailNicknamePrefixes, "mailNickname");
      if (this.MailPrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.MailPrefixes, "mail");
      if (this.OnPremisesSecurityIdentifiers != null)
        graphFilterBuilder.WithSearchEqualByString(this.OnPremisesSecurityIdentifiers, "onPremisesSecurityIdentifier");
      if (!this.IncludeDistributionGroups)
        graphFilterBuilder.WithSingleSearchEqualByBooleanExpressionAnd("true", "securityEnabled");
      return graphFilterBuilder.BuildFilter();
    }

    public override string ToString() => string.Format("GetGroupsRequest{{DisplayNamePrefixes={0},MailNicknamePrefixes={1},MailPrefixes={2},OnPremisesSecurityIdentifiers={3},IncludeDistributionGroups={4}}}", (object) AadQueryUtils.ToString(this.DisplayNamePrefixes), (object) AadQueryUtils.ToString(this.MailNicknamePrefixes), (object) AadQueryUtils.ToString(this.MailPrefixes), (object) AadQueryUtils.ToString(this.OnPremisesSecurityIdentifiers), (object) this.IncludeDistributionGroups);
  }
}
