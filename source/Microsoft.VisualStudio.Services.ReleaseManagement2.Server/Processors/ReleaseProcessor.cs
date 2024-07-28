// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseProcessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class ReleaseProcessor
  {
    private readonly Dictionary<Tuple<ReleaseStatus, ReleaseStatus>, Func<Release, string, Release>> releaseOperationMap;

    public ReleaseProcessor(IVssRequestContext context, Guid projectId)
      : this(ReleaseStatusResolver.GetReleaseOperationsMap(context, projectId))
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Requires for tests")]
    protected ReleaseProcessor(
      Dictionary<Tuple<ReleaseStatus, ReleaseStatus>, Func<Release, string, Release>> releaseOperationMap)
    {
      this.releaseOperationMap = releaseOperationMap;
    }

    public Release UpdateReleaseStatus(Release release, ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      if (release == null || releaseUpdateMetadata == null || releaseUpdateMetadata.Status == ReleaseStatus.Undefined)
        throw new InvalidRequestException(Resources.ProvideReleaseStatus);
      if (!ReleaseStatusResolver.IsReleaseStatusUpdateAllowed(release.Status, releaseUpdateMetadata.Status))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseStatusUpdateNotAllowed, (object) release.Status, (object) releaseUpdateMetadata.Status));
      return this.releaseOperationMap[Tuple.Create<ReleaseStatus, ReleaseStatus>(release.Status, releaseUpdateMetadata.Status)](release, releaseUpdateMetadata.Comment);
    }
  }
}
