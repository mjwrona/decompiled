// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DisplayVersionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Health;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class DisplayVersionService : IDisplayVersionService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public string GetReleaseVersion(IVssRequestContext requestContext)
    {
      string releaseVersion = TeamFoundationSqlResourceComponent.CurrentServiceLevel.ToString();
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        releaseVersion = ReleaseManifest.LoadFrom(Path.Combine(OnPremRegistryUtil.InstallPath, "Tools\\Deploy\\TfsServicingFiles\\ReleaseManifest.xml")).CurrentRelease.UpdatePackages.Last<UpdatePackage>().Description;
      BuildInfo[] buildInfo = BuildInfoReader.GetBuildInfo();
      if (buildInfo != null && buildInfo.Length != 0)
        releaseVersion = releaseVersion + " (" + buildInfo[0].BuildNumber + ")";
      return releaseVersion;
    }
  }
}
