// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.MacUserPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class MacUserPropertyProvider : IPropertyProvider
  {
    private const string AdminValue = "Administrator";
    private const string NormalUserValue = "NormalUser";
    private readonly IUserInformationProvider userInfoProvider;
    private readonly Lazy<bool> adminInformation;

    public MacUserPropertyProvider(IUserInformationProvider theUserInfoProvider)
    {
      theUserInfoProvider.RequiresArgumentNotNull<IUserInformationProvider>(nameof (theUserInfoProvider));
      this.userInfoProvider = theUserInfoProvider;
      this.adminInformation = new Lazy<bool>((Func<bool>) (() => this.InitializeAdminInformation()), false);
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.Id", (object) this.userInfoProvider.UserId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.IsMicrosoftInternal", (object) this.userInfoProvider.IsUserMicrosoftInternal));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.Location.GeoId", (object) RegionInfo.CurrentRegion.GeoId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.Type", (object) this.userInfoProvider.UserType.ToString()));
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.User.IsDomainMember", (object) false);
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.User.Location.CountryName", (object) RegionInfo.CurrentRegion.EnglishName);
      if (token.IsCancellationRequested)
        return;
      string propertyValue = this.adminInformation.Value ? "Administrator" : "NormalUser";
      telemetryContext.PostProperty("VS.Core.User.Privilege", (object) propertyValue);
    }

    private bool InitializeAdminInformation() => false;
  }
}
