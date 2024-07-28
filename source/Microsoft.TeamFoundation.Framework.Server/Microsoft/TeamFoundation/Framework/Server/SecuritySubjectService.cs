// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuritySubjectService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuritySubjectService : 
    VssBaseService,
    IVssSecuritySubjectService,
    IVssFrameworkService
  {
    private ILockName m_lock;
    private IDictionary<Guid, SecuritySubjectEntry> m_data;
    private long m_sequenceId;
    private INotificationRegistration m_securitySubjectRegistration;
    private const string c_area = "Security";
    private const string c_layer = "SecuritySubjectService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(56490, "Security", nameof (SecuritySubjectService), nameof (ServiceStart));
      try
      {
        systemRequestContext.CheckDeploymentRequestContext();
        this.m_lock = this.CreateLockName(systemRequestContext, "sss");
        this.m_securitySubjectRegistration = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.SecuritySubjectEntriesChanged, new SqlNotificationCallback(this.OnSecuritySubjectEntriesChanged), false, false);
        this.LoadSecuritySubjectData(systemRequestContext);
      }
      catch (Exception ex)
      {
        systemRequestContext.TraceException(56491, "Security", nameof (SecuritySubjectService), ex);
        throw;
      }
      finally
      {
        systemRequestContext.TraceLeave(56492, "Security", nameof (SecuritySubjectService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.m_securitySubjectRegistration.Unregister(systemRequestContext);

    public SecuritySubjectEntry GetSecuritySubjectEntry(
      IVssRequestContext requestContext,
      Guid entryId)
    {
      requestContext.TraceEnter(56339, "Security", nameof (SecuritySubjectService), nameof (GetSecuritySubjectEntry));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        using (requestContext.AcquireReaderLock(this.m_lock))
        {
          SecuritySubjectEntry securitySubjectEntry = (SecuritySubjectEntry) null;
          return this.m_data.TryGetValue(entryId, out securitySubjectEntry) ? securitySubjectEntry : (SecuritySubjectEntry) null;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56340, "Security", nameof (SecuritySubjectService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56341, "Security", nameof (SecuritySubjectService), nameof (GetSecuritySubjectEntry));
      }
    }

    public IEnumerable<SecuritySubjectEntry> GetSecuritySubjectEntries(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56333, "Security", nameof (SecuritySubjectService), nameof (GetSecuritySubjectEntries));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        using (requestContext.AcquireReaderLock(this.m_lock))
          return (IEnumerable<SecuritySubjectEntry>) this.m_data?.Values ?? Enumerable.Empty<SecuritySubjectEntry>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56334, "Security", nameof (SecuritySubjectService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56335, "Security", nameof (SecuritySubjectService), nameof (GetSecuritySubjectEntries));
      }
    }

    private void OnSecuritySubjectEntriesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56336, "Security", nameof (SecuritySubjectService), nameof (OnSecuritySubjectEntriesChanged));
      try
      {
        this.LoadSecuritySubjectData(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56337, "Security", nameof (SecuritySubjectService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56338, "Security", nameof (SecuritySubjectService), nameof (OnSecuritySubjectEntriesChanged));
      }
    }

    private void LoadSecuritySubjectData(IVssRequestContext requestContext)
    {
      long sequenceId;
      IDictionary<Guid, SecuritySubjectEntry> dictionary = this.LoadSecuritySubjectDataHelper(requestContext, out sequenceId);
      using (requestContext.AcquireWriterLock(this.m_lock))
      {
        if (this.m_data != null && this.m_sequenceId >= sequenceId)
          return;
        this.m_data = dictionary;
        this.m_sequenceId = sequenceId;
      }
    }

    private IDictionary<Guid, SecuritySubjectEntry> LoadSecuritySubjectDataHelper(
      IVssRequestContext requestContext,
      out long sequenceId)
    {
      IReadOnlyList<SecuritySubjectComponent.SecuritySubjectEntry> securitySubjectEntryList;
      using (SecuritySubjectComponent component = requestContext.CreateComponent<SecuritySubjectComponent>())
        securitySubjectEntryList = component.QuerySecuritySubjectEntries(out sequenceId);
      Dictionary<Guid, SecuritySubjectEntry> dictionary = new Dictionary<Guid, SecuritySubjectEntry>();
      foreach (SecuritySubjectComponent.SecuritySubjectEntry entry in (IEnumerable<SecuritySubjectComponent.SecuritySubjectEntry>) securitySubjectEntryList)
      {
        SecuritySubjectEntry fromComponentType;
        try
        {
          fromComponentType = SecuritySubjectEntry.CreateFromComponentType(entry);
        }
        catch (ArgumentException ex)
        {
          requestContext.TraceException(56339, "Security", nameof (SecuritySubjectService), (Exception) ex);
          continue;
        }
        dictionary.Add(fromComponentType.Id, fromComponentType);
      }
      return (IDictionary<Guid, SecuritySubjectEntry>) dictionary;
    }
  }
}
