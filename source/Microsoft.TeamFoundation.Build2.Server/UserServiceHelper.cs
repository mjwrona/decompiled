// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.UserServiceHelper
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class UserServiceHelper
  {
    private const string c_layer = "UserServiceHelper";
    private const string c_wizardContinuationAttributeName = "pipelines.wizard-continuation-token";
    private const string c_pipelineEditAttributeName = "pipelines.edit-continuation-token";
    private static readonly int TracePoint = 15287209;

    public static bool HasUserContext(IVssRequestContext requestContext, out string repositoryId)
    {
      Guid projectId;
      string str;
      return UserServiceHelper.TryGetContinuationContext(requestContext, out projectId, out repositoryId, out str) || UserServiceHelper.TryGetEditContext(requestContext, out projectId, out str, out string _, out repositoryId);
    }

    public static void SaveContinuationContext(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string telemetrySession,
      int expirationInMinutes)
    {
      DateTime dateTime = DateTime.UtcNow.AddMinutes((double) expirationInMinutes);
      string continuationContext = JsonConvert.SerializeObject((object) new
      {
        collectionId = requestContext.ServiceHost.InstanceId,
        projectId = projectId,
        expiration = dateTime.ToString("o"),
        repositoryId = repositoryId,
        telemetrySession = telemetrySession
      });
      IUserService service = requestContext.GetService<IUserService>();
      service.DeleteAttribute(requestContext, requestContext.GetUserIdentity().Id, "pipelines.edit-continuation-token");
      UserServiceHelper.SaveContextObject(requestContext, service, continuationContext, "pipelines.wizard-continuation-token");
    }

    public static void SaveEditContext(
      IVssRequestContext requestContext,
      Guid projectId,
      string pipelineId,
      string repositoryId,
      string nonce,
      int expirationInMinutes)
    {
      DateTime dateTime = DateTime.UtcNow.AddMinutes((double) expirationInMinutes);
      string continuationContext = JsonConvert.SerializeObject((object) new
      {
        collectionId = requestContext.ServiceHost.InstanceId,
        projectId = projectId,
        expiration = dateTime.ToString("o"),
        pipelineId = pipelineId,
        repositoryId = repositoryId,
        nonce = nonce
      });
      IUserService service = requestContext.GetService<IUserService>();
      service.DeleteAttribute(requestContext, requestContext.GetUserIdentity().Id, "pipelines.wizard-continuation-token");
      UserServiceHelper.SaveContextObject(requestContext, service, continuationContext, "pipelines.edit-continuation-token");
    }

    public static void DeleteEditContext(IVssRequestContext requestContext) => requestContext.GetService<IUserService>().DeleteAttribute(requestContext, requestContext.GetUserIdentity().Id, "pipelines.edit-continuation-token");

    private static void SaveContextObject(
      IVssRequestContext requestContext,
      IUserService userService,
      string continuationContext,
      string saveAttributeName)
    {
      requestContext.TraceInfo(UserServiceHelper.TracePoint, nameof (UserServiceHelper), "{0} - context={1}", (object) nameof (SaveContextObject), (object) continuationContext);
      Guid id = requestContext.GetUserIdentity().Id;
      userService.SetAttribute(requestContext, id, saveAttributeName, continuationContext);
    }

    public static bool TryGetContinuationContext(
      IVssRequestContext requestContext,
      out Guid projectId,
      out string repositoryId,
      out string telemetrySession)
    {
      try
      {
        object contextObject = UserServiceHelper.GetContextObject(requestContext, "pipelines.wizard-continuation-token");
        ref Guid local1 = ref projectId;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, Guid>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Guid), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, Guid> target1 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, Guid>> p1 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__1;
        object obj1 = contextObject;
        object obj2;
        if (obj1 == null)
        {
          obj2 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (projectId), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj2 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__0, obj1);
        }
        Guid guid = target1((CallSite) p1, obj2);
        local1 = guid;
        ref string local2 = ref repositoryId;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target2 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p3 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__3;
        object obj3 = contextObject;
        object obj4;
        if (obj3 == null)
        {
          obj4 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (repositoryId), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj4 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__2.Target((CallSite) UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__2, obj3);
        }
        string str1 = target2((CallSite) p3, obj4);
        local2 = str1;
        ref string local3 = ref telemetrySession;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target3 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p5 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__5;
        object obj5 = contextObject;
        object obj6;
        if (obj5 == null)
        {
          obj6 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (telemetrySession), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj6 = UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__4.Target((CallSite) UserServiceHelper.\u003C\u003Eo__8.\u003C\u003Ep__4, obj5);
        }
        string str2 = target3((CallSite) p5, obj6);
        local3 = str2;
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(UserServiceHelper.TracePoint, nameof (UserServiceHelper), ex);
      }
      projectId = Guid.Empty;
      repositoryId = (string) null;
      telemetrySession = (string) null;
      return false;
    }

    public static bool TryGetEditContext(
      IVssRequestContext requestContext,
      out Guid projectId,
      out string pipelineId,
      out string nonce,
      out string repositoryId)
    {
      try
      {
        object contextObject = UserServiceHelper.GetContextObject(requestContext, "pipelines.edit-continuation-token");
        ref string local1 = ref pipelineId;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target1 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p1 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__1;
        object obj1 = contextObject;
        object obj2;
        if (obj1 == null)
        {
          obj2 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (pipelineId), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj2 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__0, obj1);
        }
        string str1 = target1((CallSite) p1, obj2);
        local1 = str1;
        ref Guid local2 = ref projectId;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, Guid>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (Guid), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, Guid> target2 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, Guid>> p3 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__3;
        object obj3 = contextObject;
        object obj4;
        if (obj3 == null)
        {
          obj4 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (projectId), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj4 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__2.Target((CallSite) UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__2, obj3);
        }
        Guid guid = target2((CallSite) p3, obj4);
        local2 = guid;
        ref string local3 = ref nonce;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target3 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p5 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__5;
        object obj5 = contextObject;
        object obj6;
        if (obj5 == null)
        {
          obj6 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (nonce), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj6 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__4.Target((CallSite) UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__4, obj5);
        }
        string str2 = target3((CallSite) p5, obj6);
        local3 = str2;
        ref string local4 = ref repositoryId;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target4 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__7.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p7 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__7;
        object obj7 = contextObject;
        object obj8;
        if (obj7 == null)
        {
          obj8 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (repositoryId), typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj8 = UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__6.Target((CallSite) UserServiceHelper.\u003C\u003Eo__9.\u003C\u003Ep__6, obj7);
        }
        string str3 = target4((CallSite) p7, obj8);
        local4 = str3;
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(UserServiceHelper.TracePoint, nameof (UserServiceHelper), ex);
      }
      projectId = Guid.Empty;
      pipelineId = (string) null;
      nonce = (string) null;
      repositoryId = (string) null;
      return false;
    }

    private static object GetContextObject(
      IVssRequestContext requestContext,
      string loadAttributeName)
    {
      UserAttribute attribute = requestContext.GetService<IUserService>().GetAttribute(requestContext, requestContext.GetUserId(), loadAttributeName);
      if (!string.IsNullOrEmpty(attribute?.Value))
      {
        requestContext.TraceInfo(UserServiceHelper.TracePoint, nameof (UserServiceHelper), "{0} - continuationContext={1}", (object) nameof (GetContextObject), (object) attribute.Value);
        object contextObject = JsonConvert.DeserializeObject(attribute.Value);
        DateTime? nullable;
        ref DateTime? local = ref nullable;
        // ISSUE: reference to a compiler-generated field
        if (UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, DateTime>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (DateTime), typeof (UserServiceHelper)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, DateTime> target = UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, DateTime>> p1 = UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__1;
        object obj1 = contextObject;
        object obj2;
        if (obj1 == null)
        {
          obj2 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "expiration", typeof (UserServiceHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj2 = UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) UserServiceHelper.\u003C\u003Eo__10.\u003C\u003Ep__0, obj1);
        }
        DateTime dateTime = target((CallSite) p1, obj2);
        local = new DateTime?(dateTime);
        if (nullable.HasValue && nullable.Value.ToUniversalTime() > DateTime.UtcNow)
          return contextObject;
      }
      return (object) null;
    }
  }
}
