// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.FeedResponseBinder
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Documents.Client
{
  internal static class FeedResponseBinder
  {
    public static FeedResponse<T> Convert<T>(FeedResponse<object> dynamicFeed)
    {
      if ((object) typeof (T) == (object) typeof (object))
        return (FeedResponse<T>) dynamicFeed;
      IList<T> result = (IList<T>) new List<T>();
      foreach (object obj1 in dynamicFeed)
      {
        // ISSUE: reference to a compiler-generated field
        if (FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, T>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (T), typeof (FeedResponseBinder)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        T obj2 = FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0.Target((CallSite) FeedResponseBinder.\u003C\u003Eo__0<T>.\u003C\u003Ep__0, obj1);
        result.Add(obj2);
      }
      return new FeedResponse<T>((IEnumerable<T>) result, dynamicFeed.Count, dynamicFeed.Headers, dynamicFeed.UseETagAsContinuation, dynamicFeed.QueryMetrics, dynamicFeed.PartitionedClientSideRequestStatistics, responseLengthBytes: dynamicFeed.ResponseLengthBytes);
    }

    public static IQueryable<T> AsQueryable<T>(FeedResponse<object> dynamicFeed) => FeedResponseBinder.Convert<T>(dynamicFeed).AsQueryable<T>();
  }
}
