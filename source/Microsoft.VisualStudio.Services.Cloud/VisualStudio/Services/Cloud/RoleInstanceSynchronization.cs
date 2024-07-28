// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RoleInstanceSynchronization
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [DataContract]
  public class RoleInstanceSynchronization
  {
    [DataMember]
    public string RoleInstance { get; set; }

    [DataMember]
    public Guid? HostId { get; set; }

    [DataMember]
    public string CurrentStatus { get; set; }

    [DataMember]
    public string PreviousStatus { get; set; }

    [DataMember]
    public DateTime LastUpdated { get; set; }
  }
}
