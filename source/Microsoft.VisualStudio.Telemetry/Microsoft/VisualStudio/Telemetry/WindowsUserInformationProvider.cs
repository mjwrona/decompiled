// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsUserInformationProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class WindowsUserInformationProvider : 
    UserInformationProviderBase,
    IUserInformationProvider
  {
    private const string SqmUserTypeRegistryPath = "Software\\Policies\\Microsoft\\VisualStudio\\SQM";
    private const string SqmUserTypeRegistryKey = "UserType";
    private readonly IRegistryTools registryTools;
    private readonly Lazy<UserType> userType;
    internal readonly Lazy<string> AADTenantId;

    public WindowsUserInformationProvider(
      IRegistryTools regTools,
      IInternalSettings internalSettings,
      IEnvironmentTools envTools,
      ILegacyApi legacyApi,
      Guid? userId)
      : base(internalSettings, envTools, legacyApi, userId)
    {
      regTools.RequiresArgumentNotNull<IRegistryTools>(nameof (regTools));
      this.registryTools = regTools;
      this.userType = new Lazy<UserType>((Func<UserType>) (() => this.CalculateUserType()), LazyThreadSafetyMode.ExecutionAndPublication);
      this.AADTenantId = new Lazy<string>((Func<string>) (() => this.GetAADTenantId()), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public override UserType UserType => this.userType.Value;

    protected override bool CalculateIsMicrosoftAADJoined() => Platform.IsWindows && string.Equals("72f988bf-86f1-41af-91ab-2d7cd011db47", this.AADTenantId.Value, StringComparison.OrdinalIgnoreCase);

    private UserType CalculateUserType()
    {
      UserType userType = UserType.External;
      object fromCurrentUserRoot = this.registryTools.GetRegistryValueFromCurrentUserRoot("Software\\Policies\\Microsoft\\VisualStudio\\SQM", "UserType");
      if (fromCurrentUserRoot != null && fromCurrentUserRoot is int && Enum.IsDefined(typeof (UserType), fromCurrentUserRoot))
        userType = (UserType) fromCurrentUserRoot;
      return userType;
    }

    private string GetAADTenantId()
    {
      if (!Platform.IsWindows)
        return string.Empty;
      try
      {
        IntPtr ppJoinInfo = IntPtr.Zero;
        if (NativeMethods.NetGetAadJoinInformation((string) null, out ppJoinInfo) == 0)
        {
          if (ppJoinInfo != IntPtr.Zero)
          {
            string tenantId = ((NativeMethods.DSREG_JOIN_INFO) Marshal.PtrToStructure(ppJoinInfo, typeof (NativeMethods.DSREG_JOIN_INFO))).TenantId;
            NativeMethods.NetFreeAadJoinInformation(ppJoinInfo);
            return tenantId;
          }
        }
      }
      catch (EntryPointNotFoundException ex)
      {
      }
      return string.Empty;
    }
  }
}
