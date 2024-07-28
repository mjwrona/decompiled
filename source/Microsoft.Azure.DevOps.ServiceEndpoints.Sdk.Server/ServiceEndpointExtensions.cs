// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.ServiceEndpointExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class ServiceEndpointExtensions
  {
    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails ToServiceEndpointDetails(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails serviceEndpointDetails = new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointDetails()
      {
        Type = serviceEndpoint.Type,
        Url = serviceEndpoint.Url,
        Data = serviceEndpoint.Data
      };
      if (serviceEndpoint.Authorization != null)
        serviceEndpointDetails.Authorization = serviceEndpoint.Authorization.Clone();
      return serviceEndpointDetails;
    }

    public static IDictionary<string, string> GetAzureStackDependencyData(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint)
    {
      if (!endpoint.Type.Equals("AzureRM", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(ServiceEndpointSdkResources.PopulateAzureStackSupportedForAzureRmEndpoints((object) endpoint.Name, (object) endpoint.Type));
      string str1;
      if (endpoint.Data.TryGetValue("environment", out str1) && !"AzureStack".Equals(str1, StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(ServiceEndpointSdkResources.PopulateAzureStackSupportedForAzureStackeEnvironment());
      Dictionary<string, string> stackDependencyData = new Dictionary<string, string>();
      try
      {
        if (endpoint.Url != (Uri) null)
        {
          string str2 = endpoint.Url.ToString().TrimEnd('/');
          string str3;
          try
          {
            int num = str2.IndexOf('.');
            str3 = str2.Remove(0, num + 1).Trim('/');
          }
          catch (Exception ex)
          {
            throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointException(ServiceEndpointSdkResources.SpecifiedAzureRMEndpointIsInvalid(), ex);
          }
          stackDependencyData.Add("EnableAdfsAuthentication", "false");
          stackDependencyData.Add("AzureKeyVaultDnsSuffix", ("vault." + str3).ToLowerInvariant());
          stackDependencyData.Add("AzureKeyVaultServiceEndpointResourceId", ("https://vault." + str3).ToLowerInvariant());
          HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri(endpoint.Url, "/metadata/endpoints?api-version=1.0"));
          httpWebRequest.Method = "GET";
          httpWebRequest.Timeout = 30000;
          using (HttpWebResponse response = (HttpWebResponse) httpWebRequest.GetResponse())
          {
            if (response.StatusCode == HttpStatusCode.OK)
            {
              object obj1 = (object) JObject.Parse(new StreamReader(response.GetResponseStream()).ReadToEnd());
              // ISSUE: reference to a compiler-generated field
              if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__1 == null)
              {
                // ISSUE: reference to a compiler-generated field
                ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              Func<CallSite, object, bool> target1 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__1.Target;
              // ISSUE: reference to a compiler-generated field
              CallSite<Func<CallSite, object, bool>> p1 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__1;
              // ISSUE: reference to a compiler-generated field
              if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
              {
                // ISSUE: reference to a compiler-generated field
                ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                {
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                  CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                }));
              }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              object obj2 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__0, obj1, (object) null);
              if (target1((CallSite) p1, obj2))
              {
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__2 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "galleryEndpoint", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj3 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__2.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__2, obj1);
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__4 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target2 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__4.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p4 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__4;
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__3 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj4 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__3.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__3, obj3, (object) null);
                if (target2((CallSite) p4, obj4))
                {
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__6 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__6 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Action<CallSite, Dictionary<string, string>, string, object> target3 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__6.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Action<CallSite, Dictionary<string, string>, string, object>> p6 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__6;
                  Dictionary<string, string> dictionary = stackDependencyData;
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__5 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj5 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__5.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__5, obj3);
                  target3((CallSite) p6, dictionary, "galleryUrl", obj5);
                }
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__7 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "graphEndpoint", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj6 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__7.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__7, obj1);
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__9 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__9 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target4 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__9.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p9 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__9;
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__8 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__8 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj7 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__8.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__8, obj6, (object) null);
                if (target4((CallSite) p9, obj7))
                {
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__11 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__11 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Action<CallSite, Dictionary<string, string>, string, object> target5 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__11.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Action<CallSite, Dictionary<string, string>, string, object>> p11 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__11;
                  Dictionary<string, string> dictionary = stackDependencyData;
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__10 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__10 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj8 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__10.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__10, obj6);
                  target5((CallSite) p11, dictionary, "graphUrl", obj8);
                }
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__12 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__12 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "portalEndpoint", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj9 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__12.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__12, obj1);
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__14 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__14 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target6 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__14.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p14 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__14;
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__13 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__13 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj10 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__13.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__13, obj9, (object) null);
                if (target6((CallSite) p14, obj10))
                {
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__16 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__16 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Action<CallSite, Dictionary<string, string>, string, object> target7 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__16.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Action<CallSite, Dictionary<string, string>, string, object>> p16 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__16;
                  Dictionary<string, string> dictionary = stackDependencyData;
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__15 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__15 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj11 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__15.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__15, obj9);
                  target7((CallSite) p16, dictionary, "armManagementPortalUrl", obj11);
                }
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__17 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__17 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "authentication", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj12 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__17.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__17, obj1);
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__19 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__19 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, bool> target8 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__19.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, bool>> p19 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__19;
                // ISSUE: reference to a compiler-generated field
                if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__18 == null)
                {
                  // ISSUE: reference to a compiler-generated field
                  ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__18 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                  {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                  }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj13 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__18.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__18, obj12, (object) null);
                if (target8((CallSite) p19, obj13))
                {
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__20 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__20 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "loginEndpoint", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj14 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__20.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__20, obj12);
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__22 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__22 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, bool> target9 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__22.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, bool>> p22 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__22;
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__21 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__21 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj15 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__21.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__21, obj14, (object) null);
                  if (target9((CallSite) p22, obj15))
                  {
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__25 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__25 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Add, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, string, object> target10 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__25.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, string, object>> p25 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__25;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__24 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__24 = CallSite<Func<CallSite, object, char, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "TrimEnd", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, char, object> target11 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__24.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, char, object>> p24 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__24;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__23 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__23 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj16 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__23.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__23, obj14);
                    object obj17 = target11((CallSite) p24, obj16, '/');
                    object obj18 = target10((CallSite) p25, obj17, "/");
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__26 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__26 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__26.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__26, stackDependencyData, "activeDirectoryAuthority", obj18);
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__27 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__27 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__27.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__27, stackDependencyData, "environmentAuthorityUrl", obj18);
                    Dictionary<string, string> dictionary = stackDependencyData;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__30 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__30 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (ServiceEndpointExtensions)));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, string> target12 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__30.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, string>> p30 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__30;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__29 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__29 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, object> target13 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__29.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, object>> p29 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__29;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__28 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__28 = CallSite<Func<CallSite, object, string, StringComparison, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "EndsWith", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj19 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__28.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__28, obj18, "/adfs/", StringComparison.OrdinalIgnoreCase);
                    object obj20 = target13((CallSite) p29, obj19);
                    string str4 = target12((CallSite) p30, obj20);
                    dictionary["EnableAdfsAuthentication"] = str4;
                  }
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__31 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__31 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "audiences", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj21 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__31.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__31, obj12);
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__37 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__37 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  Func<CallSite, object, bool> target14 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__37.Target;
                  // ISSUE: reference to a compiler-generated field
                  CallSite<Func<CallSite, object, bool>> p37 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__37;
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__32 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__32 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
                    }));
                  }
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  object obj22 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__32.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__32, obj21, (object) null);
                  // ISSUE: reference to a compiler-generated field
                  if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__36 == null)
                  {
                    // ISSUE: reference to a compiler-generated field
                    ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__36 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsFalse, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                    {
                      CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                  }
                  object obj23;
                  // ISSUE: reference to a compiler-generated field
                  // ISSUE: reference to a compiler-generated field
                  if (!ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__36.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__36, obj22))
                  {
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__35 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__35 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.And, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, object, object> target15 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__35.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, object, object>> p35 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__35;
                    object obj24 = obj22;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__34 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__34 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.GreaterThan, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, int, object> target16 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__34.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, int, object>> p34 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__34;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__33 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__33 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj25 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__33.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__33, obj21);
                    object obj26 = target16((CallSite) p34, obj25, 0);
                    obj23 = target15((CallSite) p35, obj24, obj26);
                  }
                  else
                    obj23 = obj22;
                  if (target14((CallSite) p37, obj23))
                  {
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__40 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__40 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Action<CallSite, Dictionary<string, string>, string, object> target17 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__40.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Action<CallSite, Dictionary<string, string>, string, object>> p40 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__40;
                    Dictionary<string, string> dictionary1 = stackDependencyData;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__39 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__39 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, object> target18 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__39.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, object>> p39 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__39;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__38 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__38 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj27 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__38.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__38, obj21, 0);
                    object obj28 = target18((CallSite) p39, obj27);
                    target17((CallSite) p40, dictionary1, "serviceManagementUrl", obj28);
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__43 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__43 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Action<CallSite, Dictionary<string, string>, string, object> target19 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__43.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Action<CallSite, Dictionary<string, string>, string, object>> p43 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__43;
                    Dictionary<string, string> dictionary2 = stackDependencyData;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__42 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__42 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, object> target20 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__42.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, object>> p42 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__42;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__41 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__41 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj29 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__41.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__41, obj21, 0);
                    object obj30 = target20((CallSite) p42, obj29);
                    target19((CallSite) p43, dictionary2, "ActiveDirectoryServiceEndpointResourceId", obj30);
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__46 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__46 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, bool> target21 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__46.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, bool>> p46 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__46;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__45 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__45 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Func<CallSite, object, int, object> target22 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__45.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Func<CallSite, object, int, object>> p45 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__45;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__44 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__44 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "Count", typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj31 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__44.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__44, obj21);
                    object obj32 = target22((CallSite) p45, obj31, 1);
                    object obj33;
                    if (!target21((CallSite) p46, obj32))
                    {
                      // ISSUE: reference to a compiler-generated field
                      if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__47 == null)
                      {
                        // ISSUE: reference to a compiler-generated field
                        ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__47 = CallSite<Func<CallSite, object, int, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
                        {
                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
                        }));
                      }
                      // ISSUE: reference to a compiler-generated field
                      // ISSUE: reference to a compiler-generated field
                      obj33 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__47.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__47, obj21, 1);
                    }
                    else
                      obj33 = (object) endpoint.Url;
                    object obj34 = obj33;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__49 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__49 = CallSite<Action<CallSite, Dictionary<string, string>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    Action<CallSite, Dictionary<string, string>, string, object> target23 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__49.Target;
                    // ISSUE: reference to a compiler-generated field
                    CallSite<Action<CallSite, Dictionary<string, string>, string, object>> p49 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__49;
                    Dictionary<string, string> dictionary3 = stackDependencyData;
                    // ISSUE: reference to a compiler-generated field
                    if (ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__48 == null)
                    {
                      // ISSUE: reference to a compiler-generated field
                      ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__48 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "ToString", (IEnumerable<Type>) null, typeof (ServiceEndpointExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
                      {
                        CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                      }));
                    }
                    // ISSUE: reference to a compiler-generated field
                    // ISSUE: reference to a compiler-generated field
                    object obj35 = ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__48.Target((CallSite) ServiceEndpointExtensions.\u003C\u003Eo__1.\u003C\u003Ep__48, obj34);
                    target23((CallSite) p49, dictionary3, "resourceManagerUrl", obj35);
                  }
                }
              }
            }
            else
            {
              string connectAzureStack = ServiceEndpointSdkResources.RequestFailedAsUnableToConnectAzureStack((object) httpWebRequest.RequestUri, (object) response.StatusCode);
              throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointException(connectAzureStack, new Exception(connectAzureStack));
            }
          }
        }
      }
      catch (WebException ex)
      {
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointException(ServiceEndpointSdkResources.UnableToConnectToTheAzureStackEnvironment(), (Exception) ex);
      }
      return (IDictionary<string, string>) stackDependencyData;
    }

    public static bool IsAzureSubscriptionTypeKubernetesEndpoint(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint)
    {
      string a;
      return endpoint?.Data != null && endpoint.Data.TryGetValue("authorizationType", out a) && string.Equals(a, "AzureSubscription", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEndpointAuthSchemePublishProfile(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint) => endpoint?.Authorization != null && endpoint.Authorization.Scheme.Equals("PublishProfile", StringComparison.OrdinalIgnoreCase);

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint GetAdditionalServiceEndpointDetails(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType)
    {
      if (endpoint.Type.Equals("Azure", StringComparison.InvariantCultureIgnoreCase))
      {
        if (!endpoint.Data.ContainsKey("SubscriptionName"))
          endpoint.Data.Add("SubscriptionName", endpoint.Name);
        if (!endpoint.Data.ContainsKey("Environment"))
          endpoint.Data.Add("Environment", "AzureCloud");
      }
      else if (endpoint.Type.Equals("AzureRM", StringComparison.InvariantCultureIgnoreCase) && !endpoint.Data.ContainsKey("environment"))
        endpoint.Data.Add("environment", "AzureCloud");
      endpoint.PopulateDependencyData(requestContext, endpointType);
      return endpoint;
    }

    public static bool IsCustomEndpointType(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint)
    {
      ArgumentUtility.CheckForNull<string>(endpoint?.Type, "endpointType");
      return ServiceEndpointExtensions.IsCustomEndpointType(endpoint.Type, endpoint.Authorization?.Scheme);
    }

    public static bool IsCustomEndpointType(this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint)
    {
      ArgumentUtility.CheckForNull<string>(endpoint?.Type, "endpointType");
      return ServiceEndpointExtensions.IsCustomEndpointType(endpoint.Type, endpoint?.Authorization?.Scheme);
    }

    public static bool IsInternalEndpointType(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint) => ServiceEndpointExtensions.IsInternalEndpointType(endpoint?.Type, endpoint?.Authorization?.Scheme);

    public static bool IsInternalEndpointType(this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint) => ServiceEndpointExtensions.IsInternalEndpointType(endpoint?.Type, endpoint?.Authorization?.Scheme);

    public static Guid RemoveAuthConfigurationIfRequired(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint)
    {
      Guid configurationId = serviceEndpoint.Authorization.GetConfigurationId();
      if (!(configurationId != Guid.Empty))
        return configurationId;
      serviceEndpoint.Authorization.Parameters.Remove("ConfigurationId");
      return configurationId;
    }

    public static Guid GetConfigurationId(this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.EndpointAuthorization authorization) => authorization == null || authorization.Scheme == null || authorization.Parameters.IsNullOrEmpty<KeyValuePair<string, string>>() || (!authorization.Scheme.Equals("OAuth2") || !authorization.Parameters.ContainsKey("ConfigurationId")) && (!authorization.Scheme.Equals("InstallationToken") || !authorization.Parameters.ContainsKey("ConfigurationId")) && (!authorization.Scheme.Equals("JiraConnectApp") || !authorization.Parameters.ContainsKey("ConfigurationId")) ? Guid.Empty : Guid.Parse(authorization.Parameters["ConfigurationId"]);

    public static OAuthConfiguration GetConfigurationFromEndpointId(
      IVssRequestContext context,
      string projectId,
      string endpointId)
    {
      if (context == null)
        throw new ApplicationException("Expected Request Context");
      new ServiceEndpointSecurity().CheckPermission(context, projectId, endpointId, 1, true, (Func<IVssRequestContext, string>) (requestContext => ServiceEndpointSdkResources.EndpointAccessDeniedForUseOperation()));
      IVssRequestContext requestContext1 = context.Elevate();
      Guid configurationId = context.GetService<IServiceEndpointService2>().GetServiceEndpointAsync(requestContext1, Guid.Parse(projectId), Guid.Parse(endpointId)).Result.Authorization.GetConfigurationId();
      if (configurationId != Guid.Empty)
        return (OAuthConfiguration) context.GetService<IOAuthConfigurationService2>().GetAuthConfiguration(requestContext1, configurationId);
      throw new ApplicationException("This deployment is not registered with a GitHub account. Configuration Not Found: " + configurationId.ToString());
    }

    public static IDictionary<string, IdentityRef> ResolveIdentityRefs(
      this IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> endpoints,
      IVssRequestContext requestContext)
    {
      IDictionary<string, IdentityRef> identities = endpoints.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (endpoint => endpoint.CreatedBy != null)).Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>) (endpoint => endpoint.CreatedBy.Id)).ToList<string>().QueryIdentities(requestContext);
      endpoints.ForEach<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Action<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endpoint =>
      {
        if (endpoint.CreatedBy == null)
          return;
        endpoint.CreatedBy = identities[endpoint.CreatedBy.Id];
      }));
      return identities;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetServiceEndpointCreator(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint,
      IVssRequestContext requestContext)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      return !string.IsNullOrEmpty(serviceEndpoint.CreatedBy?.Id) ? service.GetIdentity(requestContext, serviceEndpoint.CreatedBy.Id) ?? requestContext.GetUserIdentity() : requestContext.GetUserIdentity();
    }

    public static void ResolveIdentityRefs(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      IVssRequestContext requestContext)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      endpoint.CreatedBy = service.GetIdentity(requestContext, endpoint.CreatedBy.Id).ToIdentityRef(requestContext);
    }

    public static IDictionary<string, string> GetFilteredEndpointData(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      bool isConfidential)
    {
      IDictionary<string, string> confidential;
      IDictionary<string, string> nonConfidential;
      endpoint.SplitEndpointData(endpointType, out confidential, out nonConfidential);
      return !isConfidential ? nonConfidential : confidential;
    }

    public static IDictionary<string, string> GetAuthorizationParameters(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      List<string> inputDescriptors)
    {
      Dictionary<string, string> authorizationParameters = new Dictionary<string, string>();
      foreach (string inputDescriptor in inputDescriptors)
      {
        string str;
        if (endpoint.Authorization.Parameters.TryGetValue(inputDescriptor, out str))
          authorizationParameters.Add(inputDescriptor, str);
      }
      return (IDictionary<string, string>) authorizationParameters;
    }

    public static IDictionary<string, string> GetFilteredEndpointData(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      bool isConfidential)
    {
      IDictionary<string, string> confidential;
      IDictionary<string, string> nonConfidential;
      endpoint.SplitEndpointData(endpointType, out confidential, out nonConfidential);
      return !isConfidential ? nonConfidential : confidential;
    }

    public static IDictionary<string, string> GetFilteredAuthorizationParameters(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      bool isConfidential)
    {
      IDictionary<string, string> confidential;
      IDictionary<string, string> nonConfidential;
      ServiceEndpointExtensions.SplitAuthorizationParameters(endpoint, endpointType, out confidential, out nonConfidential);
      return !isConfidential ? nonConfidential : confidential;
    }

    public static IDictionary<string, string> GetFilteredAuthorizationParameters(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      bool isConfidential)
    {
      IDictionary<string, string> confidential;
      IDictionary<string, string> nonConfidential;
      endpoint.SplitAuthorizationParameters(endpointType, out confidential, out nonConfidential);
      return !isConfidential ? nonConfidential : confidential;
    }

    public static void ClearConfidentialDataEntries(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType)
    {
      if (endpoint == null || endpointType == null && string.Equals(endpoint?.Authorization?.Scheme, "WorkloadIdentityFederation", StringComparison.InvariantCulture))
        return;
      IDictionary<string, string> confidential;
      IDictionary<string, string> nonConfidential;
      endpoint.SplitEndpointData(endpointType, out confidential, out nonConfidential);
      endpoint.Data = nonConfidential;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) confidential)
        endpoint.Data[keyValuePair.Key] = (string) null;
    }

    public static void ClearConfidentialAuthorizationParameters(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType)
    {
      if (endpoint == null)
        return;
      IDictionary<string, string> confidential;
      IDictionary<string, string> nonConfidential;
      ServiceEndpointExtensions.SplitAuthorizationParameters(endpoint, endpointType, out confidential, out nonConfidential);
      endpoint.Authorization.Parameters = nonConfidential;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) confidential)
        endpoint.Authorization.Parameters[keyValuePair.Key] = (string) null;
    }

    public static Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint GetEndpointCloneWithOutSecrets(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType)
    {
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint cloneWithOutSecrets = endpoint.Clone();
      cloneWithOutSecrets.Data = endpoint.GetFilteredEndpointData(endpointType, false);
      cloneWithOutSecrets.Authorization.Parameters = endpoint.GetFilteredAuthorizationParameters(endpointType, false);
      return cloneWithOutSecrets;
    }

    private static bool IsCustomEndpointType(string endpointType, string authScheme)
    {
      if (string.Equals(authScheme, "OAuth2", StringComparison.OrdinalIgnoreCase) && endpointType.Equals("GitHub", StringComparison.OrdinalIgnoreCase))
        return false;
      if (string.Equals(authScheme, "OAuth2", StringComparison.OrdinalIgnoreCase))
        return true;
      return (!endpointType.Equals("Bitbucket", StringComparison.OrdinalIgnoreCase) || !string.Equals(authScheme, "OAuth", StringComparison.OrdinalIgnoreCase)) && !endpointType.Equals("Git", StringComparison.OrdinalIgnoreCase) && !endpointType.Equals("Generic", StringComparison.OrdinalIgnoreCase) && !endpointType.Equals("GitHub", StringComparison.OrdinalIgnoreCase) && !endpointType.Equals("GitHubBoards", StringComparison.OrdinalIgnoreCase) && !endpointType.Equals("Subversion", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInternalEndpointType(string endpointType, string authScheme)
    {
      if (string.IsNullOrEmpty(endpointType))
        return true;
      return (ServiceEndpointExtensions.IsCustomEndpointType(endpointType, authScheme) ? 1 : 0) == 0 | (endpointType.Equals("TfsVersionControl", StringComparison.OrdinalIgnoreCase) || endpointType.Equals("TfsGit", StringComparison.OrdinalIgnoreCase) || endpointType.Equals("Git", StringComparison.OrdinalIgnoreCase) || endpointType.Equals("GitHub", StringComparison.OrdinalIgnoreCase) || endpointType.Equals("GitHubEnterprise", StringComparison.OrdinalIgnoreCase) || endpointType.Equals("Bitbucket", StringComparison.OrdinalIgnoreCase) || endpointType.Equals("Svn", StringComparison.OrdinalIgnoreCase));
    }

    private static void SplitEndpointData(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      out IDictionary<string, string> confidential,
      out IDictionary<string, string> nonConfidential)
    {
      confidential = (IDictionary<string, string>) new Dictionary<string, string>();
      nonConfidential = (IDictionary<string, string>) new Dictionary<string, string>();
      if (endpoint == null)
        return;
      ServiceEndpointExtensions.SplitEndpointData(endpoint.Data, endpointType, endpoint.IsInternalEndpointType(), out confidential, out nonConfidential);
    }

    private static void SplitEndpointData(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      out IDictionary<string, string> confidential,
      out IDictionary<string, string> nonConfidential)
    {
      confidential = (IDictionary<string, string>) new Dictionary<string, string>();
      nonConfidential = (IDictionary<string, string>) new Dictionary<string, string>();
      if (endpoint == null)
        return;
      ServiceEndpointExtensions.SplitEndpointData(endpoint.Data, endpointType, endpoint.IsInternalEndpointType(), out confidential, out nonConfidential);
    }

    private static void SplitEndpointData(
      IDictionary<string, string> endpointData,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      bool isInternalEndpointType,
      out IDictionary<string, string> confidential,
      out IDictionary<string, string> nonConfidential)
    {
      confidential = (IDictionary<string, string>) new Dictionary<string, string>();
      nonConfidential = (IDictionary<string, string>) new Dictionary<string, string>();
      if (endpointData == null)
        return;
      if (endpointType?.InputDescriptors != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) endpointData)
        {
          InputDescriptor inputDescriptor;
          endpointType.TryGetInputDescriptor(keyValuePair.Key, out inputDescriptor);
          if (inputDescriptor != null && inputDescriptor.IsConfidential)
            confidential[keyValuePair.Key] = keyValuePair.Value;
          else
            nonConfidential[keyValuePair.Key] = keyValuePair.Value;
        }
      }
      else
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) endpointData)
        {
          if (isInternalEndpointType)
            nonConfidential[keyValuePair.Key] = keyValuePair.Value;
          else
            confidential[keyValuePair.Key] = keyValuePair.Value;
        }
      }
    }

    private static void SplitAuthorizationParameters(
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      out IDictionary<string, string> confidential,
      out IDictionary<string, string> nonConfidential)
    {
      confidential = (IDictionary<string, string>) new Dictionary<string, string>();
      nonConfidential = (IDictionary<string, string>) new Dictionary<string, string>();
      if (endpoint?.Authorization == null)
        return;
      ServiceEndpointExtensions.SplitAuthorizationParameters(endpoint.Authorization.Parameters, endpoint.Authorization.Scheme, !string.IsNullOrEmpty(endpoint.Type) && endpoint.IsCustomEndpointType(), endpointType, out confidential, out nonConfidential);
    }

    private static void SplitAuthorizationParameters(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint endpoint,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      out IDictionary<string, string> confidential,
      out IDictionary<string, string> nonConfidential)
    {
      confidential = (IDictionary<string, string>) new Dictionary<string, string>();
      nonConfidential = (IDictionary<string, string>) new Dictionary<string, string>();
      if (endpoint?.Authorization == null)
        return;
      ServiceEndpointExtensions.SplitAuthorizationParameters(endpoint.Authorization.Parameters, endpoint.Authorization.Scheme, !string.IsNullOrEmpty(endpoint.Type) && endpoint.IsCustomEndpointType(), endpointType, out confidential, out nonConfidential);
    }

    private static void SplitAuthorizationParameters(
      IDictionary<string, string> authParameters,
      string endpointAuthScheme,
      bool isCustomEndpointType,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      out IDictionary<string, string> confidential,
      out IDictionary<string, string> nonConfidential)
    {
      confidential = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      nonConfidential = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (authParameters == null)
        return;
      try
      {
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme authScheme = endpointType.HasAuthenticationSchemes() ? endpointType.AuthenticationSchemes.FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme, bool>) (a => string.Compare(endpointAuthScheme, a.Scheme, StringComparison.OrdinalIgnoreCase) == 0)) : (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme) null;
        if (authScheme != null)
        {
          ServiceEndpointExtensions.SplitAuthorizationParamsBasedOnInputDescriptorsInScheme(authParameters, confidential, nonConfidential, authScheme);
          ServiceEndpointExtensions.SplitAuthorizationParamsForSpecialCases(authParameters, endpointType, confidential, nonConfidential, authScheme);
        }
        else
          ServiceEndpointExtensions.SplitAuthorizationParametersForNonExtensibleEndpointTypes(authParameters, isCustomEndpointType, confidential, nonConfidential);
      }
      catch (Exception ex)
      {
        throw new Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointException(ServiceEndpointSdkResources.ErrorInSplitAuthorizationParameters((object) endpointType, (object) endpointAuthScheme, (object) ex.Message));
      }
    }

    private static void SplitAuthorizationParamsForSpecialCases(
      IDictionary<string, string> authParameters,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType,
      IDictionary<string, string> confidential,
      IDictionary<string, string> nonConfidential,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme authScheme)
    {
      if (string.Equals(endpointType.Name, "kubernetes", StringComparison.OrdinalIgnoreCase) && string.Equals("Kubernetes", authScheme.Scheme, StringComparison.OrdinalIgnoreCase))
      {
        ServiceEndpointExtensions.RemoveAuthParameter(authParameters, nonConfidential, "ClientCertificateData");
        ServiceEndpointExtensions.RemoveAuthParameter(authParameters, nonConfidential, "ClientKeyData");
        ServiceEndpointExtensions.RemoveAuthParameter(authParameters, nonConfidential, "password");
        ServiceEndpointExtensions.RemoveAuthParameter(authParameters, nonConfidential, "apiToken");
      }
      if (authScheme.IsOauth())
      {
        if (string.Equals(endpointType.Name, "Bitbucket", StringComparison.OrdinalIgnoreCase) && authParameters.ContainsKey("RefreshToken"))
        {
          confidential["RefreshToken"] = authParameters["RefreshToken"];
          if (nonConfidential.ContainsKey("RefreshToken"))
            nonConfidential.Remove("RefreshToken");
        }
        if (authParameters.ContainsKey("AccessToken"))
        {
          confidential["AccessToken"] = authParameters["AccessToken"];
          if (nonConfidential.ContainsKey("AccessToken"))
            nonConfidential.Remove("AccessToken");
        }
      }
      if (!authScheme.IsOauth2())
        return;
      foreach (KeyValuePair<string, string> authParameter in (IEnumerable<KeyValuePair<string, string>>) authParameters)
      {
        if (authParameter.Key.Equals("ConfigurationId", StringComparison.OrdinalIgnoreCase))
        {
          nonConfidential["ConfigurationId"] = authParameters["ConfigurationId"];
          if (confidential.ContainsKey("ConfigurationId"))
            confidential.Remove("ConfigurationId");
        }
        else
        {
          confidential[authParameter.Key] = authParameters[authParameter.Key];
          if (nonConfidential.ContainsKey(authParameter.Key))
            nonConfidential.Remove(authParameter.Key);
        }
      }
    }

    private static void SplitAuthorizationParamsBasedOnInputDescriptorsInScheme(
      IDictionary<string, string> authParameters,
      IDictionary<string, string> confidential,
      IDictionary<string, string> nonConfidential,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointAuthenticationScheme authScheme)
    {
      foreach (KeyValuePair<string, string> authParameter in (IEnumerable<KeyValuePair<string, string>>) authParameters)
      {
        InputDescriptor inputDescriptor;
        authScheme.TryGetInputDescriptor(authParameter.Key, out inputDescriptor);
        if (inputDescriptor != null && inputDescriptor.IsConfidential)
          confidential[authParameter.Key] = authParameter.Value;
        else
          nonConfidential[authParameter.Key] = authParameter.Value;
      }
    }

    private static void SplitAuthorizationParametersForNonExtensibleEndpointTypes(
      IDictionary<string, string> authParameters,
      bool isCustomEndpointType,
      IDictionary<string, string> confidential,
      IDictionary<string, string> nonConfidential)
    {
      foreach (KeyValuePair<string, string> authParameter in (IEnumerable<KeyValuePair<string, string>>) authParameters)
      {
        if (!isCustomEndpointType && (authParameter.Key.Equals("Username", StringComparison.OrdinalIgnoreCase) || authParameter.Key.Equals("ConfigurationId", StringComparison.OrdinalIgnoreCase)))
          nonConfidential[authParameter.Key] = authParameter.Value;
        else
          confidential[authParameter.Key] = authParameter.Value;
      }
    }

    private static void RemoveAuthParameter(
      IDictionary<string, string> authParameters,
      IDictionary<string, string> nonConfidential,
      string parameter)
    {
      if (!authParameters.ContainsKey(parameter))
        return;
      nonConfidential.Remove(parameter);
    }

    private static void PopulateDependencyData(
      this Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint endpoint,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointType endpointType)
    {
      string str1;
      if (endpoint.Data.TryGetValue("environment", out str1))
      {
        if ("AzureStack".Equals(str1, StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            using (IEnumerator<KeyValuePair<string, string>> enumerator = endpoint.GetAzureStackDependencyData().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<string, string> current = enumerator.Current;
                endpoint.Data[current.Key] = current.Value;
              }
              return;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceWarning(34000204, "ServiceEndpoints", ServiceEndpointSdkResources.UnableToPopulateAzureStackData(), (object) ex.ToString());
            return;
          }
        }
      }
      if (endpoint.Authorization == null || endpoint.Authorization.Scheme == null || endpoint.Type == null || endpointType == null || endpointType.DependencyData == null)
        return;
      foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.DependencyData dependencyData in endpointType.DependencyData)
      {
        string input = dependencyData.Input;
        string str2;
        if (!string.IsNullOrEmpty(input) && endpoint.Data.TryGetValue(input, out str2) && !string.IsNullOrEmpty(str2))
        {
          foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> keyValuePair in dependencyData.Map)
          {
            if (keyValuePair.Key.Equals(str2, StringComparison.InvariantCultureIgnoreCase))
            {
              using (List<KeyValuePair<string, string>>.Enumerator enumerator = keyValuePair.Value.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  KeyValuePair<string, string> current = enumerator.Current;
                  if (!string.IsNullOrEmpty(current.Key))
                    endpoint.Data[current.Key] = current.Value;
                }
                break;
              }
            }
          }
        }
      }
    }
  }
}
