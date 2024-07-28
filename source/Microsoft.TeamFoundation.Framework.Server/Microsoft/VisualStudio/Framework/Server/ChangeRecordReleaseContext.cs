// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Framework.Server.ChangeRecordReleaseContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Framework.Server
{
  public class ChangeRecordReleaseContext
  {
    public string ReleaseId { get; private set; }

    public string ReleaseName { get; private set; }

    public string ReleaseDescription { get; private set; }

    public string QueuedBy { get; private set; }

    public string BuildNumber { get; private set; }

    public string DeploymentRing { get; private set; }

    public string IpAddress { get; private set; }

    public DateTimeOffset ReleaseStartTime { get; private set; }

    public ChangeRecordReleaseContext()
      : this((string) null, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, DateTimeOffset.MinValue)
    {
    }

    public ChangeRecordReleaseContext(
      string releaseId,
      string releaseName,
      string releaseDescription,
      string queuedBy,
      string buildNumber,
      string deploymentRing,
      string ipAddress,
      DateTimeOffset startTime)
    {
      this.ReleaseId = releaseId;
      this.ReleaseName = releaseName;
      this.ReleaseDescription = releaseDescription;
      this.QueuedBy = queuedBy;
      this.BuildNumber = buildNumber;
      this.DeploymentRing = deploymentRing;
      this.IpAddress = ipAddress;
      this.ReleaseStartTime = startTime;
    }

    public static ChangeRecordReleaseContext GetRmoReleaseContext(
      string ipAddress,
      string deploymentRing,
      string queuedOnBehalfOf = null)
    {
      string environmentVariable1 = Environment.GetEnvironmentVariable("RELEASE_RELEASENAME");
      string str1 = Environment.GetEnvironmentVariable("RELEASE_RELEASEDESCRIPTION") + " - " + Environment.GetEnvironmentVariable("RELEASE_ENVIRONMENTNAME");
      string str2 = queuedOnBehalfOf ?? Environment.GetEnvironmentVariable("RELEASE_REQUESTEDFOR");
      string environmentVariable2 = Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER");
      string environmentVariable3 = Environment.GetEnvironmentVariable("RELEASE_RELEASEID");
      DateTimeOffset result;
      if (!DateTimeOffset.TryParse(Environment.GetEnvironmentVariable("RELEASE_DEPLOYMENT_STARTTIME"), out result))
        result = (DateTimeOffset) DateTime.UtcNow;
      string releaseName = environmentVariable1;
      string releaseDescription = str1;
      string queuedBy = str2;
      string buildNumber = environmentVariable2;
      string deploymentRing1 = deploymentRing;
      string ipAddress1 = ipAddress;
      DateTimeOffset startTime = result;
      return new ChangeRecordReleaseContext(environmentVariable3, releaseName, releaseDescription, queuedBy, buildNumber, deploymentRing1, ipAddress1, startTime);
    }

    public static ChangeRecordReleaseContext GetRmoReleaseContext() => ChangeRecordReleaseContext.GetRmoReleaseContext((string) null, (string) null);
  }
}
