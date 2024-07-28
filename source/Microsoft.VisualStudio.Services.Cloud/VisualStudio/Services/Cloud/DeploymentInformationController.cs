// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.DeploymentInformationController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "Migration", ResourceName = "DeploymentInformation")]
  public class DeploymentInformationController : TfsApiController
  {
    [HttpGet]
    public DeploymentInformation GetDeploymentInformation()
    {
      HostMigrationUtil.CheckPermission(this.TfsRequestContext, HostMigrationPermissions.Read);
      IEnumerable<IPAddress> outboundViPs = this.TfsRequestContext.GetService<IDeploymentInformationService>().GetOutboundVIPs(this.TfsRequestContext);
      bool flag = HostMigrationUtil.IsMigrationEnabled(this.TfsRequestContext, false, out string _);
      string str = this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Cloud.HostMigration.MigrationCertSignedSecrets") ? MigrationRegistryUtil.GetDefaultMigrationCertificate(this.TfsRequestContext) : string.Empty;
      return new DeploymentInformation()
      {
        OutboundVIPs = outboundViPs.Select<IPAddress, string>((Func<IPAddress, string>) (x => x.ToString())).ToArray<string>(),
        MigrationEnabled = flag,
        MigrationCertificateThumbprint = str
      };
    }
  }
}
