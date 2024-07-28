// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanFilterHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public static class PlanFilterHelper
  {
    public static PlanViewFilter CreatePlanFilter(
      Plan plan,
      IDictionary<string, object> filterProperties)
    {
      return plan.Type == PlanType.DeliveryTimelineView ? PlanFilterHelper.CreatePlanFilterImpl(plan, filterProperties) : throw new ViewTypeDoesNotExistException();
    }

    public static PlanViewFilter CreatePlanFilterImpl(
      Plan view,
      IDictionary<string, object> filterProperties)
    {
      ArgumentUtility.CheckForNull<Plan>(view, nameof (view));
      if (view.Properties == null)
        return (PlanViewFilter) new DeliveryViewFilter();
      DeliveryViewPropertyCollection properties = (DeliveryViewPropertyCollection) view.Properties;
      DateTime? startDate = new DateTime?();
      DateTime? endDate = new DateTime?();
      if (filterProperties != null && filterProperties.Count > 0)
      {
        if (filterProperties.ContainsKey("StartDate"))
        {
          // ISSUE: reference to a compiler-generated field
          if (PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__1 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, DateTime?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (DateTime?), typeof (PlanFilterHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, DateTime?> target = PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__1.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, DateTime?>> p1 = PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__1;
          // ISSUE: reference to a compiler-generated field
          if (PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToDateTime", (IEnumerable<Type>) null, typeof (PlanFilterHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj = PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__0, typeof (Convert), filterProperties["StartDate"]);
          startDate = target((CallSite) p1, obj);
        }
        if (filterProperties.ContainsKey("EndDate"))
        {
          // ISSUE: reference to a compiler-generated field
          if (PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, DateTime?>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (DateTime?), typeof (PlanFilterHelper)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, DateTime?> target = PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__3.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, DateTime?>> p3 = PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__3;
          // ISSUE: reference to a compiler-generated field
          if (PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__2 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToDateTime", (IEnumerable<Type>) null, typeof (PlanFilterHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj = PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__2.Target((CallSite) PlanFilterHelper.\u003C\u003Eo__1.\u003C\u003Ep__2, typeof (Convert), filterProperties["EndDate"]);
          endDate = target((CallSite) p3, obj);
        }
      }
      IEnumerable<string> fieldReferenceNames = PlanFilterHelper.GetValidAdditionalFieldReferenceNames(properties?.CardSettings?.Fields?.AdditionalFields);
      return (PlanViewFilter) DeliveryViewFilterBuilder.Create(properties, view.Revision, startDate, endDate, fieldReferenceNames);
    }

    public static IEnumerable<string> GetValidAdditionalFieldReferenceNames(
      IEnumerable<FieldInfo> additionalFields)
    {
      return additionalFields == null ? (IEnumerable<string>) null : additionalFields.Where<FieldInfo>((Func<FieldInfo, bool>) (x => !x.ReferenceName.IsNullOrEmpty<char>() && !x.DisplayName.IsNullOrEmpty<char>())).Select<FieldInfo, string>((Func<FieldInfo, string>) (valid => valid.ReferenceName));
    }
  }
}
