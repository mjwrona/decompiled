// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.IdentityProviderClaimType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  [EntitySet("IdentityProviderClaimTypes")]
  [DataServiceKey("Id")]
  public class IdentityProviderClaimType : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _ClaimTypeId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _IdentityProviderId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private ClaimType _ClaimType;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private IdentityProvider _IdentityProvider;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static IdentityProviderClaimType CreateIdentityProviderClaimType(
      long ID,
      long claimTypeId,
      long identityProviderId,
      bool systemReserved)
    {
      return new IdentityProviderClaimType()
      {
        Id = ID,
        ClaimTypeId = claimTypeId,
        IdentityProviderId = identityProviderId,
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
    public long ClaimTypeId
    {
      get => this._ClaimTypeId;
      set
      {
        this._ClaimTypeId = value;
        this.OnPropertyChanged(nameof (ClaimTypeId));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public long IdentityProviderId
    {
      get => this._IdentityProviderId;
      set
      {
        this._IdentityProviderId = value;
        this.OnPropertyChanged(nameof (IdentityProviderId));
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
    public ClaimType ClaimType
    {
      get => this._ClaimType;
      set
      {
        this._ClaimType = value;
        this.OnPropertyChanged(nameof (ClaimType));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public IdentityProvider IdentityProvider
    {
      get => this._IdentityProvider;
      set
      {
        this._IdentityProvider = value;
        this.OnPropertyChanged(nameof (IdentityProvider));
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
