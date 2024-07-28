// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.UserInformationProviderBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class UserInformationProviderBase
  {
    private const string FullUserDomainEnvironmentKey = "USERDNSDOMAIN";
    protected const string MicrosoftTenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";
    private readonly Lazy<bool> canCollectPrivateInformation;
    private readonly Lazy<bool> isUserMicrosoftInternal;
    private readonly Lazy<bool> isMicrosoftAADJoined;
    private readonly Lazy<Guid> userId;
    private readonly IEnvironmentTools environmentTools;
    private readonly IInternalSettings internalSettings;
    private readonly ILegacyApi legacyApi;
    private static readonly HashSet<string> CanCollectPrivateInformationDomainList = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "redmond.corp.microsoft.com",
      "northamerica.corp.microsoft.com",
      "fareast.corp.microsoft.com",
      "ntdev.corp.microsoft.com",
      "wingroup.corp.microsoft.com",
      "southpacific.corp.microsoft.com",
      "wingroup.windeploy.ntdev.microsoft.com",
      "ddnet.microsoft.com"
    };
    private static readonly HashSet<string> MicrosoftInternalDomainList = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "redmond.corp.microsoft.com",
      "northamerica.corp.microsoft.com",
      "fareast.corp.microsoft.com",
      "ntdev.corp.microsoft.com",
      "wingroup.corp.microsoft.com",
      "southpacific.corp.microsoft.com",
      "wingroup.windeploy.ntdev.microsoft.com",
      "ddnet.microsoft.com",
      "europe.corp.microsoft.com"
    };

    public UserInformationProviderBase(
      IInternalSettings internalSettings,
      IEnvironmentTools envTools,
      ILegacyApi legacyApi,
      Guid? userId)
    {
      UserInformationProviderBase informationProviderBase = this;
      internalSettings.RequiresArgumentNotNull<IInternalSettings>(nameof (internalSettings));
      envTools.RequiresArgumentNotNull<IEnvironmentTools>(nameof (envTools));
      legacyApi.RequiresArgumentNotNull<ILegacyApi>(nameof (legacyApi));
      this.internalSettings = internalSettings;
      this.environmentTools = envTools;
      this.legacyApi = legacyApi;
      this.canCollectPrivateInformation = new Lazy<bool>(new Func<bool>(this.CalculateCanCollectPrivateInformation), LazyThreadSafetyMode.ExecutionAndPublication);
      this.isUserMicrosoftInternal = new Lazy<bool>(new Func<bool>(this.CalculateIsInternal), LazyThreadSafetyMode.ExecutionAndPublication);
      this.isMicrosoftAADJoined = new Lazy<bool>(new Func<bool>(this.CalculateIsMicrosoftAADJoined), LazyThreadSafetyMode.ExecutionAndPublication);
      this.userId = new Lazy<Guid>((Func<Guid>) (() => !userId.HasValue ? informationProviderBase.legacyApi.ReadSharedUserId() : userId.Value), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public bool CanCollectPrivateInformation => this.canCollectPrivateInformation.Value;

    public bool IsUserMicrosoftInternal => this.isUserMicrosoftInternal.Value;

    public bool IsMicrosoftAADJoined => this.isMicrosoftAADJoined.Value;

    public Guid UserId => this.userId.Value;

    public abstract UserType UserType { get; }

    protected abstract bool CalculateIsMicrosoftAADJoined();

    private bool CalculateIsInternal()
    {
      if (this.internalSettings.IsForcedUserExternal())
        return false;
      return this.UserType != UserType.External || this.ValidateDomainInformation(UserInformationProviderBase.MicrosoftInternalDomainList);
    }

    private bool CalculateCanCollectPrivateInformation() => !this.internalSettings.IsForcedUserExternal() && this.ValidateDomainInformation(UserInformationProviderBase.CanCollectPrivateInformationDomainList);

    private bool ValidateDomainInformation(HashSet<string> domainList)
    {
      string str = (string) null;
      try
      {
        str = this.environmentTools.GetEnvironmentVariable("USERDNSDOMAIN");
      }
      catch (SecurityException ex)
      {
      }
      if (str == null)
        str = this.internalSettings.GetIPGlobalConfigDomainName();
      return domainList.Contains(str);
    }
  }
}
