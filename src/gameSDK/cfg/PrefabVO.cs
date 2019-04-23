namespace gameSDK
{
    public class PrefabVO
    {
        public string fileName;
        public string keys="";

        public PrefabVO()
        {

        }

        public string prefabPath;

        public bool hasFBX;
        public string fbxPath;
        public string rootFolder;
        public string rootKey;

        public string getDisplayName()
        {
            if (string.IsNullOrEmpty(keys))
            {
                return fileName;
            }

            return fileName;
        }

        public override string ToString()
        {
            return this.fileName;
        }
    }
}