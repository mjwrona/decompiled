// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HeaderContentRewriteModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HeaderContentRewriteModule : IHttpModule
  {
    private const string AcceptEncodingHeader = "Accept-Encoding";
    private const string AcceptEncodingHeaderValueAny = "*";
    private const string AcceptEncodingHeaderValueGzip = "gzip";
    private const string AcceptEncodingRewriteFeatureEnabled = "MS.TF.Framework.Server.HttpModule.AcceptEncodingRewriteFeatureEnabled";
    private const string Area = "HeaderContentRewriteModule";
    private const string Layer = "HeaderContentRewriteModule";
    private static Func<HttpContext, string> Getter;
    private static Action<HttpContext> Setter;

    static HeaderContentRewriteModule()
    {
      try
      {
        HeaderContentRewriteModule.CreateLinqExpressionGetter();
        HeaderContentRewriteModule.CreateLinqExpressionSetter();
      }
      catch (Exception ex)
      {
        Trace.Fail(ex.StackTrace);
      }
    }

    public void Dispose()
    {
    }

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.BeginRequest);

    private void BeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      if (!TeamFoundationApplicationCore.DeploymentInitialized)
        TeamFoundationApplicationCore.ApplicationStart();
      if (!this.AcceptEncodingHeaderHasValueAny(context))
        return;
      using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
      {
        if (!this.AcceptEncodingRewriteEnabled(systemContext))
          return;
        context.Request.Headers["Accept-Encoding"] = "gzip";
        Action<HttpContext> setter = HeaderContentRewriteModule.Setter;
        if (setter != null)
          setter(context);
        systemContext.Trace(1050010, TraceLevel.Info, nameof (HeaderContentRewriteModule), nameof (HeaderContentRewriteModule), "Overwritten Accept-Encoding header, from * to gzip.");
      }
    }

    private bool AcceptEncodingRewriteEnabled(IVssRequestContext deploymentContext) => deploymentContext.IsFeatureEnabled("MS.TF.Framework.Server.HttpModule.AcceptEncodingRewriteFeatureEnabled");

    private bool AcceptEncodingHeaderHasValueAny(HttpContext context) => (HeaderContentRewriteModule.Getter != null ? HeaderContentRewriteModule.Getter(context) : string.Empty) == "*";

    private static void CreateLinqExpressionGetter()
    {
      ParameterExpression parameterExpression1;
      MemberExpression memberExpression;
      ConstantExpression constantExpression1;
      HeaderContentRewriteModule.CreateCommonLinqExpressions().Deconstruct<ParameterExpression, MemberExpression, ConstantExpression>(out parameterExpression1, out memberExpression, out constantExpression1);
      ParameterExpression parameterExpression2 = parameterExpression1;
      MemberExpression instance = memberExpression;
      ConstantExpression constantExpression2 = constantExpression1;
      MethodInfo method = typeof (HttpWorkerRequest).GetMethod("GetKnownRequestHeader", new Type[1]
      {
        typeof (int)
      });
      HeaderContentRewriteModule.Getter = Expression.Lambda<Func<HttpContext, string>>((Expression) Expression.Call((Expression) instance, method, (Expression) constantExpression2), parameterExpression2).Compile();
    }

    private static void CreateLinqExpressionSetter()
    {
      ParameterExpression parameterExpression1;
      MemberExpression memberExpression1;
      ConstantExpression constantExpression1;
      HeaderContentRewriteModule.CreateCommonLinqExpressions().Deconstruct<ParameterExpression, MemberExpression, ConstantExpression>(out parameterExpression1, out memberExpression1, out constantExpression1);
      ParameterExpression parameterExpression2 = parameterExpression1;
      MemberExpression memberExpression2 = memberExpression1;
      ConstantExpression constantExpression2 = constantExpression1;
      Type type = typeof (HostingEnvironment).Assembly.GetType("System.Web.Hosting.IIS7WorkerRequest");
      HeaderContentRewriteModule.Setter = ((Expression<Action<HttpContext>>) (parameterExpression => Expression.Assign(Expression.ArrayAccess((Expression) Expression.Field((Expression) Expression.Convert((Expression) memberExpression2, type), "_knownRequestHeaders"), (Expression) constantExpression2), "gzip"))).Compile();
    }

    private static Tuple<ParameterExpression, MemberExpression, ConstantExpression> CreateCommonLinqExpressions()
    {
      ParameterExpression parameterExpression = Expression.Parameter(typeof (HttpContext));
      return Tuple.Create<ParameterExpression, MemberExpression, ConstantExpression>(parameterExpression, Expression.PropertyOrField((Expression) parameterExpression, "WorkerRequest"), Expression.Constant((object) 22));
    }
  }
}
