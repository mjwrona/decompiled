// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHostStateValidationDriver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationHostStateValidationDriver : IDisposable
  {
    private DisposableCollection<ITeamFoundationHostStateValidator> m_validators;

    internal TeamFoundationHostStateValidationDriver(string pluginDirectory) => this.LoadHostStateValidators(pluginDirectory);

    internal TeamFoundationHostReadyState QueryHostReadyState(
      IVssRequestContext requestContext,
      TeamFoundationServiceHostProperties hostProperties)
    {
      TeamFoundationTrace.Info("Querying host ready state. HostId: {0}", (object) hostProperties.Id);
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      if (this.m_validators.Count == 0)
      {
        TeamFoundationTrace.Warning("No host state validators are registered in the system.");
      }
      else
      {
        foreach (ITeamFoundationHostStateValidator validator in this.m_validators)
        {
          try
          {
            TeamFoundationHostReadyState hostReadyState = validator.GetHostReadyState(requestContext1, hostProperties);
            if (hostReadyState != null)
            {
              if (!hostReadyState.IsReady)
                return hostReadyState;
            }
            else
              TeamFoundationTrace.Warning("Host state validator '{0}' returned null. HostId: {1}.", (object) validator, (object) hostProperties.Id);
          }
          catch (Exception ex)
          {
            TeamFoundationTrace.Warning("Host state validator '{0}' threw an exception. HostId: {1}.", (object) validator, (object) hostProperties.Id);
            TeamFoundationTrace.TraceException(ex);
            return new TeamFoundationHostReadyState(false, FrameworkResources.HostNotReadyValidatorException((object) this.GetHostNameSafe(requestContext, hostProperties.Id), (object) ex.Message), ex.Message);
          }
        }
      }
      return TeamFoundationHostReadyState.Ready;
    }

    private string GetHostNameSafe(IVssRequestContext requestContext, Guid hostId)
    {
      if (requestContext.ServiceHost.InstanceId == hostId)
        return requestContext.ServiceHost.Name;
      try
      {
        return requestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext, hostId).Name;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        return hostId.ToString();
      }
    }

    private void LoadHostStateValidators(string pluginDirectory)
    {
      TeamFoundationTrace.Info("Loading service host state validators.");
      if (this.m_validators != null)
        this.m_validators.Dispose();
      IDisposableReadOnlyList<ITeamFoundationHostStateValidator> extensionsRaw = VssExtensionManagementService.GetExtensionsRaw<ITeamFoundationHostStateValidator>(pluginDirectory ?? string.Empty);
      List<ITeamFoundationHostStateValidator> elements = new List<ITeamFoundationHostStateValidator>((IEnumerable<ITeamFoundationHostStateValidator>) extensionsRaw);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      elements.Sort(TeamFoundationHostStateValidationDriver.\u003C\u003EO.\u003C0\u003E__CompareByPriority ?? (TeamFoundationHostStateValidationDriver.\u003C\u003EO.\u003C0\u003E__CompareByPriority = new Comparison<ITeamFoundationHostStateValidator>(TeamFoundationHostStateValidationDriver.CompareByPriority)));
      TeamFoundationTrace.Info("{0} service host state validators were found.", (object) elements.Count);
      string str = (string) null;
      try
      {
        str = Environment.GetEnvironmentVariable("TFS_SKIP_SERVICE_HOST_VALIDATORS");
      }
      catch (SecurityException ex)
      {
      }
      if (!string.IsNullOrEmpty(str))
      {
        TeamFoundationTrace.Info("TFS_SKIP_SERVICE_HOST_VALIDATORS is set to: {0}", (object) str);
        string[] array = str.Split(';');
        for (int index = elements.Count - 1; index >= 0; --index)
        {
          string fullName = elements[index].GetType().FullName;
          if (Array.IndexOf<string>(array, fullName) != -1)
          {
            ITeamFoundationHostStateValidator hostStateValidator = elements[index];
            TeamFoundationTrace.Info("Removing {0} validator.", (object) hostStateValidator.GetType().FullName);
            elements.RemoveAt(index);
            if (hostStateValidator is IDisposable)
              ((IDisposable) hostStateValidator).Dispose();
          }
        }
      }
      this.m_validators = new DisposableCollection<ITeamFoundationHostStateValidator>((IReadOnlyList<ITeamFoundationHostStateValidator>) elements, new IDisposable[1]
      {
        (IDisposable) extensionsRaw
      });
    }

    private static int CompareByPriority(
      ITeamFoundationHostStateValidator op1,
      ITeamFoundationHostStateValidator op2)
    {
      int num = (op1 is ITeamFoundationSystemHostStateValidator).CompareTo(op2 is ITeamFoundationSystemHostStateValidator);
      if (num != 0)
        return num;
      op1.Priority.CompareTo(op2.Priority);
      return num;
    }

    public void Dispose()
    {
      if (this.m_validators == null)
        return;
      this.m_validators.Dispose();
      this.m_validators = (DisposableCollection<ITeamFoundationHostStateValidator>) null;
    }
  }
}
