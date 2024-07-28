// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationStrongBoxServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class TeamFoundationStrongBoxServiceBase : 
    ITeamFoundationStrongBoxService,
    IVssFrameworkService,
    IEncryptionHelper
  {
    internal const int MAX_DRAWER_NAME_LEN = 255;
    private ILockName m_notificationLockName;
    private Dictionary<StrongBoxItemChangedCallback, TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackEntry> m_notifications;
    private INotificationRegistration m_strongBoxRegistration;
    private readonly RegistryQuery m_batchSizeQuery = new RegistryQuery("/Service/StrongBox/Constants/ReencryptionBatchSize");
    private static readonly Lazy<XmlSerializer> s_itemNameSerializer = new Lazy<XmlSerializer>((Func<XmlSerializer>) (() => new XmlSerializer(typeof (List<StrongBoxItemName>))));

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      try
      {
        this.SigningService = systemRequestContext.GetService<ITeamFoundationSigningService>();
        this.m_notificationLockName = systemRequestContext.ServiceHost.CreateLockName("notification");
        this.m_notifications = new Dictionary<StrongBoxItemChangedCallback, TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackEntry>((IEqualityComparer<StrongBoxItemChangedCallback>) DelegateComparer.Instance);
        this.m_strongBoxRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.StrongBoxItemChanged, new SqlNotificationHandler(this.OnItemChanged), false, false);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(109001, "StrongBox", "Service", ex);
        this.ServiceEndInternal(systemRequestContext);
        throw;
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_strongBoxRegistration.Unregister(systemRequestContext);
      this.ServiceEndInternal(systemRequestContext);
    }

    public void RegisterNotification(
      IVssRequestContext requestContext,
      StrongBoxItemChangedCallback callback,
      string drawerName,
      IEnumerable<string> filters)
    {
      ArgumentUtility.CheckForNull<StrongBoxItemChangedCallback>(callback, nameof (callback));
      ArgumentUtility.CheckStringForNullOrEmpty(drawerName, nameof (drawerName));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) filters, nameof (filters));
      requestContext.Trace(109170, TraceLevel.Verbose, "StrongBox", "Service", "Attempting Register Notification to {0}.{1} for {2} with {3} filters", (object) TeamFoundationStrongBoxServiceBase.GetReflectedTypeName(callback), (object) callback.Method.Name, (object) drawerName, (object) filters.Count<string>());
      StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, drawerName, false);
      if (drawerInfo == null)
      {
        requestContext.TraceAlways(109171, TraceLevel.Warning, "StrongBox", "Service", "Could not load drawer {0} in to register a notification on it", (object) drawerName);
      }
      else
      {
        using (requestContext.AcquireWriterLock(this.m_notificationLockName))
        {
          TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackEntry changedCallbackEntry;
          if (!this.m_notifications.TryGetValue(callback, out changedCallbackEntry))
            this.m_notifications[callback] = new TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackEntry(callback, drawerInfo.DrawerId, filters);
          else if (changedCallbackEntry.DrawerId.Equals(drawerInfo.DrawerId))
          {
            foreach (string filter in filters)
              changedCallbackEntry.Filters.Add(filter);
          }
          else
            requestContext.TraceAlways(109172, TraceLevel.Warning, "StrongBox", "Service", "Ignoring addition of callback {0}.{1} to drawer {2} because it is already on {3}.", (object) TeamFoundationStrongBoxServiceBase.GetReflectedTypeName(changedCallbackEntry.Callback), (object) changedCallbackEntry.Callback.Method.Name, (object) drawerInfo.DrawerId, (object) changedCallbackEntry.DrawerId);
        }
      }
    }

    public void UnregisterNotification(
      IVssRequestContext requestContext,
      StrongBoxItemChangedCallback callback)
    {
      ArgumentUtility.CheckForNull<StrongBoxItemChangedCallback>(callback, nameof (callback));
      using (requestContext.AcquireWriterLock(this.m_notificationLockName))
      {
        if (this.m_notifications.Remove(callback))
          return;
        requestContext.Trace(109173, TraceLevel.Warning, "StrongBox", "Service", "Failed to remove notification registration for {0}.{1}", (object) TeamFoundationStrongBoxServiceBase.GetReflectedTypeName(callback), (object) callback.Method.Name);
      }
    }

    private void OnItemChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      if (args.Data == null)
        return;
      List<StrongBoxItemName> strongBoxItemNameList = (List<StrongBoxItemName>) TeamFoundationStrongBoxServiceBase.s_itemNameSerializer.Value.Deserialize((TextReader) new StringReader(args.Data));
      List<TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackInvocation> callbackInvocationList = new List<TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackInvocation>();
      StrongBoxItemCacheService service = requestContext.GetService<StrongBoxItemCacheService>();
      foreach (StrongBoxItemName strongBoxItemName in strongBoxItemNameList)
        service.InvalidateItem(requestContext, strongBoxItemName.DrawerId, strongBoxItemName.LookupKey);
      using (requestContext.AcquireReaderLock(this.m_notificationLockName))
      {
        requestContext.Trace(109008, TraceLevel.Verbose, "StrongBox", "Service", "OnItemChange invoked with {0} items, and {1} registered callbacks", (object) strongBoxItemNameList.Count, (object) this.m_notifications.Count);
        foreach (TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackEntry changedCallbackEntry in this.m_notifications.Values)
        {
          IEnumerable<StrongBoxItemName> strongBoxItemNames = changedCallbackEntry.FilteredItems((IEnumerable<StrongBoxItemName>) strongBoxItemNameList);
          if (!strongBoxItemNames.IsNullOrEmpty<StrongBoxItemName>())
            callbackInvocationList.Add(new TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackInvocation(changedCallbackEntry.Callback, strongBoxItemNames));
        }
      }
      using (new TraceWatch(requestContext, 109007, TraceLevel.Error, TimeSpan.FromSeconds(1.0), "StrongBox", "Service", "{0} StrongBoxItemChanged callbacks invoked for {1} items (ex: {2}.{3})", new object[4]
      {
        (object) callbackInvocationList.Count,
        (object) strongBoxItemNameList.Count,
        (object) strongBoxItemNameList.FirstOrDefault<StrongBoxItemName>().DrawerId,
        (object) (strongBoxItemNameList.FirstOrDefault<StrongBoxItemName>().LookupKey ?? string.Empty)
      }))
      {
        foreach (TeamFoundationStrongBoxServiceBase.StrongBoxItemChangedCallbackInvocation callbackInvocation in callbackInvocationList)
        {
          requestContext.Trace(109002, TraceLevel.Verbose, "StrongBox", "Service", "OnStrongBoxItemChanged invocation started. ");
          using (new TraceWatch(requestContext, 109003, TraceLevel.Error, TimeSpan.FromMilliseconds(500.0), "StrongBox", "Service", "Callback {0}.{1}", new object[2]
          {
            (object) TeamFoundationStrongBoxServiceBase.GetReflectedTypeName(callbackInvocation.Callback),
            (object) callbackInvocation.Callback.Method.Name
          }))
          {
            try
            {
              requestContext.Trace(109004, TraceLevel.Verbose, "StrongBox", "Service", "OnStrongBoxItemChanged is delivering a real set of filtered entries to {0}.{1}", (object) TeamFoundationStrongBoxServiceBase.GetReflectedTypeName(callbackInvocation.Callback), (object) callbackInvocation.Callback.Method.Name);
              callbackInvocation.Callback(requestContext, callbackInvocation.Items);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(109005, "StrongBox", "Service", ex);
            }
          }
        }
      }
    }

    private void ServiceEndInternal(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      this.SigningService = (ITeamFoundationSigningService) null;
    }

    public Guid CreateDrawer(IVssRequestContext requestContext, string name)
    {
      requestContext.TraceEnter(109006, "StrongBox", "Service", nameof (CreateDrawer));
      try
      {
        Guid drawerId = Guid.NewGuid();
        Guid defaultSigningKey = this.GetDefaultSigningKey(requestContext, drawerId, name);
        this.CreateDrawerWithExplicitSigningKeyImpl(requestContext, name, drawerId, defaultSigningKey);
        return drawerId;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109011, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109012, "StrongBox", "Service", nameof (CreateDrawer));
      }
    }

    public Guid CreateDrawerWithExplicitSigningKey(
      IVssRequestContext requestContext,
      string name,
      Guid signingKeyId)
    {
      Guid drawerId = Guid.NewGuid();
      this.CreateDrawerWithExplicitSigningKeyImpl(requestContext, name, drawerId, signingKeyId);
      return drawerId;
    }

    public Guid UnlockDrawer(IVssRequestContext requestContext, string name, bool throwOnFailure)
    {
      requestContext.Trace(109301, TraceLevel.Info, "StrongBox", "Service", "UnlockDrawer drawerName:{0}", (object) name);
      try
      {
        StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, name, throwOnFailure);
        if (drawerInfo != null)
          this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerInfo.DrawerId), 32);
        return drawerInfo != null ? drawerInfo.DrawerId : Guid.Empty;
      }
      catch (StrongBoxDrawerNotFoundException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109014, "StrongBox", "Service", ex);
        throw;
      }
    }

    public Guid UnlockOrCreateDrawer(IVssRequestContext requestContext, string name)
    {
      Guid drawer = this.UnlockDrawer(requestContext, name, false);
      if (drawer == Guid.Empty)
      {
        try
        {
          drawer = this.CreateDrawer(requestContext, name);
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          drawer = this.UnlockDrawer(requestContext, name, true);
        }
      }
      return drawer;
    }

    public void DeleteDrawer(IVssRequestContext requestContext, Guid drawerId)
    {
      requestContext.TraceEnter(109016, "StrongBox", "Service", nameof (DeleteDrawer));
      try
      {
        this.CheckPermission(requestContext, FrameworkSecurity.StrongBoxSecurityNamespaceRootToken, 2);
        this.DeleteDrawerAndContents(requestContext, drawerId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109017, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109018, "StrongBox", "Service", nameof (DeleteDrawer));
      }
    }

    public StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey)
    {
      return this.GetItemInfo(requestContext, drawerName, lookupKey, true);
    }

    public StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      string drawerName,
      string lookupKey,
      bool throwIfNotFound)
    {
      Guid drawerId = this.UnlockDrawer(requestContext, drawerName, throwIfNotFound);
      if (!(drawerId != Guid.Empty))
        return (StrongBoxItemInfo) null;
      requestContext.TraceEnter(109019, "StrongBox", "Service", nameof (GetItemInfo));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        return this.GetItemInfoInternal(requestContext, drawerId, lookupKey, throwIfNotFound);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109020, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109021, "StrongBox", "Service", nameof (GetItemInfo));
      }
    }

    public StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey)
    {
      return this.GetItemInfo(requestContext, drawerId, lookupKey, true);
    }

    public StrongBoxItemInfo GetItemInfo(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      bool throwIfNotFound)
    {
      requestContext.TraceEnter(109022, "StrongBox", "Service", nameof (GetItemInfo));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        return this.GetItemInfoInternal(requestContext, drawerId, lookupKey, throwIfNotFound);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109023, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109024, "StrongBox", "Service", nameof (GetItemInfo));
      }
    }

    public List<StrongBoxItemInfo> GetDrawerContents(
      IVssRequestContext requestContext,
      Guid drawerId)
    {
      requestContext.TraceEnter(109025, "StrongBox", "Service", nameof (GetDrawerContents));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
          return component.ReadContents(drawerId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109026, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109027, "StrongBox", "Service", nameof (GetDrawerContents));
      }
    }

    public List<StrongBoxItemInfo> GetDrawerContentsContainingPartialLookupKey(
      IVssRequestContext requestContext,
      Guid drawerId,
      string partialLookupKey)
    {
      requestContext.TraceEnter(109025, "StrongBox", "Service", nameof (GetDrawerContentsContainingPartialLookupKey));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
          return component.ReadContentsWithPartialLookupKey(drawerId, partialLookupKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109026, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109027, "StrongBox", "Service", nameof (GetDrawerContentsContainingPartialLookupKey));
      }
    }

    public bool IsDrawerEmpty(IVssRequestContext requestContext, Guid drawerId)
    {
      requestContext.TraceEnter(109120, "StrongBox", "Service", nameof (IsDrawerEmpty));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
          return component.IsDrawerEmpty(drawerId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109121, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109122, "StrongBox", "Service", nameof (IsDrawerEmpty));
      }
    }

    public string GetString(IVssRequestContext requestContext, Guid drawerId, string lookupKey)
    {
      requestContext.TraceEnter(109028, "StrongBox", "Service", nameof (GetString));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        StrongBoxItemInfo itemInfoInternal = this.GetItemInfoInternal(requestContext, drawerId, lookupKey, true);
        return Encoding.UTF8.GetString(this.GetBytes(requestContext, itemInfoInternal, false));
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109029, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109030, "StrongBox", "Service", nameof (GetString));
      }
    }

    public string GetString(IVssRequestContext requestContext, StrongBoxItemInfo item)
    {
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
      requestContext.TraceEnter(109031, "StrongBox", "Service", nameof (GetString));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 32);
        byte[] bytes = this.GetBytes(requestContext, item, false);
        try
        {
          return Encoding.UTF8.GetString(bytes);
        }
        finally
        {
          Array.Clear((Array) bytes, 0, bytes.Length);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109032, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109033, "StrongBox", "Service", nameof (GetString));
      }
    }

    public SecureString GetSecureString(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey)
    {
      requestContext.TraceEnter(109034, "StrongBox", "Service", nameof (GetSecureString));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        StrongBoxItemInfo itemInfoInternal = this.GetItemInfoInternal(requestContext, drawerId, lookupKey, true);
        byte[] bytes = this.GetBytes(requestContext, itemInfoInternal, false);
        try
        {
          return bytes.ToSecureString();
        }
        finally
        {
          Array.Clear((Array) bytes, 0, bytes.Length);
        }
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109035, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109036, "StrongBox", "Service", nameof (GetSecureString));
      }
    }

    public SecureString GetSecureString(IVssRequestContext requestContext, StrongBoxItemInfo item)
    {
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
      requestContext.TraceEnter(109037, "StrongBox", "Service", nameof (GetSecureString));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 32);
        return this.GetBytes(requestContext, item, false).ToSecureString();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109038, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109039, "StrongBox", "Service", nameof (GetSecureString));
      }
    }

    public Stream RetrieveFile(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      out long streamLength)
    {
      requestContext.TraceEnter(109040, "StrongBox", "Service", nameof (RetrieveFile));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        StrongBoxItemInfo itemInfoInternal1 = this.GetItemInfoInternal(requestContext, drawerId, lookupKey, true);
        if (itemInfoInternal1.ItemKind != StrongBoxItemKind.File)
          throw new UnexpectedItemKindException();
        long encryptedLength;
        try
        {
          return this.RetrieveFileInternal(requestContext, itemInfoInternal1, out encryptedLength, out streamLength);
        }
        catch (NotSupportedException ex)
        {
          requestContext.TraceException(109043, "StrongBox", "Service", (Exception) ex);
          requestContext.GetService<StrongBoxItemCacheService>().InvalidateItem(requestContext, drawerId, lookupKey);
          StrongBoxItemInfo itemInfoInternal2 = this.GetItemInfoInternal(requestContext, drawerId, lookupKey, true);
          return this.RetrieveFileInternal(requestContext, itemInfoInternal2, out encryptedLength, out streamLength);
        }
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109041, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109042, "StrongBox", "Service", nameof (RetrieveFile));
      }
    }

    public Stream RetrieveFile(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      out long streamLength)
    {
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
      requestContext.TraceEnter(109043, "StrongBox", "Service", nameof (RetrieveFile));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 32);
        return item.ItemKind == StrongBoxItemKind.File ? this.RetrieveFileInternal(requestContext, item, out long _, out streamLength) : throw new UnexpectedItemKindException();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109044, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109045, "StrongBox", "Service", nameof (RetrieveFile));
      }
    }

    public void CheckValueFitsInsideStrongBox(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      string value)
    {
    }

    public void AddString(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      string value)
    {
      IVssRequestContext requestContext1 = requestContext;
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo();
      strongBoxItemInfo.DrawerId = drawerId;
      strongBoxItemInfo.LookupKey = lookupKey;
      string str = value;
      this.AddString(requestContext1, strongBoxItemInfo, str);
    }

    public void AddString(IVssRequestContext requestContext, StrongBoxItemInfo item, string value) => this.AddStrings(requestContext, new List<Tuple<StrongBoxItemInfo, string>>()
    {
      new Tuple<StrongBoxItemInfo, string>(item, value)
    });

    public void AddStrings(
      IVssRequestContext requestContext,
      List<Tuple<StrongBoxItemInfo, string>> items)
    {
      requestContext.TraceEnter(109046, "StrongBox", "Service", nameof (AddStrings));
      List<byte[]> numArrayList = new List<byte[]>();
      try
      {
        List<Tuple<StrongBoxItemInfo, Stream>> items1 = new List<Tuple<StrongBoxItemInfo, Stream>>();
        foreach (Tuple<StrongBoxItemInfo, string> tuple in items)
        {
          StrongBoxItemInfo strongBoxItemInfo = tuple.Item1;
          string str = tuple.Item2;
          ArgumentUtility.CheckStringForNullOrEmpty(strongBoxItemInfo.LookupKey, "lookupKey");
          ArgumentUtility.CheckForNull<string>(str, "value");
          this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(strongBoxItemInfo.DrawerId), 16);
          byte[] bytes = Encoding.UTF8.GetBytes(str);
          numArrayList.Add(bytes);
          Stream stream = this.PrepareBytes(requestContext, strongBoxItemInfo, bytes);
          items1.Add(new Tuple<StrongBoxItemInfo, Stream>(strongBoxItemInfo, stream));
        }
        this.AddStream(requestContext, (IEnumerable<Tuple<StrongBoxItemInfo, Stream>>) items1);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109047, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        foreach (byte[] numArray in numArrayList)
          Array.Clear((Array) numArray, 0, numArray.Length);
        requestContext.TraceLeave(109048, "StrongBox", "Service", nameof (AddStrings));
      }
    }

    public void UploadFile(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      Stream content)
    {
      IVssRequestContext requestContext1 = requestContext;
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo();
      strongBoxItemInfo.DrawerId = drawerId;
      strongBoxItemInfo.LookupKey = lookupKey;
      Stream content1 = content;
      this.UploadFile(requestContext1, strongBoxItemInfo, content1, false);
    }

    public void UploadFile(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Stream content)
    {
      this.UploadFile(requestContext, item, content, false);
    }

    public void UploadSecureFile(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      Stream content)
    {
      IVssRequestContext requestContext1 = requestContext;
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo();
      strongBoxItemInfo.DrawerId = drawerId;
      strongBoxItemInfo.LookupKey = lookupKey;
      Stream content1 = content;
      this.UploadFile(requestContext1, strongBoxItemInfo, content1, true);
    }

    public X509Certificate2 RetrieveFileAsCertificate(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      bool exportable = false,
      bool expectPrivateKey = false)
    {
      return this.RetrieveFileAsCertificate(requestContext, item, exportable, expectPrivateKey, false);
    }

    public X509Certificate2 RetrieveFileAsCertificate(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      bool exportable = false,
      bool throwOnFailure = true)
    {
      StrongBoxItemInfo itemInfoInternal = this.GetItemInfoInternal(requestContext, drawerId, lookupKey, throwOnFailure);
      return itemInfoInternal == null ? (X509Certificate2) null : this.RetrieveFileAsCertificate(requestContext, itemInfoInternal, exportable, false, false);
    }

    public void DeleteItem(IVssRequestContext requestContext, Guid drawerId, string lookupKey)
    {
      requestContext.TraceEnter(109058, "StrongBox", "Service", nameof (DeleteItem));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 64);
        this.RemoveValue(requestContext, drawerId, lookupKey);
        requestContext.GetService<StrongBoxItemCacheService>().InvalidateItem(requestContext, drawerId, lookupKey);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109059, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109060, "StrongBox", "Service", nameof (DeleteItem));
      }
    }

    public bool ShouldRotate(IVssRequestContext requestContext, Guid drawerId, TimeSpan? maxKeyAge)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 32);
        StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, drawerId, true);
        SigningKeyType signingKeyType = this.SigningService.GetSigningKeyType(requestContext, drawerInfo.SigningKeyId);
        return signingKeyType != SigningKeyType.DeploymentCertificateSecured && (maxKeyAge.HasValue && drawerInfo.LastRotateDate < DateTime.UtcNow - maxKeyAge.Value || signingKeyType == SigningKeyType.RSAStored);
      }
      catch (Exception ex) when (!(ex is StrongBoxDrawerNotFoundException))
      {
        requestContext.TraceException(109108, "StrongBox", "Service", ex);
        throw;
      }
    }

    public void RotateSigningKey(
      IVssRequestContext requestContext,
      Guid drawerId,
      SigningKeyType keyType = SigningKeyType.Default)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      requestContext.TraceEnter(109110, "StrongBox", "Service", nameof (RotateSigningKey));
      try
      {
        HashSet<Guid> guidSet = new HashSet<Guid>();
        StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, drawerId, true);
        guidSet.Add(drawerInfo.SigningKeyId);
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerInfo.DrawerId), 128);
        if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.StrongBox.PreserveKeyType"))
        {
          SigningKeyType signingKeyType = this.SigningService.GetSigningKeyType(requestContext, drawerInfo.SigningKeyId);
          if (keyType == SigningKeyType.Default && signingKeyType == SigningKeyType.RsaSecuredByKeyEncryptionKey)
            keyType = signingKeyType;
        }
        Guid key = this.SigningService.GenerateKey(requestContext, keyType);
        this.UpdateDrawerSigningKey(requestContext, drawerId, key);
        if (requestContext.IsFeatureEnabled(FrameworkServerConstants.StrongBoxReentrantItemRotationFeatureName))
          return;
        foreach (StrongBoxItemInfo drawerContent in this.GetDrawerContents(requestContext, drawerId))
        {
          requestContext.Trace(109111, TraceLevel.Verbose, "StrongBox", "Service", "Re-encrypting strongbox item {0};{1}", (object) drawerContent.DrawerId, (object) drawerContent.LookupKey);
          Guid signingKeyId = drawerContent.SigningKeyId;
          if (this.ReencryptItem(requestContext, drawerContent, key))
            guidSet.Add(signingKeyId);
        }
      }
      catch (Exception ex) when (!(ex is StrongBoxDrawerNotFoundException))
      {
        requestContext.TraceException(109113, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109114, "StrongBox", "Service", nameof (RotateSigningKey));
      }
    }

    public void FinishSigningKeyRotation(IVssRequestContext requestContext, Guid drawerId)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      requestContext.TraceEnter(109143, "StrongBox", "Service", nameof (FinishSigningKeyRotation));
      try
      {
        HashSet<Guid> guidSet = new HashSet<Guid>();
        StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, drawerId, true);
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerInfo.DrawerId), 128);
        foreach (StrongBoxItemInfo strongBoxItemInfo in this.GetDrawerContents(requestContext, drawerInfo.DrawerId).Where<StrongBoxItemInfo>((Func<StrongBoxItemInfo, bool>) (item => item.SigningKeyId != drawerInfo.SigningKeyId)))
        {
          requestContext.Trace(109144, TraceLevel.Verbose, "StrongBox", "Service", "Re-encrypting strongbox item {0};{1}", (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey);
          Guid signingKeyId = strongBoxItemInfo.SigningKeyId;
          if (this.ReencryptItem(requestContext, strongBoxItemInfo, drawerInfo.SigningKeyId))
            guidSet.Add(signingKeyId);
        }
      }
      catch (Exception ex) when (!(ex is StrongBoxDrawerNotFoundException))
      {
        requestContext.TraceException(109145, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109146, "StrongBox", "Service", nameof (FinishSigningKeyRotation));
      }
    }

    public void FinishSigningKeyRotation2(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckSystemRequestContext();
      requestContext.TraceEnter(109160, "StrongBox", "Service", nameof (FinishSigningKeyRotation2));
      try
      {
        IList<Exception> exceptionList = (IList<Exception>) new List<Exception>();
        if (requestContext.IsFeatureEnabled("Microsoft.AzureDevOps.StrongBox.BatchedItemsReencryption"))
        {
          ReencryptResults reencryptResults = this.ReencryptItemsInBatches(requestContext);
          requestContext.Trace(109187, TraceLevel.Info, "StrongBox", "Service", string.Format("The {0} items were re-encrypted", (object) reencryptResults.SuccessCount));
          exceptionList.AddRange<Exception, IList<Exception>>((IEnumerable<Exception>) reencryptResults.Failures);
        }
        else
        {
          ICollection<StrongBoxItemInfo> itemsToReencrypt = this.GetItemsToReencrypt(requestContext);
          requestContext.Trace(109161, TraceLevel.Info, "StrongBox", "Service", string.Format("Discovered {0} items to re-encrypt", (object) itemsToReencrypt.Count));
          foreach (StrongBoxItemInfo strongBoxItemInfo in (IEnumerable<StrongBoxItemInfo>) itemsToReencrypt)
          {
            Guid signingKeyId = strongBoxItemInfo.SigningKeyId;
            try
            {
              StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, strongBoxItemInfo.DrawerId, true);
              this.ReencryptItem(requestContext, strongBoxItemInfo, drawerInfo.SigningKeyId);
            }
            catch (CryptographicException ex) when (ex.Message.Contains("1073741823"))
            {
              requestContext.TraceAlways(109163, TraceLevel.Error, "StrongBox", "Service", string.Format("Crypto error ({0}) while decrypting {1};{2}, secured by signing key {3}", (object) ex.HResult, (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey, (object) signingKeyId));
            }
            catch (CryptographicException ex) when (ex.Message.Contains("-1073741811"))
            {
              requestContext.TraceAlways(109168, TraceLevel.Error, "StrongBox", "Service", string.Format("Crypto error ({0}) while decrypting {1};{2}, secured by signing key {3}", (object) ex.HResult, (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey, (object) signingKeyId));
            }
            catch (StrongBoxDrawerNotFoundException ex)
            {
              requestContext.TraceCatch(109164, "StrongBox", "Service", (Exception) ex);
            }
            catch (StrongBoxItemNotFoundException ex)
            {
              requestContext.TraceCatch(109165, "StrongBox", "Service", (Exception) ex);
            }
            catch (Exception ex)
            {
              requestContext.TraceAlways(109167, TraceLevel.Error, "StrongBox", "Service", string.Format("{0} ({1}) while decrypting {2};{3}, secured by signing key {4}: {5}", (object) ex.GetType().Name, (object) ex.HResult, (object) strongBoxItemInfo.DrawerId, (object) strongBoxItemInfo.LookupKey, (object) signingKeyId, (object) ex));
              exceptionList.Add(ex);
            }
          }
        }
        if (exceptionList.Any<Exception>())
          throw new AggregateException((IEnumerable<Exception>) exceptionList);
      }
      finally
      {
        requestContext.TraceLeave(109167, "StrongBox", "Service", nameof (FinishSigningKeyRotation2));
      }
    }

    public SigningKeyType GetSigningKeyType(IVssRequestContext requestContext, Guid drawerId)
    {
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      try
      {
        StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, drawerId, true);
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerInfo.DrawerId), 32);
        return this.SigningService.GetSigningKeyType(requestContext, drawerInfo.SigningKeyId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109141, "StrongBox", "Service", ex);
        throw;
      }
    }

    public void RefreshCache(IVssRequestContext requestContext, string drawerName)
    {
      requestContext.CheckDeploymentRequestContext();
      Guid drawerId = this.UnlockDrawer(requestContext, drawerName, false);
      if (!(drawerId != Guid.Empty))
        return;
      foreach (StrongBoxItemInfo drawerContent in this.GetDrawerContents(requestContext, drawerId))
        this.RefreshCache(requestContext, drawerContent);
    }

    private void RefreshCache(IVssRequestContext requestContext, StrongBoxItemInfo itemInfo)
    {
      requestContext.CheckDeploymentRequestContext();
      ItemCacheEntry itemCacheEntry;
      if (!requestContext.GetService<StrongBoxItemCacheService>().TryGetValue(requestContext, new StrongBoxItemName()
      {
        DrawerId = itemInfo.DrawerId,
        LookupKey = itemInfo.LookupKey
      }, out itemCacheEntry))
        return;
      if (itemCacheEntry.ProtectedContent != null && itemCacheEntry.ProtectedContent.Length != 0)
        this.GetBytes(requestContext, itemInfo, true);
      else if (itemCacheEntry.ExportableCertificate != null)
      {
        this.RetrieveFileAsCertificate(requestContext, itemInfo, true, false, true);
      }
      else
      {
        if (itemCacheEntry.NonExportableCertificate == null)
          return;
        this.RetrieveFileAsCertificate(requestContext, itemInfo, false, false, true);
      }
    }

    protected ITeamFoundationSigningService SigningService { get; set; }

    void IEncryptionHelper.CheckPermission(
      IVssRequestContext requestContext,
      string token,
      int permissions)
    {
      this.CheckPermission(requestContext, token, permissions);
    }

    private void CheckPermission(IVssRequestContext requestContext, string token, int permissions)
    {
      if (requestContext.IsSystemContext)
        return;
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.StrongBoxNamespaceId).CheckPermission(requestContext, token, permissions, false);
    }

    private void SetDrawerPermissions(IVssRequestContext requestContext, Guid drawerId)
    {
      if (requestContext.IsSystemContext)
        return;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.StrongBoxNamespaceId);
      string tokenForDrawer = TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId);
      securityNamespace.SetPermissions(requestContext, tokenForDrawer, requestContext.UserContext, 240, 0, false);
      securityNamespace.SetInheritFlag(requestContext, tokenForDrawer, true);
    }

    internal static string GetTokenForDrawer(Guid drawerId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) FrameworkSecurity.StrongBoxSecurityNamespaceRootToken, (object) FrameworkSecurity.StrongBoxSecurityPathSeparator, (object) drawerId.ToString());

    private int GetStrongBoxVersion(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(requestContext, "StrongBox", "Default").Version;

    Stream IEncryptionHelper.PrepareBytes(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      byte[] plainBytes)
    {
      return this.PrepareBytes(requestContext, item, plainBytes);
    }

    private Stream PrepareBytes(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      byte[] plainBytes)
    {
      this.SetDefaultSigningKey(requestContext, item);
      item.Value = (string) null;
      return (Stream) new MemoryStream(plainBytes, false);
    }

    private void SetDefaultSigningKey(IVssRequestContext requestContext, StrongBoxItemInfo item)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
      StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, item.DrawerId, true);
      if (drawerInfo.SigningKeyId == Guid.Empty)
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          Guid databaseSigningKey = this.SigningService.GetDatabaseSigningKey(requestContext.Elevate());
          if (databaseSigningKey == FrameworkServerConstants.FrameworkSigningKey)
            item.SigningKeyId = item.DrawerId;
          else
            item.SigningKeyId = databaseSigningKey;
        }
        else
          item.SigningKeyId = item.DrawerId;
      }
      else
        item.SigningKeyId = drawerInfo.SigningKeyId;
    }

    protected Guid GetDefaultSigningKey(
      IVssRequestContext requestContext,
      Guid drawerId,
      string drawerName)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return drawerId;
      Guid databaseSigningKey = this.SigningService.GetDatabaseSigningKey(requestContext.Elevate());
      if (!(databaseSigningKey == FrameworkServerConstants.FrameworkSigningKey))
        return databaseSigningKey;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return drawerId;
      throw new InvalidOperationException("Deployment Level Strongbox Certificate not set up. Cannot continue.");
    }

    public StrongBoxDrawerInfo GetDrawerInfo(
      IVssRequestContext requestContext,
      string drawerName,
      bool throwOnFailure)
    {
      StrongBoxDrawerInfo drawerInfo = requestContext.GetService<StrongBoxDrawerCacheService>().GetDrawerInfo(requestContext, drawerName, (Func<string, StrongBoxDrawerInfo>) (name =>
      {
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
          return component.GetDrawerInfo(name);
      }));
      return !throwOnFailure || drawerInfo != null ? drawerInfo : throw new StrongBoxDrawerNotFoundException(drawerName);
    }

    private StrongBoxDrawerInfo GetDrawerInfo(
      IVssRequestContext requestContext,
      Guid drawerId,
      bool throwOnFailure)
    {
      StrongBoxDrawerInfo drawerInfo = requestContext.GetService<StrongBoxDrawerCacheService>().GetDrawerInfo(requestContext, drawerId, (Func<Guid, StrongBoxDrawerInfo>) (id =>
      {
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
          return component.GetDrawerInfo(id);
      }));
      return !throwOnFailure || drawerInfo != null ? drawerInfo : throw new StrongBoxDrawerNotFoundException(drawerId);
    }

    private static int GetFileId(StrongBoxItemInfo itemInfo) => Math.Max(itemInfo.FileId, itemInfo.SecureFileId);

    private StrongBoxItemInfo GetItemInfoInternal(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      bool throwOnFailure,
      bool forceUpdateCache = false)
    {
      return requestContext.GetService<StrongBoxItemCacheService>().GetItem(requestContext, drawerId, lookupKey, forceUpdateCache, (Func<StrongBoxItemInfo>) (() =>
      {
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          StrongBoxItemInfo itemInfo = component.GetItemInfo(drawerId, lookupKey);
          if (throwOnFailure && itemInfo == null)
          {
            requestContext.Trace(109091, TraceLevel.Verbose, "StrongBox", "Service", "drawerId={0} lookupKey={1}", (object) drawerId, (object) lookupKey);
            throw new StrongBoxItemNotFoundException(lookupKey);
          }
          return itemInfo;
        }
      }));
    }

    private byte[] ResolveCacheMissOfTheDecryptedBytes(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item)
    {
      if (item == null)
        return Array.Empty<byte>();
      if (item.Value == null)
        return this.RetrieveFileBytes(requestContext, item);
      if (string.IsNullOrEmpty(item.Value))
        return Array.Empty<byte>();
      try
      {
        return this.SigningService.Decrypt(requestContext.Elevate(), item.SigningKeyId, Convert.FromBase64String(item.Value), TeamFoundationStrongBoxServiceBase.GetSigningAlgorithm(item));
      }
      catch (Exception ex)
      {
        requestContext.Trace(987689616, TraceLevel.Error, "StrongBox", "Service", "Error encountered decrypting strongbox item drawerId={0}, lookupKey={1}, hresult={2}: {3}", (object) item.DrawerId, (object) item.LookupKey, (object) ex.HResult, (object) ex);
        throw;
      }
    }

    byte[] IEncryptionHelper.GetBytes(IVssRequestContext requestContext, StrongBoxItemInfo item) => this.GetBytes(requestContext, item, false);

    private byte[] GetBytes(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      bool forceUpdateCache = false)
    {
      StrongBoxItemCacheService service = requestContext.GetService<StrongBoxItemCacheService>();
      try
      {
        return service.GetDecryptedBytes(requestContext, item, forceUpdateCache, (Func<byte[]>) (() => this.ResolveCacheMissOfTheDecryptedBytes(requestContext, item)));
      }
      catch (Exception ex)
      {
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          StrongBoxItemInfo itemInfo = component.GetItemInfo(item.DrawerId, item.LookupKey);
          return itemInfo == null ? Array.Empty<byte>() : service.GetDecryptedBytes(requestContext, itemInfo, true, (Func<byte[]>) (() => this.ResolveCacheMissOfTheDecryptedBytes(requestContext, itemInfo)));
        }
      }
    }

    private X509Certificate2 RetrieveFileAsCertificate(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      bool exportable = false,
      bool expectPrivateKey = false,
      bool forceUpdateCache = false)
    {
      ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
      requestContext.TraceEnter(109056, "StrongBox", "Service", nameof (RetrieveFileAsCertificate));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 32);
        return requestContext.GetService<StrongBoxItemCacheService>().GetCertificate(requestContext, item, exportable, expectPrivateKey, forceUpdateCache, (Func<X509Certificate2>) (() =>
        {
          byte[] certBytes = this.RetrieveFileBytes(requestContext, item);
          try
          {
            return this.BytesToCertificate(requestContext, exportable, expectPrivateKey, certBytes);
          }
          finally
          {
            Array.Clear((Array) certBytes, 0, certBytes.Length);
          }
        }));
      }
      finally
      {
        requestContext.TraceLeave(109057, "StrongBox", "Service", nameof (RetrieveFileAsCertificate));
      }
    }

    void IEncryptionHelper.DeleteFileNoThrow(IVssRequestContext requestContext, int fileId) => this.DeleteFileNoThrow(requestContext, fileId);

    private void DeleteFileNoThrow(IVssRequestContext requestContext, int fileId)
    {
      requestContext.TraceEnter(109061, "StrongBox", "Service", nameof (DeleteFileNoThrow));
      try
      {
        TeamFoundationFileService service = requestContext.GetService<TeamFoundationFileService>();
        requestContext.TraceAlways(109062, TraceLevel.Info, "StrongBox", "Service", "Deleting file, with id {0} from file service.", (object) fileId);
        service.DeleteFile(requestContext, (long) fileId);
      }
      catch (Exception ex)
      {
        requestContext.Trace(109063, TraceLevel.Warning, "StrongBox", "Service", "Deletion of old file, with id {0}, from file service failed. The following exception is begin swallowed.");
        requestContext.TraceException(109064, "StrongBox", "Service", ex);
      }
      finally
      {
        requestContext.TraceLeave(109065, "StrongBox", "Service", nameof (DeleteFileNoThrow));
      }
    }

    private void UploadFile(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Stream content,
      bool useFileService)
    {
      requestContext.TraceEnter(109049, "StrongBox", "Service", nameof (UploadFile));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 16);
        this.SetDefaultSigningKey(requestContext, item);
        item.ItemKind = StrongBoxItemKind.File;
        this.AddStream(requestContext, item, content, useFileService);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109050, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109051, "StrongBox", "Service", nameof (UploadFile));
      }
    }

    private void AddStream(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Stream data,
      bool useFileService = false)
    {
      IVssRequestContext requestContext1 = requestContext;
      List<Tuple<StrongBoxItemInfo, Stream>> items = new List<Tuple<StrongBoxItemInfo, Stream>>();
      items.Add(new Tuple<StrongBoxItemInfo, Stream>(item, data));
      int num = useFileService ? 1 : 0;
      this.AddStream(requestContext1, (IEnumerable<Tuple<StrongBoxItemInfo, Stream>>) items, num != 0);
    }

    private void AddStream(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<StrongBoxItemInfo, Stream>> items,
      bool useFileService = false)
    {
      try
      {
        this.AddStreamImpl(requestContext, items, useFileService);
      }
      catch (StrongBoxSigningKeyRotatedException ex1)
      {
        requestContext.Trace(109147, TraceLevel.Warning, "StrongBox", "Service", "Drawer signing key rotated while adding item(s); retrying with new signing key id");
        foreach (Tuple<StrongBoxItemInfo, Stream> tuple in items)
          this.SetDefaultSigningKey(requestContext, tuple.Item1);
        try
        {
          this.AddStreamImpl(requestContext, items, useFileService);
        }
        catch (StrongBoxSigningKeyRotatedException ex2)
        {
          requestContext.TraceException(109147, "StrongBox", "Service", (Exception) ex2);
          throw;
        }
      }
    }

    private void AddStreamImpl(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<StrongBoxItemInfo, Stream>> items,
      bool useFileService = false)
    {
      requestContext.TraceEnter(109066, "StrongBox", "Service", nameof (AddStreamImpl));
      try
      {
        int strongBoxVersion = this.GetStrongBoxVersion(requestContext);
        if (strongBoxVersion < 5 || useFileService && strongBoxVersion < 8)
          throw new NotSupportedException("AddStreamImpl failed due to being on an unsupported component version");
        CachedRegistryService service1 = requestContext.GetService<CachedRegistryService>();
        Dictionary<string, StrongBoxItemInfo> dictionary = new Dictionary<string, StrongBoxItemInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        List<int> intList = new List<int>();
        List<Tuple<StrongBoxItemInfo, Stream>> tupleList = new List<Tuple<StrongBoxItemInfo, Stream>>();
        int num = useFileService ? service1.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxMaxFileContentLength, 10485760) : service1.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxMaxContentLength, 1048576);
        ArgumentUtility.CheckForNull<IEnumerable<Tuple<StrongBoxItemInfo, Stream>>>(items, nameof (items));
        Tuple<StrongBoxItemInfo, Stream> var = items.FirstOrDefault<Tuple<StrongBoxItemInfo, Stream>>();
        ArgumentUtility.CheckForNull<Tuple<StrongBoxItemInfo, Stream>>(var, "items.First");
        Guid drawerId = var.Item1.DrawerId;
        foreach (Tuple<StrongBoxItemInfo, Stream> tuple in items)
        {
          StrongBoxItemInfo itemInfo = tuple.Item1;
          Stream data = tuple.Item2;
          if (data.Length > (long) num)
            throw new ArgumentOutOfRangeException("data.Length");
          if (drawerId != itemInfo.DrawerId)
            throw new ArgumentException(FrameworkResources.AllItemsMustBeInSameDrawer());
          if (dictionary.ContainsKey(itemInfo.LookupKey))
            throw new ArgumentException(FrameworkResources.LookupKeysMustBeUnique((object) itemInfo.LookupKey));
          StrongBoxItemInfo itemInfoInternal = this.GetItemInfoInternal(requestContext, itemInfo.DrawerId, itemInfo.LookupKey, false);
          int fileId = TeamFoundationStrongBoxServiceBase.GetFileId(itemInfo);
          if (itemInfoInternal != null && itemInfoInternal.ItemKind == StrongBoxItemKind.File && fileId > 0)
          {
            intList.Add(fileId);
            requestContext.Trace(109067, TraceLevel.Info, "StrongBox", "Service", "Value for key {0} already exists with file id {1}. This will be deleted.", (object) itemInfo.LookupKey, (object) fileId);
          }
          using (Aes fileEncrypt = Aes.Create())
          {
            using (MemoryStream output = new MemoryStream())
            {
              using (BinaryWriter keyWriter = new BinaryWriter((Stream) output))
              {
                Stream content = this.EncryptContent(requestContext, itemInfo, fileEncrypt, data, keyWriter, useFileService);
                if (useFileService)
                {
                  try
                  {
                    TeamFoundationFileService service2 = requestContext.GetService<TeamFoundationFileService>();
                    itemInfo.SecureFileId = service2.UploadFile(requestContext, content, OwnerId.Generic, Guid.Empty);
                  }
                  catch (Exception ex)
                  {
                    requestContext.Trace(109070, TraceLevel.Error, "StrongBox", "Service", "Upload of data to FileService failed.");
                    throw;
                  }
                }
                else
                  itemInfo.EncryptedContent = (content as MemoryStream).ToArray();
              }
            }
          }
          dictionary.Add(itemInfo.LookupKey, itemInfo);
          requestContext.Trace(109103, TraceLevel.Verbose, "StrongBox", "Service", "Adding item drawerId={0} lookupKey={1}", (object) itemInfo.DrawerId, (object) itemInfo.LookupKey);
        }
        try
        {
          using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
            component.AddStrongBoxItems(drawerId, (IEnumerable<StrongBoxItemInfo>) dictionary.Values);
        }
        catch (StrongBoxSigningKeyRotatedException ex)
        {
          if (useFileService)
          {
            foreach (int fileId in dictionary.Values.Select<StrongBoxItemInfo, int>((Func<StrongBoxItemInfo, int>) (itemInfo => itemInfo.SecureFileId)))
              this.DeleteFileNoThrow(requestContext, fileId);
          }
          throw;
        }
        StrongBoxItemCacheService service3 = requestContext.GetService<StrongBoxItemCacheService>();
        foreach (StrongBoxItemInfo strongBoxItemInfo in dictionary.Values)
          service3.InvalidateItem(requestContext, strongBoxItemInfo.DrawerId, strongBoxItemInfo.LookupKey);
        foreach (int fileId in intList)
          this.DeleteFileNoThrow(requestContext, fileId);
        requestContext.Trace(109071, TraceLevel.Info, "StrongBox", "Service", "StrongBoxService AddStream End");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109072, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109073, "StrongBox", "Service", nameof (AddStreamImpl));
      }
    }

    Stream IEncryptionHelper.EncryptContent(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Aes fileEncrypt,
      Stream data,
      BinaryWriter keyWriter,
      bool useFileService)
    {
      return this.EncryptContent(requestContext, item, fileEncrypt, data, keyWriter, useFileService);
    }

    internal Stream EncryptContent(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Aes fileEncrypt,
      Stream data,
      BinaryWriter keyWriter,
      bool useFileService)
    {
      ICryptoTransform encryptor = fileEncrypt.CreateEncryptor();
      CryptoStream cryptoStream = new CryptoStream(data, encryptor, CryptoStreamMode.Read);
      byte[] buffer1;
      if (useFileService)
      {
        buffer1 = Array.Empty<byte>();
      }
      else
      {
        try
        {
          using (MemoryStream destination = new MemoryStream())
          {
            cryptoStream.CopyTo((Stream) destination);
            buffer1 = destination.ToArray();
          }
        }
        catch (Exception ex)
        {
          requestContext.Trace(109068, TraceLevel.Error, "StrongBox", "Service", "Encryption of byte stream failed.");
          throw;
        }
      }
      byte[] buffer2;
      try
      {
        SigningAlgorithm signingAlgorithm = TeamFoundationStrongBoxServiceBase.GetSigningAlgorithm(item);
        buffer2 = this.EncryptBySigningService(requestContext, item, fileEncrypt, signingAlgorithm);
      }
      catch (Exception ex)
      {
        requestContext.Trace(109069, TraceLevel.Error, "StrongBox", "Service", "Encryption of Symmetric Key failed. DrawerId = {0}; LookupKey = {1}", (object) item.DrawerId, (object) item.LookupKey);
        throw;
      }
      keyWriter.Write(buffer2.Length);
      keyWriter.Write(buffer2);
      keyWriter.Write(fileEncrypt.IV.Length);
      keyWriter.Write(fileEncrypt.IV);
      keyWriter.Write(data.Length);
      keyWriter.Write((long) buffer1.Length);
      keyWriter.Write(buffer1);
      keyWriter.Flush();
      if (!useFileService)
        return keyWriter.BaseStream;
      return (Stream) new CombinedStream(new Stream[2]
      {
        keyWriter.BaseStream,
        (Stream) cryptoStream
      });
    }

    protected virtual byte[] EncryptBySigningService(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Aes fileEncrypt,
      SigningAlgorithm algorithm)
    {
      return this.SigningService.Encrypt(requestContext.Elevate(), item.SigningKeyId, fileEncrypt.Key, algorithm);
    }

    internal Stream DecryptStream(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Stream dataStream,
      out long unencrytpedDataLength,
      out long encryptedDataLength)
    {
      byte[] iv;
      byte[] keyFromDataStream = TeamFoundationStrongBoxServiceBase.GetEncryptedKeyFromDataStream(dataStream, out iv, out unencrytpedDataLength, out encryptedDataLength);
      SigningAlgorithm signingAlgorithm = TeamFoundationStrongBoxServiceBase.GetSigningAlgorithm(item);
      byte[] symmetricKey;
      try
      {
        symmetricKey = this.SigningService.Decrypt(requestContext.Elevate(), item.SigningKeyId, keyFromDataStream, signingAlgorithm);
      }
      catch (Exception ex)
      {
        requestContext.Trace(547121990, TraceLevel.Error, "StrongBox", "Service", "Error decrypting strongbox item, drawerId={0}, lookupKey={1}, hresult={2}, signingKeyId={3}: {4}", (object) item.DrawerId, (object) item.LookupKey, (object) ex.HResult, (object) item.SigningKeyId, (object) ex);
        throw;
      }
      try
      {
        return TeamFoundationStrongBoxServiceBase.DecryptCryptoStream(dataStream, symmetricKey, iv);
      }
      catch (Exception ex)
      {
        requestContext.Trace(109074, TraceLevel.Error, "StrongBox", "Service", "CryptoStream creation failed.");
        throw;
      }
    }

    Stream IEncryptionHelper.RetrieveFileInternal(
      IVssRequestContext requestContext,
      StrongBoxItemInfo itemInfo,
      out long encryptedLength,
      out long unencryptedLength)
    {
      return this.RetrieveFileInternal(requestContext, itemInfo, out encryptedLength, out unencryptedLength);
    }

    private Stream RetrieveFileInternal(
      IVssRequestContext requestContext,
      StrongBoxItemInfo itemInfo,
      out long encryptedLength,
      out long unencryptedLength)
    {
      requestContext.TraceEnter(109075, "StrongBox", "Service", "RetrieveFile");
      try
      {
        int fileId1 = TeamFoundationStrongBoxServiceBase.GetFileId(itemInfo);
        Stream dataStream;
        if (fileId1 > 0)
        {
          TeamFoundationFileService service = requestContext.GetService<TeamFoundationFileService>();
          requestContext.Trace(109076, TraceLevel.Verbose, "StrongBox", "Service", "Retrieving Value from FileService with fileId = {0}", (object) fileId1);
          CompressionType compressionType = CompressionType.None;
          IVssRequestContext requestContext1 = requestContext;
          long fileId2 = (long) fileId1;
          byte[] numArray;
          ref byte[] local1 = ref numArray;
          long num;
          ref long local2 = ref num;
          ref CompressionType local3 = ref compressionType;
          dataStream = service.RetrieveFile(requestContext1, fileId2, false, out local1, out local2, out local3);
        }
        else if (itemInfo.EncryptedContent != null && itemInfo.EncryptedContent.Length != 0)
        {
          dataStream = (Stream) new MemoryStream(itemInfo.EncryptedContent, false);
        }
        else
        {
          encryptedLength = 0L;
          unencryptedLength = 0L;
          return (Stream) new MemoryStream();
        }
        return this.DecryptStream(requestContext, itemInfo, dataStream, out unencryptedLength, out encryptedLength);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(109077, TraceLevel.Error, "StrongBox", "Service", string.Format("DrawerId: {0} ; LookupKey: {1} ; SigningKeyId: {2} ; ", (object) itemInfo?.DrawerId, (object) itemInfo?.LookupKey, (object) itemInfo?.SigningKeyId) + "Exception:\n" + ex.ToString());
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109078, "StrongBox", "Service", "RetrieveFile");
      }
    }

    private X509Certificate2 BytesToCertificate(
      IVssRequestContext requestContext,
      bool exportable,
      bool expectPrivateKey,
      byte[] certBytes)
    {
      X509Certificate2 cert = (X509Certificate2) null;
      X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet;
      if (exportable)
        flags |= X509KeyStorageFlags.Exportable;
      Func<Exception, bool> canRetry = (Func<Exception, bool>) (e => e is CryptographicException cryptographicException && cryptographicException.HResult == -2146893808);
      Action traceRetry = (Action) (() => requestContext.Trace(109131, TraceLevel.Info, "StrongBox", "Service", string.Format("Sleeping for {0} ms", (object) 50)));
      if (!new RetryManager(5, TimeSpan.FromMilliseconds(50.0), (Action<Exception>) (e =>
      {
        if (!canRetry(e))
          return;
        requestContext.TraceAlways(109130, TraceLevel.Warning, "StrongBox", "Service", "Failed to import certificate due to Access Denied CryptographicException.");
        traceRetry();
      }), canRetry).Try((Func<bool>) (() =>
      {
        cert = new X509Certificate2(certBytes, (string) null, flags);
        int num = !expectPrivateKey ? 1 : (cert.HasPrivateKey ? 1 : 0);
        if (num != 0)
          return num != 0;
        traceRetry();
        return num != 0;
      })))
        throw new CryptographicException("Certificate with thumbprint " + cert.Thumbprint + " lacks expected private key");
      if (cert.HasPrivateKey)
      {
        try
        {
          AsymmetricAlgorithm rsaPrivateKey = (AsymmetricAlgorithm) cert.GetRSAPrivateKey();
          requestContext.Trace(109111, TraceLevel.Verbose, "StrongBox", "Service", "StrongBox cert with thumbprint {0}: Private key signature algorithm={1}, key exchange algorithm={2}, key size={3}", (object) cert.Thumbprint, (object) rsaPrivateKey.SignatureAlgorithm, (object) rsaPrivateKey.KeyExchangeAlgorithm, (object) rsaPrivateKey.KeySize);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(109112, "StrongBox", "Service", ex);
          throw;
        }
      }
      else
        requestContext.Trace(109113, TraceLevel.Verbose, "StrongBox", "Service", "StrongBox cert with thumbprint {0} doesn't have a private key.", (object) cert.Thumbprint);
      return cert;
    }

    private byte[] RetrieveFileBytes(IVssRequestContext requestContext, StrongBoxItemInfo itemInfo)
    {
      requestContext.TraceEnter(109079, "StrongBox", "Service", nameof (RetrieveFileBytes));
      try
      {
        long unencryptedLength;
        using (Stream cryptoStream = this.RetrieveFileInternal(requestContext, itemInfo, out long _, out unencryptedLength))
          return TeamFoundationStrongBoxServiceBase.GetBytesFromCryptoStream(cryptoStream, unencryptedLength);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109080, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109081, "StrongBox", "Service", nameof (RetrieveFileBytes));
      }
    }

    private void RemoveValue(IVssRequestContext requestContext, Guid drawerId, string lookupKey)
    {
      requestContext.TraceEnter(109082, "StrongBox", "Service", nameof (RemoveValue));
      try
      {
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          StrongBoxItemInfo itemInfo = component.GetItemInfo(drawerId, lookupKey);
          if (itemInfo != null)
          {
            requestContext.TraceAlways(109104, TraceLevel.Info, "StrongBox", "Service", "Deleting item drawerId={0} lookupKey={1}", (object) itemInfo.DrawerId, (object) itemInfo.LookupKey);
            component.RemoveItem(itemInfo.DrawerId, itemInfo.LookupKey);
            if (itemInfo.ItemKind != StrongBoxItemKind.File)
              return;
            int fileId = TeamFoundationStrongBoxServiceBase.GetFileId(itemInfo);
            if (fileId <= 0)
              return;
            this.DeleteFileNoThrow(requestContext, fileId);
          }
          else
            requestContext.Trace(109083, TraceLevel.Info, "StrongBox", "Service", "File Entry for key {0} has already been deleted.", (object) lookupKey);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109084, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109085, "StrongBox", "Service", nameof (RemoveValue));
      }
    }

    private void CreateDrawerWithExplicitSigningKeyImpl(
      IVssRequestContext requestContext,
      string name,
      Guid drawerId,
      Guid signingKeyId)
    {
      ArgumentUtility.CheckStringLength(name, nameof (name), (int) byte.MaxValue, 1);
      ArgumentUtility.CheckForEmptyGuid(drawerId, nameof (drawerId));
      ArgumentUtility.CheckForEmptyGuid(signingKeyId, nameof (signingKeyId));
      this.CheckPermission(requestContext, FrameworkSecurity.StrongBoxSecurityNamespaceRootToken, 1);
      requestContext.Trace(109009, TraceLevel.Verbose, "StrongBox", "Service", "StrongBoxService CreateDrawer: Start - DrawerName: {0}, DrawerId: {1}, SigningKeyId: {2}", (object) name, (object) drawerId, (object) signingKeyId);
      using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
      {
        try
        {
          component.CreateDrawer(name, drawerId, signingKeyId);
        }
        catch (SqlException ex) when (ex.Number == 2601)
        {
          throw new StrongBoxDrawerExistsException(name);
        }
      }
      this.SetDrawerPermissions(requestContext, drawerId);
      requestContext.Trace(109010, TraceLevel.Info, "StrongBox", "Service", "StrongBoxService CreateDrawer: End - DrawerId = {0}", (object) drawerId);
    }

    private void DeleteDrawerAndContents(IVssRequestContext requestContext, Guid drawerId)
    {
      requestContext.TraceEnter(109086, "StrongBox", "Service", nameof (DeleteDrawerAndContents));
      try
      {
        TeamFoundationFileService service = requestContext.GetService<TeamFoundationFileService>();
        List<StrongBoxItemInfo> strongBoxItemInfoList;
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          strongBoxItemInfoList = component.ReadContents(drawerId);
          requestContext.TraceAlways(109105, TraceLevel.Info, "StrongBox", "Service", "Deleting drawerId={0}, containing {1} strongbox items", (object) drawerId, (object) strongBoxItemInfoList.Count);
          component.DeleteDrawerAndContents(drawerId);
        }
        foreach (StrongBoxItemInfo itemInfo in strongBoxItemInfoList)
        {
          try
          {
            if (itemInfo.ItemKind == StrongBoxItemKind.File)
            {
              int fileId = TeamFoundationStrongBoxServiceBase.GetFileId(itemInfo);
              if (fileId > 0)
              {
                requestContext.TraceAlways(109106, TraceLevel.Info, "StrongBox", "Service", "Deleting file, with id {0} from file service.", (object) fileId);
                service.DeleteFile(requestContext, (long) fileId);
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.Trace(109087, TraceLevel.Warning, "StrongBox", "Service", "Deletion of old file, with id {0}, from file service failed. The following exception is begin swallowed.");
            requestContext.TraceException(109088, "StrongBox", "Service", ex);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109089, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109090, "StrongBox", "Service", nameof (DeleteDrawerAndContents));
      }
    }

    internal ICollection<StrongBoxItemInfo> GetItemsToReencrypt(
      IVssRequestContext requestContext,
      Guid? signingKeyId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckSystemRequestContext();
      using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        return component.QueryItemsToReencrypt(signingKeyId);
    }

    public ICollection<string> GetDrawerNames(
      IVssRequestContext requestContext,
      SigningKeyType? keyTypeFilter = null)
    {
      using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        return keyTypeFilter.HasValue ? (ICollection<string>) component.QueryDrawerNamesByType(keyTypeFilter.Value) : (ICollection<string>) component.QueryDrawerNames();
    }

    public void UpdateDrawerSigningKey(
      IVssRequestContext requestContext,
      Guid drawerId,
      Guid signingKeyId)
    {
      requestContext.TraceEnter(109092, "StrongBox", "Service", nameof (UpdateDrawerSigningKey));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(drawerId), 128);
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          if (component.Version < 10)
            return;
          component.UpdateDrawerSigningKey(drawerId, signingKeyId);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(109093, "StrongBox", "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(109094, "StrongBox", "Service", nameof (UpdateDrawerSigningKey));
      }
    }

    internal IEnumerable<StrongBoxItemInfo> GetLegacyItems(IVssRequestContext requestContext)
    {
      using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        return (IEnumerable<StrongBoxItemInfo>) component.QueryLegacyItems();
    }

    public bool ReencryptItem(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Guid signingKeyId)
    {
      if (item.ItemKind == StrongBoxItemKind.String)
      {
        byte[] bytes = this.GetBytes(requestContext, item, false);
        try
        {
          Stream stream = this.PrepareBytes(requestContext, item, bytes);
          item.SigningKeyId = signingKeyId;
          if (this.GetStrongBoxVersion(requestContext) >= 11)
            return this.ReplaceStringItem(requestContext, item, stream);
          this.AddStream(requestContext, item, stream);
          return true;
        }
        finally
        {
          Array.Clear((Array) bytes, 0, bytes.Length);
        }
      }
      else
      {
        long unencryptedLength;
        Stream input = this.RetrieveFileInternal(requestContext, item, out long _, out unencryptedLength);
        if (unencryptedLength > (long) int.MaxValue)
          throw new NotSupportedException();
        byte[] buffer;
        using (BinaryReader binaryReader = new BinaryReader(input))
          buffer = binaryReader.ReadBytes((int) unencryptedLength);
        try
        {
          item.SigningKeyId = signingKeyId;
          if (this.GetStrongBoxVersion(requestContext) >= 11)
            return this.ReplaceFileItem(requestContext, item, (Stream) new MemoryStream(buffer));
          this.UploadFile(requestContext, item, (Stream) new MemoryStream(buffer), item.SecureFileId > 0);
          return true;
        }
        finally
        {
          Array.Clear((Array) buffer, 0, buffer.Length);
        }
      }
    }

    public ReencryptResults ReencryptItemsInBatches(IVssRequestContext requestContext)
    {
      requestContext.Trace(109380, TraceLevel.Verbose, "StrongBox", "Service", "entering ReencryptItemsInBatches");
      requestContext.CheckSystemRequestContext();
      ReencryptResults reencryptResults = new ReencryptResults();
      this.GetStrongBoxVersion(requestContext);
      int num1 = 0;
      int num2 = 1;
      int num3 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in this.m_batchSizeQuery, true, 100);
      int batchSize = num3 <= 0 ? 100 : num3;
      TeamFoundationStrongBoxServiceBase.ReencryptionBehaviourProvider reencryptionBehaviourProvider = new TeamFoundationStrongBoxServiceBase.ReencryptionBehaviourProvider(this, requestContext);
      StrongBoxItemInfo lastAttemptedItem = (StrongBoxItemInfo) null;
      List<TeamFoundationStrongBoxServiceBase.ReencryptionData> items = new List<TeamFoundationStrongBoxServiceBase.ReencryptionData>();
      List<Exception> collection = new List<Exception>();
      int num4 = 0;
      ICollection<StrongBoxItemInfo> reencryptMultisets;
      do
      {
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
          reencryptMultisets = component.QueryItemsToReencryptMultisets(batchSize, lastAttemptedItem);
        requestContext.Trace(109379, TraceLevel.Verbose, "StrongBox", "Service", reencryptMultisets.Count > 0 ? string.Format("Retreived {0} item, last item lookupKey {1}, drawerId: {2}", (object) reencryptMultisets.Count, (object) reencryptMultisets.Last<StrongBoxItemInfo>().LookupKey, (object) reencryptMultisets.Last<StrongBoxItemInfo>().DrawerId) : string.Format("Retreived {0} item", (object) reencryptMultisets.Count));
        if (reencryptMultisets.Count != 0)
        {
          TeamFoundationStrongBoxServiceBase.BatchReencryptionResult reencryptionResult = this.ReencryptOneBatch(requestContext, reencryptMultisets, reencryptionBehaviourProvider);
          int reencryptedItemsCount = reencryptionResult.SuccessfullyReencryptedItemsCount;
          items.AddRange((IEnumerable<TeamFoundationStrongBoxServiceBase.ReencryptionData>) reencryptionResult.ProcessedItems);
          collection.AddRange((IEnumerable<Exception>) reencryptionResult.FoundExceptions);
          lastAttemptedItem = reencryptMultisets.Last<StrongBoxItemInfo>();
          bool failedWithError = false;
          try
          {
            using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
              component.ReencryptMultipleItems((IEnumerable<TeamFoundationStrongBoxServiceBase.ReencryptionData>) items);
            num1 += reencryptedItemsCount;
          }
          catch (Exception ex)
          {
            requestContext.TraceException(109383, TraceLevel.Error, "StrongBox", "Service", ex);
            failedWithError = true;
            collection.Add(ex);
          }
          foreach (TeamFoundationStrongBoxServiceBase.ReencryptionData reencryptionData in items)
          {
            StrongBoxItemInfo reencryptedItem = reencryptionData.ReencryptedItem;
            TeamFoundationStrongBoxServiceBase.IReencryptionStrategy behaviourForType = reencryptionBehaviourProvider.GetReencryptionBehaviourForType(reencryptedItem.ItemKind);
            try
            {
              behaviourForType.CleanUp(requestContext, reencryptedItem, reencryptionData.PreviousFileId, failedWithError);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(109384, TraceLevel.Error, "StrongBox", "Service", ex);
            }
          }
          num4 += reencryptMultisets.Count - reencryptedItemsCount;
          ++num2;
          items.Clear();
          if (num4 >= batchSize)
          {
            requestContext.TraceAlways(10916473, TraceLevel.Error, "StrongBox", "Service", "Reencryption error. More faulty items in DB than batch size");
            break;
          }
        }
        else
          break;
      }
      while (reencryptMultisets.Count >= batchSize);
      requestContext.Trace(109381, TraceLevel.Info, "StrongBox", "Service", string.Format("Reencrypted {0} items in {1} batches. Failed to reencrypt {2} items.", (object) num1, (object) (num2 - 1), (object) num4));
      reencryptResults.FailureCount = num4;
      reencryptResults.Failures.AddRange((IEnumerable<Exception>) collection);
      reencryptResults.SuccessCount = num1;
      return reencryptResults;
    }

    private TeamFoundationStrongBoxServiceBase.BatchReencryptionResult ReencryptOneBatch(
      IVssRequestContext requestContext,
      ICollection<StrongBoxItemInfo> itemsToReencrypt,
      TeamFoundationStrongBoxServiceBase.ReencryptionBehaviourProvider reencryptionBehaviourProvider)
    {
      List<TeamFoundationStrongBoxServiceBase.ReencryptionData> batchProcessedItems = new List<TeamFoundationStrongBoxServiceBase.ReencryptionData>();
      List<Exception> foundExceptions = new List<Exception>();
      StrongBoxItemInfo strongBoxItemInfo1 = itemsToReencrypt.First<StrongBoxItemInfo>();
      int reecryptedItemCountInBatch = 0;
      StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(requestContext, strongBoxItemInfo1.DrawerId, true);
      foreach (StrongBoxItemInfo strongBoxItemInfo2 in (IEnumerable<StrongBoxItemInfo>) itemsToReencrypt)
      {
        try
        {
          if (!strongBoxItemInfo2.DrawerId.Equals(drawerInfo.DrawerId))
            drawerInfo = this.GetDrawerInfo(requestContext, strongBoxItemInfo2.DrawerId, true);
          TeamFoundationStrongBoxServiceBase.ReencryptionData reencryptionData = reencryptionBehaviourProvider.GetReencryptionBehaviourForType(strongBoxItemInfo2.ItemKind).ReencryptItem(requestContext, strongBoxItemInfo2, drawerInfo.SigningKeyId);
          batchProcessedItems.Add(reencryptionData);
          ++reecryptedItemCountInBatch;
        }
        catch (CryptographicException ex) when (ex.Message.Contains("1073741823"))
        {
          requestContext.TraceAlways(109163, TraceLevel.Error, "StrongBox", "Service", string.Format("Crypto error ({0}) while decrypting {1};{2}, secured by signing key {3}", (object) ex.HResult, (object) strongBoxItemInfo2.DrawerId, (object) strongBoxItemInfo2.LookupKey, (object) strongBoxItemInfo2.SigningKeyId));
        }
        catch (CryptographicException ex) when (ex.Message.Contains("-1073741811"))
        {
          requestContext.TraceAlways(109168, TraceLevel.Error, "StrongBox", "Service", string.Format("Crypto error ({0}) while decrypting {1};{2}, secured by signing key {3}", (object) ex.HResult, (object) strongBoxItemInfo2.DrawerId, (object) strongBoxItemInfo2.LookupKey, (object) strongBoxItemInfo2.SigningKeyId));
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(109382, TraceLevel.Error, "StrongBox", "Service", string.Format("The reencryption failed for item: LookupKey:{0};DrawerId:{1}. Reason: {2}.", (object) strongBoxItemInfo2.LookupKey, (object) strongBoxItemInfo2.DrawerId, (object) ex.Message));
          foundExceptions.Add(ex);
        }
      }
      return new TeamFoundationStrongBoxServiceBase.BatchReencryptionResult(batchProcessedItems, foundExceptions, reecryptedItemCountInBatch);
    }

    internal bool ReplaceStringItem(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Stream rawData)
    {
      requestContext.TraceEnter(277103743, "StrongBox", "Service", nameof (ReplaceStringItem));
      try
      {
        int num = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxMaxContentLength, 1048576);
        ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
        if (rawData.Length > (long) num)
          throw new ArgumentOutOfRangeException("rawData.Length");
        byte[] encryptedContent = item.EncryptedContent;
        int secureFileId = item.SecureFileId;
        using (Aes fileEncrypt = Aes.Create())
        {
          using (BinaryWriter keyWriter = new BinaryWriter((Stream) new MemoryStream()))
          {
            MemoryStream memoryStream = this.EncryptContent(requestContext, item, fileEncrypt, rawData, keyWriter, false) as MemoryStream;
            item.EncryptedContent = memoryStream.ToArray();
          }
        }
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          if (!component.ReplaceItem(item, encryptedContent, secureFileId))
          {
            requestContext.TraceAlways(277103744, TraceLevel.Warning, "StrongBox", "Service", "Safe-write of encrypted content item {0} failed; this may be due to item update occurring during re-encryption", (object) item.LookupKey);
            return false;
          }
          requestContext.Trace(277103745, TraceLevel.Verbose, "StrongBox", "Service", "Safe-write of encrypted content item {0} succeeded", (object) item.LookupKey);
          return true;
        }
      }
      finally
      {
        requestContext.TraceLeave(277103746, "StrongBox", "Service", nameof (ReplaceStringItem));
      }
    }

    internal bool ReplaceFileItem(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      Stream rawData)
    {
      requestContext.TraceEnter(498440340, "StrongBox", "Service", nameof (ReplaceFileItem));
      try
      {
        this.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 16);
        CachedRegistryService service1 = requestContext.GetService<CachedRegistryService>();
        List<int> intList = new List<int>();
        IVssRequestContext requestContext1 = requestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) FrameworkServerConstants.StrongBoxMaxFileContentLength;
        int num = service1.GetValue<int>(requestContext1, in local, 10485760);
        if (rawData.Length > (long) num)
          throw new ArgumentOutOfRangeException("decryptedContent.Length");
        int secureFileId = item.SecureFileId;
        byte[] encryptedContent = item.EncryptedContent;
        using (Aes fileEncrypt = Aes.Create())
        {
          using (BinaryWriter keyWriter = new BinaryWriter((Stream) new MemoryStream()))
          {
            bool useFileService = secureFileId > 0;
            Stream content = this.EncryptContent(requestContext, item, fileEncrypt, rawData, keyWriter, useFileService);
            if (useFileService)
            {
              try
              {
                TeamFoundationFileService service2 = requestContext.GetService<TeamFoundationFileService>();
                item.SecureFileId = service2.UploadFile(requestContext, content, OwnerId.Generic, Guid.Empty);
              }
              catch (Exception ex)
              {
                requestContext.Trace(498440342, TraceLevel.Error, "StrongBox", "Service", "Upload of data to FileService failed.");
                throw;
              }
            }
            else
              item.EncryptedContent = (content as MemoryStream).ToArray();
          }
        }
        bool flag = false;
        using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        {
          if (!component.ReplaceItem(item, encryptedContent, secureFileId))
          {
            requestContext.TraceAlways(498440343, TraceLevel.Warning, "StrongBox", "Service", "Safe-write of encrypted content item {0} failed; this may be due to item update occurring during re-encryption", (object) item.LookupKey);
            intList.Add(item.SecureFileId);
          }
          else
          {
            requestContext.Trace(498440344, TraceLevel.Verbose, "StrongBox", "Service", "Safe-write of encrypted content item {0} succeeded", (object) item.LookupKey);
            intList.Add(secureFileId);
            flag = true;
          }
        }
        foreach (int fileId in intList)
        {
          if (fileId > 0)
            this.DeleteFileNoThrow(requestContext, fileId);
        }
        return flag;
      }
      finally
      {
        requestContext.TraceLeave(498440345, "StrongBox", "Service", nameof (ReplaceFileItem));
      }
    }

    internal static string GetDeploymentStringRaw(
      ISqlConnectionInfo configDbConnectionInfo,
      string drawerName,
      string lookupKey)
    {
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(configDbConnectionInfo, nameof (configDbConnectionInfo));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(drawerName, nameof (drawerName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(lookupKey, nameof (lookupKey));
      if (configDbConnectionInfo.InitialCatalog.IndexOf("Configuration", StringComparison.OrdinalIgnoreCase) < 0)
        throw new InvalidOperationException("TeamFoundationStrongBoxService.GetDeploymentStringRaw is supported only on configuration databases.");
      StrongBoxItemInfo strongBoxItemInfo = TeamFoundationStrongBoxServiceBase.GetDeploymentStrongBoxItemInfo(configDbConnectionInfo, drawerName, lookupKey);
      if (TeamFoundationStrongBoxServiceBase.GetFileId(strongBoxItemInfo) >= 0 || strongBoxItemInfo.EncryptedContent == null || strongBoxItemInfo.EncryptedContent.Length == 0)
        throw new NotImplementedException("TeamFoundationStrongBoxService.GetDeploymentStringRaw is currently implemented for fetching data where file service is not used");
      MemoryStream dataStream = new MemoryStream(strongBoxItemInfo.EncryptedContent, false);
      byte[] iv;
      long unencrytpedDataLength;
      byte[] keyFromDataStream = TeamFoundationStrongBoxServiceBase.GetEncryptedKeyFromDataStream((Stream) dataStream, out iv, out unencrytpedDataLength, out long _);
      SigningAlgorithm signingAlgorithm = TeamFoundationStrongBoxServiceBase.GetSigningAlgorithm(strongBoxItemInfo);
      using (Stream cryptoStream = TeamFoundationStrongBoxServiceBase.DecryptCryptoStream((Stream) dataStream, TeamFoundationSigningService.DecryptRaw(configDbConnectionInfo, strongBoxItemInfo.SigningKeyId, keyFromDataStream, signingAlgorithm), iv))
        return Encoding.UTF8.GetString(TeamFoundationStrongBoxServiceBase.GetBytesFromCryptoStream(cryptoStream, unencrytpedDataLength));
    }

    private static SigningAlgorithm GetSigningAlgorithm(StrongBoxItemInfo item) => item.SigningKeyId == item.DrawerId ? SigningAlgorithm.SHA1 : SigningAlgorithm.SHA256;

    private static StrongBoxItemInfo GetDeploymentStrongBoxItemInfo(
      ISqlConnectionInfo configDbConnectionInfo,
      string drawerName,
      string lookupKey)
    {
      using (StrongBoxComponent componentRaw = configDbConnectionInfo.CreateComponentRaw<StrongBoxComponent>())
      {
        componentRaw.PartitionId = DatabasePartitionConstants.DeploymentHostPartitionId;
        return componentRaw.GetItemInfo((componentRaw.GetDrawerInfo(drawerName) ?? throw new StrongBoxDrawerNotFoundException(drawerName)).DrawerId, lookupKey) ?? throw new StrongBoxItemNotFoundException(lookupKey);
      }
    }

    private static Stream DecryptCryptoStream(Stream dataStream, byte[] symmetricKey, byte[] iv)
    {
      Aes aes = Aes.Create();
      aes.Key = symmetricKey;
      aes.IV = iv;
      ICryptoTransform decryptor = aes.CreateDecryptor();
      return (Stream) new CryptoStream(dataStream, decryptor, CryptoStreamMode.Read);
    }

    private static byte[] GetBytesFromCryptoStream(Stream cryptoStream, long unencrytpedDataLength)
    {
      byte[] buffer = unencrytpedDataLength <= (long) int.MaxValue ? new byte[unencrytpedDataLength] : throw new ArgumentOutOfRangeException("stream.Length");
      cryptoStream.Read(buffer, 0, (int) unencrytpedDataLength);
      return buffer;
    }

    private static byte[] GetEncryptedKeyFromDataStream(
      Stream dataStream,
      out byte[] iv,
      out long unencrytpedDataLength,
      out long encryptedDataLength)
    {
      BinaryReader binaryReader = new BinaryReader(dataStream);
      int count1 = binaryReader.ReadInt32();
      byte[] keyFromDataStream = binaryReader.ReadBytes(count1);
      int count2 = binaryReader.ReadInt32();
      iv = binaryReader.ReadBytes(count2);
      unencrytpedDataLength = binaryReader.ReadInt64();
      encryptedDataLength = binaryReader.ReadInt64();
      return keyFromDataStream;
    }

    private static string GetReflectedTypeName(StrongBoxItemChangedCallback callback) => callback.Method.ReflectedType?.FullName ?? string.Empty;

    public void DeleteStrongBoxOrphans(IVssRequestContext requestContext)
    {
      int batchSize = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxDeleteOrphansBatchSize, 500);
      using (StrongBoxComponent component = requestContext.CreateComponent<StrongBoxComponent>())
        component.DeleteStrongBoxOrphans(batchSize);
    }

    private class StrongBoxItemChangedCallbackEntry
    {
      public StrongBoxItemChangedCallbackEntry(
        StrongBoxItemChangedCallback callback,
        Guid drawerId,
        IEnumerable<string> filters)
      {
        this.Callback = callback;
        this.DrawerId = drawerId;
        this.Filters = new HashSet<string>(filters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }

      public StrongBoxItemChangedCallback Callback { get; set; }

      public Guid DrawerId { get; set; }

      public HashSet<string> Filters { get; set; }

      public IEnumerable<StrongBoxItemName> FilteredItems(IEnumerable<StrongBoxItemName> inputItems)
      {
        foreach (StrongBoxItemName inputItem in inputItems)
        {
          StrongBoxItemName item = inputItem;
          if (item.DrawerId.Equals(this.DrawerId))
          {
            foreach (string filter in this.Filters)
            {
              if (Wildcard.Match(item.LookupKey, filter))
                yield return item;
            }
          }
          item = new StrongBoxItemName();
        }
      }
    }

    private class StrongBoxItemChangedCallbackInvocation
    {
      public StrongBoxItemChangedCallbackInvocation(
        StrongBoxItemChangedCallback callback,
        IEnumerable<StrongBoxItemName> items)
      {
        this.Callback = callback;
        this.Items = items;
      }

      public StrongBoxItemChangedCallback Callback { get; set; }

      public IEnumerable<StrongBoxItemName> Items { get; set; }
    }

    private class ReencryptionBehaviourProvider
    {
      private TeamFoundationStrongBoxServiceBase.IReencryptionStrategy stringReencryptionNew;
      private TeamFoundationStrongBoxServiceBase.IReencryptionStrategy fileReencryptionNew;

      public ReencryptionBehaviourProvider(
        TeamFoundationStrongBoxServiceBase strongBoxService,
        IVssRequestContext requestContext)
      {
        CachedRegistryService service1 = requestContext.GetService<CachedRegistryService>();
        TeamFoundationFileService service2 = requestContext.GetService<TeamFoundationFileService>();
        this.stringReencryptionNew = (TeamFoundationStrongBoxServiceBase.IReencryptionStrategy) new TeamFoundationStrongBoxServiceBase.StringItemReencryptionStrategy((IEncryptionHelper) strongBoxService, (IVssRegistryService) service1);
        this.fileReencryptionNew = (TeamFoundationStrongBoxServiceBase.IReencryptionStrategy) new TeamFoundationStrongBoxServiceBase.FileItemReencryptionStrategy((IEncryptionHelper) strongBoxService, (IVssRegistryService) service1, service2);
      }

      public TeamFoundationStrongBoxServiceBase.IReencryptionStrategy GetReencryptionBehaviourForType(
        StrongBoxItemKind itemType)
      {
        return itemType == StrongBoxItemKind.String ? this.stringReencryptionNew : this.fileReencryptionNew;
      }
    }

    internal class BatchReencryptionResult
    {
      internal BatchReencryptionResult(
        List<TeamFoundationStrongBoxServiceBase.ReencryptionData> batchProcessedItems,
        List<Exception> foundExceptions,
        int reecryptedItemCountInBatch)
      {
        this.ProcessedItems = batchProcessedItems;
        this.FoundExceptions = foundExceptions;
        this.SuccessfullyReencryptedItemsCount = reecryptedItemCountInBatch;
      }

      public List<TeamFoundationStrongBoxServiceBase.ReencryptionData> ProcessedItems { get; set; }

      public List<Exception> FoundExceptions { get; set; }

      public int SuccessfullyReencryptedItemsCount { get; set; }
    }

    public class ReencryptionData
    {
      internal ReencryptionData(
        StrongBoxItemInfo item,
        int previousFieldId,
        byte[] previousContent)
      {
        this.ReencryptedItem = item;
        this.PreviousFileId = previousFieldId;
        this.PreviousContent = previousContent;
      }

      public StrongBoxItemInfo ReencryptedItem { get; set; }

      public int PreviousFileId { get; set; }

      public byte[] PreviousContent { get; set; }
    }

    private interface IReencryptionStrategy
    {
      TeamFoundationStrongBoxServiceBase.ReencryptionData ReencryptItem(
        IVssRequestContext requestContext,
        StrongBoxItemInfo item,
        Guid signingKeyId);

      void CleanUp(
        IVssRequestContext requestContext,
        StrongBoxItemInfo item,
        int prevFileId,
        bool failedWithError);
    }

    internal class FileItemReencryptionStrategy : 
      TeamFoundationStrongBoxServiceBase.IReencryptionStrategy
    {
      private IEncryptionHelper service;
      private IVssRegistryService registryService;
      private TeamFoundationFileService fileService;

      public FileItemReencryptionStrategy(
        IEncryptionHelper strongBoxService,
        IVssRegistryService registryService,
        TeamFoundationFileService fileService)
      {
        this.service = strongBoxService;
        this.registryService = registryService;
        this.fileService = fileService;
      }

      public void CleanUp(
        IVssRequestContext requestContext,
        StrongBoxItemInfo item,
        int prevFileId,
        bool failedWithError)
      {
        if (failedWithError)
          this.service.DeleteFileNoThrow(requestContext, item.SecureFileId);
        else
          this.service.DeleteFileNoThrow(requestContext, prevFileId);
      }

      public TeamFoundationStrongBoxServiceBase.ReencryptionData ReencryptItem(
        IVssRequestContext requestContext,
        StrongBoxItemInfo item,
        Guid signingKeyId)
      {
        long unencryptedLength;
        Stream input = this.service.RetrieveFileInternal(requestContext, item, out long _, out unencryptedLength);
        if (unencryptedLength > (long) int.MaxValue)
          throw new NotSupportedException();
        byte[] buffer;
        using (BinaryReader binaryReader = new BinaryReader(input))
          buffer = binaryReader.ReadBytes((int) unencryptedLength);
        MemoryStream data = new MemoryStream(buffer);
        try
        {
          item.SigningKeyId = signingKeyId;
          requestContext.TraceEnter(498440340, "StrongBox", "Service", nameof (ReencryptItem));
          this.service.CheckPermission(requestContext, TeamFoundationStrongBoxServiceBase.GetTokenForDrawer(item.DrawerId), 16);
          int num = this.registryService.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxMaxFileContentLength, 10485760);
          if (data.Length > (long) num)
            throw new ArgumentOutOfRangeException("decryptedContent.Length");
          int secureFileId = item.SecureFileId;
          byte[] encryptedContent = item.EncryptedContent;
          using (Aes fileEncrypt = Aes.Create())
          {
            using (BinaryWriter keyWriter = new BinaryWriter((Stream) new MemoryStream()))
            {
              bool useFileService = secureFileId > 0;
              Stream content = this.service.EncryptContent(requestContext, item, fileEncrypt, (Stream) data, keyWriter, useFileService);
              if (useFileService)
              {
                try
                {
                  item.SecureFileId = this.fileService.UploadFile(requestContext, content, OwnerId.Generic, Guid.Empty);
                }
                catch (Exception ex)
                {
                  requestContext.Trace(498440342, TraceLevel.Error, "StrongBox", "Service", "Upload of data to FileService failed.");
                  throw;
                }
              }
              else
                item.EncryptedContent = (content as MemoryStream).ToArray();
            }
          }
          return new TeamFoundationStrongBoxServiceBase.ReencryptionData(item, secureFileId, encryptedContent);
        }
        finally
        {
          Array.Clear((Array) buffer, 0, buffer.Length);
          requestContext.TraceLeave(498440345, "StrongBox", "Service", nameof (ReencryptItem));
        }
      }
    }

    internal class StringItemReencryptionStrategy : 
      TeamFoundationStrongBoxServiceBase.IReencryptionStrategy
    {
      private IEncryptionHelper service;
      private IVssRegistryService registryService;

      public StringItemReencryptionStrategy(
        IEncryptionHelper strongBoxService,
        IVssRegistryService registryService)
      {
        this.service = strongBoxService;
        this.registryService = registryService ?? throw new ArgumentNullException(nameof (registryService));
      }

      public void CleanUp(
        IVssRequestContext requestContext,
        StrongBoxItemInfo into,
        int prevFieldId,
        bool failedWithError)
      {
      }

      public TeamFoundationStrongBoxServiceBase.ReencryptionData ReencryptItem(
        IVssRequestContext requestContext,
        StrongBoxItemInfo item,
        Guid signingKeyId)
      {
        byte[] bytes = this.service.GetBytes(requestContext, item);
        try
        {
          Stream data = this.service.PrepareBytes(requestContext, item, bytes);
          item.SigningKeyId = signingKeyId;
          requestContext.TraceEnter(277103743, "StrongBox", "Service", nameof (ReencryptItem));
          int num = this.registryService.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.StrongBoxMaxContentLength, 1048576);
          ArgumentUtility.CheckForNull<StrongBoxItemInfo>(item, nameof (item));
          if (data.Length > (long) num)
            throw new ArgumentOutOfRangeException("itemData.Length");
          int secureFileId = item.SecureFileId;
          byte[] encryptedContent = item.EncryptedContent;
          using (Aes fileEncrypt = Aes.Create())
          {
            using (BinaryWriter keyWriter = new BinaryWriter((Stream) new MemoryStream()))
            {
              MemoryStream memoryStream = this.service.EncryptContent(requestContext, item, fileEncrypt, data, keyWriter, false) as MemoryStream;
              item.EncryptedContent = memoryStream.ToArray();
            }
          }
          return new TeamFoundationStrongBoxServiceBase.ReencryptionData(item, secureFileId, encryptedContent);
        }
        finally
        {
          Array.Clear((Array) bytes, 0, bytes.Length);
          requestContext.TraceLeave(277103746, "StrongBox", "Service", nameof (ReencryptItem));
        }
      }
    }
  }
}
