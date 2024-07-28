// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.PublishingUtils
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class PublishingUtils
  {
    private static readonly string s_layer = typeof (PublishingUtils).Name;
    private static readonly string s_area = typeof (PublishingUtils).Namespace;
    private const string c_openPullRequestState = "open";
    private const string c_helpCommandKeyword = "help";
    private static readonly IReadOnlyList<string> s_supportedKeywords = (IReadOnlyList<string>) new List<string>()
    {
      "/AzurePipelines",
      "/AzurePipeline",
      "/AzPipelines",
      "/AzPipeline",
      "/Azp"
    };

    public static bool DoesIncomingPayloadMatchHash(
      IVssRequestContext requestContext,
      byte[] payload,
      string payloadHash,
      string strongBoxDrawerName,
      string strongBoxKey,
      PublishingUtils.HashingMethod hashingMethod = PublishingUtils.HashingMethod.SHA1)
    {
      ArgumentUtility.CheckForNull<byte[]>(payload, nameof (payload));
      ArgumentUtility.CheckStringForNullOrEmpty(payloadHash, nameof (payloadHash));
      ArgumentUtility.CheckStringForNullOrEmpty(strongBoxDrawerName, nameof (strongBoxDrawerName));
      ArgumentUtility.CheckStringForNullOrEmpty(strongBoxKey, nameof (strongBoxKey));
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, strongBoxDrawerName, false);
      if (drawerId == Guid.Empty)
      {
        requestContext.Trace(1064260, TraceLevel.Error, PublishingUtils.s_area, PublishingUtils.s_layer, "Strong box drawer '" + strongBoxDrawerName + "' does not exist");
        throw new HttpException(403, (string) null);
      }
      string s;
      try
      {
        s = service.GetString(requestContext, drawerId, strongBoxKey);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        requestContext.TraceException(1064230, TraceLevel.Error, PublishingUtils.s_area, PublishingUtils.s_layer, (Exception) ex);
        return false;
      }
      byte[] bytes = Encoding.UTF8.GetBytes(s);
      byte[] hash;
      using (HMAC hmac = hashingMethod == PublishingUtils.HashingMethod.SHA1 ? (HMAC) new HMACSHA1(bytes) : (HMAC) new HMACSHA256(bytes))
        hash = hmac.ComputeHash(payload);
      byte[] byteArray = HexConverter.ToByteArray(payloadHash);
      return SecureCompare.TimeInvariantEquals(hash, byteArray);
    }

    public static ExternalGitPush GitHubPushEventToExternalGitPush(
      JObject gitHubPushEvent,
      string pipelineEventId = null)
    {
      object obj1 = (object) gitHubPushEvent;
      ExternalGitPush externalGitPush1 = new ExternalGitPush();
      ExternalGitPush externalGitPush2 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p2 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string, object> target2 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string, object>> p1 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.ResultIndexed, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__0, obj1);
      object obj3 = target2((CallSite) p1, obj2, "pushed_at");
      string str1 = target1((CallSite) p2, obj3);
      externalGitPush2.Id = str1;
      ExternalGitPush externalGitPush3 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p4 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "before", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__3.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__3, obj1);
      string str2 = target3((CallSite) p4, obj4);
      externalGitPush3.BeforeSha = str2;
      ExternalGitPush externalGitPush4 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__6.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p6 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__6;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "after", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__5.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__5, obj1);
      string str3 = target4((CallSite) p6, obj5);
      externalGitPush4.AfterSha = str3;
      ExternalGitPush externalGitPush5 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__8.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p8 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__8;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (pipelineEventId), typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__7.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__7, obj1) ?? (object) pipelineEventId;
      string str4 = target5((CallSite) p8, obj6);
      externalGitPush5.PipelineEventId = str4;
      ExternalGitPush externalGitPush6 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, ExternalGitRepo>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitRepo), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitRepo> target6 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__22.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitRepo>> p22 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__22;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__21 = CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitRepo", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[7]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object, object> target7 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>> p21 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__21;
      Type type1 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p10 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__10;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__9.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__9, obj1);
      object obj8 = target8((CallSite) p10, obj7);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target9 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__12.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p12 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__12;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj9 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__11.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__11, obj1);
      object obj10 = target9((CallSite) p12, obj9);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target10 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p14 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__14;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__13.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__13, obj1);
      object obj12 = target10((CallSite) p14, obj11);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "clone_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target11 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__16.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p16 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__16;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__15.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__15, obj1);
      object obj14 = target11((CallSite) p16, obj13);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target12 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__18.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p18 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__18;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj15 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__17.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__17, obj1);
      object obj16 = target12((CallSite) p18, obj15);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "default_branch", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__20.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p20 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__20;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj17 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__19.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__19, obj1);
      object obj18 = target13((CallSite) p20, obj17);
      object obj19 = target7((CallSite) p21, type1, obj8, obj10, obj12, obj14, obj16, obj18);
      ExternalGitRepo externalGitRepo = target6((CallSite) p22, obj19);
      externalGitPush6.Repo = externalGitRepo;
      ExternalGitPush externalGitPush7 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target14 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__24.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p24 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__24;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj20 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__23.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__23, obj1, "ref");
      string str5 = target14((CallSite) p24, obj20);
      externalGitPush7.GitRef = str5;
      ExternalGitPush externalGitPush8 = externalGitPush1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__38 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target15 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__38.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p38 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__38;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__37 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__37 = CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[7]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "avatarUrl"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "name"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object, object> target16 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__37.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>> p37 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__37;
      Type type2 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pusher", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj21 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__25.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__25, obj1);
      object obj22;
      if (obj21 == null)
      {
        obj22 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__26 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj22 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__26.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__26, obj21);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pusher", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj23 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__27.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__27, obj1);
      object obj24;
      if (obj23 == null)
      {
        obj24 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__28 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj24 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__28.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__28, obj23);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj25 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__29.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__29, obj1);
      object obj26;
      if (obj25 == null)
      {
        obj26 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__30 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj26 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__30.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__30, obj25);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj27 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__31.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__31, obj1);
      object obj28;
      if (obj27 == null)
      {
        obj28 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__32 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj28 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__32.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__32, obj27);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__33 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pusher", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj29 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__33.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__33, obj1);
      object obj30;
      if (obj29 == null)
      {
        obj30 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__34 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj30 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__34.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__34, obj29);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__35 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj31 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__35.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__35, obj1);
      object obj32;
      if (obj31 == null)
      {
        obj32 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__36 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj32 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__36.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__36, obj31);
      }
      object obj33 = target16((CallSite) p37, type2, obj22, obj24, obj26, obj28, obj30, obj32);
      ExternalGitUser externalGitUser1 = target15((CallSite) p38, obj33);
      externalGitPush8.PushedBy = externalGitUser1;
      externalGitPush1.Commits = (IList<ExternalGitCommit>) new List<ExternalGitCommit>();
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__63 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__63 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, IEnumerable> target17 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__63.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, IEnumerable>> p63 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__63;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__62 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__62 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "commits", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj34 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__62.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__62, obj1);
      foreach (object obj35 in target17((CallSite) p63, obj34))
      {
        ExternalGitCommit externalGitCommit1 = new ExternalGitCommit();
        ExternalGitCommit externalGitCommit2 = externalGitCommit1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__40 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__40 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target18 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__40.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p40 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__40;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__39 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj36 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__39.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__39, obj35, "id");
        string str6 = target18((CallSite) p40, obj36);
        externalGitCommit2.Sha = str6;
        externalGitCommit1.Repo = externalGitPush1.Repo;
        ExternalGitCommit externalGitCommit3 = externalGitCommit1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__48 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__48 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, ExternalGitUser> target19 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__48.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, ExternalGitUser>> p48 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__48;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__47 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__47 = CallSite<Func<CallSite, Type, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "name")
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, Type, object, object, object, object> target20 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__47.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, Type, object, object, object, object>> p47 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__47;
        Type type3 = typeof (PublishingUtils);
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__42 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target21 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__42.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p42 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__42;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__41 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj37 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__41.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__41, obj35, "author");
        object obj38 = target21((CallSite) p42, obj37);
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__44 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target22 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__44.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p44 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__44;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__43 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__43 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj39 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__43.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__43, obj35, "author");
        object obj40 = target22((CallSite) p44, obj39);
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__46 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__46 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target23 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__46.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p46 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__46;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__45 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__45 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj41 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__45.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__45, obj35, "author");
        object obj42 = target23((CallSite) p46, obj41);
        object obj43 = target20((CallSite) p47, type3, obj38, obj40, obj42);
        ExternalGitUser externalGitUser2 = target19((CallSite) p48, obj43);
        externalGitCommit3.Author = externalGitUser2;
        ExternalGitCommit externalGitCommit4 = externalGitCommit1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__50 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__50 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target24 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__50.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p50 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__50;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__49 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__49 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj44 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__49.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__49, obj35, "message");
        string str7 = target24((CallSite) p50, obj44);
        externalGitCommit4.Message = str7;
        ExternalGitCommit externalGitCommit5 = externalGitCommit1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__52 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__52 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target25 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__52.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p52 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__52;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__51 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__51 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj45 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__51.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__51, obj35, "url");
        string str8 = target25((CallSite) p52, obj45);
        externalGitCommit5.WebUrl = str8;
        ExternalGitCommit externalGitCommit6 = externalGitCommit1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__55 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__55 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target26 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__55.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p55 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__55;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__54 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__54 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target27 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__54.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p54 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__54;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__53 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__53 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj46 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__53.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__53, obj35, "distinct");
        object obj47 = target27((CallSite) p54, obj46, (object) null);
        if (target26((CallSite) p55, obj47))
        {
          externalGitCommit6.AdditionalProperties = (IDictionary<string, object>) new Dictionary<string, object>();
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__57 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__57 = CallSite<Action<CallSite, IDictionary<string, object>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Action<CallSite, IDictionary<string, object>, string, object> target28 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__57.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Action<CallSite, IDictionary<string, object>, string, object>> p57 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__57;
          IDictionary<string, object> additionalProperties = externalGitCommit6.AdditionalProperties;
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__56 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__56 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj48 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__56.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__56, obj35, "distinct");
          target28((CallSite) p57, additionalProperties, "Distinct", obj48);
        }
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__61 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__61 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target29 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__61.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p61 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__61;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__60 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__60 = CallSite<\u003C\u003EF\u007B00000200\u007D<CallSite, Type, object, DateTime, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "TryParse", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsOut, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: variable of a compiler-generated type
        \u003C\u003EF\u007B00000200\u007D<CallSite, Type, object, DateTime, object> target30 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__60.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<\u003C\u003EF\u007B00000200\u007D<CallSite, Type, object, DateTime, object>> p60 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__60;
        Type type4 = typeof (DateTime);
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__58 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__58 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj49 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__58.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__58, obj35, "timestamp");
        object obj50;
        if (obj49 == null)
        {
          obj50 = (object) null;
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__59 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__59 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          obj50 = PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__59.Target((CallSite) PublishingUtils.\u003C\u003Eo__7.\u003C\u003Ep__59, obj49);
        }
        DateTime dateTime;
        ref DateTime local = ref dateTime;
        object obj51 = target30((CallSite) p60, type4, obj50, ref local);
        if (target29((CallSite) p61, obj51))
          externalGitCommit6.CommitedDate = dateTime.ToUniversalTime();
        externalGitPush1.Commits.Add(externalGitCommit6);
      }
      return externalGitPush1;
    }

    public static ExternalGitPullRequest GitHubPullRequestEventToExternalGitPullRequest(
      JObject gitHubPullRequestEvent,
      string pipelineEventId = null)
    {
      object obj1 = (object) gitHubPullRequestEvent;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, JObject>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (JObject), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, JObject> target1 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, JObject>> p1 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "pull_request", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__0, obj1);
      JObject gitHubPullRequest = target1((CallSite) p1, obj2);
      if (gitHubPullRequest == null)
        throw new InvalidPayloadException(ServiceHooksPublisherResources.Error_InvalidPullRequestPayload);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target2 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p3 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (pipelineEventId), typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__2.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__2, obj1) ?? (object) pipelineEventId;
      pipelineEventId = target2((CallSite) p3, obj3);
      ExternalGitPullRequest externalGitPullRequest1 = PublishingUtils.GitHubPullRequestToExternalGitPullRequest(gitHubPullRequest, pipelineEventId);
      ExternalGitPullRequest externalGitPullRequest2 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target3 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p17 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__16 = CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[7]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "avatarUrl"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "type")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object, object> target4 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__16.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>> p16 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__16;
      Type type = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target5 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p5 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__4.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__4, obj1);
      object obj5 = target5((CallSite) p5, obj4);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p7 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__6.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__6, obj1);
      object obj7 = target6((CallSite) p7, obj6);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p9 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__8.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__8, obj1);
      object obj9 = target7((CallSite) p9, obj8);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p11 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__10.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__10, obj1);
      object obj11 = target8((CallSite) p11, obj10);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target9 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p13 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__12.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__12, obj1);
      object obj13 = target9((CallSite) p13, obj12);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "type", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target10 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__15.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p15 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__15;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj14 = PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__14.Target((CallSite) PublishingUtils.\u003C\u003Eo__8.\u003C\u003Ep__14, obj1);
      object obj15 = target10((CallSite) p15, obj14);
      object obj16 = target4((CallSite) p16, type, obj5, obj7, obj9, obj11, obj13, obj15);
      ExternalGitUser externalGitUser = target3((CallSite) p17, obj16);
      externalGitPullRequest2.Sender = externalGitUser;
      return externalGitPullRequest1;
    }

    public static ExternalGitPullRequest GitHubPullRequestToExternalGitPullRequest(
      JObject gitHubPullRequest,
      string pipelineEventId = null)
    {
      object obj1 = (object) gitHubPullRequest;
      ExternalGitPullRequest externalGitPullRequest1 = new ExternalGitPullRequest();
      ExternalGitPullRequest externalGitPullRequest2 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p1 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__0, obj1);
      string str1 = target1((CallSite) p1, obj2);
      externalGitPullRequest2.Id = str1;
      ExternalGitPullRequest externalGitPullRequest3 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target2 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p3 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__2.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__2, obj1);
      string str2 = target2((CallSite) p3, obj3);
      externalGitPullRequest3.Number = str2;
      ExternalGitPullRequest externalGitPullRequest4 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p5 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "title", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__4.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__4, obj1);
      string str3 = target3((CallSite) p5, obj4);
      externalGitPullRequest4.Title = str3;
      ExternalGitPullRequest externalGitPullRequest5 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p7 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__6.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__6, obj1);
      string str4 = target4((CallSite) p7, obj5);
      externalGitPullRequest5.Url = str4;
      ExternalGitPullRequest externalGitPullRequest6 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p9 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__8.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__8, obj1);
      string str5 = target5((CallSite) p9, obj6);
      externalGitPullRequest6.WebUrl = str5;
      ExternalGitPullRequest externalGitPullRequest7 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target6 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p11 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "body", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__10.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__10, obj1);
      string str6 = target6((CallSite) p11, obj7);
      externalGitPullRequest7.Description = str6;
      ExternalGitPullRequest externalGitPullRequest8 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target7 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p13 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "state", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__12.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__12, obj1);
      string str7 = target7((CallSite) p13, obj8);
      externalGitPullRequest8.State = str7;
      ExternalGitPullRequest externalGitPullRequest9 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target8 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__15.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p15 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__15;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "created_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj9 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__14.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__14, obj1);
      string str8 = target8((CallSite) p15, obj9);
      externalGitPullRequest9.CreatedAt = str8;
      ExternalGitPullRequest externalGitPullRequest10 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target9 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p17 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "updated_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__16.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__16, obj1);
      string str9 = target9((CallSite) p17, obj10);
      externalGitPullRequest10.UpdatedAt = str9;
      ExternalGitPullRequest externalGitPullRequest11 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target10 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__29.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p29 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__29;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__28 = CallSite<Func<CallSite, Type, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "avatarUrl"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object> target11 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__28.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object>> p28 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__28;
      Type type1 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target12 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p19 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__18.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__18, obj1);
      object obj12 = target12((CallSite) p19, obj11);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p21 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__21;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__20.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__20, obj1);
      object obj14 = target13((CallSite) p21, obj13);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__23.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p23 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__23;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj15 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__22.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__22, obj1);
      object obj16 = target14((CallSite) p23, obj15);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target15 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__25.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p25 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__25;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj17 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__24.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__24, obj1);
      object obj18 = target15((CallSite) p25, obj17);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target16 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__27.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p27 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__27;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj19 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__26.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__26, obj1);
      object obj20 = target16((CallSite) p27, obj19);
      object obj21 = target11((CallSite) p28, type1, obj12, obj14, obj16, obj18, obj20);
      ExternalGitUser externalGitUser = target10((CallSite) p29, obj21);
      externalGitPullRequest11.Sender = externalGitUser;
      ExternalGitPullRequest externalGitPullRequest12 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target17 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__31.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p31 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__31;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__30 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (pipelineEventId), typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj22 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__30.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__30, obj1) ?? (object) pipelineEventId;
      string str10 = target17((CallSite) p31, obj22);
      externalGitPullRequest12.PipelineEventId = str10;
      ExternalGitPullRequest externalGitPullRequest13 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__33 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target18 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__33.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p33 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__33;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__32 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "author_association", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj23 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__32.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__32, obj1);
      string str11 = target18((CallSite) p33, obj23);
      externalGitPullRequest13.AuthorAssociation = str11;
      ExternalGitPullRequest externalGitPullRequest14 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__36 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target19 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__36.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p36 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__36;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__35 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ref", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target20 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__35.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p35 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__35;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__34 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj24 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__34.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__34, obj1);
      object obj25 = target20((CallSite) p35, obj24);
      string str12 = target19((CallSite) p36, obj25);
      externalGitPullRequest14.TargetRef = str12;
      ExternalGitPullRequest externalGitPullRequest15 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__39 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target21 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__39.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p39 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__39;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__38 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sha", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target22 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__38.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p38 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__38;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__37 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj26 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__37.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__37, obj1);
      object obj27 = target22((CallSite) p38, obj26);
      string str13 = target21((CallSite) p39, obj27);
      externalGitPullRequest15.TargetSha = str13;
      externalGitPullRequest1.IsFork = false;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__48 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__48 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target23 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__48.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p48 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__48;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__42 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target24 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__42.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p42 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__42;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__41 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target25 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__41.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p41 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__41;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__40 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__40 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj28 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__40.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__40, obj1);
      object obj29 = target25((CallSite) p41, obj28);
      object obj30 = target24((CallSite) p42, obj29, (object) null);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__47 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__47 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      object obj31;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__47.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__47, obj30))
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__46 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__46 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target26 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__46.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p46 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__46;
        object obj32 = obj30;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__45 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__45 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target27 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__45.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p45 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__45;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__44 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target28 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__44.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p44 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__44;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__43 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__43 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj33 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__43.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__43, obj1);
        object obj34 = target28((CallSite) p44, obj33);
        object obj35 = target27((CallSite) p45, obj34, (object) null);
        obj31 = target26((CallSite) p46, obj32, obj35);
      }
      else
        obj31 = obj30;
      if (target23((CallSite) p48, obj31))
      {
        ExternalGitPullRequest externalGitPullRequest16 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__56 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__56 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target29 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__56.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p56 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__56;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__55 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__55 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target30 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__55.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p55 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__55;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__51 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__51 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target31 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__51.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p51 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__51;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__50 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__50 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target32 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__50.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p50 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__50;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__49 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__49 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj36 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__49.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__49, obj1);
        object obj37 = target32((CallSite) p50, obj36);
        object obj38 = target31((CallSite) p51, obj37);
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__54 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__54 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target33 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__54.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p54 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__54;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__53 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__53 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target34 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__53.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p53 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__53;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__52 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__52 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj39 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__52.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__52, obj1);
        object obj40 = target34((CallSite) p53, obj39);
        object obj41 = target33((CallSite) p54, obj40);
        object obj42 = target30((CallSite) p55, obj38, obj41);
        int num = target29((CallSite) p56, obj42) ? 1 : 0;
        externalGitPullRequest16.IsFork = num != 0;
      }
      externalGitPullRequest1.Draft = false;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__59 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__59 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target35 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__59.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p59 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__59;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__58 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__58 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target36 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__58.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p58 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__58;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__57 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__57 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "draft", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj43 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__57.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__57, obj1);
      object obj44 = target36((CallSite) p58, obj43, (object) null);
      if (target35((CallSite) p59, obj44))
      {
        ExternalGitPullRequest externalGitPullRequest17 = externalGitPullRequest1;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__61 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__61 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target37 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__61.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p61 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__61;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__60 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__60 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "draft", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj45 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__60.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__60, obj1);
        int num = target37((CallSite) p61, obj45) ? 1 : 0;
        externalGitPullRequest17.Draft = num != 0;
      }
      ExternalGitPullRequest externalGitPullRequest18 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__64 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__64 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target38 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__64.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p64 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__64;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__63 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__63 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ref", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target39 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__63.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p63 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__63;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__62 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__62 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj46 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__62.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__62, obj1);
      object obj47 = target39((CallSite) p63, obj46);
      string str14 = target38((CallSite) p64, obj47);
      externalGitPullRequest18.SourceRef = str14;
      ExternalGitPullRequest externalGitPullRequest19 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__67 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__67 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target40 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__67.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p67 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__67;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__66 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__66 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sha", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target41 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__66.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p66 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__66;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__65 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__65 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "head", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj48 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__65.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__65, obj1);
      object obj49 = target41((CallSite) p66, obj48);
      string str15 = target40((CallSite) p67, obj49);
      externalGitPullRequest19.SourceSha = str15;
      ExternalGitPullRequest externalGitPullRequest20 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__68 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__68 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string str16 = string.Format("refs/pull/{0}/merge", PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__68.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__68, obj1));
      externalGitPullRequest20.MergeRef = str16;
      ExternalGitPullRequest externalGitPullRequest21 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__70 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__70 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target42 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__70.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p70 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__70;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__69 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__69 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "merged_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj50 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__69.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__69, obj1);
      string str17 = target42((CallSite) p70, obj50);
      externalGitPullRequest21.MergedAt = str17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__72 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__72 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target43 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__72.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p72 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__72;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__71 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__71 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "mergeable", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj51 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__71.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__71, obj1);
      object obj52 = target43((CallSite) p72, obj51, (object) null);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__78 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__78 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__78.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__78, obj52))
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__77 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__77 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target44 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__77.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p77 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__77;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__76 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__76 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.Or, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target45 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__76.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p76 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__76;
        object obj53 = obj52;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__75 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__75 = CallSite<Func<CallSite, object, JTokenType, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, JTokenType, object> target46 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__75.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, JTokenType, object>> p75 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__75;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__74 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__74 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Type", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target47 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__74.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p74 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__74;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__73 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__73 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "mergeable", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj54 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__73.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__73, obj1);
        object obj55 = target47((CallSite) p74, obj54);
        object obj56 = target46((CallSite) p75, obj55, JTokenType.Null);
        object obj57 = target45((CallSite) p76, obj53, obj56);
        if (!target44((CallSite) p77, obj57))
        {
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__80 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__80 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof (bool), typeof (PublishingUtils)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target48 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__80.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p80 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__80;
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__79 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__79 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "mergeable", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj58 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__79.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__79, obj1);
          if (target48((CallSite) p80, obj58))
          {
            ExternalGitPullRequest externalGitPullRequest22 = externalGitPullRequest1;
            // ISSUE: reference to a compiler-generated field
            if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__82 == null)
            {
              // ISSUE: reference to a compiler-generated field
              PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__82 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, string> target49 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__82.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, string>> p82 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__82;
            // ISSUE: reference to a compiler-generated field
            if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__81 == null)
            {
              // ISSUE: reference to a compiler-generated field
              PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__81 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "merge_commit_sha", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj59 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__81.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__81, obj1);
            string str18 = target49((CallSite) p82, obj59);
            externalGitPullRequest22.MergeCommitSha = str18;
            externalGitPullRequest1.IsMergeable = new bool?(true);
            goto label_179;
          }
          else
          {
            externalGitPullRequest1.IsMergeable = new bool?(false);
            goto label_179;
          }
        }
      }
      externalGitPullRequest1.IsMergeable = new bool?();
