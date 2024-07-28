// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContextInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.Azure.DevOps.Licensing;
using Microsoft.VisualStudio.Services.Licensing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ContextInfo
  {
    private static readonly ConcurrentDictionary<string, byte[]> s_objectNameEncodings = new ConcurrentDictionary<string, byte[]>();
    private static readonly Encoding s_encoding = Encoding.ASCII;
    internal const int MaxContextInfoSize = 128;
    internal const byte Heading = 30;
    internal const byte Version = 5;
    private static int s_sequenceNumber = 0;

    public static byte[] Encode(IVssRequestContext requestContext, string objectName)
    {
      byte[] buffer = new byte[128];
      int num1 = 0;
      byte[] numArray1 = buffer;
      int index1 = num1;
      int offset1 = index1 + 1;
      numArray1[index1] = (byte) 30;
      buffer[(int) sbyte.MaxValue] = (byte) 5;
      int offset2 = offset1 + ContextInfo.WriteInt(Interlocked.Increment(ref ContextInfo.s_sequenceNumber), buffer, offset1);
      if (requestContext != null)
      {
        ContextType contextType = ContextType.None;
        int offset3 = offset2 + ContextInfo.WriteGuid(requestContext.ActivityId, buffer, offset2);
        Guid result = requestContext.GetUserId();
        int offset4;
        if (requestContext.IsPipelineIdentity())
        {
          contextType |= ContextType.Pipeline;
          Guid guid;
          requestContext.RootContext.Items.TryGetValue<Guid>(RequestContextItemsKeys.AuthorizationId, out guid);
          offset4 = offset3 + ContextInfo.WriteGuid(guid, buffer, offset3);
        }
        else if (requestContext.IsRootContextAnonymous())
        {
          contextType |= ContextType.Anonymous;
          Guid.TryParse(requestContext.GetAnonymousIdentifier(), out result);
          offset4 = offset3 + ContextInfo.WriteBytes(ContextInfo.GetEncodedIPAddress(requestContext), buffer, offset3);
        }
        else
        {
          if (requestContext.IsRootContextPublicUser())
            contextType |= ContextType.Public;
          offset4 = offset3 + ContextInfo.WriteGuid(requestContext.UniqueIdentifier, buffer, offset3);
        }
        int offset5 = offset4 + ContextInfo.WriteGuid(requestContext.RootContext.ServiceHost.InstanceId, buffer, offset4);
        int offset6 = offset5 + ContextInfo.WriteGuid(result, buffer, offset5);
        int offset7 = offset6 + ContextInfo.WriteInt(0, buffer, offset6);
        int num2 = offset7 + ContextInfo.WriteInt(0, buffer, offset7);
        if (requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.IsActivity))
          contextType |= ContextType.Activity;
        if (requestContext.IsHostProcessType(HostProcessType.JobAgent))
          contextType |= ContextType.Job;
        string str;
        if (requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.TaskPoolName, out str))
        {
          if ("TaskPool".Equals(str))
            contextType |= ContextType.Task;
          else
            contextType |= ContextType.Notification;
        }
        if (contextType == ContextType.None)
          contextType = ContextType.Other;
        byte[] numArray2 = buffer;
        int index2 = num2;
        int num3 = index2 + 1;
        int num4 = (int) Convert.ToByte((object) contextType);
        numArray2[index2] = (byte) num4;
        byte[] numArray3 = buffer;
        int index3 = num3;
        int offset8 = index3 + 1;
        int num5 = (int) Convert.ToByte(requestContext.RootContext.Items.ContainsKey(RequestContextItemsKeys.GovernXEvents));
        numArray3[index3] = (byte) num5;
        int num6 = offset8 + ContextInfo.WriteInt(requestContext.ServiceHost.PartitionId, buffer, offset8);
        byte[] numArray4 = buffer;
        int index4 = num6;
        int num7 = index4 + 1;
        int num8 = (int) Convert.ToByte((object) requestContext.GetRUCompatibleAccountLicenseType(true));
        numArray4[index4] = (byte) num8;
      }
      if (!string.IsNullOrEmpty(objectName))
        ContextInfo.WriteObjectName(objectName, buffer, 84);
      return buffer;
    }

    public static bool TryDecode(byte[] contextInfo, out ContextInfoData data)
    {
      if (contextInfo.Length != 128 || contextInfo[0] != (byte) 30 || contextInfo[(int) sbyte.MaxValue] < (byte) 4)
      {
        data = new ContextInfoData();
        return false;
      }
      switch (contextInfo[(int) sbyte.MaxValue])
      {
        case 5:
          data = new ContextInfoData(ContextInfo.ReadInt(contextInfo, 1), ContextInfo.ReadGuid(contextInfo, 5), ContextInfo.ReadGuid(contextInfo, 21), ContextInfo.ReadGuid(contextInfo, 37), ContextInfo.ReadGuid(contextInfo, 53), (ContextType) contextInfo[77], contextInfo[78] > (byte) 0, ContextInfo.ReadInt(contextInfo, 79), (AccountLicenseType) contextInfo[83], ContextInfo.ReadObjectName(contextInfo, 84));
          break;
        case 6:
          data = new ContextInfoData(ContextInfo.ReadInt(contextInfo, 1), ContextInfo.ReadGuid(contextInfo, 5), ContextInfo.ReadGuid(contextInfo, 21), ContextInfo.ReadGuid(contextInfo, 37), ContextInfo.ReadGuid(contextInfo, 53), (ContextType) contextInfo[69], contextInfo[70] > (byte) 0, ContextInfo.ReadInt(contextInfo, 71), (AccountLicenseType) contextInfo[75], ContextInfo.ReadObjectName(contextInfo, 76));
          break;
        default:
          data = new ContextInfoData(ContextInfo.ReadInt(contextInfo, 1), ContextInfo.ReadGuid(contextInfo, 5), ContextInfo.ReadGuid(contextInfo, 21), ContextInfo.ReadGuid(contextInfo, 37), ContextInfo.ReadGuid(contextInfo, 53), (ContextType) contextInfo[77], contextInfo[78] > (byte) 0, ContextInfo.ReadInt(contextInfo, 79), AccountLicenseType.None, ContextInfo.ReadObjectName(contextInfo, 80));
          break;
      }
      return true;
    }

    public static byte[] GetEncodedIPAddress(IVssRequestContext requestContext)
    {
      object encodedIpAddress;
      if (!requestContext.RootContext.Items.TryGetValue(RequestContextItemsKeys.EncodedIpAddress, out encodedIpAddress))
      {
        string str = requestContext.RemoteIPAddress();
        string ipString;
        if (str == null)
          ipString = (string) null;
        else
          ipString = ((IEnumerable<string>) str.Split(',')).FirstOrDefault<string>();
        encodedIpAddress = (object) ContextInfo.EncodeIPAddress(ipString);
        requestContext.RootContext.Items.Add(RequestContextItemsKeys.EncodedIpAddress, encodedIpAddress);
      }
      return (byte[]) encodedIpAddress;
    }

    public static byte[] EncodeIPAddress(string ipString)
    {
      IPAddress address;
      return IPAddress.TryParse(ipString, out address) ? address.MapToIPv6().GetAddressBytes() : new byte[16];
    }

    public static string DecodeIPAddress(byte[] encodedIpAddress)
    {
      IPAddress ipAddress = new IPAddress(encodedIpAddress);
      return ipAddress.Equals((object) IPAddress.IPv6Any) ? string.Empty : (ipAddress.IsIPv4MappedToIPv6 ? (object) ipAddress.MapToIPv4() : (object) ipAddress).ToString();
    }

    private static unsafe Guid ReadGuid(byte[] buffer, int offset)
    {
      if (offset < 0 || offset > buffer.Length - sizeof (Guid))
        throw new ArgumentOutOfRangeException(nameof (offset));
      Guid guid;
      byte* numPtr = (byte*) &guid;
      for (int index = 0; index < sizeof (Guid); index += 4)
      {
        numPtr[index] = buffer[offset];
        numPtr[index + 1] = buffer[offset + 1];
        numPtr[index + 2] = buffer[offset + 2];
        numPtr[index + 3] = buffer[offset + 3];
        offset += 4;
      }
      return guid;
    }

    private static unsafe int WriteGuid(Guid value, byte[] buffer, int offset)
    {
      byte* numPtr = (byte*) &value;
      for (int index = 0; index < 16; index += 4)
      {
        buffer[offset] = numPtr[index];
        buffer[offset + 1] = numPtr[index + 1];
        buffer[offset + 2] = numPtr[index + 2];
        buffer[offset + 3] = numPtr[index + 3];
        offset += 4;
      }
      return 16;
    }

    private static int WriteBytes(byte[] value, byte[] buffer, int offset)
    {
      Buffer.BlockCopy((Array) value, 0, (Array) buffer, offset, value.Length);
      return value.Length;
    }

    internal static int ReadInt(byte[] buffer, int offset)
    {
      if (offset < 0 || offset > buffer.Length - 4)
        throw new ArgumentOutOfRangeException(nameof (offset));
      return BitConverter.ToInt32(buffer, offset);
    }

    private static int WriteInt(int value, byte[] buffer, int offset)
    {
      buffer[offset] = (byte) value;
      buffer[offset + 1] = (byte) (value >> 8);
      buffer[offset + 2] = (byte) (value >> 16);
      buffer[offset + 3] = (byte) (value >> 24);
      return 4;
    }

    private static string ReadObjectName(byte[] buffer, int offset) => ContextInfo.s_encoding.GetString(buffer, offset, (int) sbyte.MaxValue - offset).TrimEnd(new char[1]);

    private static void WriteObjectName(string value, byte[] buffer, int offset)
    {
      byte[] orAdd = ContextInfo.s_objectNameEncodings.GetOrAdd(value, (Func<string, byte[]>) (key => ContextInfo.s_encoding.GetBytes(key)));
      int count = Math.Min(orAdd.Length, (int) sbyte.MaxValue - offset);
      Buffer.BlockCopy((Array) orAdd, 0, (Array) buffer, offset, count);
    }
  }
}
