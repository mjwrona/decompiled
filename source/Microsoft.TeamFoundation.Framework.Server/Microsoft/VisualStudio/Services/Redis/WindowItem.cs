// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.WindowItem
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Redis
{
  public struct WindowItem
  {
    private long m_increment;
    private long m_maximum;

    public WindowItem(long increment, long maximum = -1)
    {
      this.m_increment = increment;
      this.m_maximum = maximum;
    }

    public long Increment => this.m_increment;

    public long Maximum => this.m_maximum;
  }
}
