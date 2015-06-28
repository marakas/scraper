using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;

namespace BasicPageCrawler
{
    class UniqloIndexer : GenericIndexer
    {
        public UniqloIndexer(CrawledPage pageToIndex) : base(pageToIndex)
        {

        }
    
        public override bool getPrice()
        {
            // find the item price
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@itemprop='price']//p");
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

        public override bool getDescription()
        {
            // find the item description
            var imgArticles = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//article[@itemprop='description']");
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

        public override bool getImage()
        {
            // find the item image
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//img[@itemprop='image']");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    if (imgTag.Attributes["itemprop"].Value == "image")
                    {
                        if (imgTag.Attributes["src"].Value != "")
                        {
                            itemImage = imgTag.Attributes["src"].Value;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public override bool getTitle()
        {
            var h1Tags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//h1[@itemprop='name']");
            // find the item name
            if (h1Tags != null)
            {
                foreach (var h1Tag in h1Tags)
                {
                    if (h1Tag.Attributes["itemprop"].Value == "name")
                    {
                        itemName = h1Tag.InnerHtml;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
