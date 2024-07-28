// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.ExceptionConverter
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  internal static class ExceptionConverter
  {
    public const int MaxParsedStackLength = 32768;

    internal static ExceptionDetails ConvertToExceptionDetails(
      Exception exception,
      ExceptionDetails parentExceptionDetails)
    {
      ExceptionDetails withoutStackInfo = ExceptionDetails.CreateWithoutStackInfo(exception, parentExceptionDetails);
      Tuple<List<Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame>, bool> tuple = ExceptionConverter.SanitizeStackFrame<System.Diagnostics.StackFrame, Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame>((IList<System.Diagnostics.StackFrame>) new StackTrace(exception, true).GetFrames(), new Func<System.Diagnostics.StackFrame, int, Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame>(ExceptionConverter.GetStackFrame), new Func<Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame, int>(ExceptionConverter.GetStackFrameLength));
      withoutStackInfo.parsedStack = (IList<Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame>) tuple.Item1;
      withoutStackInfo.hasFullStack = tuple.Item2;
      return withoutStackInfo;
    }

    private static Tuple<List<TOutput>, bool> SanitizeStackFrame<TInput, TOutput>(
      IList<TInput> inputList,
      Func<TInput, int, TOutput> converter,
      Func<TOutput, int> lengthGetter)
    {
      List<TOutput> outputList = new List<TOutput>();
      bool flag = true;
      if (inputList != null && inputList.Count > 0)
      {
        int num = 0;
        for (int index1 = 0; index1 < inputList.Count; ++index1)
        {
          int index2 = index1 % 2 == 0 ? inputList.Count - 1 - index1 / 2 : index1 / 2;
          TOutput output = converter(inputList[index2], index2);
          num += lengthGetter(output);
          if (num > 32768)
          {
            flag = false;
            break;
          }
          outputList.Insert(outputList.Count / 2, output);
        }
      }
      return new Tuple<List<TOutput>, bool>(outputList, flag);
    }

    private static Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame GetStackFrame(
      System.Diagnostics.StackFrame stackFrame,
      int frameId)
    {
      Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame stackFrame1 = new Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame()
      {
        level = frameId
      };
      MethodBase method = stackFrame.GetMethod();
      string str = !(method.DeclaringType != (Type) null) ? method.Name : method.DeclaringType.FullName + "." + method.Name;
      stackFrame1.method = str;
      stackFrame1.assembly = method.Module.Assembly.FullName;
      stackFrame1.fileName = stackFrame.GetFileName();
      int fileLineNumber = stackFrame.GetFileLineNumber();
      if (fileLineNumber != 0)
        stackFrame1.line = fileLineNumber;
      return stackFrame1;
    }

    private static int GetStackFrameLength(Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.StackFrame stackFrame) => (stackFrame.method == null ? 0 : stackFrame.method.Length) + (stackFrame.assembly == null ? 0 : stackFrame.assembly.Length) + (stackFrame.fileName == null ? 0 : stackFrame.fileName.Length);
  }
}
