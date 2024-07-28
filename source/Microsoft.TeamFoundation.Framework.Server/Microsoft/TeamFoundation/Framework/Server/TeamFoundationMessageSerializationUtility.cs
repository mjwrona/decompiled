// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationMessageSerializationUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class TeamFoundationMessageSerializationUtility
  {
    private static XmlReaderSettings s_readerSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null,
      IgnoreProcessingInstructions = true
    };
    private static XmlWriterSettings s_writerSettings = new XmlWriterSettings()
    {
      Encoding = Encoding.UTF8,
      OmitXmlDeclaration = true
    };

    public static Message Deserialize(string serializedMessage)
    {
      MessageVersion version = OperationContext.Current != null ? OperationContext.Current.IncomingMessageVersion : MessageVersion.Soap12WSAddressing10;
      return Message.CreateMessage(XmlReader.Create((TextReader) new StringReader(serializedMessage), TeamFoundationMessageSerializationUtility.s_readerSettings), int.MaxValue, version);
    }

    public static string SerializeToString(Message messageToSerialize)
    {
      ArgumentUtility.CheckForNull<Message>(messageToSerialize, nameof (messageToSerialize));
      using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlWriter writer = XmlWriter.Create((TextWriter) output, TeamFoundationMessageSerializationUtility.s_writerSettings))
        {
          using (MessageBuffer bufferedCopy = messageToSerialize.CreateBufferedCopy(int.MaxValue))
          {
            using (Message message = bufferedCopy.CreateMessage())
            {
              messageToSerialize.Close();
              message.WriteMessage(writer);
            }
          }
        }
        return output.ToString();
      }
    }
  }
}
