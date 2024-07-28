// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ServerDefaultFieldValue
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServerDefaultFieldValue
  {
    private ServerDefaultType m_type;

    public ServerDefaultFieldValue(ServerDefaultType t) => this.m_type = t;

    public ServerDefaultType Type => this.m_type;

    public override bool Equals(object o) => o is ServerDefaultFieldValue defaultFieldValue && defaultFieldValue.Type == this.Type;

    public override int GetHashCode() => (int) this.m_type;
  }
}
