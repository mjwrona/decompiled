// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.IdentityProvider
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
  [EntitySet("IdentityProviders")]
  [DataServiceKey("Id")]
  public class IdentityProvider : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _IssuerId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _DisplayName;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Description;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _WebSSOProtocolType;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Realm;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _LoginLinkName;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _LoginParameters;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<IdentityProviderAddress> _IdentityProviderAddresses = new DataServiceCollection<IdentityProviderAddress>((IEnumerable<IdentityProviderAddress>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<IdentityProviderClaimType> _IdentityProviderClaimTypes = new DataServiceCollection<IdentityProviderClaimType>((IEnumerable<IdentityProviderClaimType>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<IdentityProviderKey> _IdentityProviderKeys = new DataServiceCollection<IdentityProviderKey>((IEnumerable<IdentityProviderKey>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private Issuer _Issuer;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<RelyingPartyIdentityProvider> _RelyingPartyIdentityProviders = new DataServiceCollection<RelyingPartyIdentityProvider>((IEnumerable<RelyingPartyIdentityProvider>) null, TrackingMode.None);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static IdentityProvider CreateIdentityProvider(
      long ID,
      long issuerId,
      bool systemReserved)
    {
      return new IdentityProvider()
      {
        Id = ID,
        IssuerId = issuerId,
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
    public long IssuerId
    {
      get => this._IssuerId;
      set
      {
        this._IssuerId = value;
        this.OnPropertyChanged(nameof (IssuerId));
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
    public string WebSSOProtocolType
    {
      get => this._WebSSOProtocolType;
      set
      {
        this._WebSSOProtocolType = value;
        this.OnPropertyChanged(nameof (WebSSOProtocolType));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string Realm
    {
      get => this._Realm;
      set
      {
        this._Realm = value;
        this.OnPropertyChanged(nameof (Realm));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string LoginLinkName
    {
      get => this._LoginLinkName;
      set
      {
        this._LoginLinkName = value;
        this.OnPropertyChanged(nameof (LoginLinkName));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string LoginParameters
    {
      get => this._LoginParameters;
      set
      {
        this._LoginParameters = value;
        this.OnPropertyChanged(nameof (LoginParameters));
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
    public DataServiceCollection<IdentityProviderAddress> IdentityProviderAddresses
    {
      get => this._IdentityProviderAddresses;
      set
      {
        this._IdentityProviderAddresses = value;
        this.OnPropertyChanged(nameof (IdentityProviderAddresses));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<IdentityProviderClaimType> IdentityProviderClaimTypes
    {
      get => this._IdentityProviderClaimTypes;
      set
      {
        this._IdentityProviderClaimTypes = value;
        this.OnPropertyChanged(nameof (IdentityProviderClaimTypes));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<IdentityProviderKey> IdentityProviderKeys
    {
      get => this._IdentityProviderKeys;
      set
      {
        this._IdentityProviderKeys = value;
        this.OnPropertyChanged(nameof (IdentityProviderKeys));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public Issuer Issuer
    {
      get => this._Issuer;
      set
      {
        this._Issuer = value;
        this.OnPropertyChanged(nameof (Issuer));
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
