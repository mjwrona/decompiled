// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.InvalidEventSerializerAttributeException
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class InvalidEventSerializerAttributeException : TeamFoundationServerException
  {
    public InvalidEventSerializerAttributeException(string m)
      : base(m)
    {
    }
  }
}
