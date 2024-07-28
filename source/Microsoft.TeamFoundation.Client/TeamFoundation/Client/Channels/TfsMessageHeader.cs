// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsMessageHeader
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TfsMessageHeader
  {
    public abstract string Name { get; }

    public abstract string Namespace { get; }

    public void Write(XmlDictionaryWriter writer)
    {
      this.OnWriteStartHeader(writer);
      this.OnWriteHeaderContents(writer);
      this.OnWriteEndHeader(writer);
    }

    public virtual XmlDictionaryReader GetReader() => throw new NotSupportedException();

    protected virtual void OnWriteStartHeader(XmlDictionaryWriter writer) => writer.WriteStartElement(this.Name, this.Namespace);

    protected virtual void OnWriteEndHeader(XmlDictionaryWriter writer) => writer.WriteEndElement();

    protected abstract void OnWriteHeaderContents(XmlDictionaryWriter writer);
  }
}
