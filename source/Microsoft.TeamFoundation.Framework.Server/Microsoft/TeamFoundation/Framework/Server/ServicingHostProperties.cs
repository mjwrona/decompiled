// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingHostProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingHostProperties
  {
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    public int DatabaseId { get; set; }

    public string ServiceLevel { get; set; }

    public DateTime LastUserAccess { get; set; }

    public TeamFoundationServiceHostStatus Status { get; set; }

    public ServiceHostSubStatus SubStatus { get; set; }

    public int Throttled { get; set; }

    public TeamFoundationHostType HostType => this.ParentId == Guid.Empty ? TeamFoundationHostType.Application : TeamFoundationHostType.ProjectCollection;

    public bool IsReadyToUpgrade => !string.IsNullOrEmpty(this.ServiceLevel) && this.Status == TeamFoundationServiceHostStatus.Started && this.SubStatus != ServiceHostSubStatus.UpgradeDuringImport;

    public static void SortByAccessTimeDesc(List<ServicingHostProperties> properties) => properties.Sort((Comparison<ServicingHostProperties>) ((prop1, prop2) => prop2.LastUserAccess.CompareTo(prop1.LastUserAccess)));

    public override string ToString() => string.Format("Host Id: {0}, Status: {1}, SubStatus: {2}", (object) this.Id, (object) this.Status, (object) this.SubStatus);
  }
}
