using Abot.Poco;

namespace BasicPageCrawler
{
    internal class HmIndexer : GenericIndexer
    {
        public HmIndexer(CrawledPage pageToIndex) : base(pageToIndex)
        {
        }

        public override bool getType()
        {
            var typeLists = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//ul[@class='breadcrumbs']//a");
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
            var colorImgs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//ul[@id='options-articles']//span");
            if (colorImgs != null)
            {

                foreach (var colorImg in colorImgs)
                {
                    itemColor += colorImg.InnerHtml + ", ";
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
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//span[@id='text-price']//span");
            if (priceDivs != null)
            {
                foreach (var priceDiv in priceDivs)
                {
                    itemPrice = priceDiv.InnerHtml;
                    return true;
                }
            }
            return false;
        }

        public override bool getDescription()
        {
            // find the item description
            var imgArticles = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@class='description']//p");
            if (imgArticles != null)
            {
                foreach (var imgArticle in imgArticles)
                {
                    itemDescription += imgArticle.InnerHtml;
                }
                return true;
            }
            return false;
        }

        public override bool getImage()
        {
            // find the item image
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@id='product-image-box']//img[@id='product-image']");
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
    }
}