// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigrationCertificateRotationExtension
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class HostMigrationCertificateRotationExtension : IJobApplicationExtension
  {
    private static readonly string s_area = "HostMigrationCertificate";
    private static readonly string s_layer = nameof (HostMigrationCertificateRotationExtension);
    private static readonly Guid s_HostMigrationCertificateRotationManagementJobId = new Guid("A161DB87-D2CD-48D8-9307-272553489529");

    public void Start(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnCertificateChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        FrameworkServerConstants.MigrationCertificateStaging
      });
    }

    public void Stop(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnCertificateChanged));

    internal void OnCertificateChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      using (requestContext.TraceBlock(109400, 109401, HostMigrationCertificateRotationExtension.s_area, HostMigrationCertificateRotationExtension.s_layer, nameof (OnCertificateChanged)))
      {
        requestContext.CheckDeploymentRequestContext();
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) itemNames, nameof (itemNames));
        StrongBoxItemName strongBoxItemName = itemNames.Single<StrongBoxItemName>();
        if (!string.Equals(strongBoxItemName.LookupKey, FrameworkServerConstants.MigrationCertificateStaging))
          throw new ArgumentException("OnCertificateChanged was invoked for an unexpected item: " + strongBoxItemName.LookupKey, nameof (itemNames));
        requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          HostMigrationCertificateRotationExtension.s_HostMigrationCertificateRotationManagementJobId
        });
      }
    }
  }
}