label_179:
      ExternalGitPullRequest externalGitPullRequest23 = externalGitPullRequest1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__102 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__102 = CallSite<Func<CallSite, object, ExternalGitRepo>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitRepo), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitRepo> target50 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__102.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitRepo>> p102 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__102;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__101 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__101 = CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitRepo", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[7]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object, object> target51 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__101.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>> p101 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__101;
      Type type2 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__85 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__85 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target52 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__85.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p85 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__85;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__84 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__84 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target53 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__84.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p84 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__84;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__83 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__83 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj60 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__83.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__83, obj1);
      object obj61 = target53((CallSite) p84, obj60);
      object obj62 = target52((CallSite) p85, obj61);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__88 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__88 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target54 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__88.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p88 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__88;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__87 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__87 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target55 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__87.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p87 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__87;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__86 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__86 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj63 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__86.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__86, obj1);
      object obj64 = target55((CallSite) p87, obj63);
      object obj65 = target54((CallSite) p88, obj64);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__91 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__91 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target56 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__91.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p91 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__91;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__90 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__90 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target57 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__90.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p90 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__90;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__89 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__89 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj66 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__89.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__89, obj1);
      object obj67 = target57((CallSite) p90, obj66);
      object obj68 = target56((CallSite) p91, obj67);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__94 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__94 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "clone_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target58 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__94.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p94 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__94;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__93 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__93 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target59 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__93.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p93 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__93;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__92 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__92 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj69 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__92.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__92, obj1);
      object obj70 = target59((CallSite) p93, obj69);
      object obj71 = target58((CallSite) p94, obj70);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__97 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__97 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target60 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__97.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p97 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__97;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__96 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__96 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target61 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__96.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p96 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__96;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__95 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__95 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj72 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__95.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__95, obj1);
      object obj73 = target61((CallSite) p96, obj72);
      object obj74 = target60((CallSite) p97, obj73);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__100 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__100 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "default_branch", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target62 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__100.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p100 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__100;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__99 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__99 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repo", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target63 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__99.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p99 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__99;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__98 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__98 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "base", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj75 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__98.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__98, obj1);
      object obj76 = target63((CallSite) p99, obj75);
      object obj77 = target62((CallSite) p100, obj76);
      object obj78 = target51((CallSite) p101, type2, obj62, obj65, obj68, obj71, obj74, obj77);
      ExternalGitRepo externalGitRepo = target50((CallSite) p102, obj78);
      externalGitPullRequest23.Repo = externalGitRepo;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__105 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__105 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target64 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__105.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p105 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__105;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__104 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__104 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target65 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__104.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p104 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__104;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__103 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__103 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "assignees", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj79 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__103.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__103, obj1);
      object obj80 = target65((CallSite) p104, obj79, (object) null);
      if (target64((CallSite) p105, obj80))
      {
        externalGitPullRequest1.Assignees = (ICollection<ExternalGitUser>) new List<ExternalGitUser>();
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__114 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__114 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, IEnumerable> target66 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__114.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, IEnumerable>> p114 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__114;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__113 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__113 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "assignees", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj81 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__113.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__113, obj1);
        foreach (object obj82 in target66((CallSite) p114, obj81))
        {
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__112 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__112 = CallSite<Action<CallSite, ICollection<ExternalGitUser>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Action<CallSite, ICollection<ExternalGitUser>, object> target67 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__112.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Action<CallSite, ICollection<ExternalGitUser>, object>> p112 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__112;
          ICollection<ExternalGitUser> assignees = externalGitPullRequest1.Assignees;
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__111 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__111 = CallSite<Func<CallSite, Type, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "avatarUrl"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login")
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, Type, object, object, object, object, object, object> target68 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__111.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, Type, object, object, object, object, object, object>> p111 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__111;
          Type type3 = typeof (PublishingUtils);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__106 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__106 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj83 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__106.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__106, obj82);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__107 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__107 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj84 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__107.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__107, obj82);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__108 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__108 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj85 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__108.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__108, obj82);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__109 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__109 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj86 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__109.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__109, obj82);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__110 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__110 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj87 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__110.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__110, obj82);
          object obj88 = target68((CallSite) p111, type3, obj83, obj84, obj85, obj86, obj87);
          target67((CallSite) p112, assignees, obj88);
        }
      }
      ExternalGitPullRequest externalGitPullRequest24 = externalGitPullRequest1;
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__117 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__117 = CallSite<Action<CallSite, Dictionary<string, object>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Dictionary<string, object>, string, object> target69 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__117.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Dictionary<string, object>, string, object>> p117 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__117;
      Dictionary<string, object> dictionary2 = dictionary1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__115 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__115 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj89 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__115.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__115, obj1);
      object obj90;
      if (obj89 == null)
      {
        obj90 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__116 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__116 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj90 = PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__116.Target((CallSite) PublishingUtils.\u003C\u003Eo__9.\u003C\u003Ep__116, obj89);
      }
      target69((CallSite) p117, dictionary2, "nodeId", obj90);
      Dictionary<string, object> dictionary3 = dictionary1;
      externalGitPullRequest24.AdditionalProperties = (IDictionary<string, object>) dictionary3;
      return externalGitPullRequest1;
    }

    public static ExternalGitIssue GitHubIssueEventToExternalGitIssue(JObject gitHubIssueEvent)
    {
      object obj1 = (object) gitHubIssueEvent;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, JObject>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (JObject), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, JObject> target1 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, JObject>> p1 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "issue", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__0, obj1);
      ExternalGitIssue externalGitIssue1 = PublishingUtils.GitHubIssueToExternalGitIssue(target1((CallSite) p1, obj2) ?? throw new InvalidPayloadException(ServiceHooksPublisherResources.Error_InvalidPullRequestPayload));
      ExternalGitIssue externalGitIssue2 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target2 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__15.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p15 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__15;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__14 = CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[7]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "avatarUrl"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "type")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object, object> target3 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>> p14 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__14;
      Type type1 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target4 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p3 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__2.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__2, obj1);
      object obj4 = target4((CallSite) p3, obj3);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target5 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p5 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__4.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__4, obj1);
      object obj6 = target5((CallSite) p5, obj5);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p7 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__6.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__6, obj1);
      object obj8 = target6((CallSite) p7, obj7);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p9 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj9 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__8.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__8, obj1);
      object obj10 = target7((CallSite) p9, obj9);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p11 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__10.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__10, obj1);
      object obj12 = target8((CallSite) p11, obj11);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "type", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target9 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p13 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__12.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__12, obj1);
      object obj14 = target9((CallSite) p13, obj13);
      object obj15 = target3((CallSite) p14, type1, obj4, obj6, obj8, obj10, obj12, obj14);
      ExternalGitUser externalGitUser = target2((CallSite) p15, obj15);
      externalGitIssue2.Sender = externalGitUser;
      ExternalGitIssue externalGitIssue3 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, ExternalGitRepo>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitRepo), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitRepo> target10 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__29.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitRepo>> p29 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__29;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__28 = CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitRepo", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[7]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object, object> target11 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__28.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object, object>> p28 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__28;
      Type type2 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target12 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p17 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__16.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__16, obj1);
      object obj17 = target12((CallSite) p17, obj16);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p19 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj18 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__18.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__18, obj1);
      object obj19 = target13((CallSite) p19, obj18);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p21 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__21;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj20 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__20.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__20, obj1);
      object obj21 = target14((CallSite) p21, obj20);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "clone_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target15 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__23.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p23 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__23;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj22 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__22.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__22, obj1);
      object obj23 = target15((CallSite) p23, obj22);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target16 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__25.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p25 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__25;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj24 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__24.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__24, obj1);
      object obj25 = target16((CallSite) p25, obj24);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "default_branch", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target17 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__27.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p27 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__27;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj26 = PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__26.Target((CallSite) PublishingUtils.\u003C\u003Eo__10.\u003C\u003Ep__26, obj1);
      object obj27 = target17((CallSite) p27, obj26);
      object obj28 = target11((CallSite) p28, type2, obj17, obj19, obj21, obj23, obj25, obj27);
      ExternalGitRepo externalGitRepo = target10((CallSite) p29, obj28);
      externalGitIssue3.Repo = externalGitRepo;
      return externalGitIssue1;
    }

    public static ExternalGitIssue GitHubIssueToExternalGitIssue(JObject gitHubIssue)
    {
      object obj1 = (object) gitHubIssue;
      ExternalGitIssue externalGitIssue1 = new ExternalGitIssue();
      ExternalGitIssue externalGitIssue2 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p1 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__0, obj1);
      string str1 = target1((CallSite) p1, obj2);
      externalGitIssue2.Id = str1;
      ExternalGitIssue externalGitIssue3 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target2 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p3 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__2.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__2, obj1);
      string str2 = target2((CallSite) p3, obj3);
      externalGitIssue3.Number = str2;
      ExternalGitIssue externalGitIssue4 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p5 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "title", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__4.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__4, obj1);
      string str3 = target3((CallSite) p5, obj4);
      externalGitIssue4.Title = str3;
      ExternalGitIssue externalGitIssue5 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target4 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p7 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj5 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__6.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__6, obj1);
      string str4 = target4((CallSite) p7, obj5);
      externalGitIssue5.Url = str4;
      ExternalGitIssue externalGitIssue6 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p9 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__8.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__8, obj1);
      string str5 = target5((CallSite) p9, obj6);
      externalGitIssue6.WebUrl = str5;
      ExternalGitIssue externalGitIssue7 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target6 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p11 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "body", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__10.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__10, obj1);
      string str6 = target6((CallSite) p11, obj7);
      externalGitIssue7.Description = str6;
      ExternalGitIssue externalGitIssue8 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target7 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p13 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "state", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__12.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__12, obj1);
      string str7 = target7((CallSite) p13, obj8);
      externalGitIssue8.State = str7;
      ExternalGitIssue externalGitIssue9 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target8 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__15.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p15 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__15;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "created_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj9 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__14.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__14, obj1);
      string str8 = target8((CallSite) p15, obj9);
      externalGitIssue9.CreatedAt = str8;
      ExternalGitIssue externalGitIssue10 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target9 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p17 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "updated_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__16.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__16, obj1);
      string str9 = target9((CallSite) p17, obj10);
      externalGitIssue10.UpdatedAt = str9;
      ExternalGitIssue externalGitIssue11 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target10 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p19 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "closed_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj11 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__18.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__18, obj1);
      string str10 = target10((CallSite) p19, obj11);
      externalGitIssue11.ClosedAt = str10;
      ExternalGitIssue externalGitIssue12 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target11 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__27.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p27 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__27;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__26 = CallSite<Func<CallSite, Type, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object> target12 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__26.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object>> p26 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__26;
      Type type1 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p21 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__21;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__20.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__20, obj1);
      object obj13 = target13((CallSite) p21, obj12);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__23.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p23 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__23;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj14 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__22.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__22, obj1);
      object obj15 = target14((CallSite) p23, obj14);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target15 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__25.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p25 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__25;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__24.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__24, obj1);
      object obj17 = target15((CallSite) p25, obj16);
      object obj18 = target12((CallSite) p26, type1, obj13, obj15, obj17);
      ExternalGitUser externalGitUser = target11((CallSite) p27, obj18);
      externalGitIssue12.Sender = externalGitUser;
      ExternalGitIssue externalGitIssue13 = externalGitIssue1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__30 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target16 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__30.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p30 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__30;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Root", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj19 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__28.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__28, obj1);
      object obj20;
      if (obj19 == null)
      {
        obj20 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__29 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj20 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__29.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__29, obj19);
      }
      string str11 = target16((CallSite) p30, obj20);
      externalGitIssue13.Action = str11;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__33 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target17 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__33.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p33 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__33;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__32 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object, object> target18 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__32.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object, object>> p32 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__32;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "assignees", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj21 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__31.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__31, obj1);
      object obj22 = target18((CallSite) p32, obj21, (object) null);
      if (target17((CallSite) p33, obj22))
      {
        externalGitIssue1.Assignees = (ICollection<ExternalGitUser>) new List<ExternalGitUser>();
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__42 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (PublishingUtils)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, IEnumerable> target19 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__42.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, IEnumerable>> p42 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__42;
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__41 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "assignees", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj23 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__41.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__41, obj1);
        foreach (object obj24 in target19((CallSite) p42, obj23))
        {
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__40 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__40 = CallSite<Action<CallSite, ICollection<ExternalGitUser>, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Action<CallSite, ICollection<ExternalGitUser>, object> target20 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__40.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Action<CallSite, ICollection<ExternalGitUser>, object>> p40 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__40;
          ICollection<ExternalGitUser> assignees = externalGitIssue1.Assignees;
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__39 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__39 = CallSite<Func<CallSite, Type, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "avatarUrl"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login")
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, Type, object, object, object, object, object, object> target21 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__39.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, Type, object, object, object, object, object, object>> p39 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__39;
          Type type2 = typeof (PublishingUtils);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__34 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj25 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__34.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__34, obj24);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__35 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj26 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__35.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__35, obj24);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__36 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "avatar_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj27 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__36.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__36, obj24);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__37 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj28 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__37.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__37, obj24);
          // ISSUE: reference to a compiler-generated field
          if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__38 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj29 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__38.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__38, obj24);
          object obj30 = target21((CallSite) p39, type2, obj25, obj26, obj27, obj28, obj29);
          target20((CallSite) p40, assignees, obj30);
        }
      }
      ExternalGitIssue externalGitIssue14 = externalGitIssue1;
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__45 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__45 = CallSite<Action<CallSite, Dictionary<string, object>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Action<CallSite, Dictionary<string, object>, string, object> target22 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__45.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Action<CallSite, Dictionary<string, object>, string, object>> p45 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__45;
      Dictionary<string, object> dictionary2 = dictionary1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__43 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__43 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj31 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__43.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__43, obj1);
      object obj32;
      if (obj31 == null)
      {
        obj32 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__44 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj32 = PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__44.Target((CallSite) PublishingUtils.\u003C\u003Eo__11.\u003C\u003Ep__44, obj31);
      }
      target22((CallSite) p45, dictionary2, "nodeId", obj32);
      Dictionary<string, object> dictionary3 = dictionary1;
      externalGitIssue14.AdditionalProperties = (IDictionary<string, object>) dictionary3;
      return externalGitIssue1;
    }

    public static ExternalCommentEventCommand ParseGitHubCommentAndValidate(
      JObject gitHubIssueComment,
      IReadOnlyList<string> supportedCommands)
    {
      ArgumentUtility.CheckForNull<JObject>(gitHubIssueComment, nameof (gitHubIssueComment));
      ArgumentUtility.CheckForNull<IReadOnlyList<string>>(supportedCommands, nameof (supportedCommands));
      object obj1 = (object) gitHubIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, JObject>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (JObject), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, JObject> target1 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, JObject>> p1 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__0, obj1);
      JObject jobject = target1((CallSite) p1, obj2);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target2 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p3 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj3 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__2.Target((CallSite) PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__2, obj1);
      string str1 = target2((CallSite) p3, obj3);
      if (jobject == null || str1 == null)
        throw new InvalidPayloadException(ServiceHooksPublisherResources.Error_InvalidPullRequestPayload);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__6.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p6 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__6;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "state", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target4 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p5 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "issue", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__4.Target((CallSite) PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__4, obj1);
      object obj5 = target4((CallSite) p5, obj4);
      if (!string.Equals(target3((CallSite) p6, obj5), "open", StringComparison.OrdinalIgnoreCase) || str1.Equals("deleted", StringComparison.OrdinalIgnoreCase))
        return (ExternalCommentEventCommand) null;
      object obj6 = (object) jobject;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__8.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p8 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__8;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "body", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj7 = PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__7.Target((CallSite) PublishingUtils.\u003C\u003Eo__12.\u003C\u003Ep__7, obj6);
      string str2 = target5((CallSite) p8, obj7);
      if (str2 != null)
      {
        string[] strArray = str2.Trim().Split(new char[1]
        {
          ' '
        }, 3, StringSplitOptions.RemoveEmptyEntries);
        string testString1 = strArray.Length != 0 ? strArray[0] : string.Empty;
        string testString2 = strArray.Length > 1 ? strArray[1] : string.Empty;
        string str3 = strArray.Length > 2 ? strArray[2] : string.Empty;
        if (!string.IsNullOrEmpty(testString1) && PublishingUtils.IsStringPresentInList(testString1, PublishingUtils.s_supportedKeywords))
        {
          if (!string.IsNullOrEmpty(testString2) && PublishingUtils.IsStringPresentInList(testString2, supportedCommands))
            return new ExternalCommentEventCommand()
            {
              CommandKeyword = testString2,
              RemainingParameters = str3
            };
          if (PublishingUtils.IsStringPresentInList("help", supportedCommands))
            return new ExternalCommentEventCommand()
            {
              CommandKeyword = "help",
              RemainingParameters = testString2
            };
        }
      }
      return (ExternalCommentEventCommand) null;
    }

    public static ExternalPullRequestCommentEvent GitHubIssueCommentEventToExternalIssueComment(
      JObject gitHubIssueComment,
      ExternalCommentEventCommand command,
      string pipelineEventId = null,
      bool trimCommentBody = true)
    {
      object obj1 = (object) gitHubIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p2 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "body", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target2 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p1 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__0, obj1);
      object obj3 = target2((CallSite) p1, obj2);
      string str1 = target1((CallSite) p2, obj3);
      if (trimCommentBody)
        str1 = str1.Trim();
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, ExternalGitRepo>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitRepo), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitRepo> target3 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitRepo>> p14 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__14;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__13 = CallSite<Func<CallSite, Type, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitRepo", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object> target4 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object>> p13 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__13;
      Type type1 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target5 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p4 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__3.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__3, obj1);
      object obj5 = target5((CallSite) p4, obj4);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__6.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p6 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__6;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__5.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__5, obj1);
      object obj7 = target6((CallSite) p6, obj6);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__8.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p8 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__8;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__7.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__7, obj1);
      object obj9 = target7((CallSite) p8, obj8);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p10 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__10;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__9.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__9, obj1);
      object obj11 = target8((CallSite) p10, obj10);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target9 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__12.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p12 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__12;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj12 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__11.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__11, obj1);
      object obj13 = target9((CallSite) p12, obj12);
      object obj14 = target4((CallSite) p13, type1, obj5, obj7, obj9, obj11, obj13);
      ExternalGitRepo externalGitRepo = target3((CallSite) p14, obj14);
      ExternalPullRequestCommentEvent externalIssueComment = new ExternalPullRequestCommentEvent();
      externalIssueComment.CommentBody = str1;
      ExternalPullRequestCommentEvent requestCommentEvent1 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target10 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p17 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "number", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target11 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__16.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p16 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__16;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "issue", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj15 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__15.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__15, obj1);
      object obj16 = target11((CallSite) p16, obj15);
      string str2 = target10((CallSite) p17, obj16);
      requestCommentEvent1.PullRequestNumber = str2;
      ExternalPullRequestCommentEvent requestCommentEvent2 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target12 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p21 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__21;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__20.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p20 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__20;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "owner", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p19 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj17 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__18.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__18, obj1);
      object obj18 = target14((CallSite) p19, obj17);
      object obj19 = target13((CallSite) p20, obj18);
      string str3 = target12((CallSite) p21, obj19);
      requestCommentEvent2.RepositoryOwner = str3;
      ExternalPullRequestCommentEvent requestCommentEvent3 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target15 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__24.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p24 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__24;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target16 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__23.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p23 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__23;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj20 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__22.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__22, obj1);
      object obj21 = target16((CallSite) p23, obj20);
      string str4 = target15((CallSite) p24, obj21);
      requestCommentEvent3.Id = str4;
      ExternalPullRequestCommentEvent requestCommentEvent4 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target17 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__27.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p27 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__27;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "author_association", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target18 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__26.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p26 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__26;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj22 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__25.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__25, obj1);
      object obj23 = target18((CallSite) p26, obj22);
      string str5 = target17((CallSite) p27, obj23);
      requestCommentEvent4.AuthorAssociation = str5;
      ExternalPullRequestCommentEvent requestCommentEvent5 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__44 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target19 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__44.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p44 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__44;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__43 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__43 = CallSite<Func<CallSite, Type, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nodeId"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "name"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "login")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object> target20 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__43.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object>> p43 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__43;
      Type type2 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__30 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target21 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__30.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p30 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__30;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target22 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__29.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p29 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__29;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj24 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__28.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__28, obj1);
      object obj25 = target22((CallSite) p29, obj24);
      object obj26 = target21((CallSite) p30, obj25);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__32 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target23 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__32.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p32 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__32;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj27 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__31.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__31, obj1);
      object obj28 = target23((CallSite) p32, obj27);
      object obj29;
      if (obj28 == null)
      {
        obj29 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__33 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj29 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__33.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__33, obj28);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__35 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target24 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__35.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p35 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__35;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__34 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj30 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__34.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__34, obj1);
      object obj31 = target24((CallSite) p35, obj30);
      object obj32;
      if (obj31 == null)
      {
        obj32 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__36 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj32 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__36.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__36, obj31);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__39 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target25 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__39.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p39 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__39;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__38 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target26 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__38.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p38 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__38;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__37 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj33 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__37.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__37, obj1);
      object obj34 = target26((CallSite) p38, obj33);
      object obj35 = target25((CallSite) p39, obj34);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__42 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target27 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__42.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p42 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__42;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__41 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target28 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__41.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p41 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__41;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__40 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__40 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj36 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__40.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__40, obj1);
      object obj37 = target28((CallSite) p41, obj36);
      object obj38 = target27((CallSite) p42, obj37);
      object obj39 = target20((CallSite) p43, type2, obj26, obj29, obj32, obj35, obj38);
      ExternalGitUser externalGitUser1 = target19((CallSite) p44, obj39);
      requestCommentEvent5.CommentedBy = externalGitUser1;
      ExternalPullRequestCommentEvent requestCommentEvent6 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__47 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__47 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target29 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__47.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p47 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__47;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__46 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__46 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "updated_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target30 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__46.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p46 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__46;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__45 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__45 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj40 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__45.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__45, obj1);
      object obj41 = target30((CallSite) p46, obj40);
      string str6 = target29((CallSite) p47, obj41);
      requestCommentEvent6.UpdatedAt = str6;
      externalIssueComment.Command = command;
      externalIssueComment.Repo = externalGitRepo;
      ExternalPullRequestCommentEvent requestCommentEvent7 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__49 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__49 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target31 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__49.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p49 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__49;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__48 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__48 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (pipelineEventId), typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj42 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__48.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__48, obj1) ?? (object) pipelineEventId;
      string str7 = target31((CallSite) p49, obj42);
      requestCommentEvent7.PipelineEventId = str7;
      ExternalPullRequestCommentEvent requestCommentEvent8 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__57 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__57 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target32 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__57.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p57 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__57;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__56 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__56 = CallSite<Func<CallSite, Type, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "nameOrLogin"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "email"),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "type")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object> target33 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__56.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object>> p56 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__56;
      Type type3 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__50 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__50 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj43 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__50.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__50, obj1);
      object obj44;
      if (obj43 == null)
      {
        obj44 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__51 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__51 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj44 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__51.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__51, obj43);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__52 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__52 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj45 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__52.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__52, obj1);
      object obj46;
      if (obj45 == null)
      {
        obj46 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__53 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__53 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj46 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__53.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__53, obj45);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__54 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__54 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj47 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__54.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__54, obj1);
      object obj48;
      if (obj47 == null)
      {
        obj48 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__55 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__55 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "type", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj48 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__55.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__55, obj47);
      }
      object obj49 = target33((CallSite) p56, type3, obj44, obj46, obj48);
      ExternalGitUser externalGitUser2 = target32((CallSite) p57, obj49);
      requestCommentEvent8.Sender = externalGitUser2;
      ExternalPullRequestCommentEvent requestCommentEvent9 = externalIssueComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__59 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__59 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target34 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__59.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p59 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__59;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__58 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__58 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj50 = PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__58.Target((CallSite) PublishingUtils.\u003C\u003Eo__13.\u003C\u003Ep__58, obj1);
      string str8 = target34((CallSite) p59, obj50);
      requestCommentEvent9.Action = str8;
      return externalIssueComment;
    }

    public static ExternalGitCommitComment GitHubCommitCommentEventToExternalCommitComment(
      JObject gitHubCommitComment)
    {
      object obj1 = (object) gitHubCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, ExternalGitRepo>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitRepo), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitRepo> target1 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitRepo>> p11 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__10 = CallSite<Func<CallSite, Type, object, object, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitRepo", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[6]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object, object, object> target2 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object, object, object>> p10 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__10;
      Type type1 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target3 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p1 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__0.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__0, obj1);
      object obj3 = target3((CallSite) p1, obj2);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "full_name", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target4 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p3 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__2.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__2, obj1);
      object obj5 = target4((CallSite) p3, obj4);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "node_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target5 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p5 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__4.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__4, obj1);
      object obj7 = target5((CallSite) p5, obj6);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p7 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__6.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__6, obj1);
      object obj9 = target6((CallSite) p7, obj8);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "html_url", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target7 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__9.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p9 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__9;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__8.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__8, obj1);
      object obj11 = target7((CallSite) p9, obj10);
      object obj12 = target2((CallSite) p10, type1, obj3, obj5, obj7, obj9, obj11);
      ExternalGitRepo externalGitRepo = target1((CallSite) p11, obj12);
      ExternalGitCommitComment externalCommitComment = new ExternalGitCommitComment();
      ExternalGitCommitComment gitCommitComment1 = externalCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target8 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p14 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__14;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "body", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target9 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p13 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__12.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__12, obj1);
      object obj14 = target9((CallSite) p13, obj13);
      string str1 = target8((CallSite) p14, obj14);
      gitCommitComment1.CommentBody = str1;
      ExternalGitCommitComment gitCommitComment2 = externalCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target10 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p17 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target11 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__16.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p16 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__16;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj15 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__15.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__15, obj1);
      object obj16 = target11((CallSite) p16, obj15);
      string str2 = target10((CallSite) p17, obj16);
      gitCommitComment2.Id = str2;
      ExternalGitCommitComment gitCommitComment3 = externalCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__25 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target12 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__25.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p25 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__25;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__24 = CallSite<Func<CallSite, Type, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object> target13 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__24.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object>> p24 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__24;
      Type type2 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__20.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p20 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__20;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target15 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p19 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj17 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__18.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__18, obj1);
      object obj18 = target15((CallSite) p19, obj17);
      object obj19 = target14((CallSite) p20, obj18);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "user", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target16 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__22.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p22 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__22;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj20 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__21.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__21, obj1);
      object obj21 = target16((CallSite) p22, obj20);
      object obj22;
      if (obj21 == null)
      {
        obj22 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__23 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj22 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__23.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__23, obj21);
      }
      object obj23 = target13((CallSite) p24, type2, obj19, obj22);
      ExternalGitUser externalGitUser1 = target12((CallSite) p25, obj23);
      gitCommitComment3.CommentedBy = externalGitUser1;
      ExternalGitCommitComment gitCommitComment4 = externalCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__28 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target17 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__28.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p28 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__28;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__27 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "updated_at", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target18 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__27.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p27 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__27;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__26 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj24 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__26.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__26, obj1);
      object obj25 = target18((CallSite) p27, obj24);
      string str3 = target17((CallSite) p28, obj25);
      gitCommitComment4.UpdatedAt = str3;
      ExternalGitCommitComment gitCommitComment5 = externalCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__31 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target19 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__31.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p31 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__31;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__30 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "commit_id", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target20 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__30.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p30 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__30;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__29 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "comment", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj26 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__29.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__29, obj1);
      object obj27 = target20((CallSite) p30, obj26);
      string str4 = target19((CallSite) p31, obj27);
      gitCommitComment5.CommitId = str4;
      externalCommitComment.Repo = externalGitRepo;
      ExternalGitCommitComment gitCommitComment6 = externalCommitComment;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__39 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, ExternalGitUser>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (ExternalGitUser), typeof (PublishingUtils)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, ExternalGitUser> target21 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__39.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, ExternalGitUser>> p39 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__39;
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__38 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__38 = CallSite<Func<CallSite, Type, object, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "CreateExternalGitUser", (IEnumerable<Type>) null, typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.NamedArgument, "type")
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, Type, object, object, object, object> target22 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__38.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, Type, object, object, object, object>> p38 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__38;
      Type type3 = typeof (PublishingUtils);
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__32 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj28 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__32.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__32, obj1);
      object obj29;
      if (obj28 == null)
      {
        obj29 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__33 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj29 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__33.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__33, obj28);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__34 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj30 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__34.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__34, obj1);
      object obj31;
      if (obj30 == null)
      {
        obj31 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__35 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "email", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj31 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__35.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__35, obj30);
      }
      // ISSUE: reference to a compiler-generated field
      if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__36 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "sender", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj32 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__36.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__36, obj1);
      object obj33;
      if (obj32 == null)
      {
        obj33 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__37 == null)
        {
          // ISSUE: reference to a compiler-generated field
          PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "type", typeof (PublishingUtils), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj33 = PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__37.Target((CallSite) PublishingUtils.\u003C\u003Eo__14.\u003C\u003Ep__37, obj32);
      }
      object obj34 = target22((CallSite) p38, type3, obj29, obj31, obj33);
      ExternalGitUser externalGitUser2 = target21((CallSite) p39, obj34);
      gitCommitComment6.Sender = externalGitUser2;
      return externalCommitComment;
    }

    public static ExternalGitRepo CreateExternalGitRepo(
      object name,
      object id,
      object nodeId,
      object url,
      object webUrl,
      object defaultBranch = null)
    {
      return new ExternalGitRepo()
      {
        Name = name?.ToString(),
        Id = id?.ToString(),
        Url = url?.ToString(),
        WebUrl = webUrl?.ToString(),
        DefaultBranch = defaultBranch?.ToString(),
        AdditionalProperties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nodeId),
            (object) nodeId?.ToString()
          },
          {
            "repoNameWithOwner",
            (object) id?.ToString()
          }
        }
      };
    }

    public static ExternalGitPullRequest GitHubPullRequestToExternalGitPullRequest(
      GitHubData.V4.PullRequest pullRequest)
    {
      ArgumentUtility.CheckForNull<GitHubData.V4.PullRequest>(pullRequest, nameof (pullRequest));
      ExternalGitPullRequest externalGitPullRequest1 = new ExternalGitPullRequest();
      externalGitPullRequest1.Id = pullRequest.DatabaseId.ToString();
      externalGitPullRequest1.Number = pullRequest.Number;
      externalGitPullRequest1.Title = pullRequest.Title;
      externalGitPullRequest1.Description = pullRequest.Body;
      externalGitPullRequest1.State = pullRequest.State.ToString();
      ExternalGitPullRequest externalGitPullRequest2 = externalGitPullRequest1;
      DateTime dateTime = pullRequest.UpdatedAt;
      dateTime = dateTime.ToUniversalTime();
      string str = dateTime.ToString("o", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      externalGitPullRequest2.UpdatedAt = str;
      if (pullRequest.HeadRef?.Target is GitHubData.V4.Commit target)
        externalGitPullRequest1.LastHeadRefCommit = PublishingUtils.GitHubCommitToExternalGitCommit(target);
      externalGitPullRequest1.Url = new GitHubApiRoot().PullRequestUri(pullRequest.Repository?.NameWithOwner, pullRequest.Number).AbsoluteUri();
      externalGitPullRequest1.WebUrl = pullRequest.Url?.AbsoluteUri;
      externalGitPullRequest1.TargetRef = pullRequest.BaseRefName;
      externalGitPullRequest1.TargetSha = pullRequest.BaseRefOid;
      externalGitPullRequest1.SourceRef = pullRequest.HeadRefName;
      externalGitPullRequest1.SourceSha = pullRequest.HeadRefOid;
      externalGitPullRequest1.MergeRef = "refs/pull/" + pullRequest.Number + "/merge";
      ExternalGitPullRequest externalGitPullRequest3 = externalGitPullRequest1;
      DateTime? mergedAt = pullRequest.MergedAt;
      string empty;
      if (!mergedAt.HasValue)
      {
        empty = string.Empty;
      }
      else
      {
        mergedAt = pullRequest.MergedAt;
        DateTime universalTime = mergedAt.Value;
        universalTime = universalTime.ToUniversalTime();
        empty = universalTime.ToString("o", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      }
      externalGitPullRequest3.MergedAt = empty;
      externalGitPullRequest1.Draft = false;
      if (pullRequest.Draft)
        externalGitPullRequest1.Draft = pullRequest.Draft;
      if (pullRequest.MergeState == GitHubData.V4.MergeableState.Mergeable)
      {
        externalGitPullRequest1.IsMergeable = new bool?(true);
        externalGitPullRequest1.MergeCommitSha = pullRequest.PotentialMergeCommit?.Oid;
      }
      else
        externalGitPullRequest1.IsMergeable = pullRequest.MergeState != GitHubData.V4.MergeableState.Unknown ? new bool?(false) : new bool?();
      externalGitPullRequest1.IsFork = false;
      if (pullRequest.HeadRepository != null && pullRequest.Repository != null)
        externalGitPullRequest1.IsFork = pullRequest.HeadRepository.Id != pullRequest.Repository.Id;
      if (pullRequest.Repository != null)
        externalGitPullRequest1.Repo = PublishingUtils.CreateExternalGitRepo((object) pullRequest.Repository.Name, (object) pullRequest.Repository.NameWithOwner, (object) pullRequest.Repository.Id, (object) pullRequest.Repository.Url, (object) pullRequest.Repository.Url);
      if (pullRequest.Author != null)
        externalGitPullRequest1.Sender = PublishingUtils.CreateExternalGitUser(pullRequest.Author);
      ExternalGitPullRequest externalGitPullRequest4 = externalGitPullRequest1;
      GitHubData.V4.Connection<GitHubData.V4.PullRequestCommit> commits = pullRequest.Commits;
      List<ExternalGitCommit> list = commits != null ? ((IEnumerable<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>>) commits.Edges).Where<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>>((Func<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>, bool>) (x => x.Node?.Commit != null)).Select<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>, ExternalGitCommit>((Func<GitHubData.V4.Edge<GitHubData.V4.PullRequestCommit>, ExternalGitCommit>) (x => PublishingUtils.GitHubCommitToExternalGitCommit(x.Node.Commit))).Reverse<ExternalGitCommit>().ToList<ExternalGitCommit>() : (List<ExternalGitCommit>) null;
      externalGitPullRequest4.Commits = (IEnumerable<ExternalGitCommit>) list;
      return externalGitPullRequest1;
    }

    public static ExternalGitCommit GitHubCommitToExternalGitCommit(GitHubData.V4.Commit commit)
    {
      ArgumentUtility.CheckForNull<GitHubData.V4.Commit>(commit, nameof (commit));
      return new ExternalGitCommit()
      {
        Sha = commit.Oid,
        Message = commit.Message,
        WebUrl = commit.CommitUrl?.AbsoluteUri,
        CommitedDate = commit.CommittedDate,
        PushedDate = commit.PushedDate,
        Author = PublishingUtils.CreateExternalGitUser(commit.Author)
      };
    }

    private static bool IsStringPresentInList(string testString, IReadOnlyList<string> validStrings)
    {
      foreach (string validString in (IEnumerable<string>) validStrings)
      {
        if (testString.Equals(validString, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static ExternalGitUser CreateExternalGitUser(GitHubData.V4.GitActor actor) => actor == null ? (ExternalGitUser) null : PublishingUtils.CreateExternalGitUser((object) actor.Name, (object) actor.Email, (object) actor.AvatarUrl?.ToString(), (object) actor.User?.Id, (object) actor.User?.Name, (object) actor.User?.Login);

    private static ExternalGitUser CreateExternalGitUser(GitHubData.V4.IActor actor) => actor == null ? (ExternalGitUser) null : PublishingUtils.CreateExternalGitUser((object) actor.Login, (object) null, (object) actor.AvatarUrl?.ToString(), (object) PublishingUtils.GetActorId(actor), (object) PublishingUtils.GetAuthorName(actor), (object) actor.Login);

    private static ExternalGitUser CreateExternalGitUser(
      object nameOrLogin,
      object email,
      object avatarUrl = null,
      object nodeId = null,
      object name = null,
      object login = null,
      object type = null)
    {
      ExternalGitUser externalGitUser = new ExternalGitUser()
      {
        Name = nameOrLogin?.ToString(),
        Email = email?.ToString(),
        AvatarUrl = avatarUrl?.ToString(),
        Type = type?.ToString()
      };
      if (nodeId != null || name != null || login != null)
        externalGitUser.AdditionalProperties = (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            nameof (nodeId),
            (object) nodeId?.ToString()
          },
          {
            nameof (name),
            (object) name?.ToString()
          },
          {
            nameof (login),
            (object) login?.ToString()
          }
        };
      return externalGitUser;
    }

    private static string GetAuthorName(GitHubData.V4.IActor actor)
    {
      switch (actor)
      {
        case GitHubData.V4.User _:
          return ((GitHubData.V4.User) actor).Name;
        case GitHubData.V4.Organization _:
          return ((GitHubData.V4.Organization) actor).Name;
        default:
          return (string) null;
      }
    }

    private static string GetActorId(GitHubData.V4.IActor actor)
    {
      switch (actor)
      {
        case GitHubData.V4.User _:
          return ((GitHubData.V4.User) actor).Id;
        case GitHubData.V4.Organization _:
          return ((GitHubData.V4.Organization) actor).Id;
        case GitHubData.V4.Bot _:
          return ((GitHubData.V4.Bot) actor).Id;
        default:
          return (string) null;
      }
    }

    public enum HashingMethod
    {
      SHA1,
      SHA256,
    }
  }
}
