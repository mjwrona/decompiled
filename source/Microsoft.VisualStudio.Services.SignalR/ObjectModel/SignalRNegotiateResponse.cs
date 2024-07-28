// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.ObjectModel.SignalRNegotiateResponse
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.SignalR.ObjectModel
{
  [DataContract]
  internal class SignalRNegotiateResponse
  {
    [DataMember]
    public string ConnectionToken { get; set; }

    [DataMember]
    public string ConnectionId { get; set; }

    [DataMember]
    public bool TryWebSockets { get; set; }
  }
}
