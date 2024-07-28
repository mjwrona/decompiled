// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountResourceRequestInternal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AccountResourceRequestInternal : AccountResourceRequest
  {
    private const string Area = "Commerce";
    private const string Layer = "AccountResourceRequestInternal";
    private bool upnValidationRequired;
    private const string c_upnSeparatorForAadForeignUser = "#EXT#";

    public AccountResourceRequestInternal(
      AccountResourceRequest request,
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      string resourceProviderNamespace)
    {
      this.Location = request.Location;
      this.OperationType = request.OperationType;
      this.Properties = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (!request.Properties.IsNullOrEmpty<KeyValuePair<string, string>>())
        this.Properties.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) request.Properties);
      this.ResourceGroupName = resourceGroupName;
      this.ResourceName = resourceName;
      this.SubscriptionId = subscriptionId;
      this.Tags = request.Tags;
      this.RequestSource = request.RequestSource;
    }

    public AccountResourceRequestInternal() => this.Properties = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public void PopulatePropertyFields(IVssRequestContext tfsRequestContext)
    {
      tfsRequestContext.TraceProperties<Dictionary<string, string>>(5106092, "Commerce", nameof (AccountResourceRequestInternal), this.Properties, (string) null);
      if (this.Properties.IsNullOrEmpty<KeyValuePair<string, string>>())
      {
        tfsRequestContext.Trace(5106090, TraceLevel.Error, "Commerce", nameof (AccountResourceRequestInternal), "Incoming provision request failed to read request properties from the body.");
        throw new ArgumentException(HostingResources.MissingRequiredProperty((object) "Properties"));
      }
      string str1;
      AccountResourceRequestOperationType result;
      if (this.Properties.TryGetValue("operationType", out str1) && Enum.TryParse<AccountResourceRequestOperationType>(str1, out result))
        this.OperationType = result;
      string str2;
      if (this.Properties.TryGetValue("RequestSource", out str2))
        this.RequestSource = str2;
      if (this.OperationType == AccountResourceRequestOperationType.Unknown)
      {
        tfsRequestContext.Trace(5106090, TraceLevel.Error, "Commerce", nameof (AccountResourceRequestInternal), string.Format("Incoming provision request has invalid operation type {0}", (object) this.OperationType));
        throw new ArgumentException(HostingResources.InvalidOperationType((object) this.OperationType));
      }
      str1 = (string) null;
      if (this.OperationType == AccountResourceRequestOperationType.Create && this.Properties.TryGetValue("ownerUpn", out str1) && !string.IsNullOrEmpty(str1))
      {
        this.Upn = str1;
        this.upnValidationRequired = true;
        tfsRequestContext.Trace(5106097, TraceLevel.Info, "Commerce", nameof (AccountResourceRequestInternal), "Owner id upn: " + this.Upn);
      }
      if (this.OperationType != AccountResourceRequestOperationType.Link)
        return;
      if (!this.Properties.TryGetValue("AccountName", out str1))
        throw new ArgumentException(HostingResources.MissingRequiredProperty((object) "AccountName"));
      this.AccountName = str1;
    }

    public void SetIdentityInfo(HttpRequestHeaders headers)
    {
      this.Upn = this.Upn ?? headers.GetValues("x-ms-client-principal-name").FirstOrDefault<string>();
      this.Email = CommerceIdentityHelper.GetEmailFromUpn(this.Upn);
      this.TenantId = headers.GetValues("x-ms-client-tenant-id").FirstOrDefault<string>();
    }

    public void Validate(IVssRequestContext tfsRequestContext)
    {
      if (!this.upnValidationRequired)
        return;
      if (string.IsNullOrEmpty(this.TenantId))
        throw new ArgumentException(HostingResources.MissingRequiredProperty((object) "IdentityDomain"));
      if (!this.CheckUserInTenant(tfsRequestContext))
      {
        tfsRequestContext.Trace(5106099, TraceLevel.Info, "Commerce", nameof (AccountResourceRequestInternal), "User " + this.Upn + " was not found in the tenant " + this.TenantId + ".");
        throw new ArgumentException(HostingResources.UserIsNotAMemberOfTenant((object) this.Upn, (object) this.TenantId));
      }
    }

    public override string ToString()
    {
      StringBuilder seed = new StringBuilder();
      seed.Append("Location:" + this.Location + "; ");
      if (this.Tags != null)
      {
        seed.Append("Tags[");
        this.Tags.Aggregate<KeyValuePair<string, string>, StringBuilder>(seed, (Func<StringBuilder, KeyValuePair<string, string>, StringBuilder>) ((b, t) => b.Append(t.Key + ":" + t.Value + "; ")));
        seed.Append("]; ");
      }
      if (this.Properties != null)
      {
        seed.Append("Properties[");
        this.Properties.Aggregate<KeyValuePair<string, string>, StringBuilder>(seed, (Func<StringBuilder, KeyValuePair<string, string>, StringBuilder>) ((b, p) => b.Append(p.Key + ":" + p.Value + "; ")));
        seed.Append("]; ");
      }
      return seed.ToString();
    }

    public Guid SubscriptionId { get; set; }

    public string ResourceGroupName { get; set; }

    public string ResourceName { get; set; }

    public string Email { get; set; }

    public string TenantId { get; set; }

    internal CustomerIntelligenceEntryPoint GetRequestSource()
    {
      CustomerIntelligenceEntryPoint result;
      return !string.IsNullOrWhiteSpace(this.RequestSource) && Enum.TryParse<CustomerIntelligenceEntryPoint>(this.RequestSource, true, out result) ? result : CustomerIntelligenceEntryPoint.Ibiza;
    }

    private bool CheckUserInTenant(IVssRequestContext tfsRequestContext)
    {
      if (!tfsRequestContext.ServiceHost.IsProduction)
        return true;
      GetUsersRequest getUsersRequest1 = new GetUsersRequest();
      getUsersRequest1.ToTenant = this.TenantId;
      getUsersRequest1.UserPrincipalNamePrefixes = (IEnumerable<string>) new string[1]
      {
        this.Upn
      };
      GetUsersRequest request1 = getUsersRequest1;
      IVssRequestContext context = tfsRequestContext.Elevate();
      AadService service = context.GetService<AadService>();
      GetUsersResponse usersResponse = service.GetUsers(context, request1);
      bool foundUser = usersResponse != null && usersResponse.Users != null && usersResponse.Users.Any<AadUser>();
      if (!foundUser)
      {
        string str = AccountResourceRequestInternal.BuildForeignUserUpnPrefix(this.Upn);
        GetUsersRequest getUsersRequest2 = new GetUsersRequest();
        getUsersRequest2.ToTenant = this.TenantId;
        getUsersRequest2.UserPrincipalNamePrefixes = (IEnumerable<string>) new string[1]
        {
          str
        };
        GetUsersRequest request2 = getUsersRequest2;
        usersResponse = service.GetUsers(context, request2);
        foundUser = usersResponse != null && usersResponse.Users != null && usersResponse.Users.Any<AadUser>();
      }
      tfsRequestContext.TraceConditionally(5106098, TraceLevel.Info, "Commerce", nameof (AccountResourceRequestInternal), (Func<string>) (() => string.Format("Owner id upn: {0}. Tenant: {1}. Found user in tenant: {2}. Object Id:{3}", (object) this.Upn, (object) this.TenantId, (object) foundUser, (object) (foundUser ? usersResponse.Users.First<AadUser>().ObjectId : Guid.Empty))));
      return foundUser;
    }

    private static string BuildForeignUserUpnPrefix(string upn)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(upn, nameof (upn));
      return upn.Replace('@', '_') + "#EXT#";
    }
  }
}
