using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Signer.Settings
{
    public class FileSettings
    {
        public string? DocumentFolder { get; set; }    
        public string? DocumentExtension { get; set; }    
        public string? SignFolder { get; set; }    
        public string? SignExtension { get; set; }    
    }
}