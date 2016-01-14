using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;   


namespace BasicPageCrawler
{
    class ImageDownloader
    {
        internal string filePath = @"c:\temp\";

        public ImageDownloader()
        {
            
        }

        public bool DownloadRemoteImageFile(string uri, string fileName)
        {
            if (doDownload(uri, fileName))
            {
                return (doUpload(fileName));
            }
            else return false;
        }

        // upload file from local to blob storage
        public bool doUpload(string fileName)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=portalvhds2s7d1dnvsbrt5;AccountKey=O1yZhuCxxsbP0T1eypXmZUe2Hz5DaQThzf/6IhGt/Gcns/+J3NIzc6MTSJAfhU0tXQtl7O908axwkHVCqIr4EQ==");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("images");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(filePath + fileName))
            {
                blockBlob.UploadFromStream(fileStream);
            }
            return false;
        }

        // download file from web to local
        public bool doDownload(string uri, string fileName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            
            if ((response.StatusCode == HttpStatusCode.OK ||
                response.StatusCode == HttpStatusCode.Moved ||
                response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download it
                try
                {
                    using (Stream inputStream = response.GetResponseStream())
                    using (Stream outputStream = File.OpenWrite(filePath + fileName))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                            outputStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);
                    }
                }
                catch (Exception) 
                { 
                    // TODO real exception handling 
                }
                return true;
            }
            else
                return false;
        }
    }
}
