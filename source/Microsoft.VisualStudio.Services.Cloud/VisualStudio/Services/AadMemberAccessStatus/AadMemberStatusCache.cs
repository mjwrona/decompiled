// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AadMemberAccessStatus.AadMemberStatusCache
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.AadMemberAccessStatus
{
  public class AadMemberStatusCache : VssCacheBase
  {
    private readonly VssMemoryCacheList<string, AadMemberStatus> cache;
    private readonly VssCacheExpiryProvider<string, AadMemberStatus> disabledUserExpiryProvider;
    private readonly VssCacheExpiryProvider<string, AadMemberStatus> deletedUserExpiryProvider;
    private readonly VssCacheExpiryProvider<string, AadMemberStatus> validUserExpiryProvider;
    private readonly VssCacheExpiryProvider<string, AadMemberStatus> invalidUserExpiryProvider;

    public AadMemberStatusCache(
      TimeSpan disabledUserExpirationInterval,
      TimeSpan deletedUserExpirationInterval,
      TimeSpan validUserExpirationInterval,
      TimeSpan invalidUserExpirationInterval)
    {
      this.disabledUserExpiryProvider = new VssCacheExpiryProvider<string, AadMemberStatus>(Capture.Create<TimeSpan>(disabledUserExpirationInterval), Capture.Create<TimeSpan>(disabledUserExpirationInterval));
      this.deletedUserExpiryProvider = new VssCacheExpiryProvider<string, AadMemberStatus>(Capture.Create<TimeSpan>(deletedUserExpirationInterval), Capture.Create<TimeSpan>(deletedUserExpirationInterval));
      this.validUserExpiryProvider = new VssCacheExpiryProvider<string, AadMemberStatus>(Capture.Create<TimeSpan>(validUserExpirationInterval), Capture.Create<TimeSpan>(validUserExpirationInterval));
      this.invalidUserExpiryProvider = new VssCacheExpiryProvider<string, AadMemberStatus>(Capture.Create<TimeSpan>(invalidUserExpirationInterval), Capture.Create<TimeSpan>(invalidUserExpirationInterval));
      this.cache = new VssMemoryCacheList<string, AadMemberStatus>((IVssCachePerformanceProvider) this, this.disabledUserExpiryProvider);
    }

    public bool TryGetValue(string cacheKey, out AadMemberStatus memberStatus) => this.cache.TryGetValue(cacheKey, out memberStatus);

    public void Set(string cacheKey, AadMemberStatus memberStatus)
    {
      DateTimeOffset statusValidUntil = memberStatus.StatusValidUntil;
      VssCacheExpiryProvider<string, AadMemberStatus> expiryProvider;
      if (memberStatus.StatusValidUntil != new DateTimeOffset() && memberStatus.StatusValidUntil > DateTimeOffset.UtcNow)
      {
        TimeSpan timeSpan = TimeSpan.FromTicks((memberStatus.StatusValidUntil - DateTimeOffset.UtcNow).Ticks * 2L);
        expiryProvider = new VssCacheExpiryProvider<string, AadMemberStatus>(Capture.Create<TimeSpan>(timeSpan), Capture.Create<TimeSpan>(timeSpan));
      }
      else
      {
        switch (memberStatus.MemberState)
        {
          case AadMemberAccessState.Deleted:
            expiryProvider = this.deletedUserExpiryProvider;
            break;
          case AadMemberAccessState.Disabled:
            expiryProvider = this.disabledUserExpiryProvider;
            break;
          case AadMemberAccessState.Invalid:
            expiryProvider = this.invalidUserExpiryProvider;
            break;
          case AadMemberAccessState.Valid:
            expiryProvider = this.validUserExpiryProvider;
            break;
          default:
            throw new InvalidAadMemberStatusException(memberStatus.MemberState);
        }
      }
      this.cache.Add(cacheKey, memberStatus, true, expiryProvider);
    }

    public void Clear() => this.cache.Clear();
  }
}
