// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.ManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.CodeDom.Compiler;
using System.Data.Services.Client;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  public class ManagementService : DataServiceContext
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<ServiceKey> _ServiceKeys;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<IdentityProvider> _IdentityProviders;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<IdentityProviderAddress> _IdentityProviderAddresses;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<IdentityProviderKey> _IdentityProviderKeys;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<ClaimType> _ClaimTypes;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<IdentityProviderClaimType> _IdentityProviderClaimTypes;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<Delegation> _Delegations;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<RelyingParty> _RelyingParties;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<RelyingPartyAddress> _RelyingPartyAddresses;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<RelyingPartyIdentityProvider> _RelyingPartyIdentityProviders;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<RelyingPartyKey> _RelyingPartyKeys;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<ServiceIdentity> _ServiceIdentities;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<ServiceIdentityKey> _ServiceIdentityKeys;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<RuleGroup> _RuleGroups;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<RelyingPartyRuleGroup> _RelyingPartyRuleGroups;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<Issuer> _Issuers;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<Rule> _Rules;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceQuery<ConditionalRule> _ConditionalRules;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public ManagementService(Uri serviceRoot)
      : base(serviceRoot)
    {
      this.ResolveName = new Func<Type, string>(this.ResolveNameFromType);
      this.ResolveType = new Func<string, Type>(this.ResolveTypeFromName);
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    protected Type ResolveTypeFromName(string typeName) => typeName.StartsWith("Microsoft.Cloud.AccessControl.Management", StringComparison.Ordinal) ? this.GetType().Assembly.GetType("Microsoft.TeamFoundation.Server.Core.AccessControl" + typeName.Substring(40), false) : (Type) null;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    protected string ResolveNameFromType(Type clientType) => clientType.Namespace.Equals("Microsoft.TeamFoundation.Server.Core.AccessControl", StringComparison.Ordinal) ? "Microsoft.Cloud.AccessControl.Management." + clientType.Name : (string) null;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<ServiceKey> ServiceKeys
    {
      get
      {
        if (this._ServiceKeys == null)
          this._ServiceKeys = this.CreateQuery<ServiceKey>(nameof (ServiceKeys));
        return this._ServiceKeys;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<IdentityProvider> IdentityProviders
    {
      get
      {
        if (this._IdentityProviders == null)
          this._IdentityProviders = this.CreateQuery<IdentityProvider>(nameof (IdentityProviders));
        return this._IdentityProviders;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<IdentityProviderAddress> IdentityProviderAddresses
    {
      get
      {
        if (this._IdentityProviderAddresses == null)
          this._IdentityProviderAddresses = this.CreateQuery<IdentityProviderAddress>(nameof (IdentityProviderAddresses));
        return this._IdentityProviderAddresses;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<IdentityProviderKey> IdentityProviderKeys
    {
      get
      {
        if (this._IdentityProviderKeys == null)
          this._IdentityProviderKeys = this.CreateQuery<IdentityProviderKey>(nameof (IdentityProviderKeys));
        return this._IdentityProviderKeys;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<ClaimType> ClaimTypes
    {
      get
      {
        if (this._ClaimTypes == null)
          this._ClaimTypes = this.CreateQuery<ClaimType>(nameof (ClaimTypes));
        return this._ClaimTypes;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<IdentityProviderClaimType> IdentityProviderClaimTypes
    {
      get
      {
        if (this._IdentityProviderClaimTypes == null)
          this._IdentityProviderClaimTypes = this.CreateQuery<IdentityProviderClaimType>(nameof (IdentityProviderClaimTypes));
        return this._IdentityProviderClaimTypes;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<Delegation> Delegations
    {
      get
      {
        if (this._Delegations == null)
          this._Delegations = this.CreateQuery<Delegation>(nameof (Delegations));
        return this._Delegations;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<RelyingParty> RelyingParties
    {
      get
      {
        if (this._RelyingParties == null)
          this._RelyingParties = this.CreateQuery<RelyingParty>(nameof (RelyingParties));
        return this._RelyingParties;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<RelyingPartyAddress> RelyingPartyAddresses
    {
      get
      {
        if (this._RelyingPartyAddresses == null)
          this._RelyingPartyAddresses = this.CreateQuery<RelyingPartyAddress>(nameof (RelyingPartyAddresses));
        return this._RelyingPartyAddresses;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<RelyingPartyIdentityProvider> RelyingPartyIdentityProviders
    {
      get
      {
        if (this._RelyingPartyIdentityProviders == null)
          this._RelyingPartyIdentityProviders = this.CreateQuery<RelyingPartyIdentityProvider>(nameof (RelyingPartyIdentityProviders));
        return this._RelyingPartyIdentityProviders;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<RelyingPartyKey> RelyingPartyKeys
    {
      get
      {
        if (this._RelyingPartyKeys == null)
          this._RelyingPartyKeys = this.CreateQuery<RelyingPartyKey>(nameof (RelyingPartyKeys));
        return this._RelyingPartyKeys;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<ServiceIdentity> ServiceIdentities
    {
      get
      {
        if (this._ServiceIdentities == null)
          this._ServiceIdentities = this.CreateQuery<ServiceIdentity>(nameof (ServiceIdentities));
        return this._ServiceIdentities;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<ServiceIdentityKey> ServiceIdentityKeys
    {
      get
      {
        if (this._ServiceIdentityKeys == null)
          this._ServiceIdentityKeys = this.CreateQuery<ServiceIdentityKey>(nameof (ServiceIdentityKeys));
        return this._ServiceIdentityKeys;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<RuleGroup> RuleGroups
    {
      get
      {
        if (this._RuleGroups == null)
          this._RuleGroups = this.CreateQuery<RuleGroup>(nameof (RuleGroups));
        return this._RuleGroups;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<RelyingPartyRuleGroup> RelyingPartyRuleGroups
    {
      get
      {
        if (this._RelyingPartyRuleGroups == null)
          this._RelyingPartyRuleGroups = this.CreateQuery<RelyingPartyRuleGroup>(nameof (RelyingPartyRuleGroups));
        return this._RelyingPartyRuleGroups;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<Issuer> Issuers
    {
      get
      {
        if (this._Issuers == null)
          this._Issuers = this.CreateQuery<Issuer>(nameof (Issuers));
        return this._Issuers;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<Rule> Rules
    {
      get
      {
        if (this._Rules == null)
          this._Rules = this.CreateQuery<Rule>(nameof (Rules));
        return this._Rules;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceQuery<ConditionalRule> ConditionalRules
    {
      get
      {
        if (this._ConditionalRules == null)
          this._ConditionalRules = this.CreateQuery<ConditionalRule>(nameof (ConditionalRules));
        return this._ConditionalRules;
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToServiceKeys(ServiceKey serviceKey) => this.AddObject("ServiceKeys", (object) serviceKey);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToIdentityProviders(IdentityProvider identityProvider) => this.AddObject("IdentityProviders", (object) identityProvider);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToIdentityProviderAddresses(IdentityProviderAddress identityProviderAddress) => this.AddObject("IdentityProviderAddresses", (object) identityProviderAddress);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToIdentityProviderKeys(IdentityProviderKey identityProviderKey) => this.AddObject("IdentityProviderKeys", (object) identityProviderKey);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToClaimTypes(ClaimType claimType) => this.AddObject("ClaimTypes", (object) claimType);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToIdentityProviderClaimTypes(
      IdentityProviderClaimType identityProviderClaimType)
    {
      this.AddObject("IdentityProviderClaimTypes", (object) identityProviderClaimType);
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToDelegations(Delegation delegation) => this.AddObject("Delegations", (object) delegation);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRelyingParties(RelyingParty relyingParty) => this.AddObject("RelyingParties", (object) relyingParty);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRelyingPartyAddresses(RelyingPartyAddress relyingPartyAddress) => this.AddObject("RelyingPartyAddresses", (object) relyingPartyAddress);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRelyingPartyIdentityProviders(
      RelyingPartyIdentityProvider relyingPartyIdentityProvider)
    {
      this.AddObject("RelyingPartyIdentityProviders", (object) relyingPartyIdentityProvider);
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRelyingPartyKeys(RelyingPartyKey relyingPartyKey) => this.AddObject("RelyingPartyKeys", (object) relyingPartyKey);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToServiceIdentities(ServiceIdentity serviceIdentity) => this.AddObject("ServiceIdentities", (object) serviceIdentity);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToServiceIdentityKeys(ServiceIdentityKey serviceIdentityKey) => this.AddObject("ServiceIdentityKeys", (object) serviceIdentityKey);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRuleGroups(RuleGroup ruleGroup) => this.AddObject("RuleGroups", (object) ruleGroup);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRelyingPartyRuleGroups(RelyingPartyRuleGroup relyingPartyRuleGroup) => this.AddObject("RelyingPartyRuleGroups", (object) relyingPartyRuleGroup);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToIssuers(Issuer issuer) => this.AddObject("Issuers", (object) issuer);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToRules(Rule rule) => this.AddObject("Rules", (object) rule);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public void AddToConditionalRules(ConditionalRule conditionalRule) => this.AddObject("ConditionalRules", (object) conditionalRule);
  }
}
