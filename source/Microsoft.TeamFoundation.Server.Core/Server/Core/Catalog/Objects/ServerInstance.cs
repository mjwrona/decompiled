// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ServerInstance
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServerInstance : CatalogObject
  {
    protected override void Reset() => base.Reset();

    public string InstanceName
    {
      get => this.GetProperty<string>(nameof (InstanceName));
      set => this.SetProperty<string>(nameof (InstanceName), value);
    }

    public bool IsClustered
    {
      get => this.GetProperty<bool>(nameof (IsClustered));
      set => this.SetProperty<bool>(nameof (IsClustered), value);
    }

    public Machine Machine => this.GetParent<Machine>();

    public string ServerName
    {
      get
      {
        if (this.Machine == null)
          return (string) null;
        return string.IsNullOrEmpty(this.InstanceName) ? this.Machine.MachineName : this.Machine.MachineName + "\\" + this.InstanceName;
      }
    }

    protected static T Register<T>(
      Machine machine,
      string displayName,
      string instanceName,
      ICollection<T> children)
      where T : ServerInstance
    {
      ArgumentUtility.CheckForNull<Machine>(machine, nameof (machine));
      ArgumentUtility.CheckForNull<ICollection<T>>(children, nameof (children));
      if (string.IsNullOrEmpty(displayName))
        displayName = machine.Context.GetDisplayName(typeof (T));
      T obj = children.FirstOrDefault<T>((Func<T, bool>) (m => TFStringComparer.CatalogItemName.Equals(m.DisplayName, displayName)));
      if ((object) obj == null)
      {
        obj = machine.CreateChild<T>(displayName);
        obj.InstanceName = instanceName ?? string.Empty;
        obj.IsClustered = false;
      }
      return obj;
    }

    public override string ToString() => this.InstanceName;

    public static class Fields
    {
      public const string InstanceName = "InstanceName";
      public const string IsClustered = "IsClustered";
    }
  }
}
