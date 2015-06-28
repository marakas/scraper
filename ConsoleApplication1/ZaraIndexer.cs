using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;

namespace BasicPageCrawler
{
    class ZaraIndexer : GenericIndexer
    {
        public ZaraIndexer(CrawledPage pageToIndex) : base(pageToIndex)
        {
        }

        public override bool getPrice()
        {
            // find the item price
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//p[@class='price']//span");
            if (priceDivs != null)
            {
                if (priceDivs[0].Attributes["data-price"].Value != "")
                {
                    itemPrice = priceDivs[0].Attributes["data-price"].Value;
                    return true;
                }
            }
            return false;
        }

        public override bool getImage()
        {
            // find the item image
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//img[@id='bigImage']");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    if (imgTag.Attributes["data-src"].Value != "")
                    {
                        itemImage = imgTag.Attributes["data-src"].Value;
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
