// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.ServiceIdentity
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
  [EntitySet("ServiceIdentities")]
  [DataServiceKey("Id")]
  public class ServiceIdentity : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Name;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Description;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _RedirectAddress;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<Delegation> _Delegations = new DataServiceCollection<Delegation>((IEnumerable<Delegation>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<ServiceIdentityKey> _ServiceIdentityKeys = new DataServiceCollection<ServiceIdentityKey>((IEnumerable<ServiceIdentityKey>) null, TrackingMode.None);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static ServiceIdentity CreateServiceIdentity(long ID, bool systemReserved) => new ServiceIdentity()
    {
      Id = ID,
      SystemReserved = systemReserved
    };

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
    public string RedirectAddress
    {
      get => this._RedirectAddress;
      set
      {
        this._RedirectAddress = value;
        this.OnPropertyChanged(nameof (RedirectAddress));
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
    public DataServiceCollection<ServiceIdentityKey> ServiceIdentityKeys
    {
      get => this._ServiceIdentityKeys;
      set
      {
        this._ServiceIdentityKeys = value;
        this.OnPropertyChanged(nameof (ServiceIdentityKeys));
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
