// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ServiceInteropWrapper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Azure.Documents
{
  internal static class ServiceInteropWrapper
  {
    internal static Lazy<bool> AssembliesExist = new Lazy<bool>((Func<bool>) (() => ServiceInteropWrapper.CheckIfAssembliesExist(out string _)));
    internal static readonly bool Is64BitProcess = IntPtr.Size == 8;
    internal static readonly bool IsWindowsOSPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private const string DisableSkipInterop = "DisableSkipInterop";
    private const string AllowGatewayToParseQueries = "AllowGatewayToParseQueries";

    internal static bool UseServiceInterop(QueryPlanGenerationMode queryPlanRetrievalMode)
    {
      switch (queryPlanRetrievalMode)
      {
        case QueryPlanGenerationMode.DefaultWindowsX64NativeWithFallbackToGateway:
          return !CustomTypeExtensions.ByPassQueryParsing();
        case QueryPlanGenerationMode.WindowsX64NativeOnly:
          return true;
        case QueryPlanGenerationMode.GatewayOnly:
          return false;
        default:
          return !CustomTypeExtensions.ByPassQueryParsing();
      }
    }

    internal static bool CheckIfAssembliesExist(out string validationMessage)
    {
      validationMessage = string.Empty;
      try
      {
        if (!ServiceInteropWrapper.IsGatewayAllowedToParseQueries())
        {
          validationMessage = "The environment variable AllowGatewayToParseQueries is overriding the service interop if exists validation.";
          return true;
        }
        DefaultTrace.TraceInformation("Assembly location: " + Assembly.GetExecutingAssembly().Location);
        if (Assembly.GetExecutingAssembly().IsDynamic)
        {
          validationMessage = "The service interop if exists validation skipped because Assembly.GetExecutingAssembly().IsDynamic is true";
          return true;
        }
        string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Microsoft.Azure.Cosmos.ServiceInterop.dll");
        validationMessage = "The service interop location checked at " + path;
        if (File.Exists(path))
          return true;
        DefaultTrace.TraceInformation("ServiceInteropWrapper assembly not found at " + path);
        return false;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning(string.Format("ServiceInteropWrapper: Falling back to gateway. Finding ServiceInterop dll threw an exception {0}", (object) ex));
      }
      if (string.IsNullOrEmpty(validationMessage))
        validationMessage = "An unexpected exception occurred while checking the file location";
      return false;
    }

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
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

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint GetPartitionKeyRangesFromQuery2(
      [In] IntPtr serviceProvider,
      [MarshalAs(UnmanagedType.LPWStr), In] string query,
      [In] bool requireFormattableOrderByQuery,
      [In] bool isContinuationExpected,
      [In] bool allowNonValueAggregateQuery,
      [In] bool hasLogicalPartitionKey,
      [In] bool bAllowDCount,
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr), In] string[] partitionKeyDefinitionPathTokens,
      [MarshalAs(UnmanagedType.LPArray), In] uint[] partitionKeyDefinitionPathTokenLengths,
      [In] uint partitionKeyDefinitionPathCount,
      [In] PartitionKind partitionKind,
      [In, Out] IntPtr serializedQueryExecutionInfoBuffer,
      [In] uint serializedQueryExecutionInfoBufferLength,
      out uint serializedQueryExecutionInfoResultLength);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint GetPartitionKeyRangesFromQuery3(
      [In] IntPtr serviceProvider,
      [MarshalAs(UnmanagedType.LPWStr), In] string query,
      [In] ServiceInteropWrapper.PartitionKeyRangesApiOptions partitionKeyRangesApiOptions,
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr), In] string[] partitionKeyDefinitionPathTokens,
      [MarshalAs(UnmanagedType.LPArray), In] uint[] partitionKeyDefinitionPathTokenLengths,
      [In] uint partitionKeyDefinitionPathCount,
      [In, Out] IntPtr serializedQueryExecutionInfoBuffer,
      [In] uint serializedQueryExecutionInfoBufferLength,
      out uint serializedQueryExecutionInfoResultLength);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern int CreateDistributedQueryContext(
      [In] IntPtr serviceProvider,
      [MarshalAs(UnmanagedType.LPWStr), In] string ownerRid,
      [MarshalAs(UnmanagedType.LPWStr), In] string query,
      [In] ulong queryLength,
      [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr), In] string[] partitionKeyDefinitionPathTokens,
      [MarshalAs(UnmanagedType.LPArray), In] uint[] partitionKeyDefinitionPathTokenLengths,
      [In] uint partitionKeyDefinitionPathCount,
      [In] PartitionKind partitionKind,
      out IntPtr queryContext);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern int CompileDistributedQuery(
      [In] IntPtr queryContext,
      out ServiceInteropWrapper.InteropDistributedQueryCompileResult result);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern int ExecuteDistributedQuery(
      [In] IntPtr queryContext,
      [In] IntPtr collatedResponses,
      [In] uint length,
      [In] bool bLocal,
      out IntPtr result,
      out uint resultLength,
      out IntPtr resultOwner);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint CreateServiceProvider(
      [MarshalAs(UnmanagedType.LPStr), In] string configJsonString,
      out IntPtr serviceProvider);

    [SuppressUnmanagedCodeSecurity]
    [DllImport("Microsoft.Azure.Cosmos.ServiceInterop.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true, ThrowOnUnmappableChar = true, BestFitMapping = false)]
    public static extern uint UpdateServiceProvider([In] IntPtr serviceProvider, [MarshalAs(UnmanagedType.LPStr), In] string configJsonString);

    internal static bool IsGatewayAllowedToParseQueries()
    {
      bool? setting = ServiceInteropWrapper.GetSetting("AllowGatewayToParseQueries");
      return !setting.HasValue || setting.Value;
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
                if (bool.TryParse(boolValueString, out result))
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
      if (nullable.HasValue)
        return new bool?(nullable.Value);
      string appSetting = ConfigurationManager.AppSettings[key];
      DefaultTrace.TraceInformation("ServiceInteropWrapper read " + key + " from AppConfig as " + appSetting + " ");
      bool? setting = ServiceInteropWrapper.BoolParse(appSetting);
      DefaultTrace.TraceInformation(string.Format("ServiceInteropWrapper read parsed {0} AppConfig as {1} ", (object) key, (object) setting));
      return setting;
    }

    public struct PartitionKeyRangesApiOptions
    {
      public int bRequireFormattableOrderByQuery;
      public int bIsContinuationExpected;
      public int bAllowNonValueAggregateQuery;
      public int bHasLogicalPartitionKey;
      public int bAllowDCount;
      public int bUseSystemPrefix;
      public int ePartitionKind;
      public int eGeospatialType;
      public long unusedReserved1;
      public long unusedReserved2;
      public long unusedReserved3;
      public long unusedReserved4;
    }

    public struct InteropDistributedQueryCompileResult
    {
      public IntPtr Query;
      public int QueryLength;
      public IntPtr ObfuscatedQuery;
      public int ObfuscatedQueryLength;
      public IntPtr Shape;
      public int ShapeLength;
      public ulong Signature;
      public ulong ShapeSignature;
      public bool NoSpatial;
      public IntPtr LocalILProgram;
      public int LocalILProgramLength;
      public IntPtr PartitionKeyRanges;
      public int PartitionKeyRangesLength;
      public IntPtr Errors;
      public int ErrorsLength;
    }
  }
}
