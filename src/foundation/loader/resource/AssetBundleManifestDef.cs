using UnityEngine;

namespace foundation
{
    public class AssetBundleManifestDef
    {
        public string manifesKey = "";
        public string manifesPrefix="";

        public AssetBundleManifest manifest;


        public override string ToString()
        {
            return manifesPrefix;
        }
    }

    public class Hash128Link
    {
        public Hash128 hash128;
        public AssetBundleManifestDef manifestDef;
        public string key;
    }
}