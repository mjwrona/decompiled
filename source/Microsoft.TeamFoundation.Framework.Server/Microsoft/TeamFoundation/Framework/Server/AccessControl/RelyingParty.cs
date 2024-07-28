// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.RelyingParty
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Services.Client;
using System.Data.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  [EntitySet("RelyingParties")]
  [DataServiceKey("Id")]
  public class RelyingParty : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Name;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _DisplayName;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Description;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _TokenType;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private int _TokenLifetime;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _AsymmetricTokenEncryptionRequired;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<Delegation> _Delegations = new DataServiceCollection<Delegation>((IEnumerable<Delegation>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<RelyingPartyAddress> _RelyingPartyAddresses = new DataServiceCollection<RelyingPartyAddress>((IEnumerable<RelyingPartyAddress>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<RelyingPartyIdentityProvider> _RelyingPartyIdentityProviders = new DataServiceCollection<RelyingPartyIdentityProvider>((IEnumerable<RelyingPartyIdentityProvider>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<RelyingPartyKey> _RelyingPartyKeys = new DataServiceCollection<RelyingPartyKey>((IEnumerable<RelyingPartyKey>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<RelyingPartyRuleGroup> _RelyingPartyRuleGroups = new DataServiceCollection<RelyingPartyRuleGroup>((IEnumerable<RelyingPartyRuleGroup>) null, TrackingMode.None);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static RelyingParty CreateRelyingParty(
      long ID,
      int tokenLifetime,
      bool asymmetricTokenEncryptionRequired,
      bool systemReserved)
    {
      return new RelyingParty()
      {
        Id = ID,
        TokenLifetime = tokenLifetime,
        AsymmetricTokenEncryptionRequired = asymmetricTokenEncryptionRequired,
        SystemReserved = systemReserved
      };
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public long Id
    {
      get => this._Id;
      set
      {
        this._Id = value;
        this.OnPropertyChanged(nameof (Id));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string Name
    {
      get => this._Name;
      set
      {
        this._Name = value;
        this.OnPropertyChanged(nameof (Name));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string DisplayName
    {
      get => this._DisplayName;
      set
      {
        this._DisplayName = value;
        this.OnPropertyChanged(nameof (DisplayName));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string Description
    {
      get => this._Description;
      set
      {
        this._Description = value;
        this.OnPropertyChanged(nameof (Description));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string TokenType
    {
      get => this._TokenType;
      set
      {
        this._TokenType = value;
        this.OnPropertyChanged(nameof (TokenType));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public int TokenLifetime
    {
      get => this._TokenLifetime;
      set
      {
        this._TokenLifetime = value;
        this.OnPropertyChanged(nameof (TokenLifetime));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public bool AsymmetricTokenEncryptionRequired
    {
      get => this._AsymmetricTokenEncryptionRequired;
      set
      {
        this._AsymmetricTokenEncryptionRequired = value;
        this.OnPropertyChanged(nameof (AsymmetricTokenEncryptionRequired));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public bool SystemReserved
    {
      get => this._SystemReserved;
      set
      {
        this._SystemReserved = value;
        this.OnPropertyChanged(nameof (SystemReserved));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public byte[] Version
    {
      get => this._Version != null ? (byte[]) this._Version.Clone() : (byte[]) null;
      set
      {
        this._Version = value;
        this.OnPropertyChanged(nameof (Version));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<Delegation> Delegations
    {
      get => this._Delegations;
      set
      {
        this._Delegations = value;
        this.OnPropertyChanged(nameof (Delegations));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<RelyingPartyAddress> RelyingPartyAddresses
    {
      get => this._RelyingPartyAddresses;
      set
      {
        this._RelyingPartyAddresses = value;
        this.OnPropertyChanged(nameof (RelyingPartyAddresses));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<RelyingPartyIdentityProvider> RelyingPartyIdentityProviders
    {
      get => this._RelyingPartyIdentityProviders;
      set
      {
        this._RelyingPartyIdentityProviders = value;
        this.OnPropertyChanged(nameof (RelyingPartyIdentityProviders));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<RelyingPartyKey> RelyingPartyKeys
    {
      get => this._RelyingPartyKeys;
      set
      {
        this._RelyingPartyKeys = value;
        this.OnPropertyChanged(nameof (RelyingPartyKeys));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<RelyingPartyRuleGroup> RelyingPartyRuleGroups
    {
      get => this._RelyingPartyRuleGroups;
      set
      {
        this._RelyingPartyRuleGroups = value;
        this.OnPropertyChanged(nameof (RelyingPartyRuleGroups));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public event PropertyChangedEventHandler PropertyChanged;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    protected virtual void OnPropertyChanged(string property)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(property));
    }
  }
}
