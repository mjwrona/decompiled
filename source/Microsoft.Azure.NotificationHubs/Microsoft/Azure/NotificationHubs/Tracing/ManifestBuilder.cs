// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.ManifestBuilder
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class ManifestBuilder
  {
    private Dictionary<int, ulong> channelKeywords;
    private ulong channelReservedKeywordMask = 9223372036854775808;
    private Dictionary<int, string> opcodeTab;
    private Dictionary<int, string> taskTab;
    private Dictionary<int, ManifestBuilder.ChannelInfo> channelTab;
    private Dictionary<ulong, string> keywordTab;
    private Dictionary<string, Type> mapsTab;
    private Dictionary<string, string> stringTab;
    private StringBuilder sb;
    private StringBuilder events;
    private StringBuilder templates;
    private string providerName;
    private ResourceManager resources;
    private string templateName;
    private int numParams;

    public ManifestBuilder(
      string providerName,
      Guid providerGuid,
      string dllName,
      string resourceDll,
      ResourceManager resources)
    {
      this.providerName = providerName;
      this.resources = resources;
      this.sb = new StringBuilder();
      this.events = new StringBuilder();
      this.templates = new StringBuilder();
      this.opcodeTab = new Dictionary<int, string>();
      this.stringTab = new Dictionary<string, string>();
      this.sb.AppendLine("<instrumentationManifest xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
      this.sb.AppendLine(" <instrumentation xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:win=\"http://manifests.microsoft.com/win/2004/08/windows/events\">");
      this.sb.AppendLine("  <events xmlns=\"http://schemas.microsoft.com/win/2004/08/events\">");
      this.sb.Append("<provider name=\"").Append(providerName).Append("\" guid=\"{").Append(providerGuid.ToString()).Append("}");
      if (resourceDll != null)
      {
        string str = Path.DirectorySeparatorChar.ToString() + Path.GetFileName(resourceDll);
        this.sb.Append("\" resourceFileName=\"").Append(str).Append("\" messageFileName=\"").Append(str);
      }
      string str1 = providerName.Replace("-", "");
      this.sb.Append("\" symbol=\"").Append(str1);
      this.sb.Append("\" >").AppendLine();
    }

    public void AddOpcode(string name, int value) => this.opcodeTab[value] = name;

    public void AddTask(string name, int value)
    {
      if (this.taskTab == null)
        this.taskTab = new Dictionary<int, string>();
      this.taskTab[value] = name;
    }

    public void AddKeyword(string name, ulong value)
    {
      long num = (long) value;
      if ((num & num - 1L) != 0L)
        throw new ArgumentException(EventSourceSR.Event_KeywordValue((object) value, (object) name));
      if (this.keywordTab == null)
        this.keywordTab = new Dictionary<ulong, string>();
      this.keywordTab[value] = name;
    }

    public void AddChannel(string name, int channelId, ChannelAttribute channelAttribute)
    {
      long num = (long) this.AddChannelKeyword((EventChannel) channelId);
      if (this.channelTab == null)
        this.channelTab = new Dictionary<int, ManifestBuilder.ChannelInfo>();
      this.channelTab[channelId] = new ManifestBuilder.ChannelInfo()
      {
        Name = name,
        Attribs = channelAttribute
      };
    }

    private ulong AddChannelKeyword(EventChannel channel)
    {
      if (this.channelKeywords == null)
        this.channelKeywords = new Dictionary<int, ulong>();
      ulong reservedKeywordMask;
      if (!this.channelKeywords.TryGetValue((int) channel, out reservedKeywordMask))
      {
        reservedKeywordMask = this.channelReservedKeywordMask;
        this.channelReservedKeywordMask >>= 1;
        this.channelKeywords[(int) channel] = reservedKeywordMask;
      }
      return reservedKeywordMask;
    }

    public ulong GetChannelKeyword(EventChannel channel)
    {
      ulong num;
      return this.channelKeywords.TryGetValue((int) channel, out num) ? num : 0UL;
    }

    public ulong[] GetChannelData()
    {
      int num = -1;
      foreach (int key in this.channelKeywords.Keys)
      {
        if (key > num)
          num = key;
      }
      ulong[] channelData = new ulong[num + 1];
      foreach (KeyValuePair<int, ulong> channelKeyword in this.channelKeywords)
        channelData[channelKeyword.Key] = channelKeyword.Value;
      return channelData;
    }

    public void StartEvent(string eventName, EventAttribute eventAttribute)
    {
      this.templateName = eventName + "Args";
      this.numParams = 0;
      this.events.Append("  <event").Append(" symbol=\"").Append(eventName).Append("\"").Append(" value=\"").Append(eventAttribute.EventId).Append("\"").Append(" version=\"").Append(eventAttribute.Version).Append("\"").Append(" level=\"").Append(ManifestBuilder.GetLevelName(eventAttribute.Level)).Append("\"");
      this.WriteMessageAttrib(this.events, "event", eventName, eventAttribute.Message);
      if (eventAttribute.Keywords != EventKeywords.None)
      {
        ulong keywords = (ulong) ((EventKeywords) ~(long) this.GetChannelKeyword(eventAttribute.Channel) & eventAttribute.Keywords);
        this.events.Append(" keywords=\"").Append(this.GetKeywords(keywords, eventName)).Append("\"");
      }
      if (eventAttribute.Opcode != EventOpcode.Info)
        this.events.Append(" opcode=\"").Append(this.GetOpcodeName(eventAttribute.Opcode, eventName)).Append("\"");
      if (eventAttribute.Task != EventTask.None)
        this.events.Append(" task=\"").Append(this.GetTaskName(eventAttribute.Task, eventName)).Append("\"");
      if (eventAttribute.Channel == EventChannel.Default)
        return;
      this.events.Append(" channel=\"").Append(this.GetChannelName(eventAttribute.Channel, eventName)).Append("\"");
    }

    public void AddEventParameter(Type type, string name)
    {
      if (this.numParams == 0)
        this.templates.Append("  <template tid=\"").Append(this.templateName).Append("\">").AppendLine();
      ++this.numParams;
      this.templates.Append("   <data name=\"").Append(name).Append("\" inType=\"").Append(ManifestBuilder.GetTypeName(type)).Append("\"");
      if (type.IsEnum)
      {
        this.templates.Append(" map=\"").Append(type.Name).Append("\"");
        if (this.mapsTab == null)
          this.mapsTab = new Dictionary<string, Type>();
        if (!this.mapsTab.ContainsKey(type.Name))
          this.mapsTab.Add(type.Name, type);
      }
      this.templates.Append("/>").AppendLine();
    }

    public void EndEvent()
    {
      if (this.numParams > 0)
      {
        this.templates.Append("  </template>").AppendLine();
        this.events.Append(" template=\"").Append(this.templateName).Append("\"");
      }
      this.events.Append("/>").AppendLine();
      this.templateName = (string) null;
      this.numParams = 0;
    }

    public byte[] CreateManifest() => Encoding.UTF8.GetBytes(this.CreateManifestString());

    private string CreateManifestString()
    {
      if (this.channelTab != null)
      {
        this.sb.Append(" <channels>").AppendLine();
        List<int> intList = new List<int>((IEnumerable<int>) this.channelTab.Keys);
        intList.Sort();
        foreach (int key in intList)
        {
          ManifestBuilder.ChannelInfo channelInfo = this.channelTab[key];
          string str1 = (string) null;
          string elementName = "channel";
          bool flag = false;
          string str2 = (string) null;
          string str3 = (string) null;
          if (channelInfo.Attribs != null)
          {
            ChannelAttribute attribs = channelInfo.Attribs;
            str1 = attribs.Type;
            if (attribs.ImportChannel != null)
            {
              str3 = attribs.ImportChannel;
              elementName = "importChannel";
            }
            flag = attribs.Enabled;
            str2 = attribs.Isolation;
          }
          if (str3 == null)
            str3 = this.providerName + "/" + str1;
          string name = channelInfo.Name.Replace('-', '_');
          this.sb.Append("  <").Append(elementName);
          this.sb.Append(" name=\"").Append(str3).Append("\"");
          this.sb.Append(" chid=\"").Append(channelInfo.Name).Append("\"");
          this.sb.Append(" symbol=\"").Append(name).Append("\"");
          this.WriteMessageAttrib(this.sb, elementName, name, str1);
          this.sb.Append(" value=\"").Append(key).Append("\"");
          if (elementName == "channel")
          {
            if (str1 != null)
              this.sb.Append(" type=\"").Append(str1).Append("\"");
            this.sb.Append(" enabled=\"").Append(flag.ToString().ToLowerInvariant()).Append("\"");
            if (str2 != null)
              this.sb.Append(" isolation=\"").Append(str2).Append("\"");
          }
          if (channelInfo.Attribs != null && channelInfo.Attribs.BufferSize > 0)
            this.sb.AppendLine(">").Append("    <publishing>").AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "      <bufferSize>{0}</bufferSize>", new object[1]
            {
              (object) channelInfo.Attribs.BufferSize
            }).Append("    </publishing>").AppendLine().AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "  </{0}>", new object[1]
            {
              (object) elementName
            }).AppendLine();
          else
            this.sb.Append("/>").AppendLine();
        }
        this.sb.Append(" </channels>").AppendLine();
      }
      if (this.taskTab != null)
      {
        this.sb.Append(" <tasks>").AppendLine();
        List<int> intList = new List<int>((IEnumerable<int>) this.taskTab.Keys);
        intList.Sort();
        foreach (int key in intList)
          this.sb.Append("  <task name=\"").Append(this.taskTab[key]).Append("\" value=\"").Append(key).Append("\"/>").AppendLine();
        this.sb.Append(" </tasks>").AppendLine();
      }
      if (this.mapsTab != null)
      {
        this.sb.Append(" <maps>").AppendLine();
        foreach (Type element in this.mapsTab.Values)
        {
          string str4 = Attribute.GetCustomAttribute((MemberInfo) element, typeof (FlagsAttribute), false) != null ? "bitMap" : "valueMap";
          this.sb.Append("  <").Append(str4).Append(" name=\"").Append(element.Name).Append("\">").AppendLine();
          foreach (FieldInfo field in element.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
          {
            object rawConstantValue = field.GetRawConstantValue();
            if (rawConstantValue != null)
            {
              string str5 = (string) null;
              if (rawConstantValue is int num2)
                str5 = num2.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture);
              else if (rawConstantValue is long num1)
                str5 = num1.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture);
              this.sb.Append("   <map value=\"0x").Append(str5).Append("\"");
              this.WriteMessageAttrib(this.sb, "map", element.Name + "." + field.Name, field.Name);
              this.sb.Append("/>").AppendLine();
            }
          }
          this.sb.Append("  </").Append(str4).Append(">").AppendLine();
        }
        this.sb.Append(" </maps>").AppendLine();
      }
      this.sb.Append(" <opcodes>").AppendLine();
      List<int> intList1 = new List<int>((IEnumerable<int>) this.opcodeTab.Keys);
      intList1.Sort();
      foreach (int key in intList1)
      {
        this.sb.Append("  <opcode");
        this.WriteNameAndMessageAttribs(this.sb, "opcode", this.opcodeTab[key]);
        this.sb.Append(" value=\"").Append(key).Append("\"/>").AppendLine();
      }
      this.sb.Append(" </opcodes>").AppendLine();
      if (this.keywordTab != null)
      {
        this.sb.Append(" <keywords>").AppendLine();
        List<ulong> ulongList = new List<ulong>((IEnumerable<ulong>) this.keywordTab.Keys);
        ulongList.Sort();
        foreach (ulong key in ulongList)
        {
          this.sb.Append("  <keyword");
          this.WriteNameAndMessageAttribs(this.sb, "keyword", this.keywordTab[key]);
          this.sb.Append(" mask=\"0x").Append(key.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture)).Append("\"/>").AppendLine();
        }
        this.sb.Append(" </keywords>").AppendLine();
      }
      this.sb.Append(" <events>").AppendLine();
      this.sb.Append((object) this.events);
      this.sb.Append(" </events>").AppendLine();
      if (this.templates.Length > 0)
      {
        this.sb.Append(" <templates>").AppendLine();
        this.sb.Append((object) this.templates);
        this.sb.Append(" </templates>").AppendLine();
      }
      this.sb.Append("</provider>").AppendLine();
      this.sb.Append("</events>").AppendLine();
      this.sb.Append("</instrumentation>").AppendLine();
      this.sb.Append("<localization>").AppendLine();
      this.sb.Append(" <resources culture=\"").Append("en-US").Append("\">").AppendLine();
      this.sb.Append("  <stringTable>").AppendLine();
      List<string> stringList = new List<string>((IEnumerable<string>) this.stringTab.Keys);
      stringList.Sort();
      foreach (string key in stringList)
        this.sb.Append("   <string id=\"").Append(key).Append("\" value=\"").Append(this.stringTab[key]).Append("\"/>").AppendLine();
      this.sb.Append("  </stringTable>").AppendLine();
      this.sb.Append(" </resources>").AppendLine();
      this.sb.Append("</localization>").AppendLine();
      this.sb.AppendLine("</instrumentationManifest>");
      return this.sb.ToString();
    }

    private void WriteNameAndMessageAttribs(
      StringBuilder stringBuilder,
      string elementName,
      string name)
    {
      stringBuilder.Append(" name=\"").Append(name).Append("\" ");
      StringBuilder sb = this.sb;
      string elementName1 = elementName;
      string name1 = name;
      this.WriteMessageAttrib(sb, elementName1, name1, name1);
    }

    private void WriteMessageAttrib(
      StringBuilder stringBuilder,
      string elementName,
      string name,
      string value)
    {
      string str1 = elementName + "_" + name;
      if (this.resources != null)
      {
        string str2 = this.resources.GetString(str1);
        if (str2 != null)
          value = str2;
      }
      if (value == null)
        return;
      if (elementName == "event")
        value = ManifestBuilder.TranslateToManifestConvention(value);
      stringBuilder.Append(" message=\"$(string.").Append(str1).Append(")\"");
      this.stringTab.Add(str1, value);
    }

    private static string GetLevelName(EventLevel level) => (level >= (EventLevel) 16 ? "" : "win:") + level.ToString();

    private string GetChannelName(EventChannel channel, string eventName)
    {
      ManifestBuilder.ChannelInfo channelInfo = (ManifestBuilder.ChannelInfo) null;
      if (this.channelTab == null || !this.channelTab.TryGetValue((int) channel, out channelInfo))
        throw new ArgumentException(EventSourceSR.EventSource_UndefinedChannel((object) channel, (object) eventName));
      return channelInfo.Name;
    }

    private string GetTaskName(EventTask task, string eventName)
    {
      if (task == EventTask.None)
        return "";
      if (this.taskTab == null)
        this.taskTab = new Dictionary<int, string>();
      string taskName;
      if (!this.taskTab.TryGetValue((int) task, out taskName))
        taskName = this.taskTab[(int) task] = eventName;
      return taskName;
    }

    private string GetOpcodeName(EventOpcode opcode, string eventName)
    {
      switch (opcode)
      {
        case EventOpcode.Info:
          return "win:Info";
        case EventOpcode.Start:
          return "win:Start";
        case EventOpcode.Stop:
          return "win:Stop";
        case EventOpcode.DataCollectionStart:
          return "win:DC_Start";
        case EventOpcode.DataCollectionStop:
          return "win:DC_Stop";
        case EventOpcode.Extension:
          return "win:Extension";
        case EventOpcode.Reply:
          return "win:Reply";
        case EventOpcode.Resume:
          return "win:Resume";
        case EventOpcode.Suspend:
          return "win:Suspend";
        case EventOpcode.Send:
          return "win:Send";
        case EventOpcode.Receive:
          return "win:Receive";
        default:
          string opcodeName;
          if (this.opcodeTab == null || !this.opcodeTab.TryGetValue((int) opcode, out opcodeName))
            throw new ArgumentException("Use of undefined opcode value " + (object) opcode + " for event " + eventName);
          return opcodeName;
      }
    }

    private string GetKeywords(ulong keywords, string eventName)
    {
      string keywords1 = "";
      for (ulong key = 1; key != 0UL; key <<= 1)
      {
        if (((long) keywords & (long) key) != 0L)
        {
          string str;
          if (this.keywordTab == null || !this.keywordTab.TryGetValue(key, out str))
            throw new ArgumentException(EventSourceSR.Event_UndefinedKeyword((object) key, (object) eventName));
          if (keywords1.Length != 0)
            keywords1 += " ";
          keywords1 += str;
        }
      }
      return keywords1;
    }

    private static string GetTypeName(Type type)
    {
      if (type.IsEnum)
        return ManifestBuilder.GetTypeName(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0].FieldType).Replace("win:Int", "win:UInt");
      switch (Type.GetTypeCode(type))
      {
        case TypeCode.Object:
          return "win:UnicodeString";
        case TypeCode.Boolean:
          return "win:Boolean";
        case TypeCode.SByte:
          return "win:Int8";
        case TypeCode.Byte:
          return "win:Uint8";
        case TypeCode.Int16:
          return "win:Int16";
        case TypeCode.UInt16:
          return "win:UInt16";
        case TypeCode.Int32:
          return "win:Int32";
        case TypeCode.UInt32:
          return "win:UInt32";
        case TypeCode.Int64:
          return "win:Int64";
        case TypeCode.UInt64:
          return "win:UInt64";
        case TypeCode.Single:
          return "win:Float";
        case TypeCode.Double:
          return "win:Double";
        case TypeCode.String:
          return "win:UnicodeString";
        default:
          if (type == typeof (Guid))
            return "win:GUID";
          throw new ArgumentException(EventSourceSR.Event_UnsupportType((object) type.Name));
      }
    }

    private static string TranslateToManifestConvention(string eventMessage)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      int startIndex = 0;
      int index = 0;
      while (index < eventMessage.Length)
      {
        if (eventMessage[index] == '{')
        {
          int num1 = index;
          ++index;
          int num2 = 0;
          for (; index < eventMessage.Length && char.IsDigit(eventMessage[index]); ++index)
          {
            int num3 = num2;
            num2 = num3 + (num3 * 10 + (int) eventMessage[index] - 48);
          }
          if (index < eventMessage.Length && eventMessage[index] == '}')
          {
            ++index;
            if (stringBuilder == null)
              stringBuilder = new StringBuilder();
            stringBuilder.Append(eventMessage, startIndex, num1 - startIndex);
            stringBuilder.Append('%').Append(num2 + 1);
            startIndex = index;
          }
        }
        else
          ++index;
      }
      if (stringBuilder == null)
        return eventMessage;
      stringBuilder.Append(eventMessage, startIndex, index - startIndex);
      return stringBuilder.ToString();
    }

    private class ChannelInfo
    {
      public string Name;
      public ChannelAttribute Attribs;
    }
  }
}
