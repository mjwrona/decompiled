// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.PrefixAzureBlobGeoRedundancyExtension
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class PrefixAzureBlobGeoRedundancyExtension : IAzureBlobGeoRedundancyExtension
  {
    private readonly string m_drawerName;
    private readonly string m_primaryStorageAccountPrefix;
    private readonly string m_secondaryStorageAccountPrefix;

    public PrefixAzureBlobGeoRedundancyExtension(
      string drawerName,
      string primaryStorageAccountPrefix,
      string secondaryStorageAccountPrefix)
    {
      ArgumentUtility.CheckForNull<string>(drawerName, nameof (drawerName));
      ArgumentUtility.CheckForNull<string>(primaryStorageAccountPrefix, nameof (primaryStorageAccountPrefix));
      ArgumentUtility.CheckForNull<string>(secondaryStorageAccountPrefix, nameof (secondaryStorageAccountPrefix));
      this.m_drawerName = drawerName;
      this.m_primaryStorageAccountPrefix = primaryStorageAccountPrefix;
      this.m_secondaryStorageAccountPrefix = secondaryStorageAccountPrefix;
    }

    public string DrawerName => this.m_drawerName;

    public string PrimaryStorageAccountPrefix => this.m_primaryStorageAccountPrefix;

    public string SecondaryStorageAccountPrefix => this.m_secondaryStorageAccountPrefix;

    public virtual IEnumerable<GeoRedundantStorageAccountSettings> GetGeoRedundantStorageAccounts(
      IVssRequestContext requestContext)
    {
      List<GeoRedundantStorageAccountSettings> redundantStorageAccounts = new List<GeoRedundantStorageAccountSettings>();
      int num = 0;
      while (true)
      {
        string lookupKey1 = string.Format("{0}{1}", (object) this.m_primaryStorageAccountPrefix, (object) num);
        if (AzureBlobGeoRedundancyUtils.StorageAccountConnectionStringExists(requestContext, this.m_drawerName, lookupKey1))
        {
          string lookupKey2 = string.Format("{0}{1}", (object) this.m_secondaryStorageAccountPrefix, (object) num);
          if (AzureBlobGeoRedundancyUtils.StorageAccountConnectionStringExists(requestContext, this.m_drawerName, lookupKey2))
            redundantStorageAccounts.Add(new GeoRedundantStorageAccountSettings()
            {
              DrawerName = this.m_drawerName,
              PrimaryLookupKey = lookupKey1,
              SecondaryLookupKey = lookupKey2
            });
          else
            requestContext.Trace(15304000, TraceLevel.Warning, this.Area, this.Layer, "Found primary storage account but no secondary storage account was configured. LookupKey: {0}", (object) lookupKey2);
          ++num;
        }
        else
          break;
      }
      return (IEnumerable<GeoRedundantStorageAccountSettings>) redundantStorageAccounts;
    }

    protected virtual string Area => "AzureBlobGeoRedundancy";

    protected virtual string Layer => nameof (PrefixAzureBlobGeoRedundancyExtension);
  }
}
