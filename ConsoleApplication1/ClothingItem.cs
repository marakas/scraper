using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BasicPageCrawler
{
    class ClothingItem
    {
        internal string itemName = "";
        internal string itemDescription = "";
        internal string itemImage = "";
        internal string itemPrice = "";
        internal string shopName = "";
        internal string url = "";
        internal string itemGender = "";
        internal string itemColor = "";
        internal string itemType = "";
        internal string itemStore = "";
        internal string itemFileName = "";

        public bool generateImageFileName(string extension)
        {            
            string filename = FirstCharToUpper(shopName) + FirstCharToUpper(itemGender) + FirstCharToUpper(itemType) + FirstCharToUpper(itemName) + extension; // capitalize each part
            itemFileName = Regex.Replace(filename, @"\s+", ""); // remove all whitespace
            return true; // TODO: fix this for error handling
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }
    }
}
