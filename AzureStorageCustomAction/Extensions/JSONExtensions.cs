//using Newtonsoft.Json;
using AzureStorageCustomAction.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AzureStorageCustomAction.Extensions
{
    public static class JSONExtensions
    {
        public static string Serialize<T>(this T obj  ) where T:SFTPDelpoymentBlob  {

            if ( obj == null ) { return "{ }"; }
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var strJSON= JsonSerializer.Serialize(obj, options );
            return strJSON;
        
        }
    }
}
