// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UxServicesModelBuilder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.UxServices;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class UxServicesModelBuilder
  {
    private readonly IVssRequestContext tfsRequestContext;
    private readonly ViewContext viewContext;
    private const string m_defaultBrand = "default";
    private const string m_enableOriginatingUxServicefeatureName = "VisualStudio.WorkflowService.EnableOriginatingUxService";

    public UxServicesModelBuilder(ViewContext context, IVssRequestContext vssRequestContext = null)
    {
      this.tfsRequestContext = vssRequestContext ?? context.TfsRequestContext();
      this.viewContext = context;
    }

    public UxServicesModel GetModel()
    {
      UxServicesModel model = new UxServicesModel();
      if (this.tfsRequestContext.GetService<CachedRegistryService>().GetValue<bool>(this.tfsRequestContext, (RegistryQuery) "/WebAccess/UxServices/Enabled", false))
      {
        CultureInfo cultureInfo;
        try
        {
          cultureInfo = CultureInfo.CreateSpecificCulture(Thread.CurrentThread.CurrentUICulture.Name);
        }
        catch (Exception ex)
        {
          cultureInfo = new CultureInfo("en-US");
        }
        try
        {
          model.UseUxServices = true;
          UxServicesProvider servicesProvider = new UxServicesProvider((IContentService) new UxServicesHttpRequestWrapper(this.tfsRequestContext), this.tfsRequestContext);
          // ISSUE: reference to a compiler-generated field
          if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "WorkflowId", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj1 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__0, this.viewContext.ViewBag);
          // ISSUE: reference to a compiler-generated field
          if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "UxServiceHeaderUrl", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__1.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__1, this.viewContext.ViewBag);
          // ISSUE: reference to a compiler-generated field
          if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "UxServiceFooterUrl", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj3 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__2.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__2, this.viewContext.ViewBag);
          // ISSUE: reference to a compiler-generated field
          if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target1 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__5.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p5 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__5;
          // ISSUE: reference to a compiler-generated field
          if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.Not, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, object> target2 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__4.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, object>> p4 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__4;
          // ISSUE: reference to a compiler-generated field
          if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SkipUxHeaderFooter", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj4 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__3.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__3, this.viewContext.ViewBag) ?? (object) false;
          object obj5 = target2((CallSite) p4, obj4);
          if (target1((CallSite) p5, obj5))
          {
            // ISSUE: reference to a compiler-generated field
            if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__8 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, bool> target3 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__8.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, bool>> p8 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__8;
            // ISSUE: reference to a compiler-generated field
            if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__7 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.Not, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object> target4 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__7.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object>> p7 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__7;
            // ISSUE: reference to a compiler-generated field
            if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__6 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SkipUxHeader", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj6 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__6.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__6, this.viewContext.ViewBag) ?? (object) false;
            object obj7 = target4((CallSite) p7, obj6);
            if (target3((CallSite) p8, obj7))
            {
              UxServicesModel uxServicesModel1 = model;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__10 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (UxServicesModelBuilder)));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target5 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__10.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p10 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__10;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__9 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "HideHeaderLowerMenu", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj8 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__9.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__9, this.viewContext.ViewBag) ?? (object) false;
              int num1 = target5((CallSite) p10, obj8) ? 1 : 0;
              uxServicesModel1.HideHeaderLowerMenu = num1 != 0;
              UxServicesModel uxServicesModel2 = model;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__12 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, bool>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (UxServicesModelBuilder)));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target6 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__12.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p12 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__12;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__11 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__11 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "HideHeaderUserOptions", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj9 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__11.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__11, this.viewContext.ViewBag) ?? (object) false;
              int num2 = target6((CallSite) p12, obj9) ? 1 : 0;
              uxServicesModel2.HideUserOptions = num2 != 0;
              UxServicesRequestData servicesRequestData1 = new UxServicesRequestData();
              UxServicesRequestData servicesRequestData2 = servicesRequestData1;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__15 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UxServicesModelBuilder)));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, string> target7 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__15.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, string>> p15 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__15;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__14 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target8 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__14.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p14 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__14;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__13 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__13 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "IsNullOrEmpty", (IEnumerable<Type>) null, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj10 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__13.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__13, typeof (string), obj1);
              object obj11 = target8((CallSite) p14, obj10) ? (object) "default" : obj1;
              string str1 = target7((CallSite) p15, obj11);
              servicesRequestData2.Brand = str1;
              servicesRequestData1.Control = "header";
              servicesRequestData1.Locale = cultureInfo.Name;
              UxServicesRequestData servicesRequestData3 = servicesRequestData1;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__16 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__16 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UxServicesModelBuilder)));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              string str2 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__16.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__16, obj2);
              servicesRequestData3.UxServiceHeaderUrl = str2;
              UxServicesRequestData uxRequestData = servicesRequestData1;
              LoginContext loginContext1 = new LoginContext();
              WebContext webContext = this.viewContext.WebContext();
              Microsoft.VisualStudio.Services.Identity.Identity currentIdentity = webContext.CurrentIdentity;
              if (currentIdentity != null)
              {
                loginContext1.IsAuthenticated = true;
                loginContext1.SignInOutText = WACommonResources.SignOut;
                loginContext1.SignInOutLink = webContext.Url.LocAwareAction("index", "_signout");
                loginContext1.ProfileLink = webContext.Url.LocAwareAction("profile", "go");
                LoginContext loginContext2 = loginContext1;
                // ISSUE: reference to a compiler-generated field
                if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__18 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UxServicesModelBuilder)));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, string> target9 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__18.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, string>> p18 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__18;
                // ISSUE: reference to a compiler-generated field
                if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__17 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ProfileDisplayName", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj12 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__17.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__17, this.viewContext.ViewBag) ?? (object) currentIdentity.DisplayName;
                string str3 = target9((CallSite) p18, obj12);
                loginContext2.ProfileText = str3;
              }
              uxRequestData.LoginContext = loginContext1;
              model.UxServicesHeader = servicesProvider.GetData(uxRequestData);
            }
            // ISSUE: reference to a compiler-generated field
            if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__21 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, bool> target10 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__21.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, bool>> p21 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__21;
            // ISSUE: reference to a compiler-generated field
            if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__20 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.Not, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            Func<CallSite, object, object> target11 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__20.Target;
            // ISSUE: reference to a compiler-generated field
            CallSite<Func<CallSite, object, object>> p20 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__20;
            // ISSUE: reference to a compiler-generated field
            if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__19 == null)
            {
              // ISSUE: reference to a compiler-generated field
              UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "SkipUxFooter", typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
              {
                CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
              }));
            }
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            object obj13 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__19.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__19, this.viewContext.ViewBag) ?? (object) false;
            object obj14 = target11((CallSite) p20, obj13);
            if (target10((CallSite) p21, obj14))
            {
              UxServicesRequestData servicesRequestData4 = new UxServicesRequestData();
              UxServicesRequestData servicesRequestData5 = servicesRequestData4;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__24 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UxServicesModelBuilder)));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, string> target12 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__24.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, string>> p24 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__24;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__23 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target13 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__23.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p23 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__23;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__22 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__22 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "IsNullOrEmpty", (IEnumerable<Type>) null, typeof (UxServicesModelBuilder), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj15 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__22.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__22, typeof (string), obj1);
              object obj16 = target13((CallSite) p23, obj15) ? (object) "default" : obj1;
              string str4 = target12((CallSite) p24, obj16);
              servicesRequestData5.Brand = str4;
              servicesRequestData4.Control = "footer";
              servicesRequestData4.Locale = cultureInfo.Name;
              UxServicesRequestData servicesRequestData6 = servicesRequestData4;
              // ISSUE: reference to a compiler-generated field
              if (UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__25 == null)
              {
                // ISSUE: reference to a compiler-generated field
                UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (UxServicesModelBuilder)));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              string str5 = UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__25.Target((CallSite) UxServicesModelBuilder.\u003C\u003Eo__4.\u003C\u003Ep__25, obj3);
              servicesRequestData6.UxServiceFooterUrl = str5;
              UxServicesRequestData uxRequestData = servicesRequestData4;
              model.UxServicesFooter = servicesProvider.GetData(uxRequestData);
            }
          }
        }
        catch (Exception ex)
        {
          this.tfsRequestContext.TraceException(1049002, TraceLevel.Error, "WebAccess", "UxServices", ex);
          this.tfsRequestContext.Trace(1049003, TraceLevel.Warning, "WebAccess", "UxServices", "Error accessing UxServices, falling back to local header");
        }
      }
      return model;
    }
  }
}
