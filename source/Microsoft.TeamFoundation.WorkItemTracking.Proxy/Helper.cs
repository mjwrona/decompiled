// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.Helper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal static class Helper
  {
    private static AccessControlEntryData[] m_zeroLengthArrayOfAccessControlEntryData;
    private static AccessControlListMetadata[] m_zeroLengthArrayOfAccessControlListMetadata;
    private static ConstantRecord[] m_zeroLengthArrayOfConstantRecord;
    private static IdRevisionPair[] m_zeroLengthArrayOfIdRevisionPair;
    private static int[] m_zeroLengthArrayOfInt32;
    private static MetadataTableHaveEntry[] m_zeroLengthArrayOfMetadataTableHaveEntry;
    private static QuerySortOrderEntry[] m_zeroLengthArrayOfQuerySortOrderEntry;
    private static RequiredPermissions[] m_zeroLengthArrayOfRequiredPermissions;
    private static string[] m_zeroLengthArrayOfString;
    private static WorkItemId[] m_zeroLengthArrayOfWorkItemId;
    private static WorkItemLinkChange[] m_zeroLengthArrayOfWorkItemLinkChange;

    internal static AccessControlEntryData[] ZeroLengthArrayOfAccessControlEntryData
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfAccessControlEntryData == null)
          Helper.m_zeroLengthArrayOfAccessControlEntryData = Array.Empty<AccessControlEntryData>();
        return Helper.m_zeroLengthArrayOfAccessControlEntryData;
      }
    }

    internal static AccessControlListMetadata[] ZeroLengthArrayOfAccessControlListMetadata
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfAccessControlListMetadata == null)
          Helper.m_zeroLengthArrayOfAccessControlListMetadata = Array.Empty<AccessControlListMetadata>();
        return Helper.m_zeroLengthArrayOfAccessControlListMetadata;
      }
    }

    internal static ConstantRecord[] ZeroLengthArrayOfConstantRecord
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfConstantRecord == null)
          Helper.m_zeroLengthArrayOfConstantRecord = Array.Empty<ConstantRecord>();
        return Helper.m_zeroLengthArrayOfConstantRecord;
      }
    }

    internal static IdRevisionPair[] ZeroLengthArrayOfIdRevisionPair
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfIdRevisionPair == null)
          Helper.m_zeroLengthArrayOfIdRevisionPair = Array.Empty<IdRevisionPair>();
        return Helper.m_zeroLengthArrayOfIdRevisionPair;
      }
    }

    internal static int[] ZeroLengthArrayOfInt32
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfInt32 == null)
          Helper.m_zeroLengthArrayOfInt32 = Array.Empty<int>();
        return Helper.m_zeroLengthArrayOfInt32;
      }
    }

    internal static MetadataTableHaveEntry[] ZeroLengthArrayOfMetadataTableHaveEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfMetadataTableHaveEntry == null)
          Helper.m_zeroLengthArrayOfMetadataTableHaveEntry = Array.Empty<MetadataTableHaveEntry>();
        return Helper.m_zeroLengthArrayOfMetadataTableHaveEntry;
      }
    }

    internal static QuerySortOrderEntry[] ZeroLengthArrayOfQuerySortOrderEntry
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfQuerySortOrderEntry == null)
          Helper.m_zeroLengthArrayOfQuerySortOrderEntry = Array.Empty<QuerySortOrderEntry>();
        return Helper.m_zeroLengthArrayOfQuerySortOrderEntry;
      }
    }

    internal static RequiredPermissions[] ZeroLengthArrayOfRequiredPermissions
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfRequiredPermissions == null)
          Helper.m_zeroLengthArrayOfRequiredPermissions = Array.Empty<RequiredPermissions>();
        return Helper.m_zeroLengthArrayOfRequiredPermissions;
      }
    }

    internal static string[] ZeroLengthArrayOfString
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfString == null)
          Helper.m_zeroLengthArrayOfString = Array.Empty<string>();
        return Helper.m_zeroLengthArrayOfString;
      }
    }

    internal static WorkItemId[] ZeroLengthArrayOfWorkItemId
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfWorkItemId == null)
          Helper.m_zeroLengthArrayOfWorkItemId = Array.Empty<WorkItemId>();
        return Helper.m_zeroLengthArrayOfWorkItemId;
      }
    }

    internal static WorkItemLinkChange[] ZeroLengthArrayOfWorkItemLinkChange
    {
      get
      {
        if (Helper.m_zeroLengthArrayOfWorkItemLinkChange == null)
          Helper.m_zeroLengthArrayOfWorkItemLinkChange = Array.Empty<WorkItemLinkChange>();
        return Helper.m_zeroLengthArrayOfWorkItemLinkChange;
      }
    }

    internal static AccessControlEntryData[] ArrayOfAccessControlEntryDataFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessControlEntryData>(serviceProvider, reader, "AccessControlEntryData", inline, Helper.\u003C\u003EO.\u003C0\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlEntryData>(AccessControlEntryData.FromXml)));
    }

    internal static AccessControlListMetadata[] ArrayOfAccessControlListMetadataFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<AccessControlListMetadata>(serviceProvider, reader, "AccessControlListMetadata", inline, Helper.\u003C\u003EO.\u003C1\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessControlListMetadata>(AccessControlListMetadata.FromXml)));
    }

    internal static ConstantRecord[] ArrayOfConstantRecordFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<ConstantRecord>(serviceProvider, reader, "ConstantRecord", inline, Helper.\u003C\u003EO.\u003C2\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C2\u003E__FromXml = new Func<IServiceProvider, XmlReader, ConstantRecord>(ConstantRecord.FromXml)));
    }

    internal static IdRevisionPair[] ArrayOfIdRevisionPairFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<IdRevisionPair>(serviceProvider, reader, "IdRevisionPair", inline, Helper.\u003C\u003EO.\u003C3\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C3\u003E__FromXml = new Func<IServiceProvider, XmlReader, IdRevisionPair>(IdRevisionPair.FromXml)));
    }

    internal static int[] ArrayOfInt32FromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<int>(reader, "int", inline, Helper.\u003C\u003EO.\u003C4\u003E__Int32FromXmlElement ?? (Helper.\u003C\u003EO.\u003C4\u003E__Int32FromXmlElement = new Func<XmlReader, int>(XmlUtility.Int32FromXmlElement)));

    internal static MetadataTableHaveEntry[] ArrayOfMetadataTableHaveEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<MetadataTableHaveEntry>(serviceProvider, reader, "MetadataTableHaveEntry", inline, Helper.\u003C\u003EO.\u003C5\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C5\u003E__FromXml = new Func<IServiceProvider, XmlReader, MetadataTableHaveEntry>(MetadataTableHaveEntry.FromXml)));
    }

    internal static QuerySortOrderEntry[] ArrayOfQuerySortOrderEntryFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<QuerySortOrderEntry>(serviceProvider, reader, "QuerySortOrderEntry", inline, Helper.\u003C\u003EO.\u003C6\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C6\u003E__FromXml = new Func<IServiceProvider, XmlReader, QuerySortOrderEntry>(QuerySortOrderEntry.FromXml)));
    }

    internal static RequiredPermissions[] ArrayOfRequiredPermissionsFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<RequiredPermissions>(serviceProvider, reader, "RequiredPermissions", inline, Helper.\u003C\u003EO.\u003C7\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C7\u003E__FromXml = new Func<IServiceProvider, XmlReader, RequiredPermissions>(RequiredPermissions.FromXml)));
    }

    internal static string[] ArrayOfStringFromXml(XmlReader reader, bool inline) => XmlUtility.ArrayOfObjectFromXml<string>(reader, "string", inline, Helper.\u003C\u003EO.\u003C8\u003E__StringFromXmlElement ?? (Helper.\u003C\u003EO.\u003C8\u003E__StringFromXmlElement = new Func<XmlReader, string>(XmlUtility.StringFromXmlElement)));

    internal static WorkItemId[] ArrayOfWorkItemIdFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<WorkItemId>(serviceProvider, reader, "WorkItemId", inline, Helper.\u003C\u003EO.\u003C9\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C9\u003E__FromXml = new Func<IServiceProvider, XmlReader, WorkItemId>(WorkItemId.FromXml)));
    }

    internal static WorkItemLinkChange[] ArrayOfWorkItemLinkChangeFromXml(
      IServiceProvider serviceProvider,
      XmlReader reader,
      bool inline)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return XmlUtility.ArrayOfObjectFromXml<WorkItemLinkChange>(serviceProvider, reader, "WorkItemLinkChange", inline, Helper.\u003C\u003EO.\u003C10\u003E__FromXml ?? (Helper.\u003C\u003EO.\u003C10\u003E__FromXml = new Func<IServiceProvider, XmlReader, WorkItemLinkChange>(WorkItemLinkChange.FromXml)));
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
        return new DateTime?(XmlUtility.DateTimeFromXmlElement(reader));
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

    internal static void ToXml(XmlWriter writer, string arrayName, AccessControlEntryData[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlEntryData[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<AccessControlEntryData>(writer, array, arrayName, "AccessControlEntryData", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C11\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C11\u003E__ToXml = new Action<XmlWriter, string, AccessControlEntryData>(AccessControlEntryData.ToXml)));
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlListMetadata[] array)
    {
      Helper.ToXml(writer, arrayName, array, false, false);
    }

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      AccessControlListMetadata[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<AccessControlListMetadata>(writer, array, arrayName, "AccessControlListMetadata", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C12\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C12\u003E__ToXml = new Action<XmlWriter, string, AccessControlListMetadata>(AccessControlListMetadata.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, ConstantRecord[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      ConstantRecord[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ConstantRecord>(writer, array, arrayName, "ConstantRecord", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C13\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C13\u003E__ToXml = new Action<XmlWriter, string, ConstantRecord>(ConstantRecord.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, IdRevisionPair[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      IdRevisionPair[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<IdRevisionPair>(writer, array, arrayName, "IdRevisionPair", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C14\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C14\u003E__ToXml = new Action<XmlWriter, string, IdRevisionPair>(IdRevisionPair.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, int[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      int[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<int>(writer, array, arrayName, "int", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C15\u003E__ToXmlElement ?? (Helper.\u003C\u003EO.\u003C15\u003E__ToXmlElement = new Action<XmlWriter, string, int>(XmlUtility.ToXmlElement)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, MetadataTableHaveEntry[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      MetadataTableHaveEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<MetadataTableHaveEntry>(writer, array, arrayName, "MetadataTableHaveEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C16\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C16\u003E__ToXml = new Action<XmlWriter, string, MetadataTableHaveEntry>(MetadataTableHaveEntry.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, QuerySortOrderEntry[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      QuerySortOrderEntry[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<QuerySortOrderEntry>(writer, array, arrayName, "QuerySortOrderEntry", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C17\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C17\u003E__ToXml = new Action<XmlWriter, string, QuerySortOrderEntry>(QuerySortOrderEntry.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, RequiredPermissions[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      RequiredPermissions[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<RequiredPermissions>(writer, array, arrayName, "RequiredPermissions", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C18\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C18\u003E__ToXml = new Action<XmlWriter, string, RequiredPermissions>(RequiredPermissions.ToXml)));
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

    internal static void ToXml(XmlWriter writer, string arrayName, WorkItemId[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      WorkItemId[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<WorkItemId>(writer, array, arrayName, "WorkItemId", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C20\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C20\u003E__ToXml = new Action<XmlWriter, string, WorkItemId>(WorkItemId.ToXml)));
    }

    internal static void ToXml(XmlWriter writer, string arrayName, WorkItemLinkChange[] array) => Helper.ToXml(writer, arrayName, array, false, false);

    internal static void ToXml(
      XmlWriter writer,
      string arrayName,
      WorkItemLinkChange[] array,
      bool inline,
      bool allowEmptyArrays)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<WorkItemLinkChange>(writer, array, arrayName, "WorkItemLinkChange", inline, allowEmptyArrays, Helper.\u003C\u003EO.\u003C21\u003E__ToXml ?? (Helper.\u003C\u003EO.\u003C21\u003E__ToXml = new Action<XmlWriter, string, WorkItemLinkChange>(WorkItemLinkChange.ToXml)));
    }
  }
}
