// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.LicensingHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public class LicensingHelper : ILicensingHelper
  {
    private const string c_area = "LicensingHelper";
    private const string c_layer = "DataImport";

    public IEnumerable<AccountEntitlement> GetAccountEntitlements(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      return (IEnumerable<AccountEntitlement>) requestContext.GetService<ILicensingEntitlementService>().GetAccountEntitlements(requestContext);
    }

    public virtual IEnumerable<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      Guid collectionHostId)
    {
      requestContext.CheckDeploymentRequestContext();
      ArgumentUtility.CheckForEmptyGuid(collectionHostId, nameof (collectionHostId));
      try
      {
        return requestContext.GetAccountLicensingHttpClient(collectionHostId).GetAccountEntitlementsAsync().SyncResult<IEnumerable<AccountEntitlement>>();
      }
      catch (HostDoesNotExistException ex)
      {
        requestContext.Trace(610013, TraceLevel.Info, nameof (LicensingHelper), "DataImport", string.Format("Could not find sps definition for hostId: {0}", (object) collectionHostId));
        return (IEnumerable<AccountEntitlement>) null;
      }
      catch (VssServiceResponseException ex)
      {
        requestContext.Trace(610014, TraceLevel.Info, nameof (LicensingHelper), "DataImport", ex.ToReadableStackTrace());
        if (ex.HttpStatusCode == HttpStatusCode.ServiceUnavailable || ex.HttpStatusCode == HttpStatusCode.NotFound)
          return (IEnumerable<AccountEntitlement>) null;
        throw;
      }
    }

    public int GetNonStakeholderLicenseCount(IVssRequestContext requestContext, ITFLogger logger = null) => this.GetNonStakeholderLicenseCount(this.GetAccountEntitlements(requestContext), logger);

    public virtual int GetNonStakeholderLicenseCount(
      IEnumerable<AccountEntitlement> accountEntitlements,
      ITFLogger logger = null)
    {
      if (accountEntitlements.IsNullOrEmpty<AccountEntitlement>())
        return 0;
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      IEnumerable<IGrouping<License, AccountEntitlement>> groupings = accountEntitlements.GroupBy<AccountEntitlement, License>((Func<AccountEntitlement, License>) (x => x.License));
      int stakeholderLicenseCount = 0;
      foreach (IGrouping<License, AccountEntitlement> source in groupings)
      {
        int num = source.Count<AccountEntitlement>();
        AccountLicense key = source.Key as AccountLicense;
        string str = !((License) key == (License) null) ? string.Format("Account-{0}", (object) key.License) : source.Key.GetType().Name;
        if ((License) key != (License) null && AccountLicense.Stakeholder.Equals((License) key))
        {
          logger.Info(string.Format("Found {0} stakeholder licenses as {1}", (object) num, (object) str));
        }
        else
        {
          stakeholderLicenseCount += num;
          logger.Info(string.Format("Found {0} licenses as {1}", (object) num, (object) str));
        }
      }
      logger.Info(string.Format("Found {0} non-Stakeholder licenses", (object) stakeholderLicenseCount));
      return stakeholderLicenseCount;
    }
  }
}
