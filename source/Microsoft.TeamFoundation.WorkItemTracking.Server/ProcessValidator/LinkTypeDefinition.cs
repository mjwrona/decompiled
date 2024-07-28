// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator.LinkTypeDefinition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator
{
  public class LinkTypeDefinition
  {
    public string ReferenceName { get; private set; }

    public string ForwardName { get; private set; }

    public string ReverseName { get; private set; }

    public string Topology { get; private set; }

    public LinkTypeDefinition(
      string referenceName,
      string forwardName,
      string reverseName,
      string topology)
    {
      this.ReferenceName = referenceName;
      this.ForwardName = forwardName;
      this.ReverseName = reverseName;
      this.Topology = topology;
    }
  }
}
