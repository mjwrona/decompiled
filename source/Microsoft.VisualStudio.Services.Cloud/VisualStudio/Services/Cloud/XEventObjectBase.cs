// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.XEventObjectBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class XEventObjectBase
  {
    protected void InitializeFromContextInfoData(ContextInfoData contextInfoData)
    {
      this.SequenceNumber = contextInfoData.SequenceNumber;
      this.ActivityId = contextInfoData.ActivityId;
      this.UniqueIdentifier = contextInfoData.UniqueIdentifier;
      this.HostId = contextInfoData.HostId;
      this.UserId = contextInfoData.UserId;
      this.Type = contextInfoData.Type;
      this.IsGoverned = contextInfoData.IsGoverned;
      this.ObjectName = contextInfoData.ObjectName;
    }

    protected StringBuilder GetStringBuilder()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("{0} - TimeStamp: {1}", (object) this.GetType().Name, (object) this.Timestamp));
      stringBuilder.Append(string.Format(", SequenceNumber: {0}", (object) this.SequenceNumber));
      stringBuilder.Append(string.Format(", ActivityId: {0}", (object) this.ActivityId));
      stringBuilder.Append(string.Format(", UniqueIdentifier: {0}", (object) this.UniqueIdentifier));
      stringBuilder.Append(string.Format(", HostId: {0}", (object) this.HostId));
      stringBuilder.Append(string.Format(", UserId: {0}", (object) this.UserId));
      stringBuilder.Append(string.Format(", Type: {0}", (object) this.Type));
      stringBuilder.Append(string.Format(", IsGoverned: {0}", (object) this.IsGoverned));
      stringBuilder.Append(", ObjectName: " + this.ObjectName);
      return stringBuilder;
    }

    public override string ToString() => this.GetStringBuilder().ToString();

    public abstract void Trace(
      IVssRequestContext requestContext,
      string serverName,
      string databaseName);

    public DateTime Timestamp { get; protected set; }

    public int SequenceNumber { get; protected set; }

    public Guid ActivityId { get; protected set; }

    public Guid UniqueIdentifier { get; protected set; }

    public Guid HostId { get; protected set; }

    public Guid UserId { get; protected set; }

    public ContextType Type { get; protected set; }

    public bool IsGoverned { get; protected set; }

    public string ObjectName { get; protected set; }

    public bool IsReadScaleOut { get; set; }
  }
}
