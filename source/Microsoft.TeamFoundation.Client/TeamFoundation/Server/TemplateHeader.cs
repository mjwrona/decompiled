// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.TemplateHeader
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Client;

namespace Microsoft.TeamFoundation.Server
{
  public sealed class TemplateHeader
  {
    public TemplateHeader()
    {
    }

    internal TemplateHeader(FrameworkTemplateHeader header)
    {
      this.Name = header.Name;
      this.Description = header.Description;
      this.Metadata = header.Metadata;
      this.Rank = header.Rank;
      this.State = header.State;
      this.TemplateId = header.TemplateId;
    }

    public string Description { get; set; }

    public string Metadata { get; set; }

    public string Name { get; set; }

    public int Rank { get; set; }

    public string State { get; set; }

    public int TemplateId { get; set; }

    public TemplateVersion GetVersion() => TemplateVersion.Create(this.Metadata);
  }
}
