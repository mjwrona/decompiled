// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.PropertyChanged
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  [Obsolete("The PropertyChanged class is obsolete.")]
  public class PropertyChanged
  {
    private string _name;
    private string _oldValue;
    private string _newValue;

    public PropertyChanged()
    {
      this._name = string.Empty;
      this._oldValue = string.Empty;
      this._newValue = string.Empty;
    }

    public PropertyChanged(string name, string oldValue, string newValue)
    {
      this._name = name == null ? string.Empty : name.Trim();
      this._oldValue = oldValue == null ? string.Empty : oldValue.Trim();
      this._newValue = newValue == null ? string.Empty : newValue.Trim();
    }

    public string Name
    {
      get => this._name;
      set => this._name = value;
    }

    public string OldValue
    {
      get => this._oldValue;
      set => this._oldValue = value;
    }

    public string NewValue
    {
      get => this._newValue;
      set => this._newValue = value;
    }
  }
}
