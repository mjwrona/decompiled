// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WhitelistingConfig
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WhitelistingConfig
  {
    [XmlAttribute("dataSource")]
    public string DataSource { get; set; }

    [XmlAttribute("servicingPriority")]
    public int ServicingPriority { get; set; }

    [XmlAttribute("area")]
    public string Area { get; set; }

    [XmlArray("SlowCommands")]
    public SlowCommand[] Commands { get; set; }

    [XmlArray("ExpectedExceptions")]
    public ExpectedExceptionType[] Exceptions { get; set; }

    [XmlArray("InteractiveUserAgentPrefixes")]
    public InteractiveUserAgentPrefix[] InteractiveUserAgentPrefixes { get; set; }

    public static WhitelistingConfig[] Deserialize(FileInfo[] whitelistingXmlFiles)
    {
      Stream[] whitelistingConfigStreams = new Stream[whitelistingXmlFiles.Length];
      try
      {
        for (int index = 0; index < whitelistingXmlFiles.Length; ++index)
        {
          FileInfo whitelistingXmlFile = whitelistingXmlFiles[index];
          whitelistingConfigStreams[index] = (Stream) whitelistingXmlFile.OpenRead();
        }
        return WhitelistingConfig.Deserialize(whitelistingConfigStreams);
      }
      finally
      {
        foreach (Stream stream in whitelistingConfigStreams)
          stream?.Dispose();
      }
    }

    private static WhitelistingConfig[] Deserialize(Stream[] whitelistingConfigStreams)
    {
      WhitelistingConfig[] whitelistingConfigArray = new WhitelistingConfig[whitelistingConfigStreams.Length];
      for (int index = 0; index < whitelistingConfigStreams.Length; ++index)
      {
        using (StreamReader streamReader = new StreamReader(whitelistingConfigStreams[index]))
        {
          string end = streamReader.ReadToEnd();
          whitelistingConfigArray[index] = TeamFoundationSerializationUtility.Deserialize<WhitelistingConfig>(end, UnknownXmlNodeProcessing.ThrowException);
        }
      }
      return whitelistingConfigArray;
    }
  }
}
