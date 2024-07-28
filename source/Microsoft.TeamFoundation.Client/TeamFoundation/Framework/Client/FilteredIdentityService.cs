// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.FilteredIdentityService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FilteredIdentityService : ITfsConnectionObject
  {
    private TfsConnection m_tfsConnection;

    internal FilteredIdentityService()
    {
    }

    public void Initialize(TfsConnection server) => this.m_tfsConnection = server;

    public IList<TeamFoundationIdentity> SearchForUsers(string searchTerm)
    {
      FilteredIdentitiesList identityList = this.m_tfsConnection.GetService<IIdentityManagementService2>().ReadFilteredIdentities(string.Format("Microsoft.TeamFoundation.Identity.DisplayName CONTAINS '{0}' AND Microsoft.TeamFoundation.Identity.Type == 'User'", (object) searchTerm), 500, (string) null, true, 0);
      this.ProcessIdentities(identityList);
      return (IList<TeamFoundationIdentity>) identityList.Items;
    }

    private void MakeDisplayNamesUnique(List<TeamFoundationIdentity> identities)
    {
      foreach (TeamFoundationIdentity identity in identities)
      {
        string domainUserName = IdentityHelper.GetDomainUserName(identity);
        identity.DisplayName = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} <{1}>", (object) identity.DisplayName, (object) domainUserName);
      }
    }

    private void ProcessIdentities(FilteredIdentitiesList identityList)
    {
      TeamFoundationIdentity foundationIdentity1 = (TeamFoundationIdentity) null;
      List<TeamFoundationIdentity> identities = new List<TeamFoundationIdentity>();
      foreach (TeamFoundationIdentity foundationIdentity2 in identityList.Items)
      {
        if (foundationIdentity1 == null || !foundationIdentity2.DisplayName.Equals(foundationIdentity1.DisplayName, StringComparison.OrdinalIgnoreCase))
        {
          if (identities.Count > 1)
            this.MakeDisplayNamesUnique(identities);
          identities.Clear();
        }
        identities.Add(foundationIdentity2);
        foundationIdentity1 = foundationIdentity2;
      }
      if (identities.Count <= 1)
        return;
      this.MakeDisplayNamesUnique(identities);
    }
  }
}
