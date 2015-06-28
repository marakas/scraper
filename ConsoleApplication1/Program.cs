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
            Uri uriToCrawl = new Uri("http://www.zara.com/us/en/sale/woman/tops/view-all/contrasting-embroidered-top-c732008p2709538.html");
            //Uri uriToCrawl = new Uri("http://www.zara.com/us/");
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
                var success = indexPages(crawledPage);
            }
        }

        private static bool indexPages(CrawledPage crawledPage)
        {
            if (indexUniqloPage(crawledPage)) return true;
            if (indexHMPage(crawledPage)) return true;
            if (indexZaraPage(crawledPage)) return true;
            return false;
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
   
        
        private static bool indexUniqloPage(CrawledPage pageToIndex)
        {
            var uniqloIndexer = new UniqloIndexer(pageToIndex);
            
            if (uniqloIndexer.getTitle() && uniqloIndexer.getPrice() && uniqloIndexer.getImage() && uniqloIndexer.getDescription() )
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3} ", uniqloIndexer.itemName, uniqloIndexer.itemPrice, uniqloIndexer.itemImage, uniqloIndexer.itemDescription);
                return true;
            }
            return false;
        }

        private static bool indexHMPage(CrawledPage pageToIndex)
        {
            var hmIndexer = new HmIndexer(pageToIndex);

            if (hmIndexer.getTitle() && hmIndexer.getPrice() && hmIndexer.getImage() && hmIndexer.getDescription())
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3} ", hmIndexer.itemName, hmIndexer.itemPrice, hmIndexer.itemImage, hmIndexer.itemDescription);
                return true;
            }
            return false;
        }

        private static bool indexZaraPage(CrawledPage pageToIndex)
        {
            var zaraIndexer = new ZaraIndexer(pageToIndex);

            if (zaraIndexer.getTitle() && zaraIndexer.getPrice() && zaraIndexer.getImage() && zaraIndexer.getDescription())
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3} ", zaraIndexer.itemName, zaraIndexer.itemPrice, zaraIndexer.itemImage, zaraIndexer.itemDescription);
                return true;
            }
            return false;
        }
    }
}
