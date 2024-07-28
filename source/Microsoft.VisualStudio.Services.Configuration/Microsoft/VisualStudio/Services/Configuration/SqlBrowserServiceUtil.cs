// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlBrowserServiceUtil
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class SqlBrowserServiceUtil
  {
    private const byte clntUcastEx = 3;
    private const int sqlBrowserServicePort = 1434;

    public static bool CheckSqlBrowserReachable(string hostName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(hostName, nameof (hostName));
      byte[] requestPacket = new byte[1]{ (byte) 3 };
      byte[] responsePacket;
      try
      {
        responsePacket = SqlBrowserServiceUtil.SendUdpRequest(hostName, 1434, requestPacket);
      }
      catch (SocketException ex)
      {
        return false;
      }
      return SqlBrowserServiceUtil.ValidResponse(responsePacket);
    }

    public static List<SqlInstanceInfo> GetAvailableInstances(string hostName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(hostName, nameof (hostName));
      byte[] requestPacket = new byte[1]{ (byte) 3 };
      byte[] numArray;
      try
      {
        numArray = SqlBrowserServiceUtil.SendUdpRequest(hostName, 1434, requestPacket);
      }
      catch (SocketException ex)
      {
        return (List<SqlInstanceInfo>) null;
      }
      if (!SqlBrowserServiceUtil.ValidResponse(numArray))
        return (List<SqlInstanceInfo>) null;
      string[] strArray = Encoding.ASCII.GetString(numArray, 3, numArray.Length - 3).Split(new string[1]
      {
        ";;"
      }, StringSplitOptions.RemoveEmptyEntries);
      List<SqlInstanceInfo> availableInstances = new List<SqlInstanceInfo>();
      foreach (string browserInfo in strArray)
      {
        SqlInstanceInfo sqlInstanceInfo = SqlInstanceInfo.Parse(browserInfo);
        availableInstances.Add(sqlInstanceInfo);
      }
      return availableInstances;
    }

    private static byte[] SendUdpRequest(string browserHostname, int port, byte[] requestPacket)
    {
      byte[] numArray = (byte[]) null;
      using (UdpClient udpClient = new UdpClient(AddressFamily.InterNetwork))
      {
        if (udpClient.SendAsync(requestPacket, requestPacket.Length, browserHostname, port).Wait(1000))
        {
          Task<UdpReceiveResult> async;
          if ((async = udpClient.ReceiveAsync()).Wait(1000))
            numArray = async.Result.Buffer;
        }
      }
      return numArray;
    }

    private static bool ValidResponse(byte[] responsePacket) => responsePacket != null && responsePacket.Length >= 3 && responsePacket[0] == (byte) 5 && (int) BitConverter.ToUInt16(responsePacket, 1) == responsePacket.Length - 3;
  }
}
