using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;

namespace BasicPageCrawler
{
    class GenericIndexer
    {
        internal CrawledPage pageToIndex;
        internal string itemName = "";
        internal string itemDescription = "";
        internal string itemImage = "";
        internal string itemPrice = "";
        internal string itemGender = "";
        internal string itemColor = "";
        internal string itemType = "";
        internal string itemBudget = "";

        public GenericIndexer(CrawledPage pageToIndex)
        {
            this.pageToIndex = pageToIndex;
           
            // gender check
            itemGender = "male";
            if (pageToIndex.Uri.AbsoluteUri.Contains("women") || // uniqlo
                    pageToIndex.Uri.AbsoluteUri.Contains("woman") || // zara
                    pageToIndex.Uri.AbsoluteUri.Contains("ladies")) itemGender = "female"; // h&m
           
        }

        public virtual bool getColor() {
            return false;
        }

        public virtual bool getType()
        {
            return false;
        }

        public virtual bool getBudget()
        {
            return false;
        }

        public virtual bool getPrice() {
            // find the item price
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//span[@id='price']");
            if (priceDivs != null)
            {
                foreach (var priceDiv in priceDivs)
                {
                    itemPrice = priceDiv.LastChild.InnerHtml;
                    return true;
                }
            }
            return false;
        }

        public virtual bool getDescription() {
            // find the item description
            var imgArticles = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//p[@class='description']");
            if (imgArticles != null)
            {
                foreach (var imgArticle in imgArticles)
                {
                    itemDescription = imgArticle.InnerHtml;
                    return true;
                }
            }
            return false;
        }

        public virtual bool getImage() {
            // find the item image
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//img");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    if (imgTag.Attributes["src"].Value != "")
                    {
                        itemImage = imgTag.Attributes["src"].Value;
                        return true;
                    }    
                }
            }
            return false;
        }

        public virtual bool getTitle() {
            var h1Tags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//h1");
            // find the item name
            if (h1Tags != null)
            {
                foreach (var h1Tag in h1Tags)
                {
                    itemName = h1Tag.FirstChild.InnerHtml;
                    return true; 
                }
            }
            return false;
        }
    }
}
