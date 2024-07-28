// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TfsmqNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TfsmqNotification
  {
    private int m_queueId;
    private string m_queueName;
    private Uri m_queueAddress;
    private static readonly Lazy<XmlReaderSettings> s_readerSettings = new Lazy<XmlReaderSettings>(new Func<XmlReaderSettings>(TfsmqNotification.CreateReaderSettings), LazyThreadSafetyMode.PublicationOnly);

    protected TfsmqNotification(int queueId, string queueName)
    {
      this.m_queueId = queueId;
      this.m_queueName = queueName;
      this.m_queueAddress = new Uri("tfsmq://" + this.m_queueName, UriKind.Absolute);
    }

    internal int QueueId => this.m_queueId;

    public string QueueName => this.m_queueName;

    public Uri QueueAddress => this.m_queueAddress;

    protected static XmlReader CreateXmlReader(string xml)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(xml, nameof (xml));
      return XmlReader.Create((TextReader) new StringReader(xml), TfsmqNotification.s_readerSettings.Value);
    }

    private static XmlReaderSettings CreateReaderSettings() => new XmlReaderSettings()
    {
      CloseInput = true,
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null,
      IgnoreWhitespace = true
    };
  }
}
