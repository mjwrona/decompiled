// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ServiceInteropWrapper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Documents
{
  internal static class ServiceInteropWrapper
  {
    internal static Lazy<bool> AssembliesExist = new Lazy<bool>((Func<bool>) (() =>
    {
      if (!ServiceInteropWrapper.IsGatewayAllowedToParseQueries())
        return true;
      DefaultTrace.TraceInformation("Assembly location: " + typeof (ServiceInteropWrapper).GetTypeInfo().Assembly.Location);
      if (typeof (ServiceInteropWrapper).GetTypeInfo().Assembly.IsDynamic)
        return true;
      string directoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      string[] strArray = new string[1]
      {
        "Microsoft.Azure.Documents.ServiceInterop.dll"
      };
      foreach (string path2 in strArray)
      {
        string path = Path.Combine(directoryName, path2);
        if (!File.Exists(path))
        {
          DefaultTrace.TraceVerbose("ServiceInteropWrapper assembly not found at " + path);
          return false;
        }
      }
      return true;
    }));
    private const string DisableSkipInterop = "DisableSkipInterop";
    private const string AllowGatewayToParseQueries = "AllowGatewayToParseQueries";

    [DllImport("Microsoft.Azure.Documents.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint GetPartitionKeyRangesFromQuery(
      [In] IntPtr serviceProvider,
      [MarshalAs(UnmanagedType.LPWStr), In] string query,
      [In] bool requireFormattableOrderByQuery,
      [In] bool isContinuationExpected,
      [In] bool allowNonValueAggregateQuery,
      [In] bool hasLogicalPartitionKey,
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr), In] string[] partitionKeyDefinitionPathTokens,
      [MarshalAs(UnmanagedType.LPArray), In] uint[] partitionKeyDefinitionPathTokenLengths,
      [In] uint partitionKeyDefinitionPathCount,
      [In] PartitionKind partitionKind,
      [In, Out] IntPtr serializedQueryExecutionInfoBuffer,
      [In] uint serializedQueryExecutionInfoBufferLength,
      out uint serializedQueryExecutionInfoResultLength);

    [DllImport("Microsoft.Azure.Documents.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint CreateServiceProvider(
      [MarshalAs(UnmanagedType.LPStr), In] string configJsonString,
      out IntPtr serviceProvider);

    [DllImport("Microsoft.Azure.Documents.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint UpdateServiceProvider([In] IntPtr serviceProvider, [MarshalAs(UnmanagedType.LPStr), In] string configJsonString);

    internal static bool IsGatewayAllowedToParseQueries()
    {
      bool? setting1 = ServiceInteropWrapper.GetSetting("AllowGatewayToParseQueries");
      if (setting1.HasValue)
        return setting1.Value;
      bool? setting2 = ServiceInteropWrapper.GetSetting("DisableSkipInterop");
      return !setting2.HasValue || !setting2.Value;
    }

    private static bool? BoolParse(string boolValueString)
    {
      if (!string.IsNullOrEmpty(boolValueString))
      {
        if (!string.Equals(bool.TrueString, boolValueString, StringComparison.OrdinalIgnoreCase))
        {
          int num = 1;
          if (!string.Equals(num.ToString(), boolValueString, StringComparison.OrdinalIgnoreCase))
          {
            if (!string.Equals(bool.FalseString, boolValueString, StringComparison.OrdinalIgnoreCase))
            {
              num = 0;
              if (!string.Equals(num.ToString(), boolValueString, StringComparison.OrdinalIgnoreCase))
              {
                bool result = false;
                if (!bool.TryParse(boolValueString, out result))
                  return new bool?(result);
                goto label_9;
              }
            }
            return new bool?(false);
          }
        }
        return new bool?(true);
      }
label_9:
      return new bool?();
    }

    private static bool? GetSetting(string key)
    {
      string environmentVariable = Environment.GetEnvironmentVariable(key);
      DefaultTrace.TraceInformation("ServiceInteropWrapper read " + key + " environment variable as " + environmentVariable);
      bool? nullable = ServiceInteropWrapper.BoolParse(environmentVariable);
      DefaultTrace.TraceInformation(string.Format("ServiceInteropWrapper read  parsed {0} environment variable as {1}", (object) key, (object) nullable));
      return nullable.HasValue ? new bool?(nullable.Value) : nullable;
    }
  }
}
