// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.RelyingPartyAddress
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  [EntitySet("RelyingPartyAddresses")]
  [DataServiceKey("Id")]
  public class RelyingPartyAddress : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _RelyingPartyId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _EndpointType;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Address;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private RelyingParty _RelyingParty;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static RelyingPartyAddress CreateRelyingPartyAddress(
      long ID,
      long relyingPartyId,
      bool systemReserved)
    {
      return new RelyingPartyAddress()
      {
        Id = ID,
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
    public string EndpointType
    {
      get => this._EndpointType;
      set
      {
        this._EndpointType = value;
        this.OnPropertyChanged(nameof (EndpointType));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string Address
    {
      get => this._Address;
      set
      {
        this._Address = value;
        this.OnPropertyChanged(nameof (Address));
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
