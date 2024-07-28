// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssEvents
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssEvents
  {
    private Dictionary<string, Dictionary<string, int>> m_eventNameMap;

    [XmlAttribute("eventSource")]
    public string EventSource { get; set; }

    public string[] EventConstantTypes { get; set; }

    [XmlArray("Events")]
    public VssEvent[] Events { get; set; }

    public static VssEvents[] Deserialize(FileInfo[] eventXmlFiles)
    {
      Stream[] vssEventStreams = new Stream[eventXmlFiles.Length];
      try
      {
        for (int index = 0; index < eventXmlFiles.Length; ++index)
        {
          FileInfo eventXmlFile = eventXmlFiles[index];
          vssEventStreams[index] = (Stream) eventXmlFile.OpenRead();
        }
        return VssEvents.Deserialize(vssEventStreams);
      }
      finally
      {
        foreach (Stream stream in vssEventStreams)
          stream?.Dispose();
      }
    }

    private static VssEvents[] Deserialize(Stream[] vssEventStreams)
    {
      VssEvents[] vssEventsArray = new VssEvents[vssEventStreams.Length];
      for (int index = 0; index < vssEventStreams.Length; ++index)
      {
        using (StreamReader streamReader = new StreamReader(vssEventStreams[index]))
        {
          string end = streamReader.ReadToEnd();
          vssEventsArray[index] = TeamFoundationSerializationUtility.Deserialize<VssEvents>(end, UnknownXmlNodeProcessing.ThrowException);
          vssEventsArray[index].Initialize();
        }
      }
      return vssEventsArray;
    }

    private void Initialize()
    {
      foreach (VssEvent vssEvent in this.Events)
      {
        vssEvent.EventSource = this.EventSource;
        int result;
        string memberName;
        if (int.TryParse(vssEvent.EventIdValue, out result))
        {
          vssEvent.EventId = result;
          memberName = string.Format("EventId {0}", (object) result);
        }
        else
          vssEvent.EventId = this.ResolveConstant(vssEvent.EventIdValue, out memberName);
        if (vssEvent.Alert != null && vssEvent.Alert.Kpi)
        {
          vssEvent.Alert.Stateful = true;
          if (vssEvent.Alert.ScopeIndex >= 0)
            throw new ArgumentException(FrameworkResources.OmitScopeIndexForKpiError());
        }
        if (string.IsNullOrWhiteSpace(vssEvent.Name))
          vssEvent.Name = memberName;
      }
    }

    private int ResolveConstant(string eventIdConstant, out string memberName)
    {
      if (this.m_eventNameMap == null)
      {
        this.m_eventNameMap = new Dictionary<string, Dictionary<string, int>>((IEqualityComparer<string>) StringComparer.Ordinal);
        if (this.EventConstantTypes != null)
        {
          foreach (string eventConstantType in this.EventConstantTypes)
          {
            Type type = Type.GetType(eventConstantType, true, false);
            Dictionary<string, int> dictionary = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.Ordinal);
            foreach (FieldInfo field in type.GetFields())
            {
              if (field.FieldType == typeof (int))
              {
                string name = field.Name;
                int num = (int) field.GetValue((object) null);
                dictionary.Add(name, num);
              }
            }
            this.m_eventNameMap.Add(type.Name, dictionary);
          }
        }
      }
      string[] strArray = !string.IsNullOrWhiteSpace(eventIdConstant) ? eventIdConstant.Split(new char[1]
      {
        '.'
      }, 2) : throw new TeamFoundationServiceException(FrameworkResources.EventIdNotSuppliedError());
      string str = strArray.Length == 2 ? strArray[0] : throw new TeamFoundationServiceException(FrameworkResources.EventIdResolutionError((object) eventIdConstant));
      string key = strArray[1];
      Dictionary<string, int> dictionary1;
      if (!this.m_eventNameMap.TryGetValue(str, out dictionary1))
        throw new TeamFoundationServiceException(FrameworkResources.EventIdResolutionError((object) eventIdConstant));
      int num1;
      if (!dictionary1.TryGetValue(key, out num1))
        throw new TeamFoundationServiceException(FrameworkResources.EventIdResolutionError((object) eventIdConstant));
      if (string.Equals(str, "TeamFoundationEventId", StringComparison.Ordinal))
      {
        if (num1 >= 20000)
          throw new TeamFoundationServiceException(FrameworkResources.SDKEventIdRangeError((object) eventIdConstant, (object) num1, (object) 20000, (object) ushort.MaxValue));
      }
      else if (num1 < 20000)
        throw new TeamFoundationServiceException(FrameworkResources.SDKConsumerEventIdRangeError((object) eventIdConstant, (object) num1, (object) 20000, (object) ushort.MaxValue));
      memberName = key;
      return num1;
    }
  }
}
