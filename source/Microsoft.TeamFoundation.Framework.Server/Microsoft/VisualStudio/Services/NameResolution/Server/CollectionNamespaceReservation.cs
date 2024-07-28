// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.CollectionNamespaceReservation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  public class CollectionNamespaceReservation
  {
    public string Namespace { get; set; }

    public bool IsPrimary { get; set; }

    public override string ToString() => this.Namespace + (this.IsPrimary ? " (Primary)" : "");
  }
}
