using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace BasicPageCrawler
{
    class ZaraIndexer : GenericIndexer
    {
        public ZaraIndexer(CrawledPage pageToIndex) : base(pageToIndex)
        {
            
        }

        public override bool getType()
        {

            foreach (HtmlNode node in pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//li[@class='current ']")) // NEED THE SPACE!
            {
                foreach (HtmlNode node2 in node.SelectNodes("a"))
                {
                    itemType = node2.InnerText;
                }
            }
            // zara has a bunch of crap in the text
            itemType = Regex.Replace(itemType, @"\r\n?|\t", "").Trim();
            if (itemType != "") return true;
            return false;
        }

        public override bool getColor()
        {
            // find the item color group div
            var colorImgs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@class='imgCont']");
            if (colorImgs != null)
            {
               
                foreach (var colorImg in colorImgs)
                {
                    itemColor += colorImg.Attributes["title"].Value + ", ";
                }
                
                // remove the last ,
                itemColor = itemColor.Remove(itemColor.Length - 2);
                return true;
            }
            return false;
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
