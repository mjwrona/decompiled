// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityTemplateService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecurityTemplateService : VssBaseService, IVssFrameworkService
  {
    private SecurityTemplateService.SecurityTemplateData m_data;
    private INotificationRegistration m_securityTemplateRegistration;
    private IDisposableReadOnlyList<ISecurityTemplateTokenGeneratorExtension> m_tokenGeneratorExtensions;
    private IDictionary<string, ISecurityTemplateTokenGeneratorExtension> m_tokenGeneratorExtensionsMap;
    private IDisposableReadOnlyList<ISecurityTemplateSubjectGeneratorExtension> m_subjectGeneratorExtensions;
    private IDictionary<string, ISecurityTemplateSubjectGeneratorExtension> m_subjectGeneratorExtensionsMap;
    private const string c_area = "Security";
    private const string c_layer = "SecurityTemplateService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56320, "Security", nameof (SecurityTemplateService), nameof (ServiceStart));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        this.m_tokenGeneratorExtensions = requestContext.GetExtensions<ISecurityTemplateTokenGeneratorExtension>();
        this.m_subjectGeneratorExtensions = requestContext.GetExtensions<ISecurityTemplateSubjectGeneratorExtension>();
        this.m_tokenGeneratorExtensionsMap = (IDictionary<string, ISecurityTemplateTokenGeneratorExtension>) new Dictionary<string, ISecurityTemplateTokenGeneratorExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        this.m_subjectGeneratorExtensionsMap = (IDictionary<string, ISecurityTemplateSubjectGeneratorExtension>) new Dictionary<string, ISecurityTemplateSubjectGeneratorExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (ISecurityTemplateTokenGeneratorExtension generatorExtension in (IEnumerable<ISecurityTemplateTokenGeneratorExtension>) this.m_tokenGeneratorExtensions)
        {
          string name = generatorExtension.Name;
          if (this.m_tokenGeneratorExtensionsMap.ContainsKey(name))
            throw new ArgumentException("Duplicate ISecurityTemplateTokenGeneratorExtension: " + name);
          this.m_tokenGeneratorExtensionsMap.Add(name, generatorExtension);
          string fullName = generatorExtension.GetType().FullName;
          if (this.m_tokenGeneratorExtensionsMap.ContainsKey(fullName))
            throw new ArgumentException("Duplicate ISecurityTemplateTokenGeneratorExtension: " + fullName);
          this.m_tokenGeneratorExtensionsMap.Add(fullName, generatorExtension);
        }
        foreach (ISecurityTemplateSubjectGeneratorExtension generatorExtension in (IEnumerable<ISecurityTemplateSubjectGeneratorExtension>) this.m_subjectGeneratorExtensions)
        {
          string name = generatorExtension.Name;
          if (this.m_subjectGeneratorExtensionsMap.ContainsKey(name))
            throw new ArgumentException("Duplicate ISecurityTemplateSubjectGeneratorExtension: " + name);
          this.m_subjectGeneratorExtensionsMap.Add(name, generatorExtension);
          string fullName = generatorExtension.GetType().FullName;
          if (this.m_subjectGeneratorExtensionsMap.ContainsKey(fullName))
            throw new ArgumentException("Duplicate ISecurityTemplateSubjectGeneratorExtension: " + fullName);
          this.m_subjectGeneratorExtensionsMap.Add(fullName, generatorExtension);
        }
        this.m_securityTemplateRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.SecurityTemplateEntriesChanged, new SqlNotificationCallback(this.OnSecurityTemplateEntriesChanged), false, false);
        Interlocked.CompareExchange<SecurityTemplateService.SecurityTemplateData>(ref this.m_data, SecurityTemplateService.LoadSecurityTemplateData(requestContext), (SecurityTemplateService.SecurityTemplateData) null);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56322, "Security", nameof (SecurityTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56321, "Security", nameof (SecurityTemplateService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_securityTemplateRegistration.Unregister(requestContext);
      if (this.m_tokenGeneratorExtensions != null)
      {
        this.m_tokenGeneratorExtensions.Dispose();
        this.m_tokenGeneratorExtensions = (IDisposableReadOnlyList<ISecurityTemplateTokenGeneratorExtension>) null;
      }
      if (this.m_subjectGeneratorExtensions == null)
        return;
      this.m_subjectGeneratorExtensions.Dispose();
      this.m_subjectGeneratorExtensions = (IDisposableReadOnlyList<ISecurityTemplateSubjectGeneratorExtension>) null;
    }

    public long SequenceId => this.m_data.SequenceId;

    public IEnumerable<SecurityTemplateEntry> GetSecurityTemplateEntries(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      Guid namespaceId,
      Guid aclStoreId,
      out long sequenceId)
    {
      requestContext.TraceEnter(56323, "Security", nameof (SecurityTemplateService), nameof (GetSecurityTemplateEntries));
      try
      {
        SecurityTemplateService.SecurityTemplateData data = this.m_data;
        sequenceId = data.SequenceId;
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && hostType != TeamFoundationHostType.ProjectCollection)
          return SecurityTemplateService.GetSecurityTemplateEntriesHelper(data, new SecurityTemplateService.TemplateKey(TeamFoundationHostType.Deployment, namespaceId, aclStoreId)).Concat<SecurityTemplateEntry>(SecurityTemplateService.GetSecurityTemplateEntriesHelper(data, new SecurityTemplateService.TemplateKey(TeamFoundationHostType.Application, namespaceId, aclStoreId)));
        hostType = this.GetEffectiveHostType(hostType);
        return SecurityTemplateService.GetSecurityTemplateEntriesHelper(data, new SecurityTemplateService.TemplateKey(hostType, namespaceId, aclStoreId));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56325, "Security", nameof (SecurityTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56324, "Security", nameof (SecurityTemplateService), nameof (GetSecurityTemplateEntries));
      }
    }

    private static IEnumerable<SecurityTemplateEntry> GetSecurityTemplateEntriesHelper(
      SecurityTemplateService.SecurityTemplateData data,
      SecurityTemplateService.TemplateKey templateKey)
    {
      IReadOnlyList<SecurityTemplateEntry> securityTemplateEntryList;
      return !data.Map.TryGetValue(templateKey, out securityTemplateEntryList) ? Enumerable.Empty<SecurityTemplateEntry>() : (IEnumerable<SecurityTemplateEntry>) securityTemplateEntryList;
    }

    private TeamFoundationHostType GetEffectiveHostType(TeamFoundationHostType hostType)
    {
      if ((hostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        return TeamFoundationHostType.Deployment;
      ArgumentUtility.CheckForMultipleBits((int) hostType, nameof (hostType));
      return hostType;
    }

    public ISecurityTemplateTokenGeneratorExtension GetTokenGeneratorExtension(
      TeamFoundationHostType hostType,
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      hostType = this.GetEffectiveHostType(hostType);
      ISecurityTemplateTokenGeneratorExtension generatorExtension;
      if (!this.m_tokenGeneratorExtensionsMap.TryGetValue(extensionName, out generatorExtension) || (hostType & generatorExtension.SupportedHostTypes) != hostType)
        generatorExtension = (ISecurityTemplateTokenGeneratorExtension) null;
      return generatorExtension;
    }

    public ISecurityTemplateSubjectGeneratorExtension GetSubjectGeneratorExtension(
      string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      ISecurityTemplateSubjectGeneratorExtension generatorExtension;
      if (!this.m_subjectGeneratorExtensionsMap.TryGetValue(extensionName, out generatorExtension))
        generatorExtension = (ISecurityTemplateSubjectGeneratorExtension) null;
      return generatorExtension;
    }

    private void OnSecurityTemplateEntriesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56350, "Security", nameof (SecurityTemplateService), nameof (OnSecurityTemplateEntriesChanged));
      try
      {
        Volatile.Write<SecurityTemplateService.SecurityTemplateData>(ref this.m_data, SecurityTemplateService.LoadSecurityTemplateData(requestContext));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56351, "Security", nameof (SecurityTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56352, "Security", nameof (SecurityTemplateService), nameof (OnSecurityTemplateEntriesChanged));
      }
    }

    private static SecurityTemplateService.SecurityTemplateData LoadSecurityTemplateData(
      IVssRequestContext requestContext)
    {
      long sequenceId;
      IReadOnlyList<SecurityTemplateComponent.SecurityTemplateEntry> componentEntries;
      using (SecurityTemplateComponent component = requestContext.CreateComponent<SecurityTemplateComponent>())
        componentEntries = component.QuerySecurityTemplateEntries(out sequenceId);
      Dictionary<SecurityTemplateService.TemplateKey, IReadOnlyList<SecurityTemplateEntry>> map = new Dictionary<SecurityTemplateService.TemplateKey, IReadOnlyList<SecurityTemplateEntry>>(SecurityTemplateService.TemplateKey.Comparer);
      foreach (IGrouping<SecurityTemplateService.TemplateKey, SecurityTemplateEntry> source in SecurityTemplateService.ConvertFromComponentEntries(requestContext, (IEnumerable<SecurityTemplateComponent.SecurityTemplateEntry>) componentEntries).GroupBy<SecurityTemplateEntry, SecurityTemplateService.TemplateKey>((Func<SecurityTemplateEntry, SecurityTemplateService.TemplateKey>) (s => (SecurityTemplateService.TemplateKey) s), SecurityTemplateService.TemplateKey.Comparer))
        map[source.Key] = (IReadOnlyList<SecurityTemplateEntry>) source.ToList<SecurityTemplateEntry>();
      return new SecurityTemplateService.SecurityTemplateData(sequenceId, map);
    }

    private static IEnumerable<SecurityTemplateEntry> ConvertFromComponentEntries(
      IVssRequestContext requestContext,
      IEnumerable<SecurityTemplateComponent.SecurityTemplateEntry> componentEntries)
    {
      foreach (SecurityTemplateComponent.SecurityTemplateEntry componentEntry in componentEntries)
      {
        SecurityTemplateEntry fromComponentType;
        try
        {
          fromComponentType = SecurityTemplateEntry.CreateFromComponentType(componentEntry);
        }
        catch (ArgumentException ex)
        {
          requestContext.TraceException(56353, "Security", nameof (SecurityTemplateService), (Exception) ex);
          continue;
        }
        yield return fromComponentType;
      }
    }

    private class SecurityTemplateData
    {
      public readonly long SequenceId;
      public readonly Dictionary<SecurityTemplateService.TemplateKey, IReadOnlyList<SecurityTemplateEntry>> Map;

      public SecurityTemplateData(
        long sequenceId,
        Dictionary<SecurityTemplateService.TemplateKey, IReadOnlyList<SecurityTemplateEntry>> map)
      {
        this.SequenceId = sequenceId;
        this.Map = map;
      }
    }

    private struct TemplateKey
    {
      public readonly TeamFoundationHostType HostType;
      public readonly Guid NamespaceId;
      public readonly Guid AclStoreId;
      public static readonly IEqualityComparer<SecurityTemplateService.TemplateKey> Comparer = (IEqualityComparer<SecurityTemplateService.TemplateKey>) new SecurityTemplateService.TemplateKey.TemplateKeyComparer();

      public TemplateKey(TeamFoundationHostType hostType, Guid namespaceId, Guid aclStoreId)
      {
        this.HostType = hostType;
        this.NamespaceId = namespaceId;
        this.AclStoreId = aclStoreId;
      }

      public static implicit operator SecurityTemplateService.TemplateKey(
        SecurityTemplateEntry entry)
      {
        return new SecurityTemplateService.TemplateKey(entry.HostType, entry.NamespaceId, entry.AclStoreId);
      }

      private class TemplateKeyComparer : IEqualityComparer<SecurityTemplateService.TemplateKey>
      {
        public bool Equals(
          SecurityTemplateService.TemplateKey x,
          SecurityTemplateService.TemplateKey y)
        {
          return x.HostType == y.HostType && x.NamespaceId == y.NamespaceId && x.AclStoreId == y.AclStoreId;
        }

        public int GetHashCode(SecurityTemplateService.TemplateKey obj)
        {
          int hostType = (int) obj.HostType;
          Guid guid = obj.NamespaceId;
          int hashCode1 = guid.GetHashCode();
          guid = obj.AclStoreId;
          int hashCode2 = guid.GetHashCode();
          int num = hashCode1 ^ hashCode2;
          return hostType + num;
        }
      }
    }
  }
}
