using Abot.Poco;

namespace BasicPageCrawler
{
    internal class HmIndexer : GenericIndexer
    {
        public HmIndexer(CrawledPage pageToIndex) : base(pageToIndex)
        {
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