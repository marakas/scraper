using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using System.Net;

namespace BasicPageCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("howdy!");

            log4net.Config.XmlConfigurator.Configure();
            //PrintDisclaimer();

            //Uri uriToCrawl = GetSiteToCrawl(args);
            //Uri uriToCrawl = new Uri("http://www.uniqlo.com/us/product/women-airism-tank-top-143149.html");
            //Uri uriToCrawl = new Uri("http://www.uniqlo.com/us/women/tops/t-shirts.html");
            //Uri uriToCrawl = new Uri("http://www.hm.com/us/product/77536?article=77536-C");
            //Uri uriToCrawl = new Uri("http://www.hm.com/us/department/LADIES");
            //Uri uriToCrawl = new Uri("http://www.zara.com/us/en/sale/woman/tops/view-all/contrasting-embroidered-top-c732008p2709538.html");
            Uri uriToCrawl = new Uri("http://www.zara.com/us/");
            IWebCrawler crawler;
            crawler = GetDefaultWebCrawler();
            
            //Subscribe to any of these asynchronous events, there are also sychronous versions of each.
            //This is where you process data about specific events of the crawl
            crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            //Start the crawl
            //This is a synchronous call
            CrawlResult result = crawler.Crawl(uriToCrawl);

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static Uri GetSiteToCrawl(string[] args)
        {
            string userInput = "";
            if (args.Length < 1)
            {
                System.Console.WriteLine("Please enter ABSOLUTE url to crawl:");
                userInput = System.Console.ReadLine();
            }
            else
            {
                userInput = args[0];
            }

            if (string.IsNullOrWhiteSpace(userInput))
                throw new ApplicationException("Site url to crawl is as a required parameter");

            return new Uri(userInput);
        }

        private static IWebCrawler GetDefaultWebCrawler()
        {
            return new PoliteWebCrawler();
        }

        static void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            //Process data
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
            else
            {
                indexUniqloPage(crawledPage);
                indexHMPage(crawledPage);
                indexZaraPage(crawledPage);
            }
                indexUniqloPage(crawledPage);
        }

        static void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        static void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        static void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
   
        private static void indexUniqloPage(CrawledPage pageToIndex)
        {
            var h1Tags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//h1[@itemprop='name']");
            // find the item name
            var itemName = "";

            if (h1Tags != null)
            {
                foreach (var h1Tag in h1Tags)
                {
                    if (h1Tag.Attributes["itemprop"].Value == "name") itemName = h1Tag.InnerHtml;
                    //Console.WriteLine("tags {0} ", val);
                }
            }
            //Console.WriteLine("Item name {0} ", itemName);

            // find the item image
            var itemImage = "";
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//img[@itemprop='image']");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    if (imgTag.Attributes["itemprop"].Value == "image")
                    {
                        if (imgTag.Attributes["src"].Value != "")
                            itemImage = imgTag.Attributes["src"].Value;
                    }
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item image {0} ", itemImage);

            // find the item description
            var itemDescription = "";
            var imgArticles = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//article[@itemprop='description']");
            if (imgArticles != null)
            {
                foreach (var imgArticle in imgArticles)
                {
                    if (imgArticle.Attributes["itemprop"].Value == "description")
                    {
                        itemDescription = imgArticle.InnerHtml;
                    }
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item description {0} ", itemDescription);

            // find the item price
            var itemPrice = "";
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@itemprop='price']//p");
            if (priceDivs != null)
            {
                foreach (var priceDiv in priceDivs)
                {
                    itemPrice = priceDiv.InnerHtml;
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item price {0} ", itemPrice);

            if (itemImage != "" && itemDescription != "" && itemName != "" && itemPrice != "")
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3} ", itemName, itemPrice, itemImage, itemDescription);
            }
            //var aTags = document.DocumentNode.SelectNodes("//a");
            //Console.WriteLine("tags {0} ", pageToIndex.Uri.AbsoluteUri);
        }

        private static void indexHMPage(CrawledPage pageToIndex)
        {
            var h1Tags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//h1");
            // find the item name
            var itemName = "";

            if (h1Tags != null)
            {
                foreach (var h1Tag in h1Tags)
                {
                    itemName = h1Tag.FirstChild.InnerHtml;

                    //h1Tag.ChildNodes.Remove(h1Tag.LastChild);
                    //sitemName = h1Tag.InnerHtml;
                    //Console.WriteLine("tags {0} ", val);
                }
            }
            //Console.WriteLine("Item name {0} ", itemName);

            // find the item image
            var itemImage = "";
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@id='product-image-box']//img[@id='product-image']");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    if (imgTag.Attributes["src"].Value != "")
                        itemImage = imgTag.Attributes["src"].Value;
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item image {0} ", itemImage);

            // find the item description
            var itemDescription = "";
            var imgArticles = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//div[@class='description']//p");
            if (imgArticles != null)
            {
                foreach (var imgArticle in imgArticles)
                {
                    itemDescription += imgArticle.InnerHtml;
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item description {0} ", itemDescription);

            // find the item price
            var itemPrice = "";
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//span[@id='text-price']//span");
            if (priceDivs != null)
            {
                foreach (var priceDiv in priceDivs)
                {
                    itemPrice = priceDiv.InnerHtml;
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item price {0} ", itemPrice);

            if (itemImage != "" && itemDescription != "" && itemName != "" && itemPrice != "")
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3} ", itemName, itemPrice, itemImage, itemDescription);
            }
            //var aTags = document.DocumentNode.SelectNodes("//a");
            //Console.WriteLine("tags {0} ", pageToIndex.Uri.AbsoluteUri);
        }

        private static void indexZaraPage(CrawledPage pageToIndex)
        {
            var h1Tags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//header//h1");
            // find the item name
            var itemName = "";

            if (h1Tags != null)
            {
                foreach (var h1Tag in h1Tags)
                {
                    itemName = h1Tag.FirstChild.InnerHtml;

                    //h1Tag.ChildNodes.Remove(h1Tag.LastChild);
                    //sitemName = h1Tag.InnerHtml;
                    //Console.WriteLine("tags {0} ", val);
                }
            }
            //Console.WriteLine("Item name {0} ", itemName);

            // find the item image
            var itemImage = "";
            var imgTags = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//img[@id='bigImage']");
            if (imgTags != null)
            {
                foreach (var imgTag in imgTags)
                {
                    if (imgTag.Attributes["data-src"].Value != "")
                        itemImage = imgTag.Attributes["data-src"].Value;
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item image {0} ", itemImage);

            // find the item description
            var itemDescription = "";
            var imgArticles = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//p[@class='description']");
            if (imgArticles != null)
            {
                foreach (var imgArticle in imgArticles)
                {
                    itemDescription += imgArticle.InnerHtml;
                    //Console.WriteLine("img {0} ", imgTag);
                }
            }
            //Console.WriteLine("Item description {0} ", itemDescription);

            // find the item price
            var itemPrice = "";
            var priceDivs = pageToIndex.HtmlDocument.DocumentNode.SelectNodes("//p[@class='price']//span");
            //"<span class=\"sale\" data-price=\"25.99  USD\">\r\n\t\t\t\t\t&nbsp;\r\n\t\t\t\t</span>"
            if (priceDivs != null)
            {
                if (priceDivs[0].Attributes["data-price"].Value != "")
                    itemPrice = priceDivs[0].Attributes["data-price"].Value;
                /*
                foreach (var priceDiv in priceDivs)
                {
                    itemPrice = priceDiv.FirstChild.InnerHtml;
                    //Console.WriteLine("img {0} ", imgTag);
                }
                */
            }
            //Console.WriteLine("Item price {0} ", itemPrice);

            if (itemImage != "" && itemDescription != "" && itemName != "" && itemPrice != "")
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3} ", itemName, itemPrice, itemImage, itemDescription);
            }
            //var aTags = document.DocumentNode.SelectNodes("//a");
            //Console.WriteLine("tags {0} ", pageToIndex.Uri.AbsoluteUri);
        }
    }
}
