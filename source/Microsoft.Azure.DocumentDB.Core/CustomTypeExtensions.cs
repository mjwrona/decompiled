// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CustomTypeExtensions
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.IO;
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
    public const string SDKName = "documentdb-netcore-sdk";
    public const string SDKVersion = "2.10.0";

    public static bool IsAssignableFrom(this Type type, Type c) => type.GetTypeInfo().IsAssignableFrom(c);

    public static bool IsSubclassOf(this Type type, Type c) => type.GetTypeInfo().IsSubclassOf(c);

    public static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr) => type.GetTypeInfo().GetMethod(name, bindingAttr);

    public static MethodInfo GetMethod(this Type type, string name) => type.GetTypeInfo().GetMethod(name);

    public static MethodInfo GetMethod(this Type type, string name, Type[] types) => type.GetTypeInfo().GetMethod(name, types);

    public static Type[] GetGenericArguments(this Type type) => type.GetTypeInfo().GetGenericArguments();

    public static PropertyInfo GetProperty(this Type type, string name) => type.GetTypeInfo().GetProperty(name);

    public static PropertyInfo[] GetProperties(this Type type) => type.GetTypeInfo().GetProperties();

    public static PropertyInfo[] GetProperties(this Type type, BindingFlags bindingAttr) => type.GetTypeInfo().GetProperties(bindingAttr);

    public static Type[] GetInterfaces(this Type type) => type.GetTypeInfo().GetInterfaces();

    public static ConstructorInfo GetConstructor(this Type type, Type[] types) => type.GetTypeInfo().GetConstructor(types);

    public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute => CustomAttributeExtensions.GetCustomAttribute<T>(type.GetTypeInfo(), inherit);

    public static IEnumerable<Attribute> GetCustomAttributes(
      this Type type,
      Type attributeType,
      bool inherit)
    {
      return CustomAttributeExtensions.GetCustomAttributes(type.GetTypeInfo(), attributeType, inherit);
    }

    public static byte[] GetBuffer(this MemoryStream stream)
    {
      ArraySegment<byte> buffer;
      stream.TryGetBuffer(out buffer);
      return buffer.Array;
    }

    public static void Close(this Stream stream) => stream.Dispose();

    public static void Close(this TcpClient tcpClient) => tcpClient.Dispose();

    public static string GetLeftPart(this Uri uri, UriPartial part)
    {
      switch (part)
      {
        case UriPartial.Scheme:
          return uri.GetComponents(UriComponents.Scheme | UriComponents.KeepDelimiter, UriFormat.UriEscaped);
        case UriPartial.Authority:
          return uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo, UriFormat.UriEscaped);
        case UriPartial.Path:
          return uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.UserInfo | UriComponents.Path, UriFormat.UriEscaped);
        case UriPartial.Query:
          return uri.GetComponents(UriComponents.HttpRequestUrl | UriComponents.UserInfo, UriFormat.UriEscaped);
        default:
          throw new ArgumentException("Invalid part", nameof (part));
      }
    }

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
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1} {2}/{3}", (object) PlatformApis.GetOSPlatform(), string.IsNullOrEmpty(osVersion) ? (object) "Unknown" : (object) osVersion.Trim(), (object) "documentdb-netcore-sdk", (object) "2.10.0");
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
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && IntPtr.Size == 8 && ServiceInteropWrapper.AssembliesExist.Value)
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
