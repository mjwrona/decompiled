// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.IdentityViewModelBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Aad;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public abstract class IdentityViewModelBase : IComparable
  {
    private List<string> m_errors;
    private List<string> m_warnings;
    private static readonly string s_layer = nameof (IdentityViewModelBase);
    private const string c_orgSecurity = "/_org/_security";

    protected IdentityViewModelBase()
    {
    }

    protected IdentityViewModelBase(TeamFoundationIdentity identity)
    {
      this.Identity = identity;
      this.Descriptor = identity.Descriptor;
      this.TeamFoundationId = identity.TeamFoundationId;
      this.TrySetEntityId(this.TeamFoundationId);
    }

    public int CompareTo(object obj)
    {
      IdentityViewModelBase var = obj as IdentityViewModelBase;
      ArgumentUtility.CheckForNull<IdentityViewModelBase>(var, nameof (obj));
      return string.Compare(this.DisplayName, var.DisplayName, StringComparison.CurrentCulture);
    }

    public string DisplayName { get; set; }

    public List<string> Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new List<string>();
        return this.m_errors;
      }
    }

    public List<string> Warnings
    {
      get
      {
        if (this.m_warnings == null)
          this.m_warnings = new List<string>();
        return this.m_warnings;
      }
    }

    public string EntityId { get; internal set; }

    public IEnumerable<string> LicenseNames { get; set; }

    public abstract string FriendlyDisplayName { get; }

    public IdentityDescriptor Descriptor { get; private set; }

    public abstract string IdentityType { get; }

    public bool IncludeDescriptor { get; set; }

    public bool IsForEdit { get; set; }

    public List<IdentityViewModelBase> MemberOf { get; private set; }

    public void PopulateMemberOf(TfsWebContext webContext)
    {
      TeamFoundationIdentity[] foundationIdentityArray = webContext.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(webContext.TfsRequestContext, this.Identity.MemberOf.ToArray<IdentityDescriptor>(), MembershipQuery.Direct, ReadIdentityOptions.None, (IEnumerable<string>) null);
      this.MemberOf = new List<IdentityViewModelBase>();
      foreach (TeamFoundationIdentity identity in foundationIdentityArray)
      {
        if (identity != null)
          this.MemberOf.Add(IdentityImageUtility.GetIdentityViewModel(identity));
      }
      this.MemberOf.Sort();
    }

    public abstract string SubHeader { get; }

    public Guid TeamFoundationId { get; set; }

    public override bool Equals(object obj)
    {
      IdentityViewModelBase var = obj as IdentityViewModelBase;
      ArgumentUtility.CheckForNull<IdentityViewModelBase>(var, nameof (obj));
      return this.TeamFoundationId.Equals(var.TeamFoundationId);
    }

    public override int GetHashCode() => this.TeamFoundationId.GetHashCode();

    protected TeamFoundationIdentity Identity { get; private set; }

    public bool IsWindowsIdentity => this.Identity != null && !(this.Identity.Descriptor == (IdentityDescriptor) null) && string.Equals(this.Identity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase);

    protected static string GetTenantName(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      string tenantName = (string) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        if (userIdentity.IsExternalUser)
        {
          try
          {
            IAadTenantDetailProvider extension = requestContext.GetExtension<IAadTenantDetailProvider>((Func<IAadTenantDetailProvider, bool>) (x => x.CanHandleRequest(requestContext)));
            if (extension != null)
              tenantName = extension.GetDisplayName(requestContext.To(TeamFoundationHostType.Deployment));
            else
              requestContext.Trace(0, TraceLevel.Warning, nameof (GetTenantName), IdentityViewModelBase.s_layer, "Could not find an implementation for IAadTenantDetailProvider extension. User ID: {0}", (object) requestContext.GetUserId());
          }
          catch (Exception ex)
          {
            requestContext.TraceException(0, TraceLevel.Error, nameof (GetTenantName), IdentityViewModelBase.s_layer, ex);
          }
        }
      }
      return tenantName;
    }

    private bool TrySetEntityId(Guid tfid)
    {
      if (Guid.Empty.Equals(tfid))
        return false;
      try
      {
        TfsWebContext tfsWebContext = (TfsWebContext) null;
        if (TfsHelpers.TryGetTfsWebContext(out tfsWebContext))
        {
          if (tfsWebContext != null)
          {
            IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
            if (!tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.AadGroupsAdminUi"))
              return false;
            if (tfsRequestContext.ExecutionEnvironment.IsHostedDeployment && tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience"))
            {
              string str = tfsWebContext.RequestContext.HttpContext.Request.Headers.Get("Referer");
              if ((str != null ? (str.ToLowerInvariant().Contains("/_org/_security") ? 1 : 0) : 0) != 0)
                tfsRequestContext = tfsRequestContext.To(TeamFoundationHostType.Application);
            }
            DirectoryDiscoveryService service = tfsRequestContext.GetService<DirectoryDiscoveryService>();
            IVssRequestContext context = tfsRequestContext;
            DirectoryConvertKeysRequest request = new DirectoryConvertKeysRequest();
            request.ConvertFrom = "VisualStudioIdentifier";
            request.ConvertTo = "DirectoryEntityIdentifier";
            request.Directories = (IEnumerable<string>) new List<string>()
            {
              "vsd"
            };
            request.Keys = (IEnumerable<string>) new List<string>()
            {
              tfid.ToString()
            };
            DirectoryConvertKeysResponse convertKeysResponse = service.ConvertKeys(context, request);
            if (convertKeysResponse != null)
            {
              if (convertKeysResponse.Results.Count == 1)
              {
                DirectoryConvertKeyResult convertKeyResult;
                if (convertKeysResponse.Results.TryGetValue(tfid.ToString(), out convertKeyResult))
                {
                  if (convertKeyResult.Exception == null)
                  {
                    this.EntityId = convertKeyResult.Key;
                    return true;
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.EntityId = string.Empty;
      }
      return false;
    }

    public virtual JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["IdentityType"] = (object) this.IdentityType;
      json["FriendlyDisplayName"] = (object) this.FriendlyDisplayName;
      json["DisplayName"] = (object) this.DisplayName;
      json["SubHeader"] = (object) this.SubHeader;
      json["TeamFoundationId"] = (object) this.TeamFoundationId;
      json["EntityId"] = (object) this.EntityId;
      json["Errors"] = (object) this.Errors;
      json["Warnings"] = (object) this.Warnings;
      if (this.IncludeDescriptor)
      {
        json["DescriptorIdentityType"] = this.Descriptor != (IdentityDescriptor) null ? (object) this.Descriptor.IdentityType : (object) string.Empty;
        json["DescriptorIdentifier"] = this.Descriptor != (IdentityDescriptor) null ? (object) this.Descriptor.Identifier : (object) string.Empty;
      }
      return json;
    }
  }
}
