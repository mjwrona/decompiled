// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.GlobalPipelineContext
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public static class GlobalPipelineContext
  {
    internal const string ContextDataName = "SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C";

    internal static void Set(CorePipelineContext pipelineContext)
    {
      if (CallContext.LogicalGetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C") is Stack<CorePipelineContext> data1)
      {
        data1.Push(pipelineContext);
      }
      else
      {
        Stack<CorePipelineContext> data = new Stack<CorePipelineContext>();
        data.Push(pipelineContext);
        CallContext.LogicalSetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C", (object) data);
      }
    }

    public static CorePipelineContext<TId, TDoc> Get<TId, TDoc>()
      where TId : IEquatable<TId>
      where TDoc : CorePipelineDocument<TId>
    {
      if (!(CallContext.LogicalGetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C") is Stack<CorePipelineContext> data) || data.Count <= 0)
        throw new SearchServiceException("Pipeline context does not exist in the current call context.");
      return data.Peek() is CorePipelineContext<TId, TDoc> ? (CorePipelineContext<TId, TDoc>) data.Peek() : throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Expected pipeline context of type [<{0}, {1}>] but found [{2}].", (object) typeof (TId), (object) typeof (TDoc), (object) data.Peek().GetType())));
    }

    public static CorePipelineContext Get() => CallContext.LogicalGetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C") is Stack<CorePipelineContext> data && data.Count > 0 ? data.Peek() : throw new SearchServiceException("Pipeline context does not exist in the current call context.");

    public static bool TryGet<TId, TDoc>(out CorePipelineContext<TId, TDoc> pipelineContext)
      where TId : IEquatable<TId>
      where TDoc : CorePipelineDocument<TId>
    {
      if (CallContext.LogicalGetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C") is Stack<CorePipelineContext> data && data.Count > 0 && data.Peek() is CorePipelineContext<TId, TDoc>)
      {
        pipelineContext = (CorePipelineContext<TId, TDoc>) data.Peek();
        return true;
      }
      pipelineContext = (CorePipelineContext<TId, TDoc>) null;
      return false;
    }

    public static bool TryGet(out CorePipelineContext pipelineContext)
    {
      if (CallContext.LogicalGetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C") is Stack<CorePipelineContext> data && data.Count > 0)
      {
        pipelineContext = data.Peek();
        return true;
      }
      pipelineContext = (CorePipelineContext) null;
      return false;
    }

    internal static bool Clear<TId, TDoc>()
      where TId : IEquatable<TId>
      where TDoc : CorePipelineDocument<TId>
    {
      if (!(CallContext.LogicalGetData("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C") is Stack<CorePipelineContext> data))
        return false;
      if (data.Count > 0)
      {
        CorePipelineContext corePipelineContext = data.Peek();
        if (corePipelineContext is CorePipelineContext<TId, TDoc>)
          data.Pop();
        else
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Expected pipeline context of type [<{0}, {1}>] but found [{2}].", (object) typeof (TId), (object) typeof (TDoc), (object) corePipelineContext.GetType())));
      }
      if (data.Count == 0)
        GlobalPipelineContext.FreeCallContextDataSlot();
      return true;
    }

    internal static void FreeCallContextDataSlot() => CallContext.FreeNamedDataSlot("SearchServiceIndexingPipelineContext-7F908A85-26F7-44E9-B167-E9ADDAA9131C");
  }
}
