// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Helper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  internal static class Helper
  {
    private static AccessControlEntry[] m_zeroLengthArrayOfAccessControlEntry;
    private static AccessControlEntry[][] m_zeroLengthArrayOfArrayOfAccessControlEntry;
    private static bool[] m_zeroLengthArrayOfBoolean;
    private static DataChanged[] m_zeroLengthArrayOfDataChanged;
    private static Identity[] m_zeroLengthArrayOfIdentity;
    private static NodeInfo[] m_zeroLengthArrayOfNodeInfo;
    private static ProjectInfo[] m_zeroLengthArrayOfProjectInfo;
    private static ProjectProperty[] m_zeroLengthArrayOfProjectProperty;
    private static Property[] m_zeroLengthArrayOfProperty;
    private static string[] m_zeroLengthArrayOfString;
    private static SyncProperty[] m_zeroLengthArrayOfSyncProperty;

    internal static AccessControlEntry[] ZeroLengthArrayOfAccessControlEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfAccessControlEntry == null)
          Helper.m_zeroLengthArrayOfAccessControlEntry = new AccessControlEntry[0];
        return Helper.m_zeroLengthArrayOfAccessControlEntry;
      }
    }

    internal static AccessControlEntry[][] ZeroLengthArrayOfArrayOfAccessControlEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfArrayOfAccessControlEntry == null)
          Helper.m_zeroLengthArrayOfArrayOfAccessControlEntry = new AccessControlEntry[0][];
        return Helper.m_zeroLengthArrayOfArrayOfAccessControlEntry;
      }
    }

    internal static bool[] ZeroLengthArrayOfBoolean
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfBoolean == null)
          Helper.m_zeroLengthArrayOfBoolean = new bool[0];
        return Helper.m_zeroLengthArrayOfBoolean;
      }
    }

    internal static DataChanged[] ZeroLengthArrayOfDataChanged
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfDataChanged == null)
          Helper.m_zeroLengthArrayOfDataChanged = new DataChanged[0];
        return Helper.m_zeroLengthArrayOfDataChanged;
      }
    }

    internal static Identity[] ZeroLengthArrayOfIdentity
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfIdentity == null)
          Helper.m_zeroLengthArrayOfIdentity = new Identity[0];
        return Helper.m_zeroLengthArrayOfIdentity;
      }
    }

    internal static NodeInfo[] ZeroLengthArrayOfNodeInfo
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfNodeInfo == null)
          Helper.m_zeroLengthArrayOfNodeInfo = new NodeInfo[0];
        return Helper.m_zeroLengthArrayOfNodeInfo;
      }
    }

    internal static ProjectInfo[] ZeroLengthArrayOfProjectInfo
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfProjectInfo == null)
          Helper.m_zeroLengthArrayOfProjectInfo = new ProjectInfo[0];
        return Helper.m_zeroLengthArrayOfProjectInfo;
      }
    }

    internal static ProjectProperty[] ZeroLengthArrayOfProjectProperty
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfProjectProperty == null)
          Helper.m_zeroLengthArrayOfProjectProperty = new ProjectProperty[0];
        return Helper.m_zeroLengthArrayOfProjectProperty;
      }
    }

    internal static Property[] ZeroLengthArrayOfProperty
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfProperty == null)
          Helper.m_zeroLengthArrayOfProperty = new Property[0];
        return Helper.m_zeroLengthArrayOfProperty;
      }
    }

    internal static string[] ZeroLengthArrayOfString
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfString == null)
          Helper.m_zeroLengthArrayOfString = new string[0];
        return Helper.m_zeroLengthArrayOfString;
      }
    }

    internal static SyncProperty[] ZeroLengthArrayOfSyncProperty
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfSyncProperty == null)
          Helper.m_zeroLengthArrayOfSyncProperty = new SyncProperty[0];
        return Helper.m_zeroLengthArrayOfSyncProperty;
      }
    }

    internal static AccessControlEntry[] ArrayOfAccessControlEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessControlEntry>(serviceProvider, reader, "AccessControlEntry", inline, Helper.\u003C\u003EO.\u003C0\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlEntry>(AccessControlEntry.FromXml)));
    }

    internal static AccessControlEntry[] ArrayOfAccessControlEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      return Helper.ArrayOfAccessControlEntryFromXml(serviceProvider, reader, false);
    }

    internal static AccessControlEntry[][] ArrayOfArrayOfAccessControlEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessControlEntry[]>(serviceProvider, reader, "ArrayOfAccessControlEntry", inline, Helper.\u003C\u003EO.\u003C1\u003E__ArrayOfAccessControlEntryFromXml ?? (Helper.\u003C\u003EO.\u003C1\u003E__ArrayOfAccessControlEntryFromXml = new Func<IServiceProvider, XmlReader, AccessControlEntry[]>(Helper.ArrayOfAccessControlEntryFromXml)));
    }

    internal static bool[] ArrayOfBooleanFromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<bool>(reader, "bool", inline, Helper.\u003C\u003EO.\u003C2\u003E__BooleanFromXmlElement ?? (Helper.\u003C\u003EO.\u003C2\u003E__BooleanFromXmlElement = new Func<XmlReader, bool>(XmlUtility.BooleanFromXmlElement)));

    internal static DataChanged[] ArrayOfDataChangedFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<DataChanged>(serviceProvider, reader, "DataChanged", inline, Helper.\u003C\u003EO.\u003C3\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C3\u003E__FromXml = new Func<IServiceProvider, XmlReader, DataChanged>(DataChanged.FromXml)));
    }

    internal static Identity[] ArrayOfIdentityFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<Identity>(serviceProvider, reader, "Identity", inline, Helper.\u003C\u003EO.\u003C4\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C4\u003E__FromXml = new Func<IServiceProvider, XmlReader, Identity>(Identity.FromXml)));
    }

    internal static NodeInfo[] ArrayOfNodeInfoFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<NodeInfo>(serviceProvider, reader, "NodeInfo", inline, Helper.\u003C\u003EO.\u003C5\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C5\u003E__FromXml = new Func<IServiceProvider, XmlReader, NodeInfo>(NodeInfo.FromXml)));
    }

    internal static ProjectInfo[] ArrayOfProjectInfoFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ProjectInfo>(serviceProvider, reader, "ProjectInfo", inline, Helper.\u003C\u003EO.\u003C6\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C6\u003E__FromXml = new Func<IServiceProvider, XmlReader, ProjectInfo>(ProjectInfo.FromXml)));
    }

    internal static ProjectProperty[] ArrayOfProjectPropertyFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ProjectProperty>(serviceProvider, reader, "ProjectProperty", inline, Helper.\u003C\u003EO.\u003C7\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C7\u003E__FromXml = new Func<IServiceProvider, XmlReader, ProjectProperty>(ProjectProperty.FromXml)));
    }

    internal static Property[] ArrayOfPropertyFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<Property>(serviceProvider, reader, "Property", inline, Helper.\u003C\u003EO.\u003C8\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C8\u003E__FromXml = new Func<IServiceProvider, XmlReader, Property>(Property.FromXml)));
    }

    internal static string[] ArrayOfStringFromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<string>(reader, "string", inline, Helper.\u003C\u003EO.\u003C9\u003E__StringFromXmlElement ?? (Helper.\u003C\u003EO.\u003C9\u003E__StringFromXmlElement = new Func<XmlReader, string>(XmlUtility.StringFromXmlElement)));

    internal static SyncProperty[] ArrayOfSyncPropertyFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<SyncProperty>(serviceProvider, reader, "SyncProperty", inline, Helper.\u003C\u003EO.\u003C10\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C10\u003E__FromXml = new Func<IServiceProvider, XmlReader, SyncProperty>(SyncProperty.FromXml)));
    }

    internal static string ArrayToString<T>(T[] array)
    {
      if (array == null)
        return "<null>";
      int num = Math.Min(array.Length, 100);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      stringBuilder.Append(array.Length);
      stringBuilder.Append("]");
      for (int index = 0; index < num; ++index)
      {
        stringBuilder.Append((object) array[index]);
        if (index != num - 1)
          stringBuilder.Append(", ");
      }
      if (num < array.Length)
        stringBuilder.Append(", ...");
      return stringBuilder.ToString();
    }

    internal static DateTime? NullableOfDateTimeFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      if (!reader.HasAttributes || !(reader.GetAttribute("xsi:nil") == "true"))
        return new DateTime?(XmlUtility.DateFromXmlElement(reader));
      reader.Read();
      return new DateTime?();
    }

    internal static void StringToXmlElement(XmlWriter writer, string element, string str)
    {
      if (str == null)
        return;
      try
      {
        writer.WriteElementString(element, str);
      }
      catch (ArgumentException ex)
      {
        throw new TeamFoundationServiceException(CommonResources.StringContainsIllegalChars());
      }
    }

    internal static void ToXml(XmlWriter writer, string arrayName, AccessControlEntry[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<AccessControlEntry>(writer, array, arrayName, "AccessControlEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C11\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C11\u003E__ToXml = new Action<XmlWriter, string, AccessControlEntry>(AccessControlEntry.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, bool[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      bool[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<bool>(writer, array, arrayName, "bool", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C12\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C12\u003E__ToXmlElement = new Action<XmlWriter, string, bool>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, NodeInfo[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      NodeInfo[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<NodeInfo>(writer, array, arrayName, "NodeInfo", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C13\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C13\u003E__ToXml = new Action<XmlWriter, string, NodeInfo>(NodeInfo.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ProjectInfo[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ProjectInfo[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ProjectInfo>(writer, array, arrayName, "ProjectInfo", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C14\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C14\u003E__ToXml = new Action<XmlWriter, string, ProjectInfo>(ProjectInfo.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ProjectProperty[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ProjectProperty[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ProjectProperty>(writer, array, arrayName, "ProjectProperty", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C15\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C15\u003E__ToXml = new Action<XmlWriter, string, ProjectProperty>(ProjectProperty.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, Property[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      Property[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<Property>(writer, array, arrayName, "Property", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C16\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C16\u003E__ToXml = new Action<XmlWriter, string, Property>(Property.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, DataChanged[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      DataChanged[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<DataChanged>(writer, array, arrayName, "DataChanged", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C17\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C17\u003E__ToXml = new Action<XmlWriter, string, DataChanged>(DataChanged.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, Identity[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      Identity[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<Identity>(writer, array, arrayName, "Identity", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C18\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C18\u003E__ToXml = new Action<XmlWriter, string, Identity>(Identity.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, string[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      string[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<string>(writer, array, arrayName, "string", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C19\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C19\u003E__ToXmlElement = new Action<XmlWriter, string, string>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, SyncProperty[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      SyncProperty[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<SyncProperty>(writer, array, arrayName, "SyncProperty", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C20\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C20\u003E__ToXml = new Action<XmlWriter, string, SyncProperty>(SyncProperty.ToXml)));
    }
  }
}
