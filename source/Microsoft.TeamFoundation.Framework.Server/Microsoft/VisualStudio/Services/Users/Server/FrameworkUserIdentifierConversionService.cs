// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Users.Server.FrameworkUserIdentifierConversionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Users.Server
{
  internal class FrameworkUserIdentifierConversionService : 
    IUserIdentifierConversionService,
    IVssFrameworkService
  {
    private const string Area = "FrameworkStorageKeyService";
    private const string Layer = "UserLogic";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual Guid GetStorageKeyByDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      return this.GetStorageKeyByDescriptor(requestContext, descriptor, false);
    }

    public virtual Guid GetStorageKeyByDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      bool queryFromNameResolutionService)
    {
      requestContext.TraceEnter(1120000, "FrameworkStorageKeyService", "UserLogic", nameof (GetStorageKeyByDescriptor));
      try
      {
        if (descriptor == new SubjectDescriptor() || !descriptor.IsCuidBased())
        {
          requestContext.TraceDataConditionally(1122001, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Input subject descriptor is invalid", (Func<object>) (() => (object) new
          {
            descriptor = descriptor
          }), nameof (GetStorageKeyByDescriptor));
          return new Guid();
        }
        IUserIdentifierConversionCacheService service = requestContext.GetService<IUserIdentifierConversionCacheService>();
        Guid storageKey = service.GetStorageKey(requestContext, descriptor);
        if (storageKey != new Guid())
        {
          requestContext.TraceDataConditionally(1122002, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Retrieved storage key from cache", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            storageKey = storageKey
          }), nameof (GetStorageKeyByDescriptor));
          return storageKey;
        }
        try
        {
          using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
          {
            storageKey = this.GetClient(requestContext).GetStorageKeyAsync(descriptor).SyncResult<Guid>();
            requestContext.TraceDataConditionally(1122003, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Retrieved storage key from client", (Func<object>) (() => (object) new
            {
              descriptor = descriptor,
              storageKey = storageKey
            }), nameof (GetStorageKeyByDescriptor));
          }
        }
        catch (Exception ex)
        {
          if (queryFromNameResolutionService && (ex is ServiceOwnerNotFoundException || ex is VssResourceNotFoundException || ex is CircuitBreakerShortCircuitException || ex is TimeoutException))
          {
            requestContext.Trace(1120002, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "User Service installation not found or does not respond.");
            storageKey = this.GetNameResolutionServiceStorageKeyByDescriptor(requestContext, descriptor);
          }
          else
          {
            if (ex is UserDoesNotExistException)
              requestContext.Trace(1120003, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "User: {0} does not exist.", (object) descriptor);
            throw;
          }
        }
        if (storageKey != new Guid())
        {
          requestContext.TraceDataConditionally(1122004, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Added storage key to cache", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            storageKey = storageKey
          }), nameof (GetStorageKeyByDescriptor));
          service.Set(requestContext, descriptor, storageKey);
        }
        return storageKey;
      }
      finally
      {
        requestContext.TraceLeave(1120000, "FrameworkStorageKeyService", "UserLogic", nameof (GetStorageKeyByDescriptor));
      }
    }

    private Guid GetNameResolutionServiceStorageKeyByDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor)
    {
      INameResolutionService service = requestContext.GetService<INameResolutionService>();
      requestContext.Trace(1120004, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Getting storage key for user: '{0}' from Name Resolution Service.", (object) subjectDescriptor);
      IVssRequestContext requestContext1 = requestContext;
      string name = subjectDescriptor.ToString();
      Guid storageKey = (service.QueryEntry(requestContext1, "UserStorageKey", name) ?? throw new UserDoesNotExistException(string.Format("User: {0} does not exist.", (object) subjectDescriptor))).Value;
      requestContext.TraceDataConditionally(1120005, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Retrieved storage key from name resolution service", (Func<object>) (() => (object) new
      {
        subjectDescriptor = subjectDescriptor,
        storageKey = storageKey
      }), nameof (GetNameResolutionServiceStorageKeyByDescriptor));
      return storageKey;
    }

    public virtual SubjectDescriptor GetDescriptorByStorageKey(
      IVssRequestContext requestContext,
      Guid storageKey)
    {
      requestContext.TraceEnter(1120100, "FrameworkStorageKeyService", "UserLogic", nameof (GetDescriptorByStorageKey));
      try
      {
        if (storageKey == new Guid())
        {
          requestContext.TraceDataConditionally(1124001, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Input storage key is invalid", (Func<object>) (() => (object) new
          {
            storageKey = storageKey
          }), nameof (GetDescriptorByStorageKey));
          return new SubjectDescriptor();
        }
        IUserIdentifierConversionCacheService service = requestContext.GetService<IUserIdentifierConversionCacheService>();
        SubjectDescriptor descriptor = service.GetSubjectDescriptor(requestContext, storageKey);
        if (descriptor != new SubjectDescriptor())
        {
          requestContext.TraceDataConditionally(1124002, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Retrieved subject descriptor from cache", (Func<object>) (() => (object) new
          {
            storageKey = storageKey,
            descriptor = descriptor
          }), nameof (GetDescriptorByStorageKey));
          return descriptor;
        }
        descriptor = this.GetClient(requestContext).GetDescriptorAsync(storageKey).SyncResult<SubjectDescriptor>();
        requestContext.TraceDataConditionally(1124003, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Retrieved subject descriptor from client", (Func<object>) (() => (object) new
        {
          storageKey = storageKey,
          descriptor = descriptor
        }), nameof (GetDescriptorByStorageKey));
        if (descriptor != new SubjectDescriptor())
        {
          requestContext.TraceDataConditionally(1124004, TraceLevel.Verbose, "FrameworkStorageKeyService", "UserLogic", "Added subject descriptor to cache", (Func<object>) (() => (object) new
          {
            storageKey = storageKey,
            descriptor = descriptor
          }), nameof (GetDescriptorByStorageKey));
          service.Set(requestContext, descriptor, storageKey);
        }
        return descriptor;
      }
      finally
      {
        requestContext.TraceLeave(1120100, "FrameworkStorageKeyService", "UserLogic", nameof (GetDescriptorByStorageKey));
      }
    }

    protected FrameworkUserHttpClient GetClient(IVssRequestContext requestContext) => requestContext.ServiceInstanceType() == FrameworkServerConstants.UserExtensionPrincipal ? requestContext.GetClient<FrameworkUserHttpClient>(InstanceManagementHelper.UserSvcPrincipal) : requestContext.GetClient<FrameworkUserHttpClient>();
  }
}
