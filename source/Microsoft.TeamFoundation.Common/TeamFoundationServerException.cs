// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationServerException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation
{
  [ExceptionMapping("0.0", "3.0", "TeamFoundationServerException", "Microsoft.TeamFoundation.TeamFoundationServerException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TeamFoundationServerException : VssException
  {
    private Dictionary<object, object> m_properties;

    public TeamFoundationServerException()
    {
    }

    public TeamFoundationServerException(string message)
      : base(message)
    {
    }

    public TeamFoundationServerException(string message, Exception innerException)
      : base(message, innerException)
    {
      if (!(innerException is SoapException soapException))
        return;
      if (soapException.Detail == null)
        return;
      try
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(soapException.Detail.OuterXml))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
          {
            reader.Read();
            while (reader.NodeType == XmlNodeType.Element)
            {
              if (reader.Name == "ExceptionProperties")
              {
                reader.Read();
                this.m_properties = new Dictionary<object, object>();
                foreach (KeyValuePair<string, object> keyValuePair in ExceptionPropertyCollection.FromXml(reader))
                  this.m_properties[(object) keyValuePair.Key.ToUpperInvariant()] = keyValuePair.Value;
              }
              reader.Read();
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
    }

    protected TeamFoundationServerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public bool IsRemoteException => this.InnerException is SoapException;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public T GetProperty<T>(string name)
    {
      if (this.m_properties == null)
        return default (T);
      object obj;
      return this.m_properties.TryGetValue((object) name.ToUpperInvariant(), out obj) ? (T) obj : default (T);
    }

    public override IDictionary Data
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new Dictionary<object, object>();
        return (IDictionary) this.m_properties;
      }
    }
  }
}
