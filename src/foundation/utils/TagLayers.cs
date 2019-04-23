using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class TagX
    {
        public const string Player = "Player";
        public const string MainCamera = "MainCamera";
        public const string GameController = "GameController";

        public const string Npc = "Npc";
        public const string Monster = "Monster";
        public const string Pet = "Pet";

        public static List<string> All = new List<string>()
        {
            Player,MainCamera,GameController,Npc,Monster,Pet
        };
    }

    public class LayerX
    {
        public const string Default = "Default";
        public const string TransparentFX = "TransparentFX";
        public const string IgnoreRaycast = "Ignore Raycast";
        public const string Water = "Water";

        public const string UI = "UI";

        public const string Terrain = "Terrain";
        public const string Player = "Player";
        public const string UI3D = "UI3D";

        public static LayerMask GetDefaultLayer()
        {
            return LayerMask.NameToLayer(LayerX.Default);
        }
        public static LayerMask GetUILayer()
        {
            return LayerMask.NameToLayer(LayerX.UI);
        }
        public static LayerMask GetUI3DLayer()
        {
            LayerMask layer = LayerMask.NameToLayer(LayerX.UI3D);
            if (layer > 100)
            {
                layer = LayerMask.NameToLayer(LayerX.UI);
            }
            return layer;
        }
        public static LayerMask GetPlayerLayer()
        {
            LayerMask layer = LayerMask.NameToLayer(LayerX.Player);
            if (layer > 100)
            {
                layer = LayerMask.NameToLayer(LayerX.Default);
            }
            return layer;
        }

        public static LayerMask GetIgnoreRaycastLayer()
        {
            LayerMask layer = LayerMask.NameToLayer(LayerX.IgnoreRaycast);
            if (layer > 100)
            {
                layer = LayerMask.NameToLayer(LayerX.Default);
            }
            return layer;
        }

        public static LayerMask GetTransparentFXLayer()
        {
            LayerMask layer = LayerMask.NameToLayer(LayerX.TransparentFX);
            if (layer > 100)
            {
                layer = LayerMask.NameToLayer(LayerX.Default);
            }
            return layer;
        }

        public static LayerMask GetTerrainLayer()
        {
            LayerMask layer = LayerMask.NameToLayer(LayerX.Terrain);
            if (layer > 100)
            {
                layer = LayerMask.NameToLayer(LayerX.Default);
            }
            return layer;
        }

        public static LayerMask GetWaterLayer()
        {
            LayerMask layer = LayerMask.NameToLayer(LayerX.Water);
            if (layer > 100)
            {
                layer = LayerMask.NameToLayer(LayerX.Default);
            }
            return layer;
        }


      
    }


    public class SortingLayerX
    {
        //todo;
    }
}