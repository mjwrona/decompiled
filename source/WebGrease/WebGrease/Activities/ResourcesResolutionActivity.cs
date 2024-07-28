// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.ResourcesResolutionActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace WebGrease.Activities
{
  internal sealed class ResourcesResolutionActivity
  {
    private static readonly Dictionary<string, IDictionary<string, string>> EmptyResult = new Dictionary<string, IDictionary<string, string>>();
    private readonly IWebGreaseContext context;

    public ResourcesResolutionActivity(IWebGreaseContext context)
    {
      this.context = context;
      this.ResourceKeys = new List<string>();
    }

    internal string SourceDirectory { get; set; }

    internal string ResourceGroupKey { get; set; }

    internal string ApplicationDirectoryName { get; set; }

    internal string SiteDirectoryName { get; set; }

    internal string DestinationDirectory { get; set; }

    internal List<string> ResourceKeys { get; private set; }

    internal FileTypes FileType { get; set; }

    internal IDictionary<string, IDictionary<string, string>> GetMergedResources()
    {
      if (!this.HasSomethingToResolve())
        return (IDictionary<string, IDictionary<string, string>>) ResourcesResolutionActivity.EmptyResult;
      return this.context.SectionedAction(nameof (ResourcesResolutionActivity), this.FileType.ToString(), this.ResourceGroupKey).Execute<IDictionary<string, IDictionary<string, string>>>((Func<IDictionary<string, IDictionary<string, string>>>) (() =>
      {
        try
        {
          return ResourcesResolver.Factory(this.context, this.SourceDirectory, this.ResourceGroupKey, this.ApplicationDirectoryName, this.SiteDirectoryName, (IEnumerable<string>) this.ResourceKeys, this.DestinationDirectory).GetMergedResources();
        }
        catch (ResourceOverrideException ex)
        {
          throw new WorkflowException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResourcesResolutionActivity - {0} has more than one value assigned. Only one value per key name is allowed in libraries and features. Resource key overrides are allowed at the product level only.", new object[1]
          {
            (object) ex.TokenKey
          }), (Exception) ex);
        }
        catch (Exception ex)
        {
          throw new WorkflowException("ResourcesResolutionActivity - Error happened while executing the resolve resources activity", ex);
        }
      }));
    }

    internal void Execute()
    {
      if (!this.HasSomethingToResolve())
        return;
      this.context.SectionedAction(nameof (ResourcesResolutionActivity), this.FileType.ToString(), this.ResourceGroupKey).Execute((Action) (() =>
      {
        try
        {
          ResourcesResolver.Factory(this.context, this.SourceDirectory, this.ResourceGroupKey, this.ApplicationDirectoryName, this.SiteDirectoryName, (IEnumerable<string>) this.ResourceKeys, this.DestinationDirectory).ResolveHierarchy();
        }
        catch (ResourceOverrideException ex)
        {
          throw new WorkflowException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResourcesResolutionActivity - {0} has more than one value assigned. Only one value per key name is allowed in libraries and features. Resource key overrides are allowed at the product level only.", new object[1]
          {
            (object) ex.TokenKey
          }), (Exception) ex);
        }
        catch (Exception ex)
        {
          throw new WorkflowException("ResourcesResolutionActivity - Error happened while executing the resolve resources activity", ex);
        }
      }));
    }

    private bool HasSomethingToResolve() => this.ResourceKeys != null && this.ResourceKeys.Any<string>() && !string.IsNullOrWhiteSpace(this.SourceDirectory) && Directory.Exists(this.SourceDirectory);
  }
}
