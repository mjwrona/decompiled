// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.DeleteProjectModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class DeleteProjectModel
  {
    private Guid? m_jobId;

    public Guid? JobId
    {
      get => this.m_jobId;
      set => this.m_jobId = value;
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["JobId"] = (object) this.JobId;
      return json;
    }
  }
}
