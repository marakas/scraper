using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Crawler;
using Abot.Poco;
using System.Net;
using System.Data.SqlClient;

namespace BasicPageCrawler
{
    class Program
    {
        public static SqlConnection myConnection;
    
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

            myConnection = new SqlConnection("user id=marakas;" +
                                       "password=M@rakas69!;server=yzf0vdv9dr.database.windows.net;" +
                                       "Trusted_Connection=False;Encrypt=True;" +
                                       "database=superfashiondb_db; " +
                                       "connection timeout=30");
            try
            {
                myConnection.Open();
                Console.WriteLine("DB OK!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

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
            ClothingItem item = new ClothingItem() ;
            if (indexUniqloPage(crawledPage, ref item))
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3}, {4}, {5}", item.itemName, item.itemPrice, item.itemImage, item.itemDescription, item.shopName, item.url);
                insertDB(item);
                return true;
            }
            if (indexHMPage(crawledPage, ref item))
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3}, {4}, {5}", item.itemName, item.itemPrice, item.itemImage, item.itemDescription, item.shopName, item.url);
                insertDB(item);
                return true;
            }
            if (indexZaraPage(crawledPage, ref item))
            {
                Console.WriteLine("Found clothing item : {0}, {1}, {2}, {3}, {4}, {5}", item.itemName, item.itemPrice, item.itemImage, item.itemDescription, item.shopName, item.url);
                insertDB(item);
                return true;
            }

            return false;
        }

        private static bool insertDB(ClothingItem item)
        {
            var cmd = myConnection.CreateCommand();
            cmd.CommandText = @"INSERT into SuperFashionDB.ShopItems (shopitemname, shopitemdescription, shopitemprice, shopitemurl, ShopName, pricerange, shopitemimgurl, shopitemimageurl ) values (@itemName,@itemDescription,@itemPrice,@url,@shopName, 1, @itemImage, @itemImage)";
            cmd.Parameters.AddWithValue("@itemName", item.itemName);
            cmd.Parameters.AddWithValue("@itemDescription", item.itemDescription);
            cmd.Parameters.AddWithValue("@itemPrice", item.itemPrice);
            cmd.Parameters.AddWithValue("@url", item.url);
            cmd.Parameters.AddWithValue("@shopName", item.shopName);
            cmd.Parameters.AddWithValue("@itemImage", item.itemImage);
            cmd.ExecuteScalar();
            return true;
        }

        private static bool indexUniqloPage(CrawledPage pageToIndex, ref ClothingItem item )
        {
            var indexer = new UniqloIndexer(pageToIndex);
            Console.WriteLine("aa : {0}", pageToIndex.Uri);
            if (indexer.getTitle() && indexer.getPrice() && indexer.getImage() && indexer.getDescription() )
            {
                item.itemName = indexer.itemName;
                item.itemPrice = indexer.itemPrice;
                item.itemImage = indexer.itemImage;
                item.itemDescription = indexer.itemDescription;
                item.shopName = "UNIQLO";
                item.url = pageToIndex.Uri.ToString();
                return true;
            }
            return false;
        }

        private static bool indexHMPage(CrawledPage pageToIndex, ref ClothingItem item)
        {
            var indexer = new HmIndexer(pageToIndex);

            if (indexer.getTitle() && indexer.getPrice() && indexer.getImage() && indexer.getDescription())
            {
                item.itemName = indexer.itemName;
                item.itemPrice = indexer.itemPrice;
                item.itemImage = indexer.itemImage;
                item.itemDescription = indexer.itemDescription;
                item.shopName = "H&M";
                item.url = pageToIndex.Uri.ToString();
                return true;
            }
            return false;
        }

        private static bool indexZaraPage(CrawledPage pageToIndex, ref ClothingItem item)
        {
            var indexer = new ZaraIndexer(pageToIndex);

            if (indexer.getTitle() && indexer.getPrice() && indexer.getImage() && indexer.getDescription())
            {
                item.itemName = indexer.itemName;
                item.itemPrice = indexer.itemPrice;
                item.itemImage = indexer.itemImage;
                item.itemDescription = indexer.itemDescription;
                item.shopName = "ZARA";
                item.url = pageToIndex.Uri.ToString();
                return true;
            }
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
   
    }
}
