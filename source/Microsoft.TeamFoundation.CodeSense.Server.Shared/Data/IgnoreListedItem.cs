// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Data.IgnoreListedItem
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server.Data
{
  public class IgnoreListedItem
  {
    private string item;
    private readonly Guid projectGuid;
    private readonly IgnoreListedItem.ComparisionKind comparisionKind;

    public IgnoreListedItem(string item, Guid projectGuid = default (Guid))
    {
      item = item.ToUpperInvariant();
      this.comparisionKind = IgnoreListedItem.ComparisionKind.None;
      if (TFStringComparer.VersionControlPath.StartsWith(item, "*"))
      {
        this.comparisionKind |= IgnoreListedItem.ComparisionKind.EndsWith;
        item = item.Substring(1);
      }
      if (TFStringComparer.VersionControlPath.EndsWith(item, "*"))
      {
        this.comparisionKind |= IgnoreListedItem.ComparisionKind.StartsWith;
        item = item.Substring(0, item.Length - 1);
      }
      this.item = item;
      this.projectGuid = projectGuid.Equals(new Guid()) ? Guid.Empty : projectGuid;
    }

    public string Item
    {
      get => this.item;
      set => this.item = value;
    }

    public Guid ProjectGuid => this.projectGuid;

    public bool Match(string serverPath)
    {
      switch (this.comparisionKind)
      {
        case IgnoreListedItem.ComparisionKind.None:
          return TFStringComparer.VersionControlPath.Equals(serverPath, this.item);
        case IgnoreListedItem.ComparisionKind.StartsWith:
          return TFStringComparer.VersionControlPath.StartsWith(serverPath, this.item);
        case IgnoreListedItem.ComparisionKind.EndsWith:
          return TFStringComparer.VersionControlPath.EndsWith(serverPath, this.item);
        case IgnoreListedItem.ComparisionKind.SubString:
          return TFStringComparer.VersionControlPath.Contains(serverPath, this.item);
        default:
          return false;
      }
    }

    public override string ToString()
    {
      switch (this.comparisionKind)
      {
        case IgnoreListedItem.ComparisionKind.None:
          return this.item;
        case IgnoreListedItem.ComparisionKind.StartsWith:
          return this.item + "*";
        case IgnoreListedItem.ComparisionKind.EndsWith:
          return "*" + this.item;
        case IgnoreListedItem.ComparisionKind.SubString:
          return "*" + this.item + "*";
        default:
          return base.ToString();
      }
    }

    [Flags]
    private enum ComparisionKind
    {
      None = 0,
      StartsWith = 1,
      EndsWith = 2,
      SubString = EndsWith | StartsWith, // 0x00000003
    }
  }
}
