// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CustomTypeExtensions
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.Azure.Documents
{
  internal static class CustomTypeExtensions
  {
    public const int UnicodeEncodingCharSize = 2;
    public const string SDKName = "cosmos-netstandard-sdk";
    public const string SDKVersion = "3.29.4";

    public static Delegate CreateDelegate(Type delegateType, object target, MethodInfo methodInfo) => methodInfo.CreateDelegate(delegateType, target);

    public static IntPtr SecureStringToCoTaskMemAnsi(SecureString secureString) => SecureStringMarshal.SecureStringToCoTaskMemAnsi(secureString);

    public static void SetActivityId(ref Guid id) => EventSource.SetCurrentThreadActivityId(id);

    public static Random GetRandomNumber()
    {
      using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
      {
        byte[] data = new byte[4];
        randomNumberGenerator.GetBytes(data);
        return new Random(BitConverter.ToInt32(data, 0));
      }
    }

    public static QueryRequestPerformanceActivity StartActivity(DocumentServiceRequest request) => (QueryRequestPerformanceActivity) null;

    public static string GenerateBaseUserAgentString()
    {
      string osVersion = PlatformApis.GetOSVersion();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1} {2}/{3}", (object) PlatformApis.GetOSPlatform(), string.IsNullOrEmpty(osVersion) ? (object) "Unknown" : (object) osVersion.Trim(), (object) "cosmos-netstandard-sdk", (object) "3.29.4");
    }

    public static bool ConfirmOpen(Socket socket)
    {
      bool blocking = socket.Blocking;
      try
      {
        byte[] buffer = new byte[1];
        socket.Blocking = false;
        socket.Send(buffer, 0, SocketFlags.None);
        return true;
      }
      catch (SocketException ex)
      {
        return ex.SocketErrorCode == SocketError.WouldBlock;
      }
      catch (ObjectDisposedException ex)
      {
        return false;
      }
      finally
      {
        socket.Blocking = blocking;
      }
    }

    public static bool ByPassQueryParsing()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && ServiceInteropWrapper.Is64BitProcess && ServiceInteropWrapper.AssembliesExist.Value)
        return false;
      DefaultTrace.TraceVerbose(string.Format("Bypass query parsing. IsWindowsOSPlatform {0} IntPtr.Size is {1} ServiceInteropWrapper.AssembliesExist {2}", (object) RuntimeInformation.IsOSPlatform(OSPlatform.Windows), (object) IntPtr.Size, (object) ServiceInteropWrapper.AssembliesExist.Value));
      return true;
    }

    public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

    public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

    public static bool IsValueType(this Type type) => type.GetTypeInfo().IsValueType;

    public static bool IsInterface(this Type type) => type.GetTypeInfo().IsInterface;

    public static Type GetBaseType(this Type type) => type.GetTypeInfo().BaseType;

    public static Type GeUnderlyingSystemType(this Type type) => type.GetTypeInfo().UnderlyingSystemType;

    public static Assembly GetAssembly(this Type type) => type.GetTypeInfo().Assembly;

    public static IEnumerable<CustomAttributeData> GetsCustomAttributes(this Type type) => type.GetTypeInfo().CustomAttributes;
  }
}
