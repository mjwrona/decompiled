// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.RelyingPartyKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  [EntitySet("RelyingPartyKeys")]
  [DataServiceKey("Id")]
  public class RelyingPartyKey : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _RelyingPartyId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _DisplayName;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Usage;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Type;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Value;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Password;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _IsPrimary;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DateTime _StartDate;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DateTime _EndDate;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private RelyingParty _RelyingParty;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static RelyingPartyKey CreateRelyingPartyKey(
      long ID,
      long relyingPartyId,
      bool isPrimary,
      bool systemReserved,
      DateTime startDate,
      DateTime endDate)
    {
      return new RelyingPartyKey()
      {
        Id = ID,
        RelyingPartyId = relyingPartyId,
        IsPrimary = isPrimary,
        SystemReserved = systemReserved,
        StartDate = startDate,
        EndDate = endDate
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
    public string Usage
    {
      get => this._Usage;
      set
      {
        this._Usage = value;
        this.OnPropertyChanged(nameof (Usage));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string Type
    {
      get => this._Type;
      set
      {
        this._Type = value;
        this.OnPropertyChanged(nameof (Type));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public byte[] Value
    {
      get => this._Value != null ? (byte[]) this._Value.Clone() : (byte[]) null;
      set
      {
        this._Value = value;
        this.OnPropertyChanged(nameof (Value));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public byte[] Password
    {
      get => this._Password != null ? (byte[]) this._Password.Clone() : (byte[]) null;
      set
      {
        this._Password = value;
        this.OnPropertyChanged(nameof (Password));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public bool IsPrimary
    {
      get => this._IsPrimary;
      set
      {
        this._IsPrimary = value;
        this.OnPropertyChanged(nameof (IsPrimary));
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
    public DateTime StartDate
    {
      get => this._StartDate;
      set
      {
        this._StartDate = value;
        this.OnPropertyChanged(nameof (StartDate));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DateTime EndDate
    {
      get => this._EndDate;
      set
      {
        this._EndDate = value;
        this.OnPropertyChanged(nameof (EndDate));
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
