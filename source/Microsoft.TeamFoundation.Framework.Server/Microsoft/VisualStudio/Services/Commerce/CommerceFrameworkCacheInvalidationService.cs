// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceFrameworkCacheInvalidationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceFrameworkCacheInvalidationService : 
    ICommerceFrameworkCacheInvalidationService,
    IVssFrameworkService
  {
    private const string CacheInvalidationServiceMasterTopic = "Microsoft.VisualStudio.Services.Commerce.CommerceFrameworkCacheInvalidationService";
    private const string s_Area = "Commerce";
    private const string s_Layer = "CommerceFrameworkCacheInvalidationService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void SendFrameworkInvalidationNotification<T>(
      IVssRequestContext requestContext,
      string topic,
      T message,
      bool tryPublish = true)
    {
      try
      {
        requestContext.TraceEnter(5104220, "Commerce", nameof (CommerceFrameworkCacheInvalidationService), nameof (SendFrameworkInvalidationNotification));
        CommerceFrameworkCacheInvalidationMessage invalidationMessage = new CommerceFrameworkCacheInvalidationMessage()
        {
          CacheInvalidationTopic = topic,
          CacheInvalidationHandlerMessageType = typeof (T).AssemblyQualifiedName,
          CacheInvalidationHandlerMessage = message.Serialize<T>()
        };
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IMessageBusPublisherService service = vssRequestContext.GetService<IMessageBusPublisherService>();
        if (tryPublish)
          service.TryPublish(vssRequestContext, "Microsoft.VisualStudio.Services.Commerce.CommerceFrameworkCacheInvalidationService", new object[1]
          {
            (object) invalidationMessage
          });
        else
          service.Publish(vssRequestContext, "Microsoft.VisualStudio.Services.Commerce.CommerceFrameworkCacheInvalidationService", new object[1]
          {
            (object) invalidationMessage
          });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5104229, "Commerce", nameof (CommerceFrameworkCacheInvalidationService), ex);
      }
      finally
      {
        requestContext.TraceLeave(5104230, "Commerce", nameof (CommerceFrameworkCacheInvalidationService), nameof (SendFrameworkInvalidationNotification));
      }
    }
  }
}
