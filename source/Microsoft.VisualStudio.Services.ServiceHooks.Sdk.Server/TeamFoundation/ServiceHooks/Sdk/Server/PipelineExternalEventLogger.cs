// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ServiceHooks.Sdk.Server.PipelineExternalEventLogger
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.ServiceHooks.Sdk.Server
{
  public static class PipelineExternalEventLogger
  {
    private const int PipelineEvents = 12030213;

    public static void GitHubWebhookPayload(
      IVssRequestContext requestContext,
      string pipelineEventId,
      object eventObj,
      string eventType)
    {
      object obj1 = (object) new ExpandoObject();
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "repository", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target1 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p2 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__2;
      object obj2 = obj1;
      object obj3 = eventObj;
      object obj4;
      if (obj3 == null)
      {
        obj4 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__0.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__0, obj3);
        if (obj5 == null)
        {
          obj4 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj4 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__1.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__1, obj5);
        }
      }
      object obj6 = target1((CallSite) p2, obj2, obj4);
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "providerId", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__3.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__3, obj1, "github");
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "event_type", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__4.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__4, obj1, eventType);
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "sha", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target2 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p9 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__9;
      object obj9 = obj1;
      object obj10;
      switch (eventType)
      {
        case "push":
          object obj11 = eventObj;
          if (obj11 == null)
          {
            obj10 = (object) null;
            break;
          }
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "after", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj10 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__5.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__5, obj11);
          break;
        case "pull_request":
          object obj12 = eventObj;
          if (obj12 == null)
          {
            obj10 = (object) null;
            break;
          }
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pull_request", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj13 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__6.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__6, obj12);
          if (obj13 == null)
          {
            obj10 = (object) null;
            break;
          }
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj14 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__7.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__7, obj13);
          if (obj14 == null)
          {
            obj10 = (object) null;
            break;
          }
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__8 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sha", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj10 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__8.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__8, obj14);
          break;
        default:
          obj10 = (object) null;
          break;
      }
      object obj15 = target2((CallSite) p9, obj9, obj10);
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "pr_number", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target3 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p11 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__11;
      object obj16 = obj1;
      object obj17;
      if (!(eventType == "pull_request"))
      {
        obj17 = (object) null;
      }
      else
      {
        object obj18 = eventObj;
        if (obj18 == null)
        {
          obj17 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__10 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj17 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__10.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__10, obj18);
        }
      }
      object obj19 = target3((CallSite) p11, obj16, obj17);
      IVssRequestContext requestContext1 = requestContext;
      string pipelineEventId1 = pipelineEventId;
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      string payload = PipelineEventProperties.Payload;
      Dictionary<string, string> dictionary2 = dictionary1;
      string key = payload;
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PipelineExternalEventLogger)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p13 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__12 = CallSite<Func<CallSite, Type, object, bool, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Serialize", (IEnumerable<Type>) null, typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.NamedArgument, "cultureInvariant")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj20 = PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__12.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__0.\u003C\u003Ep__12, typeof (JsonUtilities), obj1, true);
      string str = target4((CallSite) p13, obj20);
      dictionary2[key] = str;
      Dictionary<string, string> properties = dictionary1;
      PipelineExternalEventLogger.Log(requestContext1, PipelineEventType.ServiceHookEventReceived, pipelineEventId1, (IDictionary<string, string>) properties);
    }

    public static void BitbucketWebhookPayload(
      IVssRequestContext requestContext,
      string pipelineEventId,
      object eventObj,
      string eventType)
    {
      object obj1 = (object) new ExpandoObject();
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "repository", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target1 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p2 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__2;
      object obj2 = obj1;
      object obj3 = eventObj;
      object obj4;
      if (obj3 == null)
      {
        obj4 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__0, obj3);
        if (obj5 == null)
        {
          obj4 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj4 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__1.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__1, obj5);
        }
      }
      object obj6 = target1((CallSite) p2, obj2, obj4);
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "providerId", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__3.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__3, obj1, "bitbucket");
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "event_type", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__4.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__4, obj1, eventType);
      if (string.Equals(eventType, "repo:push", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, BitbucketData.V2.PushEvent>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (BitbucketData.V2.PushEvent), typeof (PipelineExternalEventLogger)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, BitbucketData.V2.PushEvent> target2 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__6.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, BitbucketData.V2.PushEvent>> p6 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__6;
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
            {
              typeof (BitbucketData.V2.PushEvent)
            }, typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj9 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__5.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__5, eventObj);
          BitbucketData.V2.Change change = ((IEnumerable<BitbucketData.V2.Change>) target2((CallSite) p6, obj9).Push.Changes).First<BitbucketData.V2.Change>();
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "sha", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj10 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__7.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__7, obj1, change?.New?.Target?.Hash);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12030213, PipelineEventProperties.PipelineEventsArea, PipelineEventProperties.PipelineEventsLayer, ex);
        }
      }
      if (string.Equals(eventType, "pullrequest:created", StringComparison.OrdinalIgnoreCase) || string.Equals(eventType, "pullrequest:updated", StringComparison.OrdinalIgnoreCase))
      {
        try
        {
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__9 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, BitbucketData.V2.PullRequestEvent>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (BitbucketData.V2.PullRequestEvent), typeof (PipelineExternalEventLogger)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, BitbucketData.V2.PullRequestEvent> target3 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__9.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, BitbucketData.V2.PullRequestEvent>> p9 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__9;
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__8 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToObject", (IEnumerable<Type>) new Type[1]
            {
              typeof (BitbucketData.V2.PullRequestEvent)
            }, typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj11 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__8.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__8, eventObj);
          BitbucketData.V2.PullRequestEvent pullRequestEvent = target3((CallSite) p9, obj11);
          // ISSUE: reference to a compiler-generated field
          if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__10 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.SetMember(CSharpBinderFlags.None, "sha", typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj12 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__10.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__10, obj1, pullRequestEvent?.PullRequest?.Source?.Commit?.Hash);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12030213, PipelineEventProperties.PipelineEventsArea, PipelineEventProperties.PipelineEventsLayer, ex);
        }
      }
      IVssRequestContext requestContext1 = requestContext;
      string pipelineEventId1 = pipelineEventId;
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      string payload = PipelineEventProperties.Payload;
      Dictionary<string, string> dictionary2 = dictionary1;
      string key = payload;
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PipelineExternalEventLogger)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__12.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p12 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__12;
      // ISSUE: reference to a compiler-generated field
      if (PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__11 = CallSite<Func<CallSite, Type, object, bool, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Serialize", (IEnumerable<Type>) null, typeof (PipelineExternalEventLogger), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.NamedArgument, "cultureInvariant")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__11.Target((CallSite) PipelineExternalEventLogger.\u003C\u003Eo__1.\u003C\u003Ep__11, typeof (JsonUtilities), obj1, true);
      string str = target4((CallSite) p12, obj13);
      dictionary2[key] = str;
      Dictionary<string, string> properties = dictionary1;
      PipelineExternalEventLogger.Log(requestContext1, PipelineEventType.ServiceHookEventReceived, pipelineEventId1, (IDictionary<string, string>) properties);
    }

    public static void Log(
      IVssRequestContext requestContext,
      PipelineEventType type,
      string pipelineEventId,
      IDictionary<string, string> properties = null)
    {
      properties = properties ?? (IDictionary<string, string>) new Dictionary<string, string>();
      if (string.IsNullOrWhiteSpace(pipelineEventId))
      {
        requestContext.Trace(12030213, TraceLevel.Error, PipelineEventProperties.PipelineEventsArea, PipelineEventProperties.PipelineEventsLayer, "PipelineEventId is null for type '{0}'\n{1}", (object) type, (object) JsonConvert.SerializeObject((object) properties));
      }
      else
      {
        string format = JsonConvert.SerializeObject((object) new Dictionary<string, object>()
        {
          [PipelineEventProperties.EventType] = (object) type.ToString(),
          [PipelineEventProperties.PipelineEventId] = (object) pipelineEventId,
          [PipelineEventProperties.Properties] = (object) properties
        });
        requestContext.TraceAlways(12030213, TraceLevel.Verbose, PipelineEventProperties.PipelineEventsArea, PipelineEventProperties.PipelineEventsLayer, format);
      }
    }

    public static string GetPipelineEventId(IVssRequestContext requestContext) => requestContext.E2EId.ToString();
  }
}
