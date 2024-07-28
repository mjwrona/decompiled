// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.JobResourcesRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  public class JobResourcesRequest
  {
    public int RequestedResources { get; set; }

    public int UsedResources { get; set; }

    public IEntityType EntityType
    {
      get
      {
        if (this.m_entityType == null)
          this.m_entityType = (IEntityType) AllEntityType.GetInstance();
        return this.m_entityType;
      }
      set => this.m_entityType = value;
    }

    private IEntityType m_entityType { get; set; }
  }
}
