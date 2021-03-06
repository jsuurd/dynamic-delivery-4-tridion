﻿using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

using Tridion.ContentDelivery.DynamicContent;
using Tridion.ContentDelivery.DynamicContent.Query;
using Query = Tridion.ContentDelivery.DynamicContent.Query.Query;
using Tridion.ContentDelivery.Meta;
using Tridion.ContentDelivery.Web.Linking;

using DD4T.ContentModel;
using DD4T.ContentModel.Exceptions;
using DD4T.ContentModel.Factories;
//using DD4T.Utils;
using System.Collections.Generic;

using System.Web.Caching;
using System.Web;
using DD4T.ContentModel.Contracts.Providers;
using System.Collections;
using System.Configuration;
using DD4T.ContentModel.Querying;
using DD4T.Utils;

namespace DD4T.Providers.SDLTridion2009
{
    /// <summary>
    /// 
    /// </summary>
    public class TridionComponentProvider : BaseProvider, IComponentProvider
    {
        private string selectByComponentTemplateId;
        private string selectByOutputFormat;
        public TridionComponentProvider()
        {
            selectByComponentTemplateId = ConfigurationHelper.SelectComponentByComponentTemplateId;
            selectByOutputFormat = ConfigurationHelper.SelectComponentByOutputFormat;
        }

        public string GetContent(string uri, string templateUri = "")
        {
            
            TcmUri tcmUri = new TcmUri(uri);
            TcmUri templateTcmUri = new TcmUri(templateUri);
            Tridion.ContentDelivery.DynamicContent.ComponentPresentationFactory cpFactory = new ComponentPresentationFactory(PublicationId);
            Tridion.ContentDelivery.DynamicContent.ComponentPresentation cp = null;


            if (!String.IsNullOrEmpty(templateUri))
            {
                cp = cpFactory.GetComponentPresentation(tcmUri.ItemId, templateTcmUri.ItemId);
                if (cp != null)
                    return cp.Content;
            }

            if (!string.IsNullOrEmpty(selectByComponentTemplateId))
            {
                cp = cpFactory.GetComponentPresentation(tcmUri.ItemId, Convert.ToInt32(selectByComponentTemplateId));
                if (cp != null)
                    return cp.Content;
            }
            if (!string.IsNullOrEmpty(selectByOutputFormat))
            {
                cp = cpFactory.GetComponentPresentationWithOutputFormat(tcmUri.ItemId, selectByOutputFormat);
                if (cp != null)
                    return cp.Content;
            }
            IList cps = cpFactory.FindAllComponentPresentations(tcmUri.ItemId);

            foreach (Tridion.ContentDelivery.DynamicContent.ComponentPresentation _cp in cps)
            {
                if (_cp != null)
                {
                    if (_cp.Content.Contains("<Component"))
                    {
                        return _cp.Content;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the Component contents which could be found. Components that couldn't be found don't appear in the list. 
        /// </summary>
        /// <param name="componentUris"></param>
        /// <returns></returns>
        public List<string> GetContentMultiple(string[] componentUris)
        {
            TcmUri uri = new TcmUri(componentUris.First());
            ComponentPresentationFactory cpFactory = new ComponentPresentationFactory(uri.PublicationId);
            var components =
                componentUris
                .Select(componentUri => (Tridion.ContentDelivery.DynamicContent.ComponentPresentation)cpFactory.FindAllComponentPresentations(componentUri)[0])
                .Where(cp => cp != null)
                .Select(cp => cp.Content)
                .ToList();

            return components;

        }

        public IList<string> FindComponents(IQuery queryParameters)
        {
            throw new NotImplementedException();
        }


        public DateTime GetLastPublishedDate(string uri)
        {
            throw new NotImplementedException();
        }
    }
}
