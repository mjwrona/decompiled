// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.AllowRule
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract(Name = "AllowRule", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class AllowRule : AuthorizationRule
  {
    public AllowRule()
    {
    }

    public AllowRule(
      string issuerName,
      string claimType,
      string claimValue,
      IEnumerable<AccessRights> rights)
    {
      this.IssuerName = issuerName;
      this.ClaimType = claimType;
      this.ClaimValue = claimValue.ToLowerInvariant();
      this.Rights = rights;
    }

    public AllowRule(
      string issuerName,
      AllowRuleClaimType claimType,
      string claimValue,
      IEnumerable<AccessRights> rights)
      : this(issuerName, claimType == AllowRuleClaimType.Upn ? "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn" : "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", claimValue, rights)
    {
    }

    public override string KeyName
    {
      get => this.ClaimValue;
      set => throw new NotSupportedException();
    }
  }
}
