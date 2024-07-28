// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingSqlColumnBinderExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class WorkItemTrackingSqlColumnBinderExtensions
  {
    private static readonly XmlSerializer s_xmlSerializer = new XmlSerializer(typeof (List<string>));
    private static readonly XmlReaderSettings s_xmlReaderSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      IgnoreProcessingInstructions = true,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };

    public static IList<string> FromXml(ref this SqlColumnBinder columnBinder, IDataReader reader)
    {
      string s = columnBinder.GetString(reader, true);
      if (string.IsNullOrEmpty(s))
        return (IList<string>) Array.Empty<string>();
      using (StringReader input = new StringReader(s))
      {
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input, WorkItemTrackingSqlColumnBinderExtensions.s_xmlReaderSettings))
          return (IList<string>) WorkItemTrackingSqlColumnBinderExtensions.s_xmlSerializer.Deserialize(xmlReader);
      }
    }
  }
}
