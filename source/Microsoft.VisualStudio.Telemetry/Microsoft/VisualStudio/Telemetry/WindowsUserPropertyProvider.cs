// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsUserPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class WindowsUserPropertyProvider : IPropertyProvider
  {
    private const string AdminValue = "Administrator";
    private const string NormalUserValue = "NormalUser";
    private readonly IUserInformationProvider userInfoProvider;
    private readonly Lazy<bool> userAdminInformation;
    private readonly Lazy<bool> processAdminInformation;

    public WindowsUserPropertyProvider(IUserInformationProvider theUserInfoProvider)
    {
      theUserInfoProvider.RequiresArgumentNotNull<IUserInformationProvider>(nameof (theUserInfoProvider));
      this.userInfoProvider = theUserInfoProvider;
      this.userAdminInformation = new Lazy<bool>((Func<bool>) (() => this.InitializeUserAdminInformation()), false);
      this.processAdminInformation = new Lazy<bool>((Func<bool>) (() => this.InitializeProcessAdminInformation()), false);
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.Id", (object) this.userInfoProvider.UserId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.IsMicrosoftInternal", (object) this.userInfoProvider.IsUserMicrosoftInternal));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.Location.GeoId", (object) RegionInfo.CurrentRegion.GeoId));
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.Type", (object) this.userInfoProvider.UserType.ToString()));
      if (!this.userInfoProvider.IsMicrosoftAADJoined)
        return;
      sharedProperties.Add(new KeyValuePair<string, object>("VS.Core.User.IsMicrosoftAADJoined", (object) this.userInfoProvider.IsMicrosoftAADJoined));
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.User.IsDomainMember", (object) NativeMethods.IsOS(NativeMethods.OSFeatureFlag.OSDomainMember));
      if (token.IsCancellationRequested)
        return;
      telemetryContext.PostProperty("VS.Core.User.Location.CountryName", (object) RegionInfo.CurrentRegion.EnglishName);
      if (token.IsCancellationRequested)
        return;
      string propertyValue1 = this.userAdminInformation.Value ? "Administrator" : "NormalUser";
      telemetryContext.PostProperty("VS.Core.User.Privilege", (object) propertyValue1);
      string propertyValue2 = this.processAdminInformation.Value ? "Administrator" : "NormalUser";
      telemetryContext.PostProperty("VS.Core.Process.Privilege", (object) propertyValue2);
      if (!(this.userInfoProvider is WindowsUserInformationProvider userInfoProvider) || string.IsNullOrWhiteSpace(userInfoProvider.AADTenantId.Value))
        return;
      telemetryContext.PostProperty("VS.Core.User.AADTenantId", (object) userInfoProvider.AADTenantId.Value);
    }

    private bool InitializeProcessAdminInformation()
    {
      WindowsIdentity current = WindowsIdentity.GetCurrent();
      return current != null && new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
    }

    private bool InitializeUserAdminInformation()
    {
      int returnLength = Marshal.SizeOf(typeof (int));
      IntPtr num = Marshal.AllocHGlobal(returnLength);
      try
      {
        WindowsIdentity current = WindowsIdentity.GetCurrent();
        if (current == null || !NativeMethods.GetTokenInformation(current.Token, 18, num, returnLength, out returnLength))
          return false;
        switch (Marshal.ReadInt32(num))
        {
          case 2:
          case 3:
            return true;
          default:
            return false;
        }
      }
      finally
      {
        if (num != IntPtr.Zero)
          Marshal.FreeHGlobal(num);
      }
    }
  }
}
