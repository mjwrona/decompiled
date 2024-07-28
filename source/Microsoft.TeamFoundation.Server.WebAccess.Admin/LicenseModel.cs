// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.LicenseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class LicenseModel
  {
    private ILicenseFeature[] m_licenseFeatures;

    public LicenseModel(IVssRequestContext requestContext, Guid licenseTypeId) => this.LicenseType = requestContext.GetService<ITeamFoundationLicensingService>().GetLicenseType(requestContext, licenseTypeId, out this.m_licenseFeatures);

    public ILicenseFeature[] LicenseFeatures => this.m_licenseFeatures;

    public ILicenseType LicenseType { get; private set; }
  }
}
