// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ProductService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ProductService : IProductService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IDictionary<InstallationTarget, ProductFamily> QueryProductFamilies(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (extension == null)
        throw new ArgumentNullException(nameof (extension));
      requestContext.Trace(12061085, TraceLevel.Info, "gallery", nameof (QueryProductFamilies), "Querying for the Product family for extension {0}", (object) extension.ExtensionId);
      return this.QueryProductFamilies(requestContext, (IList<InstallationTarget>) extension.InstallationTargets);
    }

    public IDictionary<InstallationTarget, ProductFamily> QueryProductFamilies(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets)
    {
      ArgumentUtility.CheckForNull<IList<InstallationTarget>>(targets, nameof (targets));
      requestContext.Trace(12061085, TraceLevel.Info, "gallery", nameof (QueryProductFamilies), "Querying for the Product family for the given installation targets. No of targets to query are {0}", (object) targets.Count<InstallationTarget>());
      return this.InternalQueryProductFamilies(requestContext, targets);
    }

    public IDictionary<InstallationTarget, IList<Product>> QueryProducts(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (extension == null)
        throw new ArgumentNullException(nameof (extension));
      requestContext.Trace(12061085, TraceLevel.Info, "gallery", nameof (QueryProducts), "Querying for the Products for extension {0}", (object) extension.ExtensionId);
      return this.QueryProducts(requestContext, (IList<InstallationTarget>) extension.InstallationTargets);
    }

    public IDictionary<InstallationTarget, IList<Product>> QueryProducts(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets)
    {
      ArgumentUtility.CheckForNull<IList<InstallationTarget>>(targets, nameof (targets));
      requestContext.Trace(12061085, TraceLevel.Info, "gallery", "QueryProductFamilies", "Querying for the Product family for the given installation targets. No of targets to query are {0}", (object) targets.Count<InstallationTarget>());
      return this.InternalQueryProducts(requestContext, targets);
    }

    public IDictionary<InstallationTarget, IList<ProductRelease>> QueryReleases(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string productType)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      List<string> listByTargetType = this.GetContributionIdsListByTargetType(requestContext, service, productType);
      if (extension == null)
        throw new ArgumentNullException(nameof (extension));
      ArgumentUtility.CheckForNull<List<InstallationTarget>>(extension.InstallationTargets, nameof (extension));
      requestContext.Trace(12061085, TraceLevel.Info, "gallery", nameof (QueryReleases), "Quering for the releases for extension {0} under product type", (object) extension.ExtensionId, (object) productType);
      return this.InternalQueryReleases(requestContext, (IList<InstallationTarget>) extension.InstallationTargets, listByTargetType);
    }

    public IDictionary<InstallationTarget, IList<ProductRelease>> QueryReleases(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      List<string> contributionIdsListByType = this.GetContributionIdsListByType(requestContext, service, "ms.vss-gallery.product-release");
      ArgumentUtility.CheckForNull<IList<InstallationTarget>>(targets, nameof (targets));
      return this.InternalQueryReleases(requestContext, targets, contributionIdsListByType);
    }

    private IDictionary<InstallationTarget, IList<ProductRelease>> InternalQueryReleases(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets,
      List<string> targetReleaseIds)
    {
      Dictionary<InstallationTarget, IList<ProductRelease>> dictionary1 = new Dictionary<InstallationTarget, IList<ProductRelease>>();
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      Dictionary<string, Product> dictionary2 = new Dictionary<string, Product>();
      bool flag = GalleryUtil.IsVSInstallationTargets((IEnumerable<InstallationTarget>) targets);
      Version version1 = GalleryServerUtil.ConvertStringToVersion("17.0", false);
      foreach (InstallationTarget target in (IEnumerable<InstallationTarget>) targets)
      {
        List<ProductRelease> productReleaseList = new List<ProductRelease>();
        foreach (string targetReleaseId in targetReleaseIds)
        {
          Contribution contribution;
          ProductContributionAssociateData contributionsData;
          this.FetchContributionAssociateData(requestContext, service, targetReleaseId, out contribution, out contributionsData);
          if (this.IsTargetPresentInContributionNode(requestContext, contribution, contributionsData, target))
          {
            Version version2 = this.RetrieveProperty<Version>(contribution, "minVersion");
            Version version3 = this.RetrieveProperty<Version>(contribution, "maxVersion");
            if (target.MaxVersion != (Version) null && target.MinVersion != (Version) null && version2 != (Version) null && version3 != (Version) null && this.IsVersionWithinRange(target, version2, version3) && (!flag || !(version2 >= version1) || !"x86".Equals(target.ProductArchitecture)))
            {
              string name = this.RetrieveProperty<string>(contribution, "name");
              if (target != null && target.ProductArchitecture != null)
              {
                string productArchitecture = target.ProductArchitecture;
                switch (productArchitecture)
                {
                  case "amd64":
                    name = name + " (" + productArchitecture.ToLower() + ")";
                    break;
                  case "arm64":
                    name = name + " (" + productArchitecture[0].ToString().ToUpper() + productArchitecture.Substring(1) + ")";
                    break;
                }
              }
              Product product = (Product) null;
              if (!dictionary2.TryGetValue(contributionsData.Parent, out product))
              {
                string parentName;
                string parentId;
                if (this.QueryById(requestContext, contributionsData.Parent, out parentName, out parentId))
                {
                  product = new Product(parentId, parentName, (ProductFamily) null, (IList<string>) null);
                  dictionary2.Add(parentId, product);
                }
                else
                  requestContext.Trace(12061085, TraceLevel.Error, "gallery", "QueryReleases", "Release is not associated with any product | ReleaseId={0}", (object) contribution.Id);
              }
              ProductRelease productRelease = new ProductRelease(contribution.Id, name, version2, version3, product, contributionsData.Child);
              productReleaseList.Add(productRelease);
            }
          }
        }
        dictionary1.Add(target, (IList<ProductRelease>) productReleaseList);
      }
      return (IDictionary<InstallationTarget, IList<ProductRelease>>) dictionary1;
    }

    private IDictionary<InstallationTarget, IList<Product>> InternalQueryProducts(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets)
    {
      Dictionary<InstallationTarget, IList<Product>> dictionary1 = new Dictionary<InstallationTarget, IList<Product>>();
      Dictionary<string, ProductFamily> dictionary2 = new Dictionary<string, ProductFamily>();
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      List<string> contributionIdsListByType = this.GetContributionIdsListByType(requestContext, service, "ms.vss-gallery.product");
      foreach (InstallationTarget target in (IEnumerable<InstallationTarget>) targets)
      {
        List<Product> productList = new List<Product>();
        foreach (string contributionNodeId in contributionIdsListByType)
        {
          Contribution contribution;
          ProductContributionAssociateData contributionsData;
          this.FetchContributionAssociateData(requestContext, service, contributionNodeId, out contribution, out contributionsData);
          if (this.IsTargetPresentInContributionNode(requestContext, contribution, contributionsData, target))
          {
            string name = this.RetrieveProperty<string>(contribution, "name");
            ProductFamily parent;
            if (!dictionary2.TryGetValue(contributionsData.Parent, out parent))
            {
              string parentName;
              string parentId;
              if (this.QueryById(requestContext, contributionsData.Parent, out parentName, out parentId))
              {
                parent = new ProductFamily();
                parent.Id = parentId;
                parent.Name = parentName;
                dictionary2.Add(contributionsData.Parent, parent);
              }
              else
                requestContext.Trace(12061085, TraceLevel.Error, "gallery", nameof (InternalQueryProducts), "Product is not associated with any productFamily | Product={0}", (object) contribution.Id);
            }
            Product product = new Product(contribution.Id, name, parent, contributionsData.Child);
            productList.Add(product);
          }
        }
        dictionary1.Add(target, (IList<Product>) productList);
      }
      return (IDictionary<InstallationTarget, IList<Product>>) dictionary1;
    }

    private IDictionary<InstallationTarget, ProductFamily> InternalQueryProductFamilies(
      IVssRequestContext requestContext,
      IList<InstallationTarget> targets)
    {
      IDictionary<InstallationTarget, ProductFamily> dictionary = (IDictionary<InstallationTarget, ProductFamily>) new Dictionary<InstallationTarget, ProductFamily>();
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      List<string> contributionIdsListByType = this.GetContributionIdsListByType(requestContext, service, "ms.vss-gallery.product-family");
      foreach (InstallationTarget target in (IEnumerable<InstallationTarget>) targets)
      {
        ProductFamily productFamily = (ProductFamily) null;
        foreach (string contributionNodeId in contributionIdsListByType)
        {
          Contribution contribution;
          ProductContributionAssociateData contributionsData;
          this.FetchContributionAssociateData(requestContext, service, contributionNodeId, out contribution, out contributionsData);
          if (this.IsTargetPresentInContributionNode(requestContext, contribution, contributionsData, target))
          {
            string name = this.RetrieveProperty<string>(contribution, "name");
            string displayName = this.RetrieveProperty<string>(contribution, "displayName");
            productFamily = new ProductFamily(contribution.Id, name, displayName, contributionsData.Child);
          }
        }
        dictionary.Add(target, productFamily);
      }
      return dictionary;
    }

    internal void UpdateProductNodesAssociatedData(IVssRequestContext requestContext)
    {
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      IEnumerable<Contribution> source1 = service.QueryContributionsForType(requestContext, "ms.vss-gallery.product-family");
      requestContext.Trace(12061085, TraceLevel.Info, "gallery", nameof (UpdateProductNodesAssociatedData), "Updating the associate data for all the nodes. No of Product Families are {0}", (object) source1.Count<Contribution>());
      foreach (Contribution contribution1 in source1)
      {
        ProductContributionAssociateData contributionAssociationData1 = new ProductContributionAssociateData();
        foreach (Contribution contribution2 in service.QueryContributionsForTarget(requestContext, contribution1.Id))
        {
          ProductContributionAssociateData contributionAssociationData2 = new ProductContributionAssociateData();
          IEnumerable<Contribution> contributions = service.QueryContributionsForTarget(requestContext, contribution2.Id);
          string[] source2 = this.ReadInstallationTargetFromProperties(contribution2);
          foreach (Contribution contribution3 in contributions)
          {
            ProductContributionAssociateData contributionAssociationData3 = new ProductContributionAssociateData();
            string[] strArray = this.ReadInstallationTargetFromProperties(contribution3);
            foreach (Contribution contribution4 in service.QueryContributionsForTarget(requestContext, contribution3.Id))
            {
              ProductContributionAssociateData contributionAssociationData4 = new ProductContributionAssociateData();
              string[] InstallationTargets = this.ReadInstallationTargetFromProperties(contribution4);
              if (InstallationTargets != null)
              {
                this.AddInstallationTargetItems(ref contributionAssociationData1, (IList<string>) InstallationTargets);
                this.AddInstallationTargetItems(ref contributionAssociationData2, (IList<string>) InstallationTargets);
                this.AddInstallationTargetItems(ref contributionAssociationData3, (IList<string>) InstallationTargets);
              }
              contributionAssociationData3.Child.Add(contribution4.Id);
              contributionAssociationData4.Parent = contribution3.Id;
              if (source2 != null)
                this.AddInstallationTargetItems(ref contributionAssociationData4, (IList<string>) ((IEnumerable<string>) source2).ToList<string>());
              if (strArray != null)
                this.AddInstallationTargetItems(ref contributionAssociationData4, (IList<string>) ((IEnumerable<string>) strArray).ToList<string>());
              IContributionServiceWithData contributionServiceWithData = service;
              IVssRequestContext requestContext1 = requestContext;
              string id = contribution4.Id;
              List<Contribution> contributionList = new List<Contribution>();
              contributionList.Add(contribution4);
              ProductContributionAssociateData associatedData = contributionAssociationData4;
              contributionServiceWithData.Set(requestContext1, id, (IEnumerable<Contribution>) contributionList, (object) associatedData);
            }
            if (strArray != null)
            {
              this.AddInstallationTargetItems(ref contributionAssociationData1, (IList<string>) strArray);
              this.AddInstallationTargetItems(ref contributionAssociationData2, (IList<string>) strArray);
            }
            contributionAssociationData2.Child.Add(contribution3.Id);
            contributionAssociationData3.Parent = contribution2.Id;
            if (source2 != null)
              this.AddInstallationTargetItems(ref contributionAssociationData3, (IList<string>) ((IEnumerable<string>) source2).ToList<string>());
            IContributionServiceWithData contributionServiceWithData1 = service;
            IVssRequestContext requestContext2 = requestContext;
            string id1 = contribution3.Id;
            List<Contribution> contributionList1 = new List<Contribution>();
            contributionList1.Add(contribution3);
            ProductContributionAssociateData associatedData1 = contributionAssociationData3;
            contributionServiceWithData1.Set(requestContext2, id1, (IEnumerable<Contribution>) contributionList1, (object) associatedData1);
          }
          string[] InstallationTargets1 = this.ReadInstallationTargetFromProperties(contribution2);
          if (InstallationTargets1 != null)
            this.AddInstallationTargetItems(ref contributionAssociationData1, (IList<string>) InstallationTargets1);
          contributionAssociationData1.Child.Add(contribution2.Id);
          contributionAssociationData2.Parent = contribution1.Id;
          IContributionServiceWithData contributionServiceWithData2 = service;
          IVssRequestContext requestContext3 = requestContext;
          string id2 = contribution2.Id;
          List<Contribution> contributionList2 = new List<Contribution>();
          contributionList2.Add(contribution2);
          ProductContributionAssociateData associatedData2 = contributionAssociationData2;
          contributionServiceWithData2.Set(requestContext3, id2, (IEnumerable<Contribution>) contributionList2, (object) associatedData2);
        }
        contributionAssociationData1.Parent = string.Empty;
        IContributionServiceWithData contributionServiceWithData3 = service;
        IVssRequestContext requestContext4 = requestContext;
        string id3 = contribution1.Id;
        List<Contribution> contributionList3 = new List<Contribution>();
        contributionList3.Add(contribution1);
        ProductContributionAssociateData associatedData3 = contributionAssociationData1;
        contributionServiceWithData3.Set(requestContext4, id3, (IEnumerable<Contribution>) contributionList3, (object) associatedData3);
      }
    }

    private string[] ReadInstallationTargetFromProperties(Contribution contribution)
    {
      string[] strArray = (string[]) null;
      contribution.Properties.TryGetValue<string[]>("installationTargets", out strArray);
      return strArray;
    }

    private T RetrieveProperty<T>(Contribution contribution, string propertyName)
    {
      T obj = default (T);
      contribution.Properties.TryGetValue<T>(propertyName, out obj);
      return obj;
    }

    private bool QueryById(
      IVssRequestContext requestContext,
      string id,
      out string parentName,
      out string parentId)
    {
      bool flag = true;
      parentId = string.Empty;
      parentName = string.Empty;
      IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
      Contribution contribution;
      ProductContributionAssociateData associatedData;
      if (!service.QueryContribution<ProductContributionAssociateData>(requestContext, id, id, out contribution, out associatedData))
      {
        this.UpdateProductNodesAssociatedData(requestContext);
        flag = service.QueryContribution<ProductContributionAssociateData>(requestContext, id, id, out contribution, out associatedData);
      }
      if (contribution != null)
      {
        parentId = contribution.Id;
        parentName = this.RetrieveProperty<string>(contribution, "name");
      }
      return flag;
    }

    private bool IsVersionWithinRange(
      InstallationTarget target,
      Version productMinversionRange,
      Version productMaxVersionRange)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (target.MinInclusive && target.MinVersion <= productMaxVersionRange)
        flag2 = true;
      else if (!target.MinInclusive && target.MinVersion < productMaxVersionRange)
        flag2 = true;
      if (target.MaxInclusive && productMinversionRange <= target.MaxVersion)
        flag1 = true;
      else if (!target.MaxInclusive && productMinversionRange < target.MaxVersion)
        flag1 = true;
      return flag2 & flag1;
    }

    private List<string> GetContributionIdsListByType(
      IVssRequestContext requestContext,
      IContributionServiceWithData contributionService,
      string contributionType)
    {
      return this.GetContributionIdsList(contributionService.QueryContributionsForType(requestContext, contributionType));
    }

    private List<string> GetContributionIdsListByTargetType(
      IVssRequestContext requestContext,
      IContributionServiceWithData contributionService,
      string targetTypeId)
    {
      return this.GetContributionIdsList(contributionService.QueryContributionsForTarget(requestContext, targetTypeId));
    }

    internal List<string> GetContributionIdsList(IEnumerable<Contribution> contributions)
    {
      List<string> contributionIdsList = new List<string>();
      foreach (Contribution contribution in contributions)
        contributionIdsList.Add(contribution.Id);
      return contributionIdsList;
    }

    private void FetchContributionAssociateData(
      IVssRequestContext requestContext,
      IContributionServiceWithData contributionService,
      string contributionNodeId,
      out Contribution contribution,
      out ProductContributionAssociateData contributionsData)
    {
      if (contributionService.QueryContribution<ProductContributionAssociateData>(requestContext, contributionNodeId, contributionNodeId, out contribution, out contributionsData))
        return;
      this.UpdateProductNodesAssociatedData(requestContext);
      contributionService.QueryContribution<ProductContributionAssociateData>(requestContext, contributionNodeId, contributionNodeId, out contribution, out contributionsData);
    }

    private bool IsTargetPresentInContributionNode(
      IVssRequestContext requestContext,
      Contribution contributionNode,
      ProductContributionAssociateData contributionsData,
      InstallationTarget target)
    {
      if (contributionNode != null)
      {
        string[] source = this.ReadInstallationTargetFromProperties(contributionNode);
        if (contributionsData != null && contributionsData.InstallationTargets != null && contributionsData.InstallationTargets.Contains<string>(target.Target, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || source != null && ((IEnumerable<string>) source).Contains<string>(target.Target, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          return true;
      }
      else
        requestContext.Trace(12061085, TraceLevel.Error, "gallery", nameof (IsTargetPresentInContributionNode), "No contribution node found for target {0}", (object) target.Target);
      return false;
    }

    internal void AddInstallationTargetItems(
      ref ProductContributionAssociateData contributionAssociationData,
      IList<string> InstallationTargets)
    {
      foreach (string installationTarget in (IEnumerable<string>) InstallationTargets)
        contributionAssociationData.InstallationTargets.Add(installationTarget);
    }
  }
}
