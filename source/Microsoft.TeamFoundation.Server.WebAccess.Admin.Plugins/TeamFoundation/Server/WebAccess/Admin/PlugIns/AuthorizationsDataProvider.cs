// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.AuthorizationsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Navigation;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class AuthorizationsDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.AuthorizationsView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      AuthorizationsViewData data1;
      try
      {
        IEnumerable<AuthorizationDetails> authorizations = requestContext.GetService<IDelegatedAuthorizationService>().GetAuthorizations(requestContext, requestContext.GetUserId());
        IList<AuthorizationViewModel> authorizationViewModelList = (IList<AuthorizationViewModel>) new List<AuthorizationViewModel>();
        IList<AuthorizationViewModel> list = (IList<AuthorizationViewModel>) authorizations.GroupBy<AuthorizationDetails, string, AuthorizationViewModel>((Func<AuthorizationDetails, string>) (auth => string.Format("{0}/{1}", (object) auth.ClientRegistration.RegistrationId, (object) auth.Authorization.Scopes)), (Func<string, IEnumerable<AuthorizationDetails>, AuthorizationViewModel>) ((clientAppId, clientAppAuthorizations) =>
        {
          AuthorizationViewModel data2 = new AuthorizationViewModel()
          {
            Authorization = clientAppAuthorizations.First<AuthorizationDetails>()
          };
          if (clientAppAuthorizations.Count<AuthorizationDetails>() > 1)
            data2.RelatedAuthorizationIds = clientAppAuthorizations.Skip<AuthorizationDetails>(1).Select<AuthorizationDetails, Guid>((Func<AuthorizationDetails, Guid>) (details => details.Authorization.AuthorizationId)).ToList<Guid>();
          return data2;
        })).ToList<AuthorizationViewModel>();
        string authRegisterAppUrl = UserSettingsContext.GetAexAuthRegisterAppUrl(requestContext);
        data1 = new AuthorizationsViewData()
        {
          Authorizations = (IEnumerable<AuthorizationViewModel>) list,
          AuthRegisterAppUrl = authRegisterAppUrl
        };
      }
      catch (Exception ex)
      {
        data1 = (AuthorizationsViewData) null;
        requestContext.TraceException(10050073, "ProfileSettings", "DataProvider", ex);
      }
      return (object) data1;
    }
  }
}
