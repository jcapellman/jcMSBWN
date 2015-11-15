using System.Collections.Generic;
using System.Runtime.Serialization;

namespace jcMSBWN.UWP.Objects {
    [DataContract]
    public class WifiNetworkSelectionItem {
        [DataMember]
        public List<string> Networks { get; set; } 
    }
}