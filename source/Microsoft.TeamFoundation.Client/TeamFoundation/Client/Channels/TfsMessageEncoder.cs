// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsMessageEncoder
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TfsMessageEncoder
  {
    public abstract string ContentType { get; }

    public abstract bool IsContentTypeSupported(string contentType);

    public abstract TfsMessage ReadMessage(string messageAsString);

    public abstract TfsMessage ReadMessage(Stream stream);

    public abstract void WriteMessage(TfsMessage message, Stream stream);
  }
}
