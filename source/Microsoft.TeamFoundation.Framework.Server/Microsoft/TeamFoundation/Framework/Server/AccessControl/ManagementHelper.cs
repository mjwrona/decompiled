// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.ManagementHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  public class ManagementHelper
  {
    private string m_managementClientSecurityToken;
    private readonly ManagementService m_managementService;
    private readonly string m_managementClientName;
    private readonly string m_managementClientPassword;
    private readonly string m_managementServiceUrl;
    private readonly string m_proxyUrl;
    private readonly string m_wsFederationMetadataUrl;
    private static readonly string[] DefaultRulesClaims = new string[3]
    {
      "emailaddress",
      "name",
      "nameidentifier"
    };
    private static readonly Dictionary<string, Dictionary<string, string>> InputClaimMappings = new Dictionary<string, Dictionary<string, string>>()
    {
      {
        "Windows Live ID",
        new Dictionary<string, string>()
        {
          {
            "name",
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          }
        }
      },
      {
        "Windows Azure AD",
        new Dictionary<string, string>()
        {
          {
            "emailaddress",
            "http://schemas.xmlsoap.org/claims/EmailAddress"
          },
          {
            "name",
            "http://schemas.xmlsoap.org/claims/EmailAddress"
          }
        }
      },
      {
        "LiveInt",
        new Dictionary<string, string>()
        {
          {
            "emailaddress",
            "http://schemas.xmlsoap.org/claims/EmailAddress"
          },
          {
            "name",
            "http://schemas.xmlsoap.org/claims/EmailAddress"
          },
          {
            "nameidentifier",
            "http://schemas.xmlsoap.org/claims/EmailAddress"
          }
        }
      }
    };
    private const string ManagementServiceSuffix = "/v2/mgmt/service";
    private const string WSFederationMetadataSuffix = "/FederationMetadata/2007-06/FederationMetadata.xml";
    private const string ManagementRelyingPartySuffix = "/mgmt";
    private const string ManagementRelyingPartySuffixV2 = "/v2/mgmt/service";
    private const string KeyTypesPassword = "Password";
    private const string KeyTypesSymmetric = "Symmetric";
    private const string KeyTypesX509Certificate = "X509Certificate";
    private const string KeyUsagesSigning = "Signing";
    private const string KeyUsagesPassword = "Password";
    private const string EndPointTypesRealm = "Realm";
    private const string RuleTypesSimple = "Simple";
    private const string RuleTypesPassthrough = "Passthrough";
    private const string PassthroughRuleDescriptionFormat = "Passthrough \"{0}\" claim from \"{1}\" as \"{2}\"";
    private const string IdentityProviderRuleDescriptionFormat = "Identity Provider Name for \"{0}\"";
    private const string LocalAuthorityIssuer = "LOCAL AUTHORITY";
    private HttpRetryHelper m_retryHelper;

    public ManagementHelper(
      string serviceUrl,
      string name,
      string password,
      string proxyUrl,
      int retries = 10)
    {
      this.m_managementClientName = name;
      this.m_managementClientPassword = password;
      this.m_managementServiceUrl = serviceUrl;
      this.m_proxyUrl = proxyUrl;
      this.m_managementService = new ManagementService(new Uri(ManagementHelper.Combine(serviceUrl, "/v2/mgmt/service")));
      this.m_managementService.SendingRequest += new EventHandler<SendingRequestEventArgs>(this.OnSendingRequest);
      this.m_wsFederationMetadataUrl = ManagementHelper.Combine(serviceUrl, "/FederationMetadata/2007-06/FederationMetadata.xml");
      this.m_retryHelper = (HttpRetryHelper) new AcsRetryHelper(retries);
    }

    public string WSFederationMetadataUrl => this.m_wsFederationMetadataUrl;

    public string ManagementClientName => this.m_managementClientName;

    public string ManagementClientPassword => this.m_managementClientPassword;

    public void AddNamePasswordServiceIdentity(
      string name,
      string password,
      string relyingPartyRealm)
    {
      this.DeleteServiceIdentityByNameIfExists(name);
      this.DeleteRulesForServiceIdentity(name, relyingPartyRealm);
      ServiceIdentityKey serviceIdentityKey = new ServiceIdentityKey()
      {
        Type = "Password",
        Usage = "Password",
        Value = Encoding.UTF8.GetBytes(password),
        DisplayName = "Password"
      };
      this.AddServiceIdentity(name, new ServiceIdentityKey[1]
      {
        serviceIdentityKey
      });
    }

    public string AddSymmetricKeyServiceIdentity(string name, string relyingPartyRealm)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(relyingPartyRealm, nameof (relyingPartyRealm));
      byte[] symmetricKey = ManagementHelper.GenerateSymmetricKey();
      this.DeleteServiceIdentityByNameIfExists(name);
      this.DeleteRulesForServiceIdentity(name, relyingPartyRealm);
      string base64String = Convert.ToBase64String(symmetricKey);
      ServiceIdentityKey[] keys = new ServiceIdentityKey[2]
      {
        new ServiceIdentityKey()
        {
          Type = "Symmetric",
          Usage = "Signing",
          Value = symmetricKey,
          DisplayName = "Symmetric"
        },
        new ServiceIdentityKey()
        {
          Type = "Password",
          Usage = "Password",
          Value = Encoding.UTF8.GetBytes(base64String),
          DisplayName = "Password"
        }
      };
      this.AddServiceIdentity(name, keys);
      return base64String;
    }

    public string AddSymmetricSigningKeyToRelyingParty(
      string keyName,
      RelyingParty relyingParty,
      string relyingPartyRealm = null,
      byte[] signingKey = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(keyName, nameof (keyName));
      ArgumentUtility.CheckForNull<RelyingParty>(relyingParty, nameof (relyingParty));
      if (signingKey == null)
      {
        RelyingPartyKey relyingPartyKey = (RelyingPartyKey) null;
        if (relyingPartyRealm != null && relyingPartyRealm.StartsWith("http://app.", StringComparison.OrdinalIgnoreCase))
          relyingPartyKey = this.GetPrimarySymmetricSigningKeyIfExists(relyingPartyRealm.Substring("http://app".Length));
        else if (relyingPartyRealm != null && relyingPartyRealm.StartsWith("https://app.", StringComparison.OrdinalIgnoreCase))
          relyingPartyKey = this.GetPrimarySymmetricSigningKeyIfExists(relyingPartyRealm.Substring("https://app".Length));
        signingKey = relyingPartyKey == null ? ManagementHelper.GenerateSymmetricKey() : relyingPartyKey.Value;
      }
      RelyingPartyKey target = new RelyingPartyKey()
      {
        DisplayName = keyName,
        Type = "Symmetric",
        Usage = "Signing",
        Value = signingKey,
        RelyingParty = relyingParty,
        IsPrimary = true
      };
      this.m_managementService.AddRelatedObject((object) relyingParty, "RelyingPartyKeys", (object) target);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges()));
      return Convert.ToBase64String(signingKey);
    }

    public string AddSymmetricSigningKeyToRelyingParty(string keyName, string relyingPartyRealm)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(keyName, nameof (keyName));
      ArgumentUtility.CheckStringForNullOrEmpty(relyingPartyRealm, nameof (relyingPartyRealm));
      RelyingParty relyingPartyIfExists = this.GetRelyingPartyIfExists(relyingPartyRealm);
      if (relyingPartyRealm == null)
        throw new ArgumentException(FrameworkResources.RelyingPartyNotFound());
      return this.AddSymmetricSigningKeyToRelyingParty(keyName, relyingPartyIfExists, relyingPartyRealm);
    }

    public RelyingPartyKey GetSymmetricSigningKeyIfExists(string keyName, RelyingParty relyingParty)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(keyName, nameof (keyName));
      ArgumentUtility.CheckForNull<RelyingParty>(relyingParty, nameof (relyingParty));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return this.m_retryHelper.Invoke<RelyingPartyKey>((Func<RelyingPartyKey>) (() => this.m_managementService.RelyingPartyKeys.Where<RelyingPartyKey>((Expression<Func<RelyingPartyKey, bool>>) (rpk => rpk.RelyingPartyId == this.relyingParty.Id && rpk.DisplayName == this.keyName && rpk.Type == "Symmetric" && rpk.Usage == "Signing")).FirstOrDefault<RelyingPartyKey>()));
    }

    public RelyingPartyKey GetPrimarySymmetricSigningKeyIfExists(string relyingPartyRealm)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(relyingPartyRealm, nameof (relyingPartyRealm));
      relyingPartyRealm = relyingPartyRealm.ToLower(CultureInfo.InvariantCulture);
      // ISSUE: reference to a compiler-generated field
      RelyingPartyAddress relyingPartyAddress = this.m_retryHelper.Invoke<RelyingPartyAddress>((Func<RelyingPartyAddress>) (() => this.m_managementService.RelyingPartyAddresses.Where<RelyingPartyAddress>((Expression<Func<RelyingPartyAddress, bool>>) (rpa => rpa.Address == this.relyingPartyRealm && rpa.EndpointType == "Realm")).FirstOrDefault<RelyingPartyAddress>()));
      RelyingPartyKey relyingPartyKey = (RelyingPartyKey) null;
      if (relyingPartyAddress != null)
      {
        // ISSUE: reference to a compiler-generated field
        this.m_retryHelper.Invoke((Action) (() => relyingPartyKey = this.m_managementService.RelyingPartyKeys.Where<RelyingPartyKey>((Expression<Func<RelyingPartyKey, bool>>) (rpk => rpk.RelyingPartyId == this.relyingPartyAddress.RelyingPartyId && rpk.IsPrimary && rpk.Type == "Symmetric" && rpk.Usage == "Signing")).FirstOrDefault<RelyingPartyKey>()));
      }
      return relyingPartyKey;
    }

    public List<ServiceKey> GetSigningCertificates() => this.GetServiceKeys().Where<ServiceKey>((Func<ServiceKey, bool>) (sk => ManagementHelper.IsSigningCertificate(sk))).ToList<ServiceKey>();

    public static X509Certificate2 CreateCertificateFrom(ServiceKey serviceKey)
    {
      SecureString password = string.Equals(serviceKey.Type, "X509Certificate", StringComparison.OrdinalIgnoreCase) ? ManagementHelper.CreateSecureString(serviceKey.Password) : throw new ArgumentException(FrameworkResources.ServiceKeyNotCertificateException());
      return new X509Certificate2(serviceKey.Value, password);
    }

    public void AddSigningCertificate(byte[] pfxCertBytes, byte[] password, bool isPrimary)
    {
      SecureString secureString = ManagementHelper.CreateSecureString(password);
      X509Certificate2 x509Certificate2 = new X509Certificate2(pfxCertBytes, secureString);
      this.m_managementService.AddToServiceKeys(new ServiceKey()
      {
        Type = "X509Certificate",
        Usage = "Signing",
        Value = pfxCertBytes,
        Password = password,
        IsPrimary = isPrimary,
        StartDate = x509Certificate2.NotBefore.ToUniversalTime(),
        EndDate = x509Certificate2.NotAfter.ToUniversalTime(),
        DisplayName = x509Certificate2.Thumbprint
      });
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public void UpdateServiceKeys(IEnumerable<ServiceKey> serviceKeys)
    {
      foreach (object serviceKey in serviceKeys)
        this.m_managementService.UpdateObject(serviceKey);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public void DeleteServiceKeys(IEnumerable<ServiceKey> serviceKeys)
    {
      foreach (object serviceKey in serviceKeys)
        this.m_managementService.DeleteObject(serviceKey);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public void AddServiceIdentity(string name, ServiceIdentityKey[] keys)
    {
      ServiceIdentity serviceIdentity = new ServiceIdentity()
      {
        Name = name
      };
      this.m_managementService.AddToServiceIdentities(serviceIdentity);
      foreach (ServiceIdentityKey key in keys)
        this.m_managementService.AddRelatedObject((object) serviceIdentity, "ServiceIdentityKeys", (object) key);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public void DeleteServiceIdentityByNameIfExists(string serviceIdentityName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceIdentityName, nameof (serviceIdentityName));
      foreach (object entity in this.m_retryHelper.Invoke<List<ServiceIdentity>>((Func<List<ServiceIdentity>>) (() => this.m_managementService.ServiceIdentities.AddQueryOption("$filter", (object) ("Name eq '" + serviceIdentityName.Replace("'", "''") + "'")).ToList<ServiceIdentity>())))
        this.m_managementService.DeleteObject(entity);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public ServiceIdentity GetServiceIdentity(string serviceIdentityName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceIdentityName, nameof (serviceIdentityName));
      return this.m_retryHelper.Invoke<ServiceIdentity>((Func<ServiceIdentity>) (() => this.m_managementService.ServiceIdentities.AddQueryOption("$filter", (object) ("Name eq '" + serviceIdentityName.Replace("'", "''") + "'")).FirstOrDefault<ServiceIdentity>()));
    }

    public ServiceIdentity[] GetServiceIdentities(string[] serviceIdentityNames)
    {
      ArgumentUtility.CheckForNull<string[]>(serviceIdentityNames, nameof (serviceIdentityNames));
      ServiceIdentity[] serviceIdentities = new ServiceIdentity[serviceIdentityNames.Length];
      for (int i = 0; i < serviceIdentityNames.Length; i++)
      {
        if (serviceIdentityNames[i] != null)
        {
          ServiceIdentity serviceIdentity = this.m_retryHelper.Invoke<ServiceIdentity>((Func<ServiceIdentity>) (() => this.m_managementService.ServiceIdentities.AddQueryOption("$filter", (object) ("Name eq '" + serviceIdentityNames[i].Replace("'", "''") + "'")).FirstOrDefault<ServiceIdentity>()));
          if (serviceIdentity != null)
            serviceIdentities[i] = serviceIdentity;
        }
      }
      return serviceIdentities;
    }

    public RelyingParty AddRelyingParty(
      string name,
      string realm,
      string returnUrl,
      RuleGroup ruleGroup,
      string tokenType = "SAML_2_0",
      bool asymmetricTokenEncryptionRequired = false,
      bool symmetricTokenEncryptionRequired = false,
      int tokenLifetimeInSeconds = 28800)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(realm, nameof (realm));
      ArgumentUtility.CheckStringForNullOrEmpty(returnUrl, nameof (returnUrl));
      ArgumentUtility.CheckForNull<RuleGroup>(ruleGroup, nameof (ruleGroup));
      ArgumentUtility.CheckStringForNullOrEmpty(tokenType, nameof (tokenType));
      RelyingParty relyingParty = new RelyingParty()
      {
        Name = realm,
        DisplayName = name,
        Description = name,
        TokenType = tokenType,
        TokenLifetime = tokenLifetimeInSeconds,
        AsymmetricTokenEncryptionRequired = asymmetricTokenEncryptionRequired
      };
      this.m_managementService.AddObject("RelyingParties", (object) relyingParty);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
      RelyingPartyRuleGroup target1 = new RelyingPartyRuleGroup()
      {
        RelyingPartyId = relyingParty.Id,
        RuleGroupId = ruleGroup.Id
      };
      this.m_managementService.AddRelatedObject((object) relyingParty, "RelyingPartyRuleGroups", (object) target1);
      RelyingPartyAddress target2 = new RelyingPartyAddress()
      {
        Address = realm,
        EndpointType = "Realm",
        RelyingParty = relyingParty
      };
      RelyingPartyAddress target3 = new RelyingPartyAddress()
      {
        Address = returnUrl,
        EndpointType = "Reply",
        RelyingParty = relyingParty
      };
      this.m_managementService.AddRelatedObject((object) relyingParty, "RelyingPartyAddresses", (object) target2);
      this.m_managementService.AddRelatedObject((object) relyingParty, "RelyingPartyAddresses", (object) target3);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
      foreach (IdentityProvider identityProvider in this.m_retryHelper.Invoke<List<IdentityProvider>>((Func<List<IdentityProvider>>) (() => this.m_managementService.IdentityProviders.ToList<IdentityProvider>())))
        this.m_managementService.AddToRelyingPartyIdentityProviders(new RelyingPartyIdentityProvider()
        {
          IdentityProviderId = identityProvider.Id,
          RelyingPartyId = relyingParty.Id
        });
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
      return relyingParty;
    }

    public void AddRuleGroupToRelyingParty(RuleGroup ruleGroup, RelyingParty relyingParty)
    {
      ArgumentUtility.CheckForNull<RelyingParty>(relyingParty, nameof (relyingParty));
      ArgumentUtility.CheckForNull<RuleGroup>(ruleGroup, nameof (ruleGroup));
      this.m_managementService.AddToRelyingPartyRuleGroups(new RelyingPartyRuleGroup()
      {
        RelyingPartyId = relyingParty.Id,
        RuleGroupId = ruleGroup.Id
      });
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges()));
    }

    public bool DeleteRelyingPartyIfExists(string relyingPartyRealm)
    {
      RelyingParty relyingPartyIfExists = this.GetRelyingPartyIfExists(relyingPartyRealm);
      if (relyingPartyIfExists != null)
      {
        this.m_managementService.DeleteObject((object) relyingPartyIfExists);
        this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges()));
      }
      return relyingPartyIfExists != null;
    }

    public RelyingParty GetRelyingPartyIfExists(string relyingPartyRealm)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(relyingPartyRealm, nameof (relyingPartyRealm));
      RelyingParty relyingParty = (RelyingParty) null;
      relyingPartyRealm = relyingPartyRealm.ToLower(CultureInfo.InvariantCulture);
      this.m_retryHelper.Invoke((Action) (() =>
      {
        // ISSUE: reference to a compiler-generated field
        RelyingPartyAddress relyingPartyAddress = this.m_managementService.RelyingPartyAddresses.Where<RelyingPartyAddress>((Expression<Func<RelyingPartyAddress, bool>>) (rpa => rpa.Address == this.relyingPartyRealm && rpa.EndpointType == "Realm")).FirstOrDefault<RelyingPartyAddress>();
        if (relyingPartyAddress == null)
          return;
        relyingParty = this.m_managementService.RelyingParties.Where<RelyingParty>((Expression<Func<RelyingParty, bool>>) (rp => rp.Id == relyingPartyAddress.RelyingPartyId)).FirstOrDefault<RelyingParty>();
      }));
      return relyingParty;
    }

    public void AddRulesForServiceIdentity(
      string serviceIdentity,
      string securityIdentifier,
      string relyingPartyRealm)
    {
      RuleGroup source = this.GetRuleGroupsForRealm(relyingPartyRealm).FirstOrDefault<RuleGroup>();
      Issuer issuer = this.m_retryHelper.Invoke<Issuer>((Func<Issuer>) (() => this.m_managementService.Issuers.AddQueryOption("$filter", (object) "Name eq 'LOCAL AUTHORITY'").First<Issuer>()));
      Rule target1 = new Rule()
      {
        IssuerId = issuer.Id,
        InputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        InputClaimValue = serviceIdentity,
        OutputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        OutputClaimValue = serviceIdentity,
        RuleGroup = source,
        Description = FrameworkResources.ServiceIdentityClaimRuleDescription((object) serviceIdentity)
      };
      this.m_managementService.AddRelatedObject((object) source, "Rules", (object) target1);
      Rule target2 = new Rule()
      {
        IssuerId = issuer.Id,
        InputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        InputClaimValue = serviceIdentity,
        OutputClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        OutputClaimValue = securityIdentifier,
        RuleGroup = source,
        Description = FrameworkResources.ServiceIdentityClaimRuleDescription((object) serviceIdentity)
      };
      this.m_managementService.AddRelatedObject((object) source, "Rules", (object) target2);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public void AddRules(string identity, Rule[] rules)
    {
      Issuer issuer = this.m_retryHelper.Invoke<Issuer>((Func<Issuer>) (() => this.m_managementService.Issuers.AddQueryOption("$filter", (object) "Name eq 'LOCAL AUTHORITY'").First<Issuer>()));
      foreach (Rule rule in rules)
      {
        if (rule.IssuerId == 0L)
          rule.IssuerId = issuer.Id;
        this.m_managementService.AddRelatedObject((object) rule.RuleGroup, "Rules", (object) rule);
      }
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public string GetServiceIdentitySecurityIdentifier(
      string serviceIdentity,
      string relyingPartyRealm)
    {
      return this.GetRulesForServiceIdentity(serviceIdentity, relyingPartyRealm).FirstOrDefault<Rule>((Func<Rule, bool>) (x => x.OutputClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"))?.OutputClaimValue;
    }

    public IList<Rule> GetRulesForServiceIdentity(string serviceIdentity, string relyingPartyRealm)
    {
      List<Rule> forServiceIdentity = new List<Rule>();
      foreach (RuleGroup ruleGroup1 in this.GetRuleGroupsForRealm(relyingPartyRealm))
      {
        RuleGroup ruleGroup = ruleGroup1;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<Rule> collection = (IEnumerable<Rule>) this.m_retryHelper.Invoke<List<Rule>>((Func<List<Rule>>) (() => this.m_managementService.Rules.Where<Rule>((Expression<Func<Rule, bool>>) (rule => rule.RuleGroupId == this.ruleGroup.Id && rule.InputClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && rule.InputClaimValue == this.CS\u0024\u003C\u003E8__locals1.serviceIdentity)).ToList<Rule>()));
        forServiceIdentity.AddRange(collection);
      }
      return (IList<Rule>) forServiceIdentity;
    }

    public void DeleteRulesForServiceIdentity(string serviceIdentity, string relyingPartyRealm)
    {
      foreach (object entity in (IEnumerable<Rule>) this.GetRulesForServiceIdentity(serviceIdentity, relyingPartyRealm))
        this.m_managementService.DeleteObject(entity);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public RuleGroup AddRuleGroup(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      RuleGroup ruleGroup = new RuleGroup() { Name = name };
      this.m_managementService.AddToRuleGroups(ruleGroup);
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
      return ruleGroup;
    }

    public bool DeleteRuleGroupIfExists(string name)
    {
      RuleGroup ruleGroupIfExists = this.GetRuleGroupIfExists(name);
      if (ruleGroupIfExists != null)
      {
        this.m_managementService.DeleteObject((object) ruleGroupIfExists);
        this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges()));
      }
      return ruleGroupIfExists != null;
    }

    public void AddDefaultRulesToGroup(RuleGroup ruleGroup)
    {
      List<IdentityProvider> identityProviderList = this.m_retryHelper.Invoke<List<IdentityProvider>>((Func<List<IdentityProvider>>) (() => this.m_managementService.IdentityProviders.ToList<IdentityProvider>()));
      List<Issuer> source1 = this.m_retryHelper.Invoke<List<Issuer>>((Func<List<Issuer>>) (() => this.m_managementService.Issuers.ToList<Issuer>()));
      Issuer localAuthority = source1.First<Issuer>((Func<Issuer, bool>) (issuer => issuer.Name.Equals("LOCAL AUTHORITY", StringComparison.Ordinal)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<Rule> source2 = this.m_retryHelper.Invoke<List<Rule>>((Func<List<Rule>>) (() => this.m_managementService.Rules.Where<Rule>((Expression<Func<Rule, bool>>) (rule => rule.RuleGroupId == this.ruleGroup.Id && rule.IssuerId != this.localAuthority.Id)).ToList<Rule>()));
      foreach (IdentityProvider identityProvider in identityProviderList)
      {
        IdentityProvider idp = identityProvider;
        Issuer issuer = source1.First<Issuer>((Func<Issuer, bool>) (item => item.Id == idp.IssuerId));
        foreach (string defaultRulesClaim in ManagementHelper.DefaultRulesClaims)
        {
          string claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/" + defaultRulesClaim;
          if (source2.FirstOrDefault<Rule>((Func<Rule, bool>) (r => r.IssuerId == issuer.Id && claimType.Equals(r.OutputClaimType, StringComparison.Ordinal))) == null)
          {
            string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Passthrough \"{0}\" claim from \"{1}\" as \"{2}\"", (object) defaultRulesClaim, (object) idp.DisplayName, (object) defaultRulesClaim);
            string str2 = claimType;
            string str3 = claimType;
            string str4;
            if (ManagementHelper.InputClaimMappings.ContainsKey(idp.DisplayName) && ManagementHelper.InputClaimMappings[idp.DisplayName].TryGetValue(defaultRulesClaim, out str4))
            {
              str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Passthrough \"{0}\" claim from \"{1}\" as \"{2}\"", (object) str2, (object) idp.DisplayName, (object) defaultRulesClaim);
              str2 = str4;
            }
            Rule target = new Rule()
            {
              IssuerId = issuer.Id,
              InputClaimType = str2,
              OutputClaimType = str3,
              RuleGroup = ruleGroup,
              Description = str1
            };
            this.m_managementService.AddRelatedObject((object) ruleGroup, "Rules", (object) target);
          }
        }
        if (source2.FirstOrDefault<Rule>((Func<Rule, bool>) (r => r.IssuerId == issuer.Id && "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider".Equals(r.OutputClaimType, StringComparison.Ordinal))) == null)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Identity Provider Name for \"{0}\"", (object) idp.DisplayName);
          Rule target = new Rule()
          {
            IssuerId = issuer.Id,
            OutputClaimType = "http://schemas.microsoft.com/teamfoundationserver/2010/12/claims/identityprovider",
            OutputClaimValue = idp.DisplayName,
            RuleGroup = ruleGroup,
            Description = str
          };
          this.m_managementService.AddRelatedObject((object) ruleGroup, "Rules", (object) target);
        }
      }
      this.m_retryHelper.Invoke((Action) (() => this.m_managementService.SaveChanges(SaveChangesOptions.Batch)));
    }

    public List<RuleGroup> GetRuleGroupsForRealm(string relyingPartyRealm)
    {
      List<RuleGroup> ruleGroupsForRealm = new List<RuleGroup>();
      RelyingParty relyingParty = this.GetRelyingPartyIfExists(relyingPartyRealm);
      if (relyingParty != null)
      {
        // ISSUE: reference to a compiler-generated field
        IEnumerable<RelyingPartyRuleGroup> relyingPartyRuleGroups = (IEnumerable<RelyingPartyRuleGroup>) this.m_retryHelper.Invoke<List<RelyingPartyRuleGroup>>((Func<List<RelyingPartyRuleGroup>>) (() => this.m_managementService.RelyingPartyRuleGroups.Where<RelyingPartyRuleGroup>((Expression<Func<RelyingPartyRuleGroup, bool>>) (x => x.RelyingPartyId == this.relyingParty.Id)).ToList<RelyingPartyRuleGroup>()));
        if (relyingPartyRuleGroups != null)
        {
          foreach (RelyingPartyRuleGroup relyingPartyRuleGroup1 in relyingPartyRuleGroups)
          {
            RelyingPartyRuleGroup relyingPartyRuleGroup = relyingPartyRuleGroup1;
            // ISSUE: reference to a compiler-generated field
            ruleGroupsForRealm.AddRange((IEnumerable<RuleGroup>) this.m_retryHelper.Invoke<List<RuleGroup>>((Func<List<RuleGroup>>) (() => this.m_managementService.RuleGroups.Where<RuleGroup>((Expression<Func<RuleGroup, bool>>) (rg => rg.Id == this.relyingPartyRuleGroup.RuleGroupId)).ToList<RuleGroup>())));
          }
        }
      }
      return ruleGroupsForRealm;
    }

    public RuleGroup GetRuleGroupIfExists(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      // ISSUE: reference to a compiler-generated field
      return this.m_retryHelper.Invoke<RuleGroup>((Func<RuleGroup>) (() => this.m_managementService.RuleGroups.Where<RuleGroup>((Expression<Func<RuleGroup, bool>>) (ruleGroup => ruleGroup.Name == this.name)).FirstOrDefault<RuleGroup>()));
    }

    public string GetServiceIdentityKey(ServiceIdentity serviceIdentity)
    {
      // ISSUE: reference to a compiler-generated field
      ServiceIdentityKey serviceIdentityKey = this.m_retryHelper.Invoke<ServiceIdentityKey>((Func<ServiceIdentityKey>) (() => this.m_managementService.ServiceIdentityKeys.Where<ServiceIdentityKey>((Expression<Func<ServiceIdentityKey, bool>>) (x => x.ServiceIdentityId == this.serviceIdentity.Id)).FirstOrDefault<ServiceIdentityKey>()));
      if (serviceIdentityKey != null)
      {
        switch (serviceIdentityKey.Type)
        {
          case "Password":
            return Encoding.UTF8.GetString(serviceIdentityKey.Value);
          case "Symmetric":
            return Convert.ToBase64String(serviceIdentityKey.Value);
        }
      }
      return (string) null;
    }

    public string GetServiceIdentityPassword(ServiceIdentity serviceIdentity)
    {
      // ISSUE: reference to a compiler-generated field
      ServiceIdentityKey serviceIdentityKey = this.m_retryHelper.Invoke<ServiceIdentityKey>((Func<ServiceIdentityKey>) (() => this.m_managementService.ServiceIdentityKeys.Where<ServiceIdentityKey>((Expression<Func<ServiceIdentityKey, bool>>) (x => x.ServiceIdentityId == this.serviceIdentity.Id && x.Type == "Password")).FirstOrDefault<ServiceIdentityKey>()));
      return serviceIdentityKey != null ? Encoding.UTF8.GetString(serviceIdentityKey.Value) : (string) null;
    }

    public bool IsValidPassword(ServiceIdentity serviceIdentity, string password)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(password, nameof (password));
      DateTime utcNow = DateTime.UtcNow;
      // ISSUE: reference to a compiler-generated field
      return ((IEnumerable<ServiceIdentityKey>) this.m_retryHelper.Invoke<ServiceIdentityKey[]>((Func<ServiceIdentityKey[]>) (() => this.m_managementService.ServiceIdentityKeys.Where<ServiceIdentityKey>((Expression<Func<ServiceIdentityKey, bool>>) (x => x.ServiceIdentityId == this.serviceIdentity.Id && x.Type == "Password")).ToArray<ServiceIdentityKey>()))).FirstOrDefault<ServiceIdentityKey>((Func<ServiceIdentityKey, bool>) (k => k.StartDate <= utcNow && k.EndDate >= utcNow && string.Equals(Encoding.UTF8.GetString(k.Value), password, StringComparison.Ordinal))) != null;
    }

    public string GetTokenFromAcs()
    {
      using (WebClient client = new WebClient())
      {
        client.BaseAddress = this.m_managementServiceUrl;
        if (!string.IsNullOrEmpty(this.m_proxyUrl))
          client.Proxy = (IWebProxy) new WebProxy(this.m_proxyUrl);
        return HttpUtility.UrlDecode(((IEnumerable<string>) Encoding.UTF8.GetString(this.m_retryHelper.Invoke<byte[]>((Func<byte[]>) (() => client.UploadValues("WRAPv0.9/", "POST", new NameValueCollection()
        {
          {
            "wrap_name",
            this.m_managementClientName
          },
          {
            "wrap_password",
            this.m_managementClientPassword
          },
          {
            "wrap_scope",
            ManagementHelper.Combine(this.m_managementServiceUrl, "/v2/mgmt/service")
          }
        })))).Split('&')).Single<string>((Func<string, bool>) (value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase))).Split('=')[1]);
      }
    }

    public static byte[] GenerateSymmetricKey()
    {
      byte[] data = new byte[32];
      new RNGCryptoServiceProvider().GetBytes(data);
      return data;
    }

    public static string GetWSFederationMetadataUrl(string serviceUrl) => ManagementHelper.Combine(serviceUrl, "/FederationMetadata/2007-06/FederationMetadata.xml");

    private List<ServiceKey> GetServiceKeys()
    {
      List<ServiceKey> serviceKeys = (List<ServiceKey>) null;
      this.m_retryHelper.Invoke((Action) (() => serviceKeys = this.m_managementService.ServiceKeys.ToList<ServiceKey>()));
      return serviceKeys;
    }

    private static bool IsSigningCertificate(ServiceKey serviceKey) => string.Equals(serviceKey.Type, "X509Certificate", StringComparison.OrdinalIgnoreCase) && string.Equals(serviceKey.Usage, "Signing", StringComparison.OrdinalIgnoreCase);

    private void OnSendingRequest(object sender, SendingRequestEventArgs args)
    {
      HttpWebRequest request = (HttpWebRequest) args.Request;
      if (this.m_managementClientSecurityToken == null)
      {
        int millisecondsTimeout = 300;
        int num = 3;
        while (!this.TryGetTokenFromAcs(out this.m_managementClientSecurityToken) && num-- != 0)
        {
          Thread.Sleep(millisecondsTimeout);
          millisecondsTimeout *= 2;
        }
      }
      if (this.m_managementClientSecurityToken != null)
        request.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=\"" + this.m_managementClientSecurityToken + "\"");
      if (string.IsNullOrEmpty(this.m_proxyUrl))
        return;
      request.Proxy = (IWebProxy) new WebProxy(this.m_proxyUrl);
    }

    private bool TryGetTokenFromAcs(out string swtToken)
    {
      swtToken = (string) null;
      try
      {
        swtToken = this.GetTokenFromAcs();
      }
      catch (WebException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, "AccessControlService", "Authentication", (Exception) ex);
        if (ex.Response != null)
        {
          using (StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream()))
            TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "AccessControlService", "Authentication", streamReader.ReadToEnd());
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, "AccessControlService", "Authentication", ex);
      }
      return swtToken != null;
    }

    private static string Combine(string baseUrl, string path)
    {
      StringBuilder stringBuilder = new StringBuilder(baseUrl, baseUrl.Length + path.Length);
      if (!string.IsNullOrEmpty(path))
      {
        if (stringBuilder[stringBuilder.Length - 1] == '/' && path[0] == '/')
          --stringBuilder.Length;
        stringBuilder.Append(path);
      }
      return stringBuilder.ToString();
    }

    private static SecureString CreateSecureString(byte[] utf8bytes)
    {
      if (utf8bytes == null)
        return (SecureString) null;
      char[] chars = Encoding.UTF8.GetChars(utf8bytes);
      SecureString secureString = new SecureString();
      for (int index = 0; index < chars.Length; ++index)
      {
        secureString.AppendChar(chars[index]);
        chars[index] = char.MinValue;
      }
      return secureString;
    }

    internal string SecurityToken => this.m_managementClientSecurityToken;
  }
}
