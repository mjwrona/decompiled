// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphGetServicePrincipalsRequest
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
  public class MsGraphGetServicePrincipalsRequest : 
    MicrosoftGraphClientPagedRequest<MsGraphGetServicePrincipalsResponse>
  {
    protected const string TraceArea = "MicrosoftGraphClientRequest";
    protected const string TraceLayer = "MsGraphGetServicePrincipalsRequest";

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> AppIds { get; set; }

    internal override MsGraphGetServicePrincipalsResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        IGraphServiceServicePrincipalsCollectionRequest servicePrincpalsRequest;
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
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          // ISSUE: method reference
          // ISSUE: type reference
          servicePrincpalsRequest = graphServiceClient.ServicePrincipals.Request().Filter(expression).Select(Expression.Lambda<Func<ServicePrincipal, object>>((Expression) Expression.New((ConstructorInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>.\u002Ector), __typeref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>)), (IEnumerable<Expression>) new Expression[4]
          {
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Entity.get_Id))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ServicePrincipal.get_DisplayName))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ServicePrincipal.get_AppId))),
            (Expression) Expression.Property((Expression) parameterExpression, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ServicePrincipal.get_AccountEnabled)))
          }, (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>.get_Id), __typeref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>.get_DisplayName), __typeref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>.get_AppId), __typeref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>)), (MemberInfo) MethodBase.GetMethodFromHandle(__methodref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>.get_AccountEnabled), __typeref (\u003C\u003Ef__AnonymousType0<string, string, string, bool?>))), parameterExpression));
          if (this.PageSize.HasValue)
            servicePrincpalsRequest = servicePrincpalsRequest.Top(this.PageSize.Value);
          context.TraceConditionally(44750025, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetServicePrincipalsRequest), (Func<string>) (() => "Calling Microsoft Graph API for Get Service Principals with filter = '" + expression + "'"));
        }
        else
        {
          servicePrincpalsRequest = (IGraphServiceServicePrincipalsCollectionRequest) new GraphServiceServicePrincipalsCollectionRequest(this.GetSecureNextRequestUrlFromPageToken(graphServiceClient), (IBaseClient) graphServiceClient, (IEnumerable<Option>) null);
          context.TraceConditionally(44750026, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetServicePrincipalsRequest), (Func<string>) (() =>
          {
            IList<QueryOption> queryOptions = ((IBaseRequest) servicePrincpalsRequest).QueryOptions;
            return "Calling Microsoft Graph API for Get Service Principals with pagingToken = '" + (queryOptions != null ? ((Option) queryOptions.FirstOrDefault<QueryOption>((Func<QueryOption, bool>) (x => string.Equals(((Option) x).Name, "$skiptoken", StringComparison.OrdinalIgnoreCase))))?.Value : (string) null) + "'";
          }));
        }
        IGraphServiceServicePrincipalsCollectionPage source = context.RunSynchronously<IGraphServiceServicePrincipalsCollectionPage>((Func<Task<IGraphServiceServicePrincipalsCollectionPage>>) (() => servicePrincpalsRequest.GetAsync(new CancellationToken())));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<AadServicePrincipal> servicePrincipals = source != null ? ((IEnumerable<ServicePrincipal>) source).Select<ServicePrincipal, AadServicePrincipal>(MsGraphGetServicePrincipalsRequest.\u003C\u003EO.\u003C0\u003E__ConvertServicePrincipal ?? (MsGraphGetServicePrincipalsRequest.\u003C\u003EO.\u003C0\u003E__ConvertServicePrincipal = new Func<ServicePrincipal, AadServicePrincipal>(MicrosoftGraphConverters.ConvertServicePrincipal))) : throw new AadGraphException("Microsoft Graph API Get Service Principals call returned null");
        MsGraphGetServicePrincipalsResponse principalsResponse = new MsGraphGetServicePrincipalsResponse();
        principalsResponse.ServicePrincipals = servicePrincipals;
        principalsResponse.PagingToken = MicrosoftGraphUtils.GetGraphPageNextLink(context, (IBaseRequest) servicePrincpalsRequest, (IBaseRequest) source.NextPageRequest);
        return principalsResponse;
      }
      finally
      {
        context.Trace(44750027, TraceLevel.Info, "MicrosoftGraphClientRequest", nameof (MsGraphGetServicePrincipalsRequest), "Leaving Microsoft Graph API for Get Service Principals");
      }
    }

    public override string ToString() => string.Format("GetServicePrincipalsRequest: DisplayNamePrefixes={0}; AppIds={1}", (object) this.DisplayNamePrefixes, (object) this.AppIds);

    private string BuildFilterExpression()
    {
      MsGraphFilterBuilder graphFilterBuilder = new MsGraphFilterBuilder();
      if (this.DisplayNamePrefixes != null)
        graphFilterBuilder.WithSearchPrefix(this.DisplayNamePrefixes, "displayName");
      if (this.AppIds != null)
        graphFilterBuilder.WithSearchEqualByGuid(this.AppIds, "appId");
      return graphFilterBuilder.BuildFilter();
    }
  }
}
