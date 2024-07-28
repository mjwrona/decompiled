// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Channels.TfsClientOperation
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client.Channels
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TfsClientOperation
  {
    public abstract string BodyName { get; }

    public virtual bool HasOutputs => false;

    public ReadOnlyCollection<TfsMessageHeader> OutputHeaders { get; internal set; }

    public virtual string ResultName => (string) null;

    public abstract string SoapAction { get; }

    public abstract string SoapNamespace { get; }

    public TfsBodyWriter CreateBodyWriter(object[] parameters) => new TfsBodyWriter(this.BodyName, this.SoapNamespace, parameters, new Action<XmlDictionaryWriter, object[]>(this.WriteParameters));

    public virtual object InitializeOutputs(out object[] parameters)
    {
      parameters = (object[]) null;
      return (object) null;
    }

    public virtual object ReadResult(IServiceProvider serviceProvider, XmlReader reader)
    {
      reader.Read();
      return (object) null;
    }

    public virtual void ReadOutput(
      IServiceProvider serviceProvider,
      XmlReader reader,
      object[] parameters)
    {
      reader.Read();
    }

    protected virtual void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
    {
    }
  }
}
