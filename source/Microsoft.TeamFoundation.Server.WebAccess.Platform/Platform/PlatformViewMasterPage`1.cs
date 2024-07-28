// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.PlatformViewMasterPage`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public class PlatformViewMasterPage<TModel> : ViewMasterPage<TModel>
  {
    public WebContext WebContext => this.ViewContext.WebContext();

    public UxServicesModel UxServicesModel => new UxServicesModelBuilder(this.ViewContext).GetModel();

    public bool HideHeader
    {
      get
      {
        if (PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__2 == null)
          PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (PlatformViewMasterPage<TModel>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        Func<CallSite, object, bool> target1 = PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__2.Target;
        CallSite<Func<CallSite, object, bool>> p2 = PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__2;
        if (PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__1 == null)
          PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (PlatformViewMasterPage<TModel>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        Func<CallSite, object, object, object> target2 = PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__1.Target;
        CallSite<Func<CallSite, object, object, object>> p1 = PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__1;
        if (PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
          PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, nameof (HideHeader), typeof (PlatformViewMasterPage<TModel>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        object obj1 = PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) PlatformViewMasterPage<TModel>.\u003C\u003Eo__5.\u003C\u003Ep__0, this.ViewContext.ViewBag);
        object obj2 = target2((CallSite) p1, obj1, (object) null);
        return target1((CallSite) p2, obj2);
      }
    }
  }
}
