// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubInstallationEventHandler
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExternalEvent;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  internal class GitHubInstallationEventHandler : IPipelineEventHandler
  {
    private const string c_layer = "GitHubInstallationEventHandler";

    public bool HandleEvent(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      string headerValue = headers.GetHeaderValue("X-GitHub-Event");
      JObject eventPayload = JObject.Parse(jsonPayload);
      ProviderInstallationEvent installationEvent;
      if (this.IsAppInstallationEvent(requestContext, headerValue, eventPayload, out installationEvent))
      {
        this.HandleAppInstallationEvent(requestContext, installationEvent, provider);
        this.PublishAnalyticsEvent(requestContext, (object) installationEvent);
        return true;
      }
      ProviderInstallationRepositoriesEvent installationRepositoriesEvent;
      if (this.IsInstallationRepositoriesEvent(requestContext, headerValue, eventPayload, out installationRepositoriesEvent))
      {
        this.HandleInstallationRepositoriesEvent(requestContext, installationRepositoriesEvent);
        this.PublishAnalyticsEvent(requestContext, (object) installationRepositoriesEvent);
        return true;
      }
      ProviderMarketplacePurchaseEvent marketplacePurchaseEvent;
      if (!this.IsMarketplacePurchaseEvent(requestContext, headerValue, eventPayload, out marketplacePurchaseEvent))
        return false;
      this.HandleMarketplacePurchaseEvent(requestContext, marketplacePurchaseEvent);
      this.PublishAnalyticsEvent(requestContext, (object) marketplacePurchaseEvent);
      return true;
    }

    private void PublishAnalyticsEvent(IVssRequestContext requestContext, object eventObject)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, eventObject);
    }

    private bool IsInstallationRepositoriesEvent(
      IVssRequestContext requestContext,
      string eventType,
      JObject eventPayload,
      out ProviderInstallationRepositoriesEvent installationRepositoriesEvent)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubInstallationEventHandler), "IsInstallationRepositoriesEvent - github");
      ArgumentUtility.CheckForNull<string>(eventType, nameof (eventType));
      ArgumentUtility.CheckForNull<JObject>(eventPayload, nameof (eventPayload));
      bool flag = string.Equals(eventType, "installation_repositories", StringComparison.OrdinalIgnoreCase);
      if (flag)
      {
        object obj1 = (object) eventPayload;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__1 = CallSite<Func<CallSite, GitHubInstallationEventHandler, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ToExternalGitRepos", (IEnumerable<Type>) null, typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, GitHubInstallationEventHandler, object, object> target1 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, GitHubInstallationEventHandler, object, object>> p1 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repositories_added", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__0, obj1);
        object obj3 = target1((CallSite) p1, this, obj2);
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__3 = CallSite<Func<CallSite, GitHubInstallationEventHandler, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ToExternalGitRepos", (IEnumerable<Type>) null, typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, GitHubInstallationEventHandler, object, object> target2 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, GitHubInstallationEventHandler, object, object>> p3 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__3;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repositories_removed", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj4 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__2.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__2, obj1);
        object obj5 = target2((CallSite) p3, this, obj4);
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target3 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p6 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__6;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "target_type", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target4 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p5 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__5;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj6 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__4.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__4, obj1);
        object obj7 = target4((CallSite) p5, obj6);
        string a1 = target3((CallSite) p6, obj7);
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target5 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__9.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p9 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__9;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository_selection", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target6 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__8.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p8 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__8;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj8 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__7.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__7, obj1);
        object obj9 = target6((CallSite) p8, obj8);
        string a2 = target5((CallSite) p9, obj9);
        ExternalAppInstallationInfo installationInfo1 = new ExternalAppInstallationInfo();
        ExternalAppInstallationInfo installationInfo2 = installationInfo1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target7 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__12.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p12 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__12;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "app_id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target8 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__11.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p11 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__11;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj10 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__10.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__10, obj1);
        object obj11 = target8((CallSite) p11, obj10);
        string str1 = target7((CallSite) p12, obj11);
        installationInfo2.AppId = str1;
        ExternalAppInstallationInfo installationInfo3 = installationInfo1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target9 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__15.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p15 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__15;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target10 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__14.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p14 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__14;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj12 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__13.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__13, obj1);
        object obj13 = target10((CallSite) p14, obj12);
        string str2 = target9((CallSite) p15, obj13);
        installationInfo3.Id = str2;
        ExternalAppInstallationInfo installationInfo4 = installationInfo1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target11 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__19.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p19 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__19;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target12 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__18.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p18 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__18;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target13 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__17.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p17 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__17;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj14 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__16.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__16, obj1);
        object obj15 = target13((CallSite) p17, obj14);
        object obj16 = target12((CallSite) p18, obj15);
        string str3 = target11((CallSite) p19, obj16);
        installationInfo4.OrgId = str3;
        installationInfo1.IsOrgAUser = string.Equals(a1, "User", StringComparison.OrdinalIgnoreCase);
        ExternalAppInstallationInfo installationInfo5 = installationInfo1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__23 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target14 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__23.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p23 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__23;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__22 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target15 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__22.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p22 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__22;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__21 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target16 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__21.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p21 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__21;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__20 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj17 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__20.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__20, obj1);
        object obj18 = target16((CallSite) p21, obj17);
        object obj19 = target15((CallSite) p22, obj18);
        string str4 = target14((CallSite) p23, obj19);
        installationInfo5.OrgName = str4;
        ExternalAppInstallationInfo installationInfo6 = installationInfo1;
        ref ProviderInstallationRepositoriesEvent local = ref installationRepositoriesEvent;
        ProviderInstallationRepositoriesEvent repositoriesEvent1 = new ProviderInstallationRepositoriesEvent();
        ProviderInstallationRepositoriesEvent repositoriesEvent2 = repositoriesEvent1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__25 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target17 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__25.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p25 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__25;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__24 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj20 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__24.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__24, obj1);
        string str5 = target17((CallSite) p25, obj20);
        repositoriesEvent2.Action = str5;
        repositoriesEvent1.AppName = "azure-pipelines";
        repositoriesEvent1.IsAllRepoSelected = string.Equals(a2, "all", StringComparison.OrdinalIgnoreCase);
        repositoriesEvent1.Installation = installationInfo6;
        ProviderInstallationRepositoriesEvent repositoriesEvent3 = repositoriesEvent1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__26 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, IEnumerable<ExternalGitRepo>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable<ExternalGitRepo>), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<ExternalGitRepo> externalGitRepos1 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__26.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__26, obj3);
        repositoriesEvent3.RepositoriesAdded = externalGitRepos1;
        ProviderInstallationRepositoriesEvent repositoriesEvent4 = repositoriesEvent1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__27 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, IEnumerable<ExternalGitRepo>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable<ExternalGitRepo>), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IEnumerable<ExternalGitRepo> externalGitRepos2 = GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__27.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__3.\u003C\u003Ep__27, obj5);
        repositoriesEvent4.RepositoriesRemoved = externalGitRepos2;
        ProviderInstallationRepositoriesEvent repositoriesEvent5 = repositoriesEvent1;
        local = repositoriesEvent5;
      }
      else
        installationRepositoriesEvent = (ProviderInstallationRepositoriesEvent) null;
      return flag;
    }

    private void HandleInstallationRepositoriesEvent(
      IVssRequestContext requestContext,
      ProviderInstallationRepositoriesEvent installationRepositoriesEvent)
    {
      GitHubInstallationEventHandler.PublishEventCI(requestContext, installationRepositoriesEvent);
    }

    private bool IsAppInstallationEvent(
      IVssRequestContext requestContext,
      string eventType,
      JObject eventPayload,
      out ProviderInstallationEvent installationEvent)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubInstallationEventHandler), "IsAppInstallationEvent - github");
      ArgumentUtility.CheckForNull<string>(eventType, nameof (eventType));
      ArgumentUtility.CheckForNull<JObject>(eventPayload, nameof (eventPayload));
      bool flag = string.Equals(eventType, "installation", StringComparison.OrdinalIgnoreCase);
      installationEvent = flag ? this.GitHubAppInstallationToExternalAppInstallation(requestContext, eventPayload) : (ProviderInstallationEvent) null;
      return flag;
    }

    private void HandleAppInstallationEvent(
      IVssRequestContext requestContext,
      ProviderInstallationEvent installationEvent,
      IPipelineSourceProvider provider)
    {
      GitHubInstallationEventHandler.PublishEventCI(requestContext, installationEvent);
      if (provider == null)
        return;
      this.HandleAppInstallationEvent(requestContext, provider, installationEvent);
    }

    private ProviderInstallationEvent GitHubAppInstallationToExternalAppInstallation(
      IVssRequestContext requestContext,
      JObject eventPayload)
    {
      object obj1 = (object) eventPayload;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target1 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p2 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "target_type", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target2 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p1 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__0, obj1);
      object obj3 = target2((CallSite) p1, obj2);
      string a1 = target1((CallSite) p2, obj3);
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target3 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p5 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__5;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repository_selection", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target4 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p4 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__4;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj4 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__3, obj1);
      object obj5 = target4((CallSite) p4, obj4);
      string a2 = target3((CallSite) p5, obj5);
      ExternalAppInstallationInfo installationInfo1 = new ExternalAppInstallationInfo();
      ExternalAppInstallationInfo installationInfo2 = installationInfo1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target5 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p8 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__8;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "app_id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target6 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p7 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__7;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__6, obj1);
      object obj7 = target6((CallSite) p7, obj6);
      string str1 = target5((CallSite) p8, obj7);
      installationInfo2.AppId = str1;
      ExternalAppInstallationInfo installationInfo3 = installationInfo1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target7 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p11 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__11;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target8 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p10 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__10;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj8 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__9, obj1);
      object obj9 = target8((CallSite) p10, obj8);
      string str2 = target7((CallSite) p11, obj9);
      installationInfo3.Id = str2;
      ExternalAppInstallationInfo installationInfo4 = installationInfo1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target9 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p15 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__15;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target10 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p14 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__14;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target11 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p13 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__13;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj10 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__12, obj1);
      object obj11 = target11((CallSite) p13, obj10);
      object obj12 = target10((CallSite) p14, obj11);
      string str3 = target9((CallSite) p15, obj12);
      installationInfo4.OrgId = str3;
      installationInfo1.IsOrgAUser = string.Equals(a1, "User", StringComparison.OrdinalIgnoreCase);
      ExternalAppInstallationInfo installationInfo5 = installationInfo1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target12 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p19 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__19;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target13 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p18 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__18;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, object> target14 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, object>> p17 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__17;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj13 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__16, obj1);
      object obj14 = target14((CallSite) p17, obj13);
      object obj15 = target13((CallSite) p18, obj14);
      string str4 = target12((CallSite) p19, obj15);
      installationInfo5.OrgName = str4;
      ExternalAppInstallationInfo installationInfo6 = installationInfo1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21 = CallSite<Func<CallSite, GitHubInstallationEventHandler, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName, "ToExternalGitRepos", (IEnumerable<Type>) null, typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, GitHubInstallationEventHandler, object, object> target15 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, GitHubInstallationEventHandler, object, object>> p21 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__21;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "repositories", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj16 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__20, obj1);
      object obj17 = target15((CallSite) p21, this, obj16);
      ProviderInstallationEvent externalAppInstallation = new ProviderInstallationEvent();
      ProviderInstallationEvent installationEvent1 = externalAppInstallation;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target16 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p23 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__23;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj18 = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__22, obj1);
      string str5 = target16((CallSite) p23, obj18);
      installationEvent1.Action = str5;
      externalAppInstallation.AppName = "azure-pipelines";
      externalAppInstallation.Installation = installationInfo6;
      externalAppInstallation.IsAllRepoSelected = string.Equals(a2, "all", StringComparison.OrdinalIgnoreCase);
      ProviderInstallationEvent installationEvent2 = externalAppInstallation;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__24 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, IEnumerable<ExternalGitRepo>>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable<ExternalGitRepo>), typeof (GitHubInstallationEventHandler)));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<ExternalGitRepo> externalGitRepos = GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__24.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__7.\u003C\u003Ep__24, obj17);
      installationEvent2.Repositories = externalGitRepos;
      return externalAppInstallation;
    }

    private bool IsUninstallEvent(
      IVssRequestContext requestContext,
      ProviderInstallationEvent installationEvent)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubInstallationEventHandler), "IsUninstallEvent - github");
      ArgumentUtility.CheckForNull<ProviderInstallationEvent>(installationEvent, nameof (installationEvent));
      return string.Equals(installationEvent.Action, "deleted");
    }

    private void HandleAppInstallationEvent(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      ProviderInstallationEvent installationEvent)
    {
      GitHubInstallationEventHandler.PublishEventCI(requestContext, installationEvent);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubInstallationEventHandler), "HandleAppInstallationEvent - provider=" + provider.ProviderId);
      ArgumentUtility.CheckForNull<ProviderInstallationEvent>(installationEvent, nameof (installationEvent));
      if (!this.IsUninstallEvent(requestContext, installationEvent))
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubInstallationEventHandler), "HandleAppInstallationEvent - application " + installationEvent.AppName + " is not installed. Ignoring");
      }
      else
      {
        Guid accountId = requestContext.ServiceHost.InstanceId;
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IProjectService service = vssRequestContext.GetService<IProjectService>();
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubInstallationEventHandler), string.Format("{0} - removing routing for installation id {1} in account {2}.", (object) nameof (HandleAppInstallationEvent), (object) installationEvent.Installation.Id, (object) accountId));
        PipelinesHostIdMappingManager.RemoveRoute(vssRequestContext, provider, installationEvent.Installation.Id);
        IVssRequestContext requestContext1 = vssRequestContext;
        foreach (ProjectInfo project in service.GetProjects(requestContext1, ProjectState.WellFormed))
        {
          ProjectInfo projectInfo = project;
          try
          {
            requestContext.RunAsUser(accountId, projectInfo.Id, provider.ConnectionCreator.IdentityRole, (Action<IVssRequestContext>) (userContext =>
            {
              requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Events.HandleEvent, nameof (GitHubInstallationEventHandler), string.Format("{0} - mark service endpoints using installation id {1} in account {2} and project {3}", (object) nameof (HandleAppInstallationEvent), (object) installationEvent.Installation.Id, (object) accountId, (object) projectInfo.Id));
              ServiceEndpointHelper.MarkStaleServiceEndpoints(userContext, projectInfo.Id, provider, installationEvent.Installation.Id);
            }));
          }
          catch (IdentityNotFoundException ex)
          {
            requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubInstallationEventHandler), "IdentityNotFoundException for project id {0}. A GitHub App is not registered to that project. Error: {1}", (object) projectInfo.Id, (object) ex.Message);
          }
        }
      }
    }

    private List<ExternalGitRepo> ToExternalGitRepos(object repoList)
    {
      List<ExternalGitRepo> externalGitRepos = new List<ExternalGitRepo>();
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p1 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__0, repoList, (object) null);
      if (target1((CallSite) p1, obj1))
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (IEnumerable), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        foreach (object obj2 in GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__8, repoList))
        {
          List<ExternalGitRepo> externalGitRepoList = externalGitRepos;
          ExternalGitRepo externalGitRepo1 = new ExternalGitRepo();
          ExternalGitRepo externalGitRepo2 = externalGitRepo1;
          // ISSUE: reference to a compiler-generated field
          if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, string> target2 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, string>> p3 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__3;
          // ISSUE: reference to a compiler-generated field
          if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj3 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__2, obj2);
          string str1 = target2((CallSite) p3, obj3);
          externalGitRepo2.Id = str1;
          ExternalGitRepo externalGitRepo3 = externalGitRepo1;
          // ISSUE: reference to a compiler-generated field
          if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, string> target3 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, string>> p5 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__5;
          // ISSUE: reference to a compiler-generated field
          if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj4 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__4, obj2);
          string str2 = target3((CallSite) p5, obj4);
          externalGitRepo3.Name = str2;
          ExternalGitRepo externalGitRepo4 = externalGitRepo1;
          // ISSUE: reference to a compiler-generated field
          if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (GitHubInstallationEventHandler)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target4 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p7 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__7;
          // ISSUE: reference to a compiler-generated field
          if (GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "private", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj5 = GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__10.\u003C\u003Ep__6, obj2);
          int num = target4((CallSite) p7, obj5) ? 1 : 0;
          externalGitRepo4.IsPrivate = num != 0;
          ExternalGitRepo externalGitRepo5 = externalGitRepo1;
          externalGitRepoList.Add(externalGitRepo5);
        }
      }
      return externalGitRepos;
    }

    private bool IsMarketplacePurchaseEvent(
      IVssRequestContext requestContext,
      string eventType,
      JObject eventPayload,
      out ProviderMarketplacePurchaseEvent marketplacePurchaseEvent)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (GitHubInstallationEventHandler), "IsMarketplacePurchaseEvent - github");
      ArgumentUtility.CheckForNull<string>(eventType, nameof (eventType));
      ArgumentUtility.CheckForNull<JObject>(eventPayload, nameof (eventPayload));
      bool flag = string.Equals(eventType, "marketplace_purchase", StringComparison.OrdinalIgnoreCase);
      if (flag)
      {
        object obj1 = (object) eventPayload;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target1 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p3 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__3;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "type", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target2 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__2.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p2 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__2;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target3 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p1 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__0, obj1);
        object obj3 = target3((CallSite) p1, obj2);
        object obj4 = target2((CallSite) p2, obj3);
        string a = target1((CallSite) p3, obj4);
        ExternalAppInstallationInfo installationInfo1 = new ExternalAppInstallationInfo();
        ExternalAppInstallationInfo installationInfo2 = installationInfo1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__7 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target4 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__7.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p7 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__7;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__6 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target5 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__6.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p6 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__6;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__5 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target6 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__5.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p5 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__5;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj5 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__4.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__4, obj1);
        object obj6 = target6((CallSite) p5, obj5);
        object obj7 = target5((CallSite) p6, obj6);
        string str1 = target4((CallSite) p7, obj7);
        installationInfo2.OrgId = str1;
        installationInfo1.IsOrgAUser = string.Equals(a, "User", StringComparison.OrdinalIgnoreCase);
        ExternalAppInstallationInfo installationInfo3 = installationInfo1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__11 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target7 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__11.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p11 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__11;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__10 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "login", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target8 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__10.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p10 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__10;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__9 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "account", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target9 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__9.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p9 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__9;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__8 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj8 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__8.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__8, obj1);
        object obj9 = target9((CallSite) p9, obj8);
        object obj10 = target8((CallSite) p10, obj9);
        string str2 = target7((CallSite) p11, obj10);
        installationInfo3.OrgName = str2;
        ExternalAppInstallationInfo installationInfo4 = installationInfo1;
        MarketplacePurchase marketplacePurchase1 = new MarketplacePurchase();
        MarketplacePurchase marketplacePurchase2 = marketplacePurchase1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__15 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target10 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__15.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p15 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__15;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__14 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "name", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target11 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__14.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p14 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__14;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__13 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "plan", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target12 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__13.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p13 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__13;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__12 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj11 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__12.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__12, obj1);
        object obj12 = target12((CallSite) p13, obj11);
        object obj13 = target11((CallSite) p14, obj12);
        string str3 = target10((CallSite) p15, obj13);
        marketplacePurchase2.PlanName = str3;
        MarketplacePurchase marketplacePurchase3 = marketplacePurchase1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__18 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target13 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__18.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p18 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__18;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__17 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "billing_cycle", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target14 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__17.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p17 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__17;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__16 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj14 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__16.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__16, obj1);
        object obj15 = target14((CallSite) p17, obj14);
        string str4 = target13((CallSite) p18, obj15);
        marketplacePurchase3.BillingCycle = str4;
        MarketplacePurchase marketplacePurchase4 = marketplacePurchase1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__22 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target15 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__22.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p22 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__22;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__21 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "unit_name", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target16 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__21.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p21 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__21;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__20 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "plan", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target17 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__20.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p20 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__20;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__19 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj16 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__19.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__19, obj1);
        object obj17 = target17((CallSite) p20, obj16);
        object obj18 = target16((CallSite) p21, obj17);
        string str5 = target15((CallSite) p22, obj18);
        marketplacePurchase4.UnitName = str5;
        MarketplacePurchase marketplacePurchase5 = marketplacePurchase1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__25 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, int>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (int), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, int> target18 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__25.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, int>> p25 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__25;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__24 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "unit_count", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target19 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__24.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p24 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__24;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__23 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj19 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__23.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__23, obj1);
        object obj20 = target19((CallSite) p24, obj19);
        int num = target18((CallSite) p25, obj20);
        marketplacePurchase5.UnitCount = num;
        MarketplacePurchase marketplacePurchase6 = marketplacePurchase1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__28 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target20 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__28.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p28 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__28;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__27 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__27 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "next_billing_date", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target21 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__27.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p27 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__27;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__26 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__26 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "marketplace_purchase", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj21 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__26.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__26, obj1);
        object obj22 = target21((CallSite) p27, obj21);
        string str6 = target20((CallSite) p28, obj22);
        marketplacePurchase6.NextBillingDate = str6;
        MarketplacePurchase marketplacePurchase7 = marketplacePurchase1;
        ref ProviderMarketplacePurchaseEvent local = ref marketplacePurchaseEvent;
        ProviderMarketplacePurchaseEvent marketplacePurchaseEvent1 = new ProviderMarketplacePurchaseEvent();
        ProviderMarketplacePurchaseEvent marketplacePurchaseEvent2 = marketplacePurchaseEvent1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__30 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target22 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__30.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p30 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__30;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__29 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "action", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj23 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__29.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__29, obj1);
        string str7 = target22((CallSite) p30, obj23);
        marketplacePurchaseEvent2.Action = str7;
        marketplacePurchaseEvent1.AppName = "azure-pipelines";
        ProviderMarketplacePurchaseEvent marketplacePurchaseEvent3 = marketplacePurchaseEvent1;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__32 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubInstallationEventHandler)));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, string> target23 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__32.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, string>> p32 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__32;
        // ISSUE: reference to a compiler-generated field
        if (GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__31 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "effective_date", typeof (GitHubInstallationEventHandler), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj24 = GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__31.Target((CallSite) GitHubInstallationEventHandler.\u003C\u003Eo__11.\u003C\u003Ep__31, obj1);
        string str8 = target23((CallSite) p32, obj24);
        marketplacePurchaseEvent3.EffectiveDate = str8;
        marketplacePurchaseEvent1.Installation = installationInfo4;
        marketplacePurchaseEvent1.MarketplacePurchase = marketplacePurchase7;
        ProviderMarketplacePurchaseEvent marketplacePurchaseEvent4 = marketplacePurchaseEvent1;
        local = marketplacePurchaseEvent4;
      }
      else
        marketplacePurchaseEvent = (ProviderMarketplacePurchaseEvent) null;
      return flag;
    }

    private void HandleMarketplacePurchaseEvent(
      IVssRequestContext requestContext,
      ProviderMarketplacePurchaseEvent marketplacePurchaseEvent)
    {
      GitHubInstallationEventHandler.PublishEventCI(requestContext, marketplacePurchaseEvent);
    }

    private static void PublishEventCI(
      IVssRequestContext requestContext,
      ProviderInstallationRepositoriesEvent installationRepositoriesEvent)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.AddInstallationRepositoriesEventData(installationRepositoriesEvent);
      ciData.PublishCI(requestContext, "GitHubApp");
    }

    private static void PublishEventCI(
      IVssRequestContext requestContext,
      ProviderInstallationEvent installationEvent)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.AddInstallationEventData(installationEvent);
      ciData.PublishCI(requestContext, "GitHubApp");
    }

    private static void PublishEventCI(
      IVssRequestContext requestContext,
      ProviderMarketplacePurchaseEvent marketplacePurchaseEvent)
    {
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      ciData.AddMarketplacePurchaseEventData(marketplacePurchaseEvent);
      ciData.PublishCI(requestContext, "GitHubApp");
    }
  }
}
