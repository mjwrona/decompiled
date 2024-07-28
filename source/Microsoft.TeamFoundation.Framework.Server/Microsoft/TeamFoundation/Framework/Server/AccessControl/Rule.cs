// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessControl.Rule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Data.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server.AccessControl
{
  [EntitySet("Rules")]
  [DataServiceKey("Id")]
  public class Rule : INotifyPropertyChanged
  {
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _Id;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _RuleGroupId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private long _IssuerId;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _Description;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _InputClaimType;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _InputClaimValue;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _OutputClaimType;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private string _OutputClaimValue;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private bool _SystemReserved;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private byte[] _Version;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private Issuer _Issuer;
    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    private RuleGroup _RuleGroup;

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public static Rule CreateRule(long ID, long ruleGroupId, long issuerId, bool systemReserved) => new Rule()
    {
      Id = ID,
      RuleGroupId = ruleGroupId,
      IssuerId = issuerId,
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
    public long RuleGroupId
    {
      get => this._RuleGroupId;
      set
      {
        this._RuleGroupId = value;
        this.OnPropertyChanged(nameof (RuleGroupId));
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
    public string InputClaimType
    {
      get => this._InputClaimType;
      set
      {
        this._InputClaimType = value;
        this.OnPropertyChanged(nameof (InputClaimType));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string InputClaimValue
    {
      get => this._InputClaimValue;
      set
      {
        this._InputClaimValue = value;
        this.OnPropertyChanged(nameof (InputClaimValue));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string OutputClaimType
    {
      get => this._OutputClaimType;
      set
      {
        this._OutputClaimType = value;
        this.OnPropertyChanged(nameof (OutputClaimType));
      }
    }

    [GeneratedCode("System.Data.Services.Design", "1.0.0")]
    public string OutputClaimValue
    {
      get => this._OutputClaimValue;
      set
      {
        this._OutputClaimValue = value;
        this.OnPropertyChanged(nameof (OutputClaimValue));
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
    public RuleGroup RuleGroup
    {
      get => this._RuleGroup;
      set
      {
        this._RuleGroup = value;
        this.OnPropertyChanged(nameof (RuleGroup));
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
