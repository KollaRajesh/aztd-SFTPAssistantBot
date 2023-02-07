using System;
using System.Collections.Generic;
using System.Text;

namespace AzureStorageCustomAction.Extensions
{
    public static class StringExtensions
    {

       public static string GetUniqueDeploymentFileName(this string partnerName)
        {
            var hashcode = Guid.NewGuid().GetHashCode();
            hashcode = hashcode > 0 ? hashcode : hashcode * -1;
            var datetime = DateTime.Now.ToString("MMddyyyyHHmmss");
            return $"{partnerName}-Deployment-{datetime}-{hashcode}.json";
        }

        public static string GetUniqueKeyFileName(this string environmentName, string keyName)
        {
            var fileExtension = string.Empty;
            var fileName = keyName;
            if (keyName.Contains("."))
            {
                var partOfKeyFileName = keyName.Split(".");
                fileExtension = partOfKeyFileName[partOfKeyFileName.Length-1];
                fileName = keyName.Remove(0, fileExtension.Length);
             }

            var hashcode = Guid.NewGuid().GetHashCode();
            hashcode = hashcode > 0 ? hashcode : hashcode * -1;
            var datetime = DateTime.Now.ToString("MMddyyyyHHmmss");
            return $"{environmentName}-{keyName}-{datetime}-{hashcode}.{fileExtension}";
        }
    }
}
