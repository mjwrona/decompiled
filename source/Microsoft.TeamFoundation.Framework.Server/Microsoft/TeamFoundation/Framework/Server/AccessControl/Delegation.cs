// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.Delegation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  [EntitySet("Delegations")]
  [DataServiceKey("Id")]
  public class Delegation : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _ServiceIdentityId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _RelyingPartyId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _IdentityProvider;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _NameIdentifier;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Scope;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _AuthorizationCode;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private RelyingParty _RelyingParty;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private ServiceIdentity _ServiceIdentity;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static Delegation CreateDelegation(
      long ID,
      long serviceIdentityId,
      long relyingPartyId,
      bool systemReserved)
    {
      return new Delegation()
      {
        Id = ID,
        ServiceIdentityId = serviceIdentityId,
        RelyingPartyId = relyingPartyId,
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
    public long ServiceIdentityId
    {
      get => this._ServiceIdentityId;
      set
      {
        this._ServiceIdentityId = value;
        this.OnPropertyChanged(nameof (ServiceIdentityId));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public long RelyingPartyId
    {
      get => this._RelyingPartyId;
      set
      {
        this._RelyingPartyId = value;
        this.OnPropertyChanged(nameof (RelyingPartyId));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string IdentityProvider
    {
      get => this._IdentityProvider;
      set
      {
        this._IdentityProvider = value;
        this.OnPropertyChanged(nameof (IdentityProvider));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string NameIdentifier
    {
      get => this._NameIdentifier;
      set
      {
        this._NameIdentifier = value;
        this.OnPropertyChanged(nameof (NameIdentifier));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string Scope
    {
      get => this._Scope;
      set
      {
        this._Scope = value;
        this.OnPropertyChanged(nameof (Scope));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string AuthorizationCode
    {
      get => this._AuthorizationCode;
      set
      {
        this._AuthorizationCode = value;
        this.OnPropertyChanged(nameof (AuthorizationCode));
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
    public RelyingParty RelyingParty
    {
      get => this._RelyingParty;
      set
      {
        this._RelyingParty = value;
        this.OnPropertyChanged(nameof (RelyingParty));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public ServiceIdentity ServiceIdentity
    {
      get => this._ServiceIdentity;
      set
      {
        this._ServiceIdentity = value;
        this.OnPropertyChanged(nameof (ServiceIdentity));
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
