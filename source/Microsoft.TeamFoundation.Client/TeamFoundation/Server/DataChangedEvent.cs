// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.DataChangedEvent
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

namespace Microsoft.TeamFoundation.Server
{
  public class DataChangedEvent
  {
    public string DataType;
    public int SeqId;

    public DataChangedEvent()
    {
      this.DataType = string.Empty;
      this.SeqId = 0;
    }

    public DataChangedEvent(string dataType, int sequenceId)
    {
      this.DataType = dataType;
      this.SeqId = sequenceId;
    }
  }
}
