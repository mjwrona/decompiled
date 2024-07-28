// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenLifetimePolicy
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class SessionTokenLifetimePolicy
  {
    private const string Area = "DelegatedAuthorizationService";
    private const string Layer = "SessionTokenLifeTimePolicy";
    private readonly DelegatedAuthorizationSettings settings;
    private readonly Guid targetIdentityId;
    private DateTime? validTo;
    private readonly Guid authenticatedUserIdentityId;

    public TraceMethod TraceMethod { get; set; }

    public SessionTokenLifetimePolicy(
      DelegatedAuthorizationSettings settings,
      Guid targetIdentityId,
      Guid authenticatedUserIdentityId,
      DateTime? validTo = null)
    {
      this.settings = settings;
      this.targetIdentityId = targetIdentityId;
      this.authenticatedUserIdentityId = authenticatedUserIdentityId;
      this.validTo = validTo;
    }

    private void Trace(string format, params object[] args)
    {
      TraceMethod traceMethod = this.TraceMethod;
      if (traceMethod == null)
        return;
      traceMethod(1048003, TraceLevel.Info, "DelegatedAuthorizationService", "SessionTokenLifeTimePolicy", format, args);
    }

    public DateTime ApplyForCreate()
    {
      if (this.validTo.HasValue)
      {
        DateTime dateTime1 = DateTime.UtcNow.Add(this.settings.SessionTokenMaxLifetime);
        DateTime dateTime2 = dateTime1;
        DateTime? validTo = this.validTo;
        if ((validTo.HasValue ? (dateTime2 <= validTo.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          this.Trace("Token lifetime date {0} cannot be greater than max allowed {1} lifetime for app token, default to max allow date", (object) this.validTo.Value, (object) dateTime1);
          this.validTo = new DateTime?(dateTime1);
        }
      }
      else if (this.authenticatedUserIdentityId != this.targetIdentityId)
      {
        this.validTo = new DateTime?(DateTime.UtcNow.Add(this.settings.SessionTokenImpersonateLifetime));
        this.Trace("Target identity {0} and authenticated identity {1} does not match, using Impersonate lifetime date {2}", (object) this.targetIdentityId, (object) this.authenticatedUserIdentityId, (object) this.validTo);
      }
      else
      {
        this.validTo = new DateTime?(DateTime.UtcNow.Add(this.settings.SessionTokenLifetime));
        this.Trace("Target identity match with authenticated identity {0}, apply default date {1}", (object) this.targetIdentityId, (object) this.validTo);
      }
      return this.validTo.Value;
    }

    public DateTime? ApplyForUpdate()
    {
      if (this.validTo.HasValue)
      {
        DateTime dateTime1 = DateTime.UtcNow.Add(this.settings.SessionTokenMaxLifetime);
        DateTime dateTime2 = dateTime1;
        DateTime? validTo = this.validTo;
        if ((validTo.HasValue ? (dateTime2 <= validTo.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          this.Trace("Token lifetime date {0} cannot be greater than max allowed {1} lifetime for app token, default to max allow date.", (object) this.validTo.Value, (object) dateTime1);
          this.validTo = new DateTime?(dateTime1);
        }
      }
      return this.validTo;
    }
  }
}
