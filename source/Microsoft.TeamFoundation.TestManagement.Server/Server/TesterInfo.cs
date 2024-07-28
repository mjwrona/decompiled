// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TesterInfo
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TesterInfo
  {
    private Guid? m_testerId;

    public Guid? Id
    {
      get => this.m_testerId;
      internal set => this.m_testerId = !value.HasValue || !(value.Value == Guid.Empty) ? value : new Guid?();
    }

    public string DisplayName { get; internal set; }
  }
}
