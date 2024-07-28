// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProjectSettings.TeamProjectSettings
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Common.SharePoint;
using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.ProjectSettings
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TeamProjectSettings
  {
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public Uri ProjectUri { get; private set; }

    public ProjectState State { get; private set; }

    public Uri Portal { get; private set; }

    public bool PortalIsSharePoint { get; private set; }

    public bool IsOwnerOfSharePointPortal { get; private set; }

    public Uri Guidance { get; private set; }

    public string GuidanceFileName { get; private set; }

    public ProcessGuidanceType GuidanceResourceType { get; private set; }

    public string ReportFolder { get; private set; }

    internal string CatalogNodeFullPath { get; private set; }

    public Uri WellKnownGuidancePageUrl => SharePointUtilities.GetWellKnownProcessGuidancePageUrl(this.GuidanceResourceType, this.Guidance, this.GuidanceFileName);

    public string SccCapFlag { get; private set; }

    public string SupportsGit { get; private set; }

    public string SupportsTfvc { get; private set; }

    internal TeamProjectSettings(
      Guid id,
      string name,
      Uri projectUri,
      ProjectState state,
      Uri portalUri,
      bool portalIsSharePoint,
      bool ownsPortal,
      Uri guidanceUri,
      string guidanceFileName,
      ProcessGuidanceType guidanceType,
      string reportFolder,
      string catalogNodePath,
      string sccCapFlag,
      string supportsGit,
      string supportsTfvc)
    {
      this.Id = id;
      this.Name = name;
      this.ProjectUri = projectUri;
      this.State = state;
      this.Portal = portalUri;
      this.PortalIsSharePoint = portalIsSharePoint;
      this.IsOwnerOfSharePointPortal = ownsPortal;
      this.Guidance = guidanceUri;
      this.GuidanceFileName = guidanceFileName;
      this.GuidanceResourceType = guidanceType;
      this.ReportFolder = reportFolder;
      this.CatalogNodeFullPath = catalogNodePath;
      this.SccCapFlag = sccCapFlag;
      this.SupportsGit = supportsGit;
      this.SupportsTfvc = supportsTfvc;
    }
  }
}
