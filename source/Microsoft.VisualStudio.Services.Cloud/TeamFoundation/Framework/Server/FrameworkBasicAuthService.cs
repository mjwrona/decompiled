// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FrameworkBasicAuthService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.BasicAuth;
using Microsoft.VisualStudio.Services.Cloud.BasicAuth;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FrameworkBasicAuthService : BasicAuthService
  {
    private const string TraceArea = "FrameworkBasicAuthService";
    private const string TraceLayer = "Service";

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public override void DeleteBasicCredential(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        requestContext.TraceEnter(1007020, nameof (FrameworkBasicAuthService), "Service", nameof (DeleteBasicCredential));
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
        IFrameworkBasicAuthCache basicAuthCache = this.GetBasicAuthCache(basicAuthContext);
        TaskExtensions.SyncResult(this.GetHttpClient(basicAuthContext).DeleteCredentialAsync(identity.Id));
        this.TryInvalidateHash(basicAuthContext, basicAuthCache, identity);
        requestContext.Trace(1007022, TraceLevel.Verbose, nameof (FrameworkBasicAuthService), "Service", "Deleted basic credentials for account {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
      }
      finally
      {
        requestContext.TraceLeave(1007029, nameof (FrameworkBasicAuthService), "Service", nameof (DeleteBasicCredential));
      }
    }

    public override bool IsBasicAuthDisabled(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        requestContext.TraceEnter(1007040, nameof (FrameworkBasicAuthService), "Service", nameof (IsBasicAuthDisabled));
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return true;
        IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
        if (identity == null)
          return true;
        BasicAuthCredential basicAuthCredential = this.GetHttpClient(basicAuthContext).GetCredentialAsync(identity.Id).SyncResult<BasicAuthCredential>();
        return basicAuthCredential != null && basicAuthCredential.Disabled;
      }
      finally
      {
        requestContext.TraceEnter(1007041, nameof (FrameworkBasicAuthService), "Service", nameof (IsBasicAuthDisabled));
      }
    }

    public override bool HasBasicCredential(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      try
      {
        requestContext.TraceEnter(1007042, nameof (FrameworkBasicAuthService), "Service", nameof (HasBasicCredential));
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return false;
        IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
        return identity != null && this.GetHttpClient(basicAuthContext).GetCredentialAsync(identity.Id).SyncResult<BasicAuthCredential>() != null;
      }
      finally
      {
        requestContext.TraceEnter(1007043, nameof (FrameworkBasicAuthService), "Service", nameof (HasBasicCredential));
      }
    }

    public override void SetBasicCredential(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string password)
    {
      try
      {
        requestContext.TraceEnter(1007010, nameof (FrameworkBasicAuthService), "Service", nameof (SetBasicCredential));
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
        IFrameworkBasicAuthCache basicAuthCache = this.GetBasicAuthCache(basicAuthContext);
        BasicAuthService.CheckAlternateCredentials();
        BasicAuthCredential credential = BasicAuthService.IsValidBasicPassword(password) ? new BasicAuthCredential()
        {
          Password = password,
          Disabled = false
        } : throw new ArgumentException(FrameworkResources.BasicAuthenticationPasswordInvalid());
        TaskExtensions.SyncResult(this.GetHttpClient(basicAuthContext).CreateCredentialAsync(identity.Id, credential));
        this.TryInvalidateHash(basicAuthContext, basicAuthCache, identity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1007018, nameof (FrameworkBasicAuthService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1007019, nameof (FrameworkBasicAuthService), "Service", nameof (SetBasicCredential));
      }
    }

    public override bool IsValidBasicCredential(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string password)
    {
      try
      {
        requestContext.TraceEnter(1007000, nameof (FrameworkBasicAuthService), "Service", nameof (IsValidBasicCredential));
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment || string.IsNullOrEmpty(password))
          return false;
        IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
        IVssRequestContext context = basicAuthContext.Elevate();
        IdentityService service = context.GetService<IdentityService>();
        IFrameworkBasicAuthCache basicAuthCache = this.GetBasicAuthCache(basicAuthContext);
        IVssRequestContext requestContext1 = context;
        IdentityDescriptor[] descriptors = new IdentityDescriptor[1]
        {
          identity.Descriptor
        };
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = service.ReadIdentities(requestContext1, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity1 == null)
        {
          requestContext.TraceDataConditionally(1007005, TraceLevel.Error, nameof (FrameworkBasicAuthService), "Service", "Identity not found at deployment level", (Func<object>) (() => (object) new
          {
            Id = identity.Id,
            Descriptor = identity.Descriptor
          }), nameof (IsValidBasicCredential));
          return false;
        }
        identity = identity1;
        byte[] bytes;
        using (DeriveBytes deriveBytes = (DeriveBytes) new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(password), identity.Id.ToByteArray(), 1))
          bytes = deriveBytes.GetBytes(32);
        byte[] cachedHash;
        if (basicAuthCache.TryGetHash(basicAuthContext, identity.Id, out cachedHash))
        {
          if (ArrayUtil.Equals(bytes, cachedHash))
          {
            requestContext.Trace(1007007, TraceLevel.Verbose, nameof (FrameworkBasicAuthService), "Service", "Successful alternate auth (cached) for {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
            return true;
          }
          requestContext.Trace(1007015, TraceLevel.Verbose, nameof (FrameworkBasicAuthService), "Service", "Cached alternate auth creds for identity {0} does not match. Performing authoritative check.", (object) identity.Descriptor.ToString());
        }
        else
          requestContext.Trace(1007016, TraceLevel.Verbose, nameof (FrameworkBasicAuthService), "Service", "Cache Miss!. Cached alternate auth creds for identity {0} not found", (object) identity.Descriptor.ToString());
        if (this.GetHttpClient(basicAuthContext).CheckCredentialAsync(identity.Id, password).SyncResult<bool>())
        {
          requestContext.Trace(1007006, TraceLevel.Verbose, nameof (FrameworkBasicAuthService), "Service", "Successful alternate auth for {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
          this.TrySetHash(basicAuthContext, basicAuthCache, identity, bytes);
          return true;
        }
        requestContext.Trace(1007008, TraceLevel.Verbose, nameof (FrameworkBasicAuthService), "Service", "Unsuccessful alternate auth for {0} ({1})", (object) identity.DisplayName, (object) identity.Id);
        this.TryInvalidateHash(basicAuthContext, basicAuthCache, identity);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(1007009, nameof (FrameworkBasicAuthService), "Service", nameof (IsValidBasicCredential));
      }
    }

    public override void EnableDisabledAccount(IVssRequestContext requestContext, Guid identityId)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      IVssRequestContext basicAuthContext = requestContext.GetScopedBasicAuthContext();
      BasicAuthCredential credential = new BasicAuthCredential()
      {
        Disabled = false
      };
      try
      {
        requestContext.TraceEnter(1007050, nameof (FrameworkBasicAuthService), "Service", nameof (EnableDisabledAccount));
        TaskExtensions.SyncResult(this.GetHttpClient(basicAuthContext).UpdateCredentialAsync(identityId, credential));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1007052, nameof (FrameworkBasicAuthService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1007050, nameof (FrameworkBasicAuthService), "Service", nameof (EnableDisabledAccount));
      }
    }

    internal virtual BasicAuthHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<BasicAuthHttpClient>();

    internal virtual IFrameworkBasicAuthCache GetBasicAuthCache(IVssRequestContext requestContext) => requestContext.GetService<IFrameworkBasicAuthCache>();

    private bool TrySetHash(
      IVssRequestContext requestContext,
      IFrameworkBasicAuthCache basicAuthCache,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      byte[] computedWeakHash)
    {
      bool flag = basicAuthCache.TrySetHash(requestContext, identity.Id, computedWeakHash);
      requestContext.Trace(1007013, flag ? TraceLevel.Verbose : TraceLevel.Warning, nameof (FrameworkBasicAuthService), "Service", "Set cached alternate auth creds for {0} ({1}) {2}", (object) identity.DisplayName, (object) identity.Id, flag ? (object) "succeeded" : (object) "failed");
      return flag;
    }

    private bool TryInvalidateHash(
      IVssRequestContext requestContext,
      IFrameworkBasicAuthCache basicAuthCache,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      bool flag = basicAuthCache.TryInvalidateHash(requestContext, identity.Id);
      requestContext.Trace(1007014, flag ? TraceLevel.Verbose : TraceLevel.Warning, nameof (FrameworkBasicAuthService), "Service", "Invalidate cached alternate auth creds for {0} ({1}) {2}", (object) identity.DisplayName, (object) identity.Id, flag ? (object) "succeeded" : (object) "failed");
      return flag;
    }
  }
}
