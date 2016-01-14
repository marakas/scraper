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
            // get the first link in any tag that has "current selected" class - the menu item open on this page
            var nodes = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//*[@class='current selected']/a");
            
            if(null == nodes || nodes.Count == 0) { // wasn't found - must be a top-level category
                // do something
                
            }
            itemType = nodes[0].InnerHtml; // get first element
            // zara has a bunch of crap in the text
            itemType = cleanUp(itemType);
            if (itemType == "View all") // need to go up a category to make any sense
            {
                nodes = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//ul[@class='current']/li[@class='current  ']/ul/li/a");
                itemType = cleanUp(nodes[0].InnerHtml);
            }
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
                        itemImage = "http:" + imgTag.Attributes["data-src"].Value;
                        return true;
                    }
                }
            }
            return false;
        }


    }
}
