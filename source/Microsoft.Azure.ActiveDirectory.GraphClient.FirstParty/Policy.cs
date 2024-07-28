// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.Policy
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [ExcludeFromCodeCoverage]
  [Entity("policys", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.Policy", "Microsoft.DirectoryServices.Policy"})]
  [JsonObject(MemberSerialization.OptIn)]
  public class Policy : DirectoryObject
  {
    private string _displayName;
    private ChangeTrackingCollection<KeyCredential> _keyCredentials;
    private bool _keyCredentialsInitialized;
    private int? _policyType;
    private ChangeTrackingCollection<string> _policyDetail;
    private bool _policyDetailInitialized;
    private int? _tenantDefaultPolicy;
    private ChangeTrackingCollection<GraphObject> _policyAppliedTo;
    private bool _policyAppliedToInitialized;

    public Policy() => this.ODataTypeName = "Microsoft.WindowsAzure.ActiveDirectory.Policy";

    public Policy(string objectId)
      : this()
    {
      this.ObjectId = objectId;
    }

    [JsonProperty("displayName")]
    public string DisplayName
    {
      get => this._displayName;
      set
      {
        this._displayName = value;
        this.ChangedProperties.Add(nameof (DisplayName));
      }
    }

    [JsonProperty("keyCredentials")]
    public ChangeTrackingCollection<KeyCredential> KeyCredentials
    {
      get
      {
        if (this._keyCredentials == null)
          this._keyCredentials = new ChangeTrackingCollection<KeyCredential>();
        if (!this._keyCredentialsInitialized)
        {
          this._keyCredentials.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (KeyCredentials)));
          this._keyCredentials.ToList<KeyCredential>().ForEach((Action<KeyCredential>) (x => x.ItemChanged += (EventHandler) ((sender, args) => this.ChangedProperties.Add(nameof (KeyCredentials)))));
          this._keyCredentialsInitialized = true;
        }
        return this._keyCredentials;
      }
      set
      {
        this._keyCredentials = value;
        this.ChangedProperties.Add(nameof (KeyCredentials));
      }
    }

    [JsonProperty("policyType")]
    public int? PolicyType
    {
      get => this._policyType;
      set
      {
        this._policyType = value;
        this.ChangedProperties.Add(nameof (PolicyType));
      }
    }

    [JsonProperty("policyDetail")]
    public ChangeTrackingCollection<string> PolicyDetail
    {
      get
      {
        if (this._policyDetail == null)
          this._policyDetail = new ChangeTrackingCollection<string>();
        if (!this._policyDetailInitialized)
        {
          this._policyDetail.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PolicyDetail)));
          this._policyDetailInitialized = true;
        }
        return this._policyDetail;
      }
      set
      {
        this._policyDetail = value;
        this.ChangedProperties.Add(nameof (PolicyDetail));
      }
    }

    [JsonProperty("tenantDefaultPolicy")]
    public int? TenantDefaultPolicy
    {
      get => this._tenantDefaultPolicy;
      set
      {
        this._tenantDefaultPolicy = value;
        this.ChangedProperties.Add(nameof (TenantDefaultPolicy));
      }
    }

    [JsonConverter(typeof (AadJsonConverter))]
    [Link("policyAppliedTo", false)]
    [JsonProperty("policyAppliedTo")]
    public ChangeTrackingCollection<GraphObject> PolicyAppliedTo
    {
      get
      {
        if (this._policyAppliedTo == null)
          this._policyAppliedTo = new ChangeTrackingCollection<GraphObject>();
        if (!this._policyAppliedToInitialized)
        {
          this._policyAppliedTo.CollectionChanged += (EventHandler) ((sender, s) => this.ChangedProperties.Add(nameof (PolicyAppliedTo)));
          this._policyAppliedToInitialized = true;
        }
        return this._policyAppliedTo;
      }
      set
      {
        this._policyAppliedTo = value;
        this.ChangedProperties.Add(nameof (PolicyAppliedTo));
      }
    }
  }
}
