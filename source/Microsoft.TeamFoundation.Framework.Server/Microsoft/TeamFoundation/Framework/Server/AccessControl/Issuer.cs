// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.Issuer
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
  [EntitySet("Issuers")]
  [DataServiceKey("Id")]
  public class Issuer : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Name;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<ConditionalRule> _ConditionalRules = new DataServiceCollection<ConditionalRule>((IEnumerable<ConditionalRule>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<IdentityProvider> _IdentityProviders = new DataServiceCollection<IdentityProvider>((IEnumerable<IdentityProvider>) null, TrackingMode.None);
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private DataServiceCollection<Rule> _Rules = new DataServiceCollection<Rule>((IEnumerable<Rule>) null, TrackingMode.None);

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static Issuer CreateIssuer(long ID, bool systemReserved) => new Issuer()
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
    public DataServiceCollection<ConditionalRule> ConditionalRules
    {
      get => this._ConditionalRules;
      set
      {
        this._ConditionalRules = value;
        this.OnPropertyChanged(nameof (ConditionalRules));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<IdentityProvider> IdentityProviders
    {
      get => this._IdentityProviders;
      set
      {
        this._IdentityProviders = value;
        this.OnPropertyChanged(nameof (IdentityProviders));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public DataServiceCollection<Rule> Rules
    {
      get => this._Rules;
      set
      {
        this._Rules = value;
        this.OnPropertyChanged(nameof (Rules));
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
