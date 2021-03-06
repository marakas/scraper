﻿using System;
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

        public override bool getType()
        {
            var typeLists = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//li[@class='breadcrumb-item']//a");
            if (typeLists != null)
            {
                // get last level of the breadcrumbs for type
                foreach (var typeList in typeLists)
                {
                    itemType = typeList.InnerHtml;
                }
                return true;
            }
            return false;
        }

        public override bool getColor()
        {
            // find the item color group div
            var colorImgs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//ul[@class='color-chips group']//img");
            if (colorImgs != null)
            {
                // Uniqlo has duplicated colors, use dict to get rid 
                Dictionary<string, int> colorDict = new Dictionary<string, int>();

                foreach (var colorImg in colorImgs)
                {
                    try
                    {
                        colorDict.Add(colorImg.Attributes["alt"].Value, 1);
                    } catch (System.ArgumentException) {
                    // dont care if there is already an item with same key
                    }  
                }
                // add multiple colors as single string - deal with this later
                foreach(KeyValuePair<string, int> color in colorDict) {
                    itemColor += color.Key + ", ";
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
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@itemprop='price']//p");
            if (priceDivs != null)
            {
                foreach (var priceDiv in priceDivs)
                {
                    itemPrice = priceDiv.LastChild.InnerHtml;
                    return true;
                }
            }
            var test = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@class='pdp-price-current']");
            // find the item price - second try
            priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@itemprop='price']");
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
                            itemImage = "http:" + imgTag.Attributes["src"].Value;
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
