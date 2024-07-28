// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.UserProfileInformationModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class UserProfileInformationModel
  {
    public UserProfileInformationModel()
    {
    }

    public UserProfileInformationModel(TfsWebContext webContext)
    {
      IVssRequestContext tfsRequestContext = webContext.TfsRequestContext;
      TeamFoundationIdentityService service = tfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(tfsRequestContext, new IdentityDescriptor[1]
      {
        tfsRequestContext.UserContext
      }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[2]
      {
        "CustomNotificationAddresses",
        "ConfirmedNotificationAddress"
      })[0];
      this.CustomDisplayName = readIdentity.CustomDisplayName;
      this.ProviderDisplayName = readIdentity.ProviderDisplayName;
      object obj = (object) null;
      if (readIdentity.TryGetProperty(IdentityPropertyScope.Global, "CustomNotificationAddresses", out obj))
      {
        this.MailAddress = obj as string;
        this.IsEmailConfirmationPending = service.IsEmailConfirmationPending(tfsRequestContext, readIdentity.TeamFoundationId);
      }
      if (tfsRequestContext.ExecutionEnvironment.IsHostedDeployment && string.IsNullOrEmpty(this.MailAddress) && readIdentity.TryGetProperty(IdentityPropertyScope.Global, "ConfirmedNotificationAddress", out obj))
      {
        this.MailAddress = obj as string;
        this.IsEmailConfirmationPending = false;
      }
      if (string.IsNullOrEmpty(this.MailAddress))
        this.MailAddress = readIdentity.GetAttribute("Mail", string.Empty);
      this.IdentityInformation = IdentityImageUtility.GetIdentityViewModel(readIdentity);
    }

    public string MailAddress { get; set; }

    public string CustomDisplayName { get; set; }

    public string ProviderDisplayName { get; set; }

    public bool IsEmailConfirmationPending { get; set; }

    public IdentityViewModelBase IdentityInformation { get; set; }
  }
}
