// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.Account
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public class Account : AccountKey
  {
    public static readonly AccountMemberComparer AccountMemberComparer = new AccountMemberComparer();

    public Account(AccountInitializationData initializationData)
      : base(initializationData.UniqueId, initializationData.ParentProviderId)
    {
      ArgumentUtility.CheckForNull<AccountDisplayInfo>(initializationData.DisplayInfo, "displayInfo");
      this.DisplayInfo = initializationData.DisplayInfo;
      ArgumentUtility.CheckStringForNullOrEmpty(initializationData.Authenticator, "authenticator");
      this.Authenticator = initializationData.Authenticator;
      ArgumentUtility.CheckForNull<IReadOnlyList<Guid>>(initializationData.SupportedAccountProviders, "supportedAccountProviders");
      this.SupportedAccountProviders = initializationData.SupportedAccountProviders;
      foreach (Guid supportedAccountProvider in (IEnumerable<Guid>) this.SupportedAccountProviders)
        ArgumentUtility.CheckForEmptyGuid(supportedAccountProvider, "supportedAccountProviders");
      ArgumentUtility.CheckForNull<IReadOnlyDictionary<string, string>>(initializationData.Properties, "properties");
      this.Properties = initializationData.Properties;
      this.NeedsReauthentication = initializationData.NeedsReauthentication;
    }

    public string Authenticator { get; private set; }

    public IReadOnlyList<Guid> SupportedAccountProviders { get; private set; }

    public AccountDisplayInfo DisplayInfo { get; private set; }

    public IReadOnlyDictionary<string, string> Properties { get; private set; }

    public bool NeedsReauthentication { get; private set; }

    internal AccountInitializationData InitializationDataFromAccount() => new AccountInitializationData()
    {
      Authenticator = this.Authenticator,
      DisplayInfo = this.DisplayInfo,
      NeedsReauthentication = this.NeedsReauthentication,
      ParentProviderId = this.ProviderId,
      SupportedAccountProviders = this.SupportedAccountProviders,
      UniqueId = this.UniqueId,
      Properties = this.Properties
    };
  }
}
